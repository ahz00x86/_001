using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths
{
    internal class Program
    {
        static void Main(string[] args)
        {
            

            for(Int32 A = 0; A <= 360; A++)
            {
                Double Ar = A * Math.PI / 180;

                Console.WriteLine("A=" + A + "  sen=" + Math.Sin(Ar) + "  R=" + ((A/45)/(((Math.Sqrt(2)/2)-(1/2)) * 2)));
                Console.ReadKey();
            }


        }
    }
}
