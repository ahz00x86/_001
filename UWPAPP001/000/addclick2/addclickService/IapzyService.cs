using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace addclickService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IapzyService
    {
        [OperationContract]
        Boolean Register(Register register);

        [OperationContract]
        LoginReturn Login(Login login);

        [OperationContract]
        Boolean ForgotPassword(String email);

        [OperationContract]
        String GetEmail(DataGET token);

        [OperationContract]
        Decimal? GetPrices(DataGET token);

        [OperationContract]
        Boolean UpdateData(DataGET token, DataUPDATE update);

        [OperationContract]
        List<DownloadMP3> GetMP3s(DataGET token);

        //...
    }

    /// <summary>Clase de datos del Registro</summary>
    [DataContract]
    public class Register
    {
        /// <summary>Obtiene o establece el nombre</summary>
        [DataMember]
        public String Nombre { get; set; }

        /// <summary>Obtiene o establece el email</summary>
        [DataMember]
        public String Email { get; set; }
    }

    /// <summary>Clase de datos Login</summary>
    [DataContract]
    public class Login
    {
        /// <summary>Obtiene o establece el email</summary>
        [DataMember]
        public String Email { get; set; }

        /// <summary>Obtiene o establece la contraseña</summary>
        [DataMember]
        public String Password { get; set; }

        /// <summary>Obtiene o establece la fecha y hora del login respecto a la web origen</summary>
        [DataMember]
        public DateTime FechaHora { get; set; }
    }

    /// <summary>Clase de datos Login de vuelta</summary>
    [DataContract]
    public class LoginReturn
    {
        /// <summary>Obtiene o establece los Datos Login</summary>
        [DataMember]
        public Login Login { get; set; }

        /// <summary>Obtiene o establece el Nombre</summary>
        [DataMember]
        public String Name { get; set; }

        /// <summary>Obtiene o establece el Token de sesión</summary>
        [DataMember]
        public Guid Token { get; set; }

        /// <summary>Obtiene o establece Fecha y hora de la operación</summary>
        [DataMember]
        public DateTime FechaHora { get; set; }
    }

    /// <summary>Clasa Data GET sesión</summary>
    [DataContract]
    public class DataGET
    {
        /// <summary>Obtiene o establece la Fecha y hora de la operación</summary>
        [DataMember]
        public DateTime FechaHora { get; set; }

        /// <summary>Obtiene o establece el Email de uso</summary>
        [DataMember]
        public String Email { get; set; }

        /// <summary>Obtiene o establece el Token de la operación</summary>
        [DataMember]
        public Guid Token { get; set; }
    }

    /// <summary>Clase de datos de actualización</summary>
    [DataContract]
    public class DataUPDATE
    {
        /// <summary>Obtiene o establece el Nombre a actualizar</summary>
        [DataMember]
        public String Nombre { get; set; }

        /// <summary>Obtiene o establece la Antigua contraseña</summary>
        [DataMember]
        public String OldPassword { get; set; }

        /// <summary>Obtiene o establece la Nueva contraseña</summary>
        [DataMember]
        public String NewPassword { get; set; }
    }

    [DataContract]
    public class DownloadMP3
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public Guid Id_User { get; set; }
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public String Url { get; set; }
        [DataMember]
        public Boolean IsDemo { get; set; }

    }
}
