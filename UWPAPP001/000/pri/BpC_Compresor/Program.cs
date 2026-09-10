using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BpC_Compresor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(" INIT apzyxGames.azurewebsites.net =)");
                Console.WriteLine("UNCOMPRESSING > " + args[0]);
                Console.WriteLine(" >>> ");
                Console.WriteLine("");

                using (System.IO.FileStream fileS = File.OpenRead(args[0]))
                {
                    fileS.Position = 0;
                    Int32 max = (Int32)fileS.Length;

                    Byte[] bSSS = new Byte[max];
                    fileS.Read(bSSS, 0, max);

                    Dictionary<Byte, Int32> BCNT = new Dictionary<Byte, Int32>();

                    for (Int32 z = 0; z < bSSS.Length; z++)
                    {
                        if (BCNT.ContainsKey(bSSS[z]))
                        {
                            BCNT[bSSS[z]]++;
                        }
                        else
                        {
                            BCNT.Add(bSSS[z], 0);
                        }
                    }

                    //Dictionary<Byte, Int32> BCNT = BL.GroupBy(x => x).Select(x => new KeyValuePair<Byte, Int32>(x.Key, x.Count())).ToDictionary(x=> x.Key, y => y.Value);
                    Byte Bp = BCNT.OrderByDescending(x => x.Value).First().Key;

                    Byte P = Convert.ToByte(100 * BCNT[Bp] / bSSS.Length);

                    List<Byte> fBCO = new List<Byte>();
                    Int32 flag = 0;
                    for (Int32 i = 0; i < bSSS.Length; i++)
                    {
                        if (Bp == bSSS[i] && flag != 1)
                        {
                            if (flag == 0)
                            {
                                flag++;
                            }
                            else
                            {
                                flag = 0;
                            }
                        }
                        else
                        {
                            fBCO.Add(bSSS[i]);
                            flag = 0;
                        }
                    }

                    String pathFile = Path.GetDirectoryName(args[0]);

                    using (var fs = new FileStream(pathFile + "\\helloBpC.bpc", FileMode.Create, FileAccess.Write))
                    {
                        fs.WriteByte(Convert.ToByte(P));
                        fs.WriteByte(Bp);

                        foreach (Byte b in fBCO)
                        {
                            fs.WriteByte(b);
                        }
                    }

                    Console.WriteLine("THE END");
                    Console.Beep();
                    Thread.Sleep(10000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EEEEEEEEEERROR =(");
                Console.Beep();
                Thread.Sleep(10000);
            }
        }
    }
}
