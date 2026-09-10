using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace FileToVideo_VideoToFile
{
    public static class SDK
    {
        public static async Task<Byte[]> WBToBytes(WriteableBitmap writableBitmap)
        {
            using (SoftwareBitmap sftBM = SoftwareBitmap.CreateCopyFromBuffer(
                    writableBitmap.PixelBuffer,
                    BitmapPixelFormat.Rgba8,
                    writableBitmap.PixelWidth,
                    writableBitmap.PixelHeight))
            {

                using (var stream = new InMemoryRandomAccessStream())
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                    encoder.SetSoftwareBitmap(sftBM);
                    await encoder.FlushAsync();
                    var bytes = new byte[stream.Size];
                    await stream.AsStream().ReadAsync(bytes, 0, bytes.Length);

                    return bytes;
                }
            }
        }
        public static Boolean CambioEnGradiente(Int32 c, Int32 oC, Int32 diferenciaC)
        {
            if (c == oC)
            {
                return false;
            }
            else if (c > oC && c > (oC + diferenciaC))
            {
                return true;
            }
            else if (c < oC && c < (oC - diferenciaC))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<Byte[]> GetBNImage(WriteableBitmap wb)
        {
            Byte[] bytes = await SDK.WBToBytes(wb);

            for (Int32 j = 54; j < bytes.Length; j += 4)
            {
                var b = bytes[j];
                var g = bytes[j + 1];
                var r = bytes[j + 2];
                var a = bytes[j + 3];

                // Obtenemos la media para Blanco y Negro
                var averageValue = ((int)r + (int)b + (int)g) / 3;

                bytes[j] = Convert.ToByte(averageValue);
                bytes[j + 1] = Convert.ToByte(averageValue);
                bytes[j + 2] = Convert.ToByte(averageValue);
            }

            return bytes;
        }

        public static Boolean[,] GetBorderMatrix(Int32 h, Int32 w, List<Int32[]> colorsMatrix, Int32 diferenciaC)
        {
            // Tenemos los datos coincidentes
            if (h == colorsMatrix.Count)
            {
                Boolean[,] bordesMatrix = new Boolean[h, w];
                for (Int32 row = 0; row < h; row++)
                {
                    for (Int32 col = 0; col < w; col++)
                    {
                        Int32 actualC = colorsMatrix[row][col];

                        // Arriba
                        Int32 aCArriba = ((row + 1) < h) ? colorsMatrix[row + 1][col] : -1;
                        if (aCArriba != -1 && SDK.CambioEnGradiente(actualC, aCArriba, diferenciaC))
                        {
                            bordesMatrix[row + 1, col] = true;
                        }

                        // Derecha
                        Int32 acDerecha = ((col + 1) < w) ? colorsMatrix[row][col + 1] : -1;
                        if (acDerecha != -1 && SDK.CambioEnGradiente(actualC, acDerecha, diferenciaC))
                        {
                            bordesMatrix[row, col + 1] = true;
                        }

                        // Arriba Derecha
                        Int32 acArribaDerecha = ((col + 1) < w) && ((row + 1) < h) ? colorsMatrix[row + 1][col + 1] : -1;
                        if (acArribaDerecha != -1 && SDK.CambioEnGradiente(actualC, acArribaDerecha, diferenciaC))
                        {
                            bordesMatrix[row + 1, col + 1] = true;
                        }
                    }
                }
                return bordesMatrix;
            }
            else
            {
                return null;
            }

        }

        public static List<Int32[]> esCuadrado(List<Int32[]> puntos)
        {
            Int32 iMinCol = 0;
            Int32 iMinRow = 0;
            Int32 iMaxCol = 0;
            Int32 iMaxRow = 0;

            for (Int32 i = 1; i < puntos.Count; i++)
            {
                // COL MIN Y MAX
                if (puntos[iMinCol][0] > puntos[i][0])
                {
                    iMinCol = i;
                }
                else if (puntos[iMinCol][0] == puntos[i][0])
                {
                    if (puntos[iMinCol][1] > puntos[i][1])
                    {
                        iMinCol = i;
                    }
                }

                if (puntos[iMaxCol][0] < puntos[i][0])
                {
                    iMaxCol = i;
                }
                else if (puntos[iMaxCol][0] == puntos[i][0])
                {
                    if (puntos[iMaxCol][1] < puntos[i][1])
                    {
                        iMaxCol = i;
                    }
                }

                // ROW MIN Y MAX
                if (puntos[iMinRow][1] > puntos[i][1])
                {
                    iMinRow = i;
                }
                else if (puntos[iMinRow][1] == puntos[i][1])
                {
                    if (puntos[iMinRow][0] > puntos[i][0])
                    {
                        iMinRow = i;
                    }
                }

                if (puntos[iMaxCol][1] < puntos[i][1])
                {
                    iMaxRow = i;
                }
                else if (puntos[iMaxRow][1] == puntos[i][1])
                {
                    if (puntos[iMaxRow][0] < puntos[i][0])
                    {
                        iMaxCol = i;
                    }
                }
            }

            if (isSquare(puntos[iMinRow], puntos[iMinCol], puntos[iMaxRow], puntos[iMaxCol]))
            {
                return new List<Int32[]>() { puntos[iMinRow], puntos[iMinCol], puntos[iMaxRow], puntos[iMaxCol] };
            }
            else
            {
                return null;
            }
        }

        static int distSq(Int32[] p, Int32[] q)
        {
            return (p[0] - q[0]) * (p[0] - q[0]) + (p[1] - q[1]) * (p[1] - q[1]);
        }

        static bool isSquare(Int32[] p1, Int32[] p2, Int32[] p3, Int32[] p4)
        {
            int d2 = distSq(p1, p2); // from p1 to p2
            int d3 = distSq(p1, p3); // from p1 to p3
            int d4 = distSq(p1, p4); // from p1 to p4

            if (d2 == 0 || d3 == 0 || d4 == 0)
                return false;

            if (d2 == d3 && 2 * d2 == d4
                && 2 * distSq(p2, p4) == distSq(p2, p3))
            {
                return true;
            }
                        
            if (d3 == d4 && 2 * d3 == d2
                && 2 * distSq(p3, p2) == distSq(p3, p4))
            {
                return true;
            }
            if (d2 == d4 && 2 * d2 == d3
                && 2 * distSq(p2, p3) == distSq(p2, p4))
            {
                return true;
            }
            return false;
        }

        public static BigInteger raizCuadradaMasUno(BigInteger elemento, out BigInteger exacta0)
        {
            return raizCuadrada(elemento, out exacta0) + 1;
        }

        public static BigInteger raizCuadrada(BigInteger elemento, out BigInteger exacta0)
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
            }
            while (aSE.Count > 0);

            exacta0 = s2;

            return BigInteger.Parse(resultString);
        }


    }
}
