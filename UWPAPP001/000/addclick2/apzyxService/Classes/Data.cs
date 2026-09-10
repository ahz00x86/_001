using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace apzyxService.Classes
{
    /// <summary>Clase Token para tiempo de acceso de usuario</summary>
    public class TokenUserTime
    {
        /// <summary>Obtiene o establece la fecha y hora de aplicación</summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Obtiene o establece el email del usuario del token
        /// </summary>
        public String Email { get; set; }

        /// <summary>Obtiene o establece el usuario acorde al token</summary>
        public Guid User { get; set; }
    }

    public static class Methods
    {
        public static async Task SendMail(String emailTo, String subject, String body)
        {
            MailMessage mail = new MailMessage("textoamp3@textoamp3.com", emailTo);
            SmtpClient client = new SmtpClient();
            client.Port = 25; //587
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("textoamp3@textoamp3.com", "password");
            client.Host = "smtp..com";
            client.EnableSsl = true;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = UTF8Encoding.UTF8;
            mail.Subject = subject;
            mail.Body = body;
            client.Send(mail);
        }

        public static string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

        public static Byte[] Encrypt(string plainText, byte[] Key)
        {
            byte[] encrypted;

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                Byte[] IV = Encoding.UTF8.GetBytes("xax_007,apzyx,te");
                rijAlg.IV = IV;
                rijAlg.Key = Key;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        public static Boolean ValidateBytes(Byte[] bs1, Byte[] bs2)
        {
            if(bs1 == null || bs2 == null || bs1.Length == 0 || bs2.Length == 0)
            {
                return false;
            }

            Int32 iMinL = bs1.Length > bs2.Length ? bs2.Length : bs1.Length;

            for (Int32 i = 0; i < iMinL; i++)
            {
                if (bs1[i] != bs2[i]) { return false; }
            }

            return true;
        }
    }
}