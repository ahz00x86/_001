using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace universalsemiotics.Models
{
    /// <summary>Modelo de uso para los banners de publicidad</summary>
    public class PublicidadModel
    {
        /// <summary>Obtiene o establece el id del banner</summary>
        public Guid Id { get; set; }

        /// <summary>Obtiene o establece el id del usuario</summary>
        public Guid IdUser { get; set; }

        /// <summary>Obtiene o establece el título o tag del banner</summary>
        public String Titulo { get; set; }

        /// <summary>Obtiene o establece la extensión de la imagen</summary>
        public String Extension { get; set; }

        /// <summary>Obtiene o establece el banner destino</summary>
        public String Banner { get; set; }

        /// <summary>Obtiene o establece la url de la imagen del banner</summary>
        public String UrlImage
        { 
            get
            {
                return String.Concat("https://universalsemiotics.blob.core.windows.net/publicidad/", Id, ".", Extension);
            } 
        }
        
        /// <summary>Obtiene o establece la url de destino del banner</summary>
        public String UrlD { get; set; }

        /// <summary>Obtiene o establece el precio de puja actual</summary>
        public Decimal Prices { get; set; }

    }
}