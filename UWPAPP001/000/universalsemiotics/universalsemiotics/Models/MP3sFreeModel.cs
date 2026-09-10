using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace universalsemiotics.Models
{
    public class MP3sModel
    {
        public List<MP3FreeModel> DescargasFree { get; set; }

        public List<MP3ProModel> DescargasPro { get; set; }

    }
    public class MP3FreeModel
    {
        public Guid Id { get; set; }

        public String Titulo { get; set; }
                                
        public DateTime Fecha { get; set; }

        public Int64 Cola { get; set; }

        public Boolean EnCola { get; set; }

    }

    public class MP3ProModel
    {
        public Guid Id { get; set; }

        public String Titulo { get; set; }

        public DateTime Fecha { get; set; }

        public Boolean EnProceso { get; set; }

    }
}