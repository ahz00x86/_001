using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace universalsemiotics.Models
{
    public class HolaModel
    {
        /// <summary>Obtiene o establece el id del proceso hola</summary>
        public Guid Id { get; set; }

        /// <summary>Obtiene o establece el idioma del proceso hola</summary>
        public String Idioma { get; set; }
    }
}