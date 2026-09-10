using Microsoft.Speech.Synthesis;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NAudio.Lame;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using textoamp3;

namespace WcfConversorDeTextoAmp3
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        public void GetData(String udk, String mail, Guid idPC, String titulo, String texto, Boolean publi)
        {
            if (udk != "gapFf@1WjRmH0xA") return;
            Task.Run(() => CONVERT(mail, idPC, titulo, texto, publi));
        }

        private async static void CONVERT(String mail, Guid id, String titulo, String texto, Boolean publicidad)
        {
            try
            {
                // REMAKE TEXTO
                String[] texts = texto.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (publicidad)
                {
                    // Obtener publicidad
                    String publi = " Conversor de texto a mp3.com, convierte texto en mp3, su opción GRATUITA incluye esta publicidad, podrá pasar archivos sin publicidad contratando 1 día o 1 mes de uso sin publicidad. ";
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
                        DeleteErrorMp3(id);
                        String bodyMail3 = "<p>Su texto con título " + titulo + "no se ha podido convertir</p>"
                            + "<p>Un texto mayor de 8000 caracteres debe poseer algún signo de punto (.)</p>"
                            + "<p>Gracias por su tiempo</p>"
                            + "<p>conversorDeTextoAmp3.com</p>";
                        Utils.SendMail(mail, "TEXT KO - conversorDeTextoAmp3.com", bodyMail3);
                        return;
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

                Utils.CheckAddBinPath();
                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=conversordetextoamp3;AccountKey=tFgSya3UaB9flnsX1pyXz95TQlczotuR+0HmYE5yDui7nDV4H7/DGHcGtS6NY1o4F7ASrBJllIBmjXoF9bffbQ==;EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference("descargas");
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(id.ToString() + ".mp3");
                using (MemoryStream containerStream = new MemoryStream())
                {

                    foreach (String s in strings)
                    {
                        using (SpeechSynthesizer reader = new SpeechSynthesizer())
                        {
                            //set some settings
                            reader.Volume = 100;
                            reader.Rate = 0; //medium
                            
                            reader.SelectVoice(reader.GetInstalledVoices(new System.Globalization.CultureInfo("es-ES"))[0].VoiceInfo.Name);
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
                using (addclickEntities entities = new addclickEntities())
                {
                    // Añadimos descarga
                    Users_MP3s prices = entities.Users_MP3s.Where(x => x.id == id).Single();
                    prices.precio = 1;

                    entities.SaveChanges();
                }

                // MANDAR MAIL

                String bodyMail4 = "<p>Su texto con título " + titulo + " está disponible.</p>"
                            + "<p>Gracias por su tiempo</p>"
                            + "<p>conversorDeTextoAmp3.com</p>";
                Utils.SendMail(mail, "TEXT OK - conversorDeTextoAmp3.com", bodyMail4);
            }
            catch (Exception ex)
            {
                DeleteErrorMp3(id);
                String bodyMail = "<p>Su texto con título " + titulo + "no se ha podido convertir</p>"
                                                    + "<p>Ha ocurrdio un error en el proceso de conversión, se le ha reintegrado el coste de su archivo.</p>"
                                                    + "<p>Gracias por su tiempo</p>";
                Utils.SendMail(mail, "TEXT KO - conversorDeTextoAmp3.com", bodyMail);
                Utils.SendMail("apzyx@yahoo.com", "TEXT KO - conversorDeTextoAmp3.com", bodyMail + ex.ToString());
            }
        }

        private static void DeleteErrorMp3(Guid id)
        {
            using (addclickEntities entities = new addclickEntities())
            {
                // Añadimos descarga
                Users_MP3s uMp3Delete = entities.Users_MP3s.Where(x => x.id == id).Single();
                entities.Users_MP3s.Remove(uMp3Delete);

                entities.SaveChanges();
            }
        }
    }
}
