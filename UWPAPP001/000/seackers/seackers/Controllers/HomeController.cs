using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using seackers.Models;
using seackers.Models.InApp;
using System.Data;
using System.Diagnostics;
using System.Security.Policy;
using System.Text.Encodings.Web;

namespace seackers.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {            
            _logger = logger;
            _localizer = localizer;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Home Page";
            //TempData["hello"] = _localizer["init"];

            var ids = (await _userManager.GetUsersInRoleAsync("Centro")).Select(x => x.Id);

            SeackersContext context = new SeackersContext();
            
    //        .Join(db.Pedidos, u => u.Id, p => p.IdUsuario, (u, p) => new { Usuario = u, Pedido = p })
    //.Where(up => up.Pedido.IdUsuario == idUsuario)
    //.Select(up => new { Usuario = up.Usuario.Nombre, CantidadPedidos = db.Pedidos.Count(p => p.IdUsuario == idUsuario) });
                
            var centros = context.DataUsers.Where(x => ids.Contains(x.UserId) && x.Latitud.HasValue && x.Longitud.HasValue)                
                .Select(x => new SeackerIndexCenter() { 
                    Name = x.Name, Phone = x.Phone, WebSite = x.Url,  Logo = x.ImgLogoUrl,
                    Latitud = x.Latitud.ToString(), Longitud = x.Longitud.ToString(), 
                    Likes = context.CenterLikes.Where(l => l.CenterId == x.UserId && l.Like.HasValue && l.Like.Value).LongCount() }).ToList();

            IndexViewModel iVM = new IndexViewModel() { Centers = centros };

            return View(iVM);
        }
                
        public IActionResult Privacy()
        {
            ViewData["Title"] = _localizer["Privacy"];
            return View();
        }
                
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {            
            //await _emailSender.SendEmailAsync("apzyx@yahoo.com", "Error Se@cker",
            //                    HttpContext.TraceIdentifier);

            return RedirectToAction("Index");
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}