using addclick.apzyService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace addclick.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Data()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Data(String name, String oldPassword, String newPassword, String newPassword2)
        {            
            if (newPassword != null && newPassword.Length > 0 && oldPassword != null && oldPassword.Length > 0)
            {
                if (newPassword == newPassword2)
                {
                    using (IapzyServiceClient cli = new IapzyServiceClient("BasicHttpBinding_IapzyService"))
                    {
                        Boolean result = cli.UpdateData(
                            new DataGET() { Token = (Guid)Session["token"], FechaHora = DateTime.Now },
                            new DataUPDATE() { Nombre = name, OldPassword = oldPassword, NewPassword = newPassword });

                        if(result)
                        {
                            TempData["msgOK"] = "Sus datos se han cambiado correctamente";
                        }
                        else
                        {
                            TempData["msgKO"] = "Ha ocurrido un error al guardar sus datos";
                        }
                    }
                }
                else
                {
                    TempData["msgKO"] = "Las textos de nueva contraseña no coinciden";
                }
            }
            else
            {
                TempData["msgKO"] = "Para guardar sus datos en necesario actualizar su contraseña";
            }

            return View();
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(String email)
        {
            using (IapzyServiceClient cli = new IapzyServiceClient("BasicHttpBinding_IapzyService"))
            {
                if(cli.ForgotPassword(email))
                {
                    TempData["msgOK"] = "Le hemos mandado un e-mail con su nueva Contraseña";
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    TempData["msgKO"] = "Ha ocurrido un error, reinténtelo o contáctenos si es necesario";
                }
            }

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {            
            if (!String.IsNullOrEmpty(email) || !String.IsNullOrEmpty(password))
            {
                Session["email"] = email;

                Login l = new Login() { Email = email, Password = password, FechaHora = DateTime.Now };
                using (IapzyServiceClient cli = new IapzyServiceClient("BasicHttpBinding_IapzyService"))
                {
                    LoginReturn lR = cli.Login(l);

                    if (lR != null)
                    {
                        Session["token"] = lR.Token;
                        Session["name"] = lR.Name;

                        FormsAuthentication.SetAuthCookie(lR.Token.ToString(), false);
                        return RedirectToAction("textoamp3", "Home");
                    }
                    else
                    {
                        TempData["msgKO"] = "Sus datos de acceso no son correctos";
                    }
                }
            }

            return RedirectToAction("Login", "Login");
        }


        [HttpGet]
        public ActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(String name, String email)
        {
            name = name.Length > 31 ? name.Substring(0, 31) : name;

            using (IapzyServiceClient cli = new IapzyServiceClient("BasicHttpBinding_IapzyService"))
            {
                ViewBag.Message = true;
                if (cli.Register(new Register() { Nombre = name, Email = email }))
                {
                    TempData["msgOK"] = "Le hemos mandado un e-mail con su nueva Contraseña";
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    TempData["msgKO"] = "Ha ocurrido un error, contáctenos si le es necesario";
                }
            }

            return View();
        }

        //public static void Main()
        //{
        //    try
        //    {

        //        string original = "Password 12345";

        //        using (RijndaelManaged myRijndael = new RijndaelManaged())
        //        {
        //            Byte[] Key = Encoding.UTF8.GetBytes("00000000000000000000000000000001");
        //            Byte[] IV = Encoding.UTF8.GetBytes("xax_007,apzyx,te");

        //            // Encriptar
        //            Byte[] encrypted = Encrypt(original, Key, IV);

        //            // Desencriptar
        //            String roundtrip = Decrypt(encrypted, Key, IV);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error: {0}", e.Message);
        //    }
        //}
        

        //static string Decrypt(Byte[] cipherText, byte[] Key, byte[] IV)
        //{
        //    string plaintext = null;

        //    using (RijndaelManaged rijAlg = new RijndaelManaged())
        //    {
        //        rijAlg.Key = Key;
        //        rijAlg.IV = IV;

        //        ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

        //        using (MemoryStream msDecrypt = new MemoryStream(cipherText))
        //        {
        //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
        //                {
        //                    plaintext = srDecrypt.ReadToEnd();
        //                }
        //            }
        //        }
        //    }

        //    return plaintext;
        //}
    }
}