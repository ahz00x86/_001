using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HiveAPI.CS;

namespace probaturas
{
    public class MessageSignerVerifier
    {
        private readonly ECDsaSigner signer = new ECDsaSigner();

        public bool Verify(SignedMessage message)
        {
            var r = new byte[32];
            var s = new byte[32];
            Buffer.BlockCopy(message.SignatureBytes, 1, r, 0, 32);
            Buffer.BlockCopy(message.SignatureBytes, 33, s, 0, 32);
            var recId = message.SignatureBytes[0] - 27;

            var magicData = ("Bitcoin Signed Message:\n").GetVarString();
            var messageData = message.Message.GetVarString();

            var data = new byte[magicData.Length + messageData.Length];
            Buffer.BlockCopy(magicData, 0, data, 0, magicData.Length);
            Buffer.BlockCopy(messageData, 0, data, magicData.Length, messageData.Length);

            var hash = SHA256.DoubleHash(data);

            var point = signer.RecoverFromSignature(hash, r.ToBigIntegerUnsigned(true), s.ToBigIntegerUnsigned(true), recId);

            var pubKeyHash = Hash160.Hash(point.EncodePoint(false));
            var addressBytes = new byte[pubKeyHash.Length + 1];
            Buffer.BlockCopy(pubKeyHash, 0, addressBytes, 1, pubKeyHash.Length);

            var address = Base58.EncodeWithCheckSum(addressBytes);

            if (address == message.Address)
                return true;
            return false;
        }

        public SignedMessage Sign(BigInteger privateKey, string message)
        {
            var magicData = ("Bitcoin Signed Message:\n").GetVarString();
            var messageData = message.GetVarString();

            var data = new byte[magicData.Length + messageData.Length];
            Buffer.BlockCopy(magicData, 0, data, 0, magicData.Length);
            Buffer.BlockCopy(messageData, 0, data, magicData.Length, messageData.Length);

            var hash = SHA256.DoubleHash(data);
            var signature = signer.GenerateSignature(privateKey, hash);

            var recId = -1;
            var publicKey = Secp256k1.G.Multiply(privateKey);

            for (var i = 0; i < 4; i++)
            {
                var Q = signer.RecoverFromSignature(hash, signature[0], signature[1], i);

                if (Q.X == publicKey.X && Q.Y == publicKey.Y)
                {
                    recId = i;
                    break;
                }
            }
            if (recId == -1) throw new Exception("Did not find proper recid");

            var signatureBytes = new byte[65];

            signatureBytes[0] = (byte)(27 + recId);
            var rByteArray = signature[0].ToByteArrayUnsigned(true);
            var sByteArray = signature[1].ToByteArrayUnsigned(true);

            Buffer.BlockCopy(rByteArray, 0, signatureBytes, 1 + (32 - rByteArray.Length), rByteArray.Length);
            Buffer.BlockCopy(sByteArray, 0, signatureBytes, 33 + (32 - sByteArray.Length), sByteArray.Length);

            var signedMessage = new SignedMessage(message, publicKey.GetBitcoinAddress(), signatureBytes);

            return signedMessage;
        }
    }

    public class ECElGamal
    {
        private RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        public ECPoint GenerateKey(ECPoint publicKey, out byte[] key)
        {
            return GenerateKey(publicKey, out key, null);
        }

        public ECPoint GenerateKey(ECPoint publicKey, out byte[] key, BigInteger? k)
        {
            for (int i = 0; i < 100; i++)
            {
                if (k == null)
                {
                    byte[] kBytes = new byte[33];
                    rngCsp.GetBytes(kBytes);
                    kBytes[32] = 0;

                    k = new BigInteger(kBytes);
                }

                if (k.Value.IsZero || k.Value >= Secp256k1.N) continue;

                var tag = Secp256k1.G.Multiply(k.Value);
                var keyPoint = publicKey.Multiply(k.Value);

                if (keyPoint.IsInfinity || tag.IsInfinity) continue;

                key = SHA256.DoubleHash(keyPoint.EncodePoint(false));

                return tag;
            }

            throw new Exception("Unable to generate key");
        }

        public byte[] DecipherKey(BigInteger privateKey, ECPoint tag)
        {
            var keyPoint = tag.Multiply(privateKey);
            var key = SHA256.DoubleHash(keyPoint.EncodePoint(false));

            return key;
        }
    }

    public class ECEncryption
    {
        private ECElGamal ecElGamal = new ECElGamal();
        private RijndaelManaged aesEncryption = new RijndaelManaged();
        private RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        public ECEncryption()
        {
            aesEncryption.KeySize = 256;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
        }

        public byte[] Encrypt(ECPoint publicKey, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            return Encrypt(publicKey, data);
        }

        public byte[] Encrypt(ECPoint publicKey, byte[] data)
        {
            byte[] key;
            var tag = ecElGamal.GenerateKey(publicKey, out key);
            var tagBytes = tag.EncodePoint(false);

            byte[] iv = new byte[16];
            rngCsp.GetBytes(iv);

            aesEncryption.IV = iv;

            aesEncryption.Key = key;

            ICryptoTransform crypto = aesEncryption.CreateEncryptor();

            byte[] cipherData = crypto.TransformFinalBlock(data, 0, data.Length);

            byte[] cipher = new byte[cipherData.Length + 65 + 16];

            Buffer.BlockCopy(tagBytes, 0, cipher, 0, 65);
            Buffer.BlockCopy(aesEncryption.IV, 0, cipher, 65, 16);
            Buffer.BlockCopy(cipherData, 0, cipher, 65 + 16, cipherData.Length);

            return cipher;
        }

        public byte[] Decrypt(BigInteger privateKey, byte[] cipherData)
        {
            byte[] tagBytes = new byte[65];
            Buffer.BlockCopy(cipherData, 0, tagBytes, 0, tagBytes.Length);
            var keyPoint = ECPoint.DecodePoint(tagBytes);

            byte[] iv = new byte[16];
            Buffer.BlockCopy(cipherData, 65, iv, 0, iv.Length);

            byte[] cipher = new byte[cipherData.Length - 16 - 65];
            Buffer.BlockCopy(cipherData, 65 + 16, cipher, 0, cipher.Length);

            byte[] key = ecElGamal.DecipherKey(privateKey, keyPoint);

            aesEncryption.IV = iv;
            aesEncryption.Key = key;

            ICryptoTransform decryptor = aesEncryption.CreateDecryptor();

            byte[] decryptedData = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return decryptedData;
        }
    }

