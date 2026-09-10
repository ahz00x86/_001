using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.CognitiveServices.Speech;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using universalsemiotics.Models;

namespace universalsemiotics.Controllers
{
    [Authorize]
    public class enController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

            if (Request != null)
            {
                using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
                {
                    List<PublicidadModel> publi = bbdd.PublicidadBanner.Where(x => x.validado && x.publicado)
                        .Join(bbdd.PublicidadBannerPago, x => x.IdPubli, y => y.IdPubli, (x, y) => new { X = x, Y = y })
                        .Where(z => z.Y.fechaIni < DateTime.Now && z.Y.fechaFin > DateTime.Now)
                        .GroupBy(z => new { z.X.IdPubli, z.X.extension, z.X.titulo, z.X.banner, z.X.urlD })
                        .Select(z => new PublicidadModel() { Id = z.Key.IdPubli, Extension = z.Key.extension, Titulo = z.Key.titulo, Banner = z.Key.banner, UrlD = z.Key.urlD, Prices = z.Sum(p => p.Y.importe) })
                        .Where(p => p.Prices > 0).OrderByDescending(o => o.Prices).ToList();

                    if (publi != null)
                    {
                        ViewBag.publiI = publi.Where(x => x.Banner == "I").ToList();
                        ViewBag.publiI1 = publi.Where(x => x.Banner == "I").FirstOrDefault();
                    }

                    if (publi != null)
                    {
                        ViewBag.publiD = publi.Where(x => x.Banner == "D").ToList();
                        ViewBag.publiD1 = publi.Where(x => x.Banner == "D").FirstOrDefault();
                    }
                }

                if (Request.Cookies["idioma"] != null)
                {
                    String idiomaC = Request.Cookies["idioma"].Value.ToString();
                    if (String.IsNullOrEmpty(idiomaC))
                    {
                        Response.Cookies.Add(new HttpCookie("idioma", "en-US"));
                    }
                    else
                    {
                        Response.Cookies["idioma"].Value = "en-US";
                    }
                }
                else
                {
                    Response.Cookies.Add(new HttpCookie("idioma", "en-US"));
                }

                string culture = "en-US";
                //if (Request.UserLanguages != null)
                //{
                //    culture = Request.UserLanguages[0];
                //}
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

            }
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Aviso_Legal()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Characters()
        {
            String userIdS = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Guid userId = new Guid(userIdS);

            using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
            {
                // COMPROBAR CARACTERES EN CURSO MENOS ACTUALES
                UsersData uD = bbdd.UsersData.Where(x => x.UserId == userId).FirstOrDefault();

                if (uD == null)
                {
                    bbdd.UsersData.Add(new UsersData() { UserId = userId, caracteres = 0 });
                    bbdd.SaveChanges();
                }
            }

            return View(userId);
        }

