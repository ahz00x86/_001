using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GWService
{
    internal class Program
    {        
        async static void Main(string[] args)
        {
            GWServer server = new GWServer();
            Console.ReadKey();
        }
    }
}
