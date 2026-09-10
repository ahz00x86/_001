using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace _666way
{
    public static class Utils
    {
        public static void SendMail(String emailTo, String subject, String body)
        {
            using (SmtpClient smtp = new SmtpClient())
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("publicidad@666way.com");
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("publicidad@666way.com", "gapFf@1WjRmH0xA");
                    smtp.Host = "smtp.ionos.es";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }

        /// <summary>
        /// Utility method for handling PayPal Responses
        /// </summary>
        public static string GetPayPalResponse(Dictionary<string, string> formVals, HttpRequestBase request)
        {
            bool useSandbox = false;

            string paypalUrl = useSandbox ? "https://www.sandbox.paypal.com/cgi-bin/webscr"
             : "https://www.paypal.com/cgi-bin/webscr";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(paypalUrl);

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            byte[] param = request.BinaryRead(request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);

            StringBuilder sb = new StringBuilder();
            sb.Append(strRequest);

            foreach (string key in formVals.Keys)
            {
                sb.AppendFormat("&{0}={1}", key, formVals[key]);
            }
            strRequest += sb.ToString();
            req.ContentLength = strRequest.Length;

            //Send the request to PayPal and get the response
            string response = "";
            using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
            {

                streamOut.Write(strRequest);
                streamOut.Close();
                using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                }
            }

            return response;
        }

    }
}