    public class ECDsaSigner
    {
        private RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        public ECPoint RecoverFromSignature(byte[] hash, BigInteger r, BigInteger s, int recId)
        {
            var x = r;
            if (recId > 1 && recId < 4)
            {
                x += Secp256k1.N;
                x = x % Secp256k1.P;
            }

            if (x >= Secp256k1.P)
            {
                return null;
            }

            byte[] xBytes = x.ToByteArrayUnsigned(true);
            byte[] compressedPoint = new Byte[33];
            compressedPoint[0] = (byte)(0x02 + (recId % 2));
            Buffer.BlockCopy(xBytes, 0, compressedPoint, 33 - xBytes.Length, xBytes.Length);

            ECPoint publicKey = ECPoint.DecodePoint(compressedPoint);

            if (!publicKey.Multiply(Secp256k1.N).IsInfinity) return null;

            var z = -hash.ToBigIntegerUnsigned(true) % Secp256k1.N;
            if (z < 0)
            {
                z += Secp256k1.N;
            }

            var rr = r.ModInverse(Secp256k1.N);
            var u1 = (z * rr) % Secp256k1.N;
            var u2 = (s * rr) % Secp256k1.N;

            var Q = Secp256k1.G.Multiply(u1).Add(publicKey.Multiply(u2));

            return Q;
        }

        public BigInteger[] GenerateSignature(BigInteger privateKey, byte[] hash)
        {
            return GenerateSignature(privateKey, hash, null);
        }

        public BigInteger[] GenerateSignature(BigInteger privateKey, byte[] hash, BigInteger? k)
        {
            for (int i = 0; i < 100; i++)
            {
                if (k == null)
                {
                    byte[] kBytes = new byte[33];
                    rngCsp.GetBytes(kBytes);
                    kBytes[32] = 0;

                    k = new BigInteger(kBytes);
                }
                var z = hash.ToBigIntegerUnsigned(true);

                if (k.Value.IsZero || k >= Secp256k1.N) continue;

                var r = Secp256k1.G.Multiply(k.Value).X % Secp256k1.N;

                if (r.IsZero) continue;

                var ss = (z + r * privateKey);
                var s = (ss * (k.Value.ModInverse(Secp256k1.N))) % Secp256k1.N;

                if (s.IsZero) continue;

                return new BigInteger[] { r, s };
            }

            throw new Exception("Unable to generate signature");
        }

        public bool VerifySignature(ECPoint publicKey, byte[] hash, BigInteger r, BigInteger s)
        {
            if (r >= Secp256k1.N || r.IsZero || s >= Secp256k1.N || s.IsZero)
            {
                return false;
            }

            var z = hash.ToBigIntegerUnsigned(true);
            var w = s.ModInverse(Secp256k1.N);
            var u1 = (z * w) % Secp256k1.N;
            var u2 = (r * w) % Secp256k1.N;
            var pt = Secp256k1.G.Multiply(u1).Add(publicKey.Multiply(u2));
            var pmod = pt.X % Secp256k1.N;

            return pmod == r;
        }
    }

    public class ECPoint : ICloneable
    {
        private readonly bool _isInfinity;
        private readonly BigInteger _x;
        private BigInteger _y;

        public ECPoint(BigInteger x, BigInteger y) : this(x, y, false)
        {
        }

        public ECPoint(BigInteger x, BigInteger y, bool isInfinity)
        {
            _x = x;
            _y = y;
            _isInfinity = isInfinity;
        }

        private ECPoint()
        {
            _isInfinity = true;
        }

        public BigInteger X
        {
            get { return _x; }
        }

        public BigInteger Y
        {
            get { return _y; }
        }

        public static ECPoint Infinity
        {
            get { return new ECPoint(); }
        }

        public bool IsInfinity
        {
            get { return _isInfinity; }
        }

        public object Clone()
        {
            return new ECPoint(_x, _y, _isInfinity);
        }

        //TODO: Rename to Encode (point is implied)
        public byte[] EncodePoint(bool compressed)
        {
            if (IsInfinity)
                return new byte[1];

            byte[] x = X.ToByteArrayUnsigned(true);
            byte[] encoded;
            if (!compressed)
            {
                byte[] y = Y.ToByteArrayUnsigned(true);
                encoded = new byte[65];
                encoded[0] = 0x04;
                Buffer.BlockCopy(y, 0, encoded, 33 + (32 - y.Length), y.Length);
            }
            else
            {
                encoded = new byte[33];
                encoded[0] = (byte)(Y.TestBit(0) ? 0x03 : 0x02);
            }

            Buffer.BlockCopy(x, 0, encoded, 1 + (32 - x.Length), x.Length);
            return encoded;
        }

        //TODO: Rename to Decode (point is implied)
        public static ECPoint DecodePoint(byte[] encoded)
        {
            if (encoded == null || ((encoded.Length != 33 && encoded[0] != 0x02 && encoded[0] != 0x03) && (encoded.Length != 65 && encoded[0] != 0x04)))
                throw new FormatException("Invalid encoded point");

            var unsigned = new byte[32];
            Buffer.BlockCopy(encoded, 1, unsigned, 0, 32);
            BigInteger x = unsigned.ToBigIntegerUnsigned(true);
            BigInteger y;
            byte prefix = encoded[0];

            if (prefix == 0x04) //uncompressed PubKey
            {
                Buffer.BlockCopy(encoded, 33, unsigned, 0, 32);
                y = unsigned.ToBigIntegerUnsigned(true);
            }
            else // compressed PubKey
            {
                // solve y
                y = ((x * x * x + 7) % Secp256k1.P).ShanksSqrt(Secp256k1.P);

                if (y.IsEven ^ prefix == 0x02) // negate y for prefix (0x02 indicates y is even, 0x03 indicates y is odd)
                    y = -y + Secp256k1.P;      // TODO:  DRY replace this and body of Negate() with call to static method
            }
            return new ECPoint(x, y);
        }

        public ECPoint Negate()
        {
            var r = (ECPoint)Clone();
            r._y = -r._y + Secp256k1.P;
            return r;
        }

        public ECPoint Subtract(ECPoint b)
        {
            return Add(b.Negate());
        }

        public ECPoint Add(ECPoint b)
        {
            BigInteger m;
            //[Resharper unused local variable] BigInteger r = 0;

            if (IsInfinity)
                return b;
            if (b.IsInfinity)
                return this;

            if (X - b.X == 0)
            {
                if (Y - b.Y == 0)
                    m = 3 * X * X * (2 * Y).ModInverse(Secp256k1.P);
                else
                    return Infinity;
            }
            else
            {
                var mx = (X - b.X);
                if (mx < 0)
                    mx += Secp256k1.P;
                m = (Y - b.Y) * mx.ModInverse(Secp256k1.P);
            }

            m = m % Secp256k1.P;

            var v = Y - m * X;
            var x3 = (m * m - X - b.X);
            x3 = x3 % Secp256k1.P;
            if (x3 < 0)
                x3 += Secp256k1.P;
            var y3 = -(m * x3 + v);
            y3 = y3 % Secp256k1.P;
            if (y3 < 0)
                y3 += Secp256k1.P;

            return new ECPoint(x3, y3);
        }

        public ECPoint Twice()
        {
            return Add(this);
        }

        public ECPoint Multiply(BigInteger b)
        {
            if (b.Sign == -1)
                throw new FormatException("The multiplicator cannot be negative");

            b = b % Secp256k1.N;

            ECPoint result = Infinity;
            ECPoint temp = null;

            //[Resharper local variable only assigned not used] int bit = 0;
            do
            {
                temp = temp == null ? this : temp.Twice();

                if (!b.IsEven)
                    result = result.Add(temp);
                //bit++;
            }
            while ((b >>= 1) != 0);

            return result;
        }
    }

