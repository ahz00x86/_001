using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using seackers.Models;
using seackers.Models.InApp;

namespace seackers.Controllers
{
    public class ViewController : Controller
    {
        private readonly IStringLocalizer<ViewController> _localizer;
        private readonly UserManager<IdentityUser> _userManager;

        public ViewController(IStringLocalizer<ViewController> localizer, UserManager<IdentityUser> userManager)
        {
            _localizer = localizer;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(String nombreCentro)
        {

            if (!String.IsNullOrEmpty(nombreCentro) && nombreCentro.Length <= 120)
            {
                SeackersContext context = new SeackersContext();

                try
                {
                    var dataCentro = context.DataUsers.Where(x => x.Name == nombreCentro).SingleOrDefault();

                    if (dataCentro != null)
                    {
                        Boolean? myLike = null;

                        if(User != null && User.IsInRole("Usuario"))
                        {
                            IdentityUser user = await _userManager.GetUserAsync(User);
                            var likebbdd = context.CenterLikes.Where(x => x.UserId == user.Id && x.CenterId == dataCentro.UserId).Select(x => x.Like).SingleOrDefault();
                            myLike = likebbdd;                         
                        }

                        ViewSeackerCenter siC = new ViewSeackerCenter() { 
                            Id = dataCentro.UserId,
                            Name = dataCentro.Name,
                            Phone = dataCentro.Phone,
                            WebSite = dataCentro.Url,
                            Logo = dataCentro.ImgLogoUrl,
                            Head = dataCentro.ImgHeadUrl,
                            Latitud = dataCentro.Latitud.HasValue ? dataCentro.Latitud.Value.ToString() : "",
                            Longitud = dataCentro.Longitud.HasValue ? dataCentro.Longitud.Value.ToString() : "",
                            Likes = context.CenterLikes.Where(x => x.CenterId == dataCentro.UserId && x.Like.HasValue && x.Like.Value).LongCount(),   
                            MyLike = myLike
                        };

                        ViewData["Title"] = nombreCentro;

                        return View(new ViewViewModel() { Center = siC });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch(Exception ex)
                {
                    //TODO: NOTIFICAR CENTROS MISMO NOMBRE

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
    }
}
