using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PrimesNumbersSearchWeb.Hubs;
using PrimesNumbersSearchWeb.Models;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace PrimesNumbersSearchWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<MessageHub> _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHubContext<MessageHub> context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Process(String prime, String user)
        {
            BigInteger pri = BigInteger.Parse(prime);
                        
            String hash = SHA512(prime);

            BigInteger exacta = -1;
            BigInteger raiz2 = raizCuadradaMasUnoNoExa(pri, out exacta, user);

            return Json(raiz2.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ProcessPrimeR2(String prime, String r2)
        {
            BigInteger pri = BigInteger.Parse(prime);

            Boolean esCompuesto = false;
            BigInteger divisorCompuesto = -1;
            for (BigInteger i = 2; i <= 100000000; i++)
            {
                BigInteger rem = BigInteger.Remainder(pri, i);

                if (rem == 0)
                {
                    esCompuesto = true;
                    divisorCompuesto = i;
                    break;
                }

            }



            return Json(esCompuesto);                        
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static BigInteger Sqrt(BigInteger number)
        {
            if (number < 9)
            {
                if (number == 0)
                    return 0;
                if (number < 4)
                    return 1;
                else
                    return 2;
            }

            BigInteger n = 0, p = 0;
            var high = number >> 1;
            var low = BigInteger.Zero;

            while (high > low + 1)
            {
                n = (high + low) >> 1;
                p = n * n;
                if (number < p)
                {
                    high = n;
                }
                else if (number > p)
                {
                    low = n;
                }
                else
                {
                    break;
                }
            }
            return number == p ? n : low;
        }


        private BigInteger raizCuadrada(BigInteger elemento, out BigInteger exacta0, String user)
        {
            String sEle = elemento.ToString();

            Stack<String> aSE = new Stack<String>();

            Int32 iAntL = -1;
            for (Int32 i = sEle.Length - 2; i > -1; i -= 2)
            {
                iAntL = i;
                aSE.Push(sEle.Substring(i, 2));
            }

            if (iAntL == 1)
            {
                aSE.Push(sEle.Substring(0, 1));
            }
            else if (iAntL == -1)
            {
                aSE.Push(sEle.Substring(0));
            }

            //httpContext.HttpContext.Session.SetInt32("ProgressTotal", aSE.Count);


            MessageHub mH = new MessageHub(_context);
            DateTime dAnt = DateTime.Now;
            var total = aSE.Count;
            Int32 anterior = -1;
            String resultString = String.Empty;
            String tempString = String.Empty;
            BigInteger s2 = BigInteger.Parse(aSE.Pop());
            Boolean isFirst = true;
            do
            {

                if (!isFirst)
                {
                    s2 = BigInteger.Parse(s2.ToString() + aSE.Pop());
                }
                else
                {
                    isFirst = false;
                }

                BigInteger m = String.IsNullOrEmpty(resultString) ? 1 : BigInteger.Parse((BigInteger.Parse(resultString) * 2) + "1");
                BigInteger mMax = m + 10;
                BigInteger mAnt = -1;
                BigInteger iAnt = -1;
                for (BigInteger i = 1; m < mMax; i++)
                {
                    BigInteger mult = m * i;
                    if (mult <= s2)
                    {
                        mAnt = mult;
                        iAnt = i;
                    }
                    else
                    {
                        break;
                    }
                    m++;
                }

                if (mAnt == -1)
                {
                    resultString += "0";
                    //s2 = 0;
                }
                else
                {
                    s2 -= mAnt;
                    resultString += iAnt;
                }

                //httpContext.HttpContext.Session.SetInt32("ProgressStatus", aSE.Count);

                var progressBar = 100 * (total - aSE.Count) / total;
                if (anterior != progressBar)
                {                    
                    //if (progressBar > 0)
                    //{
                    //    DateTime iter = DateTime.Now;
                    //    TimeSpan tS = iter - dAnt;
                    //    dAnt = iter;
                    //    BigInteger s = new BigInteger(tS.TotalMilliseconds);
                    //    BigInteger v = s / progressBar;
                    //    BigInteger f = (s * v) - v;

                    //    for (Int32 i = progressBar + 1; i <= 100; i++)
                    //    {
                    //        s = f;
                    //        v = s / i;
                    //        f = (s * v) - v;
                    //    }

                    //    f = f / 1000 / 60;

                    //    ProgressResponse TiempoR = new ProgressResponse() { 
                    //        IsProgressBar = false, Tiempo = "Tiempo estimado de espera: " + (f == 0 ? "Calculando" : (new TimeSpan(0, Convert.ToInt32(f.ToString()), 0)).ToString("d'd 'h'h 'm'm 's's'")) };
                    //    mH.SendMessage(user, JsonConvert.SerializeObject(TiempoR));
                    //}

                    ProgressResponse progressBarR = new ProgressResponse() { IsProgressBar = true, ProgressBar = progressBar };
                    mH.SendMessage(user, JsonConvert.SerializeObject(progressBarR));
                    
                    anterior = progressBar;
                }
            }
            while (aSE.Count > 0);
            exacta0 = s2;

            return BigInteger.Parse(resultString);
        }

        private BigInteger raizCuadradaMasUnoNoExa(BigInteger elemento, out BigInteger exacta0, String user)
        {
            return raizCuadrada(elemento, out exacta0, user) + (exacta0 == 0 ? 0 : 1);
        }

        private static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        
    }

    [Serializable]
    public class ProgressResponse
    {
        public Boolean IsProgressBar { get; set; }

        public Int32 ProgressBar { get; set; }

        public String Tiempo { get; set; }
    }

    [Serializable]
    public class ProcessResponse
    {
        public Boolean IsCompuesto { get; set; }
                
        public String Divisor { get; set; }
    }

}