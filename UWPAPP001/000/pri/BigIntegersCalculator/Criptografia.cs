using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BigIntegersCalculator
{
    public static class Criptografia
    {
        /// <summary>Cambiar un número de base decimal a base descrita en target_base</summary>
        public static string FromBase10(String number, int target_base)
        {

            if (target_base < 2 || target_base > 36) return "";
            if (target_base == 10) return number.ToString();

            int n = target_base;
            BigInteger q = BigInteger.Parse(number);
            BigInteger r;
            string rtn = "";

            while (q >= n)
            {

                r = q % n;
                q = q / n;

                if (r < 10)
                    rtn = r.ToString() + rtn;
                else
                    rtn = Convert.ToChar((Int32)(r + 55)).ToString() + rtn;

            }

            if (q < 10)
                rtn = q.ToString() + rtn;
            else
                rtn = Convert.ToChar((Int32)(q + 55)).ToString() + rtn;

            return rtn;

        }

        public static String ToBase10(String number, int start_base)
        {
            if (start_base < 2 || start_base > 36) return "";
            if (start_base == 10) return number;

            char[] chrs = number.ToCharArray();
            int m = chrs.Length - 1;
            BigInteger n = start_base;
            BigInteger x;
            BigInteger rtn = 0;

            foreach (char c in chrs)
            {

                if (char.IsNumber(c))
                    x = int.Parse(c.ToString());
                else
                    x = Convert.ToInt32(c) - 55;

                rtn += x * BigInteger.Pow(n, m);

                m--;

            }

            return rtn.ToString();

        }

        /// <summary>Método que convierte un número decimal BigInteger en otro en String con base dada en función de sus dígitos o caracteres</summary>
        public static String BaseX(Char[] digitosBaseORI, String numeroBaseORI, Char[] digitosBaseFIN)
        {
            String resultado = "0";            
            for (Int32 iORI = 0; iORI < numeroBaseORI.Length; iORI++)
            {
                for(Int32 iDigORI = 0; iDigORI < digitosBaseORI.Length; iDigORI++)
                {
                    if(numeroBaseORI[numeroBaseORI.Length - 1 - iORI] == digitosBaseORI[iDigORI])
                    {
                        List<Char> resultadoTemp = new List<Char>() { };
                        Char c = '0';
                        //Int32 subCuentas = 0;
                        //for (Int32 iDigFIN = 0; iDigFIN < iDigORI; iDigFIN++)
                        //{
                        //    if (iDigFIN != 0 && iDigFIN % digitosBaseFIN.Length == 0)
                        //    {
                        //        subCuentas++;
                        //    }
                        //    c = digitosBaseFIN[iDigFIN % digitosBaseFIN.Length];
                        //}
                        //resultado.Add(c);
                        BigInteger resolucionCuentas = BigInteger.Parse(iDigORI.ToString());
                        while (resolucionCuentas > 0)
                        {
                            Int32 subCuentas = 0;
                            for (Int32 iCuentas = 0; iCuentas <= resolucionCuentas; iCuentas++)
                            {
                                if (iCuentas != 0 && iCuentas % digitosBaseFIN.Length == 0)
                                {
                                    subCuentas++;                                    
                                }
                                c = digitosBaseFIN[iCuentas % digitosBaseFIN.Length];
                            }
                            resultadoTemp.Add(c);
                            resolucionCuentas = subCuentas;
                        }
                        resultadoTemp.Reverse();
                        String resultadoTS = String.Empty;
                        resultadoTemp.ForEach(x => resultadoTS += x);

                        for(Int32 j = 0; j <= digitosBaseORI.Length * iORI; j++)
                        {
                            resultado += "0";
                        }

                        resultado = SumBaseX(digitosBaseFIN, resultado, resultadoTS);

                        break;
                    }
                }
            }

            return resultado;
        }


        public static String SumBaseX(Char[] digitosBase, String n1, String n2)
        {
            // Obtenemos el tamaño del string más largo
            Int32 maxL = 0;
            if (n1.Length > n2.Length)
            {
                maxL = n1.Length;
            }
            else
            {
                maxL = n2.Length;
            }

            // Proceso de suma de strings
            Boolean acarreo = false;
            Boolean acarreoAnterior = false;
            Char cPosicion1 = digitosBase[0];
            Char cPosicion2 = digitosBase[0];
            List<Char> resultado = new List<Char>(); ;
            for (Int32 i = 0; i < maxL; i++)
            {
                acarreo = false;
                cPosicion1 = digitosBase[0];
                Int32 j1 = n1.Length - 1 - i;
                if (j1 >= 0)
                {
                    cPosicion1 = n1[j1];
                }
                cPosicion2 = digitosBase[0];
                Int32 j2 = n2.Length - 1 - i;
                if (j2 >= 0)
                {
                    cPosicion2 = n2[j2];
                }
                for (Int32 iCP1 = 0; iCP1 < digitosBase.Length; iCP1++)
                {
                    if (digitosBase[iCP1] == cPosicion1)
                    {
                        for (Int32 iCP2 = 0; iCP2 < digitosBase.Length; iCP2++)
                        {
                            Int32 iDop = iCP1 + iCP2 + (acarreoAnterior ? 1 : 0);

                            if (iDop != 0 && iDop % digitosBase.Length == 0)
                            {
                                acarreo = true;
                            }

                            if (digitosBase[iCP2] == cPosicion2)
                            {
                                resultado.Add(digitosBase[iDop % digitosBase.Length]);
                                acarreoAnterior = acarreo;
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            if (acarreo) { resultado.Add(digitosBase[1]); }

            String resultadoS = String.Empty;
            resultado.Reverse();
            resultado.ForEach(x => resultadoS += x);

            return resultadoS;


        }
    }
}
