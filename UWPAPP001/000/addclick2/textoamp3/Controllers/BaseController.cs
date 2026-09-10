using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using textoamp3.Models;

namespace textoamp3.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationUserManager _userManager;
        public _LayoutModel model;

        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

            if (Request != null && Request.IsAuthenticated)
            {
                // Obtener prices desde email de usuario
                string userId = User.Identity.GetUserId();
                var user = UserManager.FindById(userId);
                String mail = user.Email.ToLower();
                using (addclickEntities addclickEntities = new addclickEntities())
                {
                    Users_Data2 uDB = addclickEntities.Users_Data2.Where(x => x.email == mail).SingleOrDefault();
                    if (uDB != null)
                    {
                        ViewBag.publi = uDB.fecha.HasValue && uDB.fecha > DateTime.Now ? false : true;
                    }
                }
            }
        }
        
        public ApplicationUserManager UserManager
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
    }
}