using System;
using System.Numerics;
using System.Reflection;

public class Aritmetica
{
    public BigInteger Sum2(BigInteger a, BigInteger b) { return a + b; }
    public BigInteger Res2(BigInteger a, BigInteger b) { return a - b; }
    public BigInteger Mul2(BigInteger a, BigInteger b) { return a * b; }
    public BigInteger Div2(BigInteger a, BigInteger b, out BigInteger remainder) { return BigInteger.DivRem(a, b, out remainder); }
    public BigInteger Pot2(BigInteger a, BigInteger b) { Int32 bInt = Int32.Parse(b.ToString()); return BigInteger.Pow(a, bInt); }

    public BigInteger Operacion(String methodName, BigInteger a, BigInteger b)
    {        
        MethodInfo methodInfo = typeof(Aritmetica).GetMethod(methodName);
                
        object result = methodInfo.Invoke(this, new object[2] { a, b });

        return (BigInteger)result;
    }
}