using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using textoamp3.Models;

namespace textoamp3.Controllers
{
    [Authorize]
    public class PayPalController : BaseController
    {
        public ActionResult PayPal()
        {
            string userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            String mail = user.Email.ToLower();

            PayPalModel model = new PayPalModel();
            using (addclickEntities entities = new addclickEntities())
            {
                List<PayPalUserPay> paypals = entities.Paypals.Where(x => x.mail == mail).Select(x => new PayPalUserPay() { Fecha = x.fecha, Importe = x.prices }).ToList();
                model.Payments = paypals;
            }

            ViewBag.email = mail;

            return View(model);
        }

        [AllowAnonymous]
        public EmptyResult PayPalPaymentNotification(PayPalCheckoutInfo payPalCheckoutInfo)
        {
            PayPalListenerModel model = new PayPalListenerModel();
            model._PayPalCheckoutInfo = payPalCheckoutInfo;
            byte[] parameters = Request.BinaryRead(Request.ContentLength);

            if (parameters != null)
            {
                model.GetStatus(parameters);
            }

            return new EmptyResult();
        }
    }
}