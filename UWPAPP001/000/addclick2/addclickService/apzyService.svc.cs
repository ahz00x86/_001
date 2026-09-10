using addclickService.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace addclickService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class apzyService : IapzyService
    {

        public Boolean Register(Register register)
        {
            if (register == null || String.IsNullOrEmpty(register.Email) || !register.Email.Contains('@'))
            {
                return false;
            }

            String[] ud = register.Email.ToLower().Split('@');
            if (ud.Length != 2)
            {
                return false;
            }

            String u = ud[0];
            String d = ud[1];
            String newPassword = Methods.RandomString(12);
            Guid id = Guid.NewGuid();
            using (addclickEntities e = new addclickEntities())
            {
                User ud3 = e.Users.Where(ud2 => ud2.u == u && ud2.d == d).SingleOrDefault();

                if (ud3 != null)
                {
                    Byte[] newPass = Methods.Encrypt(newPassword, Encoding.UTF8.GetBytes(ud3.id.ToString().Replace("-", "")));
                    ud3.Users_Data.password = newPass;
                }
                else
                {
                    e.Users.Add(new User() { u = u, d = d, id = id });
                    e.Users_Data.Add(new Users_Data() { id = id, nombre = register.Nombre, password = Methods.Encrypt(newPassword, Encoding.UTF8.GetBytes(id.ToString().Replace("-", ""))) });
                }

                e.SaveChanges();
            }

            String body = @"<h1>textoamp3.com</h1>
                            <h3>BIENVENIDO</h3>
                            <br/><p>Para acceder al servicio textoamp3.com puede usar su nueva contraseña: " + newPassword + @"</p>
                            <br /><br /><p>Podrá configurar su contraseña en la sección 'Mis datos' después de iniciar sesión</p>
                            <br /><br /><p>Gracias por su tiempo.</p>
                            <p>textoamp3.com</p>";

            // Mandar por e-mail
            //Methods.SendMail(register.Email, "BIENVENIDO - textoamp3.com", body);

            return true;
        }

        public LoginReturn Login(Login login)
        {
            if (login == null || String.IsNullOrEmpty(login.Email) || String.IsNullOrEmpty(login.Password) || !login.Email.Contains('@'))
            {
                return null;
            }

            String[] ud = login.Email.ToLower().Split('@');
            if (ud.Length != 2)
            {
                return null;
            }

            String u = ud[0];
            String d = ud[1];

            using (addclickEntities e = new addclickEntities())
            {
                User u3 = e.Users.Where(ud2 => ud2.u == u && ud2.d == d).SingleOrDefault();

                if (u3 == null)
                {
                    return null;
                }

                if (!Methods.ValidateBytes(Methods.Encrypt(login.Password, Encoding.UTF8.GetBytes(u3.id.ToString().Replace("-", ""))), u3.Users_Data.password))
                {
                    return null;
                }

                login.Password = String.Empty;
                String name = u3.Users_Data.nombre;
                Guid token = Guid.NewGuid();

                HttpContext.Current.Application[token.ToString()] = new TokenUserTime() { Date = login.FechaHora, User = u3.id, Email = login.Email.ToLower() };

                LoginReturn lR = new LoginReturn() { Login = login, Name = name, Token = token };
                return lR;
            }
        }

        public Boolean ForgotPassword(String email)
        {
            if (email == null || String.IsNullOrEmpty(email) || !email.Contains('@'))
            {
                return false;
            }

            String[] ud = email.ToLower().Split('@');
            if (ud.Length != 2)
            {
                return false;
            }

            String u = ud[0];
            String d = ud[1];
            String newPassword = Methods.RandomString(12);

            using (addclickEntities e = new addclickEntities())
            {
                User ud3 = e.Users.Where(ud2 => ud2.u == u && ud2.d == d).SingleOrDefault();

                if (ud3 == null)
                {
                    Guid id = Guid.NewGuid();
                    e.Users.Add(new User() { u = u, d = d, id = id });
                    e.Users_Data.Add(new Users_Data() { id = id, password = Methods.Encrypt(newPassword, Encoding.UTF8.GetBytes(id.ToString().Replace("-", ""))) });
                }
                else
                {
                    Byte[] newPass = Methods.Encrypt(newPassword, Encoding.UTF8.GetBytes(ud3.id.ToString().Replace("-", "")));
                    ud3.Users_Data.password = newPass;
                }

                e.SaveChanges();
            }

            String body = @"<h1>textoamp3.com</h1>
                            <h3>CONTRASEÑA REESTABLECIDA</h3>
                            <br/><p>Para acceder al servicio textoamp3.com puede usar su nueva contraseña: " + newPassword + @"</p>
                            <br /><br /><p>Podrá configurar su contraseña en la sección 'Mis datos' después de iniciar sesión</p>
                            <br /><br /><p>Gracias por su tiempo.</p>
                            <p>textoamp3.com</p>";

            // Mandar por e-mail
            //Methods.SendMail(email, "CONTRASEÑA REESTABLECIDA - textoamp3.com", body);

            return true;
        }

        public String GetEmail(DataGET token)
        {
            TokenUserTime tUt = (TokenUserTime)HttpContext.Current.Application[token.Token.ToString()];
            if (tUt.Email != token.Email)
            {
                return null;
            }

            tUt.Date = token.FechaHora;
            HttpContext.Current.Application[token.Token.ToString()] = tUt;

            using (addclickEntities e = new addclickEntities())
            {
                User us = e.Users.Where(u => u.id == tUt.User).SingleOrDefault();

                return String.Concat(us.u, "@", us.d);
            }
        }

        public Decimal? GetPrices(DataGET token)
        {
            TokenUserTime tUt = (TokenUserTime)HttpContext.Current.Application[token.Token.ToString()];
            if (tUt.Email != token.Email)
            {
                return null;
            }

            tUt.Date = token.FechaHora;
            HttpContext.Current.Application[token.Token.ToString()] = tUt;

            using (addclickEntities e = new addclickEntities())
            {
                Users_Data usD = e.Users_Data.Where(u => u.id == tUt.User).SingleOrDefault();

                return usD.prices;
            }
        }

        public Boolean UpdateData(DataGET token, DataUPDATE update)
        {
            TokenUserTime tUt = (TokenUserTime)HttpContext.Current.Application[token.Token.ToString()];
            if (tUt.Email != token.Email)
            {
                return false;
            }

            tUt.Date = token.FechaHora;
            HttpContext.Current.Application[token.Token.ToString()] = tUt;

            using (addclickEntities e = new addclickEntities())
            {
                Users_Data ud3 = e.Users_Data.Where(ud2 => ud2.id == tUt.User).SingleOrDefault();

                if (ud3 == null)
                {
                    return false;
                }

                if (Methods.ValidateBytes(Methods.Encrypt(update.OldPassword, Encoding.UTF8.GetBytes(ud3.id.ToString().Replace("-", ""))), ud3.password))
                {
                    return false;
                }

                Byte[] newPass = Methods.Encrypt(update.NewPassword, Encoding.UTF8.GetBytes(ud3.id.ToString().Replace("-", "")));
                ud3.password = newPass;
                ud3.nombre = update.Nombre;
                e.SaveChanges();
            }

            return true;
        }

        public List<DownloadMP3> GetMP3s(DataGET token)
        {
            TokenUserTime tUt = (TokenUserTime)HttpContext.Current.Application[token.Token.ToString()];
            if (tUt.Email != token.Email)
            {
                return null;
            }

            tUt.Date = token.FechaHora;
            HttpContext.Current.Application[token.Token.ToString()] = tUt;

            using (addclickEntities e = new addclickEntities())
            {
                List<DownloadMP3> ud3 = e.Users_MP3.Where(ud2 => ud2.id_User == tUt.User)
                    .Select(x => new DownloadMP3()
                    {
                        Id = x.id,
                        Id_User = x.id_User,
                        Name = x.titulo,
                        Url = x.url,
                        IsDemo = x.precio == null
                    }).ToList();

                return ud3;
            }
        }
    }
}
