using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace universalsemiotics
{
    public static class CONVERT
    {
        public async static void CONVERTm(String mail, Guid id, Guid idUser, String titulo, String texto, String voz, HttpServerUtilityBase server)
        {
            try
            {
                // REMAKE TEXTO
                String[] texts = texto.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (texts.Length == 0) { texts = new string[] { String.Empty }; }
                List<String> strings = new List<String>();
                String temp = String.Empty;
                for (Int32 i = 0; i < texts.Length; i++)
                {
                    if (texts[i].Length > 5000)
                    {
                        // MANDAR EMAIL PUNTOS
                        //DeleteErrorMp3(id);
                        String bodyMail3 = "<p>Su texto con título " + titulo + " no se ha podido convertir</p>"
                            + "<p>Un texto mayor de 8000 caracteres debe poseer algún signo de punto (.)</p>"
                            + "<p>Gracias por su tiempo</p>"
                            + "<p>conversorDeTextoAmp3.com</p>";
                        Utils.SendMail("apzyx@yahoo.com", "TEXT KO - conversorDeTextoAmp3.com", bodyMail3);
                        return;
                    }

                    if (temp.Length + texts[i].Length <= 5000)
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

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=universalsemiotics;AccountKey=" + ConfigurationManager.AppSettings["blb"] + ";EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference("mp3pro");
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(id.ToString() + ".mp3");
                String idMP3 = server.MapPath(String.Concat(@"~/App_Data/", id.ToString(), ""));
                String idMP3R = server.MapPath(String.Concat(@"~/App_Data/", id.ToString(), "R"));


                SpeechConfig config = SpeechConfig.FromSubscription(ConfigurationManager.AppSettings["cog"], "northeurope");
                config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio24Khz48KBitRateMonoMp3);
                config.SpeechSynthesisVoiceName = voz;

                using (FileStream containerStream = new FileStream(idMP3R, FileMode.Create))
                {
                    foreach (String s in strings)
                    {                        
                        using (var fileOutput = AudioConfig.FromWavFileOutput(idMP3))
                        using (SpeechSynthesizer reader = new SpeechSynthesizer(config, fileOutput))
                        {
                            var result = await reader.SpeakTextAsync(s);
                        }

                        using (FileStream f = new FileStream(idMP3, FileMode.Open))
                        {
                            f.CopyTo(containerStream);
                        }
                        File.Delete(idMP3);
                    }                    
                }

                blob.UploadFromFile(idMP3R);
                File.Delete(idMP3R);


                using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
                {
                    // Añadimos descarga
                    Mp3Pro mp3 = bbdd.Mp3Pro.Where(x => x.Id == id).Single();
                    mp3.fechaFin = DateTime.Now;
                    mp3.estado = true;

                    bbdd.SaveChanges();
                }

                // MANDAR MAIL

                String bodyMail4 = "<p>Su texto con título " + titulo + " está disponible.</p>"
                            + "<p>Gracias por su tiempo</p>"
                            + "Puede descargarlo aquí <a href='https://www.universalsemiotics.com/es/Descargas'>www.universalsemiotics.com/es/Descargas</a>";
                Utils.SendMail(mail, "OK - universalsemiotics.com", bodyMail4);
            }
            catch (Exception ex)
            {                
                String bodyMail = "<p>Su texto con título " + titulo + " no se ha podido convertir</p>"
                    + "<p>Ha ocurrido un error en el proceso de conversión.</p>"
                    + "<p>Gracias por su tiempo</p>";
                Utils.SendMail(mail, "ERROR - universalsemiotics.com", bodyMail);
                Utils.SendMail("apzyx@yahoo.com", "TEXT KO - universalsemiotics.com", bodyMail + ex.ToString());
            }
        }
        private static void DeleteErrorMp3(Guid id, Guid userId, Int64 caracteres)
        {
            //////using (addclickEntities entities = new addclickEntities())
            //////{
            //////    // Añadimos descarga
            //////    Users_MP3s uMp3Delete = entities.Users_MP3s.Where(x => x.id == id).Single();
            //////    entities.Users_MP3s.Remove(uMp3Delete);

            //////    entities.SaveChanges();
            //////}
        }
    }
}