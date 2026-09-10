using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using seackers.Models;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using seackers.Models.InApp;

namespace seackers.Controllers
{
    [Authorize(Roles = "Centro")]
    public class CenterController : Controller
    {
        private readonly ILogger<Ad001Controller> _logger;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CenterController(ILogger<Ad001Controller> logger,
            IStringLocalizer<HomeController> localizer,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _localizer = localizer;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<IActionResult> Users()
        {
            var centerId = _userManager.GetUserId(User);

            SeackersContext context = new SeackersContext();
            var InitusuariosIdsAccept = context.CenterUsers.Where(x => x.CenterId == centerId).Select(x => new KeyValuePair<String, Boolean>( x.UserId, x.Accepted ?? false )).ToDictionary(x => x.Key, x => x.Value);

            List<IdentityUser> usuariosIds = new List<IdentityUser>();
            foreach (var userIdAccept in InitusuariosIdsAccept.Keys)
            {
                var user = await _userManager.FindByIdAsync(userIdAccept);

                if (user == null)
                {
                    //TODO: Notificar inconsistencia to the admin
                }
                else
                {
                    // Comprobar si el usuario pertenece al rol
                    if (await _userManager.IsInRoleAsync(user, "Usuario"))
                    {
                        usuariosIds.Add(user);
                    }
                }
            }

            var dataUsers = context.DataUsers.Where(x => usuariosIds.Select(x => x.Id).Contains(x.UserId)).ToList();

            List<UsuarioSeacker> usersSea = new List<UsuarioSeacker>();
            foreach (IdentityUser iU in usuariosIds)
            {
                if (InitusuariosIdsAccept[iU.Id])
                {
                    DataUser dU = dataUsers.Where(x => x.UserId == iU.Id).SingleOrDefault();
                    UsuarioSeacker c = new UsuarioSeacker()
                    {
                        Id = iU.Id,
                        Name = dU != null ? dU.Name ?? "" : "",
                        Email = iU.Email,
                        UrlLogo = dU != null ? dU.ImgLogoUrl ?? "" : "",
                        Accepted = true,
                        Lock = dU != null ? dU.Lock ?? false : false,
                    };
                    usersSea.Add(c);
                }
                else
                {
                    UsuarioSeacker c = new UsuarioSeacker()
                    {
                        Id = iU.Id,                        
                        Email = iU.Email,                        
                        Accepted = false                        
                    };
                    usersSea.Add(c);
                }

                
            }

            var model = new UsersViewModel { UsuariosSeacker = usersSea };
            return View(model);
        }

        public async Task<IActionResult> AddUser(String NewUserEmail, Boolean Notificar)
        {
            if (ModelState.IsValid)
            {
                String myId = _userManager.GetUserId(User);

                SeackersContext context = new SeackersContext();

                var user = await _userManager.FindByEmailAsync(NewUserEmail);

                if (user == null)
                {
                    user = CreateUser();
                    await _userStore.SetUserNameAsync(user, NewUserEmail, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, NewUserEmail, CancellationToken.None);
                    String password = "Se@0" + Guid.NewGuid().ToString();
                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);
                        await _userManager.AddToRoleAsync(user, "Usuario");

                        
                        context.DataUsers.Add(new DataUser() { UserId = user.Id, Name = NewUserEmail });
                        context.SaveChanges();                                                
                    }
                }
                else
                {
                    if(!(await _userManager.IsInRoleAsync(user, "Usuario")))
                    {
                        await _userManager.AddToRoleAsync(user, "Usuario");
                    }
                }

                if(!context.CenterUsers.Where(x => x.CenterId == myId && x.UserId == user.Id).Any())
                {
                    Guid key = Guid.NewGuid();

                    context.CenterUsers.Add(new CenterUser() { CenterId = myId, UserId = user.Id, Started = DateTime.Now, Key = key });
                    context.SaveChanges();

                    if (true)
                    {
                        var host = _httpContextAccessor.HttpContext.Request.Host;
                        var esquema = _httpContextAccessor.HttpContext.Request.Scheme;
                        //var pathBase = _httpContextAccessor.HttpContext.Request.PathBase;

                        // Construye la URL completa
                        var url = $"{esquema}://{host}/User/ConfirmUserCenter?code="+ myId + "_" + key.ToString() + "_" + user.Id;

                        //var callbackUrl = Url.Page(
                        //    "/Home/ConfirmCenter",
                        //    pageHandler: null,
                        //    values: new { code = myId + "_" + key.ToString() + "_" + user.Id },
                        //    protocol: Request.Scheme);

                        var dataCenter = context.DataUsers.Where(x => x.UserId == myId).SingleOrDefault();

                        if (dataCenter != null)
                        {
                            await _emailSender.SendEmailAsync(NewUserEmail, "Confirmación registro en Centro Se@cker",
                                "El centro Se@cker " + dataCenter.Name + $" quiere incluirle en su registro de usuarios, para aceptar <a target='_blank' href='{HtmlEncoder.Default.Encode(url)}'>clique aquí</a><br/>" +
                                "<br/>Datos del centro:<br/>" + dataCenter.Name + "<br/>Tlf: " + (dataCenter.Phone ?? " - ") + "<br/>web: " + (dataCenter.Url ?? " - ") +
                                "<br/><br/>Si usted no ha hecho la petición al centro asociado, no tiene que tomar ninguna acción, aunque si lo desea puede contactar con nosotros a este mismo correo."+
                                "<br/><a target='_blank' href='https://www.seackers.com'>Se@ckers</a>");
                        }
                    }

                }

                return RedirectToAction("Users");
            }
            else
            {
                return RedirectToAction("Users");
            }
        }

    }
}