        [HttpGet]
        public ActionResult text_to_speech_pro()
        {
            String userIdS = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Guid userId = new Guid(userIdS);

            Int64 caracteres = 0;

            using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
            {
                // COMPROBAR CARACTERES EN CURSO MENOS ACTUALES
                UsersData uD = bbdd.UsersData.Where(x => x.UserId == userId).FirstOrDefault();

                if (uD == null)
                {
                    bbdd.UsersData.Add(new UsersData() { UserId = userId, caracteres = 0 });
                    bbdd.SaveChanges();
                }
                else
                {
                    caracteres = uD.caracteres;
                }
            }

            return View(caracteres);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<String> text_to_speech_pro(String titulo, String texto, String idioma, String voz)
        {
            try
            {
                if (String.IsNullOrEmpty(titulo)) { return "... The title cannot be blank ..."; }
                if (String.IsNullOrEmpty(texto)) { return "... The text cannot be blank ..."; }
                if (String.IsNullOrEmpty(voz)) { return "... The voice cannot be blank ..."; }
                if (idioma != "es-ES" && idioma != "en-US" && idioma != "fr-FR" && idioma != "de-DE" && idioma != "it-IT") { return "... Incompatible language ..."; }

                String userIdS = System.Web.HttpContext.Current.User.Identity.GetUserId();
                Guid userId = new Guid(userIdS);

                using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
                {
                    // COMPROBAR CARACTERES EN CURSO MENOS ACTUALES
                    UsersData uD = bbdd.UsersData.Where(x => x.UserId == userId).FirstOrDefault();

                    if (uD == null)
                    {
                        bbdd.UsersData.Add(new UsersData() { UserId = userId, caracteres = 0 });
                        bbdd.SaveChanges();
                        return "... You don't have enough characters for this conversion ...";
                    }
                    else
                    {
                        if (uD.caracteres - texto.Length >= 0)
                        {
                            uD.caracteres -= texto.Length;
                            bbdd.SaveChanges();
                        }
                        else
                        {
                            return "... You don't have enough characters for this conversion ...";
                        }
                    }
                }

                Guid id = Guid.NewGuid();
                String fileName = String.Concat(id.ToString(), ".us");

                Uri blobUri = new Uri(String.Concat("https://universalsemiotics.blob.core.windows.net/docspro/", fileName));
                Azure.Storage.StorageSharedKeyCredential storageCredentials =
                    new StorageSharedKeyCredential("universalsemiotics", ConfigurationManager.AppSettings["blb"]);
                BlobClient blobClient = new BlobClient(blobUri, storageCredentials);
                using (MemoryStream file = new MemoryStream(Encoding.UTF8.GetBytes(texto)))
                {
                    await blobClient.UploadAsync(file);
                }

                Int64 caracteres = texto.LongCount();

                using (universalsemioticsEntities p = new universalsemioticsEntities())
                {
                    p.Mp3Pro.Add(new Mp3Pro() { Id = id, UserId = userId, titulo = titulo, idioma = idioma, caracteres = caracteres, fechaIni = DateTime.Now });
                    p.SaveChanges();
                }

                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                        .GetUserManager<ApplicationUserManager>()
                        .FindById(userIdS);

                try
                {
                    Task.Run(() => CONVERT.CONVERTm(user.Email, id, userId, titulo, texto, voz, Server));
                }
                catch (Exception ex)
                {
                    String bodyMail = "<p>Your titled text " + titulo + " could not be converted</p>"
                        + "<p>An error occurred in the conversion process.</p>"
                        + "<p>Thanks for your time</p>";
                    Utils.SendMail("apzyx@yahoo.com", "ERROR - universalsemiotics.com", bodyMail + ex.ToString());
                }

                return "... ALL RIGHT =) ...";
            }
            catch (Exception ex)
            {
                return "... An error has occurred in the process ...";
            }
        }

        [HttpGet]
        public ActionResult text_to_speech()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<String> text_to_speech(String titulo, String texto, String idioma)
        {
            try
            {
                string captchaResponse = Request.Form["g-Recaptcha-Response"];
                Utils.ReCaptchaValidationResult resultCaptcha = Utils.IsValid(captchaResponse);

                if (!resultCaptcha.Success)
                {
                    if (resultCaptcha.ErrorCodes != null && resultCaptcha.ErrorCodes.Count > 0)
                    {
                        return resultCaptcha.ErrorCodes[0];
                        //foreach (string err in resultCaptcha.ErrorCodes)
                        //{
                        //    ModelState.AddModelError("", err);
                        //}
                    }
                    else
                    {
                        ModelState.AddModelError("", "invalid reCaptcha");
                    }
                    return "... invalid reCaptcha ...";
                }

                if (String.IsNullOrEmpty(titulo)) { return "... The title cannot be blank ..."; }
                if (String.IsNullOrEmpty(texto)) { return "... The text cannot be blank ..."; }
                if (idioma != "es-ES" && idioma != "en-US" && idioma != "fr-FR" && idioma != "de-DE" && idioma != "it-IT") { return "... Incompatible language ..."; }

                Guid userId = new Guid(System.Web.HttpContext.Current.User.Identity.GetUserId());

                using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
                {
                    Mp3Free lastmp3 = bbdd.Mp3Free.Where(x => x.UserId == userId && !x.estado.HasValue).OrderByDescending(o => o.fechaIni).FirstOrDefault();

                    if (lastmp3 != null)
                    {
                        DateTime fechaProximoProceso = lastmp3.fechaIni.AddSeconds(lastmp3.caracteres);
                        TimeSpan resto = fechaProximoProceso - DateTime.Now;

                        if (resto.TotalSeconds > 0)
                        {
                            return "... Depending on the size of your last file you should wait [" + resto.TotalSeconds.ToString("N0") + "] seconds to process another file or wait for the current one to finish ...";
                        }
                    }
                }

                Guid id = Guid.NewGuid();
                String fileName = String.Concat(id.ToString(), ".us");

                Uri blobUri = new Uri(String.Concat("https://universalsemiotics.blob.core.windows.net/docsfree/", fileName));
                Azure.Storage.StorageSharedKeyCredential storageCredentials =
                    new StorageSharedKeyCredential("universalsemiotics", ConfigurationManager.AppSettings["blb"]);
                BlobClient blobClient = new BlobClient(blobUri, storageCredentials);
                using (MemoryStream file = new MemoryStream(Encoding.UTF8.GetBytes(texto)))
                {
                    await blobClient.UploadAsync(file);
                }

                Int64 caracteres = texto.LongCount();

                using (universalsemioticsEntities p = new universalsemioticsEntities())
                {
                    p.Mp3Free.Add(new Mp3Free() { Id = id, UserId = userId, titulo = titulo, idioma = idioma, caracteres = caracteres, fechaIni = DateTime.Now });
                    p.SaveChanges();
                }

                return "... ALL RIGHT =) ...";
            }
            catch (Exception ex)
            {
                return "... An error has occurred in the process ...";
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> getVoices(String idioma)
        {
            SpeechConfig sC = SpeechConfig.FromSubscription(ConfigurationManager.AppSettings["cog"], "northeurope");
            SpeechSynthesizer sS = new SpeechSynthesizer(sC);
            SynthesisVoicesResult vR = await sS.GetVoicesAsync(idioma);
            IReadOnlyCollection<VoiceInfo> vocesInfo = vR.Voices;
            List<String> voces = new List<String>();
            foreach (VoiceInfo voz in vocesInfo)
            {
                voces.Add(voz.ShortName);
            }

            return Json(voces.ToArray());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public String getEPUBPDFText()
        {
            String resultS = String.Empty;
            try
            {
                if (Request.Files[0].ContentLength > 50 * 1024 * 1024) return "Your file is too large, please reduce its contents.";

                Guid id = Guid.NewGuid();
                if (Request.Files[0].FileName.ToLower().Contains(".epub"))
                {
                    String idAD = Server.MapPath(String.Concat(@"~/App_Data/", id.ToString(), "/"));
                    System.IO.Compression.ZipArchive zA = new System.IO.Compression.ZipArchive(Request.Files[0].InputStream);
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
                    foreach (String s in result)
                    {
                        resultS += String.Concat(s.Replace("\r\n", " ").Replace("\t", " ").Replace(".", ". ").Replace("   ", " ").Replace("  ", " "), "\r\n");
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
                    resultS = Utils.ExtractTextFromPDF(idPDF).Replace(".\r\n", "<!--77474-->").Replace("\r\n", " ").Replace("\n", " ").Replace("\t", " ").Replace("<!--77474-->", ".\r\n").Replace(".", ". ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ");
                    System.IO.File.Delete(idPDF);
                }
            }
            catch (Exception ex)
            {
                resultS = "Can't read this EPUB/PDF";
            }

            return resultS;
        }

        [HttpGet]
        public ActionResult Downloads()
        {
            Guid userId = new Guid(System.Web.HttpContext.Current.User.Identity.GetUserId());

            MP3sModel model = new MP3sModel();
            using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
            {
                DateTime fechaDESCARGASPRO = DateTime.Now.AddDays(-7);
                List<MP3ProModel> pros = bbdd.Mp3Pro.Where(x => x.UserId == userId && (!x.fechaFin.HasValue || x.fechaFin.Value > fechaDESCARGASPRO))
                    .OrderByDescending(x => x.fechaIni).Select(x => new MP3ProModel() { Id = x.Id, Titulo = x.titulo, Fecha = x.fechaIni, EnProceso = !x.estado.HasValue }).ToList();

                model.DescargasPro = pros;

                DateTime fechaDESCARGAS = DateTime.Now.AddDays(-2);
                List<MP3FreeModel> mp3s = bbdd.Mp3Free.Where(x => x.UserId == userId && (!x.estado.HasValue || x.estado.Value) && (!x.fechaFin.HasValue || x.fechaFin.Value > fechaDESCARGAS))
                    .OrderByDescending(x => x.fechaIni).Select(x => new MP3FreeModel() { Id = x.Id, Titulo = x.titulo, Fecha = x.fechaIni, EnCola = !x.estado.HasValue }).ToList();

                if (mp3s != null && mp3s.Count > 0)
                {
                    List<Guid> guids = bbdd.Mp3Free.Where(x => !x.estado.HasValue).OrderBy(o => o.fechaIni).Select(x => x.Id).ToList();

                    for (Int32 j = 0; j < mp3s.Count; j++)
                    {
                        Int64 i = 1;
                        foreach (Guid g in guids)
                        {
                            if (mp3s[j].Id == g)
                            {
                                mp3s[j].Cola = i;
                                break;
                            }

                            i++;
                        }
                    }

                }

                model.DescargasFree = mp3s;
            }

            return View(model);
        }

        [HttpGet]
        public FileStreamResult Download(Guid id)
        {
            Guid userId = new Guid(System.Web.HttpContext.Current.User.Identity.GetUserId());
            using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
            {
                DateTime fechaDESCARGAS = DateTime.Now.AddDays(-2);
                Mp3Free mp3 = bbdd.Mp3Free.Where(x => x.UserId == userId && x.estado.HasValue && x.estado.Value && x.fechaFin > fechaDESCARGAS && x.Id == id).SingleOrDefault();

                if (mp3 == null)
                {
                    return File(new MemoryStream(), null);
                }

                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=universalsemiotics;AccountKey=" + ConfigurationManager.AppSettings["blb"] + ";EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference("mp3free");
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(id.ToString() + ".mp3");
                Stream blobStream = blob.OpenReadAsync().Result;
                Response.BufferOutput = false;
                return File(blobStream, blob.Properties.ContentType);
            }
        }

        [HttpGet]
        public FileStreamResult DownloadPro(Guid id)
        {
            Guid userId = new Guid(System.Web.HttpContext.Current.User.Identity.GetUserId());
            using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
            {
                DateTime fechaDESCARGAS = DateTime.Now.AddDays(-7);
                Mp3Pro mp3 = bbdd.Mp3Pro.Where(x => x.UserId == userId && x.estado.HasValue && x.estado.Value && x.fechaFin > fechaDESCARGAS && x.Id == id).SingleOrDefault();

                if (mp3 == null)
                {
                    return File(new MemoryStream(), null);
                }

                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=universalsemiotics;AccountKey=" + ConfigurationManager.AppSettings["blb"] + ";EndpointSuffix=core.windows.net");
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference("mp3pro");
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(id.ToString() + ".mp3");
                Stream blobStream = blob.OpenReadAsync().Result;
                Response.BufferOutput = false;
                return File(blobStream, blob.Properties.ContentType);
            }
        }

        #region PUBLICIDAD

        [HttpGet]
        public ActionResult PublicidadI()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PublicidadD()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<String> PublicidadI(String titulo, String urlD)
        {
            try
            {
                if (Request.Files[0].InputStream.Length != 0 && !String.IsNullOrEmpty(urlD) && !String.IsNullOrEmpty(titulo))
                {
                    String[] nombreEx = Request.Files[0].FileName.ToLower().Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    String extension = nombreEx[nombreEx.Length - 1];

                    if (extension != "png" && extension != "jpg" && extension != "gif" && extension != "jpeg")
                    {
                        return "... Only image files with gif, png, jpg or jpeg extension are supported ...";
                    }

                    using (System.Drawing.Bitmap objbitmap = new System.Drawing.Bitmap(Request.Files[0].InputStream))
                    {
                        if (objbitmap.Height != 100 || objbitmap.Width != 100)
                        {
                            return "... Only 100 x 100 pixel images are supported (gif, png, jpg, or jpeg) ...";
                        }
                    }

                    Request.Files[0].InputStream.Position = 0;

                    Guid id = Guid.NewGuid();

                    String fileName = String.Concat(id.ToString(), ".", extension);

                    Uri blobUri = new Uri(String.Concat("https://universalsemiotics.blob.core.windows.net/publicidad/", fileName));
                    Azure.Storage.StorageSharedKeyCredential storageCredentials =
                        new StorageSharedKeyCredential("universalsemiotics", ConfigurationManager.AppSettings["blb"]);
                    BlobClient blobClient = new BlobClient(blobUri, storageCredentials);
                    await blobClient.UploadAsync(Request.Files[0].InputStream);

                    String userId = System.Web.HttpContext.Current.User.Identity.GetUserId();

                    using (universalsemioticsEntities p = new universalsemioticsEntities())
                    {
                        p.PublicidadBanner.Add(new PublicidadBanner() { IdPubli = id, IdUser = new Guid(userId), banner = "I", extension = extension, urlD = urlD, titulo = titulo, fechaIni = DateTime.Now, validado = false, publicado = false });
                        p.SaveChanges();
                    }

                    ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                            .GetUserManager<ApplicationUserManager>()
                            .FindById(userId);

                    String body = "<p> <a href='https://www.universalsemiotics.com/en/xdc4cb595729d341d6c5d12943a5a3371e7612528?id32po=" + id + "'>ver</a><br/>" + urlD + "<br/>" + user.Email + "</p>";

                    Utils.SendMail("apzyx@yahoo.com", "NEW AD", body);

                    return "... Your advertising was sent correctly ...";
                }
                else
                {
                    return "... You must fill in all the fields ...";
                }
            }
            catch (Exception ex)
            {
                return "... An error occurred during the process ...";
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<String> PublicidadD(String titulo, String urlD)
        {
            try
            {
                if (Request.Files[0].InputStream.Length != 0 && !String.IsNullOrEmpty(urlD) && !String.IsNullOrEmpty(titulo))
                {
                    String[] nombreEx = Request.Files[0].FileName.ToLower().Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    String extension = nombreEx[nombreEx.Length - 1];

                    if (extension != "png" && extension != "jpg" && extension != "gif" && extension != "jpeg")
                    {
                        return "... Only image files with gif, png, jpg or jpeg extension are supported ...";
                    }

                    using (System.Drawing.Bitmap objbitmap = new System.Drawing.Bitmap(Request.Files[0].InputStream))
                    {
                        if (objbitmap.Height != 100 || objbitmap.Width != 100)
                        {
                            return "... Only 100 x 100 pixel images are supported (gif, png, jpg, or jpeg) ...";
                        }
                    }

                    Request.Files[0].InputStream.Position = 0;

                    Guid id = Guid.NewGuid();

                    String fileName = String.Concat(id.ToString(), ".", extension);

                    Uri blobUri = new Uri(String.Concat("https://universalsemiotics.blob.core.windows.net/publicidad/", fileName));
                    Azure.Storage.StorageSharedKeyCredential storageCredentials =
                        new StorageSharedKeyCredential("universalsemiotics", ConfigurationManager.AppSettings["blb"]);
                    BlobClient blobClient = new BlobClient(blobUri, storageCredentials);
                    await blobClient.UploadAsync(Request.Files[0].InputStream);

                    String userId = System.Web.HttpContext.Current.User.Identity.GetUserId();

                    using (universalsemioticsEntities p = new universalsemioticsEntities())
                    {
                        p.PublicidadBanner.Add(new PublicidadBanner() { IdPubli = id, IdUser = new Guid(userId), banner = "D", extension = extension, urlD = urlD, titulo = titulo, fechaIni = DateTime.Now, validado = false, publicado = false });
                        p.SaveChanges();
                    }

                    ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                            .GetUserManager<ApplicationUserManager>()
                            .FindById(userId);

                    String body = "<p> <a href='https://www.universalsemiotics.com/en/xdc4cb595729d341d6c5d12943a5a3371e7612528?id32po=" + id + "'>show</a><br/>" + urlD + "<br/>" + user.Email + "</p>";

                    Utils.SendMail("apzyx@yahoo.com", "NEW AD", body);

                    return "... Your advertising was sent correctly ...";
                }
                else
                {
                    return "... You must fill in all the fields ...";
                }
            }
            catch (Exception ex)
            {
                return "... An error occurred during the process ...";
            }
        }

        [HttpGet]
        public ActionResult xdc4cb595729d341d6c5d12943a5a3371e7612528()
        {
            String id = Request.QueryString["id32po"];

            if (String.IsNullOrEmpty(id)) { return null; }

            PublicidadModel pI = null;
            using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
            {
                pI = bbdd.PublicidadBanner.Where(x => x.IdPubli == new Guid(id)).Select(x => new PublicidadModel() { Id = x.IdPubli, Extension = x.extension, Titulo = x.titulo, Banner = x.banner, UrlD = x.urlD }).Single();
            }

            return View(pI);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult xdc4cb595729d341d6c5d12943a5a3371e7612528(String pps, String id)
        {
            if (pps == "holaadios")
            {
                using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
                {
                    PublicidadBanner p = bbdd.PublicidadBanner.Where(x => x.IdPubli == new Guid(id)).Single();
                    p.validado = true;
                    bbdd.SaveChanges();

                    PublicidadModel pM = new PublicidadModel() { Id = p.IdPubli, IdUser = p.IdUser, Extension = p.extension, UrlD = p.urlD, Banner = p.banner, Titulo = p.titulo };

                    String body = "<h1>Banner Accepted</h1>" +
                        "<a target='_blank' href='" + pM.UrlD + "'><img alt='" + pM.Titulo + "' src='" + pM.UrlImage + "' height='100' width='100' style='height: 100px; width: 100px;' /></a>" +
                        "<br/><br/><p>Now you can make the investment you want in your banner</p>" +
                        "<p><a href='https://www.universalsemiotics.com/en/InversionBANNER?id=" + id + "' target='_blank'>click here to make your investment</a></p>" +
                        "<br/><br/><p>Thanks for your time.</p>" +
                        "<br/><br/><p><a href='https://www.universalsemiotics.com' target='_blank'>www.universalsemiotics.com</a></p>";

                    ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                        .GetUserManager<ApplicationUserManager>()
                        .FindById(pM.IdUser.ToString());

                    Utils.SendMail(user.Email, "Banner OK", body);
                }
            }

            return null;
        }

        [HttpGet]
        public ActionResult InversionBANNER()
        {
            if (Request.QueryString.Count == 1 && Request.QueryString.Keys[0] == "id")
            {
                String id = Request.QueryString["id"];
                if (String.IsNullOrEmpty(id)) { return null; }
                using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
                {
                    PublicidadModel p = bbdd.PublicidadBanner.Where(x => x.IdPubli == new Guid(id) && x.validado)
                            .Select(x => new PublicidadModel() { Id = x.IdPubli, Titulo = x.titulo, Extension = x.extension, Banner = x.banner, UrlD = x.urlD }).Single();
                    return View(p);
                }

            }
            return null;
        }

        [HttpPost]
        [AllowAnonymous]
        public void PayPal_IPN()
        {
            //log.Info("IPN listener invoked");

            try
            {
                var formVals = new Dictionary<string, string>();
                formVals.Add("cmd", "_notify-validate");

                string response = Utils.GetPayPalResponse(formVals, Request);

                if (response == "VERIFIED")
                {
                    string transactionId = Request["txn_id"];
                    string sAmountPaid = Request["mc_gross"];
                    string orderId = Request["custom"];

                    //_logger.Info("IPN Verified for order " + orderID);

                    //validate the order
                    Decimal amountPaid = 0;
                    amountPaid = Convert.ToDecimal(sAmountPaid, CultureInfo.InvariantCulture);
                    //Decimal.TryParse(sAmountPaid, out amountPaid);

                    // check these first
                    bool verified = true;

                    string businessEmail = HttpUtility.UrlDecode(Request["business"]);
                    if (String.Compare(businessEmail, "apzyx@yahoo.com", true) != 0)
                        verified = false;

                    string currencyCode = HttpUtility.UrlDecode(Request["mc_currency"]);
                    if (String.Compare(currencyCode, "EUR", true) != 0)
                        verified = false;

                    string paymentStatus = HttpUtility.UrlDecode(Request["payment_status"]);
                    if (String.Compare(paymentStatus, "Completed", true) != 0)
                        verified = false;

                    //log.Info("Business : " + businessEmail);
                    //log.Info("currency : " + currencyCode);
                    //log.Info("payment status : " + paymentStatus);
                    //log.Info("amount valid : " + AmountPaidIsValid(order, amountPaid).ToString());

                    // process the transaction
                    if (verified)
                    {
                        // Añadir Banner Izquierdo
                        using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
                        {
                            Guid idPubli = new Guid(orderId);
                            PublicidadBanner pD = bbdd.PublicidadBanner.Where(x => x.IdPubli == idPubli).Single();
                            pD.publicado = true;

                            Guid idPago = Guid.NewGuid();
                            bbdd.PublicidadBannerPago.Add(new PublicidadBannerPago() { Id = idPago, IdPubli = idPubli, fechaIni = DateTime.Now, fechaFin = DateTime.Now.AddDays(90), importe = amountPaid });

                            bbdd.SaveChanges();
                        }
                    }
                    else
                    {
                        String body = "<h1>PayPal_IPN universalsemiotics ERROR1</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            Request.ToString() +
                            "<br/><br/><p>Thanks for your time.</p>" +
                            "<br/><br/><p><a href='https://www.universalsemiotics.com' target='_blank'>www.universalsemiotics.com</a></p>";

                        Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN universalsemiotics", body);
                    }
                }
                else
                {
                    String body = "<h1>PayPal_IPN universalsemiotics ERROR2</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            Request.ToString() +
                            "<br/><br/><p>Thanks for your time.</p>" +
                            "<br/><br/><p><a href='https://www.universalsemiotics.com' target='_blank'>www.universalsemiotics.com</a></p>";

                    Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN universalsemiotics", body);
                }
            }
            catch (Exception ex)
            {
                String body = "<h1>PayPal_IPN universalsemiotics ERROR3</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            ex.ToString() +
                            "<br/><br/><p>Thanks for your time.</p>" +
                            "<br/><br/><p><a href='https://www.universalsemiotics.com' target='_blank'>www.universalsemiotics.com</a></p>";

                Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN universalsemiotics", body);
            }
        }

        #endregion

        [HttpPost]
        [AllowAnonymous]
        public void PayPal_IPNC()
        {
            //log.Info("IPN listener invoked");

            try
            {
                var formVals = new Dictionary<string, string>();
                formVals.Add("cmd", "_notify-validate");

                string response = Utils.GetPayPalResponse(formVals, Request);

                if (response == "VERIFIED")
                {
                    string transactionId = Request["txn_id"];
                    string sAmountPaid = Request["mc_gross"];
                    string userIdS = Request["custom"];

                    //_logger.Info("IPN Verified for order " + orderID);

                    //validate the order
                    Decimal amountPaid = 0;
                    amountPaid = Convert.ToDecimal(sAmountPaid, CultureInfo.InvariantCulture);
                    //Decimal.TryParse(sAmountPaid, out amountPaid);

                    // check these first
                    bool verified = true;

                    string businessEmail = HttpUtility.UrlDecode(Request["business"]);
                    if (String.Compare(businessEmail, "apzyx@yahoo.com", true) != 0)
                        verified = false;

                    string currencyCode = HttpUtility.UrlDecode(Request["mc_currency"]);
                    if (String.Compare(currencyCode, "EUR", true) != 0)
                        verified = false;

                    string paymentStatus = HttpUtility.UrlDecode(Request["payment_status"]);
                    if (String.Compare(paymentStatus, "Completed", true) != 0)
                        verified = false;

                    //log.Info("Business : " + businessEmail);
                    //log.Info("currency : " + currencyCode);
                    //log.Info("payment status : " + paymentStatus);
                    //log.Info("amount valid : " + AmountPaidIsValid(order, amountPaid).ToString());

                    // process the transaction
                    if (verified)
                    {
                        // Añadir Banner Izquierdo
                        using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
                        {
                            Guid userId = new Guid(userIdS);
                            UsersData pD = bbdd.UsersData.Where(x => x.UserId == userId).Single();
                            pD.caracteres +=
                                amountPaid == 5 ? 100000 :
                                amountPaid == 10 ? 200000 :
                                amountPaid == 25 ? 600000 :
                                amountPaid == 50 ? 1500000 : 0;
                            bbdd.SaveChanges();
                        }

                        String body = "<h1>PayPal_IPNC universalsemiotics OK</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            Request.ToString() +
                            "<br/><br/><p>Gracias por su tiempo.</p>";

                        foreach (String s in Request.Form.AllKeys)
                        {
                            body += String.Concat("<br>", s, ":", Request[s]);
                        }

                        body += String.Concat("<br>amountDECIMAL: ", amountPaid.ToString());

                        Utils.SendMail("apzyx@yahoo.com", "PayPal_IPNC universalsemiotics OK", body);

                    }
                    else
                    {
                        String body = "<h1>PayPal_IPN universalsemiotics ERROR</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            Request.ToString() +
                            "<br/><br/><p>Gracias por su tiempo.</p>" +
                            "<br/><br/><p><a href='https://www.universalsemiotics.com' target='_blank'>www.universalsemiotics.com</a></p>";

                        Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN universalsemiotics", body);
                    }
                }
                else
                {
                    String body = "<h1>PayPal_IPN universalsemiotics ERROR</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            Request.ToString() +
                            "<br/><br/><p>Gracias por su tiempo.</p>" +
                            "<br/><br/><p><a href='https://www.universalsemiotics.com' target='_blank'>www.universalsemiotics.com</a></p>";

                    Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN universalsemiotics", body);
                }
            }
            catch (Exception ex)
            {
                String body = "<h1>PayPal_IPN universalsemiotics ERROR</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            ex.ToString() +
                            "<br/><br/><p>Gracias por su tiempo.</p>" +
                            "<br/><br/><p><a href='https://www.universalsemiotics.com' target='_blank'>www.universalsemiotics.com</a></p>";

                Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN universalsemiotics", body);
            }
        }
    }
}