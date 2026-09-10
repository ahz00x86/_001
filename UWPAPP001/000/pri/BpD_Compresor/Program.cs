using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BpD_Compresor
{
    internal class Program
    {
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

        /// <summary>
        /// I.001 
        /// </summary>        
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(" INIT apzyxGames.azurewebsites.net =)");
                Console.WriteLine("COMPRESSING > " + args[0]);
                Console.WriteLine(" >>> ");
                Console.WriteLine("");
                                
                using (System.IO.FileStream fileS = File.OpenRead(args[0]))
                {
                    fileS.Position = 0;
                    Int32 max = (Int32)fileS.Length;

                    Byte[] bSSS = new Byte[max];
                    fileS.Read(bSSS, 0, max);
                    
                    String sha512ID = SHA512(bSSS);
                    
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

                    //String pathFile = Path.GetDirectoryName(args[0]);                    

                    FileInfo fI = new FileInfo(args[0]);

                    using (var fs = new FileStream(fI.DirectoryName + "\\" + fI.Name + ".bpc", FileMode.Create, FileAccess.Write))
                    {
                        fs.WriteByte(Convert.ToByte(P));
                        fs.WriteByte(Bp);
                        Byte[] BsSha = Encoding.UTF8.GetBytes(sha512ID);
                        foreach (Byte BSha in BsSha)
                        {
                            fs.WriteByte(BSha);
                        }

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
            catch(Exception e)
            {
                Console.WriteLine("EEEEEEEEEERROR =(");
                Console.Beep();
                Thread.Sleep(10000);
            }
        }
    }
}
