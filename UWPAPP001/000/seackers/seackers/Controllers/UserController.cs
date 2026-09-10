using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using NuGet.Configuration;
using seackers.Models;
using seackers.Models.InApp;
using SendGrid.Helpers.Mail;
using System.Data;

namespace seackers.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class UserController : Controller
    {
        private readonly IStringLocalizer<UserController> _localizer;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(IStringLocalizer<UserController> localizer, UserManager<IdentityUser> userManager)
        {
            _localizer = localizer;
            _userManager = userManager;
        }

        public async Task<ViewResult> ConfirmUserCenter(String code)
        {
            if(!String.IsNullOrEmpty(code))
            {
                String[] codes = code.Split('_');

                if(codes.Length == 3) 
                {
                    try
                    {
                        var myId = _userManager.GetUserId(User);                        
                        if (myId == codes[2])
                        {
                            var center = await _userManager.FindByIdAsync(codes[0]);

                            if (center != null)
                            {
                                Guid key = new Guid(codes[1]);
                                SeackersContext context = new SeackersContext();
                                var cenUse = context.CenterUsers.Where(x => x.Key == key && x.CenterId == center.Id && x.UserId == myId).SingleOrDefault();
                                if (cenUse != null)
                                {
                                    cenUse.Accepted = true;
                                    cenUse.AceeptD = DateTime.Now;
                                    context.SaveChanges();
                                    return View(false);
                                }
                            }
                        }
                    }
                    catch (Exception ex) 
                    {
                        
                    }
                }
            }

            return View(true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Like(String centerId, Boolean like)
        {
            String idU = _userManager.GetUserId(User);
            var center = await _userManager.FindByIdAsync(centerId);

            if (center != null)
            {
                SeackersContext context = new SeackersContext();
                var likeUC = context.CenterLikes.Where(x => x.UserId == idU && x.CenterId == center.Id).SingleOrDefault();

                if(likeUC != null)
                {
                    likeUC.Like = like;
                }
                else
                {
                    context.CenterLikes.Add(new CenterLike() { CenterId = center.Id, UserId = idU, Like = like });
                }
                context.SaveChanges();

                return new JsonResult(true);
            }
            else
            {
                return new JsonResult(false);
            }
        }

        public async Task<ContenidosViewModel> GetContents()
        {
            String lang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            ContenidosViewModel cVM = new ContenidosViewModel() { ContenidosUsuario = new List<Models.InApp.Contenido>() };

            if (lang != "es" && lang != "en")
            {
                return cVM;
            }

            SeackersContext context = new SeackersContext();

            Guid? langThread = context.Contenidos.Where(x => !x.IdTema.HasValue && x.Titulo == lang)
                .Select(x => x.Id).SingleOrDefault();

            if (langThread != null)
            {
                var contenidos = context.Contenidos.Where(x => x.IdTema == langThread)
                    .Select(x => new Models.InApp.Contenido() { Id = x.Id, Title = x.Titulo }).ToList();

                cVM.ContenidosUsuario = contenidos;
            }

            return cVM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetContent(Guid ContentId)
        {
            ContenidosViewModel FVM = new ContenidosViewModel();

            SeackersContext context = new SeackersContext();

            var content = context.Contenidos.Where(x => x.Id == ContentId).SingleOrDefault();

            if(content != null)
            {
                FVM.MainContent = content.Contenido1;
            }

            List<Models.InApp.Contenido> contents = context.Contenidos.Where(x => x.IdTematica == ContentId && x.IdTematica.HasValue && (!x.Borrado.HasValue || !x.Borrado.Value))
                .Select(x => new Models.InApp.Contenido()
                {
                    Id = x.Id,
                    IdTema = x.IdTema,
                    IdPadre = x.IdPadre,
                    Title = x.Titulo
                }).ToList();

            List<Models.InApp.Contenido> temasFinal = new List<Models.InApp.Contenido>();
            Models.InApp.Contenido tP = contents.Where(x => x.IdTema == ContentId && !x.IdPadre.HasValue).SingleOrDefault();
            while (tP != null)
            {
                temasFinal.Add(tP);
                IterTemaS(tP, contents);
                tP = contents.Where(x => x.IdTema == ContentId && x.IdPadre == tP.Id).SingleOrDefault();

            }
            //TemaS tema = contents.Where(x => !x.IdTema.HasValue && !x.IdPadre.HasValue).SingleOrDefault();


            FVM.HaveQuest = context.Quests.Where(x => x.IdTema == ContentId && !x.IdQuestion.HasValue).Count() > 0;
                        
            String userId = _userManager.GetUserId(User);
            var listQuestionThemeIds = context.Quests.Where(x => x.IdTema == ContentId && !x.IdQuestion.HasValue).Select(x => x.Id).ToList();
            FVM.HaveResponse = context.UserQuestSelects.Where(x => x.IdUser == userId && listQuestionThemeIds.Contains(x.IdQuestion)).Any();
                
            FVM.ContenidosUsuario = temasFinal;

            return View(FVM);
        }

        private void IterTemaS(Models.InApp.Contenido tP, List<Models.InApp.Contenido> tS)
        {
            tP.Secciones = new List<Models.InApp.Contenido>();
            Models.InApp.Contenido tH = tS.Where(x => x.IdTema == tP.Id && !x.IdPadre.HasValue).SingleOrDefault();
            while (tH != null)
            {
                tP.Secciones.Add(tH);
                IterTemaS(tH, tS);
                var tHTemp = tS.Where(x => x.IdTema == tP.Id && x.IdPadre == tH.Id).SingleOrDefault();
                tH = tHTemp;

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetTheme(Guid ThemeId)
        {
            SeackersContext context = new SeackersContext();

            SimpleContestQuestViewModel sCVM = new SimpleContestQuestViewModel();

            sCVM.Content = context.Contenidos.Where(x => x.Id == ThemeId).Select(x => x.Contenido1).SingleOrDefault();

            sCVM.IsEval = context.Quests.Where(x => x.IdTema == ThemeId).Count() > 0;

            String userId = _userManager.GetUserId(User);
            var listQuestionThemeIds = context.Quests.Where(x => x.IdTema == ThemeId && !x.IdQuestion.HasValue).Select(x => x.Id).ToList();
            sCVM.HasResponse = context.UserQuestSelects.Where(x => x.IdUser == userId && listQuestionThemeIds.Contains(x.IdQuestion)).Any();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject(sCVM, Formatting.Indented, settings);

            return Json(s);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult StartEV(Guid ThemeId)
        {
            SeackersContext context = new SeackersContext();

            var questsIds = context.Quests.Where(x => x.IdTema == ThemeId && !x.IdQuestion.HasValue)
                .Select(x => x.Id.ToString() + ",")
                .OrderBy(x => Guid.NewGuid()).ToList();

            EvalViewModel evalVM = new EvalViewModel();

            if(questsIds.Count() > 0)
            {
                Guid idAE = new Guid(questsIds.First().Replace(",",""));
                evalVM.ActualQuest = context.Quests.Where(x => x.Id == idAE)
                .Select(x => new Eval() { Id = x.Id, Text = x.Text, Multi = (x.IsCorrect.HasValue ? x.IsCorrect.Value : false) })
                .Single();

                evalVM.ActualQuest.Responses = context.Quests.Where(x => x.IdQuestion == idAE)
                .Select(x => new Eval() { Id = x.Id, Text = x.Text }).ToList();

                evalVM.QuestWay = String.Concat(questsIds);
                evalVM.ThemeIdQ = ThemeId;
            }


            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject(evalVM, Formatting.Indented, settings);

            return Json(s);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddResponse(String wayQuest, Guid ThemeIdQ, Guid IdQuest, String idResponse)
        {
            SeackersContext context = new SeackersContext();

            String[] responsesIds = idResponse.Split(",", StringSplitOptions.RemoveEmptyEntries);

            foreach (String responseId in responsesIds)
            {
                Guid gRId = new Guid(responseId);

                UserQuestSelect uqs = new UserQuestSelect();
                uqs.IdQuestion = IdQuest;
                uqs.IdUser = _userManager.GetUserId(User);
                uqs.IdSelect = gRId;
                uqs.Time = DateTime.Now;

                context.UserQuestSelects.Add(uqs);
                                
            }

            context.SaveChanges();



            EvalViewModel evalVM = new EvalViewModel();

            Boolean isEnd = true;
            String[] idsQs = wayQuest.Split(',', StringSplitOptions.RemoveEmptyEntries);
            Guid aQuestId = IdQuest;
            if (idsQs.Length > 0)
            {
                if (new Guid(idsQs[idsQs.Length - 1]) != aQuestId)
                {
                    for (Int32 i = 0; i < idsQs.Length; i++)
                    {
                        if (new Guid(idsQs[i]) == aQuestId)
                        {
                            aQuestId = new Guid(idsQs[i + 1]);
                            isEnd = false;
                            break;
                        }
                    }
                }
            }

            evalVM.IsEnd = isEnd;

            if (!isEnd)
            {
                Guid idAE = aQuestId;
                evalVM.ActualQuest = context.Quests.Where(x => x.Id == idAE)
                .Select(x => new Eval() { Id = x.Id, Text = x.Text, Multi = (x.IsCorrect.HasValue ? x.IsCorrect.Value : false) })
                .Single();

                evalVM.ActualQuest.Responses = context.Quests.Where(x => x.IdQuestion == idAE)
                .Select(x => new Eval() { Id = x.Id, Text = x.Text }).ToList();

                evalVM.QuestWay = wayQuest;
                evalVM.ThemeIdQ = ThemeIdQ;
            }


            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject(evalVM, Formatting.Indented, settings);

            return Json(s);

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ViewEV(Guid ThemeId)
        {
            SeackersContext context = new SeackersContext();

            String idUser = _userManager.GetUserId(User);

            var preguntasRespuestas = context.Quests.Where(x => x.IdTema == ThemeId).ToList();
            var onlyQuestionsIds = preguntasRespuestas.Where(y => !y.IdQuestion.HasValue).Select(y => y.Id);
            var respSelectUser = context.UserQuestSelects.Where(x => x.IdUser == idUser && onlyQuestionsIds.Contains(x.IdQuestion));

            ViewQuestViewModel vQVM = new ViewQuestViewModel();

            vQVM.TotalBien = preguntasRespuestas.Where(x => x.IdQuestion.HasValue && x.IsCorrect.HasValue && x.IsCorrect.Value).Count();

            vQVM.Bien = 0;
            vQVM.Mal = 0;

            foreach (var rSU in respSelectUser)
            {
                var origResp = preguntasRespuestas.Where(x => x.Id == rSU.IdSelect).Single();

                if(origResp.IsCorrect.HasValue && origResp.IsCorrect.Value)
                {
                    vQVM.Bien++;
                }
                else
                {
                    vQVM.Mal++;
                }

            }

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            String s = JsonConvert.SerializeObject(vQVM, Formatting.Indented, settings);

            return Json(s);

        }

        [HttpGet]        
        public ActionResult TiendaOnline()
        {
            String lang = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            TiendaOnlineViewModel TOVM = new TiendaOnlineViewModel();

            if (lang != "es" && lang != "en")
            {
                return View(TOVM);
            }

            SeackersContext context = new SeackersContext();

            Guid? langThread = context.Categorias.Where(x => !x.IdTema.HasValue && x.Titulo == lang)
                .Select(x => x.Id).SingleOrDefault();

            if (langThread != null)
            {
                var allCategories = context.Categorias //.Where(x => !x.Publico.HasValue || x.Publico.Value)
                    .Select(x => new Models.InApp.Categoria()
                {
                    Id = x.Id,
                    IdTema = x.IdTema,
                    IdPadre = x.IdPadre,
                    Nombre = x.Titulo
                }).ToList();

                List<Models.InApp.Categoria> categoriasFinal = new List<Models.InApp.Categoria>();
                Models.InApp.Categoria tP = allCategories.Where(x => x.IdTema == langThread).SingleOrDefault();
                                
                while (tP != null)
                {
                    categoriasFinal.Add(tP);
                    IterTemaSCA(tP, allCategories);
                    tP = allCategories.Where(x => x.IdPadre == tP.Id).SingleOrDefault();

                }
                
                
                TOVM.Categorias = categoriasFinal;

                return View(TOVM);
            }

            return View(TOVM);

        }

        private void IterTemaSCA(Models.InApp.Categoria tP, List<Models.InApp.Categoria> tS)
        {
            tP.CategoriasHIJAS = new List<Models.InApp.Categoria>();
            Models.InApp.Categoria tH = tS.Where(x => x.IdTema == tP.Id && !x.IdPadre.HasValue).SingleOrDefault();
            while (tH != null)
            {
                tP.CategoriasHIJAS.Add(tH);
                IterTemaSCA(tH, tS);
                var tHTemp = tS.Where(x => x.IdTema == tP.Id && x.IdPadre == tH.Id).SingleOrDefault();
                tH = tHTemp;

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetProducts(String categorias)
        {
            SeackersContext context = new SeackersContext();

            if (!String.IsNullOrEmpty(categorias))
            { 
                List<Guid> catgs = categorias.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => new Guid(x)).ToList();

                List<Guid> productsIds = context.ProdCats.Where(x => catgs.Contains(x.CategoriaId))
                    .Select(x => x.ProductId).Distinct().ToList();

                    List<Producto> products = context.ProductosBs.Where(x => productsIds.Contains(x.Id))
                    .Select(x => new Producto()
                    {
                        Id = x.Id,
                        Nombre = x.Nombre,
                        //Contenido = x.Contenido,
                        Precio = x.Pvp,
                        ImageUrl = x.UrlImgMain

                    }).ToList();

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                String s = JsonConvert.SerializeObject(new TiendaOnlineViewModel() { ProductosINIT = products }, Formatting.Indented, settings);

                return Json(s);
            }
            else
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                return Json(JsonConvert.SerializeObject(new TiendaOnlineViewModel() { ProductosINIT = new List<Producto>() }, Formatting.Indented, settings));
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetPCKsw(String CKsw)
        {
            SeackersContext context = new SeackersContext();

            if (!String.IsNullOrEmpty(CKsw))
            {
                List<String> prodsCantidadR = CKsw.Split('_', StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList();

                List<String[]> prodCan = prodsCantidadR.Select(x => x.Split(",", StringSplitOptions.RemoveEmptyEntries)).ToList();
                Dictionary<Guid, Int32> pC = prodCan.ToDictionary(x => new Guid(x[0]), x => Convert.ToInt32(x[1]));

                List<Producto> products = context.ProductosBs.Where(x => pC.Keys.Contains(x.Id))
                .Select(x => new Producto()
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    //Contenido = x.Contenido,
                    Precio = x.Pvp,
                    ImageUrl = x.UrlImgMain,
                    Cantidad = pC[x.Id]
                    

                }).ToList();

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                String s = JsonConvert.SerializeObject(new TiendaOnlineViewModel() { ProductosINIT = products }, Formatting.Indented, settings);

                return Json(s);
            }
            else
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                return Json(JsonConvert.SerializeObject(new TiendaOnlineViewModel() { ProductosINIT = new List<Producto>() }, Formatting.Indented, settings));
            }

        }

        
    }
}
