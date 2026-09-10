using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Text;
using System.IO;
using System.Threading;
using NAudio.Wave;
using NAudio.Lame;
using Microsoft.Speech.Synthesis;

namespace textoamp3
{
    public static class CONVERTS
    {
        public async static void CONVERT(String mail, Guid id, String titulo, String texto, String idioma, Boolean publicidad, HttpServerUtilityBase server)
        {
            try
            {
                // REMAKE TEXTO
                String[] texts = publicidad ? new String[1] { texto.Substring(0, (texto.Length > 500 ? 500 : texto.Length)) } : texto.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
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
                        DeleteErrorMp3(id);
                        String bodyMail3 = "<p>Su texto con título " + titulo + " no se ha podido convertir</p>"
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

                //Utils.CheckAddBinPath();
                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=conversordetextoamp3;AccountKey=FGoegFxvAFiS9H9inR11fR8n6RcV5I8bPLawC2KN8Eff/UpljVXtPGQhq2W6uAYbdqy8wmxuVou6xx52oU8GuA==;EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference("descargas");
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(id.ToString() + ".mp3");
                String idMP3 = server.MapPath(String.Concat(@"~/App_Data/", id.ToString(), ".mp3"));
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

                using (addclickEntities entities = new addclickEntities())
                {
                    // Añadimos descarga
                    Users_MP3s prices = entities.Users_MP3s.Where(x => x.id == id).Single();
                    prices.precio =  publicidad ? 0 : 1;

                    entities.SaveChanges();
                }

                // MANDAR MAIL

                String bodyMail4 = "<p>Su texto con título " + titulo + " está disponible.</p>"
                            + "<p>Gracias por su tiempo</p>"
                            + "<p>universalsemiotics.com</p>";
                Utils.SendMail(mail, "TEXT OK - universalsemiotics.com", bodyMail4);
            }
            catch (Exception ex)
            {
                DeleteErrorMp3(id);
                String bodyMail = "<p>Su texto con título " + titulo + " no se ha podido convertir</p>"
                    + "<p>Ha ocurrido un error en el proceso de conversión.</p>"
                    + "<p>Gracias por su tiempo</p>";
                Utils.SendMail(mail, "ERROR - universalsemiotics.com", bodyMail);
                Utils.SendMail("apzyx@yahoo.com", "TEXT KO - universalsemiotics.com", bodyMail + ex.ToString());
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

        public static void RETURNTEXTPRICES(Guid id, String mail, Decimal textPrices)
        {
            using (addclickEntities entities = new addclickEntities())
            {
                // Añadimos descarga
                Users_MP3s remove = entities.Users_MP3s.Where(x => x.id == id).Single();
                entities.Users_MP3s.Remove(remove);

                // Realizamos conversión en caso de saldo correcto
                Users_Data2 dtDB = entities.Users_Data2.Where(x => x.email == mail).SingleOrDefault();
                dtDB.prices += textPrices;

                entities.SaveChanges();
            }
        }
    }
}