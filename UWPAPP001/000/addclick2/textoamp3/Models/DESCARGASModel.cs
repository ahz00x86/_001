using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace textoamp3.Models
{
    public class DESCARGASModel
    {
        public List<DESCARGA> DESCARGAS { get; set; }
        
    }
    public class DESCARGA
    {
        public Guid Id { get; set; }

        public String Titulo { get; set; }

        public String Url { get; set; }

        public Boolean IsDemo { get; set; }

        public DateTime Fecha { get; set; }

        public Boolean IsConverting { get; set; }

    }
}