using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using static System.Net.WebRequestMethods;
using System.Threading;
using Windows.ApplicationModel.Appointments.DataProvider;
using Windows.UI.Xaml.Controls.Primitives;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace FileToVideo_VideoToFile
{
    public struct CMYK
    {
        private double _c;
        private double _m;
        private double _y;
        private double _k;

        public CMYK(double c, double m, double y, double k)
        {
            this._c = c;
            this._m = m;
            this._y = y;
            this._k = k;
        }

        public double C
        {
            get { return this._c; }
            set { this._c = value; }
        }

        public double M
        {
            get { return this._m; }
            set { this._m = value; }
        }

        public double Y
        {
            get { return this._y; }
            set { this._y = value; }
        }

        public double K
        {
            get { return this._k; }
            set { this._k = value; }
        }

        public bool Equals(CMYK cmyk)
        {
            return (this.C == cmyk.C) && (this.M == cmyk.M) && (this.Y == cmyk.Y) && (this.K == cmyk.K);
        }
    }

    public struct RGB
    {
        private byte _r;
        private byte _g;
        private byte _b;

        public RGB(byte r, byte g, byte b)
        {
            this._r = r;
            this._g = g;
            this._b = b;
        }

        public byte R
        {
            get { return this._r; }
            set { this._r = value; }
        }

        public byte G
        {
            get { return this._g; }
            set { this._g = value; }
        }

        public byte B
        {
            get { return this._b; }
            set { this._b = value; }
        }

        public bool Equals(RGB rgb)
        {
            return (this.R == rgb.R) && (this.G == rgb.G) && (this.B == rgb.B);
        }
    }

    

    public static class apzCode
    {
        public static RGB CMYKToRGB(CMYK cmyk)
        {
            byte r = (byte)(255 * (1 - cmyk.C) * (1 - cmyk.K));
            byte g = (byte)(255 * (1 - cmyk.M) * (1 - cmyk.K));
            byte b = (byte)(255 * (1 - cmyk.Y) * (1 - cmyk.K));

            return new RGB(r, g, b);
        }

        //ushort for WORD and uint for DWORD, LONG > ulong        
        private static String sHex2 = "42 4D"
                        + "00 00 00 00"
                        + "00 00"
                        + "00 00"
                        + "3E 00 00 00"

                        + "28 00 00 00"
                        + "05 00 00 00"
                        + "05 00 00 00"
                        + "01 00"
                        + "01 00"
                        + "00 00 00 00"
                        + "00 00 00 00"
                        + "00 00 00 00"
                        + "00 00 00 00"
                        + "00 00 00 00"
                        + "00 00 00 00"

                        + "FF FF FF 00"
                        + "00 00 00 00"

                        + "20 00 00 00"
                        + "20 00 00 00"
                        + "20 00 00 00"
                        + "F8 00 00 00"
                        + "20 00 00 00";

        private static String sHex0 =
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
       private static String sHex1 = "01 00" // NC 1 plane Number of color planes being used
            + "01 00" //	1 bits Number of bits per pixel
            + "00 00 00 00" // NC	0	BI_RGB, no pixel array compression used
            + "00 00 00 00" //	1 bytes Size of the raw bitmap data(including padding)
            + "00 00 00 00" //	2835 pixels/metre horizontal
                            //	Print resolution of the image, 72 DPI × 39.3701 inches per metre yields 2834.6472
            + "00 00 00 00" //	2835 pixels/metre vertical
            + "00 00 00 00" // NC	0 colors Number of colors in the palette
            + "00 00 00 00" // NC	0 important colors	0 means all colors are important
                            //Start of pixel array(bitmap data)
            + "FF FF FF 00" // color 1 MONOCROME
            + "00 00 00 00"; // color 2 MONOCROME
                            // ARRAY
            //+ "80 00 00 00" //	10010000
            //+ "40 00 00 00";//	10010000
            
            

        private static String sHexPixel =
              // BMP Header
              "42 4D" //"BM"	ID field (42h, 4Dh)
            + "46 00 00 00" //	70 bytes(54+16)    Size of the BMP file(54 bytes header + 16 bytes data)
            + "00 00" //	Unused Application specific
            + "00 00" //	Unused Application specific
            + "36 00 00 00" //	54 bytes(14+40)    Offset where the pixel array(bitmap data) can be found
            //DIB Header
            + "28 00 00 00" //	40 bytes Number of bytes in the DIB header(from this point)
            + "02 00 00 00" //	2 pixels(left to right order)  Width of the bitmap in pixels
            + "02 00 00 00" //	2 pixels(bottom to top order)  Height of the bitmap in pixels.Positive for bottom to top pixel order.
            + "01 00" //	1 plane Number of color planes being used
            + "18 00" //	24 bits Number of bits per pixel
            + "00 00 00 00" //	0	BI_RGB, no pixel array compression used
            + "10 00 00 00" //	16 bytes Size of the raw bitmap data(including padding)
            + "13 0B 00 00" //	2835 pixels/metre horizontal    Print resolution of the image,
                        //72 DPI × 39.3701 inches per metre yields 2834.6472
            + "13 0B 00 00" //	2835 pixels/metre vertical
            + "00 00 00 00" //	0 colors Number of colors in the palette
            + "00 00 00 00" //	0 important colors	0 means all colors are important
            //Start of pixel array(bitmap data)
            + "00 00 FF" //	0 0 255	Red, Pixel(0,1)
            + "FF FF FF" //	255 255 255	White, Pixel(1,1)
            + "00 00" //	0 0	Padding for 4 byte alignment(could be a value other than zero)
            + "FF 00 00" //	255 0 0	Blue, Pixel(0,0)
            + "00 FF 00" //	0 255 0	Green, Pixel(1,0)
            + "00 00"; //	0 0	Padding for 4 byte alignment(could be a value other than zero)";

        private static String sHxRedbrickbmp =     "42 4d 76 02 00 00 00 00  00 00 76 00 00 00 28 00"
                                    + "00 00 20 00 00 00 20 00  00 00 01 00 04 00 00 00"
                                    + "00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00"
                                    + "00 00 00 00 00 00 00 00  00 00 00 00 80 00 00 80"
                                    + "00 00 00 80 80 00 80 00  00 00 80 00 80 00 80 80"
                                    + "00 00 80 80 80 00 c0 c0  c0 00 00 00 ff 00 00 ff"
                                    + "00 00 00 ff ff 00 ff 00  00 00 ff 00 ff 00 ff ff"
                                    + "00 00 ff ff ff 00 00 00  00 00 00 00 00 00 00 00"
                                    + "00 00 00 00 00 00 00 00  00 00 00 00 00 00 09 00"
                                    + "00 00 00 00 00 00 11 11  01 19 11 01 10 10 09 09"
                                    + "01 09 11 11 01 90 11 01  19 09 09 91 11 10 09 11"
                                    + "09 11 19 10 90 11 19 01  19 19 10 10 11 10 09 01"
                                    + "91 10 91 09 10 10 90 99  11 11 11 11 19 00 09 01"
                                    + "91 01 01 19 00 99 11 10  11 91 99 11 09 90 09 91"
                                    + "01 11 11 11 91 10 09 19  01 00 11 90 91 10 09 01"
                                    + "11 99 10 01 11 11 91 11  11 19 10 11 99 10 09 10"
                                    + "01 11 11 11 19 10 11 09  09 10 19 10 10 10 09 01"
                                    + "11 19 00 01 10 19 10 11  11 01 99 01 11 90 09 19"
                                    + "11 91 11 91 01 11 19 10  99 00 01 19 09 10 09 19"
                                    + "10 91 11 01 11 11 91 01  91 19 11 00 99 90 09 01"
                                    + "01 99 19 01 91 10 19 91  91 09 11 99 11 10 09 91"
                                    + "11 10 11 91 99 10 90 11  01 11 11 19 11 90 09 11"
                                    + "00 19 10 11 01 11 99 99  99 99 99 99 99 99 09 99"
                                    + "99 99 99 99 99 99 00 00  00 00 00 00 00 00 00 00"
                                    + "00 00 00 00 00 00 90 00  00 00 00 00 00 00 00 00"
                                    + "00 00 00 00 00 00 99 11  11 11 19 10 19 19 11 09"
                                    + "10 90 91 90 91 00 91 19  19 09 01 10 09 01 11 11"
                                    + "91 11 11 11 10 00 91 11  01 19 10 11 10 01 01 11"
                                    + "90 11 11 11 91 00 99 09  19 10 11 90 09 90 91 01"
                                    + "19 09 91 11 01 00 90 10  19 11 00 11 11 00 10 11"
                                    + "01 10 11 19 11 00 90 19  10 91 01 90 19 99 00 11"
                                    + "91 01 11 01 91 00 99 09  09 01 10 11 91 01 10 91"
                                    + "99 11 10 90 91 00 91 11  00 10 11 01 10 19 19 09"
                                    + "10 00 99 01 01 00 91 01  19 91 19 91 11 09 10 11"
                                    + "00 91 00 10 90 00 99 01  11 10 09 10 10 19 09 01"
                                    + "91 90 11 09 11 00 90 99  11 11 11 90 19 01 19 01"
                                    + "91 01 01 19 09 00 91 10  11 91 99 09 09 90 11 91"
                                    + "01 19 11 11 91 00 91 19  01 00 11 00 91 10 11 01"
                                    + "11 11 10 01 11 00 99 99  99 99 99 99 99 99 99 99"
                                    + "99 99 99 99 99 90";
            
        private static byte[] hexStringToByteArray(string hex)
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


        public async static Task<WriteableBitmap> Generate(String text, Canvas cMain)
        {
            Int32 minPixel = 8;

            Byte[] bytes = UTF8Encoding.UTF8.GetBytes(text);
            Int32 nBits = bytes.Length * 8;
            Int32 nCuadrado = 5;
            Int32 cuadrado = nCuadrado * nCuadrado;
            while(nBits > cuadrado - (nCuadrado + 2))
            {
                nCuadrado += 2;
                cuadrado = nCuadrado * nCuadrado;
            }
            //nBits += Convert.ToInt32(sqBits);

            //TODO: Calcular with height a partir de los datos text,
            Int32 heightwidth = Convert.ToInt32(Math.Sqrt((nCuadrado + 7) * (nCuadrado + 7) * minPixel * minPixel));
            
            // Length mínimo para respetar el ajuste a Bytes, será siempre mayor o igual al height/with y múltiplo de 32
            Int32 minByteLenght = getMinByteLenght(heightwidth);
            heightwidth = minByteLenght;

            Int32 dataSpace = (heightwidth - (7 * minPixel)) / minPixel;

            // Obtenemos el hexadecimal little endian para alto y ancho (es una imagen cuadrada)
            String hwHex = Convert.ToString(heightwidth, 16);
            hwHex = hwHex.Length % 2 == 0 ? hwHex : String.Concat("0", hwHex);
            hwHex = LittleEndian(hwHex);

            // Enlazamos y obtenemos los bytes de la cabecera
            String sHex = String.Concat(sHex0, hwHex, hwHex, sHex1);            
            Byte[] bytesH = hexStringToByteArray(sHex);

            // Ejemplo 2 pixels
            //Byte[] bytesD = GetBytesFromBinaryString("1000000000000000000000000000000001000000000000000000000000000000");

            // Estos serán los datos de la imagen a construir
            // El tamaño de estos datos será del tamaño height por with
            // con un padding ajustado a bytes y con un mínimo de 4 bytes
            String cBinario = String.Empty;

            

            GenerateLineInit(ref cBinario, heightwidth, minByteLenght, minPixel);

            String binarioActual = String.Empty;
            foreach (Byte b in bytes)
            {
                binarioActual += Convert.ToString(b, 2).PadLeft(8, '0');
            }

            //String binarioCompress = encodingBitsApzyxMethod("00000001111111");

            // Primer dato

            String minPix0 = "".PadRight(minPixel, '0');
            String minPix1 = "".PadRight(minPixel, '1');

            // Creamos el cuerpo del apzCode
            Boolean isBlack = true;
            for (Int32 j = 0; j < dataSpace; j++)
            {
                //txtB.Text = "Processing " + j + " / " + dataSpace;
                //Thread.Sleep(10);

                String parteBinariA = binarioActual.Length > j * (dataSpace) ?
                    binarioActual.Length > (j + 1) * (dataSpace) ?
                    binarioActual.Substring(j * (dataSpace), (dataSpace)) :
                    binarioActual.Substring(j * (dataSpace), binarioActual.Length - (j * (dataSpace))).PadRight((dataSpace), '0') :
                    "".PadRight((dataSpace), '0')
                    ;

                String represBinarioActual = GetBinaryRepresentation(parteBinariA, minPixel);


                for (Int32 k = 0; k < minPixel; k++)
                {
                    cBinario += minPix0;
                    cBinario += minPix1;
                    cBinario += isBlack ? minPix1 : minPix0;
                    
                    // DATOS EN Sí CONCRETOS FUERA DE LA NORMA DEL MARCO
                    cBinario += represBinarioActual;

                    //PaddingRest(minPixel, ref cBinario);

                    cBinario += isBlack ? minPix1 : minPix0;
                    cBinario += minPix1;
                    cBinario += minPix0;
                    cBinario += minPix0;
                }

                //binarioActual = binarioActual == "1010101" ? "0101010" : "1010101";

                isBlack = !isBlack;
            }

            GenerateLineEnd(ref cBinario, heightwidth, minByteLenght, minPixel);

            Byte[] bytesD = GetBytesFromBinaryString(cBinario);
            

            // Los datos se representarán de abajo a arriba y de izquierda a derecha

            Byte[] resultBytes = new Byte[bytesH.Length + bytesD.Length];
            Array.Copy(bytesH, resultBytes, bytesH.Length);
            Array.Copy(bytesD, 0, resultBytes, bytesH.Length, bytesD.Length);

            MemoryStream mS = new MemoryStream(resultBytes);

            WriteableBitmap wb = new WriteableBitmap(heightwidth, heightwidth);
            //////var picker = new Windows.Storage.Pickers.FileOpenPicker();
            //////picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            //////picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            //////picker.FileTypeFilter.Add("*");
            //////Windows.Storage.StorageFile fileP = await picker.PickSingleFileAsync();
            //////await wb.SetSourceAsync((await fileP.OpenStreamForWriteAsync()).AsRandomAccessStream());

            await wb.SetSourceAsync(mS.AsRandomAccessStream());

            //return wb;

            // GUARDAR EN ARCHIVO

            //String s = await Decode(wb, cMain);

            //var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            //savePicker.FileTypeChoices.Add("Unknown", new List<string>() { "." });
            //savePicker.SuggestedStartLocation =
            //    Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            //savePicker.SuggestedFileName = "prueba";
            //Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            var file = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync("temp", CreationCollisionOption.GenerateUniqueName);

            if (file != null)
            {
                ////Windows.Storage.CachedFileManager.DeferUpdates(file);
                ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                ////Windows.Storage.Provider.FileUpdateStatus status =
                ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                ////{
                
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                    Stream pixelStream = wb.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                        (uint)wb.PixelWidth,
                                        (uint)wb.PixelHeight,
                                        96.0,
                                        96.0,
                                        pixels);
                    await encoder.FlushAsync();
                }
            ////}
            ////else
            ////{
            ////    throw new Exception("Error");
            ////}
            }
            else
            {
                throw new Exception("Error");
            }

            return wb;
        }

        

        public async static Task<WriteableBitmap> Generate01BN(String text, int width)
        {                    
            Int32 nBits = text.Length;

            //nBits += Convert.ToInt32(sqBits);

            //TODO: Calcular with height a partir de los datos text,

            Int32 height = text.Length % width == 0 ? text.Length / width : (text.Length / width) + 1;
                        
            Int32 dataSpace = width;

            // Obtenemos el hexadecimal little endian para alto y ancho (es una imagen cuadrada)
            String wHex = Convert.ToString(width, 16);
            wHex = wHex.Length % 2 == 0 ? wHex : String.Concat("0", wHex);
            wHex = LittleEndian(wHex);

            String hHex = Convert.ToString(height, 16);
            hHex = hHex.Length % 2 == 0 ? hHex : String.Concat("0", hHex);
            hHex = LittleEndian(hHex);

            // Enlazamos y obtenemos los bytes de la cabecera
            String sHex = String.Concat(sHex0, wHex, hHex, sHex1);
            Byte[] bytesH = hexStringToByteArray(sHex);

            // Ejemplo 2 pixels
            //Byte[] bytesD = GetBytesFromBinaryString("1000000000000000000000000000000001000000000000000000000000000000");

            // Estos serán los datos de la imagen a construir
            // El tamaño de estos datos será del tamaño height por with
            // con un padding ajustado a bytes y con un mínimo de 4 bytes

            String binaryRepresentation = String.Empty;

            for (Int32 i = 0; (i * width) < text.Length; i++)
            {
                String data = (i * width) + width < text.Length ? 
                    text.Substring(i * width, width) : 
                    text.Substring(i * width);

                Int32 j = 0;
                while ((j * 32) < width)
                {
                    j++;
                }
                data = data.PadRight(j * 32, '0');
                

                binaryRepresentation += data;

            }


            Byte[] bytesD = GetBytesFromBinaryString(binaryRepresentation);


            // Los datos se representarán de abajo a arriba y de izquierda a derecha

            Byte[] resultBytes = new Byte[bytesH.Length + bytesD.Length];
            Array.Copy(bytesH, resultBytes, bytesH.Length);
            Array.Copy(bytesD, 0, resultBytes, bytesH.Length, bytesD.Length);

            MemoryStream mS = new MemoryStream(resultBytes);

            WriteableBitmap wb = new WriteableBitmap(width, height);
            

            await wb.SetSourceAsync(mS.AsRandomAccessStream());

            //return wb;

            // GUARDAR EN ARCHIVO

            //String s = await Decode(wb, cMain);

            StorageFolder videosFolder = KnownFolders.VideosLibrary;
            StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
            StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
            var file = await vtfVideoImgsFolder.CreateFileAsync(width.ToString() + "x" + height.ToString() + ".png", CreationCollisionOption.GenerateUniqueName);

            if (file != null)
            {
                ////Windows.Storage.CachedFileManager.DeferUpdates(file);
                ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                ////Windows.Storage.Provider.FileUpdateStatus status =
                ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                ////{

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                    Stream pixelStream = wb.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                        (uint)wb.PixelWidth,
                                        (uint)wb.PixelHeight,
                                        96.0,
                                        96.0,
                                        pixels);
                    await encoder.FlushAsync();
                }
                ////}
                ////else
                ////{
                ////    throw new Exception("Error");
                ////}
            }
            else
            {
                throw new Exception("Error");
            }

            return wb;
        }

        public static async Task< WriteableBitmap> WriteableBitmapFromBitString24BN(string bitsString, int width)
        {
            int height = bitsString.Length / (width * 24);
            WriteableBitmap bitmap = new WriteableBitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int byteIndex = (y * width + x) * 24;

                    byte gray = BitStringToByte(bitsString.Substring(byteIndex, 24));

                    byte[] pixelData = new byte[4];
                    pixelData[0] = gray; // componente azul
                    pixelData[1] = gray; // componente verde
                    pixelData[2] = gray; // componente rojo
                    pixelData[3] = 255; // componente alpha

                    bitmap.PixelBuffer.AsStream().Seek((y * width + x) * 4, SeekOrigin.Begin);
                    bitmap.PixelBuffer.AsStream().Write(pixelData, 0, 4);
                }
            }

            StorageFolder videosFolder = KnownFolders.VideosLibrary;
            StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
            StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
            var file = await vtfVideoImgsFolder.CreateFileAsync(width.ToString() + "x" + height.ToString() + ".png", CreationCollisionOption.GenerateUniqueName);

            if (file != null)
            {
                ////Windows.Storage.CachedFileManager.DeferUpdates(file);
                ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                ////Windows.Storage.Provider.FileUpdateStatus status =
                ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                ////{

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                    Stream pixelStream = bitmap.PixelBuffer.AsStream();
                    byte[] pixels2 = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels2, 0, pixels2.Length);

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                        (uint)bitmap.PixelWidth,
                                        (uint)bitmap.PixelHeight,
                                        96.0,
                                        96.0,
                                        pixels2);
                    await encoder.FlushAsync();
                }
                ////}
                ////else
                ////{
                ////    throw new Exception("Error");
                ////}
            }
            else
            {
                throw new Exception("Error");
            }

            return bitmap;
        }

        public static async Task<WriteableBitmap> GenerateBitmapFromBitStringBN(string bitsString, int width)
        {
            int height = bitsString.Length / (width * 8);

            WriteableBitmap bitmap = new WriteableBitmap(width, height);

            using (Stream stream = bitmap.PixelBuffer.AsStream())
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int byteIndex = (y * width + x) * 8;

                        byte grayValue = BitStringToByteBN(bitsString.Substring(byteIndex, 8));

                        stream.WriteByte(grayValue);
                        stream.WriteByte(grayValue);
                        stream.WriteByte(grayValue);
                        stream.WriteByte(255);
                    }
                }
            }

            StorageFolder videosFolder = KnownFolders.VideosLibrary;
            StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
            StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
            var file = await vtfVideoImgsFolder.CreateFileAsync(width.ToString() + "x" + height.ToString() + ".png", CreationCollisionOption.GenerateUniqueName);

            if (file != null)
            {
                ////Windows.Storage.CachedFileManager.DeferUpdates(file);
                ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                ////Windows.Storage.Provider.FileUpdateStatus status =
                ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                ////{

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                    Stream pixelStream = bitmap.PixelBuffer.AsStream();
                    byte[] pixels2 = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels2, 0, pixels2.Length);

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                        (uint)bitmap.PixelWidth,
                                        (uint)bitmap.PixelHeight,
                                        96.0,
                                        96.0,
                                        pixels2);
                    await encoder.FlushAsync();
                }
                ////}
                ////else
                ////{
                ////    throw new Exception("Error");
                ////}
            }
            else
            {
                throw new Exception("Error");
            }

            return bitmap;
        }

        public static async Task<WriteableBitmap> GenerateBitmapFromBitString(string bitsString, int width)
        {
            int height = bitsString.Length / (width * 24);
            var bitmap = new WriteableBitmap(width, height);

            using (var stream = bitmap.PixelBuffer.AsStream())
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int byteIndex = (y * width + x) * 3;
                        byte red = BitStringToByte(bitsString.Substring(byteIndex, 8));
                        byte green = BitStringToByte(bitsString.Substring(byteIndex + 8, 8));
                        byte blue = BitStringToByte(bitsString.Substring(byteIndex + 16, 8));

                        stream.WriteByte(blue);
                        stream.WriteByte(green);
                        stream.WriteByte(red);
                        stream.WriteByte(255);
                    }
                }
            }

            StorageFolder videosFolder = KnownFolders.VideosLibrary;
            StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
            StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
            var file = await vtfVideoImgsFolder.CreateFileAsync(width.ToString() + "x" + height.ToString() + ".png", CreationCollisionOption.GenerateUniqueName);

            if (file != null)
            {
                ////Windows.Storage.CachedFileManager.DeferUpdates(file);
                ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                ////Windows.Storage.Provider.FileUpdateStatus status =
                ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                ////{

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                    Stream pixelStream = bitmap.PixelBuffer.AsStream();
                    byte[] pixels2 = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels2, 0, pixels2.Length);

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                        (uint)bitmap.PixelWidth,
                                        (uint)bitmap.PixelHeight,
                                        96.0,
                                        96.0,
                                        pixels2);
                    await encoder.FlushAsync();
                }
                ////}
                ////else
                ////{
                ////    throw new Exception("Error");
                ////}
            }
            else
            {
                throw new Exception("Error");
            }

            return bitmap;
        }


        public static async Task<WriteableBitmap> GenerateBitmapFromBitString(string bitsString, int width, int height)
        {
            // Crear un nuevo objeto WriteableBitmap con el ancho y alto especificados
            WriteableBitmap bitmap = new WriteableBitmap(width, height);

            // Obtener el número total de bytes necesarios para representar la imagen
            int bytesPerPixel = 3;
            int pixelCount = width * height;
            int byteCount = pixelCount * bytesPerPixel;

            // Crear un array de bytes para almacenar los datos de la imagen
            byte[] pixelData = new byte[byteCount];

            // Rellenar el array de bytes con los datos de la imagen
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * width + x) * bytesPerPixel;

                    // Obtener los valores R, G, B a partir de la cadena de bits
                    byte r = BitStringToByte(bitsString.Substring(pixelIndex * 8, 8));
                    byte g = BitStringToByte(bitsString.Substring((pixelIndex + 1) * 8, 8));
                    byte b = BitStringToByte(bitsString.Substring((pixelIndex + 2) * 8, 8));

                    // Establecer los valores R, G, B en el array de bytes
                    pixelData[pixelIndex] = r;
                    pixelData[pixelIndex + 1] = g;
                    pixelData[pixelIndex + 2] = b;
                }
            }

            // Copiar los datos de la imagen al objeto WriteableBitmap
            using (Stream pixelStream = bitmap.PixelBuffer.AsStream())
            {
                pixelStream.Write(pixelData, 0, byteCount);
            }

            StorageFolder videosFolder = KnownFolders.VideosLibrary;
            StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
            StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
            var file = await vtfVideoImgsFolder.CreateFileAsync(width.ToString() + "x" + height.ToString() + ".png", CreationCollisionOption.GenerateUniqueName);

            if (file != null)
            {
                ////Windows.Storage.CachedFileManager.DeferUpdates(file);
                ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                ////Windows.Storage.Provider.FileUpdateStatus status =
                ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                ////{

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                    Stream pixelStream = bitmap.PixelBuffer.AsStream();
                    byte[] pixels2 = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels2, 0, pixels2.Length);

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                        (uint)bitmap.PixelWidth,
                                        (uint)bitmap.PixelHeight,
                                        96.0,
                                        96.0,
                                        pixels2);
                    await encoder.FlushAsync();
                }
                ////}
                ////else
                ////{
                ////    throw new Exception("Error");
                ////}
            }
            else
            {
                throw new Exception("Error");
            }

            return bitmap;
        }

        public static byte BitStringToByte(string bitsString)
        {            
            int porcentual = 256 / (int)Math.Ceiling(Math.Pow(2, bitsString.Length));

            Int32 cInt = Convert.ToInt32(bitsString, 2);

            Byte color = (Byte)(cInt * porcentual);

            return color;
        }

        private static byte BitStringToByteBN(string bitsString)
        {
            if (bitsString.Length != 8)
            {
                throw new ArgumentException("El string de bits debe tener una longitud de 8 caracteres.");
            }

            int byteValue = 0;

            for (int i = 0; i < 8; i++)
            {
                if (bitsString[i] == '1')
                {
                    byteValue += (int)Math.Pow(2, 7 - i);
                }
            }

            return (byte)byteValue;
        }

        public static async Task<Boolean> GenerateImageFromBitString24BN(string bitsString, int width)
        {
            int height = bitsString.Length / 8 / width;
            if (height > 0)
            {
                WriteableBitmap bitmap = new WriteableBitmap(width, height);
                using (Stream stream = bitmap.PixelBuffer.AsStream())
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int byteIndex = (y * width + x) * 8;
                            CMYK cmyk = new CMYK();

                            cmyk.C = BitStringToByte(bitsString.Substring(byteIndex, 2));
                            cmyk.M = BitStringToByte(bitsString.Substring(byteIndex + 2, 2));
                            cmyk.Y = BitStringToByte(bitsString.Substring(byteIndex + 4, 2));
                            cmyk.K = BitStringToByte(bitsString.Substring(byteIndex + 6, 2));
                            
                            RGB rgb = CMYKToRGB(cmyk);
                            stream.WriteByte(rgb.R);
                            stream.WriteByte(rgb.G);
                            stream.WriteByte(rgb.B);
                            stream.WriteByte(255);
                        }
                    }
                }
                StorageFolder videosFolder = KnownFolders.VideosLibrary;
                StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
                StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
                var file = await vtfVideoImgsFolder.CreateFileAsync(width.ToString() + "x" + height.ToString() + ".png", CreationCollisionOption.GenerateUniqueName);

                if (file != null)
                {
                    ////Windows.Storage.CachedFileManager.DeferUpdates(file);
                    ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                    ////Windows.Storage.Provider.FileUpdateStatus status =
                    ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    ////{

                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                        Stream pixelStream = bitmap.PixelBuffer.AsStream();
                        byte[] pixels2 = new byte[pixelStream.Length];
                        await pixelStream.ReadAsync(pixels2, 0, pixels2.Length);

                        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                            (uint)bitmap.PixelWidth,
                                            (uint)bitmap.PixelHeight,
                                            96.0,
                                            96.0,
                                            pixels2);
                        await encoder.FlushAsync();
                    }
                    ////}
                    ////else
                    ////{
                    ////    throw new Exception("Error");
                    ////}
                }
                else
                {
                    throw new Exception("Error");
                }

                return true;
            }
            return false;
        }

        public static async Task<WriteableBitmap> GenerateImageFromBitString8BN(string bitsString, int width)
        {
            int height = bitsString.Length / 24 / width;
            WriteableBitmap bitmap = new WriteableBitmap(width, height);
            using (Stream stream = bitmap.PixelBuffer.AsStream())
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int byteIndex = (y * width + x) * 3 * 8;
                        //byte red = BitStringToByteBN(bitsString.Substring(byteIndex, 8));
                        //byte green = BitStringToByteBN(bitsString.Substring(byteIndex + 8, 8));
                        byte blue = BitStringToByteBN(bitsString.Substring(byteIndex + 16, 8));
                        //byte gray = BitStringToByteBN(bitsString.Substring(byteIndex, 24));
                        //stream.WriteByte(gray);
                        //stream.WriteByte(gray);
                        //stream.WriteByte(gray);
                        stream.WriteByte(blue);
                        stream.WriteByte(blue);
                        stream.WriteByte(blue);
                        stream.WriteByte(255);
                    }
                }
            }
            StorageFolder videosFolder = KnownFolders.VideosLibrary;
            StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
            StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
            var file = await vtfVideoImgsFolder.CreateFileAsync(width.ToString() + "x" + height.ToString() + ".png", CreationCollisionOption.GenerateUniqueName);

            if (file != null)
            {
                ////Windows.Storage.CachedFileManager.DeferUpdates(file);
                ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                ////Windows.Storage.Provider.FileUpdateStatus status =
                ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                ////{

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                    Stream pixelStream = bitmap.PixelBuffer.AsStream();
                    byte[] pixels2 = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels2, 0, pixels2.Length);

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                        (uint)bitmap.PixelWidth,
                                        (uint)bitmap.PixelHeight,
                                        96.0,
                                        96.0,
                                        pixels2);
                    await encoder.FlushAsync();
                }
                ////}
                ////else
                ////{
                ////    throw new Exception("Error");
                ////}
            }
            else
            {
                throw new Exception("Error");
            }

            return bitmap;
        }

        //public static async void GenerateImageA(string bitsString, int width, int height)
        //{
        //    int bitsPerColor = 8; // 8 bits por color
        //    int stride = (width * bitsPerColor + 7) / 8; // número de bytes por fila
        //    byte[] pixels = new byte[stride * height * 3]; // arreglo de bytes para los pixeles

        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            // Calculamos la posición en el arreglo de bytes
        //            int position = (y * stride) + (x * bitsPerColor / 8);

        //            // Obtenemos los valores RGB a partir de los bits
        //            byte r = Convert.ToByte(bitsString[(y * width + x) * 3], 2);
        //            byte g = Convert.ToByte(bitsString[(y * width + x) * 3 + 1], 2);
        //            byte b = Convert.ToInt32(bitsString[(y * width + x) * 3 + 2], 2);

        //            // Asignamos los valores RGB a los pixeles
        //            pixels[position] = b;
        //            pixels[position + 1] = g;
        //            pixels[position + 2] = r;
        //        }
        //    }

        //    // Creamos la imagen
        //    WriteableBitmap bitmap = new WriteableBitmap(width, height);
        //    using (Stream stream = bitmap.PixelBuffer.AsStream())
        //    {
        //        await stream.WriteAsync(pixels, 0, pixels.Length);
        //    }

        //    StorageFolder videosFolder = KnownFolders.VideosLibrary;
        //    StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
        //    StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
        //    var file = await vtfVideoImgsFolder.CreateFileAsync(width.ToString() + "x" + height.ToString() + ".png", CreationCollisionOption.GenerateUniqueName);

        //    if (file != null)
        //    {
        //        ////Windows.Storage.CachedFileManager.DeferUpdates(file);
        //        ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
        //        ////Windows.Storage.Provider.FileUpdateStatus status =
        //        ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
        //        ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
        //        ////{

        //        using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
        //        {
        //            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
        //            Stream pixelStream = bitmap.PixelBuffer.AsStream();
        //            byte[] pixels2 = new byte[pixelStream.Length];
        //            await pixelStream.ReadAsync(pixels2, 0, pixels2.Length);

        //            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
        //                                (uint)bitmap.PixelWidth,
        //                                (uint)bitmap.PixelHeight,
        //                                96.0,
        //                                96.0,
        //                                pixels2);
        //            await encoder.FlushAsync();
        //        }
        //        ////}
        //        ////else
        //        ////{
        //        ////    throw new Exception("Error");
        //        ////}
        //    }
        //    else
        //    {
        //        throw new Exception("Error");
        //    }
        //}

        private static void GenerateLineInit(ref String cBinario, Int32 hw, Int32 minByteLenght, Int32 minPixel)
        {
            String minPix0 = "".PadRight(minPixel, '0');
            String minPix1 = "".PadRight(minPixel, '1');

            // Creamos la línea blanca inferior del apzCode que tendrá min pixeles de alto
            for (Int32 j = 0; j < minPixel; j++)
            {
                for (Int32 i = 0; i < minByteLenght / minPixel; i++)
                {
                    cBinario += minPix0;
                }
            }

            // Creamos la línea negra inferior del apzCode que tendrá 5 pixeles de alto
            for (Int32 j = 0; j < minPixel; j++)
            {
                cBinario += minPix0;
                

                for (Int32 i = 0; i < (minByteLenght - (3 * minPixel)) / minPixel; i++)
                {
                    cBinario += minPix1;
                }

                cBinario += minPix0;
                cBinario += minPix0;
            }

            

            // Creamos la línea negra/blanca inferior de señal del apzCode que tendrá 5 pixeles de alto
            // y alternará negro blanco negro...
            for (Int32 j = 0; j < minPixel; j++)
            {
                cBinario += minPix0;
                cBinario += minPix1;

                Boolean isBlack = false;
                for (Int32 i = 0; i < (minByteLenght - (5 * minPixel)) / minPixel; i++)
                {
                    cBinario += isBlack ? minPix1 : minPix0;
                    isBlack = !isBlack;
                    
                }

                cBinario += minPix1;
                cBinario += minPix0;
                cBinario += minPix0;
            }
        }

        private static void GenerateLineEnd(ref String cBinario, Int32 hw, Int32 minByteLenght, Int32 minPixel)
        {
            //////String minPix0 = "".PadRight(minPixel, '0');
            //////String minPix1 = "".PadRight(minPixel, '1'); ;

            //////// Creamos la línea negra/blanca superior de señal del apzCode que tendrá 5 pixeles de alto
            //////// y alternará negro blanco negro...
            //////for (Int32 j = 0; j < minPixel; j++)
            //////{
            //////    for (Int32 i = 0; i < minPixel; i++)
            //////    {
            //////        cBinario += "0";
            //////    }

            //////    for (Int32 i = 0; i < minPixel; i++)
            //////    {
            //////        cBinario += "1";
            //////    }

            //////    Boolean isBlack = false;
            //////    for (Int32 i = 0; i < minByteLenght - (5 * minPixel); i++)
            //////    {
            //////        cBinario += isBlack ? "1" : "0";

            //////        if ((i + 1) % minPixel == 0)
            //////        {
            //////            isBlack = !isBlack;
            //////        }
            //////    }

            //////    for (Int32 i = 0; i < minPixel; i++)
            //////    {
            //////        cBinario += "1";
            //////    }

            //////    for (Int32 i = 0; i < minPixel; i++)
            //////    {
            //////        cBinario += "0";
            //////    }
            //////    for (Int32 i = 0; i < minPixel; i++)
            //////    {
            //////        cBinario += "0";
            //////    }
            //////}

            //////// Creamos la línea negra superior del apzCode que tendrá 5 pixeles de alto
            //////for (Int32 j = 0; j < minPixel; j++)
            //////{
            //////    for (Int32 i = 0; i < minPixel; i++)
            //////    {
            //////        cBinario += "0";
            //////    }

            //////    for (Int32 i = 0; i < (minByteLenght - (3 * minPixel)); i++)
            //////    {
            //////        cBinario += "1";
            //////    }

            //////    for (Int32 i = 0; i < minPixel; i++)
            //////    {
            //////        cBinario += "0";
            //////    }
            //////    for (Int32 i = 0; i < minPixel; i++)
            //////    {
            //////        cBinario += "0";
            //////    }
            //////}

            //////// Creamos la línea blanca superior del apzCode que tendrá min pixeles de alto
            //////for (Int32 j = 0; j < minPixel; j++)
            //////{
            //////    for (Int32 i = 0; i < minByteLenght; i++)
            //////    {
            //////        cBinario += "0";
            //////    }
            //////}
            //////// Creamos la línea blanca superior del apzCode que tendrá min pixeles de alto
            //////for (Int32 j = 0; j < minPixel; j++)
            //////{
            //////    for (Int32 i = 0; i < minByteLenght; i++)
            //////    {
            //////        cBinario += "0";
            //////    }
            //////}
            ///

            String minPix0 = "".PadRight(minPixel, '0');
            String minPix1 = "".PadRight(minPixel, '1');

            // Creamos la línea negra/blanca superior de señal del apzCode que tendrá 5 pixeles de alto
            // y alternará negro blanco negro...
            for (Int32 j = 0; j < minPixel; j++)
            {
                cBinario += minPix0;
                cBinario += minPix1;
                

                Boolean isBlack = false;
                for (Int32 i = 0; i < (minByteLenght - (5 * minPixel)) / minPixel; i++)
                {
                    cBinario += isBlack ? minPix1 : minPix0;
                    isBlack = !isBlack;                    
                }

                cBinario += minPix1;
                cBinario += minPix0;
                cBinario += minPix0;
            }

            // Creamos la línea negra superior del apzCode que tendrá 5 pixeles de alto
            for (Int32 j = 0; j < minPixel; j++)
            {
                cBinario += minPix0;

                for (Int32 i = 0; i < (minByteLenght - (3 * minPixel)) / minPixel; i++)
                {
                    cBinario += minPix1;
                }

                cBinario += minPix0;
                cBinario += minPix0;
            }

            // Creamos la línea blanca superior del apzCode que tendrá min pixeles de alto
            for (Int32 j = 0; j < minPixel; j++)
            {
                for (Int32 i = 0; i < minByteLenght / minPixel; i++)
                {
                    cBinario += minPix0;
                }
            }
            // Creamos la línea blanca superior doble del apzCode que tendrá min pixeles de alto
            for (Int32 j = 0; j < minPixel; j++)
            {
                for (Int32 i = 0; i < minByteLenght / minPixel; i++)
                {
                    cBinario += minPix0;
                }
            }
        }

        private static String GetBinaryRepresentation(String sBinary, Int32 minPixel)
        {
            String result = String.Empty;

            foreach (Char c in sBinary)
            {
                for (Int32 i = 0; i < minPixel; i++)
                {
                    result += c;
                }
            }

            return result;
        }

        public async static Task<String> Decode(WriteableBitmap image)
        {
            
            //Color borderColor = Colors.White;

            //Byte[] bytes = await SDK.GetBNImage(image);

            //String sHex = String.Empty;
            //foreach (Byte b in bytes)
            //{
            //    sHex += Convert.ToString(b, 16).PadLeft(2, '0');
            //}

            Int32 w = image.PixelWidth;
            Int32 h = image.PixelHeight;
            Boolean salir = false;
            Int32 diferenciaC = 5;
            
            Byte[] originalB = await SDK.WBToBytes(image);
            Byte[] bytes = await SDK.GetBNImage(image);

            Int32 row = 0;
            Int32 col = 0;

            // Estando la imagen en blanco y negro r, g y b son iguales
            // Obtnemos la componente green del color (ó red ó blue en su defecto)
            List<Boolean[]> colorsMatrix = new List<Boolean[]>();
            Boolean[] colorRow = new Boolean[w];
            for (Int32 j = 54; j < bytes.Length; j += 4)
            {
                //var b = bytes[j];
                var g = Convert.ToInt32(bytes[j + 1]);
                //var r = bytes[j + 2];
                //var a = bytes[j + 3];

                Int32 c = g;
                colorRow[col] = c > 127 ? false : true;

                col++;

                if (col != 0 && col % w == 0)
                {
                    Boolean[] tempCRow = new Boolean[w];
                    colorRow.CopyTo(tempCRow, 0);
                    colorsMatrix.Add(tempCRow);
                    colorRow = new Boolean[w];
                    row++;
                    col = 0;
                }
            }

            //////Boolean[,] bordesMatrix = SDK.GetBorderMatrix(h, w, colorsMatrix, diferenciaC);
            List<List<Boolean>> decapiteMatrix = new List<List<Boolean>>();
            for (Int32 iRowDecapite = 0; iRowDecapite < colorsMatrix[0].Length; iRowDecapite++)
            {
                List<Boolean> lBools = colorsMatrix[iRowDecapite].ToList();
                if (lBools.Contains(false) && lBools.Contains(true))
                {
                    decapiteMatrix.Add(lBools);
                }
            }

            if (decapiteMatrix.Count == 0)
            {
                return null;
            }

            // Eliminar columnas

            List<Int32> indexsDel = new List<Int32>();

            Int32 lastiColDecapite = 0;            
            for (Int32 iColDecapite = lastiColDecapite; iColDecapite < colorsMatrix[0].Count(); iColDecapite++)
            {
                Boolean delete = true;
                Boolean antBool = colorsMatrix[0][iColDecapite];
                for (Int32 iRowDecapite = 1; iRowDecapite < colorsMatrix.Count; iRowDecapite++)
                {
                    Boolean b = colorsMatrix[iRowDecapite][iColDecapite];
                    if (antBool != b)
                    {
                        delete = false;
                        break;
                    }
                }

                if (delete)
                {
                    indexsDel.Add(iColDecapite);                                        
                }
            }

            foreach (Int32 indexDel in indexsDel.OrderByDescending(x => x))
            {
                for (Int32 iRowDel = 0; iRowDel < decapiteMatrix.Count; iRowDel++)
                {
                    decapiteMatrix[iRowDel].RemoveAt(indexDel);
                }
            }                        

            Int32 maxRow = decapiteMatrix.Count;
            Int32 maxCol = decapiteMatrix[0].Count;

            Boolean[,] dataMatrix = new Boolean[maxRow, maxCol]; ;

            for(Int32 iRow  = 0; iRow < maxRow; iRow++)
            {                
                for (Int32 iCol = 0; iCol < maxCol; iCol++)
                {
                    Boolean dataBool = decapiteMatrix[iRow][iCol];

                    dataMatrix[iRow, iCol] = dataBool;
                }
            }


            // Recorremos las posiciones de entrada a posibles códigos de abajo izquierda a arriba derecha, por ello si ante el 33 no hemos encontrado nada, nada habrá            
            for (Int32 iRow = 0; iRow < maxRow - 33; iRow++)
            {
                for (Int32 iCol = 0; iCol < maxCol - 33; iCol++)
                {                    
                    Int32 iRowTemp = iRow;
                    Int32 iColTemp = iCol;

                    Boolean lastBool = dataMatrix[iRow, iCol];

                    Boolean TheEnd = false;

                    Int32 initApzCodeRow = -1;
                    Int32 initApzCodeCol = -1;

                    Int32 endApzCodeRow = -1;
                    Int32 endApzCodeCol = -1;

                    while (!TheEnd)
                    {
                        iColTemp++;

                        if (iColTemp >= maxCol)
                        {
                            iColTemp = 0;
                            iRowTemp++;
                            if (iRowTemp >= maxRow)
                            {
                                TheEnd = true;
                                break;
                            }
                        }

                        Boolean actualBool = dataMatrix[iRowTemp, iColTemp];
                        if (lastBool != actualBool)
                        {
                            initApzCodeRow = iRowTemp + 1;
                            initApzCodeCol = iColTemp + 1;
                            Int32 iRowSearchEnd = iRowTemp;
                            Int32 iColSearchEnd = iColTemp;

                            var tasks = new List<Task>();
                                                            
                            tasks.Add(Task.Run(() =>
                            {
                                iColSearchEnd++;
                                if (iColSearchEnd < maxCol - 1)
                                {
                                    Boolean b = dataMatrix[initApzCodeRow, iColSearchEnd];
                                    iColSearchEnd++;
                                    while (b == actualBool && iColSearchEnd < maxCol)
                                    {
                                        b = dataMatrix[initApzCodeRow, iColSearchEnd];
                                        iColSearchEnd++;
                                    }
                                }
                            }));

                            tasks.Add(Task.Run(() =>
                            {
                                iRowSearchEnd++;
                                if (iRowSearchEnd < maxRow - 1)
                                {                                
                                    Boolean b = dataMatrix[iRowSearchEnd, initApzCodeCol];
                                    iRowSearchEnd++;
                                    while (b == actualBool && iRowSearchEnd < maxRow)
                                    {
                                        b = dataMatrix[iRowSearchEnd, initApzCodeCol];
                                        iRowSearchEnd++;                                    
                                    }
                                }
                            }));


                            //wait until all the tasks in the list are completed
                            await Task.WhenAll(tasks);

                            // Tenemos un posible candidatos
                            if (iColSearchEnd != maxCol && iRowSearchEnd != maxRow && ((iColSearchEnd - initApzCodeCol) - 3) > 33 &&
                                ((iColSearchEnd - initApzCodeCol) + 3) > (iRowSearchEnd - initApzCodeRow) && 
                                ((iColSearchEnd - initApzCodeCol) - 3) < (iRowSearchEnd - initApzCodeRow))
                            {

                                Int32 iRowSearchEnd2 = iRowTemp;
                                Int32 iColSearchEnd2 = iColTemp;

                                tasks = new List<Task>();

                                tasks.Add(Task.Run(() =>
                                {
                                    iColSearchEnd2++;
                                    if (iColSearchEnd2 < maxCol - 1)
                                    {
                                        Boolean b = dataMatrix[iRowSearchEnd - 2, iColSearchEnd2];
                                        iColSearchEnd2++;
                                        while (b == actualBool && iColSearchEnd2 < maxCol)
                                        {
                                            b = dataMatrix[iRowSearchEnd - 2, iColSearchEnd2];
                                            iColSearchEnd2++;

                                        }
                                    }
                                }));

                                tasks.Add(Task.Run(() =>
                                {
                                    iRowSearchEnd2++;
                                    if (iRowSearchEnd2 < maxRow - 1)
                                    {
                                        Boolean b = dataMatrix[iRowSearchEnd2, iColSearchEnd - 2];
                                        iRowSearchEnd2++;
                                        while (b == actualBool && iRowSearchEnd2 < maxRow)
                                        {
                                            b = dataMatrix[iRowSearchEnd2, iColSearchEnd - 2];
                                            iRowSearchEnd2++;

                                        }
                                    }
                                }));

                                await Task.WhenAll(tasks);

                                if (iColSearchEnd2 != maxCol && iRowSearchEnd2 != maxRow && ((iColSearchEnd2 - initApzCodeCol) - 3) > 33 &&
                                    ((iColSearchEnd2 - initApzCodeCol) + 3) > (iRowSearchEnd2 - initApzCodeRow) &&
                                ((iColSearchEnd2 - initApzCodeCol) - 3) < (iRowSearchEnd2 - initApzCodeRow))
                                {
                                    endApzCodeRow = iRowSearchEnd2 - 3;
                                    endApzCodeCol = iColSearchEnd2 - 3;

                                    List<Int32> coordenadasROWInferior = new List<Int32>();
                                    List<Int32> coordenadasROWSuperior = new List<Int32>();
                                    List<Int32> coordenadasCOLIzquierda = new List<Int32>();
                                    List<Int32> coordenadasCOLDerecha = new List<Int32>();


                                    tasks = new List<Task>();

                                    // coordenadasROWInferior
                                    tasks.Add(Task.Run(() =>
                                    {                                        
                                        for (Int32 coordRow = initApzCodeRow; coordRow < endApzCodeRow; coordRow++)
                                        {
                                            Boolean encontradoPatron = false;
                                            Boolean b = true;
                                            Int32 pos = initApzCodeCol;
                                            for (Int32 coordCol = initApzCodeCol; coordCol < endApzCodeCol; coordCol++)
                                            {
                                                Boolean coordBool = dataMatrix[coordRow, coordCol];
                                                if (b != coordBool)
                                                {
                                                    encontradoPatron = true;
                                                    coordenadasROWInferior.Add((pos + coordCol) / 2);
                                                    b = coordBool;
                                                    pos = coordCol;
                                                }
                                            }

                                            if (coordenadasROWInferior.Count >= 5)
                                            {
                                                Int32 coorValidate = coordenadasROWInferior[1];
                                                Int32 coorRest = coordenadasROWInferior[1] - coordenadasROWInferior[0];
                                                for (Int32 iValidate = 2; iValidate < coordenadasROWInferior.Count; iValidate++)
                                                {
                                                    Int32 actCoord = coordenadasROWInferior[iValidate];
                                                    Int32 actualRest = actCoord - coorValidate;
                                                    if (coorRest + 3 > actualRest && coorRest - 3 < actualRest)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        encontradoPatron = false;
                                                        coordenadasROWInferior = new List<Int32>();
                                                        break;
                                                    }
                                                    coorValidate = actCoord;
                                                }
                                            }

                                            if(encontradoPatron)
                                            {
                                                break;
                                            }
                                        }
                                    }));

                                    //coordenadasROWSuperior
                                    tasks.Add(Task.Run(() =>
                                    {
                                        for (Int32 coordRow = endApzCodeRow; coordRow > initApzCodeRow; coordRow--)
                                        {
                                            Boolean encontradoPatron = false;
                                            Boolean b = true;
                                            Int32 pos = initApzCodeCol;
                                            for (Int32 coordCol = initApzCodeCol; coordCol < endApzCodeCol; coordCol++)
                                            {
                                                Boolean coordBool = dataMatrix[coordRow, coordCol];
                                                if (b != coordBool)
                                                {
                                                    encontradoPatron = true;
                                                    coordenadasROWSuperior.Add((pos + coordCol) / 2);
                                                    b = coordBool;
                                                    pos = coordCol;
                                                }
                                            }

                                            if (coordenadasROWSuperior.Count >= 5)
                                            {
                                                Int32 coorValidate = coordenadasROWSuperior[1];
                                                Int32 coorRest = coordenadasROWSuperior[1] - coordenadasROWSuperior[0];
                                                for (Int32 iValidate = 2; iValidate < coordenadasROWSuperior.Count; iValidate++)
                                                {
                                                    Int32 actCoord = coordenadasROWSuperior[iValidate];
                                                    Int32 actualRest = actCoord - coorValidate;
                                                    if (coorRest + 3 > actualRest && coorRest - 3 < actualRest)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        encontradoPatron = false;
                                                        coordenadasROWSuperior = new List<Int32>();
                                                        break;
                                                    }
                                                    coorValidate = actCoord;
                                                }
                                            }

                                            if (encontradoPatron)
                                            {
                                                break;
                                            }
                                        }
                                    }));

                                    tasks.Add(Task.Run(() =>
                                    {
                                        for (Int32 coordCol = initApzCodeCol; coordCol < endApzCodeCol; coordCol++)                                            
                                        {
                                            Boolean encontradoPatron = false;
                                            Boolean b = true;
                                            Int32 pos = initApzCodeRow;
                                            for (Int32 coordRow = initApzCodeRow; coordRow < endApzCodeRow; coordRow++)
                                            {
                                                Boolean coordBool = dataMatrix[coordRow, coordCol];
                                                if (b != coordBool)
                                                {
                                                    encontradoPatron = true;
                                                    coordenadasCOLIzquierda.Add((pos + coordRow) / 2);
                                                    b = coordBool;
                                                    pos = coordRow;
                                                }
                                            }

                                            if (coordenadasCOLIzquierda.Count >= 5)
                                            {
                                                Int32 coorValidate = coordenadasCOLIzquierda[1];
                                                Int32 coorRest = coordenadasCOLIzquierda[1] - coordenadasCOLIzquierda[0];
                                                for (Int32 iValidate = 2; iValidate < coordenadasCOLIzquierda.Count; iValidate++)
                                                {
                                                    Int32 actCoord = coordenadasCOLIzquierda[iValidate];
                                                    Int32 actualRest = actCoord - coorValidate;
                                                    if (coorRest + 3 > actualRest && coorRest - 3 < actualRest)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        encontradoPatron = false;
                                                        coordenadasCOLIzquierda = new List<Int32>();
                                                        break;
                                                    }
                                                    coorValidate = actCoord;
                                                }
                                            }

                                            if (encontradoPatron)
                                            {
                                                break;
                                            }
                                        }
                                    }));

                                    tasks.Add(Task.Run(() =>
                                    {
                                        for (Int32 coordCol = endApzCodeCol; coordCol > initApzCodeCol; coordCol--)
                                        {
                                            Boolean encontradoPatron = false;
                                            Boolean b = true;
                                            Int32 pos = initApzCodeRow;
                                            for (Int32 coordRow = initApzCodeRow; coordRow < endApzCodeRow; coordRow++)
                                            {
                                                Boolean coordBool = dataMatrix[coordRow, coordCol];
                                                if (b != coordBool)
                                                {
                                                    encontradoPatron = true;
                                                    coordenadasCOLDerecha.Add((pos + coordRow) / 2);
                                                    b = coordBool;
                                                    pos = coordRow;
                                                }
                                            }

                                            if (coordenadasCOLDerecha.Count >= 5)
                                            {
                                                Int32 coorValidate = coordenadasCOLDerecha[1];
                                                Int32 coorRest = coordenadasCOLDerecha[1] - coordenadasCOLDerecha[0];
                                                for (Int32 iValidate = 2; iValidate < coordenadasCOLDerecha.Count; iValidate++)
                                                {
                                                    Int32 actCoord = coordenadasCOLDerecha[iValidate];
                                                    Int32 actualRest = actCoord - coorValidate;
                                                    if (coorRest + 3 > actualRest && coorRest - 3 < actualRest)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        encontradoPatron = false;
                                                        coordenadasCOLDerecha = new List<Int32>();
                                                        break;
                                                    }
                                                    coorValidate = actCoord;
                                                }
                                            }

                                            if (encontradoPatron)
                                            {
                                                break;
                                            }
                                        }
                                    }));

                                    await Task.WhenAll(tasks);

                                    String s = "stop";

                                    if (coordenadasROWInferior.Count == coordenadasROWSuperior.Count &&
                                        coordenadasROWInferior.Count == coordenadasCOLIzquierda.Count &&
                                        coordenadasROWInferior.Count == coordenadasCOLDerecha.Count)
                                    {
                                        String binaryData = String.Empty;
                                        for (Int32 iCoorCol = 2; iCoorCol < coordenadasCOLIzquierda.Count - 1; iCoorCol++)
                                        {
                                            for (Int32 iCoorRow = 2; iCoorRow < coordenadasROWInferior.Count - 1; iCoorRow++)
                                            {                                           
                                                Int32 rowCoor = coordenadasCOLIzquierda[iCoorCol];
                                                Int32 colCoor = coordenadasROWInferior[iCoorRow];

                                                Int32 trueData = 0;
                                                Int32 falseData = 0;

                                                List<Boolean> dataList = new List<Boolean>();
                                                Boolean d = dataMatrix[rowCoor, colCoor];
                                                dataList.Add(d);
                                                Boolean d2 = dataMatrix[rowCoor + 1, colCoor];
                                                dataList.Add(d2);
                                                Boolean d3 = dataMatrix[rowCoor, colCoor + 1];
                                                dataList.Add(d3);
                                                Boolean d4 = dataMatrix[rowCoor - 1, colCoor];
                                                dataList.Add(d4);
                                                Boolean d5 = dataMatrix[rowCoor, colCoor - 1];
                                                dataList.Add(d5);
                                                Boolean d6 = dataMatrix[rowCoor + 1, colCoor + 1];
                                                dataList.Add(d6);
                                                Boolean d7 = dataMatrix[rowCoor - 1, colCoor - 1];
                                                dataList.Add(d7);
                                                Boolean d8 = dataMatrix[rowCoor + 1, colCoor - 1];
                                                dataList.Add(d8);
                                                Boolean d9 = dataMatrix[rowCoor - 1, colCoor + 1];
                                                dataList.Add(d9);

                                                foreach (Boolean b in dataList)
                                                {
                                                    if (b)
                                                    {
                                                        trueData++;
                                                    }
                                                    else
                                                    {
                                                        falseData++;
                                                    }
                                                }

                                                binaryData += trueData > falseData ? "1" : "0";

                                            }
                                        }

                                        //String reverseSData = String.Empty;
                                        //foreach(Char c in binaryData.Reverse())
                                        //{
                                        //    reverseSData += c.ToString();
                                        //}

                                        int numOfBytes = binaryData.Length / 8;
                                        byte[] bytesData = new byte[numOfBytes];
                                        for (int i = 0; i < numOfBytes; ++i)
                                        {
                                            Byte b = Convert.ToByte(binaryData.Substring(8 * i, 8), 2);
                                            bytesData[i] = b;
                                        }

                                        String data = UTF8Encoding.UTF8.GetString(bytesData);
                                        return data;                                                                                
                                    }

                                }

                            }
                            else
                            {
                                break;
                            }

                        }

                    }
                }
            }



            //////List<String> datos01 = new List<String>();
            //////for (row = 0; row < h; row++)
            //////{
            //////    String s = String.Empty;
            //////    for (col = 0; col < w; col++)
            //////    {
            //////        s += bordesMatrix[row, col] ? "1" : "0";
            //////    }
            //////    datos01.Add(s);
            //////}

            //////if (bordesMatrix != null)
            //////{
            //////    row = 0;
            //////    col = 0;

            //////    //Boolean salir = false;
            //////    Boolean[,] coordenadasMatrix = new Boolean[h, w];
            //////    List<Polygon> polygons = new List<Polygon>();
            //////    Canvas c = new Canvas();
            //////    c.Width = w;
            //////    c.Height = h;
            //////    GeneralTransform gT = c.TransformToVisual(c);

            //////    // Buscamos un borde que podrá ser o no ser del cuadrado
            //////    for (; row < h; row++)
            //////    {
            //////        await Task.Delay(1);

            //////        for (; col < w; col++)
            //////        {
            //////            if (bordesMatrix[row, col])
            //////            {
            //////                List<Int32[]> coordenadasContorno = new List<Int32[]>();
            //////                coordenadasContorno.Add(new int[] { col, row });
            //////                coordenadasMatrix[row, col] = true;
            //////                Int32 rowEdge = row;
            //////                Int32 colEdge = col;
            //////                Boolean posibleCode = true;
            //////                Boolean edgeFinded = false;
            //////                Int32 direccion = 2;
            //////                while (posibleCode && !edgeFinded)
            //////                {
            //////                    if (salir)
            //////                    {
            //////                        break;
            //////                    }

            //////                    // Tendremos primeramente en cuenta unas coordenadas según la dirección a la que vayamos
            //////                    switch (direccion)
            //////                    {
            //////                        // Si vamos hacia izquierda 
            //////                        case 0:
            //////                            // abajo
            //////                            if ((rowEdge + 1) < h && bordesMatrix[rowEdge + 1, colEdge])
            //////                            {
            //////                                direccion = 1;
            //////                                rowEdge++;
            //////                            }
            //////                            // a diagonal abajo izquierda
            //////                            else if ((rowEdge + 1) < h && colEdge > 0 && bordesMatrix[rowEdge + 1, colEdge - 1])
            //////                            {
            //////                                rowEdge++;
            //////                                colEdge--;
            //////                            }
            //////                            // Izquierda
            //////                            else if (colEdge > 0 && bordesMatrix[rowEdge, colEdge - 1])
            //////                            {
            //////                                direccion = 0;
            //////                                colEdge--;
            //////                            }
            //////                            // a diagonal arriba izquierda
            //////                            else if (rowEdge > 0 && colEdge > 0 && bordesMatrix[rowEdge - 1, colEdge - 1])
            //////                            {
            //////                                rowEdge--;
            //////                                colEdge--;
            //////                            }
            //////                            // arriba
            //////                            else if (rowEdge > 0 && bordesMatrix[rowEdge - 1, colEdge])
            //////                            {
            //////                                direccion = 3;
            //////                                rowEdge--;
            //////                            }
            //////                            else
            //////                            {
            //////                                posibleCode = false;
            //////                            }
            //////                            break;

            //////                        // Si vamos hacia abajo
            //////                        case 1:
            //////                            // derecha
            //////                            if ((colEdge + 1) < w && bordesMatrix[rowEdge, colEdge + 1])
            //////                            {
            //////                                direccion = 2;
            //////                                colEdge++;
            //////                            }
            //////                            // a diagonal abajo derecha
            //////                            else if ((rowEdge + 1) < h && (colEdge + 1) < w && bordesMatrix[rowEdge + 1, colEdge + 1])
            //////                            {
            //////                                rowEdge++;
            //////                                colEdge++;
            //////                            }
            //////                            // abajo
            //////                            else if ((rowEdge + 1) < h && bordesMatrix[rowEdge + 1, colEdge])
            //////                            {
            //////                                direccion = 1;
            //////                                rowEdge++;
            //////                            }
            //////                            // a diagonal abajo izquierda
            //////                            else if ((rowEdge + 1) < h && colEdge > 0 && bordesMatrix[rowEdge + 1, colEdge - 1])
            //////                            {
            //////                                rowEdge++;
            //////                                colEdge--;
            //////                            }
            //////                            // Izquierda
            //////                            else if (colEdge > 0 && bordesMatrix[rowEdge, colEdge - 1])
            //////                            {
            //////                                direccion = 0;
            //////                                colEdge--;
            //////                            }
            //////                            else
            //////                            {
            //////                                posibleCode = false;
            //////                            }
            //////                            break;

            //////                        // Si vamos hacia la derecha
            //////                        case 2:
            //////                            // arriba
            //////                            if (rowEdge > 0 && bordesMatrix[rowEdge - 1, colEdge])
            //////                            {
            //////                                direccion = 3;
            //////                                rowEdge--;
            //////                            }
            //////                            // a diagonal arriba derecha
            //////                            else if (rowEdge > 0 && (colEdge + 1) < w && bordesMatrix[rowEdge - 1, colEdge + 1])
            //////                            {
            //////                                rowEdge--;
            //////                                colEdge++;
            //////                            }
            //////                            // derecha
            //////                            else if ((colEdge + 1) < w && bordesMatrix[rowEdge, colEdge + 1])
            //////                            {
            //////                                direccion = 2;
            //////                                colEdge++;
            //////                            }
            //////                            // a diagonal abajo derecha
            //////                            else if ((rowEdge + 1) < h && (colEdge + 1) < w && bordesMatrix[rowEdge + 1, colEdge + 1])
            //////                            {
            //////                                rowEdge++;
            //////                                colEdge++;
            //////                            }
            //////                            // abajo
            //////                            else if ((rowEdge + 1) < h && bordesMatrix[rowEdge + 1, colEdge])
            //////                            {
            //////                                direccion = 1;
            //////                                rowEdge++;
            //////                            }
            //////                            else
            //////                            {
            //////                                posibleCode = false;
            //////                            }
            //////                            break;

            //////                        // Vamos hacia arriba
            //////                        case 3:
            //////                            // Izquierda
            //////                            if (colEdge > 0 && bordesMatrix[rowEdge, colEdge - 1])
            //////                            {
            //////                                direccion = 0;
            //////                                colEdge--;
            //////                            }
            //////                            // a diagonal arriba izquierda
            //////                            else if (rowEdge > 0 && colEdge > 0 && bordesMatrix[rowEdge - 1, colEdge - 1])
            //////                            {
            //////                                rowEdge--;
            //////                                colEdge--;
            //////                            }
            //////                            // arriba
            //////                            else if (rowEdge > 0 && bordesMatrix[rowEdge - 1, colEdge])
            //////                            {
            //////                                direccion = 3;
            //////                                rowEdge--;
            //////                            }
            //////                            // a diagonal arriba derecha
            //////                            else if (rowEdge > 0 && (colEdge + 1) < w && bordesMatrix[rowEdge - 1, colEdge + 1])
            //////                            {
            //////                                rowEdge--;
            //////                                colEdge++;
            //////                            }
            //////                            // derecha
            //////                            else if ((colEdge + 1) < w && bordesMatrix[rowEdge, colEdge + 1])
            //////                            {
            //////                                direccion = 2;
            //////                                colEdge++;
            //////                            }
            //////                            else
            //////                            {
            //////                                posibleCode = false;
            //////                            }

            //////                            break;
            //////                    }

            //////                    Boolean contieneCoordenada = false;
            //////                    for (int i = 0; i < coordenadasContorno.Count; i++)
            //////                    {
            //////                        if (coordenadasContorno[i][0] == colEdge && coordenadasContorno[i][1] == rowEdge)// SequenceEqual(new Int32[] { rowEdge, colEdge }))
            //////                        {
            //////                            contieneCoordenada = true;
            //////                        }
            //////                    }

            //////                    if (contieneCoordenada)
            //////                    {
            //////                        edgeFinded = true;
            //////                        // TODO: Probar a buscar azpCode

            //////                        List<Int32[]> cuadrado = SDK.esCuadrado(coordenadasContorno);

            //////                        if (cuadrado != null)
            //////                        {
            //////                            // Tenemos un cuadrado
            //////                            // Ahora analizaremos el apzCode


            //////                        }

            //////                    }
            //////                    else
            //////                    {
            //////                        coordenadasContorno.Add(new Int32[] { colEdge, rowEdge });

            //////                    }
            //////                }
            //////            }
            //////        }

            //////        if (salir)
            //////        {
            //////            break;
            //////        }
            //////        col = 0;

            //////    }

            ////if (!salir)
            ////{
            ////    // Componemos las coordenadasMatrix con los poligonos que tenemos como finalizados
            ////    foreach (Polygon polygon in polygons)
            ////    {
            ////        foreach (Windows.Foundation.Point pPoly in polygon.Points)
            ////        {
            ////            Int32 y = Convert.ToInt32(pPoly.Y);
            ////            Int32 x = Convert.ToInt32(pPoly.X);

            ////            coordenadasMatrix[y, x] = true;
            ////            // arriba
            ////            if (y > 0)
            ////            {
            ////                coordenadasMatrix[y - 1, x] = true;
            ////            }
            ////            // abajo
            ////            if ((y + 1) < h)
            ////            {
            ////                coordenadasMatrix[y + 1, x] = true;
            ////            }
            ////            // izquierda
            ////            if (x > 0)
            ////            {
            ////                coordenadasMatrix[y, x - 1] = true;
            ////            }
            ////            // derecha
            ////            if ((x + 1) < w)
            ////            {
            ////                coordenadasMatrix[y, x + 1] = true;
            ////            }
            ////            // diagonal arriba izquierda
            ////            if (y > 0 && x > 0)
            ////            {
            ////                coordenadasMatrix[y - 1, x - 1] = true;
            ////            }
            ////            // diagonal abajo derecha
            ////            if ((y + 1) < h && (x + 1) < w)
            ////            {
            ////                coordenadasMatrix[y + 1, x + 1] = true;
            ////            }
            ////            // diagonal arriba derecha
            ////            if (y > 0 && (x + 1) < w)
            ////            {
            ////                coordenadasMatrix[y - 1, x + 1] = true;
            ////            }
            ////            // diagonal abajo izquierda
            ////            if ((y + 1) < h && x > 0)
            ////            {
            ////                coordenadasMatrix[y + 1, x - 1] = true;
            ////            }
            ////        }
            ////    }

            ////    row = 0;
            ////    col = 0;
            ////    // Componemos el nuevo archivo
            ////    for (Int32 j = 54; j < originalB.Length; j += 4)
            ////    {
            ////        if (coordenadasMatrix[row, col])
            ////        {
            ////            originalB[j] = borderColor.B;
            ////            originalB[j + 1] = borderColor.G;
            ////            originalB[j + 2] = borderColor.R;
            ////            originalB[j + 3] = borderColor.A;
            ////        }
            ////        else
            ////        {
            ////            Byte tempB = originalB[j];
            ////            originalB[j] = originalB[j + 2];
            ////            //originalB[j + 1] = Convert.ToByte(0);
            ////            originalB[j + 2] = tempB;
            ////        }

            ////        col++;

            ////        if (col != 0 && col % w == 0)
            ////        {
            ////            row++;
            ////            col = 0;
            ////        }
            ////    }

            ////    //// Convertimos los nuevos bytes a un nuevo WriteableBitmap
            ////    //using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            ////    //{
            ////    //    stream.AsStreamForWrite().Write(bytes, 0, bytes.Length);
            ////    //    stream.Seek(0);
            ////    //    WriteableBitmap wb = new WriteableBitmap(_wB.PixelWidth, _wB.PixelHeight);
            ////    //    wb.SetSource(stream);

            ////    //    _wB = wb;
            ////    //    imgMain.Source = _wB;
            ////    //}

            ////    MemoryStream mS = new MemoryStream(originalB);
            ////    WriteableBitmap wb = new WriteableBitmap(w, h);
            ////    await wb.SetSourceAsync(mS.AsRandomAccessStream());
            ////    parent.Tag = wb;
            ////}
            ////else
            ////{
            ////    // Create the message dialog and set its content
            ////    var messageDialog = new MessageDialog("No es posible procesar contornos en esta imagen");
            ////    messageDialog.Commands.Add(new UICommand("Cerrar"));
            ////    messageDialog.CancelCommandIndex = 0;
            ////    // Show the message dialog
            ////    await messageDialog.ShowAsync();
            ////}
            //////}

            //cMain.UpdateLayout();

            return null;
        }

        private static Boolean isValidColor(Color c)
        {
            // Tiene componente clara
            if (c.R > 192 && c.G > 192 && c.B > 192)
            {
                return true;
            }
            // Tiene componente oscura
            else if (c.R < 64 && c.G < 64 && c.B < 64)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static Boolean isForWhite(Color c)
        {
            if (c.R > 192 && c.G > 192 && c.B > 192)
            {
                // Tiene componente clara
                return true;
            }            
            else if (c.R < 64 && c.G < 64 && c.B < 64)
            {
                // Tiene componente oscura
                return false;
            }
            else
            {
                throw new Exception("Color Nó Válido");
            }
        }

        //private unsafe void EditPixels(SoftwareBitmap bitmap)
        //{
        //    // Effect is hard-coded to operate on BGRA8 format only
        //    if (bitmap.BitmapPixelFormat == BitmapPixelFormat.Bgra8)
        //    {
        //        // In BGRA8 format, each pixel is defined by 4 bytes
        //        const int BYTES_PER_PIXEL = 4;

        //        using (var buffer = bitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
        //        using (var reference = buffer.CreateReference())
        //        {
        //            // Get a pointer to the pixel buffer
        //            byte* data;
        //            uint capacity;
        //            ((IMemoryBufferByteAccess)reference).GetBuffer(out data, out capacity);

        //            // Get information about the BitmapBuffer
        //            var desc = buffer.GetPlaneDescription(0);

        //            // Iterate over all pixels
        //            for (uint row = 0; row < desc.Height; row++)
        //            {
        //                for (uint col = 0; col < desc.Width; col++)
        //                {
        //                    // Index of the current pixel in the buffer (defined by the next 4 bytes, BGRA8)
        //                    var currPixel = desc.StartIndex + desc.Stride * row + BYTES_PER_PIXEL * col;

        //                    // Read the current pixel information into b,g,r channels (leave out alpha channel)
        //                    var b = data[currPixel + 0]; // Blue
        //                    var g = data[currPixel + 1]; // Green
        //                    var r = data[currPixel + 2]; // Red

        //                    // Boost the green channel, leave the other two untouched
        //                    data[currPixel + 0] = b;
        //                    data[currPixel + 1] = (byte)Math.Min(g + 80, 255);
        //                    data[currPixel + 2] = r;
        //                }
        //            }
        //        }
        //    }
        //}

        private static async Task<Byte[]> WBToBytes(WriteableBitmap writableBitmap)
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

        private static String LittleEndian(String num)
        {
            Int32 number = Convert.ToInt32(num, 16);
            Byte[] bytes = BitConverter.GetBytes(number);
            String retval = "";
            foreach (Byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }

        private static Int32 getMinByteLenght(Int32 hw)
        {
            Int32 k = -1;
            for (Int32 i = 8 * 4; i < hw + (8 * 4); i += (8 * 4))
            {
                k = i;
            }
            return k;
        }
                
        public static async void GuardarSoftwareBitmapEnArchivo(WriteableBitmap writableBitmap)
        {
            SoftwareBitmap sftBM = SoftwareBitmap.CreateCopyFromBuffer(
                    writableBitmap.PixelBuffer,
                    BitmapPixelFormat.Rgba8,
                    writableBitmap.PixelWidth,
                    writableBitmap.PixelHeight);

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.FileTypeChoices.Add("Unknown", new List<string>() { "." });
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            savePicker.SuggestedFileName = "prueba.bmp";
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                encoder.SetSoftwareBitmap(sftBM);
                await encoder.FlushAsync();
            }
        }

        private static String encodingBitsApzyxMethod(String binaryString)
        {
            String sResult = String.Empty;

            Boolean isZero = true;
            Int32 i = 0;                        
            while (i < binaryString.Length)
            {                
                String s = binaryString[i].ToString();

                if(s == "0" && isZero || s == "1" && !isZero)
                {
                    Boolean enterSR = false;
                    for(Int32 j = 1; j < 8; j++) // to 16
                    {
                        if(i + 1 >= binaryString.Length)
                        {
                            i++;
                            enterSR = true;
                            sResult += FromBase10(j.ToString(), 2).PadLeft(3, '0'); // to4
                            isZero = !isZero;
                            break;
                        }

                        String sJ = binaryString[i + 1].ToString();
                        if (s == sJ)
                        {
                            i++;                            
                        }
                        else
                        {
                            i++;
                            enterSR = true;
                            sResult += FromBase10(j.ToString(), 2).PadLeft(3, '0'); //to 4
                            isZero = !isZero;
                            break;
                        }
                    }

                    if (!enterSR)
                    {
                        sResult += "111"; //to "1111"
                    }
                }
                else
                {
                    isZero = !isZero;
                    sResult += "000"; // to "0000"
                }
            }

            return sResult;
        }

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

    }
}
