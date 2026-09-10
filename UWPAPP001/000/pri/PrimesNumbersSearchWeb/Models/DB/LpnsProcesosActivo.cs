using System;
using System.Collections.Generic;

namespace PrimesNumbersSearchWeb.Models.DB
{
    public partial class LpnsProcesosActivo
    {
        public string Numero { get; set; } = null!;
        public int Colision { get; set; }
    }
}
