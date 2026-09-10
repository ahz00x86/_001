using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using universalsemiotics.Models;

namespace universalsemiotics.Controllers
{
    public class ServiceController : Controller
    {
        [HttpPost]
        public JsonResult Hola(String pH)
        {
            if (pH == ConfigurationManager.AppSettings["hol"])
            {
                using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
                {
                    List<HolaModel> procesos = bbdd.Mp3Free.Where(x => !x.estado.HasValue).OrderBy(x => x.fechaIni).Select(x => new HolaModel(){ Id = x.Id, Idioma = x.idioma }).Take(2).ToList();
                    return Json(procesos);
                }
            }

            return Json(null);
        }

        [HttpPost]
        public Boolean Adios(String id, String pA)
        {
            if (pA == ConfigurationManager.AppSettings["adi"])
            {
                using (universalsemioticsEntities bbdd = new universalsemioticsEntities())
                {
                    Mp3Free mp3 = bbdd.Mp3Free.Where(x => x.Id == new Guid(id) && !x.estado.HasValue).SingleOrDefault();
                    mp3.estado = true;
                    mp3.fechaFin = DateTime.Now;
                    bbdd.SaveChanges();

                    ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                            .GetUserManager<ApplicationUserManager>()
                            .FindById(mp3.UserId.ToString());

                    String bodyMail4 = ""
                            + "<p>Su texto con título " + mp3.titulo + " está disponible.</p>"
                            + "<p>Gracias por su tiempo</p>"
                            + "Puede descargarlo aquí <a href='https://www.universalsemiotics.com/es/Descargas'>www.universalsemiotics.com/es/Descargas</a><br /><br/>"
                            + "<p>Your " + mp3.titulo + " title text is available.</p>"
                            + "<p>Thanks for your time</p>"
                            + "You can download it here <a href='https://www.universalsemiotics.com/en/Downloads'>www.universalsemiotics.com/en/Downloads</a>";
                    Utils.SendMail(user.Email, "OK - universalsemiotics.com", bodyMail4);


                    return true;
                }
            }

            return false;
        }
    }
}