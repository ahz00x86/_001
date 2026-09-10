using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace apz_Image_Editor
{
    public static class SDK
    {
        public static String sHex1_0 =
              // BMP Header
              "42 4D" // NC "BM" ID field (42h, 4Dh)
            + "00 00 00 00" // > bytes(54+x)    Size of the BMP file(54 bytes header + 1 bytes data) 42
            + "00 00" // NC	Unused Application specific
            + "00 00" // NC	Unused Application specific
            + "3E 00 00 00" // NC	54 bytes(14+40+8)    Offset where the pixel array(bitmap data) can be found
                            //DIB Header
            + "28 00 00 00"; // NC	40 bytes Number of bytes in the DIB header(from this point)
                             //+ "91 00 00 00" // >	pixels(left to right order)  Width of the bitmap in pixels
                             //+ "91 00 00 00" // >	pixels(bottom to top order)  Height of the bitmap in pixels.Positive for bottom to top pixel order.

        //"424d3a4200000000000036000000280000004100000041000000010020000000000000000000c40e0000c40e00000000000000000000ffffffffffff"
        public static String sHex1_1 = "01 00" // NC 1 plane Number of color planes being used
             + "01 00" //	1 bits Number of bits per pixel
             + "00 00 00 00" // NC	0	BI_RGB, no pixel array compression used
             + "00 00 00 00" //	1 bytes Size of the raw bitmap data(including padding)
             + "00 00 00 00" //	2835 pixels/metre horizontal
                             //	Print resolution of the image, 72 DPI × 39.3701 inches per metre yields 2834.6472
             + "00 00 00 00" //	2835 pixels/metre vertical
             + "00 00 00 00" // NC	0 colors Number of colors in the palette
             + "00 00 00 00" // NC	0 important colors	0 means all colors are important
                             //Palette
             + "FF FF FF 00" // color 1 MONOCROME
             + "00 00 00 00"; // color 2 MONOCROME


        public static String sHex32_0 =
              // BMP Header
              "42 4D" // NC "BM" ID field (42h, 4Dh)
            + "00 00 00 00" // > bytes(54+x)    Size of the BMP file(54 bytes header + 1 bytes data) 42
            + "00 00" // NC	Unused Application specific
            + "00 00" // NC	Unused Application specific
            + "36 00 00 00" // NC	54 bytes(14+40+8)    Offset where the pixel array(bitmap data) can be found
                            //DIB Header
            + "28 00 00 00"; // NC	40 bytes Number of bytes in the DIB header(from this point)
                             //+ "91 00 00 00" // >	pixels(left to right order)  Width of the bitmap in pixels
                             //+ "91 00 00 00" // >	pixels(bottom to top order)  Height of the bitmap in pixels.Positive for bottom to top pixel order.

        //"424d3a4200000000000036000000280000004100000041000000010020000000000000000000c40e0000c40e00000000000000000000ffffffffffff"
        public static String sHex32_1 = "01 00" // NC 1 plane Number of color planes being used
             + "20 00" //	1 bits Number of bits per pixel
             + "00 00 00 00" // NC	0	BI_RGB, no pixel array compression used
             + "00 00 00 00" //	1 bytes Size of the raw bitmap data(including padding)
             + "00 00 00 00" //	2835 pixels/metre horizontal
                             //	Print resolution of the image, 72 DPI × 39.3701 inches per metre yields 2834.6472
             + "00 00 00 00" //	2835 pixels/metre vertical
             + "00 00 00 00" // NC	0 colors Number of colors in the palette
             + "00 00 00 00"; // NC	0 important colors	0 means all colors are important
                             

        public static async Task<Byte[]> WBToBytes(WriteableBitmap writableBitmap)
        {
            using (SoftwareBitmap sftBM = SoftwareBitmap.CreateCopyFromBuffer(
                    writableBitmap.PixelBuffer,
                    BitmapPixelFormat.Rgba8,
                    writableBitmap.PixelWidth,
                    writableBitmap.PixelHeight, BitmapAlphaMode.Straight))
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

        public static byte[] HexStringToByteArray(string hex)
        {
            String resultS = String.Empty;
            foreach (Char s in hex)
            {
                if (s != ' ')
                {
                    resultS += s;
                }
            }

            return Enumerable.Range(0, resultS.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(resultS.Substring(x, 2), 16))
                             .ToArray();
        }

        public static Byte[] GetBytesFromBinaryString(String input)
        {
            Int32 numOfBytes = input.Length / 8;
            Byte[] bytes = new Byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
            }

            return bytes;
        }

        public static String LittleEndian(String num)
        {
            Int32 number = Convert.ToInt32(num, 16);
            Byte[] bytes = BitConverter.GetBytes(number);
            String retval = "";
            foreach (Byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }

        public static Boolean IsForBlack(Int32 c)
        {
            if (c < 128)
            {
                // Tiene componente clara
                return true;
            }
            else
            {
                // Tiene componente oscura
                return false;
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
                //var a = bytes[j + 3];

                // Obtenemos la media para Blanco y Negro
                var averageValue = ((int)r + (int)b + (int)g) / 3;

                bytes[j] = Convert.ToByte(averageValue);
                bytes[j + 1] = Convert.ToByte(averageValue);
                bytes[j + 2] = Convert.ToByte(averageValue);
            }

            return bytes;
        }

        public static async Task<Byte[,]> GetBNImageMatrix(WriteableBitmap wb)
        {
            Byte[] bytes = await SDK.WBToBytes(wb);

            Byte[,] matrixBN = new byte[wb.PixelHeight, wb.PixelWidth];

            Int32 f = 0;
            Int32 c = -1;

            for (Int32 j = 54; j < bytes.Length; j += 4)
            {
                var b = bytes[j];
                var g = bytes[j + 1];
                var r = bytes[j + 2];
                //var a = bytes[j + 3];

                // Obtenemos la media para Blanco y Negro
                var averageValue = ((int)r + (int)b + (int)g) / 3;

                if (c >= wb.PixelWidth - 1)
                {
                    f++;
                    c = -1;
                }

                c++;
                matrixBN[f, c] = averageValue > 127 ? (Byte)0 : (Byte)255;



            }

            return matrixBN;
        }

        public static Int32[,] GetEncuadreMatrix(Int32 h, Int32 w, Byte[,] colorsMatrix, Int32 diferenciaC)
        {
            // Tenemos los datos coincidentes
            if (h == colorsMatrix.GetLength(0))
            {
                Int32 maxw = 0;

                Int32 i = -1;
                Int32 j = -1;

                Int32[,] bordesMatrix = new Int32[h, w];
                for (Int32 row = 0; row < h; row++)
                {
                    i++;
                    j = -1;
                    Int32 prevC = -1;
                    Boolean rowLeftOK = false;
                    for (Int32 col = 0; col < w; col++)
                    {
                        Int32 actualC = colorsMatrix[row,col];

                        if (prevC == -1)
                        {
                            prevC = actualC;
                            continue;
                        }

                        if (!rowLeftOK && SDK.CambioEnGradiente(actualC, prevC, diferenciaC))
                        {
                            rowLeftOK = true;
                        }

                        if (rowLeftOK)
                        {
                            j++;
                            bordesMatrix[i, j] = actualC;                            
                        }
                    }

                    //prevC = -1;
                    //rowLeftOK = false;
                    //for (Int32 col = w - 1; col >= 0; col--)
                    //{
                    //    Int32 actualC = colorsMatrix[row,col];

                    //    if (prevC == -1)
                    //    {
                    //        prevC = actualC;
                    //        continue;
                    //    }

                    //    if (!rowLeftOK && SDK.CambioEnGradiente(actualC, prevC, diferenciaC))
                    //    {
                    //        maxw = col > maxw ? col : maxw;
                    //        rowLeftOK = true;
                    //    }

                    //    if (rowLeftOK)
                    //    {
                    //        break;
                    //    }
                    //}
                }

                //Int32[,] minR = new Int32[i, maxw];
                //for (Int32 row = 0; row < i; row++)
                //{
                //    for (Int32 col = 0; col < maxw; col++)
                //    {
                //        minR[row, col] = bordesMatrix[row, col];
                //    }
                //}
                return bordesMatrix;
            }
            else
            {
                return null;
            }

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

        public static Boolean IsPointInPolygon(Int32[] p, List<Polygon> polygons, Int32 h, Int32 w)
        {            
            if (polygons.Count == 0)
            {
                return false;
            }

            //Boolean estaDentro = false;
            
            foreach (Polygon polygon in polygons)
            {
                Canvas c = new Canvas();
                c.Width = w;
                c.Height = h;
                c.Children.Add(polygon);
                GeneralTransform gT = polygon.TransformToVisual(c);
                Point canvasPoint = gT.TransformPoint(new Point(p[1], p[0]));
                IEnumerable<UIElement> elementos = VisualTreeHelper.FindElementsInHostCoordinates(canvasPoint, c, false);
                if (elementos.Count() > 0)
                {
                    c.Children.Remove(polygon);
                    return true;
                }
                c.Children.Remove(polygon);
            }
            
            //}

            return false;

            //////Int32 pX = p[1];
            //////Int32 pY = p[0];
            //////foreach (List<Int32[]> polygon in polygons)
            //////{
            //////    Int32 minX = polygon[0][1];
            //////    Int32 maxX = polygon[0][1];
            //////    Int32 minY = polygon[0][0];
            //////    Int32 maxY = polygon[0][0];
            //////    for (int i = 1; i < polygon.Count; i++)
            //////    {
            //////        Int32[] q = polygon[i];
            //////        minX = Math.Min(q[1], minX);
            //////        maxX = Math.Max(q[1], maxX);
            //////        minY = Math.Min(q[0], minY);
            //////        maxY = Math.Max(q[0], maxY);
            //////    }

            //////    // Si está fuera del rectángulo que forma el polígono salimos
            //////    if (pX < minX || pX > maxX || pY < minY || pY > maxY)
            //////    {
            //////        estaDentro = false;
            //////    }

            //////    if (estaDentro)
            //////    {
            //////        Int32 countVeces = 0;
            //////        for (Int32 x = pX; x < w; x++)
            //////        {                        
                        
            //////            //Boolean esSeguido = false;
            //////            //Boolean isFirst = true;
            //////            foreach (Int32[] polyPoint in polygon.Where(pP => pP[0] == pY).OrderBy(pP => pP[1]))
            //////            {                            
            //////                if (polyPoint[1] == x && polyPoint[0] == pY)
            //////                {
            //////                    countVeces++;
            //////                    //if (!esSeguido)
            //////                    //{
            //////                    //    countVeces++;
            //////                    //    esSeguido = true;
            //////                    //}
            //////                }
            //////                //else
            //////                //{
            //////                //    esSeguido = false;
            //////                //}
            //////            }

            //////            //if (countVeces > 1 && esSeguido)
            //////            //{
            //////            //    estaDentro = false;
            //////            //}
            //////            //else 
            //////            //{
                            
            //////            //}
            //////        }
            //////        if (countVeces % 2 == 0)
            //////        {
            //////            estaDentro = false;
            //////        }
            //////        else
            //////        {
            //////            estaDentro = true;
            //////        }
            //////    }

                //for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
                //{
                //    if ((polygon[i][0] > p[0]) != (polygon[j][0] > p[0]) &&
                //         p[1] < (polygon[j][1] - polygon[i][1]) * (p[0] - polygon[i][0]) / (polygon[j][0] - polygon[i][0]) + polygon[i][1])
                //    {
                //        inside = !inside;
                //    }
                //}                
            //////}

            //////return estaDentro;
        }

        public static Color? ComprobarListadoDeColoresPorToleranciaLIMPIAR(List<Color> colores, Color color, Int32 tolerancia)
        {
            Int32 r = color.R;
            Int32 g = color.G;
            Int32 b = color.B;

            List<Color> coloresSuperanTolerancia = new List<Color>();
            foreach (Color cAPrueba in colores)
            {
                Int32 rAP = cAPrueba.R;
                Int32 gAP = cAPrueba.G;
                Int32 bAP = cAPrueba.B;

                Int32 rR = r - rAP;
                Int32 gR = g - gAP;
                Int32 bR = b - bAP;

                if (tolerancia < rR || -tolerancia > rR || tolerancia < gR || -tolerancia > gR || tolerancia < bR || -tolerancia > bR)
                {
                    coloresSuperanTolerancia.Add(cAPrueba);
                }
            }

            if (coloresSuperanTolerancia.Count > (colores.Count / 2))
            {
                Int32 rNew = 0;
                Int32 gNew = 0;
                Int32 bNew = 0;
                Int32 aNew = 0;

                foreach (Color cST in coloresSuperanTolerancia)
                {
                    rNew += cST.R;
                    gNew += cST.G;
                    bNew += cST.B;
                    aNew += cST.A;
                }

                rNew /= coloresSuperanTolerancia.Count;
                gNew /= coloresSuperanTolerancia.Count;
                bNew /= coloresSuperanTolerancia.Count;
                aNew /= coloresSuperanTolerancia.Count;

                return Color.FromArgb(Convert.ToByte(aNew), Convert.ToByte(rNew), Convert.ToByte(gNew), Convert.ToByte(bNew));
            }
            else
            {
                return null;
            }

        }
    }
}
