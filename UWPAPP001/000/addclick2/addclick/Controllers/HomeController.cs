using addclick.apzyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;

namespace addclick.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult DESCARGAS()
        {
            return View();
        }

        [HttpGet]
        public ActionResult textoamp3()
        {
            if(!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult textoamp3(String titulo, String texto)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Login");
            }

            // Comprobamos saldo

            // Realizamos conversión en caso de saldo correcto

            // Devolvemos datos de mp3

            return View();
        }

        [HttpGet]        
        public ActionResult textoamp3DEMO()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult textoamp3DEMO(String email, String titulo, String texto)
        {
            if(texto.Length > 500)
            {
                texto = texto.Substring(0, 500);
            }

            texto = String.Concat("textoamp3.com convierte texto en mp3, su opción DEMO está limitada e incluye publicidad. ", texto, " textoamp3.com convierte texto en mp3, su opción DEMO está limitada e incluye publicidad.");

            return View();
        }

        public ActionResult Index()
        {            
            return View();
        }

        /// <summary>Método Acerca de la web</summary>        
        public ActionResult About()
        {
            ViewBag.Message = "Texto a MP3.";

            return View();
        }
    }
}