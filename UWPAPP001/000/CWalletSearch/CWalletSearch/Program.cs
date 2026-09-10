using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpCompress.Common;

namespace CWalletSearch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainSEARCH(args);
            Console.ReadKey();
        }

        static bool IsCompressed(string filePath)
        {
            try
            {
                using (var archive = ArchiveFactory.Open(filePath))
                {
                    return true;
                }
            }
            catch (InvalidOperationException ex)
            {
                // Si se lanza una excepción de operación inválida, podría ser debido a una contraseña
                if (ex.Message.ToLower().Contains("password"))
                {
                    Console.WriteLine(filePath);
                    Console.WriteLine(ex.Message);                    
                }
                return false;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

        static void Decompress(string filePath, string extractPath)
        {
            using (var archive = ArchiveFactory.Open(filePath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(extractPath, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }
            }
        }

        static async void MainSEARCH(string[] args)
        {
            List<String> mainPaths = Directory.GetDirectories(@"E:\SEARCH").ToList();
            Int32 cI = 0;
            Int32 cF = 0;
            while (mainPaths.Count > 0)
            {
                List<String> morePaths = new List<String>();

                foreach (String path in mainPaths)
                {
                    morePaths.AddRange(Directory.GetDirectories(path));

                    String[] filesDir = Directory.GetFiles(path);

                    int maxDegreeOfParallelism = 100; // Número máximo de tareas en paralelo
                    SemaphoreSlim semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

                    List<Task> tasks = new List<Task>();

                    foreach (String filePath in filesDir)
                    {





                        cI++;
                        Int32 d2d2 = cI;

                        await semaphore.WaitAsync();

                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                //Console.WriteLine($"Tarea {filePath} ejecutada en el hilo {Task.CurrentId}");
                                //await Task.Delay(1000); // Simula trabajo

                                try
                                {

                                    //Console.WriteLine(cF.ToString() + "_" + d2d2.ToString() + "_" + filePath);
                                    FileInfo fileI = new FileInfo(filePath);


                                    if (true) //fileI.Length < 666000000) // && (filePath.ToLower().Contains("wallet") || filePath.ToLower().Contains(".pps")))
                                    {
                                        String fileName = Path.GetFileName(filePath);

                                        if ((fileI.Extension.ToLower() == ".zip" || fileI.Extension.ToLower() == ".rar") && IsCompressed(filePath))
                                        {

                                        }

                                            //if ((fileI.Extension.ToLower() == ".zip" || fileI.Extension.ToLower() == ".rar") && IsCompressed(filePath))
                                            //{
                                            //    string extractPath = @"E:\ZIP\Z" + d2d2.ToString();
                                            //    Directory.CreateDirectory(extractPath);
                                            //    Decompress(filePath, extractPath);
                                            //    //Console.WriteLine(filePath);
                                            //    //Console.WriteLine("ZIP OK.");
                                            //    morePaths.Add(extractPath);
                                            //}
                                            //else
                                            //{

                                            //    try
                                            //    {
                                            //        String textF = File.ReadAllText(filePath).ToLower();
                                            //        if (textF.Contains("bestblock_nomerkle"))
                                            //        {
                                            //            //Task.Run(() =>
                                            //            //{
                                            //            File.Copy(filePath, @"C:\Users\apzyx\OneDrive\Escritorio\search\holi" + d2d2.ToString() + ".xax");
                                            //            File.AppendAllText(@"C:\Users\apzyx\OneDrive\Escritorio\search\holi" + d2d2.ToString() + ".xai", "\n\r");
                                            //            File.AppendAllText(@"C:\Users\apzyx\OneDrive\Escritorio\search\holi" + d2d2.ToString() + ".xai", filePath);

                                            //            Console.WriteLine("VVV");
                                            //            Console.WriteLine(filePath);
                                            //            cF++;
                                            //            //});
                                            //        }


                                            //    }
                                            //    catch (Exception ex)
                                            //    {
                                            //        Console.WriteLine(ex.ToString());
                                            //        File.AppendAllText(@"C:\Users\apzyx\OneDrive\Escritorio\search\errors\holi" + d2d2.ToString() + ".err", "\n\r" + ex.ToString());
                                            //    }
                                            //}

                                            GC.Collect();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                    File.AppendAllText(@"C:\Users\apzyx\OneDrive\Escritorio\search\errors\holi" + d2d2.ToString() + ".err", "\n\r" + ex.ToString());
                                }
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }));
                    }

                    await Task.WhenAll(tasks);

                }

                mainPaths = morePaths;
            }

            Console.WriteLine("___F_I_N___");
            Console.ReadKey();
        }
    }
}
