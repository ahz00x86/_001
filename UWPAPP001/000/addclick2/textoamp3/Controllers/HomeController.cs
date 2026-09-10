using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using textoamp3.Models;
using System.IO.Compression;
using System.Xml;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using System.Net;

namespace textoamp3.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
                
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public new ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        
        [AllowAnonymous]
        public ActionResult Acerca()
        {
            ViewBag.Message = "Conversor de texto a mp3.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Aviso_Legal()
        {
            return View();
        }

        [HttpGet]
        public ActionResult textoamp3()
        {
            if (Request.IsAuthenticated)
            {
                string userId = User.Identity.GetUserId();
                var user = UserManager.FindById(userId);
                String mail = user.Email.ToLower();

                ViewBag.email = mail;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> textoamp3(String email, String titulo, String texto, String idioma)
        {
            // PROCESO COGNITIVE INI
            Guid idPC = Guid.NewGuid();
            String mail;
            if (Request.IsAuthenticated)
            {
                string userId = User.Identity.GetUserId();
                var user = UserManager.FindById(userId);
                mail = user.Email.ToLower();
            }
            else
            {
                mail = email.ToLower();
            }

            Boolean publi = true;
            using (addclickEntities entities = new addclickEntities())
            {
                AspNetUser uaspDB = entities.AspNetUsers.Where(x => x.Email == mail).SingleOrDefault();
                if (uaspDB == null)
                {
                    ApplicationUser user = new ApplicationUser { UserName = mail, Email = mail };
                    String password = Utils.RandomString(11);
                    password = password.Insert(5, "@") + "0xA";
                    var result = await UserManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {

                        entities.Users_Data2.Add(new Users_Data2() { email = mail, prices = 0 });
                        entities.SaveChanges();
                        
                        String bodyMail = "<p>Acceda a su cuenta con su email y esta contraseña: " + password + "</p>"
                        + "<br/><p>Gracias por su tiempo</p>"
                        + "<p>conversorDeTextoAmp3.com</p>";
                        Utils.SendMail(mail, "ACCEDA - conversorDeTextoAmp3.com", bodyMail);
                    }
                }

                DateTime hourMenos = DateTime.Now.AddHours(-1);
                if (0 < entities.Users_MP3s.Where(x => x.mail == mail && x.precio < 0 && x.fecha > hourMenos).Count())
                {
                    ViewBag.textoamp3e = "Sólo puede convertir un archivo a la vez, cuando finalize la conversión en proceso podrá pasar otro.";
                    return View();
                }

                // Realizamos conversión en caso de saldo correcto
                Users_Data2 dtDB = entities.Users_Data2.Where(x => x.email == mail).SingleOrDefault();
                publi = dtDB.fecha.HasValue && dtDB.fecha.Value > DateTime.Now ? false : true;
                if (publi)
                {
                    DateTime hourMenosDia = DateTime.Now.AddDays(-5);
                    if (0 < entities.Users_MP3s.Where(x => x.mail == mail && x.precio == 0 && x.fecha > hourMenosDia).Count())
                    {
                        ViewBag.textoamp3e = "Sólo puede convertir 1 archivo gratuito cada 5 días, pruebe la opción quitar publicidad...";
                        return View();
                    }
                }

                // Añadimos descarga
                entities.Users_MP3s.Add(new Users_MP3s() { mail = mail, id = idPC, titulo = titulo, url = "https://conversordetextoamp3.blob.core.windows.net/descargas/" + idPC.ToString() + ".mp3", fecha = DateTime.Now, precio = -1 });
                entities.SaveChanges();                
            }

            try
            {
                Task.Run(() => CONVERTS.CONVERT(mail, idPC, titulo, texto, idioma, publi, Server));                
            }
            catch (Exception ex)
            {
                DeleteErrorMp3(idPC);
                String bodyMail = "<p>Su texto con título " + titulo + " no se ha podido convertir</p>"
                    + "<p>Ha ocurrdio un error en el proceso de conversión.</p>"
                    + "<p>Gracias por su tiempo</p>";
                Utils.SendMail(mail, "TEXT KO - conversorDeTextoAmp3.com", bodyMail);
                Utils.SendMail("apzyx@yahoo.com", "TEXT KO - conversorDeTextoAmp3.com", bodyMail + ex.ToString());
            }
            ViewBag.StatusMessage = "EN PROCESO DE CONVERSIóN, puede tardar más de una hora, le enviaremos un correo electrónico cuando finalize, también puede consultarlo en DESCARGAS.";

            ViewBag.textoamp3 = String.Empty;
            return View();
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

        public ActionResult DESCARGAS()
        {
            DESCARGASModel model = new DESCARGASModel();
            using (addclickEntities entities = new addclickEntities())
            {
                DateTime fechaDESCARGAS = DateTime.Now.AddDays(-15);
                String mail = User.Identity.Name;
                List<DESCARGA> mp3s = entities.Users_MP3s.Where(x => x.mail == mail && !x.borrado && x.fecha > fechaDESCARGAS).OrderByDescending(x=> x.fecha).Select(x => new DESCARGA() { Id = x.id, Titulo = x.titulo, Url = x.url, IsDemo = x.precio == 0, Fecha = x.fecha, IsConverting = x.precio < 0 }).ToList();
                model.DESCARGAS = mp3s;
            }

            return View(model);            
        }

        public FileStreamResult Download(Guid id)
        {
            DESCARGASModel model = new DESCARGASModel();
            using (addclickEntities entities = new addclickEntities())
            {
                DateTime fechaDESCARGAS = DateTime.Now.AddDays(-15);
                String mail = User.Identity.Name;
                Users_MP3s mp3 = entities.Users_MP3s.Where(x => x.mail == mail && !x.borrado && x.fecha > fechaDESCARGAS && x.id == id).SingleOrDefault();

                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=conversordetextoamp3;AccountKey=FGoegFxvAFiS9H9inR11fR8n6RcV5I8bPLawC2KN8Eff/UpljVXtPGQhq2W6uAYbdqy8wmxuVou6xx52oU8GuA==;EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference("descargas");
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(id.ToString() + ".mp3");

                Stream blobStream = blob.OpenRead();
                return File(blobStream, blob.Properties.ContentType);
            }            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult getEPUBText(byte[] epubFile)
        {
            try
            {
                Guid id = Guid.NewGuid();
                if (Request.Files[0].FileName.ToLower().Contains(".epub"))
                {                    
                    String idAD = Server.MapPath(String.Concat(@"~/App_Data/", id.ToString(), "/"));
                    ZipArchive zA = new ZipArchive(Request.Files[0].InputStream);
                    zA.ExtractToDirectory(idAD);

                    String container = Server.MapPath(String.Concat(@"~/App_Data/", id.ToString(), "/META-INF/container.xml"));
                    XmlReader xmlRc = XmlReader.Create(container);
                    xmlRc.MoveToContent();

                    String dir = String.Empty;

                    while (xmlRc.Read())
                    {
                        if (xmlRc.NodeType == XmlNodeType.Element)
                        {
                            if (xmlRc.Name == "rootfile")
                            {
                                dir = xmlRc.GetAttribute("full-path");
                                break;
                            }
                        }
                    }
                    xmlRc.Close();
                    xmlRc.Dispose();

                    StringBuilder sB = new StringBuilder();
                    String content = Server.MapPath(String.Concat(@"~/App_Data/", id.ToString(), "/", dir));
                    FileInfo fI = new FileInfo(content);
                    String dirContent = fI.DirectoryName;

                    XmlReader xmlRi = XmlReader.Create(content);
                    xmlRi.MoveToContent();
                    String dirCapitulo = String.Empty;
                    while (xmlRi.Read())
                    {
                        if (xmlRi.NodeType == XmlNodeType.Element)
                        {
                            if (xmlRi.Name == "item")
                            {
                                dirCapitulo = xmlRi.GetAttribute("href");
                                if (!dirCapitulo.Contains(".css") && !dirCapitulo.Contains(".png") && !dirCapitulo.Contains(".jpeg") && !dirCapitulo.Contains(".jpg") && !dirCapitulo.Contains(".ttf") && !dirCapitulo.Contains(".ncx"))
                                {
                                    String dirItem = String.Concat(dirContent, "\\", dirCapitulo.Replace("/", "\\"));
                                    sB.Append(System.IO.File.ReadAllText(dirItem));
                                }
                            }
                        }
                    }
                    xmlRi.Close();
                    xmlRi.Dispose();
                    Directory.Delete(idAD, true);
                    String sBStringLimpioHtml = Utils.LimpiarHTML(sB.ToString()).Replace("&nbsp;", " ").Replace("&lt;", " ").Replace("&gt;", " ");
                    String[] result = sBStringLimpioHtml.Split(new String[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    ViewBag.textoamp3 = "Texto EPUB:";
                    foreach (String s in result)
                    {
                        ViewBag.textoamp3 += String.Concat(s.Replace("\r\n", " ").Replace("\t", " ").Replace(".", ". ").Replace("   ", " ").Replace("  ", " "), "\r\n");
                    }
                }
                else if (Request.Files[0].FileName.ToLower().Contains(".pdf"))
                {                    
                    String idPDF = Server.MapPath(String.Concat(@"~/App_Data/", id.ToString(), ".pdf"));
                    FileStream fS = new FileStream(idPDF, FileMode.Create);
                    Request.Files[0].InputStream.CopyTo(fS);
                    fS.Flush();
                    fS.Close();
                    fS.Dispose();
                    PDDocument document = PDDocument.load(idPDF);
                    PDFTextStripper stripper = new PDFTextStripper();
                    ViewBag.textoamp3 = "Texto PDF:\r\n";
                    ViewBag.textoamp3 += stripper.getText(document).Replace("\r\n", " ").Replace("\t", " ").Replace(".", ". ").Replace("   ", " ").Replace("  ", " ");
                    document.close();
                    System.IO.File.Delete(idPDF);
                }
            }
            catch (Exception ex)
            {
                ViewBag.textoamp3 = "No se puede leer este EPUB/PDF";
            }
            return View("textoamp3");
        }
    }
}