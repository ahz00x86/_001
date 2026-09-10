using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace priRem
{
    class Program
    {
        static void Main(string[] args)
        {
            sucesionCreciente();
        }

        public static void sucesionCreciente()
        {
            BigInteger numero = 3;
            BigInteger resultado = 0;

            while (true)
            {
                resultado = BigInteger.Add(resultado, numero);

                BigInteger remainder = 0;
                BigInteger div = BigInteger.DivRem(resultado, numero, out remainder);

                if (remainder == 0)
                {
                    addTextEscritorio("[" + (numero).ToString() + "]  +" + (resultado).ToString() + "+", "_PriREMsucCre_");
                    resultado = div;
                }

                numero = BigInteger.Add(numero, 2);
            }
        }

        public static void sucesionCreciente2()
        {
            BigInteger numero = 2;
            BigInteger resultado = 0;

            while (true)
            {
                resultado = BigInteger.Add(resultado, numero);

                BigInteger remainder = 0;
                BigInteger div = BigInteger.DivRem(resultado, numero, out remainder);

                if (remainder == 0)
                {
                    addTextEscritorio("[" + (numero).ToString() + "]  +" + (resultado).ToString() + "+", "_PriREMsucCre_");
                    resultado = div;
                }

                numero = BigInteger.Add(numero, 1);
            }
        }

        private static void addTextEscritorio(String text, String name)
        {
            using (StreamWriter writer = new StreamWriter(@"C:\Users\apzyx\OneDrive\Escritorio\" + name + ".txt", true))
            {
                writer.WriteLine(text.ToString());
            }
        }
    }
}
