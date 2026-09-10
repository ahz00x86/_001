using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BigIntegersCalculator
{
    public static class CalculoAlgebraico
    {
        public static String CalculoAritmetico(String formula, Boolean existeN, BigInteger nDesde, BigInteger nHasta, Boolean esGrafica)
        {
            List<BigInteger> calculoGrafica = new List<BigInteger>();
            StringBuilder stb = new StringBuilder();
            stb.AppendLine(formula +" >");
            formula = formula.Replace(" ", "");
            List<String> parametros = new List<String>();
            BigInteger cohesionParentesis = 0;
            Boolean existenParentesis = false;
            String num = String.Empty;
            
            if(!existeN)
            {
                nDesde = 1;
                nHasta = 1;
            }
            else
            {
                if(nHasta < nDesde)
                {
                    throw new Exception("'n Desde' debe ser menor o igual que 'n Hasta' / 'n From' it must be less than or equal to 'n Until' ");

                }
            }

            for (BigInteger n = nDesde; n <= nHasta; n++) 
            {
                parametros = new List<String>();
                cohesionParentesis = 0;
                existenParentesis = false;
                num = String.Empty;

                for (Int32 i = 0; i < formula.Length; i++)
                {
                    if (!(new List<Char>() { ')', '(', '*', '/', '^', '+', '-', 'n' }).Contains(formula[i]) && !Char.IsNumber(formula[i]))
                    {
                        throw new Exception("Revise su expresión, hay operadores o letras no admitidas / Review your expression, there are operators or letters not supported");
                    }
                }

                for (Int32 i = 0; i < formula.Length; i++)
                {
                    if (formula[i] == 'n')
                    {
                        if (i != 0 && Char.IsNumber(formula[i - 1]))
                        {
                            parametros.Add(num);
                            parametros.Add("*");
                            num = String.Empty;
                            parametros.Add(n.ToString());
                        }
                        else if ((i == 1 && formula[i - 1] == '-') || (i > 1 && formula[i - 1] == '-' && (new List<Char>() { '(', '*', '/', '^', '+', '-' }).Contains(formula[i - 2])))
                        {
                            num += 1;
                            parametros.Add(num);
                            parametros.Add("*");
                            parametros.Add(n.ToString());
                            num = String.Empty;
                        }
                        else
                        {
                            parametros.Add(n.ToString());
                        }
                        
                        continue;
                    }

                    if (formula[i] == '(' || formula[i] == ')')
                    {
                        cohesionParentesis += (formula[i] == '(' ? 1 : -1);
                        if (cohesionParentesis < 0) { throw new Exception("Existe un error en la forma de los Paréntesis / There is an error in the form of the Parentheses"); }
                        existenParentesis = true;
                    }

                    if (Char.IsNumber(formula[i]) || (i == 0 && (formula[i] == '-' || formula[i] == '+')) || (i > 0 && (formula[i] == '-' || formula[i] == '+') && ((new List<Char>() { '(', '*', '/', '^', '+', '-' }).Contains(formula[i - 1]))))
                    {
                        num += formula[i];
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(num))
                        {
                            parametros.Add(formula[i].ToString());
                        }
                        else
                        {
                            parametros.Add(num.ToString());
                            parametros.Add(formula[i].ToString());
                            num = String.Empty;
                        }
                    }
                }
                if (!String.IsNullOrEmpty(num))
                {
                    parametros.Add(num.ToString());
                }

                if (!cohesionParentesis.IsZero)
                {
                    throw new Exception("Existe un error en el número de Paréntesis / There is an error in the number of Parentheses");
                }


                Boolean resuelta = false;
                while (!resuelta)
                {
                    if (existenParentesis)
                    {
                        Int32 indicePri = 0;
                        Int32 indiceFin = 0;
                        for (Int32 i = 0; i < parametros.Count; i++)
                        {
                            if (parametros[i] == "(" || parametros[i] == ")")
                            {
                                if (parametros[i] == "(")
                                {
                                    indicePri = i;
                                }
                                else if (parametros[i] == ")")
                                {
                                    indiceFin = i;
                                    break;
                                }
                                else
                                {
                                    throw new Exception("Error 747, nunca debería haber hecho esto =)");
                                }
                            }
                        }

                        if (indicePri != 0 || indiceFin != 0)
                        {
                            List<String> resolucion = resolucionFormulaSimple(parametros.GetRange(indicePri + 1, indiceFin - indicePri - 1));
                            parametros.RemoveRange(indicePri, indiceFin - indicePri + 1);
                            parametros.InsertRange(indicePri, resolucion);
                        }
                        else
                        {
                            throw new Exception("Error 747, nunca debería haber hecho esto =)");
                        }

                        existenParentesis = existenParentesisM(parametros);
                    }
                    else
                    {
                        parametros = resolucionFormulaSimple(parametros);
                    }

                    if (parametros.Count == 1) { resuelta = true; }
                }
                if (!existeN)
                {
                    return parametros[0];
                }
                else
                {
                    if (esGrafica)
                    {
                        calculoGrafica.Add(n);
                        calculoGrafica.Add(BigInteger.Parse(parametros[0]));
                    }
                    stb.AppendLine("n = " + n.ToString() + " > " + parametros[0]);
                }
            }

            if(esGrafica)
            {
                String resultadoGrafica = String.Empty;

                for (Int32 i = 0; i < calculoGrafica.Count; i++)
                {
                    if(i % 2 == 1)
                    {
                        resultadoGrafica += - 1 * calculoGrafica[i];
                        resultadoGrafica += " ";
                    }
                    else
                    {
                        resultadoGrafica += calculoGrafica[i] + ",";
                    }
                }

                resultadoGrafica.Trim();
                return resultadoGrafica;
            }

            return stb.ToString();
        }


        private static List<String> resolucionFormulaSimple(List<String> formulaSimple)
        {
            List<String> temp = new List<String>();
            Boolean resuelta = false;
            while (!resuelta)
            {
                Boolean existenPotencias = existenPotenciasM(formulaSimple);
                Boolean existenMultDiv = existenMultDivM(formulaSimple);

                if (existenPotencias)
                {
                    for (Int32 i = 0; i < formulaSimple.Count; i++)
                    {
                        if (formulaSimple[i] == "^")
                        {
                            BigInteger result = BigInteger.Pow(BigInteger.Parse(formulaSimple[i - 1]), Convert.ToInt32(formulaSimple[i + 1]));
                            formulaSimple.RemoveRange(i - 1, 3);
                            formulaSimple.Insert(i - 1, result.ToString());
                        }
                    }
                }
                else if (existenMultDiv)
                {
                    for (Int32 i = 0; i < formulaSimple.Count; i++)
                    {
                        if (formulaSimple[i] == "*")
                        {
                            BigInteger result = BigInteger.Multiply(BigInteger.Parse(formulaSimple[i - 1]), BigInteger.Parse(formulaSimple[i + 1]));
                            formulaSimple.RemoveRange(i - 1, 3);
                            formulaSimple.Insert(i - 1, result.ToString());
                        }
                        else if (formulaSimple[i] == "/")
                        {
                            BigInteger rem = 0;
                            BigInteger result = BigInteger.DivRem(BigInteger.Parse(formulaSimple[i - 1]), BigInteger.Parse(formulaSimple[i + 1]), out rem);
                            if (!rem.IsZero) { throw new Exception("Las divisiones directas o indirectas, tiene que ser exactas para un resultado exacto / Direct or indirect divisions must be exact for an exact result"); }
                            formulaSimple.RemoveRange(i - 1, 3);
                            formulaSimple.Insert(i - 1, result.ToString());
                        }
                    }
                }
                else
                {
                    for (Int32 i = 0; i < formulaSimple.Count; i++)
                    {
                        if (formulaSimple[i] == "+")
                        {
                            BigInteger result = BigInteger.Add(BigInteger.Parse(formulaSimple[i - 1]), BigInteger.Parse(formulaSimple[i + 1]));
                            formulaSimple.RemoveRange(i - 1, 3);
                            formulaSimple.Insert(i - 1, result.ToString());
                        }
                        else if (formulaSimple[i] == "-")
                        {
                            BigInteger result = BigInteger.Subtract(BigInteger.Parse(formulaSimple[i - 1]), BigInteger.Parse(formulaSimple[i + 1]));
                            formulaSimple.RemoveRange(i - 1, 3);
                            formulaSimple.Insert(i - 1, result.ToString());
                        }
                    }
                }

                if (formulaSimple.Count == 1) { resuelta = true; }
            }

            return formulaSimple;
        }

        /// <summary>Método que recorre los paramétros de la operación para comprobar si siguen quedando paréntesis en la misma</summary>
        private static Boolean existenParentesisM(List<String> validarParentesis)
        {
            foreach (String c in validarParentesis)
            {
                if (c == "(") { return true; }
            }
            return false;
        }

        /// <summary>Método que recorre los paramétros de la operación para comprobar si siguen quedando Potencias en la misma</summary>
        private static Boolean existenPotenciasM(List<String> validarParentesis)
        {
            foreach (String c in validarParentesis)
            {
                if (c == "^") { return true; }
            }
            return false;
        }

        /// <summary>Método que recorre los paramétros de la operación para comprobar si siguen quedando Multiplicaciones o Divisiones en la misma</summary>
        private static Boolean existenMultDivM(List<String> validarParentesis)
        {
            foreach (String c in validarParentesis)
            {
                if (c == "*" || c == "/") { return true; }
            }
            return false;
        }
    }
}
