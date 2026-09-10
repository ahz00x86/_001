using System;
using System.Collections.Generic;

namespace PrimesNumbersSearchWeb.Models.DB
{
    public partial class Proceso
    {
        public DateTime Fecha { get; set; }
        public Guid Id { get; set; }
        public Guid Estado { get; set; }
        public string Cuenta { get; set; } = null!;
        public int? Pe { get; set; }
        public int? Pr { get; set; }
        public bool R { get; set; }
        public bool Pagado { get; set; }
    }
}
