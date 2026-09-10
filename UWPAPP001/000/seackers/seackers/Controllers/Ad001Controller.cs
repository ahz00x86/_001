using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using seackers.Models;
using seackers.Models.InApp;
using System.Data;
using System.Text;
using System.Text.Encodings.Web;

namespace seackers.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class Ad001Controller : Controller
    {
        private readonly ILogger<Ad001Controller> _logger;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public Ad001Controller(ILogger<Ad001Controller> logger,
            IStringLocalizer<HomeController> localizer,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender, IConfiguration config)
        {
            _logger = logger;
            _localizer = localizer;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
            _config = config;
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        public async Task<IActionResult> Centers()
        {            
            var centros = await _userManager.GetUsersInRoleAsync("Centro");

            List<String> idsCentros = centros.Select(x => x.Id).ToList();

            SeackersContext context = new SeackersContext();

            var dataCentres = context.DataUsers.Where(x => idsCentros.Contains(x.UserId)).ToList();
            
            List<CentroSeacker> centersSea = new List<CentroSeacker>();
            foreach (IdentityUser iU in centros)
            {
                DataUser dU = dataCentres.Where(x => x.UserId == iU.Id).SingleOrDefault();

                CentroSeacker c = new CentroSeacker() {
                    Id = iU.Id,
                    Name = dU != null ? dU.Name ?? "" : "",
                    Email = iU.Email,  
                    Phone = dU != null ? dU.Phone ?? "" : "",
                    UrlLogo = dU != null ? dU.ImgLogoUrl ?? "" : "",
                    Url = dU != null ? dU.Url ?? "" : "",
                    Lock = dU != null ? dU.Lock ?? false : false,
                };

                centersSea.Add(c);
            }

            var model = new CentrosViewModel { CentrosSeacker = centersSea };                        
            return View(model);                        
        }

        public async Task<IActionResult> AddCenter(String NewCenterName, String NewCenterEmail, Boolean Notificar)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(NewCenterEmail);

                if (user == null)
                {
                    user = CreateUser();
                    await _userStore.SetUserNameAsync(user, NewCenterEmail, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, NewCenterEmail, CancellationToken.None);
                    String password = "Se@0" + Guid.NewGuid().ToString();
                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);
                        await _userManager.AddToRoleAsync(user, "Centro");

                        SeackersContext context = new SeackersContext();
                        context.DataUsers.Add(new DataUser() { UserId = user.Id, Name = NewCenterName });
                        context.SaveChanges();

                        if (Notificar)
                        {
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                            var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { area = "Identity", userId = userId, code = code },
                                protocol: Request.Scheme);

                            await _emailSender.SendEmailAsync(NewCenterEmail, "Confirm your email",
                                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.<br/> Use this password for your first login " + password + " (change this password in your profile settings)");
                        }                        
                    }
                }
                else
                {
                    if (!(await _userManager.IsInRoleAsync(user, "Centro")))
                    {
                        await _userManager.AddToRoleAsync(user, "Centro");
                    }
                }
                return RedirectToAction("Centers");
            }
            else
            {
                return RedirectToAction("Centers");
            }
        }


        public IActionResult Formation()
        {
            SeackersContext context = new SeackersContext();

            List<TemaS> contents = context.Contenidos.Where(x => !x.Borrado.HasValue || !x.Borrado.Value)
                .Select(x => new TemaS() 
                {
                    Id = x.Id,
                    IdTematica = x.IdTematica,
                    IdTema = x.IdTema,
                    IdPadre = x.IdPadre,
                    Titulo = x.Titulo,
                    Publico = x.Publico                    
                }).ToList();

            List<TemaS> temasFinal = new List<TemaS>();
            TemaS tP = contents.Where(x => !x.IdTema.HasValue && !x.IdPadre.HasValue).SingleOrDefault();
            while (tP != null)
            {
                tP.SuperT = null;
                temasFinal.Add(tP);
                IterTemaS(tP, contents, null);
                tP = contents.Where(x => !x.IdTema.HasValue && x.IdPadre == tP.Id).SingleOrDefault();

            }
            //TemaS tema = contents.Where(x => !x.IdTema.HasValue && !x.IdPadre.HasValue).SingleOrDefault();

            FormationViewModel FVM = new FormationViewModel() { Temas = temasFinal };
            return View(FVM);            
        }

        private void IterTemaS(TemaS tP, List<TemaS> tS, TemaS superT)
        {
            tP.Secciones = new List<TemaS>();
            TemaS tH = tS.Where(x => x.IdTema == tP.Id && !x.IdPadre.HasValue).SingleOrDefault();
            while (tH != null)
            {
                tH.SuperT = superT == null ? tH : superT;
                tP.Secciones.Add(tH);
                IterTemaS(tH, tS, superT == null ? tH : superT);
                var tHTemp = tS.Where(x => x.IdTema == tP.Id && x.IdPadre == tH.Id).SingleOrDefault();                
                tH = tHTemp;

            }

        }

        public IActionResult ETiendaOnline()
        {
            SeackersContext context = new SeackersContext();

            List<Models.InApp.Categoria> contents = context.Categorias.Where(x => !x.Borrado.HasValue || !x.Borrado.Value)
                .Select(x => new Models.InApp.Categoria()
                {
                    Id = x.Id,
                    IdTematica = x.IdTematica,
                    IdTema = x.IdTema,
                    IdPadre = x.IdPadre,
                    Nombre = x.Titulo,
                    Publico = x.Publico
                }).ToList();

            List<Models.InApp.Categoria> temasFinal = new List<Models.InApp.Categoria>();
            List<Models.InApp.Categoria> tPs = contents.Where(x => !x.IdTema.HasValue && !x.IdPadre.HasValue).ToList();

            foreach (Models.InApp.Categoria tPo in tPs) 
            {
                Models.InApp.Categoria tP = tPo;

                while (tP != null)
                {
                    tP.SuperC = null;
                    temasFinal.Add(tP);
                    IterTemaSCA(tP, contents, null);
                    tP = contents.Where(x => !x.IdTema.HasValue && x.IdPadre == tP.Id).SingleOrDefault();

                } 
            }
            //TemaS tema = contents.Where(x => !x.IdTema.HasValue && !x.IdPadre.HasValue).SingleOrDefault();

            String userId = _userManager.GetUserId(User);

            List<Models.InApp.Producto> productos = 
                context.ProductosBs.Where(x => x.UserId == userId).Select(x =>
                new Models.InApp.Producto() 
                { 
                    Id = x.Id,
                    IdB = x.UserId,
                    IdCategoria = x.IdCategoria,
                    Nombre = x.Nombre,
                    Precio = x.Pvp,
                    Contenido = x.Contenido,
                    ImageUrl = x.UrlImgMain ?? ""

                }).ToList();


            TiendaOnlineViewModel FVM = new TiendaOnlineViewModel() { Categorias = temasFinal, MINEProductos = productos };
            return View(FVM);
        }

        private void IterTemaSCA(Models.InApp.Categoria tP, List<Models.InApp.Categoria> tS, Models.InApp.Categoria superT)
        {
            tP.CategoriasHIJAS = new List<Models.InApp.Categoria>();
            Models.InApp.Categoria tH = tS.Where(x => x.IdTema == tP.Id && !x.IdPadre.HasValue).SingleOrDefault();
            while (tH != null)
            {
                tH.SuperC = superT == null ? tH : superT;
                tP.CategoriasHIJAS.Add(tH);
                IterTemaSCA(tH, tS, superT == null ? tH : superT);
                var tHTemp = tS.Where(x => x.IdTema == tP.Id && x.IdPadre == tH.Id).SingleOrDefault();
                tH = tHTemp;

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SaveProduct(Guid? idP, String categorias, String title, String content, Decimal precio, IFormFile dFile, Int32 stock, Guid languaje)
        {
            Guid ID = idP ?? Guid.NewGuid();

            SeackersContext context = new SeackersContext();
            String userId = _userManager.GetUserId(User);
            ProductosB prod = context.ProductosBs.Where(x => x.Id == ID && x.UserId == userId).SingleOrDefault();

            //TODO: IMAGEN PRODUCTO
            String[] categs = categorias != null ? categorias.Split(';', StringSplitOptions.RemoveEmptyEntries) : new String[0];

            Boolean isLogo = false;
            String urlImg = String.Empty;
            if (dFile != null)
            {
                if (dFile.ContentType.StartsWith("image/") && dFile.Length < 1048576 && dFile.Length > 0)
                {
                    // Si el archivo no es una imagen, haz algo, como devolver un mensaje de error
                    //return BadRequest("El archivo seleccionado no es una imagen válida.");


                    //using var stream = new StreamReader(dFile.OpenReadStream());
                    //var bytes = Encoding.UTF8.GetBytes(stream.ReadToEnd());

                    // Obtener la referencia a la cuenta de almacenamiento de Azure
                    string connectionString = _config.GetValue<string>("blb");
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    // Obtener la referencia al contenedor de blobs públicos existente
                    string nombreContenedor = "productsimages";
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(nombreContenedor);

                    // Generar un nombre único para el archivo de imagen
                    string nombreArchivo = "I" + (prod != null ? prod.Id : ID) + Path.GetExtension(dFile.FileName);

                    // Obtener una referencia al blob de destino en el contenedor
                    BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

                    // Guardar los bytes de la imagen en el blob
                    using (var stream = dFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, overwrite: true);
                    }

                    // Generar la URL pública del blob
                    //string urlBlob = $"{blobClient.Uri.Scheme}://{blobClient.Uri.Host}/{blobClient.Uri.Segments[1]}{nombreContenedor}/{nombreArchivo}";                    
                    string urlBlob = $"{blobClient.Uri.Scheme}://{blobClient.Uri.Host}/{nombreContenedor}/{nombreArchivo}";

                    urlImg = urlBlob;
                    isLogo = true;
                }
            }

            if (prod == null)
            {
                prod = new ProductosB() { Id = ID, UserId = userId, UrlImgMain = urlImg, IdCategoria = languaje, Stock = stock,  Nombre = title, Contenido = content, Pvp = precio, Started = DateTime.Now };

                context.ProductosBs.Add(prod);                


            }
            else
            {                
                prod.Nombre = title;
                prod.Contenido = content;
                prod.IdCategoria = languaje;
                prod.Pvp = precio;
                prod.Stock = stock;
                prod.UrlImgMain = String.IsNullOrEmpty(prod.UrlImgMain) ? (isLogo ? urlImg : String.Empty) : prod.UrlImgMain;
                var prodCats = context.ProdCats.Where(x => x.UserId == userId && x.ProductId == prod.Id);
                context.ProdCats.RemoveRange(prodCats);
            }

            foreach (String idCat in categs)
            {
                context.ProdCats.Add(new ProdCat() { UserId = userId, ProductId = prod.Id, CategoriaId = new Guid(idCat) });
            }

            context.SaveChanges();
                        
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject((true), Formatting.Indented, settings);
            return Json(s);
        }
            

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddTemaS(Guid? tematica, Guid? idP, Guid? idT, String title)
        {
            Guid nID = Guid.NewGuid();

            SeackersContext context = new SeackersContext();
            Models.Contenido nC = new Models.Contenido() { Id = nID, IdTematica = (tematica.HasValue ? tematica : (idT.HasValue ? nID : null)), IdPadre = idP, IdTema = idT, Titulo = title, Contenido1 = String.Empty, Publico = false };
            context.Contenidos.Add(nC);

            var padreAnterior = context.Contenidos.Where(x => x.Id != nID && x.IdTema == idT && x.IdPadre == idP).SingleOrDefault();

            if(padreAnterior != null)
            {
                padreAnterior.IdPadre = nID;
            }
            context.SaveChanges();

            List<TemaS> contents = context.Contenidos.Where(x => !x.Borrado.HasValue || !x.Borrado.Value)
                .Select(x => new TemaS()
                {
                    Id = x.Id,
                    IdTematica = x.IdTematica,
                    IdTema = x.IdTema,
                    IdPadre = x.IdPadre,
                    Titulo = x.Titulo,
                    Publico = x.Publico
                }).ToList();

            List<TemaS> temasFinal = new List<TemaS>();
            TemaS tP = contents.Where(x => !x.IdTema.HasValue && !x.IdPadre.HasValue).SingleOrDefault();
            while (tP != null)
            {
                tP.SuperT = null;
                temasFinal.Add(tP);
                IterTemaS(tP, contents, null);
                tP = contents.Where(x => !x.IdTema.HasValue && x.IdPadre == tP.Id).SingleOrDefault();
                            
            }

            //var json = new JsonResult(new FormationViewModel() { Temas = temasFinal });

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject((new FormationViewModel() { Temas = temasFinal }), Formatting.Indented, settings);
            return Json(s);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddCategoriaS(Guid? tematica, Guid? idP, Guid? idT, String title)
        {
            Guid nID = Guid.NewGuid();

            SeackersContext context = new SeackersContext();
            Models.Categoria nC = new Models.Categoria() { Id = nID, IdTematica = (tematica.HasValue ? tematica : (idT.HasValue ? nID : null)), IdPadre = idP, IdTema = idT, Titulo = title, Contenido = title, Publico = false };
            context.Categorias.Add(nC);

            var padreAnterior = context.Categorias.Where(x => x.Id != nID && x.IdTema == idT && x.IdPadre == idP).SingleOrDefault();

            if (padreAnterior != null)
            {
                padreAnterior.IdPadre = nID;
            }
            context.SaveChanges();

            List<Models.InApp.Categoria> contents = context.Categorias.Where(x => !x.Borrado.HasValue || !x.Borrado.Value)
                .Select(x => new Models.InApp.Categoria()
                {
                    Id = x.Id,
                    IdTematica = x.IdTematica,
                    IdTema = x.IdTema,
                    IdPadre = x.IdPadre,
                    Nombre = x.Titulo,
                    Publico = x.Publico
                }).ToList();

            List<Models.InApp.Categoria> temasFinal = new List<Models.InApp.Categoria>();
            List<Models.InApp.Categoria> tPs = contents.Where(x => !x.IdTema.HasValue && !x.IdPadre.HasValue).ToList();
            foreach (Models.InApp.Categoria tPo in tPs)
            {
                Models.InApp.Categoria tP = tPo;

                while (tP != null)
                {
                    tP.SuperC = null;
                    temasFinal.Add(tP);
                    IterTemaSCA(tP, contents, null);
                    tP = contents.Where(x => !x.IdTema.HasValue && x.IdPadre == tP.Id).SingleOrDefault();

                }
            }
            //var json = new JsonResult(new FormationViewModel() { Temas = temasFinal });

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject((new TiendaOnlineViewModel() { Categorias = temasFinal }), Formatting.Indented, settings);
            return Json(s);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetContent(Guid idT)
        {
            SeackersContext context = new SeackersContext();
            var contenido = context.Contenidos.Where(x => x.Id == idT).SingleOrDefault();
            ContentQuestsAdminViewModel cQAdVM = new ContentQuestsAdminViewModel();
            cQAdVM.IdContent = idT;
            cQAdVM.Content = contenido.Contenido1;
            List<QuestVM> quests = context.Quests.Where(x => x.IdTema == idT)
                .Select(x => new QuestVM() { Id = x.Id, Text = x.Text, IdQuestion = x.IdQuestion,
                    MultiSelection = (x.IsCorrect.HasValue ? x.IsCorrect.Value: false) }).ToList();


            List<QuestVM> finalQuests = quests.Where(x => !x.IdQuestion.HasValue)
                .Select(x => new QuestVM() { Id = x.Id, IdQuestion = x.IdQuestion, MultiSelection = x.MultiSelection,
                    Text = x.Text, Responses = quests.Where(y => y.IdQuestion == x.Id).ToList()})
                .ToList();

            cQAdVM.Questions = finalQuests;

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject(cQAdVM, Formatting.Indented, settings);

            return Json(s);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetProduct(Guid idT)
        {
            SeackersContext context = new SeackersContext();
            
            var producto = context.ProductosBs.Where(x => x.Id == idT)
                .Select(x => new Producto() 
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Precio = x.Pvp,
                    Stock = x.Stock ?? 0,
                    Contenido = x.Contenido,
                    ImageUrl = x.UrlImgMain
                    
                })
                .Single();

            producto.CategoriasProd = context.ProdCats.Where(x => x.ProductId == idT)
                .Select(x => new Models.InApp.Categoria()
                {
                    Id = x.Categoria.Id,
                    Checked = true                    

                }).ToList();


            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject(producto, Formatting.Indented, settings);

            return Json(s);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveContent(Guid idT, String title, String idP)
        {
            SeackersContext context = new SeackersContext();
            var contenido = context.Contenidos.Where(x => x.Id == idT).SingleOrDefault();
            contenido.Contenido1 = title;
            contenido.Titulo = idP;
            context.SaveChanges();

            return new JsonResult(true);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AddFile(String nFile, IFormFile dFile)
        {
            Guid idF = Guid.NewGuid();

            Boolean isFile = false;
            if (dFile != null)
            {
                if (dFile.ContentType.StartsWith("application/pdf") && dFile.Length > 0)
                {
                    // Si el archivo no es una imagen, haz algo, como devolver un mensaje de error
                    //return BadRequest("El archivo seleccionado no es una imagen válida o es superior a 5 Mb.");


                    // Obtener la referencia a la cuenta de almacenamiento de Azure
                    string connectionString = _config.GetValue<string>("blb"); ;
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    // Obtener la referencia al contenedor de blobs públicos existente
                    string nombreContenedor = "docfiles";
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(nombreContenedor);

                    // Generar un nombre único para el archivo de imagen
                    string nombreArchivo = "F" + idF.ToString() + Path.GetExtension(dFile.FileName);

                    // Obtener una referencia al blob de destino en el contenedor
                    BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

                    // Guardar los bytes de la imagen en el blob
                    using (var stream = dFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, overwrite: true);
                    }

                    // Generar la URL pública del blob
                    //string urlBlob = $"{blobClient.Uri.Scheme}://{blobClient.Uri.Host}/{blobClient.Uri.Segments[1]}{nombreContenedor}/{nombreArchivo}";                    
                    string urlBlob = $"{blobClient.Uri.Scheme}://{blobClient.Uri.Host}/{nombreContenedor}/{nombreArchivo}";

                    SeackersContext context = new SeackersContext();

                    context.Archivos.Add(new Archivo() { Id = idF, Titulo = nFile, FileName = dFile.FileName, Url = urlBlob });

                    //Input.ImgHeadUrl = urlBlob;
                    isFile = true;
                }
            }

            return new JsonResult(isFile ? idF.ToString() : "FALSE") ;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddQuestS(Guid idT, Guid? idP, String title, Boolean isCorrect)
        {
            Guid idQ = Guid.NewGuid();
            SeackersContext context = new SeackersContext();

            var r = context.Quests.Add(new Quest() { Id = idQ, IdTema = idT, IdQuestion = idP, Text = title, IsCorrect = isCorrect });
            context.SaveChanges();

            ContentQuestsAdminViewModel cQAdVM = new ContentQuestsAdminViewModel();
            cQAdVM.IdContent = idT;
            List<QuestVM> quests = context.Quests.Where(x => x.IdTema == idT)
                .Select(x => new QuestVM()
                {
                    Id = x.Id,
                    Text = x.Text,
                    IdQuestion = x.IdQuestion,
                    MultiSelection = (x.IsCorrect.HasValue ? x.IsCorrect.Value : false)
                }).ToList();


            List<QuestVM> finalQuests = quests.Where(x => !x.IdQuestion.HasValue)
                .Select(x => new QuestVM()
                {
                    Id = x.Id,
                    IdQuestion = x.IdQuestion,
                    MultiSelection = x.MultiSelection,
                    Text = x.Text,
                    Responses = quests.Where(y => y.IdQuestion == x.Id).ToList()
                })
                .ToList();

            cQAdVM.Questions = finalQuests;

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject(cQAdVM, Formatting.Indented, settings);

            return Json(s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult IsCorrectMulti(Guid idT, Guid idP, Boolean QisCorrect)
        {            
            SeackersContext context = new SeackersContext();

            var r = context.Quests.Where(x => x.Id == idP).SingleOrDefault();
            r.IsCorrect = QisCorrect;
            context.SaveChanges();

            Boolean multi = false;
            if(context.Quests.Where(x => x.IdQuestion == r.IdQuestion && x.IsCorrect.HasValue && x.IsCorrect.Value).Count() > 1)
            {
                multi = true;
            }
            var q = context.Quests.Where(x => x.Id == r.IdQuestion).SingleOrDefault();
            q.IsCorrect = multi;
            context.SaveChanges();

            ContentQuestsAdminViewModel cQAdVM = new ContentQuestsAdminViewModel();
            cQAdVM.IdContent = idT;
            List<QuestVM> quests = context.Quests.Where(x => x.IdTema == idT)
                .Select(x => new QuestVM()
                {
                    Id = x.Id,
                    Text = x.Text,
                    IdQuestion = x.IdQuestion,
                    MultiSelection = (x.IsCorrect.HasValue ? x.IsCorrect.Value : false)
                }).ToList();


            List<QuestVM> finalQuests = quests.Where(x => !x.IdQuestion.HasValue)
                .Select(x => new QuestVM()
                {
                    Id = x.Id,
                    IdQuestion = x.IdQuestion,
                    MultiSelection = x.MultiSelection,
                    Text = x.Text,
                    Responses = quests.Where(y => y.IdQuestion == x.Id).ToList()
                })
                .ToList();

            cQAdVM.Questions = finalQuests;

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject(cQAdVM, Formatting.Indented, settings);

            return Json(s);
        }

    }
}