    /// <summary>
    /// Modified from CodesInChaos' public domain code
    /// 
    /// https://gist.github.com/CodesInChaos/3175971
    /// </summary>
    public static class Base58
    {
        public const int CheckSumSizeInBytes = 4;

        private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static string EncodeWithCheckSum(byte[] data)
        {
            return Encode(AddCheckSum(data));
        }

        public static byte[] RemoveCheckSum(byte[] data)
        {
            byte[] result = new byte[data.Length - CheckSumSizeInBytes];
            Buffer.BlockCopy(data, 0, result, 0, data.Length - CheckSumSizeInBytes);

            return result;
        }

        public static bool VerifyCheckSum(byte[] data)
        {
            byte[] result = new byte[data.Length - CheckSumSizeInBytes];
            Buffer.BlockCopy(data, 0, result, 0, data.Length - CheckSumSizeInBytes);
            byte[] correctCheckSum = GetCheckSum(result);
            for (int i = CheckSumSizeInBytes; i >= 1; i--)
            {
                if (data[data.Length - i] != correctCheckSum[CheckSumSizeInBytes - i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool DecodeWithCheckSum(string base58, out byte[] decoded)
        {
            var dataWithCheckSum = Decode(base58);
            var success = VerifyCheckSum(dataWithCheckSum);
            decoded = RemoveCheckSum(dataWithCheckSum);

            return success;
        }

        public static string Encode(byte[] data)
        {
            // Decode byte[] to BigInteger
            BigInteger intData = 0;
            for (int i = 0; i < data.Length; i++)
            {
                intData = intData * 256 + data[i];
            }

            // Encode BigInteger to Base58 string
            string result = "";
            while (intData > 0)
            {
                int remainder = (int)(intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (int i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }
            return result;
        }

        public static byte[] Decode(string base58)
        {
            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (int i = 0; i < base58.Length; i++)
            {
                int digit = Digits.IndexOf(base58[i]); //Slow
                if (digit < 0)
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", base58[i], i));
                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            int leadingZeroCount = base58.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
                intData.ToByteArray()
                .Reverse()// to big endian
                .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
            return result;
        }

        public static byte[] AddCheckSum(byte[] data)
        {
            byte[] checkSum = GetCheckSum(data);

            var result = new byte[checkSum.Length + data.Length];
            Buffer.BlockCopy(data, 0, result, 0, data.Length);
            Buffer.BlockCopy(checkSum, 0, result, data.Length, checkSum.Length);
            return result;
        }

        private static byte[] GetCheckSum(byte[] data)
        {
            var hash = SHA256.DoubleHash(data);
            Array.Resize(ref hash, CheckSumSizeInBytes);
            return hash;
        }
    }

    public class RIPEMD160
    {
        private static readonly RIPEMD160Managed ripemd160 = new RIPEMD160Managed();

        public static byte[] Hash(byte[] data)
        {
            return ripemd160.ComputeHash(data);
        }

        public static byte[] Hash(string hexData)
        {
            byte[] bytes = Hex.HexToBytes(hexData);
            return Hash(bytes);
        }
    }

    public class Hash160
    {
        public static byte[] Hash(byte[] data)
        {
            return RIPEMD160.Hash(SHA256.Hash(data));
        }

        public static byte[] Hash(string hexData)
        {
            byte[] bytes = Hex.HexToBytes(hexData);
            return Hash(bytes);
        }
    }

    public static class BigIntExtensions
    {
        public static BigInteger ModInverse(this BigInteger n, BigInteger p)
        {
            BigInteger x = 1;
            BigInteger y = 0;
            BigInteger a = p;
            BigInteger b = n;

            while (b != 0)
            {
                BigInteger t = b;
                BigInteger q = BigInteger.Divide(a, t);
                b = a - q * t;
                a = t;
                t = x;
                x = y - q * t;
                y = t;
            }

            if (y < 0)
                return y + p;
            //else
            return y;
        }

        public static bool TestBit(this BigInteger i, int n)
        {
            //[resharper:unused local variable] int bitLength = i.BitLength();
            return !(i >> n).IsEven;
        }

        public static int BitLength(this BigInteger i)
        {
            int bitLength = 0;
            do
            {
                bitLength++;
            }
            while ((i >>= 1) != 0);
            return bitLength;
        }

        public static byte[] ToByteArrayUnsigned(this BigInteger i, bool bigEndian)
        {
            byte[] bytes = i.ToByteArray();
            if (bytes[bytes.Length - 1] == 0x00)
                Array.Resize(ref bytes, bytes.Length - 1);
            if (bigEndian)
                Array.Reverse(bytes, 0, bytes.Length);

            return bytes;
        }

        public static BigInteger Order(this BigInteger b, BigInteger p)
        {
            BigInteger m = 1;
            BigInteger e = 0;

            while (BigInteger.ModPow(b, m, p) != 1)
            {
                m *= 2;
                e++;
            }

            return e;
        }

        private static BigInteger FindS(BigInteger p)
        {
            BigInteger s = p - 1;
            BigInteger e = 0;

            while (s % 2 == 0)
            {
                s /= 2;
                e += 1;
            }

            return s;
        }

        private static BigInteger FindE(BigInteger p)
        {
            BigInteger s = p - 1;
            BigInteger e = 0;

            while (s % 2 == 0)
            {
                s /= 2;
                e += 1;
            }

            return e;
        }

        private static BigInteger TwoExp(BigInteger e)
        {
            BigInteger a = 1;

            while (e > 0)
            {
                a *= 2;
                e--;
            }

            return a;
        }

        public static string ToHex(this BigInteger b)
        {
            return Hex.BigIntegerToHex(b);
        }

        public static string ToHex(this byte[] bytes)
        {
            return Hex.BytesToHex(bytes);
        }

        public static BigInteger HexToBigInteger(this string s)
        {
            return Hex.HexToBigInteger(s);
        }

        public static byte[] HexToBytes(this string s)
        {
            return Hex.HexToBytes(s);
        }

        public static BigInteger ToBigInteger(this byte[] bytes, bool bigEndian)
        {
            byte[] clone = new byte[bytes.Length];
            Buffer.BlockCopy(bytes, 0, clone, 0, bytes.Length);
            Array.Reverse(clone);

            return new BigInteger(bytes);
        }

        public static BigInteger ToBigIntegerUnsigned(this byte[] bytes, bool bigEndian)
        {
            byte[] clone;
            if (bigEndian)
            {
                if (bytes[0] != 0x00)
                {
                    clone = new byte[bytes.Length + 1];
                    Buffer.BlockCopy(bytes, 0, clone, 1, bytes.Length);
                    Array.Reverse(clone);
                    return new BigInteger(clone);
                }
                clone = new byte[bytes.Length];
                Buffer.BlockCopy(bytes, 0, clone, 0, bytes.Length);
                Array.Reverse(clone);
                return new BigInteger(clone);
            }

            if (bytes[bytes.Length - 1] == 0x00)
                return new BigInteger(bytes);

            clone = new byte[bytes.Length + 1];
            Buffer.BlockCopy(bytes, 0, clone, 0, bytes.Length);
            return new BigInteger(clone);
        }

        public static string GetBitcoinAddress(this ECPoint publicKey, bool compressed = true)
        {
            var pubKeyHash = Hash160.Hash(publicKey.EncodePoint(compressed));

            byte[] addressBytes = new byte[pubKeyHash.Length + 1];
            Buffer.BlockCopy(pubKeyHash, 0, addressBytes, 1, pubKeyHash.Length);
            return Base58.EncodeWithCheckSum(addressBytes);
        }

        public static BigInteger ShanksSqrt(this BigInteger a, BigInteger p)
        {
            if (BigInteger.ModPow(a, (p - 1) / 2, p) == (p - 1))
                return -1;

            if (p % 4 == 3)
                return BigInteger.ModPow(a, (p + 1) / 4, p);

            //Initialize 
            BigInteger s = FindS(p);
            BigInteger e = FindE(p);
            BigInteger n = 2;

            while (BigInteger.ModPow(n, (p - 1) / 2, p) == 1)
                n++;

            BigInteger x = BigInteger.ModPow(a, (s + 1) / 2, p);
            BigInteger b = BigInteger.ModPow(a, s, p);
            BigInteger g = BigInteger.ModPow(n, s, p);
            BigInteger r = e;
            BigInteger m = b.Order(p);

#if(DEBUG)
            Debug.WriteLine("{0}, {1}, {2}, {3}, {4}", m, x, b, g, r);
#endif
            while (m > 0)
            {
                x = (x * BigInteger.ModPow(g, TwoExp(r - m - 1), p)) % p;
                b = (b * BigInteger.ModPow(g, TwoExp(r - m), p)) % p;
                g = BigInteger.ModPow(g, TwoExp(r - m), p);
                r = m;
                m = b.Order(p);

#if(DEBUG)
                Debug.WriteLine("{0}, {1}, {2}, {3}, {4}", m, x, b, g, r);
#endif
            }

            return x;
        }
    }

    public static class VarIntExtensions
    {
        /// <summary>
        ///     Returns a VarInt as a variable length byte array representing the unsigned byte (uint8_t).
        ///     VarInt is a Bitcoin Protocol specific structure used for efficient transmission of integrals over the wire.
        ///     https://en.bitcoin.it/wiki/Protocol_specification#Variable_length_string
        /// </summary>
        /// <param name="value">byte on which the extension method is called</param>
        /// <returns>Byte Array of one to nine bytes containing unsigned integral value in Bitcoin VarInt format</returns>
        public static byte[] GetVarIntBytes(this byte value)
        {
            return new VarInt(value).ToByteArray();
        }

        /// <summary>
        ///     Returns a VarInt as a variable length byte array representing the unsigned short (uint16_t).
        ///     VarInt is a Bitcoin Protocol specific structure used for efficient transmission of integrals over the wire.
        ///     https://en.bitcoin.it/wiki/Protocol_specification#Variable_length_string
        /// </summary>
        /// <param name="value">ushort on which the extension method is called</param>
        /// <returns>Byte Array of one to nine bytes containing unsigned integral value in Bitcoin VarInt format</returns>
        public static byte[] GetVarIntBytes(this ushort value)
        {
            return new VarInt(value).ToByteArray();
        }

        /// <summary>
        ///     Returns a VarInt as a variable length byte array representing the unsigned int (uint32_t).
        ///     VarInt is a Bitcoin Protocol specific structure used for efficient transmission of integrals over the wire.
        ///     https://en.bitcoin.it/wiki/Protocol_specification#Variable_length_string
        /// </summary>
        /// <param name="value">uint on which the extension method is called</param>
        /// <returns>Byte Array of one to nine bytes containing unsigned integral value in Bitcoin VarInt format</returns>
        public static byte[] GetVarIntBytes(this uint value)
        {
            return new VarInt(value).ToByteArray();
        }

        /// <summary>
        ///     Returns a VarInt as a variable length byte array representing the unsigned long (uint64_t).
        ///     VarInt is a Bitcoin Protocol specific structure used for efficient transmission of integrals over the wire.
        ///     https://en.bitcoin.it/wiki/Protocol_specification#Variable_length_string
        /// </summary>
        /// <param name="value">ulong on which the extension method is called</param>
        /// <returns>Byte Array of one to nine bytes containing unsigned integral value in Bitcoin VarInt format</returns>
        public static byte[] GetVarIntBytes(this ulong value)
        {
            return new VarInt(value).ToByteArray();
        }
    }

    public class VarInt
    {
        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        public VarInt(ulong value)
        {
            Value = value;
        }

        public VarInt(long value) : this((ulong)value) { }

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        public VarInt(byte[] buffer)
            : this(buffer, 0) { }

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public VarInt(byte[] buffer, int offset)
        {
            Value = Decode(buffer, offset);
        }

        /// <summary>
        /// </summary>
        public ulong Value { get; private set; }

        /// <summary>
        /// </summary>
        public int SizeInBytes
        {
            get
            {
                if (Value <= 252) //0xfc
                    return 1; //directly stored as a single byte (0x00 to 0xfc)
                if (Value <= ushort.MaxValue)
                    return 3; // 1 byte prefix (0xfd) + value as uint16 (2 bytes)
                if (Value <= uint.MaxValue)
                    return 5; // 1 byte prefix (0xfe) + value as unit32 (4 bytes)
                              //else
                return 9; // 1 byte prefix (0xff) + value as unit64 (8 bytes)
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            byte[] varIntBytes;

            if (Value <= 252) //Values up to 0xfc stored directly without prefix (0xfd, 0xfe, and 0xff in first byte reserved as prefixes)
            {
                varIntBytes = new byte[1];
                // no prefix indicates value stored as byte directly
                varIntBytes[0] = (byte)Value;
            }
            else if (Value <= UInt16.MaxValue)
            {
                varIntBytes = new byte[3];
                varIntBytes[0] = 0xfd; // leading byte prefix indicates uint16 follows
                BitConverter.GetBytes((ushort)Value).CopyTo(varIntBytes, 1);
            }
            else if (Value <= UInt32.MaxValue)
            {
                varIntBytes = new byte[5];
                varIntBytes[0] = 0xfe; // leading byte prefix indicates uint32 follows
                BitConverter.GetBytes((uint)Value).CopyTo(varIntBytes, 1);
            }
            else
            {
                varIntBytes = new byte[9];
                varIntBytes[0] = 0xff; // leading byte prefix indicates uint64 follows
                BitConverter.GetBytes(Value).CopyTo(varIntBytes, 1);
            }
            return varIntBytes;
        }

        public static ulong Decode(byte[] buffer, int offset = 0)
        {
            switch (buffer[offset]) //check first byte for prefix value (0xfd, 0xfe, or 0xff)
            {
                case 253: //0xfd
                    return BitConverter.ToUInt16(buffer, offset + 1); // value stored as prefix + uint16

                case 254: //0xfe
                    return BitConverter.ToUInt32(buffer, offset + 1); // value stored as prefix + uint32

                case 255: //0xff
                    return BitConverter.ToUInt64(buffer, offset + 1); // value stored as prefix + uint64

                default: //0x00 to 0xfc
                    return buffer[offset]; // value stored as direct byte without a prefix
            }
        }

        public static byte[] Encode(ulong value)
        {
            return new VarInt(value).ToByteArray();
        }
    }

    class VarString
    {
        public string Value { get; private set; }

        public int Length
        {
            get { return Value.Length; }
        }

        public VarInt VarLength
        {
            get { return new VarInt((ulong)Length); }
        }

        public VarString(string str)
        {
            Value = str;
        }

        public VarString(byte[] buff, int offset = 0) : this(Decode(buff, offset)) { }

        public byte[] GetBytes()
        {
            return Encode(Value);
        }

        public static byte[] Encode(string str)
        {
            if (str == null)
                return new byte[] { 0 };

            // strings of more than 2,147,483,647 bytes are not possible due to length property being of type (signed) Int32
            var varIntBytes = ((uint)str.Length).GetVarIntBytes();
            var stringBytes = new byte[varIntBytes.Length + str.Length]; // Make byte array large enough to hold length & string
            varIntBytes.CopyTo(stringBytes, 0); // VarInt of up to 9 bytes
            Encoding.UTF8.GetBytes(str, 0, str.Length, stringBytes, varIntBytes.Length); // string contents
            return stringBytes;
        }

        public static string Decode(byte[] buff, int offset = 0)
        {
            var varLength = new VarInt(buff, offset);

            if (varLength.Value > Int32.MaxValue)
                throw new NotImplementedException("Encoded length of string is greater than Int32.MaxValue");

            return Encoding.UTF8.GetString(buff, offset + varLength.SizeInBytes, (int)varLength.Value);
        }
    }
    public static class Hex
    {
        private static readonly string[] _byteToHex = new[]
        {
            "00", "01", "02", "03", "04", "05", "06", "07",
            "08", "09", "0a", "0b", "0c", "0d", "0e", "0f",
            "10", "11", "12", "13", "14", "15", "16", "17",
            "18", "19", "1a", "1b", "1c", "1d", "1e", "1f",
            "20", "21", "22", "23", "24", "25", "26", "27",
            "28", "29", "2a", "2b", "2c", "2d", "2e", "2f",
            "30", "31", "32", "33", "34", "35", "36", "37",
            "38", "39", "3a", "3b", "3c", "3d", "3e", "3f",
            "40", "41", "42", "43", "44", "45", "46", "47",
            "48", "49", "4a", "4b", "4c", "4d", "4e", "4f",
            "50", "51", "52", "53", "54", "55", "56", "57",
            "58", "59", "5a", "5b", "5c", "5d", "5e", "5f",
            "60", "61", "62", "63", "64", "65", "66", "67",
            "68", "69", "6a", "6b", "6c", "6d", "6e", "6f",
            "70", "71", "72", "73", "74", "75", "76", "77",
            "78", "79", "7a", "7b", "7c", "7d", "7e", "7f",
            "80", "81", "82", "83", "84", "85", "86", "87",
            "88", "89", "8a", "8b", "8c", "8d", "8e", "8f",
            "90", "91", "92", "93", "94", "95", "96", "97",
            "98", "99", "9a", "9b", "9c", "9d", "9e", "9f",
            "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7",
            "a8", "a9", "aa", "ab", "ac", "ad", "ae", "af",
            "b0", "b1", "b2", "b3", "b4", "b5", "b6", "b7",
            "b8", "b9", "ba", "bb", "bc", "bd", "be", "bf",
            "c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7",
            "c8", "c9", "ca", "cb", "cc", "cd", "ce", "cf",
            "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7",
            "d8", "d9", "da", "db", "dc", "dd", "de", "df",
            "e0", "e1", "e2", "e3", "e4", "e5", "e6", "e7",
            "e8", "e9", "ea", "eb", "ec", "ed", "ee", "ef",
            "f0", "f1", "f2", "f3", "f4", "f5", "f6", "f7",
            "f8", "f9", "fa", "fb", "fc", "fd", "fe", "ff"
        };

        private static readonly Dictionary<string, byte> _hexToByte = new Dictionary<string, byte>();

        static Hex()
        {
            for (byte b = 0; b < 255; b++)
            {
                _hexToByte[_byteToHex[b]] = b;
            }

            _hexToByte["ff"] = 255;
        }

        public static string BigIntegerToHex(BigInteger value)
        {
            return BytesToHex(value.ToByteArrayUnsigned(true));
        }

        public static BigInteger HexToBigInteger(string hex)
        {
            byte[] bytes = HexToBytes(hex);
            Array.Reverse(bytes);
            Array.Resize(ref bytes, bytes.Length + 1);
            bytes[bytes.Length - 1] = 0x00;
            return new BigInteger(bytes);
        }

        public static string BytesToHex(byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.Append(_byteToHex[b]);
            }

            return hex.ToString();
        }

        public static byte[] HexToBytes(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                hex = "0" + hex;
            }

            hex = hex.ToLower();

            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length / 2; i++)
            {
                bytes[i] = _hexToByte[hex.Substring(i * 2, 2)];
            }

            return bytes;
        }

        public static string AsciiToHex(string ascii)
        {
            char[] chars = ascii.ToCharArray();
            var hex = new StringBuilder(ascii.Length);

            foreach (var currentChar in chars)
            {
                hex.Append(String.Format("{0:X}", Convert.ToInt32(currentChar)));
            }

            return hex.ToString();
        }
    }

    public class SHA256
    {
        private static readonly SHA256Managed sha256 = new SHA256Managed();

        public static byte[] Hash(byte[] data)
        {
            return sha256.ComputeHash(data);
        }

        public static byte[] DoubleHash(byte[] data)
        {
            return sha256.ComputeHash(sha256.ComputeHash(data));
        }

        public static byte[] DoubleHashCheckSum(byte[] data)
        {
            byte[] checksum = DoubleHash(data);
            Array.Resize(ref checksum, 4);
            return checksum;
        }

        public static byte[] Hash(string hexData)
        {
            byte[] bytes = Hex.HexToBytes(hexData);
            return Hash(bytes);
        }

        public static byte[] DoubleHash(string hexData)
        {
            byte[] bytes = Hex.HexToBytes(hexData);
            return DoubleHash(bytes);
        }

        public static byte[] DoubleHashCheckSum(string hexData)
        {
            byte[] bytes = Hex.HexToBytes(hexData);
            return DoubleHashCheckSum(bytes);
        }
    }

    public static class Secp256k1
    {
        public static readonly BigInteger P = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F".HexToBigInteger();
        public static readonly ECPoint G = ECPoint.DecodePoint("0479BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8".HexToBytes());
        public static readonly BigInteger N = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141".HexToBigInteger();
    }

    public static class VarStringExtensions
    {
        /// <summary>
        ///     Extension Method for string type
        ///     Returns a byte array with string length as a VarInt followed by string contents
        ///     Is a Bitcoin Protocol specific structure
        ///     https://en.bitcoin.it/wiki/Protocol_specification#Variable_length_string
        ///     Limitation: The Bitcoin protocol supports string up to 2^64 -1 (unit64) bytes in length.
        ///     The C# built in type "string" only supports strings up to 2^31-1 (singed int32) bytes in length.
        ///     To strictly comply with Bitcoin spec would require a new class capable of longer strings. Although
        ///     as a practical matter no existing messages support string lengths beyond the Int32 limit.
        /// </summary>
        /// <param name="str">Instance of the string class being extended.</param>
        /// <returns>Byte Array containing the variable length of the string.</returns>
        public static byte[] GetVarString(this string str)
        {
            return VarString.Encode(str);
        }
    }
    public class SignedMessage
    {
        private string _message;

        public string Message
        {
            get { return _message; }
        }

        private string _address;

        public string Address
        {
            get { return _address; }
        }

        private byte[] _signatureBytes;

        public byte[] SignatureBytes
        {
            get { return _signatureBytes; }
        }

        private string _signature = null;

        public string Signature
        {
            get
            {
                if (_signature == null)
                {
                    _signature = Convert.ToBase64String(SignatureBytes);
                }

                return _signature;
            }
        }

        private string _formattedMessage = null;
        public string FormattedMessage
        {
            get
            {
                if (_formattedMessage == null)
                {
                    _formattedMessage = "-----BEGIN BITCOIN SIGNED MESSAGE-----\n";
                    _formattedMessage += Message;
                    _formattedMessage += "\n-----BEGIN SIGNATURE-----\n";
                    _formattedMessage += Address;
                    _formattedMessage += "\n";
                    _formattedMessage += Signature;
                    _formattedMessage += "\n-----END BITCOIN SIGNED MESSAGE-----";
                }

                return _formattedMessage;
            }
        }

        public SignedMessage(string message, string address, byte[] signatureBytes)
        {
            this._address = address;
            this._message = message;
            this._signatureBytes = signatureBytes;
        }
    }

    public static class Extensions
    {
        public static string Slice(this string s,
            int beginIndex, int endIndex)
        {
            if (s == null) return null;

            return beginIndex >= 0 ?
                s.Substring(beginIndex, endIndex) :
                s.Substring(s.Length - beginIndex);
        }

        public static string Slice(this string s,
            int beginIndex = 0)
        {
            if (s == null) return null;

            return Slice(s, beginIndex, s.Length);
        }

        public static Array Slice(this Array a, int beginIndex = 0)
        {
            if (a == null || a.Length == 0) return a;

            return Slice(a, beginIndex, a.Length);
        }

        public static Array Slice(this Array a,
            int beginIndex, int endIndex)
        {
            if (a == null || a.Length == 0) return a;

            var e = Enumerable.Cast<object>(a);

            return beginIndex >= 0 ?
                e.Skip(beginIndex).ToArray() :
                e.Skip(e.Count() - beginIndex).ToArray();
        }
    }

    static class Enc
    {
        public static string EncodeBase64(this string plainText, Encoding encoding = null)
        {
            if (plainText == null) return null;

            // use UTF8 as default encoding type
            encoding = encoding ?? Encoding.UTF8;
            var bytes = encoding.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }
    }
    internal class Program
    {
        //public static string GetSignature(Byte[] privateKey, String message)
        //{
        //    var curve = SecNamedCurves.GetByName("secp256k1");

        //    var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

        //    var keyParameters = new ECPrivateKeyParameters(new Org.BouncyCastle.Math.BigInteger(privateKey), domain);

        //    ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");

        //    signer.Init(true, keyParameters);

        //    signer.BlockUpdate(Encoding.UTF8.GetBytes(message), 0, message.Length);
        //    //signer.BlockUpdate(data, 0, data.Length);
        //    var signature = signer.GenerateSignature();

        //    return Base58.Encode(signature);

        //}

        public static string GetSignature(string privateKey, string message)
        {
            var curve = SecNamedCurves.GetByName("secp256k1");
            var domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);
            var keyParameters = new ECPrivateKeyParameters(new Org.BouncyCastle.Math.BigInteger(privateKey, 16), domain);

            var signer = new Org.BouncyCastle.Crypto.Signers.ECDsaSigner();// new HMacDsaKCalculator(new Sha256Digest()));
            signer.Init(true, keyParameters);

            var messageBytes = ConvertToBytes(message);
            var hash = GetHash(messageBytes); //GetHash(GetHash(messageBytes));
            var signature = signer.GenerateSignature(hash);

            //var signature = signer.GenerateSignature(Encoding.UTF8.GetBytes(message));

            var r = signature[0];
            var s = signature[1];

            var otherS = curve.Curve.Order.Subtract(s);

            if (s.CompareTo(otherS) == 1)
            {
                s = otherS;
            }

            var derSignature = new DerSequence
            (
                new DerInteger(new Org.BouncyCastle.Math.BigInteger(1, r.ToByteArray())),
                new DerInteger(new Org.BouncyCastle.Math.BigInteger(1, s.ToByteArray()))
            )
            .GetDerEncoded();

            return ConvertToString(derSignature);
        }

        public static string ConvertToString(byte[] input)
        {
            return string.Concat(input.Select(x => x.ToString("x2")));
        }

        public static byte[] ConvertToBytes(string input)
        {
            if (input.StartsWith("0x")) input = input.Remove(0, 2);

            return Enumerable.Range(0, input.Length / 2).Select(x => System.Convert.ToByte(input.Substring(x * 2, 2), 16)).ToArray();
        }

        private static byte[] GetHash(byte[] data)
        {
            var digest = new Sha256Digest();
            var hash = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(hash, 0);
            return hash;
        }

        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        static void Main(string[] args)
        {

            HttpClient httpClient = new HttpClient();
            
            CHived hived = new CHived(httpClient, "https://api.hive.blog");

            COperations.custom_json cJson = new COperations.custom_json();
            cJson.id = "ssc-mainnet-hive";
            cJson.required_auths = new string[] { "hk-students" };
            cJson.required_posting_auths = new string[] { };
            cJson.json = "{\"contractName\":\"tokens\",\"contractAction\":\"issue\",\"contractPayload\":{\"symbol\":\"HKLEARN\",\"to\":\"apzyx\",\"quantity\":\"7.000\"}}";

            String s = hived.broadcast_transaction(new object[] { cJson }, new string[] { "5JC241FGfd6yrCbeVmP3hZNsk7rjypPZ8VxFM3Hdg5PUmE8W3pF" });

            String s2 = "";

            //{
            //    "id": "4ebb1a034b8ccbaf21d204f18b32f99e8d255430",
            //    "ref_block_num": 59777,
            //    "ref_block_prefix": 1063802710,
            //    "expiration": "2022-08-15T11:03:15",
            //    "operations": [
            //        [
            //            "custom_json",
            //            {
            //                        "required_auths": [
            //                            "hk-students"
            //                ],
            //                "required_posting_auths": [],
            //                "id": "ssc-mainnet-hive",
            //                "json": "{\"contractName\":\"tokens\",\"contractAction\":\"issue\",\"contractPayload\":{\"symbol\":\"HKLEARN\",\"to\":\"apzyx\",\"quantity\":\"1.000\"}}"
            //            }
            //        ]
            //    ],
            //    "extensions": [],
            //    "signatures": [
            //        "1f6c09e4454e31131ee3e7e1b346e656fdfa94468d0d14c0ae158b0f91b135bded1b511a835bf5753fd093f8ce4fb0a983e797ffffc7083deea69ba9e433aa88a2"
            //    ]
            //}

            ////using (var myStream = new MemoryStream())
            ////{
            ////    var myWriter = new BinaryWriter(myStream);

            ////        myWriter.Write("beeab0de00000000000000000000000000000000000000000000000000000000");
            ////        myWriter.Write((UInt32)59777);
            ////        myWriter.Write((UInt32)1063802710);
            ////        myWriter.Write("2022-08-15T11:03:15");
            ////        myWriter.Write(1);
            ////        myWriter.Write(18); // customjson operation
            ////        myWriter.Write(1);
            ////        myWriter.Write("hk-students");
            ////        myWriter.Write(0);
            ////        myWriter.Write("ssc-mainnet-hive");
            ////        myWriter.Write("{\\\"contractName\\\":\\\"tokens\\\",\\\"contractAction\\\":\\\"issue\\\",\\\"contractPayload\\\":{\\\"symbol\\\":\\\"HKLEARN\\\",\\\"to\\\":\\\"apzyx\\\",\\\"quantity\\\":\\\"7.000\\\"}}");
            ////        myWriter.Write(0);
            ////    // write here                                                                

            ////    myStream.Flush();
            ////    myStream.Position = 0;

            ////    var myReader = new BinaryReader(myStream);

            Byte[] array1 = BitEndianConverter.GetBytes("beeab0de00000000000000000000000000000000000000000000000000000000", true);
            Byte[] array2 = BitEndianConverter.GetBytes((UInt32)59777, true);
            Byte[] array3 = BitEndianConverter.GetBytes((UInt32)1063802710, true);
            Byte[] array4 = BitEndianConverter.GetBytes("2022-08-15T11:03:15", true);
            Byte[] array5 = BitEndianConverter.GetBytes(1, true);
            Byte[] array6 = BitEndianConverter.GetBytes(18, true);
            Byte[] array7 = BitEndianConverter.GetBytes(1, true);
            Byte[] array8 = BitEndianConverter.GetBytes("hk-students", true);
            Byte[] array9 = BitEndianConverter.GetBytes(0, true);
            Byte[] array10 = BitEndianConverter.GetBytes("ssc-mainnet-hive", true);
            Byte[] array11 = BitEndianConverter.GetBytes("{\\\"contractName\\\":\\\"tokens\\\",\\\"contractAction\\\":\\\"issue\\\",\\\"contractPayload\\\":{\\\"symbol\\\":\\\"HKLEARN\\\",\\\"to\\\":\\\"apzyx\\\",\\\"quantity\\\":\\\"7.000\\\"}}", true);
            Byte[] array12 = BitEndianConverter.GetBytes(0, true);
            Byte[] data = Combine(array1, array2, array3, array4, array5, array6, array7, array8, array9, array10, array11, array12);

                //myReader.Read(data, 0, (Int32)myStream.Length);
                                    
                    Byte[] digest = GetHash(data);

                string utfInput = ByteArrayToString(digest);

                String privateHex = "5JC241FGfd6yrCbeVmP3hZNsk7rjypPZ8VxFM3Hdg5PUmE8W3pF";

                    Byte[] keyBuffer = Base58.Decode(privateHex); ;

                    //string iS = ByteArrayToString(inputKey);

                    byte[] take1 = keyBuffer.Take(keyBuffer.Length - 4).ToArray();
                    var reverse1 = take1.Reverse().ToArray();
                    var take2 = reverse1.Take(reverse1.Length - 1).ToArray();
                    var privateKeyA = take2.Reverse().ToArray();
                var privateAHEX = ByteArrayToString(privateKeyA);

                BigInteger privateKey = Hex.HexToBigInteger(privateAHEX);

                MessageSignerVerifier messageSigner = new MessageSignerVerifier();
                SignedMessage signedMessage = messageSigner.Sign(privateKey, utfInput);
                var signature2 = ByteArrayToString(signedMessage.SignatureBytes);
                bool verified = messageSigner.Verify(signedMessage);

            probaturas();

            Console.ReadKey();
            return;
        }

        class model
        {
            public String jsonrpc { get; set; }
            public String method { get; set; }            
            public Int32 id { get; set; }
        }

        class response
        {
            public String id { get; set; }
            public String json { get; set; }
            public result result { get; set; }
        }

        class result
        {
            public Int32 head_block_number { get; set; }
            public String head_block_id { get; set; }

            public String time { get; set; }
        }

        internal class WebApiClient : IDisposable
        {

            private bool _isDispose;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void Dispose(bool disposing)
            {
                if (!_isDispose)
                {

                    if (disposing)
                    {

                    }
                }

                _isDispose = true;
            }

            private void SetHeaderParameters(WebClient client)
            {
                client.Headers.Clear();
                client.Headers.Add("Content-Type", "application/json");
                client.Encoding = Encoding.UTF8;
            }

            public async Task<String> PostJsonWithModelAsync(string address, string data)
            {
                using (var client = new WebClient())
                {
                    SetHeaderParameters(client);
                    string result = await client.UploadStringTaskAsync(address, data); //  method:
                                                                                       //The HTTP method used to send the file to the resource. If null, the default is  POST 
                    return result;
                }
            }
        }

        static string LittleEndian(string num)
        {
            return num[1].ToString() + num[0].ToString() ;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


        static bool isCanonicalSignature(Byte[] signature)
        {
            return (
              !Convert.ToBoolean(signature[0] & 0x80) &&
              !(Convert.ToBoolean(signature[0] == 0) && !Convert.ToBoolean(signature[1] & 0x80)) &&
              !Convert.ToBoolean(signature[32] & 0x80) &&
              !(Convert.ToBoolean(signature[32] == 0) && !Convert.ToBoolean(signature[33] & 0x80))
            );
        }


        private static async void probaturas()
        {
            var url = "https://api.hive.blog";
            
            try
            {
                using (var client = new WebApiClient())
                {
                    var serializeModel = "{ \"jsonrpc\":\"2.0\",\"method\":\"database_api.get_dynamic_global_properties\",\"id\":1}";
                    //JsonConvert.SerializeObject(model);// using Newtonsoft.Json;
                    var response = await client.PostJsonWithModelAsync(url, serializeModel);
                    var jsonResponse = JsonConvert.DeserializeObject<response>(response);
                    if (jsonResponse != null && jsonResponse.result!= null) {
                        //var ref_block_numInt = jsonResponse.result.head_block_number & 0xffff;
                        UInt32 ref_block_num = (UInt32)(jsonResponse.result.head_block_number & 0xffff);

                        //byte[] bytes = Encoding.Default.GetBytes(jsonResponse.result.head_block_id);
                        //string hexString = BitConverter.ToString(bytes).Replace("-", "");                                                              
                        var buffer = StringToByteArray(jsonResponse.result.head_block_id);
                        var value = BitConverter.ToUInt32(buffer, 4);
                        var ref_block_prefix = value;
                        //var rs = BitConverter.ToUInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0); BigEnddian

                        //String pruebaHEX = "2109199834aee8ff";       // R:4293439028   

                        DateTime time = Convert.ToDateTime(jsonResponse.result.time);
                        var expiration = time.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss");

                        Byte[] array1 = BitEndianConverter.GetBytes("beeab0de00000000000000000000000000000000000000000000000000000000", true);
                        Byte[] array2 = BitEndianConverter.GetBytes((UInt32)ref_block_num, true);
                        Byte[] array3 = BitEndianConverter.GetBytes((UInt32)ref_block_prefix, true);
                        Byte[] array4 = BitEndianConverter.GetBytes(expiration, true);
                        Byte[] array5 = BitEndianConverter.GetBytes(1, true);
                        Byte[] array6 = BitEndianConverter.GetBytes(18, true);
                        Byte[] array7 = BitEndianConverter.GetBytes(1, true);
                        Byte[] array8 = BitEndianConverter.GetBytes("hk-students", true);
                        Byte[] array9 = BitEndianConverter.GetBytes(0, true);
                        Byte[] array10 = BitEndianConverter.GetBytes("ssc-mainnet-hive", true);
                        Byte[] array11 = BitEndianConverter.GetBytes("{\\\"contractName\\\":\\\"tokens\\\",\\\"contractAction\\\":\\\"issue\\\",\\\"contractPayload\\\":{\\\"symbol\\\":\\\"HKLEARN\\\",\\\"to\\\":\\\"apzyx\\\",\\\"quantity\\\":\\\"7.000\\\"}}", true);
                        Byte[] array12 = BitEndianConverter.GetBytes(0, true);
                        Byte[] data = Combine(array1, array2, array3, array4, array5, array6, array7, array8, array9, array10, array11, array12);

                        //myReader.Read(data, 0, (Int32)myStream.Length);

                        Byte[] digest = GetHash(data);

                        string utfInput = ByteArrayToString(digest);

                        String privateHex = "5JC241FGfd6yrCbeVmP3hZNsk7rjypPZ8VxFM3Hdg5PUmE8W3pF";
                        
                        Byte[] keyBuffer = Base58.Decode(privateHex); ;

                        //byte[] take1 = keyBuffer.Take(keyBuffer.Length - 4).ToArray();
                        //var reverse1 = take1.Reverse().ToArray();
                        //var take2 = reverse1.Take(reverse1.Length - 1).ToArray();
                        //var privateKeyA = take2.Reverse().ToArray();
                        //var privateAHEX = ByteArrayToString(privateKeyA);

                        BigInteger privateKey = Hex.HexToBigInteger(ByteArrayToString(keyBuffer));

                        

                        MessageSignerVerifier messageSigner = new MessageSignerVerifier();

                        var verified = false;
                        var signature2 = String.Empty;
                        while (!verified)
                        {

                            SignedMessage signedMessage = messageSigner.Sign(privateKey, utfInput);
                            signature2 = ByteArrayToString(signedMessage.SignatureBytes);
                            verified = isCanonicalSignature(signedMessage.SignatureBytes);
                        }

                        var jsonSerialize = "{ \"jsonrpc\":\"2.0\",\"method\":\"condenser_api.broadcast_transaction\"," +
                            "\"params\": [{" +
                                "\"id\": \"" + jsonResponse.result.head_block_id + "\"," +
                                "\"ref_block_num\": " + ref_block_num+ "," +
                                "\"ref_block_prefix\": " + ref_block_prefix + "," +
                                "\"expiration\": \"" + expiration + "\"," +
                                "\"operations\": " +
                                    "[[\"custom_json\"," +
                                        "{\"required_auths\": [\"hk-students\"]," +
                                        "\"required_posting_auths\": []," +
                                        "\"id\": \"ssc-mainnet-hive\"," +
                                        "\"json\": " +
                                            "\"{\\\"contractName\\\":\\\"tokens\\\",\\\"contractAction\\\":\\\"issue\\\"," +
                                            "\\\"contractPayload\\\":{\\\"symbol\\\":\\\"HKLEARN\\\",\\\"to\\\":\\\"apzyx\\\",\\\"quantity\\\":\\\"7.000\\\"}}\"}" +
                                    "]]," +
                                "\"extensions\": []," +
                                "\"signatures\": [\"" + signature2 + "\"]" +
                            "}]," +
                            "\"id\":1}";

                        var response2 = await client.PostJsonWithModelAsync(url, jsonSerialize);
                        var jsonResponse2 = JsonConvert.DeserializeObject(response2);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
                                                
            //tx.operations = new KeyValuePair<string, KeyValuePair<string, string>[]>("transfer", );
            HttpContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("jsonrpc", "2.0"),
                    new KeyValuePair<string, string>("method", "condenser_api.broadcast_transaction"),
                    new KeyValuePair<string, string>("id", "1"),
                    new KeyValuePair<string, string>("params", "{ \"trx\": \"HKLEARN\", \"to\": \"apzyx\", \"quantity\": \"7\" }")
                });

            using (var httpClient = new HttpClient())
            {
                try
                {
                    DateTime unMinutoDespues = DateTime.Now.AddMinutes(1);

                    var httpResponse = await httpClient.PostAsync(url, content);
                    var responseBody = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var jsonR = JsonConvert.DeserializeObject(responseBody);

                }
                catch (Exception ex)
                {

                }
            }
        }

    }

    public static class BitEndianConverter
    {
        public static byte[] GetBytes(bool value, bool littleEndian)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value), littleEndian);
        }

        public static byte[] GetBytes(String value, bool littleEndian)
        {
            return ReverseAsNeeded(Encoding.UTF8.GetBytes(value), littleEndian);
        }

        public static byte[] GetBytes(char value, bool littleEndian)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value), littleEndian);
        }
        public static byte[] GetBytes(double value, bool littleEndian)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value), littleEndian);
        }
        public static byte[] GetBytes(float value, bool littleEndian)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value), littleEndian);
        }
        public static byte[] GetBytes(int value, bool littleEndian)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value), littleEndian);
        }
        public static byte[] GetBytes(long value, bool littleEndian)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value), littleEndian);
        }
        public static byte[] GetBytes(short value, bool littleEndian)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value), littleEndian);
        }
        public static byte[] GetBytes(uint value, bool littleEndian)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value), littleEndian);
        }
        public static byte[] GetBytes(ulong value, bool littleEndian)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value), littleEndian);
        }
        public static byte[] GetBytes(ushort value, bool littleEndian)
        {
            return ReverseAsNeeded(BitConverter.GetBytes(value), littleEndian);
        }

        private static byte[] ReverseAsNeeded(byte[] bytes, bool wantsLittleEndian)
        {
            if (wantsLittleEndian == BitConverter.IsLittleEndian)
                return bytes;
            else
                return (byte[])bytes.Reverse().ToArray();
        }
    }

}
