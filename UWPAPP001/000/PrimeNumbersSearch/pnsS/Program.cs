using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace pnsS
{
    internal class Program
    {
        #region COSMOSDB
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private static String databaseId = "ToDoList";
        private static String containerId = "Items";
        #endregion


        static ConcurrentDictionary<BigInteger, String> ayudaIPsN = new ConcurrentDictionary<BigInteger, String>();
        static ConcurrentDictionary<BigInteger, BigInteger[]> ayudaInternacional = new ConcurrentDictionary<BigInteger, BigInteger[]>();
        
        static async Task Main(string[] args)
        {
            // Probar función raiz cuadrada
            //BigInteger n = 2500;
            //while (true)
            //{
            //    BigInteger result = raizCuadradaMasUno(n);

            //    n++;
            //}

            //BigInteger[] d = busquedaIterativa(108);
            //BigInteger result = raizCuadradaMasUno(100);
            //return;

            Program p = new Program();                
            BigInteger seguirPor = await p.inicializar();
            await p.seguirLaBusqueda(seguirPor);
            

        }
                
        public  async Task<BigInteger> inicializar()
        {
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            container = cosmosClient.GetContainer(databaseId, containerId);

            // BUSCAMOS EL PRIMO MáXIMO CONOCIDO (Tener en cuenta que el 2 es obviado y no puede estar en el proceso)
            BigInteger maxP = BigInteger.Parse(await GetMaxItemAsync());

            // DEVOLVEMOS EL SIGUIENTE NúMERO
            return maxP + 1;                        
        }

        private async Task<String> GetMaxItemAsync()
        {
            var sqlQueryText = "SELECT MAX(LENGTH(c.id)) FROM c";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<Parameter> queryResultSetIterator = container.GetItemQueryIterator<Parameter>(queryDefinition);

            String numberS = String.Empty;
            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Parameter> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Parameter nS in currentResultSet)
                {
                    numberS = nS.d1;
                    Console.WriteLine("\tRead {0}\n", nS.d1);
                }
            }

            sqlQueryText = "SELECT MAX(c.id) FROM c where LENGTH(c.id) = " + numberS;
            queryDefinition = new QueryDefinition(sqlQueryText);
            queryResultSetIterator = container.GetItemQueryIterator<Parameter>(queryDefinition);

            numberS = String.Empty;
            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Parameter> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Parameter nS in currentResultSet)
                {
                    numberS = nS.d1;
                    Console.WriteLine("\tRead {0}\n", nS.d1);
                }
            }


            return numberS;
        }

        private async Task<NumberSearch> GetItemByIdAsync(BigInteger n)
        {
            try
            {                
                ItemResponse<NumberSearch> nSResponse = await container.ReadItemAsync<NumberSearch>(n.ToString(), PartitionKey.None);
                return nSResponse.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }


        private async Task seguirLaBusqueda(BigInteger seguirPor)
        {            
            BigInteger vamosPor = seguirPor;
            Boolean ayudaErratica = false;
            Boolean seguir = true;
            while (seguir)
            {                
                if (!ayudaErratica && ayudaInternacional.ContainsKey(vamosPor))
                {
                    BigInteger[] d;
                    if (ayudaInternacional.TryRemove(vamosPor, out d))
                    {                        
                        if (d != null)
                        {
                            if (!d.Contains(1) && !d.Contains(vamosPor))
                            {
                                ayudaErratica = false;
                                BigInteger div = vamosPor;
                                for (Int64 i = 0; i < d.LongLength; i++)
                                {
                                    BigInteger resto;
                                    div = BigInteger.DivRem(div, (BigInteger)d.GetValue(i), out resto);
                                    if (resto != 0)
                                    {
                                        ayudaErratica = true;
                                        break;
                                    }
                                }

                                if (!ayudaErratica)
                                {
                                    // ES UN NUMERO COMPUESTO CON DIVISOR d.Value (Información externa correcta)

                                    // ES COMPUESTO
                                    await compuestoFactorizado(vamosPor, d);
                                }
                            }
                            else
                            {
                                ayudaErratica = true;
                            }
                        }
                        else
                        {
                            // Comprobamos
                            d = busquedaIterativa(vamosPor);
                            // SI d == null ES UN NUMERO PRIMO (Información externa correcta)
                            if (d == null)
                            {
                                // ES PRIMO
                                await primoEncontrado(vamosPor);
                            }
                            else
                            {
                                ayudaErratica = true;
                            }
                        }
                    }
                    else
                    {
                        // ALGO PASó CON EL DATO
                        ayudaErratica = true;
                    }
                                        
                    if (ayudaErratica)
                    {
                        // REALIZAMOS DE NUEVO EL BUCLE SIN TENER EN CUENTA LOS DATOS FACILITADOS
                        continue;
                    }
                }
                else
                {
                    ayudaErratica = false;
                    BigInteger[] dMain = busquedaIterativa(vamosPor);

                    // SI dMain ES NULL ES NúMERO PRIMO, EN CASO CONTRARIO, ES UN NÚMERO COMPUESTO

                    if (dMain != null)
                    {
                        // ES COMPUESTO
                        await compuestoFactorizado(vamosPor, dMain);
                    }
                    else
                    {
                        // ES PRIMO
                        await primoEncontrado(vamosPor);
                    }

                }

                // INCREMENTAMOS EL IMPAR COMO POSIBLE PRIMO
                vamosPor += 1;
            }
        }

        private static BigInteger[] busquedaIterativa(BigInteger elemento)
        {
            List<BigInteger> divisores = new List<BigInteger>();

            BigInteger elementoAValidar = elemento;
            Boolean seguir = true;
            while (seguir)
            {
                BigInteger raiz2M3 = raizCuadradaMasUno(elementoAValidar);
                for (BigInteger i = 2; i < raiz2M3; i++)
                {
                    BigInteger resto = -1;
                    BigInteger div = BigInteger.DivRem(elementoAValidar, i, out resto);

                    if (resto == 0)
                    {                        
                        if (div != 1)
                        {
                            divisores.Add(i);
                            elementoAValidar = div;
                            i = 1;
                        }
                    }
                }

                if(elementoAValidar != 1 && elementoAValidar != elemento)
                {
                    divisores.Add(elementoAValidar);
                }

                seguir = false;

            }

            return divisores.Count > 0 ? divisores.ToArray() : null;            
        }

        private async Task primoEncontrado(BigInteger primo)
        {
            String pS = primo.ToString();
            Console.WriteLine("\tRead {0}\n", pS);
            await container.CreateItemAsync<dynamic>(new { id = pS, isP = true});
        }

        private async Task compuestoFactorizado(BigInteger compuesto, BigInteger[] divisores)
        {
            String cS = compuesto.ToString();
            Console.WriteLine("\tRead {0}\n", cS);
            String divisoresS = String.Empty;
            foreach (BigInteger d in divisores)
            {
                divisoresS += String.Concat(d.ToString(), ",");
            }
            divisoresS = divisoresS.Substring(0, divisoresS.Length - 1);

            //NumberSearch nS = new NumberSearch() { id = compuesto.ToString(), d = divisoresS};
            
            await container.CreateItemAsync<dynamic>(new { id = cS, d = divisoresS });            
        }

        private static BigInteger raizCuadradaMasUno(BigInteger elemento)
        {
            return raizCuadrada(elemento) + 1;
        }

        private static BigInteger raizCuadrada(BigInteger elemento)
        {
            String sEle = elemento.ToString();

            Stack<String> aSE = new Stack<String>();

            Int32 iAntL = -1;
            for (Int32 i = sEle.Length - 2; i > -1; i -= 2)
            {
                iAntL = i;
                aSE.Push(sEle.Substring(i, 2));
            }

            if(iAntL == 1)
            {
                aSE.Push(sEle.Substring(0, 1));
            }
            else if (iAntL == -1)
            {
                aSE.Push(sEle.Substring(0));
            }

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
                }
                else
                {
                    s2 -= mAnt;
                    resultString += iAnt;
                }                                
            } 
            while (aSE.Count > 0);

            return BigInteger.Parse(resultString);
        }

    }

    public class NumberSearch
    {        
        public String id { get; set; }        
        public String d { get; set; }
        public Boolean isP { get; set; }
        public Boolean v { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Parameter
    {
        [JsonProperty(PropertyName = "$1")]
        public String d1 { get; set; }
    }
}
