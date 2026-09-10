using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace universalsemiotics
{
    public static class Utils
    {
        public static string RandomString(int size)
        {
            char[] chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static void SendMail(String emailTo, String subject, String body)
        {
            try
            {
                using (SmtpClient smtp = new SmtpClient())
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("gestion@universalsemiotics.com");
                        mail.To.Add(emailTo);
                        mail.Subject = subject;
                        mail.Body = body;
                        mail.IsBodyHtml = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("gestion@universalsemiotics.com", ConfigurationManager.AppSettings["cor"]);
                        smtp.Host = "smtp.ionos.es";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public static String LimpiarHTML(String htmlS)
        {
            String regP = @"<(.|\n)*?>";
            return Regex.Replace(htmlS, regP, string.Empty);
        }

        public static String ExtractTextFromPDF(string filePath)
        {
            String texto = String.Empty;

            PdfReader pdfReader = new PdfReader(filePath);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                texto += PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
            }
            pdfDoc.Close();
            pdfReader.Close();

            return texto;
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

        public class RemotePost
        {
            private Dictionary<string, string> Inputs = new Dictionary<string, string>();
            public string Url = "";
            public string Method = "post";
            public string FormName = "form1";
            public StringBuilder strPostString;

            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }
            public void generatePostString()
            {
                strPostString = new StringBuilder();

                strPostString.Append("<html><head>");
                strPostString.Append("</head><body onload=\"document.form1.submit();\">");
                strPostString.Append("<form name=\"form1\" method=\"post\" action=\"" + Url + "\" >");

                foreach (KeyValuePair<string, string> oPar in Inputs)
                    strPostString.Append(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", oPar.Key, oPar.Value));

                strPostString.Append("</form>");
                strPostString.Append("</body></html>");
            }
            public void Post()
            {
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.Write(strPostString.ToString());
                System.Web.HttpContext.Current.Response.End();
            }
        }

        public class ReCaptchaValidationResult
        {
            public bool Success { get; set; }
            public string HostName { get; set; }
            [JsonProperty("challenge_ts")]
            public string TimeStamp { get; set; }
            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }

        public static ReCaptchaValidationResult IsValid(String captchaResponse)
        {
            if (string.IsNullOrWhiteSpace(captchaResponse))
            {
                return new ReCaptchaValidationResult()
                { Success = false };
            }

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://www.google.com");

            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>
            ("secret", ConfigurationManager.AppSettings["reC"]));
            values.Add(new KeyValuePair<string, string>
             ("response", captchaResponse));
            FormUrlEncodedContent content = new FormUrlEncodedContent(values);

            HttpResponseMessage response = client.PostAsync
            ("/recaptcha/api/siteverify", content).Result;

            string verificationResponse = response.Content.
            ReadAsStringAsync().Result;

            var verificationResult = JsonConvert.DeserializeObject
            <ReCaptchaValidationResult>(verificationResponse);

            return verificationResult;
        }
    }
}