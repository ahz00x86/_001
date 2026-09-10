using HiveAPI.CS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Transactions;
using TokensWay.Models;

namespace TokensWay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        class response
        {
            public Boolean error { get; set; }
            public String data { get; set; }            
        }

        class MyModel
        {
            public String account { get; set; }
            public String from { get; set; }
            public String to { get; set; }
            public String memo { get; set; }
            public String symbol { get; set; }
            public String quantity { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Process(String account, Guid? guid)
        {
            if (guid.HasValue)
            {
                Boolean existsTransfer = false;
                var url = "https://history.hive-engine.com/accountHistory?account=tw-vault&symbol=BUDSX";
                
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        DateTime unMinutoDespues = DateTime.Now.AddMinutes(1);

                        while (!existsTransfer || DateTime.Now > unMinutoDespues)
                        {
                            Thread.Sleep(5000);
                            var httpResponse = await httpClient.GetAsync(url);
                            var responseBody = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            var jsonR = JsonConvert.DeserializeObject<IEnumerable<MyModel>>(responseBody);

                            if (jsonR != null)
                            {
                                foreach (var registro in jsonR)
                                {
                                    if (registro.from == account && registro.to == "tw-vault" && 
                                        registro.memo.Contains(guid.Value.ToString()) && registro.quantity == "500.000" && registro.symbol == "BUDSX")
                                    {
                                        existsTransfer = true;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (existsTransfer)// Si existe pago
                {
                    tokenswayContext context = new tokenswayContext(_configuration);
                    List<Proceso> procesos = context.Procesos.Where(p => p.Id == guid && p.Cuenta == account).OrderByDescending(o => o.Fecha).ToList();

                    if (procesos.Count == 1 && !procesos[0].Pe.HasValue && !procesos[0].Pr.HasValue)
                    {
                        Proceso p = procesos[0];                        
                        p.R = true;                        
                        context.Procesos.Update(p);                        
                        context.SaveChanges();

                        response r = new response() { error = false, data = guid.Value.ToString() };
                        return Json(r);
                    }
                    else
                    {
                        response r = new response() { error = true, data = "Existe un problema con el estado del juego" };
                        return Json(r);
                    }
                    
                }
                else
                {
                    response r = new response() { error = true, data = "Hemos esperado un minutos pero no se detecta el pago" };
                    return Json(r);
                }
            }
            else
            {
                try
                {
                    Guid newGuid = Guid.NewGuid();

                    // Guardar GUID en bbdd comor juegor nuevor con la fecha y el account
                    tokenswayContext context = new tokenswayContext(_configuration);
                    context.Procesos.Add(new Proceso() { Fecha = DateTime.Now, Cuenta = account, Id = newGuid, Estado = newGuid, R = false, Pagado = false });
                    context.SaveChanges();

                    response r = new response() { error = false, data = newGuid.ToString() };
                    return Json(r);
                }
                catch (Exception ex)
                {
                    response r = new response() { error = true, data = ex.Message };
                    return Json(r);
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DoorProcess(Int32 nP, Guid process, Guid state, String account)
        {
            //Obtener el número de puertas según el estado del proceso en bbdd
            tokenswayContext context = new tokenswayContext(_configuration);
            List<Proceso> procesos = context.Procesos.Where(p => p.Id == process).OrderByDescending(o => o.Fecha).ToList();

            Int32 nPuertasR = 9999;
            if (procesos.Count > 0)
            {
                if (procesos[0].Estado == state && !procesos[0].Pagado && procesos[0].Cuenta == account && !procesos[0].Pe.HasValue && !procesos[0].Pr.HasValue)
                {
                    nPuertasR = 3 + procesos.Count;
                }
                else
                {
                    response r = new response() { error = true, data = "EL ESTADO DEL PROCESO NO ES CORRECTO 0x001" };
                    return Json(r);
                }
            }
            else
            {
                response r = new response() { error = true, data = "NO SE DETECTARON PROCESOS" };
                return Json(r);
            }

            Random random = new Random();
            Int32 result = random.Next(1, nPuertasR);

            if (nP == result)
            {
                Guid newGuid = Guid.NewGuid();
                //TODO: Guardar GUID en bbdd comor nuevor registror y actualizar resultado correcto
                Proceso p = procesos[0];
                p.Pr = result;
                p.Pe = nP;
                p.R = true;
                p.Pagado = false;
                context.Procesos.Update(p);
                context.Procesos.Add(new Proceso() { Fecha = DateTime.Now, Cuenta = account, Id = p.Id, Estado = newGuid, R = true, Pagado = false });
                context.SaveChanges();

                response r = new response() { error = false, data = newGuid.ToString() };
                return Json(r);                                
            }
            else
            {
                Proceso p = procesos[0];
                p.Pr = result;
                p.Pe = nP;
                p.R = false;
                p.Pagado = true;
                context.Procesos.Update(p);
                context.SaveChanges();

                // MANDAMOS TWAY POR EL VALOR DE LOS ACIERTOS
                //var builder = WebApplication.CreateBuilder();
                //var key = builder.Configuration["keytkv"];
                var key = String.Empty;

                
#if (DEBUG)
                var builder = WebApplication.CreateBuilder();
                key = builder.Configuration["keytkv"];
#else
                key = _configuration["keytkv"];
#endif

                HttpClient httpClient = new HttpClient();
                CHived hived = new CHived(httpClient, "https://api.hive.blog");
                COperations.custom_json cJson = new COperations.custom_json();
                cJson.id = "ssc-mainnet-hive";
                cJson.required_auths = new string[] { "tw-vault" };
                cJson.required_posting_auths = new string[] { };
                cJson.json = "{\"contractName\":\"tokens\",\"contractAction\":\"issue\",\"contractPayload\":{\"symbol\":\"TWAY\",\"to\":\"" + account + "\",\"quantity\":\"" + procesos.Count + ".000\"}}";

                String s = hived.broadcast_transaction(new object[] { cJson }, new string[] { key });

                //String s2 = "";
                response r = new response() { error = false, data = Guid.Empty.ToString() };
                return Json(r);
            }            
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult TEST()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult tkvault(Guid process, Guid state, String account)
        {
            tokenswayContext context = new tokenswayContext(_configuration);
            List<Proceso> procesos = context.Procesos.Where(p => p.Id == process && p.Cuenta == account).OrderByDescending(o => o.Fecha).ToList();

            if (procesos.Count > 0)
            {
                if (procesos[0].Estado == state && procesos[0].Cuenta == account &&
                    procesos[0].R && !procesos[0].Pagado)
                {
                    Int32 reembolso = 500;
                    Int32 multiplicador = 2;
                    Boolean exiteTruco = false;

                    Boolean isFirst = true;
                    foreach (Proceso p in procesos)
                    {
                        if (!isFirst) 
                        {
                            if (p.Cuenta == account &&
                                p.Pe.HasValue && p.Pr.HasValue &&
                                p.Pe.Value == p.Pr.Value && p.R && !p.Pagado)
                            {
                                reembolso *= multiplicador;
                                multiplicador++;
                            }
                            else
                            {
                                exiteTruco = true;
                                break;
                            }
                        }
                        else
                        {
                            if (p.Cuenta == account && p.R && !p.Pagado)
                            {
                                String vamos = "bien";
                                
                            }
                            else
                            {
                                exiteTruco = true;
                                break;
                            }
                            isFirst = false;
                        }
                    }

                    if (!exiteTruco)
                    {
                        //var builder = WebApplication.CreateBuilder();
                        //var key = builder.Configuration["keytkv"];
                        var key = String.Empty;
                        #if (DEBUG)
                                                var builder = WebApplication.CreateBuilder();
                                                key = builder.Configuration["keytkv"];
                        #else
                                        key = _configuration["keytkv"];
                        #endif
                        procesos[0].Pagado = true;
                        context.Procesos.Update(procesos[0]);
                        context.SaveChanges();

                        // TRANSFERIMOS LAS GANANCIAS
                        HttpClient httpClient = new HttpClient();
                        CHived hived = new CHived(httpClient, "https://api.hive.blog");
                        COperations.custom_json cJson = new COperations.custom_json();
                        cJson.id = "ssc-mainnet-hive";
                        cJson.required_auths = new string[] { "tw-vault" };
                        cJson.required_posting_auths = new string[] { };
                        cJson.json = "{\"contractName\":\"tokens\",\"contractAction\":\"transfer\",\"contractPayload\":{\"symbol\":\"BUDSX\",\"to\":\"" + account + "\",\"quantity\":\"" + reembolso +".000\",\"memo\":\"www.TokensWay.com PJ:" + process.ToString() + "\"}}";

                        String s = hived.broadcast_transaction(new object[] { cJson }, new string[] { key });

                        response r = new response() { error = false, data = "JUEGO REEMBOLSADO" };
                        return Json(r);
                    }
                    else
                    {
                        response r = new response() { error = true, data = "ERROR AL REEMBOLSAR EL JUEGO ¿TRUCO O TRATO?" };
                        return Json(r);
                    }
                }
                else
                {
                    response r = new response() { error = true, data = "EL PROCESO YA HA SIDO REEMBOLSADO O SU ESTADO NO ES CORRECTO" };
                    return Json(r);
                }
            }
            else
            {
                response r = new response() { error = true, data = "NO SE DETECTARON PROCESOS"};
                return Json(r);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}