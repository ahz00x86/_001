using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace apzyxCompressor
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
            //    for(int i = 0; i < 8; i++)
            //    {
            //        if(i < values.Length)
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

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(" INIT apzyxGames.azurewebsites.net =)");
                Console.WriteLine("COMPRESSING > " + args[0]);
                Console.WriteLine(" >>> ");
                Console.WriteLine("");

                //Int32 ahorro = 7;

                using (System.IO.FileStream fileS = File.OpenRead(args[0]))
                {
                    fileS.Position = 0;
                    Int32 max = (Int32)fileS.Length;

                    Byte[] bSSS = new Byte[max];
                    fileS.Read(bSSS, 0, max);

                    BigInteger bG = new BigInteger(bSSS);

                    // Primos
                    //List<Int32> options = new List<Int32>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
                    //    31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
                    //    73, 79, 83, 89, 97, 101, 103, 107, 109, 113,
                    //    127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
                    //    179, 181, 191, 193, 197, 199 };

                    String finalP = String.Empty;
                    Boolean seguir = true;

                    while (seguir)
                    {
                        BigInteger[,] vals2 = new BigInteger[512, 3];

                        for (Int32 i = 3; i < 512; i+=2)
                        {
                            BigInteger index = 1;
                            BigInteger r = i;

                            while (r < bG)
                            {
                                r *= r;
                                index++;
                            }

                            vals2[i, 0] = i;
                            vals2[i, 1] = index;                            
                            vals2[i, 2] = r - bG;

                            //BigInteger supMax = r;

                            //for (Int32 m = 1; m < 200; m++)
                            //{                                 
                            //    if (supMax - m - bG < vals2[i, 3])
                            //    {
                            //        vals2[i, 2] = m;
                            //        vals2[i, 3] = (bG - m) - minMax;
                            //    }

                            //}
                        }

                        BigInteger bG22 = -1;
                        String fNP = String.Concat(vals2[2, 0].ToString(), "_",
                            vals2[2, 1].ToString());
                        //,"_",
                        //vals2[2, 2].ToString());

                        String nF = String.Concat(vals2[3, 0].ToString(), "_",
                            vals2[3, 1].ToString(), "_",
                            vals2[3, 2].ToString());
                        //, "_",
                          //  vals2[2, 3].ToString());

                        for (Int32 i = 5; i < 510; i+=2)
                        {
                            String fNPt = String.Concat(vals2[i, 0].ToString(), "_",
                            vals2[i, 1].ToString());
                            //, "_",
                            //vals2[i, 2].ToString());

                            String t = String.Concat(vals2[i, 0].ToString(), "_",
                            vals2[i, 1].ToString(), "_",
                            vals2[i, 2].ToString());
                            //, "_",
                            //vals2[i, 3].ToString());

                            if (t.Length < nF.Length)
                            {
                                fNP = fNPt;
                                nF = t;
                                bG22 = vals2[i, 2];
                            }
                        }

                        finalP += String.Concat(fNP, "_");
                        bG = bG22;

                        if (bG22 < 512)
                        {
                            finalP += String.Concat(bG22.ToString(), "_");
                            seguir = false;
                        }

                    }

                    ////////BigInteger[,] vals = new BigInteger[512, 4];

                    ////////for (Int32 i = 2; i < 512; i++)
                    ////////{
                    ////////    BigInteger index = 1;
                    ////////    BigInteger r = i;

                    ////////    while (r < bG)
                    ////////    {
                    ////////        r *= r;
                    ////////        index++;
                    ////////    }

                    ////////    vals[i, 0] = i;
                    ////////    vals[i, 1] = index - 1;
                    ////////    vals[i, 2] = 0;
                    ////////    vals[i, 3] = bG - (r / i);

                    ////////    for (Int32 m = 1; m < 200; m++)
                    ////////    {
                    ////////        BigInteger minMax = BigInteger.Pow(vals[i, 0], Convert.ToInt32(vals[i, 1].ToString()));

                    ////////        if ((bG - m) - minMax < vals[i, 3])
                    ////////        {
                    ////////            vals[i, 2] = m;
                    ////////            vals[i, 3] = (bG - m) - minMax;
                    ////////        }

                    ////////    }
                    ////////}

                    ////////String finalP = String.Concat(vals[2, 0].ToString(), "_",
                    ////////    vals[2, 1].ToString(), "_",
                    ////////    vals[2, 2].ToString(), "_",
                    ////////    vals[2, 3].ToString());

                    ////////for (Int32 i = 3; i < 510; i++)
                    ////////{
                    ////////    String t = String.Concat(vals[i, 0].ToString(), "_",
                    ////////    vals[i, 1].ToString(), "_",
                    ////////    vals[i, 2].ToString(), "_",
                    ////////    vals[i, 3].ToString());

                    ////////    if (t.Length < finalP.Length)
                    ////////    {
                    ////////        finalP = t;
                    ////////    }
                    ////////}

                    String pathFile = Path.GetDirectoryName(args[0]);
                    Byte[] listB = Encoding.UTF8.GetBytes(finalP);

                    using (var fs = new FileStream(pathFile + "\\helloCOMP.apz", FileMode.Create, FileAccess.Write))
                    {
                        foreach (Byte b in listB)
                        {
                            fs.WriteByte(b);
                        }
                    }
                    

                    ////////////List<Byte> listB = new List<Byte>();

                    ////////////String sBinTemp = String.Empty;

                    ////////////List<Boolean> lTEMPBOOL = new List<Boolean>();

                    ////////////Console.WriteLine("COMPRESSING " + max + " ByteS > ");
                    ////////////Int32 nCEROs = 0;
                    ////////////Int32 nUNOs = 0;
                    ////////////for (Int32 inFi = 0; inFi < max; inFi ++)
                    ////////////{
                    ////////////    String s = "% " + Math.Round((double)(inFi * 100 / max)).ToString() + " > nB: " + inFi.ToString().PadLeft(max.ToString().Length, '0');

                    ////////////    Console.Title = s;

                    ////////////    Byte B = bSSS[inFi];

                    ////////////    bool[] sBIN = ByteToBoolsBE(B);

                    ////////////    nCEROs += sBIN.Count(x => !x);
                    ////////////    nUNOs += sBIN.Count(x => x); ;

                    ////////////    String bS = BoolsToBINString(sBIN);

                    ////////////    sBinTemp += bS;

                    ////////////    while(sBinTemp.Length > ahorro - 1)
                    ////////////    {
                    ////////////        String subS = sBinTemp.Substring(0, ahorro);
                    ////////////        sBinTemp = sBinTemp.Remove(0, ahorro);

                    ////////////        Char ant = 'x';
                    ////////////        foreach(Char c in subS)
                    ////////////        {
                    ////////////            if (ant == 'x')
                    ////////////            {
                    ////////////                ant = c;
                    ////////////            }
                    ////////////            else
                    ////////////            {
                    ////////////                lTEMPBOOL.Add(ant != c);
                    ////////////                ant = c;
                    ////////////            }
                    ////////////        }

                    ////////////        //////switch (subS)
                    ////////////        //////{
                    ////////////        //////    case "000":
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        break;
                    ////////////        //////    case "001":
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        lTEMPBOOL.Add(true);
                    ////////////        //////        break;
                    ////////////        //////    case "010":
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        break;
                    ////////////        //////    case "011":
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        lTEMPBOOL.Add(true);
                    ////////////        //////        break;
                    ////////////        //////    case "100":
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        lTEMPBOOL.Add(true);
                    ////////////        //////        break;
                    ////////////        //////    case "101":
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        break;
                    ////////////        //////    case "110":
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        lTEMPBOOL.Add(true);
                    ////////////        //////        break;
                    ////////////        //////    case "111":
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        lTEMPBOOL.Add(false);
                    ////////////        //////        break;
                    ////////////        //////}
                    ////////////    }

                    ////////////    while(lTEMPBOOL.Count >= 8)
                    ////////////    {
                    ////////////        listB.Add(BoolsToByteBE(lTEMPBOOL.GetRange(0, 8).ToArray()));
                    ////////////        lTEMPBOOL.RemoveRange(0, 8);
                    ////////////    }



                    ////////////}

                    ////////////bool[] endinGLength = new bool[8];
                    ////////////bool[] endinGBOOL = new bool[8];
                    ////////////bool[] endinGEND = new bool[8];

                    ////////////if (lTEMPBOOL.Count > 0)
                    ////////////{
                    ////////////    switch (lTEMPBOOL.Count)
                    ////////////    {
                    ////////////        case 0:
                    ////////////            endinGLength[0] = false;
                    ////////////            endinGLength[1] = false;
                    ////////////            endinGLength[2] = false;
                    ////////////            endinGLength[3] = false;
                    ////////////            break;
                    ////////////        case 1:
                    ////////////            endinGLength[0] = false;
                    ////////////            endinGLength[1] = false;
                    ////////////            endinGLength[2] = false;
                    ////////////            endinGLength[3] = true;
                    ////////////            break;
                    ////////////        case 2:
                    ////////////            endinGLength[0] = false;
                    ////////////            endinGLength[1] = false;
                    ////////////            endinGLength[2] = true;
                    ////////////            endinGLength[3] = false;
                    ////////////            break;
                    ////////////        case 3:
                    ////////////            endinGLength[0] = false;
                    ////////////            endinGLength[1] = false;
                    ////////////            endinGLength[2] = true;
                    ////////////            endinGLength[3] = true;
                    ////////////            break;
                    ////////////        case 4:
                    ////////////            endinGLength[0] = false;
                    ////////////            endinGLength[1] = true;
                    ////////////            endinGLength[2] = false;
                    ////////////            endinGLength[3] = false;
                    ////////////            break;
                    ////////////        case 5:
                    ////////////            endinGLength[0] = false;
                    ////////////            endinGLength[1] = true;
                    ////////////            endinGLength[2] = false;
                    ////////////            endinGLength[3] = true;
                    ////////////            break;
                    ////////////        case 6:
                    ////////////            endinGLength[0] = false;
                    ////////////            endinGLength[1] = true;
                    ////////////            endinGLength[2] = true;
                    ////////////            endinGLength[3] = false;
                    ////////////            break;
                    ////////////        case 7:
                    ////////////            endinGLength[0] = false;
                    ////////////            endinGLength[1] = true;
                    ////////////            endinGLength[2] = true;
                    ////////////            endinGLength[3] = true;
                    ////////////            break;
                    ////////////    }

                    ////////////    for (int iEnding = 0; iEnding < lTEMPBOOL.Count; iEnding++)
                    ////////////    {
                    ////////////        endinGBOOL[iEnding] = lTEMPBOOL[iEnding];
                    ////////////    }
                    ////////////}

                    ////////////if (sBinTemp.Length > 0)
                    ////////////{
                    ////////////    switch (sBinTemp.Length)
                    ////////////    {
                    ////////////        case 0:
                    ////////////            endinGLength[4] = false;
                    ////////////            endinGLength[5] = false;
                    ////////////            endinGLength[6] = false;
                    ////////////            endinGLength[7] = false;
                    ////////////            break;
                    ////////////        case 1:
                    ////////////            endinGLength[4] = false;
                    ////////////            endinGLength[5] = false;
                    ////////////            endinGLength[6] = false;
                    ////////////            endinGLength[7] = true;
                    ////////////            break;
                    ////////////        case 2:
                    ////////////            endinGLength[4] = false;
                    ////////////            endinGLength[5] = false;
                    ////////////            endinGLength[6] = true;
                    ////////////            endinGLength[7] = false;
                    ////////////            break;
                    ////////////        case 3:
                    ////////////            endinGLength[4] = false;
                    ////////////            endinGLength[5] = false;
                    ////////////            endinGLength[6] = true;
                    ////////////            endinGLength[7] = true;
                    ////////////            break;
                    ////////////        case 4:
                    ////////////            endinGLength[4] = false;
                    ////////////            endinGLength[5] = true;
                    ////////////            endinGLength[6] = false;
                    ////////////            endinGLength[7] = false;
                    ////////////            break;
                    ////////////        case 5:
                    ////////////            endinGLength[4] = false;
                    ////////////            endinGLength[5] = true;
                    ////////////            endinGLength[6] = false;
                    ////////////            endinGLength[7] = true;
                    ////////////            break;
                    ////////////        case 6:
                    ////////////            endinGLength[4] = false;
                    ////////////            endinGLength[5] = true;
                    ////////////            endinGLength[6] = true;
                    ////////////            endinGLength[7] = false;
                    ////////////            break;
                    ////////////        case 7:
                    ////////////            endinGLength[4] = false;
                    ////////////            endinGLength[5] = true;
                    ////////////            endinGLength[6] = true;
                    ////////////            endinGLength[7] = true;
                    ////////////            break;
                    ////////////    }


                    ////////////    for (Int32 iEnD = 0; iEnD < sBinTemp.Length; iEnD++)
                    ////////////    {
                    ////////////        endinGEND[iEnD] = sBinTemp[iEnD] == '1' ? true : false;                            
                    ////////////    }
                    ////////////}


                    ////////////listB.Add(BoolsToByteBE(endinGBOOL));
                    ////////////listB.Add(BoolsToByteBE(endinGEND));
                    ////////////listB.Add(BoolsToByteBE(endinGLength));


                    ////////////String sha512 = SHA512(bSSS);

                    ////////////Byte[] bSHA = System.Text.Encoding.UTF8.GetBytes(sha512);

                    ////////////String pathFile = Path.GetDirectoryName(args[0]);

                    ////////////List<Byte> dataE = bSHA.ToList();

                    ////////////byte[] bCEROS = BitConverter.GetBytes(nCEROs);
                    ////////////byte[] bUNOS = BitConverter.GetBytes(nUNOs);
                    ////////////listB.InsertRange(0, bCEROS);
                    ////////////listB.InsertRange(0, bUNOS);
                    ////////////listB.InsertRange(0, dataE);
                    ////////////listB.InsertRange(0, new Byte[] { 1 });

                    ////////////try
                    ////////////{
                    ////////////    using (var fs = new FileStream(pathFile + "\\helloCOMP.apz", FileMode.Create, FileAccess.Write))
                    ////////////    {
                    ////////////        foreach (Byte b in listB)
                    ////////////        {
                    ////////////            fs.WriteByte(b);
                    ////////////        }                            
                    ////////////    }
                    ////////////}
                    ////////////catch (Exception ex)
                    ////////////{
                    ////////////    throw new Exception("UPS! ALGO SALIó MAL 0x003, GRACIAS POR SU TIEMPO,");
                    ////////////}
                }

                Console.WriteLine("THE END");
                Console.Beep();
                //Console.ReadKey();
            }
            catch (Exception ex)            
            {
                throw new Exception("UPS! ALGO SALIó MAL 0x001, GRACIAS POR SU TIEMPO,");
            }

        }
    }
}
