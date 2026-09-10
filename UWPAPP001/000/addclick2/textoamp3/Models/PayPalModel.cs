using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace textoamp3.Models
{
    public class PayPalModel
    {
        public List<PayPalUserPay> Payments { get; set; }
        
    }

    public class PayPalUserPay
    {
        public DateTime Fecha { get; set; }

        public Decimal Importe { get; set; }
    }
}