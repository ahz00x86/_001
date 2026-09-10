using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WIP
{
    internal class Program
    {

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

        static void Main(string[] args)
        {                        
            BigInteger n0 = new BigInteger(0);
            BigInteger nMAX = "".PadLeft(45, '1')
                .Aggregate(BigInteger.Zero, (s, a) => (s << 1) + a - '0');
            nMAX++;

            //List<Task> tasks = new List<Task>();

            while (n0 < nMAX)
            {
                Task tsk = Task.Factory.StartNew(() =>
                {
                    BigInteger n = n0;
                                        
                    String[][] WIPTEMP = new String[8][];

                    for (Int32 i = 0; i < 8; i++)
                    {
                        WIPTEMP[i] = new String[8] { "x", "x", "x", "x", "x", "x", "x", "x" };
                    }



                    // N = 19
                    WIPTEMP[0][7] = "0";
                    WIPTEMP[1][1] = "0";
                    WIPTEMP[1][2] = "0";
                    WIPTEMP[1][5] = "1";
                    WIPTEMP[2][1] = "0";
                    WIPTEMP[2][5] = "1";
                    WIPTEMP[2][7] = "0";
                    WIPTEMP[3][2] = "1";
                    WIPTEMP[4][0] = "0";
                    WIPTEMP[4][1] = "0";
                    WIPTEMP[4][3] = "1";
                    WIPTEMP[4][6] = "1";
                    WIPTEMP[5][4] = "1";
                    WIPTEMP[6][0] = "1";
                    WIPTEMP[6][1] = "1";
                    WIPTEMP[6][5] = "0";
                    WIPTEMP[6][7] = "1";
                    WIPTEMP[7][1] = "1";
                    WIPTEMP[7][7] = "1";

                    //String[][] WIP = new String[8][];

                    //for (Int32 i = 0; i < 8; i++)
                    //{
                    //    WIP[i] = new String[8] { "x", "x", "x", "x", "x", "x", "x", "x" };
                    //}



                    //// N = 19
                    //WIP[0][7] = "0";
                    //WIP[1][1] = "0";
                    //WIP[1][2] = "0";
                    //WIP[1][5] = "1";
                    //WIP[2][1] = "0";
                    //WIP[2][5] = "1";
                    //WIP[2][7] = "0";
                    //WIP[3][2] = "1";
                    //WIP[4][0] = "0";
                    //WIP[4][1] = "0";
                    //WIP[4][3] = "1";
                    //WIP[4][6] = "1";
                    //WIP[5][4] = "1";
                    //WIP[6][0] = "1";
                    //WIP[6][1] = "1";
                    //WIP[6][5] = "0";
                    //WIP[6][7] = "1";
                    //WIP[7][1] = "1";
                    //WIP[7][7] = "1";

                    // 8 x 8 = 64 - 19 = 45

                    //String[][] WIPTEMP = new String[8][];

                    //for (Int32 i = 0; i < 8; i++)
                    //{
                    //    WIPTEMP[i] = new String[8] { "x", "x", "x", "x", "x", "x", "x", "x" };
                    //}

                    //// CLEAR MATRIZ TEMP                                
                    //for (int i = 0; i < WIPTEMP.Length; i++)
                    //{
                    //    for (int j = 0; j < WIPTEMP[i].Length; j++)
                    //    {
                    //        WIPTEMP[i][j] = WIP[i][j];
                    //    }
                    //}

                    // PROPONER SUPUESTO
                    String s = ToBinaryString(n).PadLeft(45, '0');

                    // AGREGAR SUPUESTO A MATRIZ

                    Int32 sIndex = 0;
                    for (int i = 0; i < WIPTEMP.Length; i++)
                    {
                        for (int j = 0; j < WIPTEMP[i].Length; j++)
                        {
                            String WIPs = WIPTEMP[i][j];

                            if (WIPs == "x")
                            {
                                WIPTEMP[i][j] = s[sIndex].ToString();
                                sIndex++;
                            }
                        }
                    }

                    // VALIDACION DE REGLAS WIP
                    Boolean ok = true;

                    Int32[] countR0 = new Int32[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    Int32[] countR1 = new Int32[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

                    Int32[] iqual0011R = new Int32[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    String[] iqualSR = new string[8] { String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty };

                    for (int i = 0; i < WIPTEMP.Length; i++)
                    {
                        Int32 countC0 = 0;
                        Int32 countC1 = 0;

                        Int32 iqual0011C = 0;
                        String iqualSC = String.Empty;
                        for (int j = 0; j < WIPTEMP[i].Length; j++)
                        {
                            String WIPs = WIPTEMP[i][j];

                            if (WIPs == "x")
                            {
                                throw new Exception("EIN?");
                            }

                            if (WIPs == iqualSC)
                            {
                                iqual0011C++;

                                if (iqual0011C > 2)
                                {
                                    ok = false;
                                    break;
                                }

                            }
                            else
                            {
                                iqual0011C = 0;
                            }
                            iqualSC = WIPs;

                            if (WIPs == iqualSR[i])
                            {
                                iqual0011R[i]++;

                                if (iqual0011R[i] > 2)
                                {
                                    ok = false;
                                    break;
                                }

                            }
                            else
                            {
                                iqual0011R[i] = 0;
                            }
                            iqualSR[i] = WIPs;

                            if (WIPs == "0")
                            {
                                countR0[i]++;

                                if (countR0[i] > 4)
                                {
                                    ok = false;
                                    break;
                                }

                                countC0++;

                                if (countC0 > 4)
                                {
                                    ok = false;
                                    break;
                                }

                            }

                            if (WIPs == "1")
                            {
                                countR1[i]++;

                                if (countR1[i] > 4)
                                {
                                    ok = false;
                                    break;
                                }

                                countC1++;

                                if (countC1 > 4)
                                {
                                    ok = false;
                                    break;
                                }

                            }

                        }

                        if (!ok)
                        {
                            break;
                        }

                    }

                    // GENERAMOS SOLUCIONA EN ARCHIVO SI OK REGLAS

                    if (ok)
                    {
                        Console.WriteLine("n:" + n.ToString());

                        for (int i = 0; i < WIPTEMP.Length; i++)
                        {
                            String line = String.Empty;

                            for (int j = 0; j < WIPTEMP[i].Length; j++)
                            {
                                String WIPs = WIPTEMP[i][j];
                                line += WIPs;
                            }

                            Console.WriteLine(line);

                        }
                    }

                    //WIPTEMP = WIP;
                    // CLEAR MATRIZ TEMP                                
                    //for (int i = 0; i < WIPTEMP.Length; i++)
                    //{
                    //    for (int j = 0; j < WIPTEMP[i].Length; j++)
                    //    {
                    //        WIPTEMP[i][j] = WIP[i][j];
                    //    }
                    //}
                });

                //tasks.Add(tsk);
                
                //tsk.ContinueWith(t =>
                //{
                //    // Bloquear la lista para evitar condiciones de carrera
                //    lock (tasks)
                //    {
                //        // Eliminar la tarea de la lista
                //        tasks.Remove(t);
                //    }
                //});

                //while (tasks.Count > 1000)
                //{
                //    Task.WaitAll();
                //    //Thread.Sleep(1000);
                //}


                

                n0++;
            }

            Console.WriteLine("THE END");
            Console.ReadKey();
        }
    }
}
