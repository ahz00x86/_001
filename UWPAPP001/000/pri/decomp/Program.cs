using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace decomp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args == null && args.Length > 0)
            {
                Console.WriteLine("Please, copy file to decompress in the same directory of this application and type his name. Fe: directory:\\>decomp.exe archivo9.bhm");
            }
            else
            {
                byte[] r = System.IO.File.ReadAllBytes(args[0]);
                     
                Console.WriteLine("Starting Decompression...");
                Byte[] temp = decomp(r);
                GC.Collect();
                                
                System.IO.File.WriteAllBytes("data9", temp);
                Console.WriteLine("The decompression ended");

                String stop = "stop";
            }

            Console.ReadKey();
        }

        private static Byte[] decomp(Byte[] data)
        {
            List<Byte> result = new List<Byte>();
            Int32 actualRound = -1;
            Boolean isFirst = true;
            do
            {
                if(!isFirst)
                {
                    data = result.ToArray();
                    result = new List<Byte>();
                }
                isFirst= false;
                
                actualRound = Convert.ToInt32(data[0]);

                Console.WriteLine("Decompressing... Round " + actualRound.ToString());

                String s02comL = getString01FromByte(data[1]);
                Int32 compFixL = Convert.ToInt32(ToBase10(s02comL.Substring(0, 3), 2));
                Int32 bytesFix = Convert.ToInt32(ToBase10(s02comL.Substring(3, 5), 2));
                                
                String sComp = String.Empty;
                String sBActual = String.Empty;
                for (Int32 i = 2; i < data.Length - 1; i++)
                {
                    sBActual += getString01FromByte(data[i]);

                    while (sBActual.Length > 5)
                    {
                        String s = sBActual.Substring(0, 3);
                        sBActual = sBActual.Remove(0, 3);

                        if (sBActual[0] == '0')
                        {
                            sComp += s;
                        }
                        else
                        {
                            sBActual = sBActual.Remove(0, 1);
                            if (sBActual[0] == '0')
                            {
                                sComp += s + s;
                            }
                            else
                            {
                                sComp += s + s + s;
                            }
                            sBActual = sBActual.Remove(0, 1);
                        }
                    }

                    while (sComp.Length > 7)
                    {
                        result.Add(getByteFromString01(sComp.Substring(0, 8)));
                        sComp = sComp.Remove(0, 8);
                    }
                }

            }
            while (actualRound > 0);

            return result.ToArray();
        }

        private static String getString01FromByte(Byte dataB)
        {
            return Convert.ToString(dataB, 2).PadLeft(8, '0');
        }

        private static Byte getByteFromString01(String data01)
        {
            if (data01.Length != 8)
            {
                throw new Exception("Error de dato 8");
            }
            return Convert.ToByte(data01.Substring(0, 8), 2);
        }

        private static string FromBase10(String number, int target_base)
        {

            if (target_base < 2 || target_base > 36) return "";
            if (target_base == 10) return number.ToString();

            int n = target_base;
            BigInteger q = BigInteger.Parse(number);
            BigInteger r;
            string rtn = "";

            while (q >= n)
            {

                r = q % n;
                q = q / n;

                if (r < 10)
                    rtn = r.ToString() + rtn;
                else
                    rtn = Convert.ToChar((Int32)(r + 55)).ToString() + rtn;

            }

            if (q < 10)
                rtn = q.ToString() + rtn;
            else
                rtn = Convert.ToChar((Int32)(q + 55)).ToString() + rtn;

            return rtn;
        }

        private static String ToBase10(String number, int start_base)
        {
            if (start_base < 2 || start_base > 36) return "";
            if (start_base == 10) return number;

            char[] chrs = number.ToCharArray();
            int m = chrs.Length - 1;
            BigInteger n = start_base;
            BigInteger x;
            BigInteger rtn = 0;

            foreach (char c in chrs)
            {

                if (char.IsNumber(c))
                    x = int.Parse(c.ToString());
                else
                    x = Convert.ToInt32(c) - 55;

                rtn += x * BigInteger.Pow(n, m);

                m--;

            }

            return rtn.ToString();

        }
    }
}
