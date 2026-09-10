using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blobsController
{
    class Program
    {
        static void Main(string[] args)
        {
              String s = getEmailList();
            //removeBlobs();
        }

        private static string getEmailList()
        {
            List<String> emails = new List<String>();
            using (addclickEntities entities = new addclickEntities())
            {
                emails = entities.AspNetUsers.Where(x => x.EmailConfirmed).Select(x => x.Email).ToList();
            }

            String body = "<h1>Información Conversor de texto a mp3</h1>";
            body += "<p>Hola! estás recibiendo este correo electrónico como usuario de conversordetextoamp3</p>";
            body += "<p>La web ha migrado a <a target='_blank' href='https://www.universalsemiotics.com'>www.universalsemiotics.com</a></p>";
            body += "<p>Las cuentas de usuario no han sido integradas, por lo que si quiere participar del nuevo proyecto deberá crear una cuenta nueva. Los datos de usuario serán borrados y este será el último correo que recibirá.</p>";
            body += "<p>La versión sin publicidad ha sido sustituida por la <b>Conversión Profesional</b>, añadiendo más voces, integrando los ultimos avances en conversión de Microsot Cognitive Services</p>";
            body += "<p>Se ha elimando el límite de caracteres en la <b>Conversión GRaTuiTa</b></p>";
            body += "<p>Hemos añadido tambien funcionalidad para que pueda integrar su publicidad en nuestra web directamente desde ella mediante banners de publicidad.</p>";
            body += "<p>Creemos que la nueva versión podrá ser de más utilidad.</p>";
            body += "<p>Os esperamos en <a target='_blank' href='https://www.universalsemiotics.com'>www.universalsemiotics.com</a></p>";
            body += "<p>Para cualquier problema, idea o sugerencia pueden ponerse en contacto conmigo.<br/>Disculpen las molestias,</p>";
            body += "<p>Gracias por tu tiempo,<br/>Pérez Sánchez, Pablo<br/>+34 645263898<br/>apzyx@yahoo.com</p>";
            textoamp3.Utils.SendMail("apzyx@yahoo.com", "Ahora, conversión GRaTuiTa con anuncios", body);
            foreach (String s in emails)
            {
                Thread.Sleep(60000);
                textoamp3.Utils.SendMail(s, "Migración a www.universalsemiotics.com", body);
            }

            textoamp3.Utils.SendMail("apzyx@yahoo.com", "Migración a www.universalsemiotics.com", body);

            return "";
        }

        private static void removeBlobs()
        {
            List<Users_MP3s> mp3s = new List<Users_MP3s>();
            DateTime dt16 = DateTime.Now.AddDays(-16);
            using (addclickEntities entities = new addclickEntities())
            {
                mp3s = entities.Users_MP3s.Where(x => !x.borrado && x.fecha < dt16).ToList();


                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=conversordetextoamp3;AccountKey=FGoegFxvAFiS9H9inR11fR8n6RcV5I8bPLawC2KN8Eff/UpljVXtPGQhq2W6uAYbdqy8wmxuVou6xx52oU8GuA==;EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference("descargas");
                foreach (Users_MP3s mp3 in mp3s)
                {
                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(mp3.id.ToString() + ".mp3");
                    try
                    {
                        blob.Delete();
                    }
                    catch (Exception yaBorrado)
                    {

                    }
                    mp3.borrado = true;
                }
                entities.SaveChanges();
            }
        }

        private static void removeBlobsEmail(String mail)
        {
            List<Users_MP3s> mp3s = new List<Users_MP3s>();
            using (addclickEntities entities = new addclickEntities())
            {
                mp3s = entities.Users_MP3s.Where(x => x.mail == mail).ToList();


                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=conversordetextoamp3;AccountKey=FGoegFxvAFiS9H9inR11fR8n6RcV5I8bPLawC2KN8Eff/UpljVXtPGQhq2W6uAYbdqy8wmxuVou6xx52oU8GuA==;EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference("descargas");
                foreach (Users_MP3s mp3 in mp3s)
                {
                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(mp3.id.ToString() + ".mp3");
                    try
                    {
                        blob.Delete();
                    }
                    catch (Exception yaBorrado)
                    {

                    }
                    mp3.borrado = true;
                }
                entities.SaveChanges();
            }
        }
    }
}
