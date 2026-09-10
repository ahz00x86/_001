using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace textoamp3
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
            //MailMessage mail = new MailMessage("", emailTo);
            //SmtpClient client = new SmtpClient();
            //client.Port = 587; //587 25
            //client.Credentials = new NetworkCredential("", "");
            //client.Host = "smtp.ionos.es";
            //client.EnableSsl = true;
            //mail.IsBodyHtml = true;
            //mail.BodyEncoding = UTF8Encoding.UTF8;
            //mail.Subject = subject;
            //mail.Body = body;
            //client.Send(mail);

            using (SmtpClient smtp = new SmtpClient())
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("ayuda@conversordetextoamp3.com");
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("ayuda@conversordetextoamp3.com", "gapFf@1WjRmH0xA");
                    smtp.Host = "smtp.ionos.es";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;                    
                    smtp.Send(mail);
                }
            }
        }

        public static String LimpiarHTML(String htmlS)
        {
            String regP = @"<(.|\n)*?>";
            return Regex.Replace(htmlS, regP, string.Empty);
        }

        public static void CheckAddBinPath()
        {
            // find path to 'bin' folder
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            // get current search path from environment
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";

            // add 'bin' folder to search path if not already present
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }
    }
}