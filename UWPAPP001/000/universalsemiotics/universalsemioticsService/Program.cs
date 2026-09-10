using Microsoft.Speech.Synthesis;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NAudio.Lame;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace universalsemioticsService
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                hola();

                Console.WriteLine("DURMIENDO 1m");
                Thread.Sleep(60000);
            }
        }

        public static void hola()
        {
            String URIHola = "https://www.universalsemiotics.com/service/Hola";
            String URIAdios = "https://www.universalsemiotics.com/service/Adios";            
                        
            try
            {
                var vmH = new { pH = "hola@123.us"};
                var dataStringHola = JsonConvert.SerializeObject(vmH);

                string HtmlResultHola = String.Empty;
                using (WebClient wc = new WebClient())
                {
                    Console.WriteLine("... HOLA ...");
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";

                    HtmlResultHola = wc.UploadString(URIHola, "POST", dataStringHola);
                }

                if (HtmlResultHola != String.Empty)
                {
                    List<HolaModel> procesos = System.Text.Json.JsonSerializer.Deserialize<List<HolaModel>>(HtmlResultHola);

                    foreach (HolaModel p in procesos)
                    {
                        Console.WriteLine("OBT TEXT: " + p.Id.ToString());
                        String texto = ObtenerTexto(p.Id);

                        Console.WriteLine("CONV TEXT: " + p.Id.ToString());
                        if (Convertir(p.Id, p.Idioma, texto))
                        {
                            Console.WriteLine("CONV OK: " + p.Id.ToString());


                            var vmA = new { id = p.Id.ToString(), pA = "adios@123.us"};
                            var dataStringAdios = JsonConvert.SerializeObject(vmA);
                            String HtmlResultAdios = String.Empty;
                            using (WebClient wc = new WebClient())
                            {
                                Console.WriteLine("... ADIOS ...");
                                wc.Headers[HttpRequestHeader.ContentType] = "application/json";

                                HtmlResultAdios = wc.UploadString(URIAdios, "POST", dataStringAdios);
                            }

                            Console.WriteLine("RES: " + HtmlResultAdios);

                        }
                        else
                        {
                            Console.WriteLine("ERROR CONV " + p.Id);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("EX: " + ex.ToString());

                Thread.Sleep(300000);
            }

        }
                    
        

        public static String ObtenerTexto(Guid id)
        {
            CloudBlobClient blobClient;
            CloudBlobContainer blobContainer;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=universalsemiotics;AccountKey=V/6Vo6+nJKM5/xQDCqABNrmu9c0jvwG/a2GpHyrTLQvNNFyXjzzB95ObwNdCnVYQjJkZaZJcCgYY4ZZ5v5ozDA==;EndpointSuffix=core.windows.net");
            blobClient = storageAccount.CreateCloudBlobClient();
            blobContainer = blobClient.GetContainerReference("docsfree");
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(id.ToString() + ".us");

            using (Stream blobStream = blob.OpenRead())
            {
                using (StreamReader reader = new StreamReader(blobStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static Boolean Convertir(Guid id, String idioma, String texto)
        {
            Boolean publicidad = true;
            try
            {
                // REMAKE TEXTO
                String[] texts = texto.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (texts.Length == 0) { texts = new string[] { String.Empty }; }
                if (publicidad)
                {
                    // Obtener publicidad
                    String publi =
                        idioma == "es-ES" ? " universal semiotics.com, convierte texto en mp3, su opción GRATUITA incluye esta publicidad. " :
                        idioma == "en-US" ? " universal semiotics.com, convert text to mp3, your FREE option includes this advertising. " :
                        idioma == "fr-FR" ? " universal semiotics.com, convertir le texte en mp3, votre option GRATUITE comprend cette publicité. " :
                        idioma == "de-DE" ? " universal semiotics.com, konvertieren Text in mp3, Ihre KOSTENLOSE Option enthält diese Werbung. " :
                        idioma == "it-IT" ? " universal semiotics.com, convertire il testo in mp3, la tua opzione GRATUITA include questa pubblicità. " :
                        " universal semiotics.com, convierte texto en mp3, su opción GRATUITA incluye esta publicidad. ";
                    texts[0] = String.Concat(publi, texts[0]);
                    Int32 textLength = 0;
                    Int32 lengthPubliUnits = 1;
                    for (Int32 i = 0; i < texts.Length; i++)
                    {
                        textLength += texts[i].Length;
                        if (textLength > 3000 * lengthPubliUnits)
                        {
                            lengthPubliUnits++;
                            texts[i] += publi;
                        }
                    }
                    texts[texts.Length - 1] += publi;
                }

                List<String> strings = new List<String>();
                String temp = String.Empty;
                for (Int32 i = 0; i < texts.Length; i++)
                {
                    if (texts[i].Length > 8400)
                    {
                        // MANDAR EMAIL PUNTOS                        
                        String bodyMail3 = "<p>Su texto con título " + id.ToString() + " no se ha podido convertir</p>"
                            + "<p>Un texto mayor de 8000 caracteres debe poseer algún signo de punto (.)</p>"
                            + "<p>Gracias por su tiempo</p>"
                            + "<p>conversorDeTextoAmp3.com</p>";
                        SendMail("apzyx@yahoo.com", "TEXT KO .. - conversorDeTextoAmp3.com", bodyMail3);
                        return false;
                    }

                    if (temp.Length + texts[i].Length <= 8400)
                    {
                        temp = String.Concat(temp, ". ", texts[i]);
                    }
                    else
                    {
                        strings.Add(temp);
                        temp = texts[i];
                    }
                }
                strings.Add(temp);

                //Utils.CheckAddBinPath();
                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=universalsemiotics;AccountKey=V/6Vo6+nJKM5/xQDCqABNrmu9c0jvwG/a2GpHyrTLQvNNFyXjzzB95ObwNdCnVYQjJkZaZJcCgYY4ZZ5v5ozDA==;EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference("mp3free");
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(id.ToString() + ".mp3");
                String idMP3 = String.Concat(@"c:\mp3s\", id.ToString(), ".mp3");
                using (FileStream containerStream = new FileStream(idMP3, FileMode.Create))
                {

                    foreach (String s in strings)
                    {
                        using (SpeechSynthesizer reader = new SpeechSynthesizer())
                        {
                            //set some settings
                            reader.Volume = 100;
                            reader.Rate = 0; //medium

                            reader.SelectVoice(reader.GetInstalledVoices(new System.Globalization.CultureInfo(idioma))[0].VoiceInfo.Name);
                            //reader.SelectVoice("Microsoft Helena Desktop - Spanish (Spain)");
                            //reader.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.Adult, 0, new System.Globalization.CultureInfo("es-ES"));
                            //save to memory stream
                            using (MemoryStream ms = new MemoryStream())
                            {
                                reader.SetOutputToWaveStream(ms);
                                reader.Speak(s);
                                ms.Position = 0;

                                using (var retMs = new MemoryStream())
                                using (var rdr = new WaveFileReader(ms))
                                using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, LAMEPreset.VBR_90))
                                {
                                    rdr.CopyTo(wtr);
                                    retMs.Position = 0;
                                    retMs.CopyTo(containerStream);
                                }
                            }
                        }
                    }
                    containerStream.Position = 0;
                    //containerStream.Flush();
                    blob.UploadFromStream(containerStream);
                    // PROCESO COGNITIVE FIN
                }

                File.Delete(idMP3);

                return true;
            }
            catch (Exception ex)
            {                
                String bodyMail = "<p>Su texto con título " + id.ToString() + " no se ha podido convertir</p>"
                    + "<p>Ha ocurrido un error en el proceso de conversión.</p>"
                    + "<p>Gracias por su tiempo</p>";                
                SendMail("apzyx@yahoo.com", "TEXT KO - universalsemiotics.com", bodyMail + ex.ToString());

                Console.Write("CONV EX: " + ex.ToString());

                return false;
            }
        }

        public static void SendMail(String emailTo, String subject, String body)
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient())
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("gestion@universalsemiotics.com");
                        mail.To.Add(emailTo);
                        mail.Subject = subject;
                        mail.Body = body;
                        mail.IsBodyHtml = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("gestion@universalsemiotics.com", "pruebasDomingoYLunes332@");
                        smtp.Host = "smtp.ionos.es";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("EMAIL EX:" + ex.ToString());
            }
        }

        public static void Serializa()
        {
            #region voices API
            //try
            //{
            //    var key = ConfigurationManager.AppSettings["cogla"];
            //    var url = "https://westeurope.customvoice.api.speech.microsoft.com/api/texttospeech/v3.0/longaudiosynthesis/voices";

            //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            //    request.Headers.Add("Ocp-Apim-Subscription-Key", key);
            //    request.Method = "GET";
            //    String test = String.Empty;
            //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            //    {
            //        Stream dataStream = response.GetResponseStream();
            //        StreamReader reader = new StreamReader(dataStream);
            //        test = reader.ReadToEnd();
            //        reader.Close();
            //        dataStream.Close();
            //    }
            //}
            //catch (Exception ex)
            //{

            //}

            //function alignModal() {
            //    var modalDialog = $("#modalBody");
            //    // Applying the top margin on modal to align it vertically center
            //    modalDialog.css("margin-top", Math.max(0, ($(window).height() - modalDialog.height()) / 2));
            //}

            //// Align modal when it is displayed
            //$(".modal").on("shown.bs.modal", alignModal);

            //// Align modal when user resize the window
            //$(window).on("resize", function () {
            //    alignModal();
            //});

            #endregion
        }
    }

    public class HolaModel
    {
        /// <summary>Obtiene o establece el id del proceso hola</summary>
        public Guid Id { get; set; }

        /// <summary>Obtiene o establece el idioma del proceso hola</summary>
        public String Idioma { get; set; }
    }
}
