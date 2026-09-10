using _666way.Models;
using Azure.Storage;
using Azure.Storage.Blobs;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _666way.Controllers
{
    public class HomeController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

            if (Request != null)
            {
                using (publicidad p = new publicidad())
                {
                    List<publicidadModel> publiI = p.publicidadI.Where(x => x.validado && x.publicado).OrderByDescending(x => x.precio).Select(x => new publicidadModel() { UrlD = x.urlD, UrlImage = x.image }).ToList();
                    if (publiI != null)
                    {
                        ViewBag.publiI = publiI;
                    }

                    List<publicidadModel> publiD = p.publicidadD.Where(x => x.validado && x.publicado).OrderByDescending(x => x.precio).Select(x => new publicidadModel() { UrlD = x.urlD, UrlImage = x.image }).ToList();
                    if (publiD != null)
                    {
                        ViewBag.publiD = publiD;
                    }
                }
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Presentacion()
        {
            return View();
        }

        public ActionResult Financiacion()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Musica()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PublicidadI()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PublicidadD()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PublicidadI(String urlD, String emailD)
        {
            if(Request.Files[0].InputStream.Length != 0 && !String.IsNullOrEmpty(urlD) && !String.IsNullOrEmpty(emailD ))
            {
                Guid id = Guid.NewGuid();
                String fileName = id.ToString();

                Uri blobUri = new Uri("https://666way.blob.core.windows.net/imgs" +
                                      "/" + fileName);
                Azure.Storage.StorageSharedKeyCredential storageCredentials =
                    new StorageSharedKeyCredential("666way", "VyA5iz29TxnI8ftIhjn+C+gly8efFodXcY3vimJg8AqCFY1hNpIjXknf88C1k9+LnT0A2sGhqjUlesKVyLCzHg==");
                BlobClient blobClient = new BlobClient(blobUri, storageCredentials);
                await blobClient.UploadAsync(Request.Files[0].InputStream);

                using (publicidad p = new publicidad())
                {
                    p.publicidadI.Add(new publicidadI() { Id = id, image = blobUri.ToString(), urlD = urlD, emailD = emailD });
                    p.SaveChanges();
                }

                String body = "<p> <a href='http://www.666way.com/Home/xdc4cb595729d341d6c5d12943a5a3371e7612528I?id32po=" + fileName + "'>ver</a><br/>" + urlD + "<br/>" + emailD + "</p>";

                Utils.SendMail("apzyx@yahoo.com", "NUEVA PUBLI", body);
            }
            else
            {

            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PublicidadD(String urlD, String emailD)
        {
            if (Request.Files[0].InputStream.Length != 0 && !String.IsNullOrEmpty(urlD) && !String.IsNullOrEmpty(emailD))
            {
                Guid id = Guid.NewGuid();
                String fileName = id.ToString();

                Uri blobUri = new Uri("https://666way.blob.core.windows.net/imgs" +
                                      "/" + fileName);
                Azure.Storage.StorageSharedKeyCredential storageCredentials =
                    new StorageSharedKeyCredential("666way", "VyA5iz29TxnI8ftIhjn+C+gly8efFodXcY3vimJg8AqCFY1hNpIjXknf88C1k9+LnT0A2sGhqjUlesKVyLCzHg==");
                BlobClient blobClient = new BlobClient(blobUri, storageCredentials);
                await blobClient.UploadAsync(Request.Files[0].InputStream);

                using (publicidad p = new publicidad())
                {
                    p.publicidadD.Add(new publicidadD() { Id = id, image = blobUri.ToString(), urlD = urlD, emailD = emailD });
                    p.SaveChanges();
                }

                String body = "<p> <a href='http://www.666way.com/Home/xdc4cb595729d341d6c5d12943a5a3371e7612528D?id32po=" + fileName + "'>ver</a><br/>" + urlD + "<br/>" + emailD + "</p>";

                Utils.SendMail("apzyx@yahoo.com", "NUEVA PUBLI", body);
            }
            else
            {

            }

            return View();
        }

        [HttpGet]
        public ActionResult xdc4cb595729d341d6c5d12943a5a3371e7612528I()
        {
            String id = Request.QueryString["id32po"];

            if (String.IsNullOrEmpty(id)) { return null; }

            publicidadModel pI = null;
            using (publicidad p = new publicidad())
            {
                pI = p.publicidadI.Where(x => x.Id == new Guid(id))
                        .Select(x => new publicidadModel() { Id = x.Id, UrlImage = x.image, UrlD = x.urlD }).Single();
            }

            return View(pI);
        }

        [HttpGet]
        public ActionResult xdc4cb595729d341d6c5d12943a5a3371e7612528D()
        {
            String id = Request.QueryString["id32po"];

            if (String.IsNullOrEmpty(id)) { return null; }

            publicidadModel pD = null;
            using (publicidad p = new publicidad())
            {
                pD = p.publicidadD.Where(x => x.Id == new Guid(id))
                        .Select(x => new publicidadModel() { Id = x.Id, UrlImage = x.image, UrlD = x.urlD }).Single();
            }

            return View(pD);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult xdc4cb595729d341d6c5d12943a5a3371e7612528I(String pps, String id)
        {
            if(pps == "holaadios")
            {
                using (publicidad p = new publicidad())
                {
                    publicidadI pI = p.publicidadI.Where(x => x.Id == new Guid(id)).Single();
                    pI.validado = true;
                    p.SaveChanges();

                    String body = "<h1>Banner Aceptado</h1>" +
                        "<a target='_blank' href=" + pI.urlD + "><img src=" + pI.image + " height='100' width='100' style='height: 100px; width: 100px;' /></a>" +
                        "<br/><br/><p>Ahora puede realizar la inversión que desee en su banner, recuerde, a mayor inversión mayor posición en la lista de banners</p>" +
                        "<p><a href='http://www.666way.com/Home/InversionBANNERI?id=" + id + "' target='_blank'>clica aquí para realizar su inversión</a></p>" +
                        "<br/><br/><p>Gracias por su tiempo.</p>" +
                        "<br/><br/><p><a href='http://www.666way.com' target='_blank'>666way.com</a></p>";

                    Utils.SendMail(pI.emailD, "Banner OK", body);
                }
            }

            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult xdc4cb595729d341d6c5d12943a5a3371e7612528D(String pps, String id)
        {
            if (pps == "holaadios")
            {
                using (publicidad p = new publicidad())
                {
                    publicidadD pD = p.publicidadD.Where(x => x.Id == new Guid(id)).Single();
                    pD.validado = true;
                    p.SaveChanges();

                    String body = "<h1>Banner Aceptado</h1>" +
                        "<a target='_blank' href=" + pD.urlD + "><img src=" + pD.image + " height='100' width='100' style='height: 100px; width: 100px;' /></a>" +
                        "<br/><br/><p>Ahora puede realizar la inversión que desee en su banner, recuerde, a mayor inversión mayor posición en la lista de banners</p>" +
                        "<p><a href='http://www.666way.com/Home/InversionBANNERD?id=" + id + "' target='_blank'>clica aquí para realizar su inversión</a></p>" +
                        "<br/><br/><p>Gracias por su tiempo.</p>" +
                        "<br/><br/><p><a href='http://www.666way.com' target='_blank'>666way.com</a></p>";

                    Utils.SendMail(pD.emailD, "Banner OK", body);
                }
            }

            return null;
        }

        public ActionResult InversionBANNERI()
        {
            if(Request.QueryString.Count == 1 && Request.QueryString.Keys[0] == "id")
            {
                String id = Request.QueryString["id"];

                if (String.IsNullOrEmpty(id)) { return null; }

                publicidadModel pI = null;
                using (publicidad p = new publicidad())
                {
                    pI = p.publicidadI.Where(x => x.Id == new Guid(id) && x.validado)
                            .Select(x => new publicidadModel() { Id = x.Id, UrlImage = x.image, UrlD = x.urlD }).Single();
                }

                return View(pI);
            }
            return null;
        }

        public ActionResult InversionBANNERD()
        {
            if (Request.QueryString.Count == 1 && Request.QueryString.Keys[0] == "id")
            {
                String id = Request.QueryString["id"];

                if (String.IsNullOrEmpty(id)) { return null; }

                publicidadModel pD = null;
                using (publicidad p = new publicidad())
                {
                    pD = p.publicidadD.Where(x => x.Id == new Guid(id) && x.validado)
                            .Select(x => new publicidadModel() { Id = x.Id, UrlImage = x.image, UrlD = x.urlD }).Single();
                }

                return View(pD);
            }
            return null;
        }

        [HttpPost]
        public void PayPal_IPNI()
        {
            //log.Info("IPN listener invoked");

            try
            {
                var formVals = new Dictionary<string, string>();
                formVals.Add("cmd", "_notify-validate");

                string response = Utils.GetPayPalResponse(formVals, Request);

                if (response == "VERIFIED")
                {
                    string transactionId = Request["txn_id"];
                    string sAmountPaid = Request["mc_gross"];
                    string orderId = Request["custom"];

                    //_logger.Info("IPN Verified for order " + orderID);

                    //validate the order
                    Decimal amountPaid = 0;
                    Decimal.TryParse(sAmountPaid, out amountPaid);

                    // check these first
                    bool verified = true;

                    string businessEmail = HttpUtility.UrlDecode(Request["business"]);
                    if (String.Compare(businessEmail, "apzyx@yahoo.com", true) != 0)
                        verified = false;

                    string currencyCode = HttpUtility.UrlDecode(Request["mc_currency"]);
                    if (String.Compare(currencyCode, "EUR", true) != 0)
                        verified = false;

                    string paymentStatus = HttpUtility.UrlDecode(Request["payment_status"]);
                    if (String.Compare(paymentStatus, "Completed", true) != 0)
                        verified = false;

                    //log.Info("Business : " + businessEmail);
                    //log.Info("currency : " + currencyCode);
                    //log.Info("payment status : " + paymentStatus);
                    //log.Info("amount valid : " + AmountPaidIsValid(order, amountPaid).ToString());

                    // process the transaction
                    if (verified)
                    {
                        // Añadir Banner Izquierdo
                        using (publicidad p = new publicidad())
                        {
                            publicidadI pI = p.publicidadI.Where(x => x.Id == new Guid(orderId)).Single();

                            pI.precio += amountPaid;
                            pI.publicado = true;

                            p.SaveChanges();
                        }
                    }
                    else
                    {
                        String body = "<h1>PayPal_IPN 666way ERROR</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            Request.ToString() +
                            "<br/><br/><p>Gracias por su tiempo.</p>" +
                            "<br/><br/><p><a href='http://www.666way.com' target='_blank'>666way.com</a></p>";

                        Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN 666way", body);
                    }
                }
                else
                {
                    String body = "<h1>PayPal_IPN 666way ERROR</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            Request.ToString() +
                            "<br/><br/><p>Gracias por su tiempo.</p>" +
                            "<br/><br/><p><a href='http://www.666way.com' target='_blank'>666way.com</a></p>";

                    Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN 666way", body);
                }
            }
            catch (Exception ex)
            {
                String body = "<h1>PayPal_IPN 666way ERROR</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            ex.ToString() +
                            "<br/><br/><p>Gracias por su tiempo.</p>" +
                            "<br/><br/><p><a href='http://www.666way.com' target='_blank'>666way.com</a></p>";

                Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN 666way", body);
            }
        }


        [HttpPost]
        public void PayPal_IPND()
        {
            //log.Info("IPN listener invoked");

            try
            {
                var formVals = new Dictionary<string, string>();
                formVals.Add("cmd", "_notify-validate");

                string response = Utils.GetPayPalResponse(formVals, Request);

                if (response == "VERIFIED")
                {
                    string transactionId = Request["txn_id"];
                    string sAmountPaid = Request["mc_gross"];
                    string orderId = Request["custom"];

                    //_logger.Info("IPN Verified for order " + orderID);

                    //validate the order
                    Decimal amountPaid = 0;
                    Decimal.TryParse(sAmountPaid, out amountPaid);

                    // check these first
                    bool verified = true;

                    string businessEmail = HttpUtility.UrlDecode(Request["business"]);
                    if (String.Compare(businessEmail, "apzyx@yahoo.com", true) != 0)
                        verified = false;

                    string currencyCode = HttpUtility.UrlDecode(Request["mc_currency"]);
                    if (String.Compare(currencyCode, "EUR", true) != 0)
                        verified = false;

                    string paymentStatus = HttpUtility.UrlDecode(Request["payment_status"]);
                    if (String.Compare(paymentStatus, "Completed", true) != 0)
                        verified = false;

                    //log.Info("Business : " + businessEmail);
                    //log.Info("currency : " + currencyCode);
                    //log.Info("payment status : " + paymentStatus);
                    //log.Info("amount valid : " + AmountPaidIsValid(order, amountPaid).ToString());

                    // process the transaction
                    if (verified)
                    {
                        // Añadir Banner Izquierdo
                        using (publicidad p = new publicidad())
                        {
                            publicidadD pD = p.publicidadD.Where(x => x.Id == new Guid(orderId)).Single();

                            pD.precio += amountPaid;
                            pD.publicado = true;

                            p.SaveChanges();
                        }
                    }
                    else
                    {
                        String body = "<h1>PayPal_IPN 666way ERROR</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            Request.ToString() +
                            "<br/><br/><p>Gracias por su tiempo.</p>" +
                            "<br/><br/><p><a href='http://www.666way.com' target='_blank'>666way.com</a></p>";

                        Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN 666way", body);
                    }
                }
                else
                {
                    String body = "<h1>PayPal_IPN 666way ERROR</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            Request.ToString() +
                            "<br/><br/><p>Gracias por su tiempo.</p>" +
                            "<br/><br/><p><a href='http://www.666way.com' target='_blank'>666way.com</a></p>";

                    Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN 666way", body);
                }
            }
            catch (Exception ex)
            {
                String body = "<h1>PayPal_IPN 666way ERROR</h1>" +
                            "<p>Response enviado:</p><br/><br/>" +
                            ex.ToString() +
                            "<br/><br/><p>Gracias por su tiempo.</p>" +
                            "<br/><br/><p><a href='http://www.666way.com' target='_blank'>666way.com</a></p>";

                Utils.SendMail("apzyx@yahoo.com", "PayPal_IPN 666way", body);
            }
        }
    }
}