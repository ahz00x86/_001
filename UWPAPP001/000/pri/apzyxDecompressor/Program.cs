using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace apzyxDecompressor
{
    internal class Program
    {
        /// <summary>
        /// Big Endian
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool[] ByteToBoolsBE(byte value)
        {
            var values = new bool[8];

            values[0] = (value & 128) == 0 ? false : true;
            values[1] = (value & 64) == 0 ? false : true;
            values[2] = (value & 32) == 0 ? false : true;
            values[3] = (value & 16) == 0 ? false : true;
            values[4] = (value & 8) == 0 ? false : true;
            values[5] = (value & 4) == 0 ? false : true;
            values[6] = (value & 2) == 0 ? false : true;
            values[7] = (value & 1) == 0 ? false : true;

            return values;
        }

        public static string ToBinaryString(BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[idx], 2);

            // Ensure leading zero exists if value is positive.
            if (binary[0] != '0' && bigint.Sign == 1)
            {
                base2.Append('0');
            }

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--)
            {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.ToString();
        }

        private static string BoolsToBINString(bool[] bools)
        {
            String binS = String.Empty;
            foreach (bool b in bools)
            {
                binS += b ? "1" : "0";
            }
            return binS;
        }

        private static Byte BoolsToByteBE(bool[] values)
        {
            byte result = 0;
            // This assumes the array never contains more than 8 elements!
            int index = 8 - values.Length;

            // Loop through the array
            foreach (bool b in values)
            {
                // if the element is 'true' set the bit at that position
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;

            //if (values.Length < 8)
            //{
            //    List<bool> bools = new List<bool>();
            //    for (int i = 0; i < 8; i++)
            //    {
            //        if (i < values.Length)
            //        {
            //            bools.Add(values[i]);
            //        }
            //        else
            //        {
            //            bools.Add(false);
            //        }
            //    }
            //    bools.Reverse();
            //    values = bools.ToArray();

            //}

            //byte b = new byte();
            //int bitIndex = 0;
            //for (int i = 0; i < 8; i++)
            //{
            //    if (values[i])
            //    {
            //        b |= (byte)(((byte)1) << bitIndex);
            //    }
            //    bitIndex++;
            //}
            //return b;
        }

        private static string SHA512(byte[] bytes)
        {
            //var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        private static string ConvertToBinaryS(int x)
        {
            char[] bits = new char[32];
            int i = 0;

            while (x != 0)
            {
                bits[i++] = (x & 1) == 1 ? '1' : '0';
                x >>= 1;
            }

            Array.Reverse(bits, 0, i);
            return new string(bits);
        }


        //public static String getInit(char state, string init)
        //{
        //    if (init != "A" && init != "B" && init != "C" && init != "D")
        //    {
        //        return init;
        //    }

        //    switch (init)
        //    {
        //        case "A":
        //            return state == '1' ? "111" : "000";
        //        case "B":
        //            return state == '1' ? "110" : "001";
        //        case "C":
        //            return state == '1' ? "101" : "010";
        //        case "D":
        //            return state == '1' ? "011" : "100";
        //    }

        //    throw new Exception("ERROR DECOMPRESSING 0x002 =(");

        //}

        public class DECOMP_T
        {            
            public String init;
        }

        public BigInteger getRandom(int length)
        {
            Random random = new Random();
            byte[] data = new byte[length];
            random.NextBytes(data);
            return new BigInteger(data);
        }

        //static String decode(List<String[]> arrs, BigInteger v) 
        //{ 
        //    String s = ""; 
        //    foreach (String[] arr in arrs) 
        //    {
        //        BigInteger M = arr.Length; 
        //        s = s + arr[Convert.ToInt32(v % M)]; v = v / M; 
        //    }
        //    return s; 
        //}

        //static void recurse(String s, List<String[]> arrs, int k, string sINITSHA, String path, Int32 nCEROS, Int32 nUNOS)
        //{
        //    try
        //    {
        //        if (k == arrs.Count)
        //        {
        //            if (s.Count(x => x == '0') == nCEROS)
        //            {

        //                List<Byte> listB = new List<Byte>();
        //                Boolean endingB = false;
        //                String endingS = String.Empty;

        //                for (int i = 0; i < s.Length; i += 8)
        //                {
        //                    if (i + 8 > s.Length)
        //                    {
        //                        endingB = true;
        //                        endingS = s.Substring(i);
        //                    }
        //                    else
        //                    {
        //                        Byte bF = Convert.ToByte(s.Substring(i, 8), 2);
        //                        listB.Add(bF);
        //                    }
        //                }

        //                if (endingB)
        //                {
        //                    endingS = endingS;
        //                }

        //                String sha512 = SHA512(listB.ToArray());

        //                if (sha512 == sINITSHA)
        //                {
        //                    Console.WriteLine("ENDING___");

        //                    String pathFile = Path.GetDirectoryName(path);

        //                    try
        //                    {
        //                        using (var fs = new FileStream(pathFile + "\\helloDECOMP.apz", FileMode.Create, FileAccess.Write))
        //                        {
        //                            foreach (Byte b in listB)
        //                            {
        //                                fs.WriteByte(b);
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        throw new Exception("UPS! ALGO SALIó MAL 0x003, GRACIAS POR SU TIEMPO,");
        //                    }

        //                    return;

        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (String o in arrs[k])
        //            {
        //                recurse(s + o, arrs, k + 1, sINITSHA, path, nCEROS, nUNOS);
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //}

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<IEnumerable<T>> lists)
        {
            // Check against an empty list.
            if (!lists.Any())
            {
                yield break;
            }

            // Create a list of iterators into each of the sub-lists.
            List<IEnumerator<T>> iterators = new List<IEnumerator<T>>();
            foreach (var list in lists)
            {
                var it = list.GetEnumerator();
                // Ensure empty sub-lists are excluded.
                if (!it.MoveNext())
                {
                    continue;
                }
                iterators.Add(it);
            }

            bool done = false;
            while (!done)
            {
                // Return the current state of all the iterator, this permutation.
                yield return from it in iterators select it.Current;

                // Move to the next permutation.
                bool recurse = false;
                var mainIt = iterators.GetEnumerator();
                mainIt.MoveNext(); // Move to the first, succeeds; the main list is not empty.
                do
                {
                    recurse = false;
                    var subIt = mainIt.Current;
                    if (!subIt.MoveNext())
                    {
                        subIt.Reset(); // Note the sub-list must be a reset-able IEnumerable!
                        subIt.MoveNext(); // Move to the first, succeeds; each sub-list is not empty.

                        if (!mainIt.MoveNext())
                        {
                            done = true;
                        }
                        else
                        {
                            recurse = true;
                        }
                    }
                }
                while (recurse);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(" INIT apzyxGames.azurewebsites.net =)");
                Console.WriteLine("DE_COMPRESSING > " + args[0]);
                Console.WriteLine(" >>> ");
                Console.WriteLine("");

                Int32 ahorro = 7;

                using (System.IO.FileStream fileS = File.OpenRead(args[0]))
                {
                    fileS.Position = 0;
                    Int32 max = (Int32)fileS.Length;

                    Byte[] bSSS = new Byte[max];
                    fileS.Read(bSSS, 0, max);

                    bool[] endingBOOL = ByteToBoolsBE(bSSS[max - 3]);
                    bool[] endingEND = ByteToBoolsBE(bSSS[max - 2]);
                    bool[] endingL = ByteToBoolsBE(bSSS[max - 1]);

                    Int32 nCOMP = bSSS[0];
                    List<String> iniTSHaStr = new List<String>();
                    Int32 nTAKE = 0;
                    for (; nTAKE < nCOMP; nTAKE ++)
                    {
                        Byte[] bSHA = bSSS.Skip(1 + (nTAKE * 128)).Take(128).ToArray();
                        String iniTSHa = System.Text.UTF8Encoding.UTF8.GetString(bSHA);
                        iniTSHaStr.Add(iniTSHa);
                    }

                    Byte[] bCEROS = bSSS.Skip(1 + (nTAKE * 128)).Take(4).ToArray();
                    Byte[] bUNOS = bSSS.Skip(1 + (nTAKE * 128) + 4).Take(4).ToArray();

                    Int32 nCEROS = BitConverter.ToInt32(bCEROS, 0);
                    Int32 nUNOS = BitConverter.ToInt32(bUNOS, 0);

                    Int32 indexSHA = 0;

                    List<String[]> nLIST = new List<String[]>();
                    
                    List<bool> bools = new List<bool>();
                    
                    for (Int32 index = 1 + (nTAKE * 128) + 8; index < bSSS.Length - 3; index++)
                    {
                        Byte b = bSSS[index];

                        bools.AddRange(ByteToBoolsBE(b));

                        while(bools.Count > ahorro - 1)
                        {
                            String sInit = "1";
                            Boolean antBoo = true;
                            foreach(bool bFluc in bools.GetRange(0, ahorro))
                            {
                                sInit += antBoo == bFluc ? "0" : "1";

                                antBoo = bFluc;
                            }

                            nLIST.Add(new string[2] { sInit, sInit.Replace('1', 'x').Replace('0','1').Replace('x', '0') });

                            bools.RemoveRange(0, ahorro);
                        }

                        //nLIST.Add(bools[0] ? (bools[1] ? new String[2] { "101", "010" } : new String[2] { "011", "100" }) : (bools[1] ? new String[2] { "110", "001" } : new String[2] { "111", "000" }));
                        //nLIST.Add(bools[2] ? (bools[3] ? new String[2] { "101", "010" } : new String[2] { "011", "100" }) : (bools[3] ? new String[2] { "110", "001" } : new String[2] { "111", "000" }));
                        //nLIST.Add(bools[4] ? (bools[5] ? new String[2] { "101", "010" } : new String[2] { "011", "100" }) : (bools[5] ? new String[2] { "110", "001" } : new String[2] { "111", "000" }));
                        //nLIST.Add(bools[6] ? (bools[7] ? new String[2] { "101", "010" } : new String[2] { "011", "100" }) : (bools[7] ? new String[2] { "110", "001" } : new String[2] { "111", "000" }));
                    }

                    Byte lenLIST = BoolsToByteBE(new bool[8] { false, false, false, false, endingL[0], endingL[1], endingL[2], endingL[3] });
                    Byte lenOffEndSET = BoolsToByteBE(new bool[8] { false, false, false, false, endingL[4], endingL[5], endingL[6], endingL[7] });

                    Int32 lenListInt = (Int32)lenLIST;
                    Int32 lenOffsetEndInt = (Int32)lenOffEndSET;

                    List<DECOMP_T> mergeEND = new List<DECOMP_T>();

                    if (lenListInt > 0)
                    {                        
                        for (Int32 j = 0; j < lenListInt; j++)
                        {
                            bools.Add(endingBOOL[j]);

                            //if (j % (ahorro - 1) == 0)
                            //{
                            //    if (j != 0)
                            //    {
                            //        String sInitL = "1";
                            //        Boolean antBoLo = true;
                            //        foreach (bool bFluc in possibilities.GetRange(0, ahorro))
                            //        {
                            //            sInitL += antBoLo == bFluc ? "0" : "1";

                            //            antBoLo = bFluc;
                            //        }

                            //        nLIST.Add(new string[2] { sInitL, sInitL.Replace('1', 'x').Replace('0', '1').Replace('x', '0') });
                            //        //nLIST.Add(first ? (second ? new String[2] { "101", "010" } : new String[2] { "011", "100" }) : (second ? new String[2] { "110", "001" } : new String[2] { "111", "000" }));
                            //    }
                            //    possibilities.Clear();
                            //    possibilities.Add(endingBOOL[j]);
                            //}
                            //else
                            //{
                            //    bools.Add(endingBOOL[j]);
                            //}

                        }

                        while (bools.Count > ahorro - 1)
                        {
                            String sInit = "1";
                            Boolean antBoo = true;
                            foreach (bool bFluc in bools.GetRange(0, ahorro))
                            {
                                sInit += antBoo == bFluc ? "0" : "1";

                                antBoo = bFluc;
                            }

                            nLIST.Add(new string[2] { sInit, sInit.Replace('1', 'x').Replace('0', '1').Replace('x', '0') });

                            bools.RemoveRange(0, ahorro);
                        }

                    }

                    String s01END = String.Empty;
                    if (lenOffsetEndInt > 0)
                    {
                        for (Int32 j = 0; j < lenOffsetEndInt; j++)
                        {
                            s01END += endingEND[j] ? "1" : "0";
                        }
                        nLIST.Add(new String[1] { s01END });
                    }

                    //recurse("", nLIST, 0, iniTSHaStr[0], args[0], nCEROS, nUNOS);

                    String stip = "aquí";

                    foreach(var ele in GetPermutations(nLIST))
                    {
                        Int32 nCeros = ele.Sum(x => x.Count(y => y == '0'));

                        if (nCeros == nCEROS)
                        {
                            var s = String.Concat(ele.ToArray());
                            List<Byte> listB = new List<Byte>();
                            Boolean endingB = false;
                            String endingS = String.Empty;

                            for (int i = 0; i < s.Length; i += 8)
                            {
                                if (i + 8 > s.Length)
                                {
                                    endingB = true;
                                    endingS = s.Substring(i);
                                }
                                else
                                {
                                    Byte bF = Convert.ToByte(s.Substring(i, 8), 2);
                                    listB.Add(bF);
                                }
                            }

                            if (endingB)
                            {
                                endingS = endingS;
                            }

                            String sha512 = SHA512(listB.ToArray());

                            if (sha512 == iniTSHaStr[0])
                            {
                                Console.WriteLine("ENDING___");

                                String pathFile = Path.GetDirectoryName(args[0]);

                                try
                                {
                                    using (var fs = new FileStream(pathFile + "\\helloDECOMP.apz", FileMode.Create, FileAccess.Write))
                                    {
                                        foreach (Byte b in listB)
                                        {
                                            fs.WriteByte(b);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("UPS! ALGO SALIó MAL 0x003, GRACIAS POR SU TIEMPO,");
                                }

                                return;

                            }
                        }

                    }



                    //BigInteger N = 1;
                    //foreach (String[] arr in nLIST) 
                    //{
                    //    N = N * arr.Length; 
                    //}
                    //for (Int32 v = 0; v < N; v++)
                    //{
                    //    String s = decode(nLIST, v);

                    //    if (s.Count(x => x == '0') == nCEROS)
                    //    {

                    //        List<Byte> listB = new List<Byte>();
                    //        Boolean endingB = false;
                    //        String endingS = String.Empty;

                    //        for (int i = 0; i < s.Length; i += 8)
                    //        {
                    //            if (i + 8 > s.Length)
                    //            {
                    //                endingB = true;
                    //                endingS = s.Substring(i);
                    //            }
                    //            else
                    //            {
                    //                Byte bF = Convert.ToByte(s.Substring(i, 8), 2);
                    //                listB.Add(bF);
                    //            }
                    //        }

                    //        if (endingB)
                    //        {
                    //            endingS = endingS;
                    //        }

                    //        String sha512 = SHA512(listB.ToArray());

                    //        if (sha512 == iniTSHaStr[0])
                    //        {
                    //            Console.WriteLine("ENDING___");

                    //            String pathFile = Path.GetDirectoryName(args[0]);

                    //            try
                    //            {
                    //                using (var fs = new FileStream(pathFile + "\\helloDECOMP.apz", FileMode.Create, FileAccess.Write))
                    //                {
                    //                    foreach (Byte b in listB)
                    //                    {
                    //                        fs.WriteByte(b);
                    //                    }
                    //                }
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                throw new Exception("UPS! ALGO SALIó MAL 0x003, GRACIAS POR SU TIEMPO,");
                    //            }

                    //            return;

                    //        }

                    //    }
                    //}



                    //String middle01 = String.Empty;
                    //Boolean flip = true;
                    //for(Int32 k = 0; k < nLIST.Count; k++)
                    //{
                    //    middle01 += flip ? "1" : "0";
                    //    flip = !flip;
                    //}

                    //BigInteger midddBV = middle01.Aggregate(BigInteger.Zero, (s, a) => (s << 1) + a - '0');

                    //BigInteger bIntUP = midddBV;
                    //BigInteger bIntDOW = midddBV - 1;

                    //BigInteger maxBInt = "".PadLeft(nLIST.Count, '1')
                    //    .Aggregate(BigInteger.Zero, (s, a) => (s << 1) + a - '0');
                    //maxBInt++;

                    //BigInteger minBInt = new BigInteger(0);

                    //Console.WriteLine("DE_COMPRESSING .. ^^^ LoOpiNgS > ");

                    //Console.Title = "% QUEDA RATO";
                    //while (bIntUP < maxBInt)
                    //{
                    //    String binUP = ToBinaryString(bIntUP).PadLeft(nLIST.Count, '0');
                    //    String stringB = new String(nLIST.SelectMany((x, i) => getInit(binUP[i], x.init)).ToArray());
                    //    Int32 cerosOF = stringB.Count(x => x == '0');

                    //    Boolean finded = cerosOF == nCEROS;

                    //    if (finded)
                    //    {
                    //        Int32 unosOF = stringB.Count(x => x == '1');
                    //        if (unosOF == nUNOS)
                    //        {

                    //        }
                    //    }

                    //String binDOW = ToBinaryString(bIntDOW).PadLeft(nLIST.Count, '0');

                    //stringB = new String(nLIST.SelectMany((x, i) => getInit(binDOW[i], x.init)).ToArray());
                    //cerosOF = stringB.Count(x => x == '0');
                    //finded = cerosOF == nCEROS;

                    //if (finded)
                    //{ 

                    //    Int32 unosOF = stringB.Count(x => x == '1');
                    //    if (unosOF == nUNOS)
                    //    {
                    //        List<Byte> listB = new List<Byte>();
                    //        Boolean endingB = false;
                    //        String endingS = String.Empty;

                    //        for (int i = 0; i < stringB.Length; i += 8)
                    //        {
                    //            if (i + 8 > stringB.Length)
                    //            {
                    //                endingB = true;
                    //                endingS = stringB.Substring(i);
                    //            }
                    //            else
                    //            {
                    //                Byte bF = Convert.ToByte(stringB.Substring(i, 8), 2);
                    //                listB.Add(bF);
                    //            }
                    //        }

                    //        if (endingB)
                    //        {
                    //            endingS = endingS;
                    //        }

                    //        String sha512 = SHA512(listB.ToArray());

                    //        if (sha512 == iniTSHaStr[indexSHA])
                    //        {
                    //            Console.WriteLine("ENDING___");

                    //            indexSHA++;
                    //            if (indexSHA == nCOMP)
                    //            {
                    //                String pathFile = Path.GetDirectoryName(args[0]);

                    //                try
                    //                {
                    //                    using (var fs = new FileStream(pathFile + "\\helloDECOMP.apz", FileMode.Create, FileAccess.Write))
                    //                    {
                    //                        foreach (Byte b in listB)
                    //                        {
                    //                            fs.WriteByte(b);
                    //                        }
                    //                    }
                    //                }
                    //                catch (Exception ex)
                    //                {
                    //                    throw new Exception("UPS! ALGO SALIó MAL 0x003, GRACIAS POR SU TIEMPO,");
                    //                }
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}




                    //    bIntUP++;
                    //    bIntDOW--;
                    //}
                }

                Console.WriteLine("THE END");
                Console.Beep();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw new Exception("UPS! ALGO SALIó MAL 0x001, GRACIAS POR SU TIEMPO,");
            }
        }
    }
}
