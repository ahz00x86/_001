using Microsoft.VisualBasic.FileIO;
using System.Buffers;
using System.IO;
using System.Net;

namespace consoleAPP001
{
    internal class Program
    {
        /// <summary>LOCAL CONSOLE</summary>
        /// <param name="args">string[];option;url/path;</param>
        static void Main(string[] args)
        {
            // MAIN

            string c0 = args[0].Substring(0, 1);
            string c1 = args[0].Substring(1, 1);
            string urlPath = args[1];

            if (c0 == c1 && c0 == "A")
            {
                Console.WriteLine(fR("A2", urlPath));
            }
            else if (c0 != c1 && c0 == "A" && c1 == "2")
            {
                string[] links = getLinks(fR("A2", urlPath));

                foreach (string link in links)
                {
                    Console.WriteLine(link);
                }
            }


            // FIN
        }

        static string fR(string option, string urlPath)
        {
            // mejor un if else else if if else... seguro,
            if (option == "A0")
            {

            }
            else if (option == "A1")
            {

            }
            else if (option == "A2")
            {
                // 1er problema: captchas < NO RESOLVER
                WebClient wC = new WebClient();

                if (Path.HasExtension(urlPath))
                {
                    wC.DownloadFile(urlPath, "\file.apz");
                    // 2do problema: wallet/monederos/blockchainsNFTs
                }
                else
                {
                    string html = wC.DownloadString(urlPath);
                    return html;
                }
                // 3er problema: usuario/contraseña (SIN CAPTCHA)
            }
            else if (option == "A3")
            {

            }
            else if (option == "A4")
            {

            }
            else if (option == "A5")
            {

            }
            else if (option == "A6")
            {

            }
            else if (option == "A7")
            {

            }
            else if (option == "A8")
            {

            }
            else
            {

            }

            return "https://nft.io/profile/ahz00x86/listed";

        }

        static string[] getLinks(string html)
        {
            //
            try
            {
                List<string> sLinks = new List<string>();
                sLinks.AddRange(html.Split(new string[10] { "'", "\"", " ", "(", ")", "[", "]", "\t", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries));

                sLinks.RemoveAll(x => !x.Contains('.'));

                List<string> links = new List<string>();
                foreach (string sLink in sLinks)
                {
                    if (sLink.ToLower().Contains("http") || sLink.ToLower().Contains("www"))
                    {
                        links.Add(sLink);
                        continue;
                    }

                    string[] lParts = sLink.Split(new string[1] { "." }, StringSplitOptions.RemoveEmptyEntries);

                    for (Int32 i = 1; i < lParts.Length; i++)
                    {
                        if (lParts[i].Length == 2 || lParts[i].Length == 3 || lParts[i].Length == 4)
                        {
                            links.Add(sLink);
                            break;
                        }
                    }


                }


                return links.ToArray();
            }
            catch(Exception ex)
            {

            }


            return new string[1] { "https://nft.io/profile/ahz00x86/listed" };

        }

    }
}