using iText.IO.Image;
using iText.Kernel.Pdf.Xobject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Utils;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Messaging;
using Org.BouncyCastle.Utilities.Encoders;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using iText.Kernel.Geom;
using System.ComponentModel;
using System.Diagnostics;
using static iText.Svg.SvgConstants;
using iText.Kernel.XMP.Impl;
using iText.Layout.Element;
using iText.Commons.Bouncycastle.Math;
using Microsoft.Extensions.Primitives;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Globalization;

namespace priConsola
{
    class AVIWriter
    {
        private FileStream fileStream;
        private BinaryWriter binaryWriter;
        private int width;
        private int height;
        private int frameRate;
        private int frameCount;

        public AVIWriter(string filePath, int width, int height, int frameRate)
        {
            this.fileStream = new FileStream(filePath, FileMode.Create);
            this.binaryWriter = new BinaryWriter(fileStream);
            this.width = width;
            this.height = height;
            this.frameRate = frameRate;
            this.frameCount = 0;
        }

        private static void WriteAVIHeader(BinaryWriter writer, int width, int height, double frameRate, int frameCount)
        {
            // Write RIFF header
            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(0); // Placeholder for file size
            writer.Write(Encoding.ASCII.GetBytes("AVI "));

            // Write LIST header for hdrl chunk
            writer.Write(Encoding.ASCII.GetBytes("LIST"));
            writer.Write(0); // Placeholder for chunk size
            writer.Write(Encoding.ASCII.GetBytes("hdrl"));

            // Write AVIH chunk
            writer.Write(Encoding.ASCII.GetBytes("avih"));
            writer.Write(0x00000038); // Chunk size
            writer.Write((int)(1000000 / frameRate)); // Microseconds per frame
            writer.Write(frameCount); // Total frames
            writer.Write(0); // Reserved
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write((int)(Math.Ceiling(width / 4.0) * 4)); // Width (padded to multiple of 4)
            writer.Write(height); // Height
            writer.Write(0); // Reserved
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0); // Reserved
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            // Write LIST header for strl chunk
            writer.Write(Encoding.ASCII.GetBytes("LIST"));
            writer.Write(0); // Placeholder for chunk size
            writer.Write(Encoding.ASCII.GetBytes("strl"));

            // Write STRH chunk
            writer.Write(Encoding.ASCII.GetBytes("strh"));
            writer.Write(0x00000038); // Chunk size
            writer.Write(Encoding.ASCII.GetBytes("vids")); // Stream type (vids for video)
            writer.Write(Encoding.ASCII.GetBytes("DIB ")); // Codec
            writer.Write(0); // Flags
            writer.Write(0); // Priority
            writer.Write(0); // Language
            writer.Write(0); // Initial frame
            writer.Write(frameCount); // Stream length
            writer.Write(0); // Suggested buffer size
            writer.Write(0); // Quality
            writer.Write(0); // Sample size
            writer.Write((short)0); // Left
            writer.Write((short)0); // Top
            writer.Write((short)width); // Right
            writer.Write((short)height); // Bottom

            // Write STRF chunk
            writer.Write(Encoding.ASCII.GetBytes("strf"));
            writer.Write(0x00000028); // Chunk size
            writer.Write(width); // Width
            writer.Write(height); // Height
            writer.Write((short)1); // Planes
            writer.Write((short)24); // Bit count
            writer.Write(Encoding.ASCII.GetBytes("DIB ")); // Compression
            writer.Write(width * height * 3); // Image size
            writer.Write(0); // X pixels per meter
            writer.Write(0); // Y pixels per meter
            writer.Write(0); // Colors used
            writer.Write(0); // Important colors
        }

        public void WriteFrame(byte[] frameData)
        {
            // Escribir los datos de video de un solo fotograma en el archivo AVI
            // Los datos de fotograma deben estar en el formato adecuado para el códec de video utilizado.
            // En este ejemplo, asumimos que los datos de fotograma se pasan como un array de bytes.

            // Escribir el tamaño del bloque de datos
            binaryWriter.Write((int)frameData.Length);

            // Escribir la etiqueta '00db' para indicar que se trata de un bloque de datos de video
            binaryWriter.Write('0');
            binaryWriter.Write('0');
            binaryWriter.Write('d');
            binaryWriter.Write('b');

            // Escribir los datos de fotograma
            binaryWriter.Write(frameData);

            // Incrementar el contador de fotogramas
            frameCount++;
        }

        public void Close()
        {
            // Escribir la cabecera final del archivo AVI, que debe incluir el número total de fotogramas y otros metadatos.
            // Cerrar el archivo y liberar los recursos.
            binaryWriter.Close();
            fileStream.Close();
        }
    }


    class Program
    {
        public static Dictionary<BigInteger, BigInteger> facts = new Dictionary<BigInteger, BigInteger>();

        public static async void desaaaa()
        {
            String binary = await getStringFromByteArray(File.ReadAllBytes("c:\\logs\\WhatIsTheQuest.pdf"));

            addTextEscritorio(binary, "WhatIsTheQuest.pdf");
        }

        public static byte[] ReadWavFile(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(fileStream))
                {
                    reader.ReadBytes(44);
                    return reader.ReadBytes((int)fileStream.Length - 44);
                }
            }
        }

        public static short[] BytesToSamples(byte[] bytes)
        {
            short[] samples = new short[bytes.Length / 3];
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = (short)((bytes[i * 3 + 2] << 16) | (bytes[i * 3 + 1] << 8) | bytes[i * 3]);
            }
            return samples;
        }

        public static Complex[] CalculateFFT(short[] samples)
        {
            int N = samples.Length;
            Complex[] X = new Complex[N];
            for (int k = 0; k < N; k++)
            {
                X[k] = new Complex(samples[k], 0);
            }

            Complex[] x = new Complex[N];
            for (int n = 0; n < N; n++)
            {
                x[n] = new Complex(0, 0);
                for (int k = 0; k < N; k++)
                {
                    x[n] += X[k] * Complex.Exp(new Complex(0, -2 * Math.PI * k * n / N));
                }
            }
            return x;
        }

        public static float[] ComplexToMagnitude(Complex[] complexValues)
        {
            float[] magnitudes = new float[complexValues.Length / 2];
            for (int i = 0; i < magnitudes.Length; i++)
            {
                magnitudes[i] = (float)complexValues[i].Magnitude;
            }
            return magnitudes;
        }

        public static Bitmap CreateSpectrumImage(float[] magnitudes)
        {
            Bitmap image = new Bitmap(magnitudes.Length, 200, PixelFormat.Format32bppArgb);
            float maxMagnitude = 0;
            for (int i = 0; i < magnitudes.Length; i++)
            {
                if (magnitudes[i] > maxMagnitude)
                {
                    maxMagnitude = magnitudes[i];
                }
            }
            for (int x = 0; x < magnitudes.Length; x++)
            {
                int height = (int)(magnitudes[x] / maxMagnitude * 200);
                for (int y = 0; y < height; y++)
                {
                    image.SetPixel(x, 199 - y, Color.White);
                }
            }
            return image;
        }


        ////public class Pokey
        ////{
        ////    private const int SampleRate = 1790;
        ////    private const int NumChannels = 3;
        ////    private const int BitsPerSample = 8;

        ////    private int[] registers;
        ////    private int frameCounter;
        ////    private int sampleCounter;

        ////    private byte[] outputBuffer;

        ////    public Pokey()
        ////    {
        ////        registers = new int[16];
        ////        outputBuffer = new byte[SampleRate * NumChannels * BitsPerSample / 8];
        ////    }

        ////    public void ProcessAudio(byte[] audioData)
        ////    {
        ////        for (int i = 0; i < audioData.Length; i++)
        ////        {
        ////            int audioSample = audioData[i];
        ////            int clockCount = (i % 64) == 0 ? 2 : 1;
        ////            for (int j = 0; j < clockCount; j++)
        ////            {
        ////                frameCounter = (frameCounter + 1) % 114;
        ////                if (frameCounter == 0)
        ////                {
        ////                    ProcessFrame();
        ////                }
        ////                sampleCounter = (sampleCounter + 1) % SampleRate;
        ////                if (sampleCounter == 0)
        ////                {
        ////                    int audioIndex = i / 64;
        ////                    int bufferIndex = audioIndex * NumChannels * BitsPerSample / 8;
        ////                    outputBuffer[bufferIndex] = (byte)registers[0];
        ////                }
        ////            }
        ////        }
        ////    }

        ////    private void ProcessFrame()
        ////    {
        ////        for (int i = 0; i < 4; i++)
        ////        {
        ////            int channel = i * 2;
        ////            int volume = registers[channel + 8] & 0xF;
        ////            int output = 0;

        ////            if ((registers[channel] & 0x10) != 0) // si el canal está habilitado
        ////            {
        ////                int frequency = ((registers[channel] & 0x0F) << 8) | registers[channel + 1];
        ////                if (frequency != 0)
        ////                {
        ////                    int divisor = (1789773 / 15) / frequency;
        ////                    int counter = registers[channel + 4] << 8 | registers[channel + 5];
        ////                    counter--;
        ////                    if (counter < 0)
        ////                    {
        ////                        counter = divisor;
        ////                        int generator = registers[channel + 6];
        ////                        int feedback = (generator & 0x01) ^ ((generator & 0x04) >> 2);
        ////                        generator = ((generator >> 1) | (feedback << 15)) & 0x7FFF;
        ////                        registers[channel + 6] = (byte)generator;

        ////                        if ((generator & 0x8000) != 0)
        ////                        {
        ////                            output = volume;
        ////                        }
        ////                    }
        ////                    registers[channel + 4] = (byte)(counter >> 8);
        ////                    registers[channel + 5] = (byte)counter;
        ////                }
        ////            }

        ////            int noisePeriod = (registers[7] & 0x0F) << 1;
        ////            if (noisePeriod == 0)
        ////            {
        ////                noisePeriod = 2;
        ////            }
        ////            int noiseCounter = registers[6];
        ////            noiseCounter--;
        ////            if (noiseCounter < 0)
        ////            {
        ////                noiseCounter = noisePeriod;
        ////                int generator = registers[7];
        ////                int feedback = (generator & 0x01) ^ ((generator & 0x04) >> 2);
        ////                generator = ((generator >> 1) | (feedback << 15)) & 0x7FFF;
        ////                registers[7] = (byte)generator;

        ////                if ((generator & 0x0001) != 0)
        ////                {
        ////                    output = volume;
        ////                }
        ////            }
        ////            registers[6] = (byte)noiseCounter;

        ////            int bufferIndex = frameCounter * NumChannels * BitsPerSample / 8 + i * BitsPerSample / 8;
        ////            outputBuffer[bufferIndex] = (byte)(output * 16);
        ////        }
        ////    }

        ////    public void SaveWav(string fileName)
        ////    {
        ////        using (var stream = new FileStream(fileName + SampleRate + "C" + NumChannels + "_" + BitsPerSample + "bits.wav", FileMode.Create))
        ////        using (var writer = new BinaryWriter(stream))
        ////        {
        ////            // escribir el encabezado del archivo WAV
        ////            writer.Write("RIFF".ToCharArray());
        ////            writer.Write(outputBuffer.Length + 36);
        ////            writer.Write("WAVE".ToCharArray());
        ////            writer.Write("fmt ".ToCharArray());
        ////            writer.Write(16);
        ////            writer.Write((short)1);
        ////            writer.Write((short)NumChannels);
        ////            writer.Write(SampleRate);
        ////            writer.Write(SampleRate * NumChannels * BitsPerSample / 8);
        ////            writer.Write((short)(NumChannels * BitsPerSample / 8));
        ////            writer.Write((short)BitsPerSample);
        ////            writer.Write("data".ToCharArray());
        ////            writer.Write(outputBuffer.Length);
        ////            writer.Write(outputBuffer);
        ////        }
        ////    }
        ////}
       

        public class Pokey
        {
        private int[] registers = new int[16];
        private double[] phaseAccumulator = new double[4];
        private const int sampleRate = 1790;
        private const double samplePeriod = 1.0 / sampleRate;
        private const int oversample = 8;

        public void Write(int address, int value)
        {
            registers[address] = value;
        }

        public byte Read(int address)
        {
            return (byte)registers[address];
        }

        public void ProcessFrame(byte[] buffer, int offset, int count)
        {
            int samples = count / 2;
            int[] output = new int[samples];

            for (int i = 0; i < samples; i++)
            {
                for (int channel = 0; channel < 4; channel++)
                {
                    int frequency = ((registers[channel] & 0xF) << 8) | registers[channel + 4];
                    int volume = registers[channel + 8] & 0xF;

                    phaseAccumulator[channel] += frequency * samplePeriod;
                    int intPart = (int)phaseAccumulator[channel];
                    phaseAccumulator[channel] -= intPart;

                    int sample = (int)(32767 * Math.Sin(2 * Math.PI * intPart / sampleRate));
                    sample = sample * volume / 15;

                    output[i] += sample;
                }

                if (i % oversample == 0)
                {
                    int index = i / oversample;
                    short value = (short)(output[i] / oversample);
                    buffer[offset + index * 2] = (byte)(value & 0xFF);
                    buffer[offset + index * 2 + 1] = (byte)((value >> 8) & 0xFF);
                }
            }
        }
    }

        static void SaveAsWav(string filePath, int sampleRate, int bitDepth, byte[] data)
        {
            using (var writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                // Cabecera del archivo WAV
                writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                writer.Write(36 + data.Length);
                writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
                writer.Write(new char[4] { 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)1);
                writer.Write(sampleRate);
                writer.Write(sampleRate * (bitDepth / 8));
                writer.Write((short)(bitDepth / 8));
                writer.Write((short)bitDepth);
                writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                writer.Write(data.Length);

                // Datos de audio
                writer.Write(data);
            }
        }

        /// <summary>
        /// A high performance BigInteger to binary string converter
        /// that supports 0 and negative numbers.
        /// License: MIT / Created by Ryan Scott White, 7/16/2022;
        /// </summary>
        public static string BigIntegerToBinaryString(BigInteger x)
        {
            return String.Empty;
            //////// Setup source
            //////ReadOnlySpan<byte> srcBytes = x.ToByteArray();
            //////int srcLoc = srcBytes.Length - 1;

            //////// Find the first bit set in the first byte so we don't print extra zeros.
            //////int msb = BitOperations.Log2(srcBytes[srcLoc]);

            //////// Setup Target
            //////Span<char> dstBytes = stackalloc char[srcLoc * 8 + msb + 2];
            //////int dstLoc = 0;

            //////// Add leading '-' sign if negative.
            //////if (x.Sign < 0)
            //////{
            //////    dstBytes[dstLoc++] = '-';
            //////}
            ////////else if (!x.IsZero) dstBytes[dstLoc++] = '0'; // add adding leading '0' (optional)

            //////// The first byte is special because we don't want to print leading zeros.
            //////byte b = srcBytes[srcLoc--];
            //////for (int j = msb; j >= 0; j--)
            //////{
            //////    dstBytes[dstLoc++] = (char)('0' + ((b >> j) & 1));
            //////}

            //////// Add the remaining bits.
            //////for (; srcLoc >= 0; srcLoc--)
            //////{
            //////    byte b2 = srcBytes[srcLoc];
            //////    for (int j = 7; j >= 0; j--)
            //////    {
            //////        dstBytes[dstLoc++] = (char)('0' + ((b2 >> j) & 1));
            //////    }
            //////}

            //////return dstBytes.ToString();
        }

        private static void sM5()
        {
            Console.WriteLine("PASO 1");
            BigInteger[][] matrix = new BigInteger[1000][];
            BigInteger n = 4;
            Int32 i = 0;
            BigInteger pow = 32;
            BigInteger[] array = new BigInteger[1000];

            List<BigInteger> offsets = new List<BigInteger>() { 1, 3, 2, 1, 6, 15, 10, 52, 55, 2, 1, 15, 129, 389, 184, 2, 1, 21, 266, 1563, 2539, 648, 2, 1, 28, 487, 4642, 16445, 16604, 2111, 2, 1, 36, 820, 11407, 69863, 169034, 105365, 6352, 2, 1, 45, 1297, 24600, 228613, 1016341, 1686534, 654030, 17337, 2 };
            Int32 iter = 0;
            while (true)
            {
                

                Console.WriteLine((n) + ": " + (pow));
                Console.ReadKey();

                pow *= Potencia(4, offsets[iter]);
                n += offsets[iter];
                iter++;
                

            }                        
        }

        public static List<List<int>> CalculateTriangle(int n)
        {
            List<List<int>> triangle = new List<List<int>>();

            for (int i = 0; i <= n; i++)
            {
                List<int> row = new List<int>();
                for (int k = 0; k < i; k++)
                {
                    int count = 0;
                    GeneratePermutations(new List<int>(Enumerable.Range(1, i)), i, k, 0, new int[i], new bool[i], ref count);
                    row.Add(count);
                }
                triangle.Add(row);
            }

            return triangle;
        }

        public static void GeneratePermutations(List<int> arr, int n, int k, int pos, int[] inversionCount, bool[] used, ref int count)
        {
            if (pos == n)
            {
                if (inversionCount[pos - 1] == k)
                {
                    count++;
                }
                return;
            }

            for (int i = 0; i < n; i++)
            {
                if (!used[i])
                {
                    used[i] = true;
                    inversionCount[pos] = inversionCount[pos - 1] + GetInversions(arr, pos, i);
                    GeneratePermutations(arr, n, k, pos + 1, inversionCount, used, ref count);
                    used[i] = false;
                }
            }
        }

        public static int GetInversions(List<int> arr, int n, int m)
        {
            int invCount = 0;
            for (int i = n; i < m; i++)
            {
                for (int j = i + 1; j <= m; j++)
                {
                    if (arr[i] > arr[j])
                    {
                        invCount++;
                    }
                }
            }
            return invCount;
        }

        public class TupleInt
        {
            public BigInteger Item1 { get; set; }
            public BigInteger Item2 { get; set; }

            public TupleInt(BigInteger item1, BigInteger item2)
            {
                Item1 = item1;
                Item2 = item2;
            }
        }

        public static List<int> Row(int n)
        {
            List<int> outList = new List<int>();
            HashSet<int[]> reach = new HashSet<int[]>();
            HashSet<int[]> frontier = new HashSet<int[]>();

            int[] perm = new int[n];
            for (int i = 0; i < n; i++)
            {
                perm[i] = i + 1;
            }

            reach.Add(perm);
            frontier.Add(perm);

            if (n == 0)
            {
                outList.Add(1);
            }

            for (int k = 0; k < n; k++)
            {
                HashSet<int[]> r1 = new HashSet<int[]>();
                outList.Add(frontier.Count);

                while (frontier.Count > 0)
                {
                    int[] q = frontier.First();
                    frontier.Remove(q);

                    for (int i = 0; i < n; i++)
                    {
                        for (int j = i + 1; j <= n; j++)
                        {
                            int[] qp = new int[n];
                            Array.Copy(q, qp, n);
                            Array.Reverse(qp, i, j - i);

                            if (!reach.Contains(qp))
                            {
                                reach.Add(qp);
                                r1.Add(qp);
                            }
                        }
                    }
                }

                frontier = r1;
            }

            return outList;
        }

        private static void sM4()
        {
            Console.WriteLine("PASO 1");
            BigInteger[][] matrix = new BigInteger[1000][];
            BigInteger n = 1;
            Int32 i = 0;
            BigInteger pow = 2;
            BigInteger[] array = new BigInteger[1000];
            while (n < 1001)
            {
                

                array[i] = pow;

                //if (resto != 0 && n % 3 != 0)
                //{
                //    //BigInteger resto3 = -1;
                //    //BigInteger div3 = BigInteger.DivRem(div, 2, out resto3);

                //    //if (resto3 == 0)
                //    //{
                //    //    if (esPrimoNormal(div3))
                //    //    {                            
                //            Console.WriteLine(String.Concat(";", n));
                //            Console.ReadKey();
                //    //    }
                //    //}
                //}
                pow *= 2;
                n++;
                i++;

            }

            Console.WriteLine("PASO 2");

            matrix[0] = array;

            
            Int32 iInit = 0;


            //while (i < 19)
            //{
            String nS = String.Empty;
            for (Int32 j = 0; j < 999; j++)
            {
                BigInteger[] arr = new BigInteger[1000]; 
                Boolean isFirst = true;
                BigInteger sum = 0;
                for (Int32 k = iInit; k < 999; k++)
                {
                    sum += matrix[j][k];

                    //if (isFirst)
                    //{
                    //    isFirst = false;
                    //    nS += " ".PadLeft(10, ' ');
                    //}
                    //else
                    //{
                        arr[k] = sum;
                    if (j < 24 && k < 24)
                    {
                        nS += " " + sum.ToString().PadLeft(10, ' ');
                    }
                    //}                                                            
                }
                matrix[j + 1] = arr;
                //////iInit++;

                if (j < 24)
                {
                    addTextEscritorio(String.Concat("", nS), "2_Pascal");
                }
                nS = String.Empty;
            }

            Console.WriteLine("PASO 3");
            BigInteger xa = 3;
            for (Int32 k = 3; k < 999; k+=2) 
            {
                BigInteger sum = matrix[0][k] ;
                //BigInteger pppp = Potencia(2, xa);
                
                //List<BigInteger> list = new List<BigInteger>();
                BigInteger nN = 0;
                for (Int32 j = 1; j < 999; j ++)
                {
                    try
                    {
                        BigInteger number = matrix[j][k];
                        
                        if(number != 0)
                        {
                            sum = number;
                            //sum += number;
                            nN++;
                            
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                                        
                }

                //Boolean print = false;
                //for (Int32 j = 1; j < 999; j++)
                //{
                //    try
                //    {
                //        BigInteger number = matrix[j][k];

                //        if (number != 0)
                //        {
                //            if(sum % number == 0)
                //            {
                //                print = true;
                //            }                            
                //        }
                //    }
                //    catch (Exception ex)
                //    {

                //    }

                //}


                BigInteger resto = -1;
                BigInteger div = BigInteger.DivRem(sum + k + 2, (k+2) * (k+2), out resto);

                //BigInteger resto2 = -1;
                //BigInteger div2 = BigInteger.DivRem(div, nN, out resto2);

                if (true)
                {
                    //String listS = String.Empty;
                    //foreach (BigInteger b in list)
                    //{
                    //    listS += ";" + b;
                    //}

                    Console.WriteLine((k + 2) + ": "+ (div));
                    //Console.WriteLine(listS);
                    //Console.WriteLine("^");
                    Console.ReadKey();
                }
                //addTextEscritorio(String.Concat("", nS), "2_Pascal");
                
            }

        }

        private static void sM3()
        {
            List<BigInteger> nNs = new List<BigInteger>() { 1,2 };
            BigInteger n = 2;
            
            while (true)
            {

                BigInteger pow = Potencia(2, n - 1);
                pow--;
                BigInteger resto = -1;
                BigInteger div = BigInteger.DivRem(pow, n, out resto);
                                
                if (resto == 0)
                {
                    BigInteger resto3 = -1;
                    BigInteger div3 = BigInteger.DivRem(div, 3, out resto3);

                    if (resto3 == 0)
                    {
                        BigInteger restoF = -1;
                        BigInteger divF = BigInteger.DivRem(div3 + n ^ 2, n , out restoF);

                        if (restoF == 0)
                        {

                            Console.WriteLine(String.Concat(";", n));                            
                            //Console.ReadKey();
                        }
                    }
                }
                                
                n++;

            }
        }

        private static void sM()
        {            
            BigInteger n = 2;
            BigInteger nI = 1;
            List<Task> tasks = new List<Task>();
            BigInteger hilos = 0;            
            while (true)
            {
                if (nI % 2 != 0)
                {
                    BigInteger resto = BigInteger.Remainder(n - 2, nI);
                    if (resto == 0)
                    {
                        //BigInteger ele2 = n;
                        //BigInteger factM = factorial(nI) - ele2;

                        //BigInteger resto02 = BigInteger.Remainder(factM, (nI * 2) + 1);
                        //if (resto02 != 0)
                        //{

                        //var list = DivisoresPof2(n);
                        BigInteger init = (n * 2) - 1; // list.Aggregate(BigInteger.Add);
                        BigInteger nN = n;
                        BigInteger nIC = nI;
                        hilos++;

                        if (!File.Exists(@"C:\logs\nM\YES\nM" + nIC + ".txt") && !File.Exists(@"C:\logs\nM\NO\nM" + nIC + ".txt"))
                        {
                            if (!File.Exists(@"C:\logs\nM\nM" + nIC + ".txt"))
                            {
                                addTextEscritorioNM(String.Concat(";", nN - 1), "nM" + nIC.ToString());
                            }

                            Task tsk = Task.Factory.StartNew(() =>
                            {
                                BigInteger nM = DivisoresRa(init - nN);

                                if (nM == -1)
                                {
                                    Console.WriteLine(String.Concat(";", nN - 1));
                                    addTextEscritorioNM(String.Concat("V"), "nM" + nIC);
                                    File.Move(@"C:\logs\nM\nM" + nIC + ".txt", @"C:\logs\nM\YES\nM" + nIC + ".txt");
                                }
                                else
                                {
                                    addTextEscritorioNM(String.Concat("d: ", nM), "nM" + nIC);
                                    File.Move(@"C:\logs\nM\nM" + nIC + ".txt", @"C:\logs\nM\NO\nM" + nIC + ".txt");
                                }
                            });

                            tasks.Add(tsk);
                            tsk.ContinueWith(t =>
                            {
                                // Bloquear la lista para evitar condiciones de carrera
                                lock (tasks)
                                {
                                    // Eliminar la tarea de la lista
                                    tasks.Remove(t);
                                }
                            });

                            while (tasks.Count > 1000)
                            {
                                Thread.Sleep(1000);
                            }
                            //}
                        }
                    }
                }

                n*=2;
                nI++;

            }
        }

        private static void whole()
        {
            BigInteger i = 2;
            BigInteger n = 1;
            Int32 arco = 1;
            Int32 arcoCON = 0;
            Boolean change = false;
            List<BigInteger> list = new List<BigInteger>();
            while (true)
            {
                n = n * 2 + 1;

                arcoCON++;

                if (arcoCON == arco)
                {
                    BigInteger r = n;

                    Boolean isDivi = false;
                    BigInteger nT = n;
                    for (Int32 z = 0; z < list.Count; z++)
                    {
                        if(nT % list[z] == 0)
                        {
                            if (nT / list[z] != 1)
                            {
                                nT = nT / list[z];
                                z = 0;
                            }
                            else
                            {
                                if (!list.Contains(nT))
                                {
                                    list.Add(nT);
                                }
                                z = 0;
                            }

                            isDivi = true;
                            
                        }
                    }

                    if (!isDivi)
                    {
                        list.Add(n);
                        Console.WriteLine(String.Concat(";", r, " i: ", i, " arco: ", arco));
                        addTextEscritorio(String.Concat(";", r, " i: ", i, " arco: ", arco), "nM");
                        Console.ReadKey();
                    }
                    else
                    {
                        if(nT != 1 && !list.Contains(nT))
                        {
                            list.Add(nT);
                        }
                    }

                if (change)
                {
                    arco++;
                }

                change = !change;
                arcoCON = 0;
            }

            i++;
                
            }
        }


            private static void longWay2()
        {
            BigInteger i = 2;
            
            while (true)
            {
                BigInteger process = i;
                for (BigInteger j = i + 1 ; j < i * i; j++)
                {
                    BigInteger resto = -1;
                    BigInteger div = BigInteger.DivRem(j, i, out resto);
                    process *= resto == 0 ? div : j;                                      
                }



                BigInteger r = process + 1;
                                
                Console.WriteLine(String.Concat(";", r));
                addTextEscritorio(String.Concat(";", r), "outPo2andR2");
                Console.ReadKey();

                i++;                
                
            }


        }

        private static void longWay()
        {            
            BigInteger iT = 3;

            while (true)
            {
                BigInteger result = iT;                
                BigInteger dividendo = 2;
                Boolean isPrime = true;
                while (result > 0)
                {
                    BigInteger tempResult = result / dividendo;
                    BigInteger more = result + tempResult;
                    BigInteger minus = result - tempResult;                    
                    BigInteger moreDiv = tempResult + dividendo;
                    BigInteger minusDiv = tempResult - dividendo;

                    if (
                       (more != 1 && more != iT && (iT % more == 0)) || 
                       (minus != 1 && minus != -1 && minus != 0 && minus != iT && minus != -iT && (iT % minus == 0)) ||
                       (moreDiv != 1 && moreDiv != iT && (iT % moreDiv == 0)) ||
                       (minusDiv != 1 && minusDiv != -1 && minusDiv != 0 && minusDiv != iT && minusDiv != -iT && (iT % minusDiv == 0)) ||
                       ((tempResult - 1) != 1 && (tempResult - 1) != -1 && (tempResult - 1) != 0 && (tempResult - 1) != iT && (iT % (tempResult - 1) == 0))
                       )
                    {
                        isPrime = false;
                        break;
                    }

                    dividendo++;
                    result = tempResult;
                }

                if (isPrime)
                {
                    Console.WriteLine(String.Concat(";", iT));
                    addTextEscritorio(String.Concat(";", iT), "vamossssss");
                    Console.ReadKey();
                }
                
                iT+=2;
            }


        }

        static bool ValidatePassword(string filePath, string password)
        {
            try
            {
                                
                //using (SevenZipExtractor.ArchiveFile extr = new SevenZipExtractor.ArchiveFile(filePath, new SevenZipExtractor.Formats( ))
                //{

                //    //var options = new SharpCompress.Readers.ReaderOptions
                //    //{
                //    //    Password = password,                    
                //    //    ArchiveEncoding = new ArchiveEncoding { Default = Encoding.UTF8, Password = Encoding.UTF8 }

                //    //};

                //    ////using (var stream = File.OpenRead(filePath))
                //    //using (var archive = RarArchive.Open(filePath, options : new SharpCompress.Readers.ReaderOptions { Password = password }))
                //    //{
                //    //    // No es necesario realizar ninguna operación
                //    //    var entry = archive.Entries.FirstOrDefault();
                //    //    if (entry != null)
                //    //    {
                //    //        entry.WriteToDirectory(".", new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                //    //    }
                //    //    return true;
                //    //}
                //    return true;
                //}
                    
            }
            catch (Exception ex)
            {
                // La contraseña es incorrecta
            }
            return false;

        }

        private static bool ByteArrayToFile(byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(@"c:\logs\FILEZ.txt", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        private static string ConvertToBinaryS(int x)
        {
            char[] bits = new char[32];
            int i = 0;

            while (x != 0)
            {
                bits[i++] = (x & 1) == 1 ? '1' : '0';
                x >>= 1;
            }

            Array.Reverse(bits, 0, i);
            return new string(bits);
        }

        // Definir una función que asocie cada número natural n con el número de vértices del polígono regular de k lados inscrito en una circunferencia de radio n, y que tiene un vértice en el punto (n, 0)
        static BigInteger Vertices(BigInteger n, int k)
        {
            // Calcular el valor de la fórmula usando los operadores aritméticos y la clase Math
            return ((BigInteger.Pow(n, k) - 1) / (n - 1));
        }

        // Definir una función que asocie cada número natural n con el número de triángulos infinitos formados por segmentos perpendiculares entre sí cuya longitud sigue una serie geométrica decreciente de razón 1/n
        static BigInteger Triangulos(BigInteger n)
        {
            // Calcular el valor de la fórmula usando los operadores aritméticos
            return n * (n - 1) / 2;
        }

        // Definir una función que asocie cada número natural n con el área del triángulo infinito formado por segmentos perpendiculares entre sí cuya longitud sigue una serie geométrica decreciente de razón 1/n
        static BigInteger Area(BigInteger n)
        {
            // Calcular el valor de la fórmula usando los operadores aritméticos y la clase Math
            return 2 * (BigInteger.Pow(n, 2) - 1);
            //return 1 / (2 * (BigInteger.Pow(n, 2) - 1));
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

        public static String FromHex(string hex)
        {
            //hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            //return raw;
            return Encoding.ASCII.GetString(raw);
        }

        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {

            // Cargar archivo binario de datos de PDF
            Byte[] pdfData = File.ReadAllBytes("c:\\logs\\enjin-multiverse-catalog.pdf");

            List<Byte> data = new List<Byte>();
            Boolean copyToDataStream = false;
            Int32 itxt = 1;
            String sLine = String.Empty;

            for (Int32 i = 0; i < pdfData.Length; i++)
            {
                sLine += Encoding.UTF8.GetString(new Byte[] { pdfData[i] });

                if (sLine.Contains("endstream"))
                {
                    sLine = String.Empty;

                    List<Byte> bytesFinal = new List<Byte>();
                    String streamNameFinal = "endstream";
                    Boolean removing = false;

                    for (Int32 removeI = data.Count - 1; removeI >= 0; removeI--)
                    {
                        foreach (char cEestream in streamNameFinal)
                        {
                            if (cEestream.ToString() == Encoding.UTF8.GetString(new byte[] { data[removeI] }))
                            {
                                streamNameFinal = streamNameFinal.Remove(0, 1);
                                removing = true;
                                break;
                            }
                        }

                        if (!removing && (Encoding.UTF8.GetString(new byte[] { data[removeI] }) != "\r" &&
                            Encoding.UTF8.GetString(new byte[] { data[removeI] }) != "\n"))
                        {
                            bytesFinal.Add(data[removeI]);
                        }

                        removing = false;
                    }

                    bytesFinal.Reverse();
                    Byte[] dataArray = bytesFinal.ToArray();
                    Byte[] dataArray2 = new byte[dataArray.Length - 2];

                    Array.Copy(dataArray, 2, dataArray2, 0, dataArray.Length - 2);

                    byte[] compressedData = dataArray2;

                    using (var compressedStream = new MemoryStream(compressedData))
                    {
                        using (var decompressionStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                        {
                            using (var decompressedStream = new MemoryStream())
                            {
                                try
                                {
                                    decompressionStream.CopyTo(decompressedStream);
                                    byte[] decompressedData = decompressedStream.ToArray();

                                    // Guardar datos descomprimidos en un archivo
                                    File.WriteAllBytes($"c:\\logs\\new\\_FLATE{itxt}DATA.bin", decompressedData);

                                    // Intentar guardar como imagen PNG
                                    try
                                    {
                                        using (MemoryStream pngStream = new MemoryStream(decompressedData))
                                        {
                                            using (Bitmap pngBitmap = new Bitmap(pngStream))
                                            {
                                                pngBitmap.Save($"c:\\logs\\new\\IDAT_{itxt}.png", ImageFormat.Png);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error al guardar PNG: {ex.Message}");
                                        // Guardar datos como texto en caso de error
                                        File.WriteAllText($"c:\\logs\\new\\_FLATE{itxt}TEXT.txt", Encoding.UTF8.GetString(decompressedData));
                                    }

                                    // Intentar guardar como archivo de texto
                                    try
                                    {
                                        string textData = Encoding.UTF8.GetString(decompressedData);
                                        File.WriteAllText($"c:\\logs\\new\\_FLATE{itxt}TEXT.txt", textData);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error al guardar texto: {ex.Message}");
                                    }

                                    // Intentar guardar como archivo WAV
                                    try
                                    {
                                        int sampleRate = 44100; // Frecuencia de muestreo
                                        short bitsPerSample = 16; // Tamaño de muestra en bits
                                        short channels = 2; // Estéreo

                                        string outputFilePath = $"c:\\logs\\new\\_FLATE{itxt}SOUND.wav";

                                        using (BinaryWriter writer = new BinaryWriter(File.Create(outputFilePath)))
                                        {
                                            writer.Write("RIFF".ToCharArray());
                                            writer.Write(36 + decompressedData.Length);
                                            writer.Write("WAVE".ToCharArray());

                                            writer.Write("fmt ".ToCharArray());
                                            writer.Write(16);
                                            writer.Write((short)1);
                                            writer.Write(channels);
                                            writer.Write(sampleRate);
                                            writer.Write(sampleRate * channels * bitsPerSample / 8);
                                            writer.Write((short)(channels * bitsPerSample / 8));
                                            writer.Write(bitsPerSample);

                                            writer.Write("data".ToCharArray());
                                            writer.Write(decompressedData.Length);
                                            writer.Write(decompressedData);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error al guardar WAV: {ex.Message}");
                                        // Guardar datos como texto en caso de error
                                        File.WriteAllText($"c:\\logs\\new\\_FLATE{itxt}TEXT.txt", Encoding.UTF8.GetString(decompressedData));
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Error al descomprimir: {e.Message}");
                                }
                            }
                        }
                    }

                    itxt++;
                    copyToDataStream = false;
                }
                else if (sLine.Contains("stream"))
                {
                    sLine = String.Empty;
                    data = new List<Byte>();
                    copyToDataStream = true;
                }
                else
                {
                    if (copyToDataStream)
                    {
                        data.Add(pdfData[i]);
                    }
                }
            }

            //// Cargar archivo binario de datos de PDF
            //Byte[] pdfData = File.ReadAllBytes("c:\\logs\\enjin-multiverse-catalog.pdf");

            //List<Byte> data1 = new List<Byte>();
            //Boolean copyToDataStream = false;
            //Int32 itxt1 = 1;
            //String sLine = String.Empty;

            //for (Int32 i = 0; i < pdfData.Length; i++)
            //{
            //    sLine += Encoding.UTF8.GetString(new Byte[] { pdfData[i] });

            //    if (sLine.Contains("endstream"))
            //    {
            //        sLine = String.Empty;

            //        List<Byte> bytesFinal = new List<Byte>();
            //        String streamNameFinal = "endstream";
            //        Boolean removing = false;

            //        for (Int32 removeI = data1.Count - 1; removeI >= 0; removeI--)
            //        {
            //            foreach (char cEestream in streamNameFinal)
            //            {
            //                if (cEestream.ToString() == Encoding.UTF8.GetString(new byte[] { data1[removeI] }))
            //                {
            //                    streamNameFinal = streamNameFinal.Remove(0, 1);
            //                    removing = true;
            //                    break;
            //                }
            //            }

            //            if (!removing && (Encoding.UTF8.GetString(new byte[] { data1[removeI] }) != "\r" &&
            //                Encoding.UTF8.GetString(new byte[] { data1[removeI] }) != "\n"))
            //            {
            //                bytesFinal.Add(data1[removeI]);
            //            }

            //            removing = false;
            //        }

            //        bytesFinal.Reverse();
            //        Byte[] dataArray = bytesFinal.ToArray();
            //        Byte[] dataArray2 = new byte[dataArray.Length - 2];

            //        Array.Copy(dataArray, 2, dataArray2, 0, dataArray.Length - 2);

            //        byte[] compressedData = dataArray2;

            //        using (var compressedStream = new MemoryStream(compressedData))
            //        {
            //            using (var decompressionStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            //            {
            //                using (var decompressedStream = new MemoryStream())
            //                {
            //                    try
            //                    {
            //                        decompressionStream.CopyTo(decompressedStream);
            //                        byte[] decompressedData = decompressedStream.ToArray();

            //                        File.WriteAllBytes($"c:\\logs\\new\\_FLATE{itxt1}TEXT.txt", decompressedData);

            //                        try
            //                        {
            //                            using (MemoryStream pngStream = new MemoryStream(decompressedData))
            //                            {
            //                                using (Bitmap pngBitmap = new Bitmap(pngStream))
            //                                {
            //                                    pngBitmap.Save($"c:\\logs\\new\\IDAT_{itxt1}.png", ImageFormat.Png);
            //                                }
            //                            }
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            Console.WriteLine($"Error al guardar PNG: {ex.Message}");
            //                        }
            //                    }
            //                    catch (Exception e)
            //                    {
            //                        Console.WriteLine($"Error al descomprimir: {e.Message}");
            //                    }
            //                }
            //            }
            //        }

            //        itxt1++;
            //        copyToDataStream = false;
            //    }
            //    else if (sLine.Contains("stream"))
            //    {
            //        sLine = String.Empty;
            //        data1 = new List<Byte>();
            //        copyToDataStream = true;
            //    }
            //    else
            //    {
            //        if (copyToDataStream)
            //        {
            //            data1.Add(pdfData[i]);
            //        }
            //    }
            //}
            return;
            //String hexS = 
            //    "424d7602000000000000760000002800" +
            //    "00002000000020000000010004000000" +
            //    "00000000000000000000000000000000" +
            //    "00000000000000000000000080000080" +
            //    "00000080800080000000800080008080" +
            //    "000080808000c0c0c0000000ff0000ff" +
            //    "000000ffff00ff000000ff00ff00ffff" +
            //    "0000ffffff0000000000000000000000" +
            //    "00000000000000000000000000000900" +
            //    "00000000000011110119110110100909" +
            //    "01091111019011011909099111100911" +
            //    "09111910901119011919101011100901" +
            //    "91109109101090991111111119000901" +
            //    "91010119009911101191991109900991" +
            //    "01111111911009190100119091100901" +
            //    "11991001111191111119101199100910" +
            //    "01111111191011090910191010100901" +
            //    "11190001101910111101990111900919" +
            //    "11911191011119109900011909100919" +
            //    "10911101111191019119110099900901" +
            //    "01991901911019919109119911100991" +
            //    "11101191991090110111111911900911" +
            //    "00191011011199999999999999990999" +
            //    "99999999999900000000000000000000" +
            //    "00000000000090000000000000000000" +
            //    "00000000000099111111191019191109" +
            //    "10909190910091191909011009011111" +
            //    "91111111100091110119101110010111" +
            //    "90111111910099091910119009909101" +
            //    "19099111010090101911001111001011" +
            //    "01101119110090191091019019990011" +
            //    "91011101910099090901101191011091" +
            //    "99111090910091110010110110191909" +
            //    "10009901010091011991199111091011" +
            //    "00910010900099011110091010190901" +
            //    "91901109110090991111119019011901" +
            //    "91010119090091101191990909901191" +
            //    "01191111910091190100110091101101" +
            //    "11111001110099999999999999999999" +
            //    "999999999990";

            //var number = BigInteger.Parse(hexS, NumberStyles.HexNumber);

            //BigInteger iRB = 0;
            //while (true)
            //{
            //    BigInteger nMas = number + iRB;
            //    BigInteger nMenos = number - iRB;

            //    var messages = new List<BigInteger>() { nMas, nMenos }; // Assume this contains 1000 items
            //    Parallel.ForEach(messages, new ParallelOptions { MaxDegreeOfParallelism = 13 }, msg =>
            //    {
            //        // Your processing logic for each message                

            //        string body = FromHex(msg.ToString("X"));
            //        MatchCollection matches = Regex.Matches(body, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");

            //        foreach (Match match in matches)
            //        {
            //            Console.WriteLine(iRB.ToString() + "_" + match.Value);
            //        }



            //        //body = FromHex(nMenos.ToString("X"));
            //        //matches = Regex.Matches(body, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");

            //        //foreach (Match match in matches)
            //        //{
            //        //    Console.WriteLine(match.Value);
            //        //}
            //        // Whatever code you want in your thread
            //    });

            //    iRB++;
            //}

            //Console.ReadKey();
            //return;
            ////List<BigInteger> nNopSumAdd = new List<BigInteger>();
            ////List<BigInteger> nNopProdAdd = new List<BigInteger>();

            ////List<BigInteger> nNopSumLess = new List<BigInteger>();
            ////List<BigInteger> nNopProdLess = new List<BigInteger>();

            //for (BigInteger n = 1; n < 101000000; n++)
            //{
            //    //for (BigInteger mM = 1; mM < 10; mM++)
            //    //{
            //        BigInteger mAdd = n + 2;




            //        BigInteger t1Add = Potencia(mAdd, 2) - Potencia(n, 2);
            //        BigInteger t2Add = 2 * mAdd * n;
            //        BigInteger t3Add = Potencia(mAdd, 2) + Potencia(n, 2); ;

            //        BigInteger sumM = t1Add + t2Add + t3Add;

            //        //if (!nNopSumAdd.Contains(mM))
            //        //{
            //            if (sumM % n == 0)
            //            {
            //                Console.WriteLine(String.Concat(";n= ", n, "; sum ", sumM, " div=" , sumM / n)); //, " > " + rem));
            //            }
            //        //    else
            //        //    {
            //        //        nNopSumAdd.Add(mM);
            //        //    }
            //        //}
            //        //BigInteger prod = t1Add * t2Add * t3Add;

            //        //if (!nNopProdAdd.Contains(mM))
            //        //{
            //        //    if (prod % n != 0)
            //        //    {
            //        //        Console.WriteLine(String.Concat(";n= ", n, "; prod ", prod, " V  mM+", mM)); //, " > " + rem));
            //        //    }
            //        //    else
            //        //    {
            //        //        nNopProdAdd.Add(mM);
            //        //    }
            //        //}


            //    //}

            //    ////////for (BigInteger mM = 1; mM < 10; mM++)
            //    ////////{

            //    //////    BigInteger mLess = n - 6;


            //    //////    BigInteger t1Less = Potencia(mLess, 2) - Potencia(n, 2);
            //    //////    BigInteger t2Less = 2 * mLess * n;
            //    //////    BigInteger t3Less = Potencia(mLess, 2) + Potencia(n, 2); ;

            //    //////    BigInteger sumL = t1Less + t2Less + t3Less;
            //    //////    //if (!nNopSumLess.Contains(mM))
            //    //////    //{
            //    //////        if (sumL / n % n == 0)
            //    //////        {
            //    //////            Console.WriteLine(String.Concat(";n= ", n, "; sumL ", sumL, " div=", sumL/n)); //, " > " + rem));
            //    //////        }
            //    //////    //    else
            //    //////    //    {
            //    //////    //        nNopSumLess.Add(mM);
            //    //////    //    }
            //    //////    //}

            //        //BigInteger prod = t1Less * t2Less * t3Less;
            //        //if (!nNopProdLess.Contains(mM))
            //        //{
            //        //    if (prod % n == 0)
            //        //    {
            //        //        Console.WriteLine(String.Concat(";n= ", n, "; prod ", prod, " V  mM-", mM)); //, " > " + rem));
            //        //    }
            //        //    else
            //        //    {
            //        //        nNopProdLess.Add(mM);
            //        //    }
            //        //}




            //    //}

            //    Console.ReadKey();

            //}

            ////String binaryS = FromBase10("54277541829991966611076000958209137570056094735226680809598097948173314031616", 2);

            ////Byte[] bbb22 = GetBytesFromBinaryString(binaryS);

            //// File.WriteAllBytes("c:\\logs\\hhh.txt", bbb22);


            //return;



            //System.Net.WebClient c = new WebClient();
            //c.DownloadDataCompleted +=
            //     new DownloadDataCompletedEventHandler(c_DownloadDataCompleted);
            ////List<String> words = hollidaysgost();
            //enjinHunt2023(c);

            //return;

            //System.Numerics.BigInteger bI01 =
            //    System.Numerics.BigInteger.Parse(ToBase10("3d7d20a47000000000000002000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000006000000000000000000000000000000000000000000000000000000000000000a0000000000000000000000000000000000000000000000000000000000000000100000000000000000000000067dcf9536f0fea888bbe3ed87e2b9faedbf6b87600000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000001", 16));

            //System.Numerics.BigInteger bI02 =
            //    System.Numerics.BigInteger.Parse(ToBase10("cd23dde000000000000000000000000000000000000000000000000000000000000001600000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000100000000000000000000000003ec388fb9aef6442c7372db3c6b7eed93469c0b00000000000000000000000000000000000000000000f4becbf58b027a640000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000c546865204d6f6e6f6c6974680000000000000000000000000000000000000000", 16));

            //System.Numerics.BigInteger bIR = bI01 + bI02;

            //String hexF = FromBase10(bIR.ToString(), 16);


            //////BigInteger n = 2;
            //////List<BigInteger> secuenciaOn = new List<BigInteger>() { 1, 1 };
            //////while (true)
            //////{
            //////    List<BigInteger> secuenciaTemp = new List<BigInteger>();
            //////    BigInteger nAnt = 1;
            //////    String sOut = ";";
            //////    foreach (BigInteger b in secuenciaOn)
            //////    {
            //////        if (nAnt + b == n)
            //////        {
            //////            secuenciaTemp.Add(n);
            //////            sOut += n.ToString() + ",";
            //////        }
            //////        secuenciaTemp.Add(b);
            //////        sOut += b.ToString() + ",";
            //////        nAnt = b;
            //////    }
            //////    sOut += ';';
            //////    secuenciaOn = secuenciaTemp;
            //////    Console.WriteLine(sOut);
            //////    Console.ReadKey();

            //////    n++;
            //////}
            ////////// Definir una variable n de tipo int
            ////////List<BigInteger> secuencia = new List<BigInteger>() { 1, 1 };
            ////////BigInteger n = 2;

            ////////// Crear un bucle while que se ejecute mientras n sea menor que 100
            ////////while (true)
            ////////{
            ////////    List<BigInteger> newSec = new List<BigInteger>();

            ////////    BigInteger antBI = -1;
            ////////    BigInteger sumIter = 0;
            ////////    foreach (BigInteger bSIn in secuencia)
            ////////    {
            ////////        //if(bSIn != -1)
            ////////        //{
            ////////        //    newSec.Add(antBI);
            ////////        //}

            ////////        if(antBI + bSIn == n)
            ////////        {
            ////////            newSec.Add(n);
            ////////            sumIter += n;
            ////////        }

            ////////        antBI = bSIn;
            ////////        newSec.Add(antBI);
            ////////        sumIter += antBI;
            ////////    }

            ////////    secuencia = newSec;

            ////////    BigInteger resto = -1;
            ////////    BigInteger div = BigInteger.DivRem(sumIter, n, out resto);

            ////////    if (resto == 0)
            ////////    {
            ////////        Console.WriteLine(String.Concat(";n= ", n, "; R > ", div - 1));
            ////////        addTextEscritorio(String.Concat(";n= ", n, "; R > ", div - 1), "divRem");
            ////////        Console.ReadKey();
            ////////    }


            ////////    n++;
            ////////}

            //return;

            ////////// Definir una variable n de tipo int
            ////////BigInteger n = 2;

            ////////// Definir una variable A de tipo double
            ////////BigInteger A = 0;

            ////////BigInteger flag = 100;

            ////////BigInteger iterFlag = 1;

            ////////// Crear un bucle while que se ejecute mientras n sea menor que 100
            ////////while (true)
            ////////{
            ////////    // Calcular el valor de la función usando la función Area
            ////////    A = Area(n);

            ////////    while (A > flag)
            ////////    {
            ////////        flag *= 10;
            ////////        iterFlag *= 10;
            ////////    }

            ////////    BigInteger div = BigInteger.Divide(flag, A);

            ////////    BigInteger r = flag - (A);

            ////////    //BigInteger rem = -1;
            ////////    //BigInteger divR = BigInteger.DivRem(r, div, out rem);
            ////////    //BigInteger exacta = -1;
            ////////    //BigInteger rZ = raizCuadrada(A + aAnt, out exacta);
            ////////    //if (exacta == 0)
            ////////    //if (rem == 0 && BigInteger.Remainder(div, n - 1) == 0)

            ////////    //if ((r + 2) % n == 0)
            ////////    //if ((A + r + (n - 2) + flag) % n == 0)
            ////////    if (true)
            ////////    {
            ////////        //BigInteger resto1 = -1;
            ////////        //BigInteger div1 = BigInteger.DivRem(A + aAnt + 1, n, out resto1);
            ////////        //BigInteger resto2 = -1;
            ////////        //BigInteger div2 = BigInteger.DivRem(div1 + 2, n, out resto2);

            ////////        //if (resto1 != 0 && resto2 != 0)
            ////////        //{

            ////////        Console.WriteLine(String.Concat(";n= ", n, "; A > ", A, "; r > ", r));
            ////////        addTextEscritorio(String.Concat(";n= ", n, "; A > ", A, "; r > ", r), "Bing");
            ////////        Console.ReadKey();
            ////////        //}
            ////////        //aAnt *= 4;
            ////////    }


            ////////    // Incrementar n en 1
            ////////    n++;
            ////////}


            //////// Definir una variable n de tipo int
            //////BigInteger n = 2;

            //////// Crear un bucle while que se ejecute mientras n sea menor que 100
            //////while (true)
            //////{
            //////    BigInteger cMn = (n * n) - n;


            //////    //BigInteger exacta = -1;
            //////    //BigInteger rZ = raizCuadrada(A + aAnt, out exacta);
            //////    //if (exacta == 0)
            //////    //if (rem == 0 && BigInteger.Remainder(div, n - 1) == 0)

            //////    //if ((r + 2) % n == 0)
            //////    //if ((A + r + (n - 2) + flag) % n == 0)

            //////    BigInteger rem = -1;
            //////    BigInteger divR = BigInteger.DivRem(cMn, (4 * (n - 1)), out rem);

            //////    if (rem == 0)
            //////    {


            //////        //BigInteger resto1 = -1;
            //////        //BigInteger div1 = BigInteger.DivRem(A + aAnt + 1, n, out resto1);
            //////        //BigInteger resto2 = -1;
            //////        //BigInteger div2 = BigInteger.DivRem(div1 + 2, n, out resto2);

            //////        //if (resto1 != 0 && resto2 != 0)
            //////        //{

            //////        Console.WriteLine(String.Concat(";n= ", n, "; cMn > ", cMn, "; d > ", divR));
            //////        addTextEscritorio(String.Concat(";n= ", n, "; cMn > ", cMn, "; d > ", divR), "Bing");
            //////        Console.ReadKey();
            //////        //}
            //////        //aAnt *= 4;
            //////    }


            //////    // Incrementar n en 1
            //////    n++;
            //////}

            ////// Definir una variable n de tipo int
            ////BigInteger n = 3;

            ////// Definir una variable k de tipo int
            ////Int32 k = 2;

            ////// Definir una variable V de tipo int
            ////BigInteger V = 0;

            ////// Definir una variable contador de tipo int
            ////BigInteger contador = 0;

            ////BigInteger antR = 0;

            ////// Crear un bucle while que se ejecute mientras n sea menor que 100
            ////while (n < 200)
            ////{
            ////    // Calcular el valor de la función usando la función Vertices
            ////    V = Vertices(n, k);

            ////    antR = (((V / k) + 1) / n) + antR;

            ////    // Mostrar el resultado por la consola
            ////    Console.WriteLine(String.Concat(";n= ", n, "; k= ", k, "; V > ", V, " R: ", antR));
            ////    addTextEscritorio(String.Concat(";n= ", n, "; k= ", k, "; V > ", V, " R: ", antR), "Bing");
            ////    Console.ReadKey();

            ////    k++;
            ////    // Incrementar n en 1
            ////    n++;
            ////}

            ////// Definir una variable n de tipo int
            ////BigInteger n = 4;

            ////// Definir una variable T de tipo int
            ////BigInteger T = 0;


            ////BigInteger tAnt = 0;
            ////// Crear un bucle while que se ejecute mientras n sea menor que 100
            ////while (n < 100)
            ////{
            ////    // Calcular el valor de la función usando la función Triangulos
            ////    T = Triangulos(n);

            ////    BigInteger rem = -1;
            ////    BigInteger div = BigInteger.DivRem(T, n, out rem);

            ////    // Mostrar el resultado por la consola
            ////    Console.WriteLine(String.Concat(";n= ", n, "; T > ", T));
            ////    addTextEscritorio(String.Concat(";n= ", n, "; T > ", T), "Bing");
            ////    Console.ReadKey();


            ////    if (rem == 0)
            ////    {
            ////        if (BigInteger.Remainder(n - 1, div - 1) == 0)
            ////        {

            ////        }
            ////    }

            ////    tAnt = T;

            ////    // Incrementar n en 1
            ////    n++;
            ////}


            ////while (true)
            ////{

            ////    String nS = n.ToString();
            ////    BigInteger count = 0;
            ////    foreach(Char cN in nS)
            ////    {
            ////        count += BigInteger.Parse(cN.ToString());
            ////    }






            ////    if (isP)
            ////    {
            ////        Console.WriteLine(String.Concat(";", r, "> ", b2S, " fluc> ", fluc));
            ////        addTextEscritorio(String.Concat(";", r, "> ", b2S, " fluc> ", fluc), "xD");
            ////    }
            ////    n++;
            ////}









            //return;

            //BigInteger nA = 11;
            //BigInteger iA = 6;
            //BigInteger fluc = 0;
            //while (true)
            //{
            //    String nAS = nA.ToString();
            //    String b2S = FromBase10(nAS, 2);


            //    BigInteger nAt = nA;
            //    Int32 n6 = 0;
            //    BigInteger exacta = BigInteger.Remainder(nAt, 3);

            //    while (exacta != 0)
            //    {
            //        n6++;
            //        nAt--;
            //        exacta = BigInteger.Remainder(nAt, 3);                    
            //    }

            //    String sT = ConvertToBinaryS(n6).Replace('\0', ' ').Trim().PadLeft(2, '0');


            //    String nAtS = String.Concat(sT,"",FromBase10((nAt/ 3).ToString(), 2));

            //    if (b2S == nAtS)
            //    {
            //        BigInteger t = nA;

            //        t++;

            //        BigInteger t2 = t - iA;
            //        BigInteger r = t - t2;
            //        r++;


            //        Boolean fluking = false;

            //        //for (Int32 aaa = 0; aaa < b2S.Length - 2; aaa++)
            //        //{
            //        if(BigInteger.Remainder(r, (b2S.Length - 2) + fluc) == 0 || BigInteger.Remainder(r, (b2S.Length - 2) - fluc) == 0)
            //        {

            //            fluking = true;
            //            fluc++;
            //        }
            //        //}
            //        if (!fluking)
            //        {
            //            Console.WriteLine(String.Concat(";", r, "> ", b2S, " fluc> ", fluc));
            //            addTextEscritorio(String.Concat(";", r, "> ", b2S, " fluc> ", fluc), "nAS");
            //            Console.ReadKey();
            //        }
            //        else
            //        {
            //            Console.WriteLine(String.Concat("X;", r, "> ", b2S, " fluc> ", fluc));
            //            addTextEscritorio(String.Concat("X;", r, "> ", b2S, " fluc> ", fluc), "nAS");
            //            Console.ReadKey();
            //        }
            //        //Console.WriteLine(String.Concat(";", nAS, "> ", b2S, " > ", nAtS));
            //        //addTextEscritorio(String.Concat(";", nAS, "> ", b2S, " > ", nAtS), "nAS");
            //        //Console.ReadKey();
            //        iA += 6;
            //    }

            //    nA++;

            //}


            //return;

            if (true)
            {
                

                return;
            }
            String b16 = "cd23dde000000000000000000000000000000000000000000000000000000000000001600000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000100000000000000000000000003ec388fb9aef6442c7372db3c6b7eed93469c0b00000000000000000000000000000000000000000000f4becbf58b027a640000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"; // c546865204d6f6e6f6c6974680000000000000000000000000000000000000000";

            String b10 = ToBase10(b16, 16);
            //String b2 = FromBase10(b10, 2);
            addTextEscritorioNM(String.Concat(";", b10), "bMonK");

            List<Byte> bbb = new List<byte>();
            //for (Int32 sI = 0; sI < b2.Length; sI += 8)
            //{
            //    String s8 = sI + 8 > b2.Length ? b2.Substring(sI).PadLeft(8, '0') : b2.Substring(sI, 8).PadLeft(8, '0');                
            //    bbb.Add(Convert.ToByte(s8, 2));
            //}

            //ByteArrayToFile(bbb.ToArray());

            for (Int32 bI = 2; bI < 32; bI++)
            {
                String bX = FromBase10(b10, bI);
                addTextEscritorioNM("", "bMonK");
                addTextEscritorioNM(";B " + bI.ToString(), "bMonK");
                
                addTextEscritorioNM(String.Concat(";", bX), "bMonK");
                //bX = FromBase10(b10, 2);
                for (Int32 addon = 3; addon < 33; addon++)
                {
                    String UTF8String = String.Empty;
                    String ASCCII = String.Empty;
                    String Unicode = String.Empty;
                    String UTF32String = String.Empty;
                    String UTF7String = String.Empty;
                    String IntToB64 = String.Empty;

                    List<Byte> bAL = new List<byte>();
                    String s001 = String.Empty;
                    for (Int32 iter = 0; iter < bX.Length; iter += addon)
                    {
                        String s0 = iter + addon > bX.Length ? bX.Substring(iter) : bX.Substring(iter, addon);
                        String s = FromBase10(ToBase10(s0, bI),2);
                        
                        for (Int32 sI = 0; sI < s.Length; sI += 8)
                        {
                            String s8 = sI + 8 > s.Length ? s.Substring(sI).PadLeft(8, '0') : s.Substring(sI, 8).PadLeft(8, '0');
                            //s8.Reverse();
                            s001 += s8;
                            bAL.Add(Convert.ToByte(s8, 2));
                            bbb.Add(Convert.ToByte(s8, 2));
                        }
                    }

                    Byte[] bA = bAL.ToArray();
                    
                    try
                    {
                        UTF8String += UTF8Encoding.UTF8.GetString(bA);
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        ASCCII += UTF8Encoding.ASCII.GetString(bA);
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        Unicode += UTF8Encoding.Unicode.GetString(bA);
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        UTF32String += UTF8Encoding.UTF32.GetString(bA);
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        UTF7String += UTF8Encoding.UTF7.GetString(bA);
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        //IntToB64 += Convert.ToBase64String(bA);
                    }
                    catch (Exception ex)
                    {

                    }


                    //String s01LE = String.Empty;
                    //String s01BE = String.Empty;

                    //foreach(Byte b in bA)
                    //{
                    //    s01LE += ByteToString01LE(b);
                    //    s01BE += ByteToString01BE(b);
                    //}

                    //addTextEscritorioNM("LE " + addon, "bMonK");
                    //addTextEscritorioNM(s01LE, "bMonK");

                    //addTextEscritorioNM("BE " + addon, "bMonK");
                    //addTextEscritorioNM(s01BE, "bMonK");

                    //String s002 = s001;
                    //double sqrt = Math.Sqrt(s001.Length);

                    //Int32 sqrtInt32 = Convert.ToInt32(sqrt);

                    //Int32 width = sqrtInt32;
                    //Int32 height = s001.Length / sqrtInt32;
                    //Int32 wInit = Convert.ToInt32(sqrt);
                    
                    ////for (Int32 w = wInit; w < wInit + (100); w++)
                    ////{
                        
                    //    width = width;
                    //    height = s001.Length / width;
                        
                    //    while (width * height != s001.Length)
                    //    {
                    //        s001 = s001.Remove(s001.Length - 1);                            
                    //        sqrt = width;
                    //        sqrtInt32 = Convert.ToInt32(sqrt);
                    //        width = sqrtInt32;
                    //        height = s001.Length / sqrtInt32;

                    //        //throw new FormatException("Size does not match");
                    //    }

                    //    Bitmap bmp = new Bitmap(width, height);

                    //    for (int r = 0; r < height; r++)
                    //    {
                    //        for (int c = 0; c < width; c++)
                    //        {
                    //            String value = s001[r * width + c].ToString();
                    //            bmp.SetPixel(c, r, value == "0" ? Color.Black : value == "1" ? Color.White : Color.Red);
                    //        }
                    //    }

                    //    bmp.Save(@"c:\logs\imgs\imgB" + bI + "_Add" + addon + "_wh" + width +"x"+ height+".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    //    s001 = s002;

                    //}

                    //for (Int32 w = wInit - 1; w > wInit - (100); w--)
                    //{

                    //    width = w;
                    //    height = s001.Length / width;

                    //    while (width * height != s001.Length)
                    //    {
                    //        s001 = s001.Remove(s001.Length - 1);
                    //        sqrt = w;
                    //        sqrtInt32 = Convert.ToInt32(sqrt);
                    //        width = sqrtInt32;
                    //        height = s001.Length / sqrtInt32;

                    //        //throw new FormatException("Size does not match");
                    //    }

                    //    Bitmap bmp = new Bitmap(width, height);

                    //    for (int r = 0; r < height; r++)
                    //    {
                    //        for (int c = 0; c < width; c++)
                    //        {
                    //            String value = s001[r * width + c].ToString();
                    //            bmp.SetPixel(c, r, value == "0" ? Color.Black : value == "1" ? Color.White : Color.Red);
                    //        }
                    //    }

                    //    bmp.Save(@"c:\logs\imgs\imgB" + bI + "_Add" + addon + "_wh" + w + "x" + height + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    //    s001 = s002;

                    //}

                    //////double sqrt = Math.Sqrt(bA.Length);

                    //////Int32 sqrtInt32 = Convert.ToInt32(sqrt);

                    //////Int32 width = sqrtInt32;
                    //////Int32 height = bA.Length / sqrtInt32;

                    //////while (width * height != bA.Length)
                    //////{
                    //////    bAL.RemoveAt(bAL.Count - 1);
                    //////    bA = bAL.ToArray();
                    //////    sqrt = Math.Sqrt(bA.Length);
                    //////    sqrtInt32 = Convert.ToInt32(sqrt);
                    //////    width = sqrtInt32;
                    //////    height = bA.Length / sqrtInt32;
                    //////    //throw new FormatException("Size does not match");
                    //////}

                    //////Bitmap bmp = new Bitmap(width, height);

                    //////for (int r = 0; r < height; r++)
                    //////{
                    //////    for (int c = 0; c < width; c++)
                    //////    {
                    //////        Byte value = bA[r * width + c];
                    //////        bmp.SetPixel(c, r, Color.FromArgb(value, value, value));
                    //////    }
                    //////}

                    //////bmp.Save(@"c:\logs\imgs\img"+bI+"_"+addon+".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                    addTextEscritorioNM(String.Concat(";s001 " + addon + ": ", s001), "bMonK");
                    //addTextEscritorioNM(String.Concat(";UTF8 " + addon + ": ", UTF8String), "bMonK");
                    //addTextEscritorioNM(String.Concat(";ASCCI " + addon + ": ", ASCCII), "bMonK");
                    //addTextEscritorioNM(String.Concat(";Unicode " + addon + ": ", Unicode), "bMonK");
                    //addTextEscritorioNM(String.Concat(";UTF32 " + addon + ": ", UTF32String), "bMonK");
                    //addTextEscritorioNM(String.Concat(";UTF7 " + addon + ": ", UTF7String), "bMonK");
                    //addTextEscritorioNM(String.Concat(";B64 " + addon + ": ", IntToB64), "bMonK");



                    //////WAV INI

                    ////// Crea un array de bytes que representa los datos de audio en formato PCM de 24 bits
                    ////byte[] audioData24Bit = bA; // Aquí deberías inicializar el array de bytes con los datos de audio

                    ////// Define las propiedades del archivo WAV, como la frecuencia de muestreo, el tamaño de muestra y el número de canales
                    ////int sampleRate = 128; // Frecuencia de muestreo
                    ////short bitsPerSample = 16; // Tamaño de muestra en bits
                    ////short channels = 1; // Mono

                    ////// Crea un archivo de salida con la extensión .wav
                    ////string outputFilePath = "c:\\logs\\wavs\\wavB"+bI+"Add"+addon+".wav";

                    ////using (BinaryWriter writer = new BinaryWriter(File.Create(outputFilePath)))
                    ////{
                    ////    // Escribe la cabecera del archivo WAV
                    ////    writer.Write("RIFF".ToCharArray()); // Identificador del archivo
                    ////    writer.Write(36 + audioData24Bit.Length); // Tamaño del archivo en bytes
                    ////    writer.Write("WAVE".ToCharArray()); // Formato del archivo

                    ////    // Escribe el chunk "fmt "
                    ////    writer.Write("fmt ".ToCharArray()); // Identificador del chunk
                    ////    writer.Write((int)16); // Tamaño del chunk (16 para PCM)
                    ////    writer.Write((short)1); // Formato de codificación (1 para PCM)
                    ////    writer.Write(channels); // Número de canales
                    ////    writer.Write(sampleRate); // Frecuencia de muestreo
                    ////    writer.Write(sampleRate * channels * bitsPerSample / 8); // Tasa de bits
                    ////    writer.Write((short)(channels * bitsPerSample / 8)); // Tamaño de bloque
                    ////    writer.Write(bitsPerSample); // Tamaño de muestra en bits

                    ////    // Escribe el chunk "data"
                    ////    writer.Write("data".ToCharArray()); // Identificador del chunk
                    ////    writer.Write(audioData24Bit.Length); // Tamaño del chunk de datos
                    ////    writer.Write(audioData24Bit); // Datos de audio
                    ////}

                    ////// WAV END

                }

            }
            ByteArrayToFile(bbb.ToArray());
            ////string filePath = "C:\\Users\\apzyx\\Videos\\Def011.rar";
            //string filePath = "C:\\Users\\apzyx\\Videos\\aaa.rar";

            //Int32 n = 1;
            //while (true)
            //{
            //    var items = new List<String> { "a","b","c","d","e","f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r",
            //    "s","t","u","v","w","x","y","z","0","1","2","3","4","5","6","7","8","9"};
            //    var list = GetPermutationsWithRept(items, n);

            //    foreach (var p in list)
            //    {
            //        String password = string.Join("", p);

            //        bool isPasswordValid = ValidatePassword(filePath, password);

            //        if (isPasswordValid)
            //        {
            //            //ExtractRAR(filePath, destinationPath, password);
            //            Console.WriteLine("La contraseña es " + password);
            //            Console.ReadKey();
            //        }
            //        else
            //        {
            //            Console.WriteLine(password);
            //        }
            //    }

            //    n++;
            //}

            //////sM5();
            return;

            //////String hexS = "cd23dde000000000000000000000000000000000000000000000000000000000000001600000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000100000000000000000000000003ec388fb9aef6442c7372db3c6b7eed93469c0b00000000000000000000000000000000000000000000f4becbf58b027a640000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000c546865204d6f6e6f6c6974680000000000000000000000000000000000000000";

            //////Byte[] bS = Hex.Decode(hexS);

            ////////BigInteger bI = new BigInteger(bS);
            //////String decimalS = ToBase10(hexS, 16);

            ////////File.WriteAllText("c:\\logs\\encodings\\enc" + "BigInteger".ToString().PadLeft(5, '0') + ".txt", decimalS.ToString());


            //////String binary = FromBase10(decimalS, 2);
            ////////File.WriteAllText("c:\\logs\\encodings\\enc" + "BiGintegerBinary".ToString().PadLeft(5, '0') + ".txt", binary);





            //////List<Byte> bytes = new List<byte>();
            //////String bitsString = String.Empty;
            //////for(Int32 i = 0; (i * 8) + 8 < binary.Length; i++)
            //////{
            //////    Byte bS2 = Convert.ToByte(Convert.ToInt32(binary.Substring(i * 8, 8), 2));
            //////    bytes.Add(bS2);
            //////    //bitsString += Convert.ToString(b, 2).PadLeft(8, '0');

            //////    //byte byte1 = Convert.ToByte(bitsString.Substring(0, 2), 2);
            //////    //byte byte2 = Convert.ToByte(bitsString.Substring(2, 2), 2);
            //////    //byte byte3 = Convert.ToByte(bitsString.Substring(4, 2), 2);
            //////    //byte byte4 = Convert.ToByte(bitsString.Substring(6, 2), 2);

            //////    //bytes.Add(byte1);
            //////    //bytes.Add(byte2);
            //////    //bytes.Add(byte3);
            //////    //bytes.Add(byte4);


            //////}

            ////////File.WriteAllText("c:\\logs\\encodings\\enc" + "BITS".ToString().PadLeft(5, '0') + ".txt", bitsString);

            //////for (Int32 i = 1; i < 65002; i++)
            //////{
            //////    try
            //////    {
            //////        String ecodingS = Encoding.GetEncoding(i).GetString(bytes.ToArray());
            //////        File.WriteAllText("c:\\logs\\encodings\\enc" + i.ToString().PadLeft(5, '0') + ".txt", ecodingS);

            //////    }
            //////    catch(Exception ex)
            //////    {

            //////    }
            //////}



            //////return;



            // Cargar archivo binario de datos de audio
            //Byte[] WHATISTHEQUESTData = File.ReadAllBytes("c:\\logs\\thebullandbear.pdf");

            //List<Byte> data = new List<Byte>();
            //Boolean copiToDataStream = false;
            //Int32 itxt = 1;
            //String sLIne = String.Empty;
            //for (Int32 i = 0; i < WHATISTHEQUESTData.Length; i++)
            //{
            //    sLIne += Encoding.UTF8.GetString(new Byte[] { WHATISTHEQUESTData[i] });

            //    if (sLIne.Contains("endstream"))
            //    {
            //        sLIne = String.Empty;
            //        //Byte[] dataArray = data.ToArray();

            //        List<Byte> bytesFinal = new List<Byte>();
            //        String streamNameFianl = "aertsdne";
            //        Boolean removing = false;
            //        for (Int32 removeI = data.Count - 1; removeI >= 0; removeI--)
            //        {
            //            foreach (char cEestream in streamNameFianl)
            //            {
            //                if (cEestream.ToString() == Encoding.UTF8.GetString(new byte[] { data[removeI] }))
            //                {
            //                    streamNameFianl = streamNameFianl.Remove(0, 1);
            //                    removing = true;
            //                    break;
            //                }
            //            }

            //            if (!removing && (Encoding.UTF8.GetString(new byte[] { data[removeI] }) != "\r" &&
            //                Encoding.UTF8.GetString(new byte[] { data[removeI] }) != "\n"))
            //            {
            //                bytesFinal.Add(data[removeI]);
            //            }
            //            else
            //            {
            //                String hola = "hola";
            //            }

            //            removing = false;
            //        }

            //        bytesFinal.Reverse();
            //        Byte[] dataArray = bytesFinal.ToArray();
            //        Byte[] dataArray2 = new byte[dataArray.Length - 2];

            //        Int32 offset = 0;
            //        foreach (Byte b in dataArray)
            //        {
            //            if (offset > 1)
            //            {
            //                dataArray2[offset - 2] = b;
            //            }

            //            offset++;
            //        }

            //        //File.WriteAllText("c:\\logs\\PARSEpfd\\_B64" + itxt.ToString() + "TEXT.txt", Convert.ToBase64String(dataArray));

            //        byte[] compressedData = dataArray2; // aquí colocas los datos comprimidos
            //                                            // Crea un stream de memoria para los datos comprimidos
            //        using (var compressedStream = new MemoryStream(compressedData))
            //        {
            //            // Crea un stream de descompresión utilizando GZipStream
            //            using (var decompressionStream = new Ionic.Zlib.DeflateStream(compressedStream, Ionic.Zlib.CompressionMode.Decompress))
            //            {
            //                // Crea un stream de memoria para los datos descomprimidos
            //                using (var decompressedStream = new MemoryStream())
            //                {
            //                    try
            //                    {
            //                        // Lee los datos descomprimidos del stream de descompresión y los guarda en el stream de memoria
            //                        decompressionStream.CopyTo(decompressedStream);

            //                        // Convierte los datos descomprimidos a un array de bytes
            //                        byte[] decompressedData = decompressedStream.ToArray();

            //                        // Utiliza los datos descomprimidos
            //                        // ...

            //                        File.WriteAllBytes("c:\\logs\\Monolith\\_FLATE" + itxt.ToString() + "TEXT.txt", decompressedData);
            //                        ////// Define las propiedades del archivo WAV, como la frecuencia de muestreo, el tamaño de muestra y el número de canales
            //                        ////int sampleRate = 15; // Frecuencia de muestreo
            //                        ////short bitsPerSample = 8; // Tamaño de muestra en bits
            //                        ////short channels = 1; // Mono

            //                        ////// Crea un archivo de salida con la extensión .wav
            //                        ////string outputFilePath = "c:\\logs\\Monolith\\_FLATE" + itxt.ToString() + "SOUND.wav";

            //                        ////using (BinaryWriter writer = new BinaryWriter(File.Create(outputFilePath)))
            //                        ////{
            //                        ////    // Escribe la cabecera del archivo WAV
            //                        ////    writer.Write("RIFF".ToCharArray()); // Identificador del archivo
            //                        ////    writer.Write(36 + decompressedData.Length); // Tamaño del archivo en bytes
            //                        ////    writer.Write("WAVE".ToCharArray()); // Formato del archivo

            //                        ////    // Escribe el chunk "fmt "
            //                        ////    writer.Write("fmt ".ToCharArray()); // Identificador del chunk
            //                        ////    writer.Write((int)16); // Tamaño del chunk (16 para PCM)
            //                        ////    writer.Write((short)1); // Formato de codificación (1 para PCM)
            //                        ////    writer.Write(channels); // Número de canales
            //                        ////    writer.Write(sampleRate); // Frecuencia de muestreo
            //                        ////    writer.Write(sampleRate * channels * bitsPerSample / 8); // Tasa de bits
            //                        ////    writer.Write((short)(channels * bitsPerSample / 8)); // Tamaño de bloque
            //                        ////    writer.Write(bitsPerSample); // Tamaño de muestra en bits

            //                        ////    // Escribe el chunk "data"
            //                        ////    writer.Write("data".ToCharArray()); // Identificador del chunk
            //                        ////    writer.Write(decompressedData.Length); // Tamaño del chunk de datos
            //                        ////    writer.Write(decompressedData); // Datos de audio
            //                        ////}
            //                    }
            //                    catch (Exception e)
            //                    {

            //                    }
            //                }
            //            }
            //        }





            //        //File.WriteAllText("c:\\logs\\PARSEpfd\\" + itxt.ToString() +"TEXT.txt", Encoding.GetEncoding(10000).GetString(dataArray));
            //        itxt++;
            //        copiToDataStream = false;
            //    }
            //    else if (sLIne.Contains("stream"))
            //    {
            //        sLIne = String.Empty;
            //        data = new List<Byte>();
            //        copiToDataStream = true;
            //    }
            //    else
            //    {
            //        if (copiToDataStream)
            //        {
            //            data.Add(WHATISTHEQUESTData[i]);

            //        }
            //    }


            //}

            return;





            //    int width = 192; // Ancho de la imagen
            //int height = 160; // Alto de la imagen
            //int fps = 30; // Cuadros por segundo
            //int numFrames = 9; // Número total de cuadros

            //// Crear el archivo binario y escribir la cabecera
            //FileStream fs = new FileStream("c:\\logs\\video.bin", FileMode.Create);
            //BinaryWriter writer = new BinaryWriter(fs);
            //writer.Write(width);
            //writer.Write(height);
            //writer.Write(fps);
            //writer.Write(numFrames);

            //// Crear cada cuadro y escribirlo en el archivo
            //for (int i = 0; i < numFrames; i++)
            //{

            //    writer.Write(WHATISTHEQUESTData); // Escribir los datos en el archivo
            //}

            //// Cerrar el archivo
            //writer.Close();
            //fs.Close();

            //return;



            // Crear instancia de la clase Pokey
            //Pokey pokey = new Pokey();

            //// Procesar los datos de audio
            //pokey.ProcessFrame(audioData, 0, audioData.Length);

            //// Guardar los datos de audio como un archivo WAV
            //SaveAsWav("c:\\logs\\audio1790.wav", 1790, 8, audioData);
            //// Crea un array de bytes que representa los datos de audio en formato PCM de 8 bits
            //byte[] audioData8Bit = File.ReadAllBytes("c:\\logs\\WhatIsTheQuest.pdf");// Aquí deberías inicializar el array de bytes con los datos de audio

            //// Define las propiedades del archivo WAV, como la frecuencia de muestreo, el tamaño de muestra y el número de canales
            //int sampleRate = 15000; // Frecuencia de muestreo
            //short bitsPerSample = 8; // Tamaño de muestra en bits
            //short channels = 3; // 3 canales

            //// Crea un archivo de salida con la extensión .wav
            //string outputFilePath = "c:\\logs\\output8bits3C15000.wav";

            //using (BinaryWriter writer = new BinaryWriter(File.Create(outputFilePath)))
            //{
            //    // Escribe la cabecera del archivo WAV
            //    writer.Write("RIFF".ToCharArray()); // Identificador del archivo
            //    writer.Write(36 + audioData8Bit.Length); // Tamaño del archivo en bytes
            //    writer.Write("WAVE".ToCharArray()); // Formato del archivo

            //    // Escribe el chunk "fmt "
            //    writer.Write("fmt ".ToCharArray()); // Identificador del chunk
            //    writer.Write((int)16); // Tamaño del chunk (16 para PCM)
            //    writer.Write((short)1); // Formato de codificación (1 para PCM)
            //    writer.Write(channels); // Número de canales
            //    writer.Write(sampleRate); // Frecuencia de muestreo
            //    writer.Write(sampleRate * channels * bitsPerSample / 8); // Tasa de bits
            //    writer.Write((short)(channels * bitsPerSample / 8)); // Tamaño de bloque
            //    writer.Write(bitsPerSample); // Tamaño de muestra en bits

            //    // Escribe el chunk "data"
            //    writer.Write("data".ToCharArray()); // Identificador del chunk
            //    writer.Write(audioData8Bit.Length); // Tamaño del chunk de datos
            //    writer.Write(audioData8Bit); // Datos de audio
            //}

            //return;

            //// Crea un array de bytes que representa los datos de audio en formato PCM de 24 bits
            //byte[] audioData24Bit = File.ReadAllBytes("c:\\logs\\WhatIsTheQuest.pdf"); // Aquí deberías inicializar el array de bytes con los datos de audio

            //// Define las propiedades del archivo WAV, como la frecuencia de muestreo, el tamaño de muestra y el número de canales
            //int sampleRate = 1790; // Frecuencia de muestreo
            //short bitsPerSample = 24; // Tamaño de muestra en bits
            //short channels = 1; // Mono

            //// Crea un archivo de salida con la extensión .wav
            //string outputFilePath = "c:\\logs\\output24bits1790.wav";

            //using (BinaryWriter writer = new BinaryWriter(File.Create(outputFilePath)))
            //{
            //    // Escribe la cabecera del archivo WAV
            //    writer.Write("RIFF".ToCharArray()); // Identificador del archivo
            //    writer.Write(36 + audioData24Bit.Length); // Tamaño del archivo en bytes
            //    writer.Write("WAVE".ToCharArray()); // Formato del archivo

            //    // Escribe el chunk "fmt "
            //    writer.Write("fmt ".ToCharArray()); // Identificador del chunk
            //    writer.Write((int)16); // Tamaño del chunk (16 para PCM)
            //    writer.Write((short)1); // Formato de codificación (1 para PCM)
            //    writer.Write(channels); // Número de canales
            //    writer.Write(sampleRate); // Frecuencia de muestreo
            //    writer.Write(sampleRate * channels * bitsPerSample / 8); // Tasa de bits
            //    writer.Write((short)(channels * bitsPerSample / 8)); // Tamaño de bloque
            //    writer.Write(bitsPerSample); // Tamaño de muestra en bits

            //    // Escribe el chunk "data"
            //    writer.Write("data".ToCharArray()); // Identificador del chunk
            //    writer.Write(audioData24Bit.Length); // Tamaño del chunk de datos
            //    writer.Write(audioData24Bit); // Datos de audio
            //}

            return;

            //List<Chunk> chunks = GetChunksFromPng(File.ReadAllBytes("c:\\logs\\0.png"));
            //CreatePngsFromChunks(chunks);

            return;

            
                                           

            //mults();
            //superBases();
            //yatebo();
            
            //creSUM212();
            //priNormalTiempo();
            //aLaDesesperada();
            //sumatorio();
            //iSumMultAdd();
            //priABCD();
            //tPasDisc2();
            //negativos();
            //numerosSSSPascal();
            ///sucesionCreciente();
            //primosNuevaEra();
            //primosSumatorioAlterado();
            //primosDel4();
            //primosCausalidad();
            //primosSucesivos();
            //primos46();
            //primosSoberanos();
            //CreadorTriangulo();
            //CreadorDeFormulas2();
            //StringBuilder s = 
            //priXYZ();
            //StringBuilder s = pri35();
            //pri322evo();
            //StringBuilder s = pri();
            //StringBuilder s = priFactorialFilas();
            //StringBuilder s = priPotencia();
            //StringBuilder s = priTetraedrico();
            //StringBuilder s = priMitad1();
            //StringBuilder s = priDivision2();
            //StringBuilder s = priCribaEratostenesBigInteger();
            //StringBuilder s = priSucesionErratico();
            //StringBuilder s = priSucesionErraticoMenos2();
            //StringBuilder s = priSucesionErratico2Iter();
            //StringBuilder s = priCribaEratostenes();
            //StringBuilder s = priSucesion();
            //StringBuilder s = 
            //priNormalRestaura();

            //using (StreamWriter writer = new StreamWriter(@"C:\Users\apzyx\OneDrive\Escritorio\a.txt"))
            //{
            //    writer.WriteLine(s.ToString());
            //}


            //StringBuilder stb = new StringBuilder();
            //Char[] digitosBase = new Char[2] { '0', '1' };
            ////Char[] digitosBase = new Char[6] { '0', '1', '2', '3', '4', '5' };
            //for (BigInteger i = 0; i <= 1000; i++)
            //{
            //    Boolean esPrimo = esPrimoNormal(i);
            //    String numeroBase = BaseX(digitosBase, i);
            //    numeroBase = esPrimo ? "<" + numeroBase + ">" : numeroBase;
            //    numeroBase = esPrimo ? numeroBase.PadLeft(10, ' ') : numeroBase.PadLeft(9, ' ').PadRight(10, ' ');
            //    if (i % digitosBase.Length == 0 && i != 0)
            //    {
            //        stb.AppendLine();
            //        stb.Append(numeroBase);
            //    }
            //    else
            //    {
            //        stb.Append(numeroBase);
            //    }
            //}
            //addTextEscritorio(stb.ToString(), "Base2");
            //esPrimoBase();
            //String resultado = SumBaseX(digitosBase, "155", "11");
            //primosD();        
        }

        /// <summary>
        /// Little Endian
        /// </summary>        
        static String ByteToString01LE(byte value)
        {
            var values = String.Empty; // new bool[8];

            values += (value & 1) == 0 ? "0" : "1";
            values += (value & 2) == 0 ? "0" : "1";
            values += (value & 4) == 0 ? "0" : "1";
            values += (value & 8) == 0 ? "0" : "1";
            values += (value & 16) == 0 ? "0" : "1";
            values += (value & 32) == 0 ? "0" : "1";
            values += (value & 64) == 0 ? "0" : "1";
            values += (value & 128) == 0 ? "0" : "1";

            return values;
        }

        /// <summary>
        /// Big Endian
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static String ByteToString01BE(byte value)
        {
            var values = String.Empty; // new bool[8];

            values += (value & 128) == 0 ? "0" : "1";
            values += (value & 64) == 0 ? "0" : "1";
            values += (value & 32) == 0 ? "0" : "1";
            values += (value & 16) == 0 ? "0" : "1";
            values += (value & 8) == 0 ? "0" : "1";
            values += (value & 4) == 0 ? "0" : "1";
            values += (value & 2) == 0 ? "0" : "1";
            values += (value & 1) == 0 ? "0" : "1";

            return values;
        }

        //BigInteger r3 = BigInteger.Parse("2") * BigInteger.Parse("3") * BigInteger.Parse("4") * BigInteger.Parse("5") *
        //                BigInteger.Parse("3") * BigInteger.Parse("6") * BigInteger.Parse("10") * BigInteger.Parse("15") *
        //                BigInteger.Parse("4") * BigInteger.Parse("10") * BigInteger.Parse("20") * BigInteger.Parse("35") *
        //                BigInteger.Parse("5") * BigInteger.Parse("15") * BigInteger.Parse("35") * BigInteger.Parse("70");

        ////r3 *= 3456000;
        //r3 += 3000;
        //r3 += 3000;
        //r3 += 108;


        //BigInteger r4 = BigInteger.Parse("2") * BigInteger.Parse("3") * BigInteger.Parse("4") * BigInteger.Parse("5") * BigInteger.Parse("6") * //BigInteger.Parse("36") *
        //                BigInteger.Parse("3") * BigInteger.Parse("6") * BigInteger.Parse("10") * BigInteger.Parse("15") * BigInteger.Parse("21") * //BigInteger.Parse("120") *
        //                BigInteger.Parse("4") * BigInteger.Parse("10") * BigInteger.Parse("20") * BigInteger.Parse("35") * BigInteger.Parse("56") * //BigInteger.Parse("330") *
        //                BigInteger.Parse("5") * BigInteger.Parse("15") * BigInteger.Parse("35") * BigInteger.Parse("70") * BigInteger.Parse("126") * //BigInteger.Parse("792") *
        //                BigInteger.Parse("6") * BigInteger.Parse("21") * BigInteger.Parse("56") * BigInteger.Parse("126") * BigInteger.Parse("252"); //BigInteger.Parse("1716") *
        //                //BigInteger.Parse("36") * BigInteger.Parse("120") * BigInteger.Parse("330") * BigInteger.Parse("792") * BigInteger.Parse("1716") * BigInteger.Parse("3432");


        //BigInteger resto = -1;
        //BigInteger div = BigInteger.DivRem(r, n + 2, out resto);

        //BigInteger resto2 = -1;
        //BigInteger div2 = BigInteger.DivRem(r, n, out resto2);

        //if (resto != 0 && r % resto != 0 && resto2 != 0 && r % resto2 != 0 && r % (n + 1) != 0)
        //{
        //    r += (n + 2);

        //    if (r % resto != 0 && r % resto2 != 0 && r % 2 != 0)
        //    {

        //    }
        //}
        private static async Task<String> getStringFromByteArray(Byte[] bytes)
        {
            String[] stringArray = new String[bytes.Length];

            List<Task> tasks = new List<Task>();
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                Int32 j = i;

                tasks.Add(Task.Run(() =>
                {
                    String s = Convert.ToString(bytes[j], 2).PadLeft(8, '0');
                    stringArray[j] = s;
                }));
            }

            await Task.WhenAll(tasks);

            return String.Concat(stringArray);
        }


        public static async void CreatePngsFromChunks(List<Chunk> chunks)
        {
            // Buscamos el ancho y alto de la imagen en el chunk IHDR
            int width = BitConverter.ToInt32(chunks[0].Data.Take(4).Reverse().ToArray(), 0);
            int height = BitConverter.ToInt32(chunks[0].Data.Skip(4).Take(4).Reverse().ToArray(), 0);

            // Recorremos los chunks en busca de los chunks de datos (IDAT)
            for (int i = 1; i < chunks.Count; i++)
            {
                if (chunks[i].Type == "IDAT")
                {
                    try
                    {
                        String binary = await getStringFromByteArray(chunks[i].Data);

                        //File.WriteAllBytes("c:\\logs\\_" + i + ".png", chunks[i].Data);

                        addTextEscritorio(binary, "b01_" + i.ToString());
                    }
                    catch(Exception ex)
                    {

                    }
                                        

                    // Creamos un nuevo array de bytes con el chunk IHDR y el chunk de datos actual (IDAT)
                    byte[] pngData = new byte[chunks[0].Data.Length + chunks[i].Data.Length];
                    Array.Copy(chunks[0].Data, pngData, chunks[0].Data.Length);
                    Array.Copy(chunks[i].Data, 0, pngData, chunks[0].Data.Length, chunks[i].Data.Length);

                    // Generamos un nuevo archivo PNG con el chunk de datos actual (IDAT)
                    using (MemoryStream pngStream = new MemoryStream(pngData))
                    {
                        try
                        {
                            using (Bitmap pngBitmap = new Bitmap(pngStream))
                            {
                                pngBitmap.Save($"c:\\logs\\IDAT_{i - 1}.png", ImageFormat.Png);
                            }
                        }
                        catch(Exception ex) { }
                    }
                }
            }
        }

        public class Chunk
        {
            public string Type { get; set; }
            public byte[] Data { get; set; }
            public uint Checksum { get; set; }
        }

        public static List<Chunk> GetChunksFromPng(byte[] pngData)
        {
            List<Chunk> chunks = new List<Chunk>();
            int offset = 8; // Saltamos los 8 bytes del encabezado

            while (offset < pngData.Length)
            {
                int length = (pngData[offset] << 24) + (pngData[offset + 1] << 16) + (pngData[offset + 2] << 8) + pngData[offset + 3];
                string type = Encoding.ASCII.GetString(pngData, offset + 4, 4);
                byte[] data = new byte[length];
                Array.Copy(pngData, offset + 8, data, 0, length);
                uint crc = (uint)((pngData[offset + 4 + length] << 24) + (pngData[offset + 5 + length] << 16) + (pngData[offset + 6 + length] << 8) + pngData[offset + 7 + length]);

                chunks.Add(new Chunk() { Type= type, Data = data, Checksum = crc });

                offset += length + 12; // Saltamos el CRC y los 4 bytes del tamaño del chunk
            }

            return chunks;
        }

        public static List<String> hollidaysgost()
        {
            //using (StreamReader writer = new StreamReader(@"C:\logs\rpo_book.txt"))
            using (StreamReader writer = new StreamReader(@"C:\logs\whatIsTheQuest.txt"))
            {
                var okcharacters = new List<String> { "A","B","C","D","E","F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R",
                "S","T","U","V","W","X","Y","Z", "a","b","c","d","e","f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r",
                "s","t","u","v","w","x","y","z","0","1","2","3","4","5","6","7","8","9"};

                List<String> words = new List<String>();
                String s = writer.ReadLine();
                while (s != null)
                {
                    String sToTrim = s;
                    foreach (Char c in s)
                    {
                        String sC = c.ToString();

                        if (sC != " ")
                        {
                            if (!okcharacters.Contains(sC))
                            {
                                sToTrim = sToTrim.Replace(sC, "");
                            }
                        }
                    }

                    String[] wordsLine = sToTrim.Split(new String[] {" "}, StringSplitOptions.RemoveEmptyEntries);

                    foreach(String w in wordsLine)
                    {
                        if (!words.Contains(w))
                        {
                            Console.WriteLine(w);
                            words.Add(w);
                        }
                    }

                    s = writer.ReadLine();
                }

                Console.WriteLine("Terminando");

                //foreach (String w in words)
                //{
                //    addTextEscritorio(String.Concat(w), "wordsRPO_WITQ");
                //}

                return words;
            }
        }

        private static List<BigInteger> DivisoresPof2(BigInteger numero)
        {
            List<BigInteger> factores = new List<BigInteger>() { 1, numero };

            BigInteger iterador = numero;

            List<Task> tasks = new List<Task>();
            Boolean encontrado = false;
            for (BigInteger i = 2; i < iterador; i*=2)
            {
                BigInteger iI = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {

                    BigInteger resto = -1;
                    BigInteger div = BigInteger.DivRem(iterador, iI, out resto);

                    if (resto == 0)
                    {
                        encontrado = true;
                        if (!factores.Contains(iI))
                        {
                            factores.Add(iI);
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            if (!encontrado)
            {
                if (!factores.Contains(iterador))
                {
                    factores.Add(iterador);
                }
            }

            return factores;

        }

        //private void SQLTest(BigInteger iterador, BigInteger iI)
        //{
            
        //    Task<BigInteger> taskConnect = Task.Factory
        //        .StartNew(() => threadConnect(iterador, iI, true), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
        //        .ContinueWith((task) => threadConnect(task.Result, true));
            
        //}

        //private BigInteger threadConnect(BigInteger iterador,  BigInteger iI, bool bContinuation)
        //{
        //    if (!bContinuation)
        //    {
        //        BigInteger resto = -1;
        //        BigInteger div = BigInteger.DivRem(iterador, iI, out resto);

        //        if (resto == 0)
        //        {
        //            return -1;
        //        }
        //    }
        //    return iterador;
        //}

        //private void threadConnectComplete(object state)
        //{
        //    //Back in the main UI thread!  Save the changes to the sql object
        //    BigInteger sql = ((BigInteger)state);

        //    Console.WriteLine(String.Concat(";", sql - 1));
        //    addTextEscritorio(String.Concat(";", sql - 1), "nM");

        //}

        private static BigInteger DivisoresRa(BigInteger numero)
        {            
            //BigInteger iterador = numero;

            //List<Task> tasks = new List<Task>();
            BigInteger exacta = -1;
            //Boolean sigo = true;
            //BigInteger r2 = -1;
            //Task.Factory.StartNew(() =>
            //{
            //    r2 = raizCuadradaMasUno(iterador, out exacta);

            //});
            BigInteger r2medios = raizCuadradaMasUno(numero, out exacta) / 2;
            BigInteger div00 = 3;
            BigInteger div02 = r2medios % 2 == 0 ? r2medios + 1: r2medios + 2;
            r2medios++;
            for (; div00 < r2medios;)
            {
                //BigInteger iI = i;

                //if (sigo)
                //{
                    //tasks.Add(Task.Factory.StartNew(() =>
                    //{
                //BigInteger resto = -1;
                BigInteger resto00 = BigInteger.Remainder(numero, div00);
                if (resto00 == 0)
                {
                    return div00;                            
                }

                BigInteger resto02 = BigInteger.Remainder(numero, div02);
                if (resto02 == 0)
                {
                    return div02;
                }

                div00 += 2;
                div02 += 2;

                //}));
                //}
                //else
                //{
                //    break;
                //}
            }

            //Task.WaitAll(tasks.ToArray());
            //tasks.Clear();
            return -1;

        }

        private static List<BigInteger> Divisores(BigInteger numero)
        {
            List<BigInteger> factores = new List<BigInteger>() { 1, numero };

            BigInteger iterador = numero;

            List<Task> tasks = new List<Task>();
            Boolean encontrado = false;
            for (BigInteger i = 3; i <= iterador / 3; i+=2)
            {
                BigInteger iI = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {

                    BigInteger resto = -1;
                    BigInteger div = BigInteger.DivRem(iterador, iI, out resto);

                    if (resto == 0)
                    {
                        encontrado = true;
                        if (!factores.Contains(iI))
                        {
                            factores.Add(iI);
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            if (!encontrado)
            {
                if (!factores.Contains(iterador))
                {
                    factores.Add(iterador);
                }
            }

            return factores;

        }


        private static List<BigInteger> FactoresPrimos(BigInteger numero)
        {
            List<BigInteger> factores = new List<BigInteger>();

            BigInteger iterador = numero;

            while (iterador != 1)
            {
                BigInteger exacta = -1;
                Boolean encontrado = false;
                for (BigInteger i = 2; i < iterador; i++)
                {
                    BigInteger resto = -1;
                    BigInteger div = BigInteger.DivRem(iterador, i, out resto);

                    if(resto == 0)
                    {
                        encontrado = true;
                        if (!factores.Contains(i))
                        {
                            factores.Add(i);
                        }
                        iterador = div;
                        break;
                    }

                }

                if(!encontrado)
                {
                    if (!factores.Contains(iterador))
                    {
                        factores.Add(iterador);
                    }
                    break;
                }
            }

            return factores;

        }

        public static void mults()
        {
            Int32 rangoMatrix = 10;

            BigInteger[,] matrix = new BigInteger[rangoMatrix, rangoMatrix];            
            matrix[0, 0] = 1;

            BigInteger i1 = 1;
            BigInteger i2 = 1;
            for (Int32 i = 1; i < rangoMatrix; i+=1)
            {
                BigInteger iM1 = i1;
                matrix[i, 0] = iM1;
                matrix[0, i] = iM1;
                BigInteger tempI = i1 + i2;
                i1 = i2;
                i2 = tempI;
            }

            for(Int32 i = 1;i < rangoMatrix; i++)
            {
                for(Int32 j = 1; j < rangoMatrix; j++)
                {
                    matrix[i, j] = matrix[i, j - 1] * matrix[i - 1, j] * matrix[i - 1, j - 1];

                }
            }

            BigInteger r = 1;
            BigInteger rSum = 0;
            for (Int32 n = 1; n < rangoMatrix; n++)
            {
                
                r *= matrix[n, n];

                for (Int32 i = n - 1; i > 0; i--)
                {
                    r *= matrix[i, n];
                    r *= matrix[n, i];
                }

                rSum *= matrix[n, n];

                for (Int32 i = n - 1; i > 0; i--)
                {
                    rSum *= matrix[i, n];
                    rSum *= matrix[n, i];
                }


                Console.WriteLine(String.Concat(";", (r + rSum).ToString(), " < ", matrix[0,n]));
                addTextEscritorio(String.Concat(";", (r + rSum).ToString(), " < ", matrix[0, n]), "mults");
                Console.ReadKey();

            }
        }

        public static void seguimosIntentandolo()
        {
            BigInteger n = 6;
            List<BigInteger> susodichos = new List<BigInteger>();
            
            while (true)
            {
                BigInteger resto = -1;
                BigInteger div = BigInteger.DivRem(n, 2, out resto);
                while (resto == 0)
                {
                    resto = -1;
                    BigInteger divInt = BigInteger.DivRem(div, 2, out resto);
                    if(resto == 0)
                    {
                        div = divInt;
                    }
                }

                if(div != 1)
                {                    
                    BigInteger r = n + div;

                    if(!susodichos.Contains(r))
                    {
                        susodichos.Add(r);
                    }                                        
                }

                BigInteger nM1 = n - 1;

                if (!susodichos.Contains(nM1))
                {

                    Console.WriteLine(String.Concat(";", (nM1).ToString()));
                    addTextEscritorio(String.Concat(";", (nM1).ToString()), "otroIntento");
                    Console.ReadKey();
                }
                else
                {
                    susodichos.Remove(nM1);
                }

                n +=2;                
            }
        }

        public static void superBases()
        {               
            BigInteger n = 1;
            BigInteger nI = 1;

            //List<String> b0 = new List<String>() { "0", "0" };
            
            while (n < 1000)            
            {
                BigInteger r = 6 * (n + 1) * (n + 2) * (n + 3) / 6;

                //BigInteger resto = -1;
                //BigInteger div = BigInteger.DivRem(r, n + 1, out resto);

                Console.WriteLine(String.Concat(";", (r).ToString()));
                addTextEscritorio(String.Concat(";", (r).ToString()), "chatGPTpri");
                Console.ReadKey();

                //if (resto == 0)
                //{
                //    if (div % (n + 2) == 0)
                //    {
                //        Console.WriteLine(String.Concat(";", (n + 2).ToString()));
                //        addTextEscritorio(String.Concat(";", (n + 2).ToString()), "T");
                //        Console.ReadKey();
                //    }
                //}
                n++;
                nI += 2;
            }
        }

        private static String[] FromBase10ToDefinition(String[] baseDigits, Int32 nBase10)
        {
            List<String> result = new List<String>();
            Int32 iter = nBase10;
            
            while (iter / baseDigits.Length >= baseDigits.Length)
            {
                result.Add(baseDigits[(iter % baseDigits.Length)]);
                iter = iter / baseDigits.Length;
            }

            result.Add(baseDigits[(iter % baseDigits.Length)]);
            result.Add(baseDigits[iter / baseDigits.Length]);
            //result.Reverse();
            return result.ToArray();
        }

        public static void yatebo()
        {
            BigInteger n = 1;
            BigInteger sum = 1;
            Boolean sum1 = true;

            while (true)
            {
                if (sum1)
                {
                    sum += 1;
                    n++;
                }
                else
                {
                    sum += n;

                    if (sum % 2 != 0)
                    {
                        Console.WriteLine(String.Concat(";", (sum).ToString()));
                        addTextEscritorio(String.Concat(";", (sum).ToString()), "yatebo");
                        Console.ReadKey();
                    }

                }
                sum1 = !sum1;
            }
        }
        public static void tarari()
        {
            BigInteger n = 4;
            BigInteger div = 2;
            BigInteger offset = 2;
            while (true)
            {
                BigInteger freezeN = n;

                BigInteger total = 0;
                for (BigInteger i = n; i < freezeN + freezeN; i++)
                {
                    total += i;
                    n++;
                }

                BigInteger resto = -1;
                BigInteger divR = BigInteger.DivRem(total, div, out resto);
                if (resto == 0)
                {
                    if (divR % offset != 0)
                    {
                        Console.WriteLine(String.Concat(";", (divR).ToString()));
                        addTextEscritorio(String.Concat(";", (divR).ToString()), "tarari");
                        Console.ReadKey();
                    }
                }
                else
                {
                    throw new Exception("Catacrash!");
                }

                
                div *= 2;
                offset++;
            }
        }

        public static void tururu()
        {
            List<List<String>> comb = GetCombinationsST(new List<string>() { 
                "0000","0001","0010","0011","0100","0101","0110","0111","1000","1001","1010","1011","1100","1101","1110","1111"
            }, 3);

            BigInteger[] n3 = new BigInteger[3] { 1, 2, 3 };

            Int32 vuelta = 1;
            while (true)
            {
                BigInteger sum = n3[0] + n3[1] + n3[2];
                BigInteger sumMv = sum - vuelta;
                BigInteger vuelta3 = 3 * vuelta;

                Boolean primo = true;

                if (sumMv % vuelta3 == 0)
                {
                    primo = false;
                }
                else if (sumMv % (vuelta3 - sumMv) == 0)
                {
                    primo = false;
                }
                else if (sumMv % ( (vuelta3 - sumMv)) == 0)
                {
                    primo = false;
                }

                if (primo)
                {
                    Console.WriteLine(String.Concat(";", (sumMv).ToString()));
                    addTextEscritorio(String.Concat(";", (sumMv).ToString()), "primo3");
                    Console.ReadKey();
                }

                n3[0] = n3[1];
                n3[1] = n3[2];
                n3[2] = n3[2] + 1;

                vuelta++;
            }
        }

        public static void primsftR()
        {
            BigInteger n = 1;
            BigInteger sum = 0;
            BigInteger n2 = 0;
            Dictionary<BigInteger, NumberP> dicc = new Dictionary<BigInteger, NumberP>();

            while (true)
            {
                sum += n;

                NumberP nP = new NumberP();
                nP.NBalls = n;
                nP.NComanded = n + (n - 1);
                                
                BigInteger r = sum;

                BigInteger cuanty = sum - nP.NComanded;
                while (cuanty > 0)
                {
                    NumberP nLoop = dicc[cuanty];
                    if (nLoop != null)
                    {
                        r += nLoop.R;
                        cuanty = cuanty - nLoop.NComanded;
                    }
                    else
                    {
                        break;
                    }

                }

                nP.R = r;
                dicc.Add(sum, nP);

                Console.WriteLine(String.Concat(";", (r + n2).ToString()));
                addTextEscritorio(String.Concat(";", (r + n2).ToString()), "msftR");
                Console.ReadKey();

                n2 += 2;
                n++;
            }
        }

        public static void prin3()
        {
            BigInteger n = 2;

            BigInteger fact = 1;
            while (true)
            {
                fact *= n;

                BigInteger r = Potencia(n, fact) - (n - 1) + fact;


                //sum += lastPow + 1;

                Console.WriteLine(String.Concat(";", (r).ToString()));
                addTextEscritorio(String.Concat(";", (r).ToString()), "prin");
                Console.ReadKey();

                n++;
            }
        }
        public static void prin2()
        {
            BigInteger n = 3;

            List<List<List<BigInteger>>> tocho = new List<List<List<BigInteger>>>();
            tocho = new List<List<List<BigInteger>>>() { new List<List<BigInteger>>() { new List<BigInteger>() { 1 }, new List<BigInteger>() { 2 } } };

            BigInteger total = 3;
            while (true)
            {
                BigInteger sumLUPUnit = 0;
                Boolean isFirst = true;
                BigInteger sumLUP = 0;                
                Int32 tochoC = tocho.Count;                
                for (Int32 i = 0; i < tochoC; i++)
                {
                    List<List<BigInteger>> ultimaPri = tocho[i];
                    List<BigInteger> newLN = new List<BigInteger>();

                    
                    Int32 ultimUltC = ultimaPri[ultimaPri.Count - 1].Count;
                    for (Int32 j = 0; j < ultimUltC; j++)                        
                    {
                        BigInteger iUP = ultimaPri[ultimaPri.Count - 1][j];
                        BigInteger iter;
                        if (isFirst)
                        {
                            iter = n;
                            sumLUPUnit++;
                        }
                        else
                        {
                            iter = n - 1;
                        }

                        for (Int32 k = 0; k < iUP; k++)
                        {
                            newLN.Add(iter);
                            total += iter;
                            if (isFirst)
                            {
                                sumLUP++;
                            }
                            else
                            {
                                sumLUPUnit++;
                            }
                        }

                        
                    }

                    ultimaPri.Add(newLN);
                    isFirst = false;
                }

                List<List<BigInteger>> newLNEW = new List<List<BigInteger>>();
                List<BigInteger> newLNE = new List<BigInteger>(); 
                for (BigInteger z = 0; z < (sumLUP - sumLUPUnit); z++)
                {
                    total += n - 1;
                    newLNE.Add(n - 1);                                        
                }
                newLNEW.Add(newLNE);

                BigInteger n1m = n - 1;
                isFirst = true;
                while (n1m > 1)
                {
                    List<BigInteger> subNs = new List<BigInteger>();
                    foreach (BigInteger w in newLNEW[0])
                    {
                        BigInteger mult = w;

                        for (BigInteger g = mult; g > 0; g--)
                        {
                            total += w - 1;
                            subNs.Add(w - 1);
                        }
                        
                        if(isFirst)
                        {
                            isFirst = false;
                            break;
                        }

                    }
                    newLNEW.Insert(0, subNs);

                    n1m--;
                }

                tocho.Add(newLNEW);



                //sum += lastPow + 1;

                Console.WriteLine(String.Concat(";", (total).ToString()));
                addTextEscritorio(String.Concat(";", (total).ToString()), "prin2");
                Console.ReadKey();

                
                n++;
            }
        }

        public static void prin()
        {
            BigInteger n = 2;

            BigInteger SUM = 1;
            while (true)
            {
                BigInteger sum = 0;
                BigInteger nSum = 0;
                for (BigInteger i = 1; i <= 5; i++)
                {
                    SUM += n;
                    sum += SUM;
                    nSum = n;
                    n++;
                }
                

                //sum += lastPow + 1;
                
                Console.WriteLine(String.Concat(";", (sum / 5).ToString()));
                addTextEscritorio(String.Concat(";", (sum / 5).ToString()), "prin");
                Console.ReadKey();

                SUM -= nSum;
                n--;
            }
        }

        public static void cosaRara()
        {            
            BigInteger n = 2;

            BigInteger divn = 2;
            BigInteger divS = 3;

            while (true)
            {

                BigInteger tempDivn = n * (n + 1);
                //BigInteger esExacta = -1;
                //BigInteger r2N2 = raizCuadrada((n + 2), out esExacta);
                //BigInteger tempDivS = esExacta == 0 ? r2N2 : (n + 2);
                BigInteger tempDivS = n + 2;

                Boolean nEsPar = n % 2 == 0;
                if (nEsPar)
                {
                    tempDivn /= 2;
                    tempDivS /= 2;
                }

                //BigInteger rDivS = tempDivS % divS == 0 ? tempDivS : divS % tempDivS  == 0 ? divS : divS * tempDivS;
                BigInteger mcd = MaximoComunDivisor(tempDivS, divS);
                BigInteger rDivS = (tempDivS * divS) / mcd;
                BigInteger rDivn = ((rDivS / divS) * divn) + ((rDivS / tempDivS) * tempDivn);

                if (rDivS % (n - 1) == 0 && rDivn % (n - 1) == 0)
                {
                    rDivn /= n - 1;
                    rDivS /= n - 1;
                }

                if (rDivS % n == 0) {
                    Console.WriteLine(String.Concat(n.ToString() + ";", rDivn.ToString()));
                    addTextEscritorio(String.Concat(n.ToString() + ";", rDivn.ToString()), "crecimientoExponencial");
                    Console.ReadKey();
                }
                divn = rDivn;
                divS = rDivS;


                n++;

            }


        }

        private static BigInteger MaximoComunDivisor(BigInteger num1, BigInteger num2)
        {
            BigInteger a = num1 > num2 ? num1 : num2;
            BigInteger b = num1 < num2 ? num1 : num2;
                        
            BigInteger res;

            
            do
            {
                res = b;
                b = a % b;
                a = res;
            } while (b != 0);

            return res;
        }

        public static void crecimientoExponencial()
        {
            List<BigInteger> lista = new List<BigInteger>() { 1, 2 , 1};

            BigInteger n = 3;
            BigInteger total = 4;
            while (true)
            {
                

                List<BigInteger> temp = new List<BigInteger>();

                BigInteger iA = -1;                                
                foreach (BigInteger l in lista)
                {
                    if (iA != -1)
                    {
                        BigInteger sum = iA + l;

                        if (sum == n)
                        {
                            total+=n;
                            temp.Add(n);
                        }
                    }
                    iA = l;
                    temp.Add(l);                    
                }

                BigInteger r = (total * (n - 1)) - 1;

                Console.WriteLine(String.Concat(n.ToString() + ";", r.ToString()));
                addTextEscritorio(String.Concat(n.ToString() + ";", r.ToString()), "crecimientoExponencial");
                Console.ReadKey();

                lista = temp;
                                
                n++;

            }


        }

        public static void Pri11()
        {
            List<BigInteger> tUno12 = new List<BigInteger>() { };
            List<BigInteger> tUno13 = new List<BigInteger>() { 2 };
            List<BigInteger> tUno23 = new List<BigInteger>() { 3 };
            List<BigInteger> tDos12 = new List<BigInteger>() { 3 };
            List<BigInteger> tDos13 = new List<BigInteger>() { 4 };
            List<BigInteger> tDos23 = new List<BigInteger>() { 5 };

            Boolean flip = false;

            BigInteger uno = 3;
            BigInteger dos = 4;
            BigInteger tres = 5;

            BigInteger iter = 0;
            while (true)
            {
                List<List<BigInteger>> checkListas = null;
                if (flip)
                {
                    checkListas = new List<List<BigInteger>>() { tDos12, tDos13, tDos23 };
                }
                else
                {
                    checkListas = new List<List<BigInteger>>() { tUno12, tUno13, tUno23 };
                }
                
                BigInteger n12 = uno + dos;
                BigInteger n13 = uno + tres;
                BigInteger n23 = dos + tres;

                for (Int32 i=0; i < checkListas.Count; i++)
                {
                    BigInteger checkN = i == 0 ? n12: i == 1 ? n13 : n23;
                    Boolean esDivisible = false;
                    foreach (BigInteger n in checkListas[i])
                    {
                        esDivisible = checkN % n == 0;
                        if (esDivisible)
                        {
                            break;
                        }
                    }

                    if (!esDivisible)
                    {
                        checkListas[0].Add(checkN);
                        Console.WriteLine(String.Concat(";", (checkN).ToString()));
                        addTextEscritorio(String.Concat(";", (checkN).ToString()), "Pri11");
                        Console.ReadKey();
                    }
                }

                uno = n12;
                dos = n13;
                tres = n23;

                

                flip = !flip;
                iter++;
            }


        }

        public static void Pri10()
        {
            BigInteger n = 2;

            List<BigInteger> ops = new List<BigInteger>() { 1 };
            
            while (true)
            {
                List<BigInteger> tempOps = new List<BigInteger>() { n };

                BigInteger antOp = ops[0];
                BigInteger tempR = n + antOp;
                tempOps.Add(tempR);
                for (Int32 i = 1; i < ops.Count; i++) 
                {
                    antOp = ops[i];
                    tempR += antOp;
                    tempOps.Add(tempR);
                    
                    
                }

                    BigInteger r = 0;

                    foreach (BigInteger op in tempOps)
                    {
                        r += op;
                    }

                    Console.WriteLine(String.Concat(n.ToString() + " ; ", (r).ToString()));
                    addTextEscritorio(String.Concat(n.ToString() + " ; ", (r).ToString()), "Pri10");
                    Console.ReadKey();
                

                ops = tempOps;
                n++;
            }


        }

        public static void Pri9()
        {
            BigInteger n = 2;

            BigInteger nSum = 1;
            while (true)
            {
                nSum += n;

                BigInteger nCuadrado = n * n;

                BigInteger r = nSum + nCuadrado;


                BigInteger offset = 0;
                if (r % 2 == n % 2)
                {
                    offset = 1;
                }
                else
                {
                    offset = 2;
                }

                BigInteger addon = n;
                BigInteger rTemp = r;
                Boolean sumCifras = false;
                do
                {
                    addon = addon - offset;                    
                    rTemp = r + addon;
                                        
                    offset = 2;

                }
                while (addon != 0 && (rTemp % addon == 0));

                r = rTemp;

                BigInteger totalCifras = 0;
                foreach (Char c in rTemp.ToString())
                {
                    totalCifras += BigInteger.Parse(c.ToString());
                }

                if (rTemp % totalCifras != 0)
                {
                    Console.WriteLine(String.Concat(n.ToString() + " ; ", (r).ToString()));
                    addTextEscritorio(String.Concat(n.ToString() + " ; ", (r).ToString()), "Pri9");
                    Console.ReadKey();
                }

                n++;
            }


        }

        public static void aaa()
        {            
            BigInteger n = 2;
            BigInteger nSumSum = 4;
                        
            while (true)
            {
                BigInteger rAnt = 0;

                Boolean isFirst = true;
                for (BigInteger i = nSumSum; i > 1; i--)
                {
                    rAnt += i * n;
                    //if(isFirst)
                    //{
                    //    isFirst = false;
                    //    rAnt += nSumSum * 1;
                    //}
                    //else
                    //{
                    //    rAnt += 1 * 2;
                    //}
                }

                rAnt += n;

                String sResult = rAnt.ToString();
                Console.WriteLine(String.Concat(";", sResult));
                addTextEscritorio(String.Concat(";", sResult), "aaa");
                    

                n++;
                nSumSum += 3;
            }


        }

        public static void crecimientoNucleicoMULT()
        {
            List<BigInteger> lista = new List<BigInteger>() { 1, 2, 1 };

            BigInteger n = 3;
            BigInteger mult = 2;

            while (true)
            {
                List<BigInteger> temp = new List<BigInteger>();

                BigInteger sumNS = 0;
                BigInteger iA = -1;
                foreach (BigInteger l in lista)
                {
                    if (iA != -1)
                    {
                        BigInteger sum = iA + l;
                        if (sum == n)
                        {
                            sumNS++;
                            temp.Add(sum);
                            mult *= sum;
                        }                        
                    }
                    iA = l;                    
                    temp.Add(l);
                }

                if (sumNS + 1 == n)
                {
                    String sResult = mult.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "crecimientoNucleicoMULT");
                }

                lista = temp;
                n++;

            }


        }

        public static void creciMult()
        {
            BigInteger n = 2;
            BigInteger mult = 1;

            BigInteger temp = 1;
            while (true)
            {
                mult *= Potencia(Potencia(n, n), temp);
                temp *= n;

                BigInteger r = mult + 1;

                String sResult = r.ToString();
                Console.WriteLine(String.Concat(";", sResult));
                addTextEscritorio(String.Concat(";", sResult), "creciMult");

                n++;
            }
        }

        public static void fact3()
        {
            BigInteger n = 2;
            BigInteger mult = 1;

            BigInteger sum = 0;
            while (true)
            {
                Boolean isFirst = true;
                for (BigInteger i = 0; i < n; i++) 
                {
                    mult *= n;
                    if (isFirst)
                    {
                        isFirst = false;
                        BigInteger r = mult + 1;

                        String sResult = r.ToString();
                        Console.WriteLine(String.Concat(";", sResult));
                        addTextEscritorio(String.Concat(";", sResult), "fact3");
                    }
                }

                n++;
            }
        }
        public static void fact2()
        {
            BigInteger n = 3;
            BigInteger mult = 1;

            BigInteger nMenos1 = 2;
            while (true)
            {
                

                BigInteger r = mult + nMenos1;

                if (r % n == 0)
                {
                    String sResult = n.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "fact2");
                }

                mult *= nMenos1;
                nMenos1 = n;
                n++;
            }
        }

        public static void fact1()
        {
            BigInteger n = 2;
            BigInteger mult = 1;
                        
            while (true)
            {
                BigInteger r = mult + 1;

                if (r % n == 0)
                {
                    String sResult = n.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "fact1");
                }

                mult *= n ;

                n++;
            }
        }

        public static void piramide0()
        {
            BigInteger n = 2;
            BigInteger i = 0;

            BigInteger iter = -1;
            while (true)
            {
                

                if(n % 2 == 0)
                {
                    for (BigInteger j = 1; j <= n; j++)
                    {
                        i++;

                        if (j == n / 2)
                        {
                            iter = i;
                        }
                    }
                }
                else
                {
                    BigInteger result = iter;

                    for (BigInteger j = 1; j <= n; j++)
                    {
                        i++;

                        if (j == ((n / 2) + 1))
                        {
                            result += i;
                        }
                    }

                    String sResult = result.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "piramide0");
                }
                
                

                n++;
            }
        }

        public static void creCRE2()
        {
            BigInteger n = 2;

            BigInteger mult = 1;
            BigInteger sum = 1;
            while (true)
            {
                if (n % 2 == 0)
                {
                    mult *= n;
                    for (BigInteger i = 0; i < mult; i++)
                    {                        
                        sum += n;                        
                    }
                }
                else
                {
                    sum += mult * n;
                    mult *= n;
                }

                String sResult = sum.ToString();
                Console.WriteLine(String.Concat(";", sResult));
                addTextEscritorio(String.Concat(";", sResult), "creCRE2");

                n++;
            }
        }

        public static void creCRE()
        {
            BigInteger n = 2;
                        
            while (true)
            {
                
                BigInteger sum = 0;
                for (BigInteger i = 1; i < n; i++)
                {
                    BigInteger mult = 0;
                    for (BigInteger j = 0; j < i; j++)
                    {
                        mult += i;
                    }

                    //BigInteger k2 = i * (n - 1);
                    for (BigInteger k = 0; k < mult; k++)
                    {
                        for (BigInteger j = 0; j < n; j++)
                        {
                            sum += n;                            
                        }                                                                        
                    }

                    

                }

                BigInteger r = sum + (n - 1);
                
                String sResult = r.ToString();
                Console.WriteLine(String.Concat(";", sResult));
                addTextEscritorio(String.Concat(";", sResult), "creCRE");

                n++;
            }
        }

        public static void creIlogic()
        {
            BigInteger n = 2;

            BigInteger mult = 1;
            BigInteger sum = 1;

            while (true)
            {
                for (BigInteger i = (n - 1); i > 0; i--)
                {
                    BigInteger result = mult * n;
                    mult = result;
                    sum += result;
                }
                BigInteger r = sum;
                //sumNS += mult;
                String sResult = sum.ToString();
                Console.WriteLine(String.Concat(";", sResult));
                addTextEscritorio(String.Concat(";", sResult), "creIlogic");

                n++;
            }
        }

        public static void creNeuron()
        {
            BigInteger n = 2;            
            BigInteger mult = 1;
            BigInteger sum = 1;

            while (true)
            {
                BigInteger result = mult * n;
                mult += result;
                result += sum;
                sum = mult;

                //sumNS += mult;
                String sResult = result.ToString();
                Console.WriteLine(String.Concat(";", sResult));
                addTextEscritorio(String.Concat(";", sResult), "creNeuron");
                

                n++;

            }
        }

        public static void creFrac()
        {
            BigInteger nX = 0;
            BigInteger nY = 0;

            BigInteger nXqq = 1;
            BigInteger nYqq = 1;

            BigInteger nXY = 2;

            BigInteger nImpar = 3;

            Boolean sum = true;

            BigInteger z = 0;

            while (true)
            {

                for (BigInteger i = nImpar; i > 0; i--)
                {
                    if (nXqq != 0)
                    {
                        nXqq--;
                        if (sum)
                        {
                            nX++;
                        }
                        else
                        {
                            nX--;
                        }
                    }
                    else if (nYqq != 0)
                    {
                        nYqq--;
                        if (sum)
                        {
                            nY++;
                        }
                        else
                        {
                            nY--;
                        }
                    }
                    else
                    {
                        sum = !sum;
                        nXqq = nXY;
                        nXqq--;
                        if (sum)
                        {
                            nX++;
                        }
                        else
                        {
                            nX--;
                        }
                        nYqq = nXY;
                        nXY++;
                    }
                }

                if (nX == z || nY == z)
                {
                    z++;
                    //sumNS += mult;
                    String sResult = nImpar.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "creFrac");
                }

                nImpar += 2;

            }


        }

        static void c_DownloadDataCompleted(object sender,
                                    DownloadDataCompletedEventArgs e)
        {

            if (e.Error == null)
            {
                String s = Encoding.ASCII.GetString(e.Result);
                

                var regex = new Regex(@"(['""])([^'""]+\.(jpg|png|bmp|gif))\1");
                var match = regex.Match(s);
                while (match.Success)
                {
                    addTextEscritorio(String.Concat(";", match.Groups[2].Value), "image_findes");
                    Console.WriteLine("IMAGE > " + match.Groups[2].Value);
                    match = match.NextMatch();
                }

                List<String> hrefs = DumpHRefs(s);

                foreach (String href in hrefs)
                {
                    if(href.Contains("enjin.io/"))
                    {
                        var r = ((WebClient)sender).DownloadString(new Uri(href));
                        Console.WriteLine("get enjin.io");
                    }
                }

            }
        }

        private static List<String> DumpHRefs(string inputString)
        {
            List<String> hrefs = new List<String>();

            string hrefPattern = @"href\s*=\s*(?:[""'](?<1>[^""']*)[""']|(?<1>[^>\s]+))";

            try
            {
                Match regexMatch = Regex.Match(inputString, hrefPattern,
                                               RegexOptions.IgnoreCase | RegexOptions.Compiled,
                                               TimeSpan.FromSeconds(10));
                while (regexMatch.Success)
                {
                    //Console.WriteLine($"Found href {regexMatch.Groups[1]} at {regexMatch.Groups[1].Index}");
                    hrefs.Add(regexMatch.Groups[1].Value);
                    regexMatch = regexMatch.NextMatch();
                }
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine("The matching operation timed out.");
            }

            return hrefs;
        }

        static IEnumerable<IEnumerable<T>> GetPermutationsWithRept<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutationsWithRept(list, length - 1)
                .SelectMany(t => list,
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        static void enjinHunt2023(WebClient client)
        {

            List<String> urlsJoinChecked = new List<string>();
            List<String> urlsChecked = new List<string>();

            //List<String> hrefs = new List<String>() { "https://urlS" };
            Int32 iImg = 0;
            BigInteger i = 6999999999883402;
            while (i > -1)
            {
                //hrefs = hrefs.Distinct().ToList();
                Thread.Sleep(8000);
                String url = "https://enjinx.io/eth/asset/" + i;
                //urlsChecked.Add(url);
                //hrefs.RemoveAt(0);

                try
                {
                    var s = client.DownloadString(new Uri(url));
                    Console.WriteLine("get " + url);

                    //String[] sS = s.Split(new String[1] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    //foreach (String SE in sS)
                    //{
                    //    String[] DerivationPath = SE.Split(new String[2] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);

                    //    if(DerivationPath.Length > 2 && !DerivationPath[0].Contains("Asset Not Found!"))                    
                    //    {
                        if (!s.Contains("EnjinX - Ethereum Blockchain Explorer"))
                        {

                            Console.WriteLine(String.Concat(i));
                            addTextEscritorio(String.Concat(s), "T_M_" + i);
                        }
                    //    }

                    //}

                    //var regex = new Regex(@"(['""])([^'""]+\.(jpg|png|bmp|gif))\1");
                    //var match = regex.Match(s);
                    //while (match.Success)
                    //{
                    //    String sImg = match.Groups[2].Value;

                    //    ////try
                    //    ////{
                    //    ////    if (!sImg.Contains("QR"))
                    //    ////    {
                    //    ////        byte[] imgBytes = client.DownloadData(sImg);
                    //    ////        File.WriteAllBytes("c:\\logs\\enjin\\" + iImg.ToString() + ".png", imgBytes);
                    //    ////        iImg++;
                    //    ////    }
                    //    ////}
                    //    ////catch (Exception ex)
                    //    ////{

                    //    ////}

                    //    //if (sImg.Contains("QR") || sImg.Contains("QR") || 
                    //    //    sImg.Contains("Qr") || sImg.Contains("Qr") ||
                    //    //    sImg.Contains("qr") || sImg.Contains("qr") ||
                    //    //    sImg.Contains("EGG") || sImg.Contains("GOLD") ||
                    //    //    sImg.Contains("Egg") || sImg.Contains("Gold")
                    //    //    || sImg.Contains("egg") || sImg.Contains("gold"))
                    //    //{
                    //    //    addTextEscritorio(String.Concat(";", url), "images_findes");
                    //    //    addTextEscritorio(String.Concat(";", sImg), "images_findes");
                    //    //    //Console.WriteLine("IMAGE > " + sImg);
                    //    //}
                    //    //match = match.NextMatch();
                    //}

                    //////List<String> localHref = DumpHRefs(s);

                    //////foreach (String lH in localHref)
                    //////{   
                    //////    if(lH.Contains("platform.enjin.io"))
                    //////    {
                    //////        addTextEscritorio(String.Concat(";", lH), "claims");
                    //////    }

                        

                    //////    if (lH.StartsWith("/"))
                    //////    {
                    //////        if (!hrefs.Contains("https://enjin.io" + lH))
                    //////        {
                    //////            if (!urlsChecked.Contains("https://enjin.io" + lH))
                    //////            {
                    //////                hrefs.Add("https://enjin.io" + lH);
                    //////            }
                    //////        }
                    //////    }
                    //////    else if(lH.StartsWith("#"))
                    //////    {
                    //////        if (!hrefs.Contains(url + lH))
                    //////        {
                    //////            if (!urlsChecked.Contains(url + lH))
                    //////            {
                    //////                hrefs.Add(url + lH);
                    //////            }
                    //////        }
                    //////    }
                    //////    else{
                            
                    //////            if (!urlsJoinChecked.Contains(lH))
                    //////            {
                    //////                addTextEscritorio(String.Concat(";", lH), "URLs");
                    //////                urlsJoinChecked.Add(lH);
                    //////            }
                    //////    }
                    //////}
                }
                catch (Exception ex)
                {
                    addTextEscritorio(String.Concat(";", url), "T_M_errors");
                }

                i--;
            }
            Console.WriteLine("Finish");
            

            //var items = new List<String> { "a","b","c","d","e","f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r",
            //"s","t","u","v","w","x","y","z","0","1","2","3","4","5","6","7","8","9"};
            //var list = GetPermutationsWithRept(items, 10);
            //GetCombination(items);

            Console.ReadKey();
        }

        static async void jointhequest(WebClient client, List<String> words)
        {
            Int32 n = 1;
            try
            {
                //var rp = client.DownloadString(new Uri("http://www.jointhequest.io/acapella"));

                //addTextEscritorio(String.Concat(";", "acapella", rp), "rpo_finded");
                //Console.WriteLine(rp);

                //addTextEscritorio(String.Concat(";", "acapella", Encoding.ASCII.GetString(rp)), "rpo_finded");
                //Console.WriteLine(Encoding.ASCII.GetString(rp));
            }
            catch(Exception ex)
            {

            }
            Boolean con = false;
            while (true)
            {
                //var items = new List<String> { "a","b","c","d","e","f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r",
                //"s","t","u","v","w","x","y","z","0","1","2","3","4","5","6","7","8","9"};
                //var list = GetPermutationsWithRept(items, n);

                var list = GetPermutationsWithRept(words, n);

                foreach (var p in list)
                {
                    String s = String.Empty;
                    foreach (String c in p)
                    {
                        s += c;
                    }
                    Console.WriteLine(s);
                    addTextEscritorioNew(String.Concat(";", s), "rpo_last");
                    if (con)
                    {
                        //var r = await client.DownloadDataTaskAsync(new Uri("http://www.jointhequest.io/" + s));

                        //if (r != null || r.Length > 0)
                        //{
                        //    String sR = Encoding.ASCII.GetString(r);
                        //    addTextEscritorio(String.Concat(";", s, Encoding.ASCII.GetString(r)), "rpo_finded");
                        //    Console.WriteLine(sR);
                        //}

                        try
                        {
                            var r = client.DownloadString(new Uri("http://www.jointhequest.io/" + s));
                            String sR = r;
                            addTextEscritorio(String.Concat(";", s, r), "rpo_finded");
                            Console.WriteLine(sR);
                        }
                        catch(Exception ex)
                        {
                            if (!ex.Message.Contains("404"))
                            {

                            }
                        }
                        

                        Thread.Sleep(10000);
                    }
                    else
                    {
                        if (s == "technologyyoure")
                        {
                            con = true;
                        }

                        //book
                        //if (s == "hopped")
                        //{
                        //    con = true;
                        //}
                    }
                }

                n++;
            }

            //var items = new List<String> { "a","b","c","d","e","f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r",
            //"s","t","u","v","w","x","y","z","0","1","2","3","4","5","6","7","8","9"};
            //var list = GetPermutationsWithRept(items, 10);
            //GetCombination(items);

            Console.Read();
        }

        static void GetCombination(List<String> list)
        {
            double count = Math.Pow(2, list.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                string str = Convert.ToString(i, 2).PadLeft(list.Count, '0');
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        Console.Write(list[j]);
                    }
                }
                Console.WriteLine();
            }
        }

        private static void allState(int index, string[] inStr)
        {
            string a = inStr[index].ToString();
            int l = index + 1;
            int k = l;
            var result = string.Empty;
            var t = inStr.Length;
            int i = index;
            while (i < t)
            {
                string s = a;
                for (int j = l; j < k; j++)
                {
                    s += inStr[j].ToString();
                }
                //result += s + ",";
                Console.WriteLine(s);
                k++;
                i++;
            }

            index++;
            //if (index < inStr.Length)
            //    result += allState(index, inStr);
            //return result.TrimEnd(new char[] { ',' });
            return;
        }

        public static void creSUM212()
        {            
            BigInteger n = 2;
            
            while (true)
            {
                BigInteger sumNS = n;
                BigInteger mult = n - 1;
                BigInteger i = mult;
                while (i > 0)
                {
                    sumNS += mult * i;
                    mult += mult * i;
                    i--;
                }
                //sumNS += mult;
                String sResult = sumNS.ToString();
                Console.WriteLine(String.Concat(";", sResult));
                addTextEscritorio(String.Concat(";", sResult), "creSUM212");
                
                n++;                
            }


        }

        public static void SUMAcrecimientoNucleico()
        {
            List<BigInteger> lista = new List<BigInteger>() { 1, 2, 1 };

            BigInteger n = 3;

            while (true)
            {
                List<BigInteger> temp = new List<BigInteger>();

                BigInteger sumNS = 0;
                BigInteger iA = -1;
                foreach (BigInteger l in lista)
                {
                    if (iA != -1)
                    {
                        BigInteger sum = iA + l;

                        sumNS += sum;
                        temp.Add(sum);
                    }
                    iA = l;
                    sumNS += l;
                    temp.Add(l);
                }

                sumNS++;


                String sResult = sumNS.ToString();
                Console.WriteLine(String.Concat(";", sResult));
                addTextEscritorio(String.Concat(";", sResult), "crecimientoArboreo");
                

                lista = temp;                                
                n++;

            }


        }
        public static void crecimiento6()
        {
            BigInteger n = 7;

            BigInteger i = 2;
            while (true)
            {
                BigInteger nCsum = 0;

                foreach (Char c in n.ToString())
                {
                    nCsum += Convert.ToInt32(c.ToString());
                }

                BigInteger nCsumRE = nCsum;

                while (nCsumRE.ToString().Length > 1)
                {
                    BigInteger nCsumREtemp = 0;
                    foreach (Char c in nCsumRE.ToString())
                    {
                        nCsumREtemp += Convert.ToInt32(c.ToString());
                    }
                    nCsumRE = nCsumREtemp;
                }


                // Si el número se considera primo se guarda en memoria y se graba en el archivo de histórico
                if (n % i != 0 && nCsum % i != 0 && n.ToString()[0] != nCsumRE.ToString()[0] && n % (nCsum + nCsumRE) != 0)
                {
                    String sResult = n.ToString();
                    Console.WriteLine(";" + sResult);
                    addTextEscritorio(String.Concat(";", sResult), "sResult");
                }


                n+=6;
                i++;
            }
        }
        public static void crecimientoPotencial2()
        {
            BigInteger n = 2;

            BigInteger factAnterior = 1;
            while (true)
            {
                BigInteger fact = factorial(n);
                BigInteger result = (Potencia(n, fact - factAnterior)) + ((fact / factAnterior) * n) + (n*n);
                factAnterior = fact;
                //for (BigInteger i = n; i > 1; i--)
                //{
                //    result *= Potencia(factorial(i), i);
                //}

                //result--;

                // Si el número se considera primo se guarda en memoria y se graba en el archivo de histórico
                if (true)
                {
                    String sResult = result.ToString();
                    Console.WriteLine(";" + sResult);
                    addTextEscritorio(String.Concat(";", sResult), "sResult");
                }


                n++;
            }
        }


        public static void crecimientoPotencial()
        {
            BigInteger n = 3;

            BigInteger nSUM = 1;
            BigInteger nFACT = 1;
            
            while (true)
            {
                BigInteger potencia = n;

                for (BigInteger i = n - 1; i > 2; i--)
                {
                    potencia = Potencia(i, potencia);
                }

                BigInteger result = Potencia(2, potencia) + potencia;
                
                // Si el número se considera primo se guarda en memoria y se graba en el archivo de histórico
                if (true)
                {
                    String sResult = result.ToString();
                    Console.WriteLine(";" + sResult);
                    addTextEscritorio(String.Concat(";", sResult), "sResult");
                }
                

                n++;                
            }
        }

        public static void priNormalTiempo()
        {
            // Lista en memoria donde tendremos todos los número primos
            List<BigInteger> primos = new List<BigInteger>();

            BigInteger numero = 3;

            while (true)
            {
                Boolean esPrimo = true;
                BigInteger exacta = -1;
                BigInteger maxNum = raizCuadradaMasTres(numero, out exacta);
                foreach (BigInteger primo in primos)
                {
                    if (BigInteger.Remainder(numero, primo) == 0)
                    {
                        esPrimo = false;
                        break;
                    }

                    if (primo > maxNum)
                    {
                        break;
                    }
                }


                // Si el número se considera primo se guarda en memoria y se graba en el archivo de histórico
                if (esPrimo)
                {
                    primos.Add(numero);
                    Console.WriteLine(numero.ToString());
                }

                // Incrementamos a sólo impares (que son los candidatos de primos)
                numero += 2;
            }
        }

        public static void aLaDesesperada()
        {            
            BigInteger n = 3;
            Dictionary<BigInteger, List<BigInteger>> listaFutura =
                new Dictionary<BigInteger, List<BigInteger>>();
            listaFutura.Add(9, new List<BigInteger>() { 3 });
            
            while (true)
            {                
                if (!listaFutura.ContainsKey(n))
                {
                    String r = n.ToString();
                    Console.WriteLine(String.Concat(";", r));
                    //addTextEscritorio(String.Concat(";", r), "aLaDesesperada");
                }
                else
                {
                    List<BigInteger> dato = listaFutura[n];

                    Dictionary<BigInteger, List<BigInteger>> nuevosFuturos = new Dictionary<BigInteger, List<BigInteger>>();

                    foreach (BigInteger bD in dato)
                    {
                        BigInteger nuevaKey = n + bD + bD;
                        if (!listaFutura.ContainsKey(nuevaKey) &&
                            !nuevosFuturos.ContainsKey(nuevaKey))
                        {
                            List<BigInteger> divisores = new List<BigInteger>() { bD };

                            BigInteger divisor = nuevaKey / bD;
                            while (divisor != 1)
                            {
                                //BigInteger resto = -1;
                                //BigInteger div = BigInteger.DivRem(divisor, 2, out resto);

                                //if (resto == 0)
                                //{
                                //    divisor = div;
                                //    if (!divisores.Contains(2))
                                //    {
                                //        divisores.Add(2);
                                //    }
                                //}
                                //else
                                //{
                                    BigInteger exacta = -1;
                                    BigInteger raizMasTres = raizCuadradaMasTres(divisor, out exacta);

                                    Boolean encontrado = false;
                                    for (BigInteger i = 3; i <= raizMasTres; i += 2)
                                    {
                                        BigInteger resto = -1;
                                        BigInteger div = BigInteger.DivRem(divisor, i, out resto);
                                        if (resto == 0)
                                        {
                                            encontrado = true;
                                            divisor = div;
                                            if (!divisores.Contains(i))
                                            {
                                                divisores.Add(i);
                                            }
                                        }
                                    }
                                    if(!encontrado)
                                    {
                                        encontrado = true;                                            
                                        if (!divisores.Contains(divisor))
                                        {
                                            divisores.Add(divisor);
                                        }
                                        divisor = 1;
                                        
                                    }
                                //}

                            }

                            nuevosFuturos.Add(nuevaKey, divisores);                            
                        }

                        listaFutura.Remove(n);
                    }

                    foreach (KeyValuePair<BigInteger, List<BigInteger>> nF in nuevosFuturos)
                    {
                        listaFutura.Add(nF.Key, nF.Value);
                    }
                    
                }
             
                n+=2;
            }
        }

        public static void crecimientoNucleico3()
        {
            List<BigInteger> lista = new List<BigInteger>() { 1, 2, 1 };

            BigInteger n = 3;

            while (true)
            {
                List<BigInteger> temp = new List<BigInteger>();

                BigInteger sumNS = 0;
                BigInteger iA = -1;
                BigInteger iB = 0;
                foreach (BigInteger l in lista)
                {
                    if (iA != -1)
                    {
                        BigInteger sum = iA + l;

                        if (sum == n)
                        {
                            sumNS += n;
                            temp.Add(n);
                            iB++;
                        }
                    }
                    iA = l;
                    sumNS += l;
                    temp.Add(l);
                }

                sumNS += n - 2;


                if (sumNS % 2 != 0)                
                {
                    String sResult = sumNS.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "crecimientoArboreo");
                }

                lista = temp;

                BigInteger length = lista.Count;

                n++;

            }


        }

        public static void creciminetoN()
        {
            BigInteger nSUP = 2;

            BigInteger n = 1;
            BigInteger nImpar = 3;

            BigInteger sumN_NImpar = 0;
            Boolean flip = true;
            while (true)
            {
                if (flip)
                {
                    sumN_NImpar += n * n;
                    n++;
                }
                else
                {
                    sumN_NImpar += nImpar;
                    nImpar += 2;
                }

                flip = !flip;

                BigInteger result = nSUP + sumN_NImpar;

                if (sumN_NImpar % 2 != 0)
                {
                    String r = sumN_NImpar.ToString();
                    Console.WriteLine(String.Concat(";", r));
                    addTextEscritorio(String.Concat(";", r), "creciminetoN");
                }                                
            }
        }

        public static void crecimientoNucleico2()
        {
            List<BigInteger> lista = new List<BigInteger>() { 1, 2 };

            BigInteger n = 3;

            while (true)
            {
                List<BigInteger> temp = new List<BigInteger>();

                BigInteger iA = -1;
                BigInteger iB = 0;
                foreach (BigInteger l in lista)
                {
                    Boolean inserto = false;
                    if (iA != -1)
                    {
                        BigInteger sum = iA + l;

                        if (sum == n)
                        {
                            inserto = true;
                            temp.Add(n);
                            iB++;
                        }
                    }

                    iA = l;
                    temp.Add(l);

                    if (inserto && iB > 2)
                    {
                        if (temp[temp.Count - 1] > temp[temp.Count - 2])
                        {
                            temp.RemoveAt(temp.Count - 2);
                        }
                        else
                        {
                            temp.RemoveAt(temp.Count - 1);
                        }
                    }
                }

                if (n % 2 != 00 && iB > 2)
                //if (iB == n - 1)
                {
                    String sResult = n.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "crecimientoNucleico2");
                }

                lista = temp;

                BigInteger length = lista.Count;

                n++;

            }


        }
        public static void oTroIntento()
        {
            

            BigInteger n = 3;
            BigInteger nMULT = 6;

            BigInteger nS1 = 2;
            BigInteger nMULTS1 = 2;

            BigInteger nS0 = 1;
            BigInteger nMULTS0 = 1;


            while (true)
            {
                BigInteger result = (nMULT / nMULTS0) + nS0;

                if (result % 2 != 0)
                {                    
                    //if ()
                    //{
                    String sResult = result.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "oTroIntento");
                    //}

                }

                nS0 = nS1;
                nMULTS0 = nMULTS1;
                nS1 = n;
                nMULTS1 = nMULT;
                n++;
                nMULT *= n;
            }
        }

        public static void DeLaNorma()
        {
            BigInteger n0 = 1;
            BigInteger n1 = 1;

            BigInteger n0B = 2;
            BigInteger n1B = 3;

            BigInteger n0C = 3;
            BigInteger n1C = 6;

            BigInteger esperado = 5;
            
            while (true)
            {
                BigInteger baseA = (n1 * n1B * n1C);
                BigInteger result = (n1C * n1 * n0B) + (n1C * n1B * n0) + (n1 * n1B * n0C);

                if (result % 2 == 0)
                {
                    result /= 2;
                }
                

                if (true)
                {                    
                    String sResult = result.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "DeLaNorma");
                }

                n0 = n0B;
                n1 = n1B;

                n0B = n0C;
                n1B = n1C;

                n0C++;
                n1C += n0C;

                esperado += 2;

            }
        }

        public static void crecimientoCresta()
        {
            BigInteger n0 = 2;
            BigInteger n1 = 3;

            Boolean flip = true;
            while (true)
            {
                BigInteger result = n0 + n1;

                if (flip)
                {
                    n0 = result;
                    String sResult = result.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "crecimientoCresta");
                }
                else
                {
                    n1 = result;
                }

                flip = !flip;



            }


        }

        public static void crecimientoNucleico()
        {               
            List<BigInteger> lista = new List<BigInteger>() { 1, 2 };

            BigInteger n = 3;
            
            while (true)
            {
                List<BigInteger> temp = new List<BigInteger>();

                BigInteger iA = -1;
                BigInteger iB = 0;
                foreach (BigInteger l in lista)
                {
                    if (iA != -1)
                    {
                        BigInteger sum = iA + l;

                        if (sum == n)
                        {
                            temp.Add(n);
                            iB++;

                        }
                    }
                    iA = l;
                    temp.Add(l);                    
                }

                if (n% 2 != 00 && iB == (n - 1) / 2)
                //if (iB == n - 1)
                {                    
                    String sResult = n.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "crecimientoArboreo");
                }

                lista = temp;

                BigInteger length = lista.Count;

                n++;

            }


        }

        public static void crecimientoArboreo()
        {
            BigInteger antnSum = 1;
            BigInteger nSum = 2;
            BigInteger n = 3;

            while (true)
            {
                BigInteger result = n;
                BigInteger tempN = n + 1;
                BigInteger tempNSum = n;

                BigInteger j = (n + nSum) - (n + 1);

                result += nSum * j;
                tempN += j;
                tempNSum += j % 2 == 0 ? ((j / 2) + (n + 1)) * j : ((j / 2) + (n + 1)) * j ;
                //for (BigInteger i = n + 1; i < n + nSum; i++)
                //{
                //    //result += nSum;
                //    //tempN++;
                //    tempNSum += i;
                //}
                n = tempN;
                nSum = tempNSum;

                if (result % 2 != 0)
                {
                    String sResult = result.ToString();
                    Console.WriteLine(String.Concat(";", sResult));
                    addTextEscritorio(String.Concat(";", sResult), "crecimientoArboreo");
                }
            }


        }

        public static void crecimientoRaro()
        {
            BigInteger n0 = -1;
            BigInteger n = 2;
            BigInteger sum = 0;
            List<BigInteger> pS = new List<BigInteger>();
            //List<BigInteger> nMs = new List<BigInteger>();
            while (true)
            {
                BigInteger exacta = -1;
                BigInteger rC = raizCuadrada(n, out exacta);

                //BigInteger nMs = 0;
                
                BigInteger tempN = n;
                while (tempN != 1)
                {
                    if (tempN % 2 == 0)
                    {
                        tempN = tempN / 2;

                        //if (!nMs.Contains(2))
                        //{
                        //    nMs.Add(2);
                        //}
                        sum += 2;
                        continue;
                    }

                    for (BigInteger nM = 3; nM <= n; nM += 2)
                    {
                        if (tempN % nM == 0)
                        {
                            tempN = tempN / nM;

                            //if (!nMs.Contains(nM))
                            //{
                            //    nMs.Add(nM);
                            //}
                            sum += nM;
                            break;
                        }
                    }
                }

                //foreach (BigInteger nM in nMs)
                //{
                //    n += nM;
                //}

                n += sum + n0;

                if (n % 2 == 0)
                {
                    //BigInteger tN = n + (n - 1);
                    //foreach (BigInteger nM in nMs)
                    //{
                    //    if (nM % 2 != 0)
                    //    {
                    //        tN += nM;
                    //    }
                    //}

                    //Boolean esPri = true;

                    //foreach (BigInteger p in pS)
                    //{
                    //    if (tN % p == 0)
                    //    {
                    //        //BigInteger delTwo = tN / p;
                    //        //pS.Add(delTwo);
                    //        //addTextEscritorio(String.Concat(">;", delTwo), "crecimientoRaro");

                    //        esPri = false;
                    //        break;
                    //    }
                    //}

                    //if (esPri)
                    //{
                    //    pS.Add(tN);
                        //addTextEscritorio(String.Concat(";", tN), "crecimientoRaro");
                    //}
                    //n++;


                }
                else
                {
                    //BigInteger tN = n + (n + 1);
                    addTextEscritorio(String.Concat(";", n), "crecimientoRaro");
                }
                //sum++;
                n0--;
            }
        }

        public static void sumatorio()
        {
            BigInteger nSum = 1;
            BigInteger n = 1;

            BigInteger nSum2 = 3;
            BigInteger n2 = 2;

            while (true)
            {
                BigInteger result = (nSum * n) + (nSum2 * n2);

                if (result % 2 != 0)
                {

                }

                n = n2;
                nSum = nSum2;

                n2++;
                nSum2 += n2;

                //addTextEscritorio(String.Concat(";", n), "sumatorio");
            }
        }

        public static void sumC()
        {
            BigInteger exacta = -1;
            BigInteger raiz3 = raizX(27, 3, out exacta);
        }

        public static void sumCd()
        {                                    
            BigInteger n = 3;
            BigInteger i = 0;
            BigInteger ieee = 0;

            BigInteger i2Temp = 0;
            BigInteger i2i = 0;
            BigInteger i2 = 0;
            Boolean changei2 = false;
            while (true)
            {
                Boolean divisible = false;                
                for (BigInteger j = 0; j <= i2; j++)
                {
                    for (BigInteger k = 0; k <= j; k++)
                    {
                        BigInteger sum0 = 0;
                        BigInteger sum = 0;
                        for (BigInteger z = 0; z <= k; z++)
                        {
                            sum0 += i - z;
                            BigInteger sum2 = i - j - z;
                            sum += sum2;
                            divisible = (sum0 > 1 && n % sum0 == 0) || (sum > 1 && n % sum == 0) || (sum2 > 1 && n % sum2 == 0) || (i2Temp > 1 && n % i2Temp == 0);
                            if (divisible)
                            {
                                break;
                            }
                        }
                        if (divisible)
                        {
                            break;
                        }
                    }
                    if (divisible)
                    {
                        break;
                    }
                }

                if (!divisible)
                {
                    for (BigInteger j = 0; j <= i2; j++)
                    {
                        for (BigInteger k = 0; k <= j; k++)
                        {
                            BigInteger sum0 = 0;
                            BigInteger sum = 0;
                            for (BigInteger z = 0; z <= k; z++)
                            {
                                sum0 += ieee - z;
                                BigInteger sum2 = ieee - j - z;
                                sum += sum2;
                                divisible = (sum0 > 1 && n % sum0 == 0) || (sum > 1 && n % sum == 0) || (sum2 > 1 && n % sum2 == 0) || (i2Temp > 1 && n % i2Temp == 0);
                                if (divisible)
                                {
                                    break;
                                }
                            }
                            if (divisible)
                            {
                                break;
                            }
                        }
                        if (divisible)
                        {
                            break;
                        }
                    }
                }
                if (i != 1 && divisible)
                {
                    BigInteger exacta = -1;
                    BigInteger raiz = raizCuadrada(n, out exacta);
                    if(exacta == 0)
                    {
                        i2Temp++;
                        ieee = i2;
                        i2i = 1;
                        changei2 = true;
                    }
                }
                else
                {
                    addTextEscritorio(String.Concat(";", n), "nnnP");
                }

                n += 2;
                if (!divisible)
                {
                    i++;
                    ieee++;
                }
                

                if (changei2)
                {
                    if (i2Temp == i2i)
                    {
                        changei2 = false;
                        i = i2Temp;
                        i2 = i2Temp;
                    }
                    else
                    {
                        i2i++;
                    }
                }
            }
        }

        public static void piramideCrecienteX()
        {
            BigInteger n = 1;
            BigInteger pN = 0;
            while (true)
            {

                BigInteger sum = 0;
                BigInteger nN = 1;
                Boolean isFirst = true;
                while (true)
                {
                    sum += nN;
                    if (!isFirst && sum + pN == n)
                    {
                        break;
                    }
                    nN++;
                    n+=2;
                    isFirst = false;
                }

                if (n % 2 != 0)
                {

                }

                pN++;
                n+=2;
            }
        }

        public static void nP3()
        {
            BigInteger n = 2;
            List<BigInteger> nPar = new List<BigInteger>() { 2 };
            
            BigInteger SUM = 0;
            while (true)
            {
                BigInteger sum = 0;                
                List<BigInteger> nTemp = nPar;
                for (Int32 i = 0; i < nPar.Count; i++)
                {
                    for (Int32 j = 0; j < nTemp.Count; j++)
                    {
                        if (i != j)
                        {
                            sum += nPar[i] + nTemp[j];
                        }
                    }
                }

                BigInteger nIMPAR = n - 1;
                BigInteger nIM = 0;
                BigInteger k = 1;
                for (BigInteger i = 1; i <= nIMPAR; i++)
                {
                    nIM += k;
                    k += 2;
                }

                BigInteger rSum = sum + nIM;

                SUM += sum;

                BigInteger rSUM = SUM + nIM;

                n +=2;
                nIMPAR += 2;
                nPar.Add(n);
            }
        }

        public static void nP2()
        {
            BigInteger n = 3;
            
            while (true)
            {
                BigInteger iN = 1;
                BigInteger n2 = n;
                BigInteger sum = 0;
                Boolean sumStart = false;
                Boolean getNext = false;
                BigInteger nNew = -1;
                List<BigInteger> operandos = new List<BigInteger>();
                using (StreamReader writer = new StreamReader(@"C:\logs\nP.txt"))
                {
                    String s = writer.ReadLine();                    
                    while (s != null)
                    {
                        BigInteger sN = BigInteger.Parse(s.Replace(";", ""));

                        if (getNext)
                        {
                            nNew= sN;
                            getNext = false;
                        }

                        if(sN == n)
                        {
                            sumStart = true;
                            getNext = true;
                        }

                        if (sumStart)
                        {
                            sum += sN;
                            operandos.Add(sN);
                        }

                        if (iN == n2)
                        {
                            break;
                        }

                        s = writer.ReadLine();
                        if (sumStart)
                        {
                            iN++;
                        }

                    }
                }

                Boolean esP = true;
                foreach(BigInteger o in operandos)
                {
                    if (sum % o == 0)
                    {
                        esP = false;
                    }
                }

                if (esP)
                {
                    BigInteger r = sum;
                }
                n = nNew;
            }
        }
        public static void Creciente()
        {
            BigInteger n = 1;
            BigInteger sum = 0;
            while (true)
            {
                sum += n;
                BigInteger resto = -1;
                BigInteger div = BigInteger.DivRem(n, 3, out resto);

                if (resto != 0)
                {

                }
                else
                {
                    sum += 3;
                }

                if (sum % 2 != 0)
                {

                }

                n++;
            }
        }

        public static void CrecienteS()
        {            
            BigInteger n = 9;
            BigInteger sum = 0;
            while (true)
            {
                List<BigInteger> divisoresNI = new List<BigInteger>();

                


                BigInteger elementoAValidarNI = n;
                Boolean seguirNI = true;
                BigInteger sum_NI = 0;
                while (seguirNI)
                {
                    BigInteger exacta0 = -1;
                    BigInteger raiz2M3 = raizCuadradaMasUno(elementoAValidarNI, out exacta0);
                    for (BigInteger i = 2; i < raiz2M3; i++)
                    {
                        BigInteger resto = -1;
                        BigInteger div = BigInteger.DivRem(elementoAValidarNI, i, out resto);

                        if (resto == 0)
                        {
                            if (div != 1)
                            {
                                divisoresNI.Add(i);
                                sum_NI += i;
                                elementoAValidarNI = div;
                                i = 1;
                            }
                        }
                    }

                    if (elementoAValidarNI != 1 && elementoAValidarNI != n)
                    {
                        divisoresNI.Add(elementoAValidarNI);
                        sum_NI += elementoAValidarNI;
                    }

                    seguirNI = false;

                }

                if (divisoresNI.Count > 0)
                {
                    foreach (BigInteger d in divisoresNI)
                    {
                        for (BigInteger i = 0; i < d; i++)
                        {
                            sum += n;
                        }
                    }
                }
                else
                {
                    sum -= n;
                }

                if (sum % 2 != 0)
                {

                }
                else
                {

                }

                n++;
            }
        }

        public static void sumatorioCreciente()
        {
            BigInteger n = 1;
            BigInteger nI = 2;
            BigInteger nD = 3;
            while (true)
            {

                List<BigInteger> divisoresNI = new List<BigInteger>();

                BigInteger elementoAValidarNI = nI;
                Boolean seguirNI = true;
                BigInteger sum_NI = 0;
                while (seguirNI)
                {
                    BigInteger exacta0 = -1;
                    BigInteger raiz2M3 = raizCuadradaMasUno(elementoAValidarNI, out exacta0);
                    for (BigInteger i = 2; i < raiz2M3; i++)
                    {
                        BigInteger resto = -1;
                        BigInteger div = BigInteger.DivRem(elementoAValidarNI, i, out resto);

                        if (resto == 0)
                        {
                            if (div != 1)
                            {
                                divisoresNI.Add(i);
                                sum_NI += i;
                                elementoAValidarNI = div;
                                i = 1;
                            }
                        }
                    }

                    if (elementoAValidarNI != 1 && elementoAValidarNI != nI)
                    {
                        divisoresNI.Add(elementoAValidarNI);
                        sum_NI += elementoAValidarNI;
                    }

                    seguirNI = false;

                }

                List<BigInteger> divisoresND = new List<BigInteger>();

                BigInteger elementoAValidar = nD;
                Boolean seguir = true;
                BigInteger sum_ = 0;
                while (seguir)
                {
                    BigInteger exacta0 = -1;
                    BigInteger raiz2M3 = raizCuadradaMasUno(elementoAValidar, out exacta0);
                    for (BigInteger i = 2; i < raiz2M3; i++)
                    {
                        BigInteger resto = -1;
                        BigInteger div = BigInteger.DivRem(elementoAValidar, i, out resto);

                        if (resto == 0)
                        {
                            if (div != 1)
                            {
                                divisoresND.Add(i);
                                sum_ += i;
                                elementoAValidar = div;
                                i = 1;
                            }
                        }
                    }

                    if (elementoAValidar != 1 && elementoAValidar != nD)
                    {
                        divisoresND.Add(elementoAValidar);
                        sum_ += elementoAValidar;
                    }

                    seguir = false;

                }

                Boolean par2NI = true;
                BigInteger iNI = 0;
                if (divisoresNI.Count > 0)
                { 
                    foreach (BigInteger d in divisoresNI)
                    {                        
                        if (2 == d)
                        {
                            par2NI = !par2NI;
                        }
                        else
                        {
                            iNI++;
                        }
                  
                    }
                }
                else
                {
                    par2NI = true;
                }

                Boolean igualesND = true;
                if (divisoresND.Count > 0)
                {                    
                    BigInteger ant = divisoresND[0];                    
                    foreach (BigInteger d in divisoresND)
                    {
                        if (ant != d)
                        {
                            igualesND = false;
                            break;
                        }
                        ant = d;
                    }

                }

                if (iNI < 2 && !par2NI && nD % 2 != 0 && igualesND)
                {
                    BigInteger sum = nI * nD;
                }


                n += 2;
                nD = n * n;
                nI = nD - 1;
            }
        }

        public static void Nx2masN2divN1()
        {
            BigInteger n = 4;
            BigInteger nAnt = 2;
            BigInteger sum = 0;
            while (true)
            {                
                List<BigInteger> divisores = new List<BigInteger>();
                
                BigInteger elementoAValidar = n;
                Boolean seguir = true;
                BigInteger sum_ = 0;
                while (seguir)
                {
                    BigInteger exacta0 = -1;
                    BigInteger raiz2M3 = raizCuadradaMasUno(elementoAValidar, out exacta0);
                    for (BigInteger i = 2; i < raiz2M3; i++)
                    {
                        BigInteger resto = -1;
                        BigInteger div = BigInteger.DivRem(elementoAValidar, i, out resto);

                        if (resto == 0)
                        {
                            if (div != 1)
                            {
                                divisores.Add(i);
                                sum_ += i;
                                elementoAValidar = div;
                                i = 1;
                            }
                        }
                    }

                    if (elementoAValidar != 1 && elementoAValidar != n)
                    {
                        divisores.Add(elementoAValidar);
                        sum_ += elementoAValidar;
                    }

                    seguir = false;

                }

                foreach (BigInteger d in divisores)
                {
                    sum += d;
                }

                sum += nAnt;

                Boolean esPrimo = true;

                //foreach (BigInteger d in divisores)
                //{
                //    if (sum % d == 0)
                //    {
                //        esPrimo = false;
                //        break;
                //    }
                //}

                //if (sum % divisores.Count == 0)
                //{
                //    esPrimo = false;
                //}

                if (esPrimo)
                {
                    String hola = sum.ToString();
                    //addTextEscritorio(String.Concat(";", result, IsPrime(result) ? " V" : " X"), "Nx2masN2divN1");
                }
                else
                {
                    String hola = "Caracola";
                }
                nAnt = n;
                n+=2;
            }
        }

        public static void numerosPrimos()
        {
            BigInteger n = 1;

            // INICIALIZAMOS AL úTIMO NúMERO IMPAR
            // Recogemos los primos ya procesados si los hubiera
            //using (StreamReader writer = new StreamReader(@"C:\logs\numerosPrimos.txt"))
            //{
            //    String s = writer.ReadLine();
            //    while (s != null)
            //    {
            //        n = BigInteger.Parse(s.Replace(";", ""));
            //        if (!IsPrime(n))
            //        {
            //            BigInteger r = raizCuadrada(n);
            //            BigInteger r3 = raizCuadradaMasTres(n);
            //        }
            //        s = writer.ReadLine();
            //    }
            //}
            n = BigInteger.Parse(ReadLastLine(@"C:\logs\numerosPrimos.txt").Replace(";", ""));
            n = n == 1 ? new BigInteger(3) : n + 2;

            while (true)
            {
                BigInteger exacta0 = 0;
                BigInteger raiz2M3 = raizCuadradaMasTres(n, out exacta0);
                Boolean esPrimo = true;
                for (BigInteger i = 3; i < raiz2M3; i+=2)
                {
                    if (n % i == 0)
                    {
                        esPrimo = false;
                        break;
                    }
                }

                if (esPrimo)
                {
                    addTextEscritorio(String.Concat(";", n), "numerosPrimos");
                }
                n+=2;
            }
        }

        public static string ReadLastLine(string path)
        {
            Stream fs = File.OpenRead(path);
            byte b;
            fs.Position = fs.Length;
            Boolean isFirst = true;
            while (fs.Position > 0)
            {
                fs.Position--;
                b = (byte)fs.ReadByte();
                if (b == '\n' && !isFirst)
                {
                    break;
                }
                isFirst = false;

                fs.Position--;
            }
            byte[] bytes = new byte[fs.Length - fs.Position];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            return Encoding.UTF8.GetString(bytes);
        }
        public static void divCompuestoMas()
        {
            BigInteger n = 2;

            
            BigInteger sumAnt = 0;
            BigInteger sum = 0;
            while (true)
            {
                
                List<BigInteger> divisores = new List<BigInteger>();
                BigInteger elementoAValidar = n;
                Boolean seguir = true;
                while (seguir)
                {
                    BigInteger exacta0 = -1;
                    BigInteger raiz2M3 = raizCuadradaMasUno(elementoAValidar, out exacta0);
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

                    if (elementoAValidar != 1 && elementoAValidar != n)
                    {
                        divisores.Add(elementoAValidar);
                    }

                    seguir = false;

                }

                if (divisores.Count > 0)
                {
                    BigInteger temp = 0;
                    Boolean iguales = true;                    
                    BigInteger ant = divisores[0];
                    BigInteger i = 0;
                    foreach (BigInteger d in divisores)
                    {
                        i++;
                        if (ant != d)
                        {
                            iguales = false;
                        }

                        temp += d;
                    }

                    if (iguales)
                    {
                        sum +=  temp;
                    }
                    else
                    {
                        sum -= temp;
                    }
                }
                else
                {
                    //sum += n;
                }

                if (sumAnt != sum && sum != 0)
                {
                    addTextEscritorio(String.Concat(";", sum, IsPrime(sum) ? " V" : " X"), "divCompuestoMas");
                }
                sumAnt = sum;
                n++;

            }
        }
        public static void divRem()
        {
            BigInteger n = 1;

            
            while (true)
            {
                BigInteger sum = 0;
                BigInteger sumResto = 0;
                for (BigInteger i = n; i > 0; i--)
                {
                    BigInteger resto = 0;
                    BigInteger div = BigInteger.DivRem(n, i, out resto);
                    sum += (n * div) + resto;
                    sumResto += resto;
                }

                //if (sum % 2 != 0 && sum % n != 0)
                //{
                    addTextEscritorio(String.Concat(";", sum, IsPrime(sum) ? " V" : " X"), "divRem");
                //}
                n++;

            }
        }

        public static void factorialMasMenos()
        {
            BigInteger n = 3;

            
            while (true)
            {
                BigInteger sum = factorial(n);
                Boolean suma = true;
                for (BigInteger i = n; i > 0 ; i--)
                {
                    if (suma)
                    {
                        sum += factorial(i);
                    }
                    else
                    {
                        //sum -= factorial(i);
                    }
                    suma = !suma;
                }

                addTextEscritorio(String.Concat(";", sum, IsPrime(sum) ? " V" : " X"), "factorialMasMenos");

                n++;

            }
        }

        public static void factorialINdivIN()
        {
            BigInteger n = 3;

            BigInteger sum = 2;
            while (true)
            {        
                for (BigInteger i = 1; i <= n - 1; i++)
                {
                    sum += factorial(i + n) / factorial(i) / factorial(n);
                }
             
                BigInteger resto = 0;
                BigInteger div = BigInteger.DivRem(sum, n, out resto);
                
                
                addTextEscritorio(String.Concat(";",sum, IsPrime(sum) ? " V" : " X"), "factorialINdivIN");
                
                n += 1;

            }
        }

        public static void piramideEquilatera()
        {
            BigInteger n = 3;
            
            while (true)
            {
                BigInteger pow = n;
                for(BigInteger i = 0; i < n - 2;i++)
                {
                    pow = Potencia(pow, n);
                }
                                
                for (BigInteger i = 0; i < n - 1; i++)
                {
                    pow *= n;
                }
                                
                for (BigInteger i = 0; i < n; i++)
                {
                    pow += n;
                }


                BigInteger r = pow;

                if (r % 2 != 0 && r % n != 0)
                {
                    addTextEscritorio(String.Concat(";", r, IsPrime(r) ? " V" : " X"), "piramideEquilatera");                    
                }
                n+=2;
                
            }
        }

        public static void base4()
        {
            BigInteger n = 5;


            while (true)
            {
                String base6s = FromBase10(n.ToString(), 6);

                BigInteger sumCFirst = 0;
                BigInteger sumC = 0;
                Boolean isFirst = true;
                foreach (Char c in base6s)
                {
                    Int16 intI = Convert.ToInt16(c.ToString());
                    sumC += intI;
                    sumCFirst += intI;

                    if (isFirst)
                    {
                        isFirst = false;
                        sumCFirst += intI;
                    }
                }
                String nS = n.ToString();
                String sumCS = sumC.ToString();
                if (nS[nS.Length - 1] != sumCS[sumCS.Length - 1] && nS[nS.Length - 1] != base6s[base6s.Length - 1] && n % Convert.ToInt16(base6s[base6s.Length - 1].ToString()) != 0 && sumC % 3 != 0)
                {
                    addTextEscritorio(String.Concat(";", n, IsPrime(n) ? " V" : " X"), "base4");
                }

                n += 2;
            }
        }

        public static void elFantasmaDel5()
        {
            BigInteger cuadrado = 25;
            BigInteger nCuadrado = 5;
            BigInteger n = 7;
            

            while (true)
            {
                BigInteger resto = 0;
                BigInteger div = BigInteger.DivRem(n, nCuadrado, out resto);

                BigInteger unoMenosUno = div - resto;

                if(unoMenosUno == 1 || unoMenosUno == -1)                
                {
                    addTextEscritorio(String.Concat(";", n, IsPrime(n) ? " V" : " X"), "elFantasmaDel5");
                }
                
                n+=2;

                if(n > cuadrado + nCuadrado)
                {
                    nCuadrado *= nCuadrado;
                    cuadrado = nCuadrado * nCuadrado;
                }

                    
            }
        }
        public static void puntosX2X3()
        {
            interseccionSegmentos(5, 25, 6, 36, 3, 27, 4, 64);
        }

        public static double[] interseccionSegmentos(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {

            // OTRO
            double ua = (((x4 - x3) * (y1 - y3)) - ((y4 - y3) * (x1 - x3))) / (((y4 - y3) * (x2 - x1)) - ((x4 - x3) * (y2 - y1)));

            double x = x1 + ua * (x2 - x1);
            double y = y1 + ua * (y2 - y1);

            return new double[] { x, y };

        }
        public static void losAnillosCrecienteDP()
        {
            List<BigInteger> primos = new List<BigInteger>();
                                    
            BigInteger n = 1;
            BigInteger n6 = 2;
            BigInteger c6 = 6;

            while (true)
            {                                                                
                addTextEscritorio(String.Concat(";", n, IsPrime(n) ? " V" : " X"), "losAnillosCrecienteDP");

                n += c6 - 2;

                addTextEscritorio(String.Concat(";", n, IsPrime(n) ? " V" : " X"), "losAnillosCrecienteDP");

                n += 2;

                c6 = 6 * n6;
                n6 *=c6;

            }
        }
        public static void sumImparesMayoresMasTMenis()
        {
            BigInteger n = 3;

            while (true)
            {
                BigInteger result = n;

                
                for (BigInteger i = n + 2; i < n * n; i += 2)
                {
                    result *= i;
                }

                BigInteger pares = 1;
                for (BigInteger j = (n * n) -1; j > 1; j-=2)
                {
                    pares *= j;
                }

                result += pares;
                result += n + (n - 2);

                //BigInteger minNum = 0;

                //BigInteger div = (minNum) / 2;

                //BigInteger resto2 = 0;
                //BigInteger div2 = BigInteger.DivRem(div, 2, out resto2); ;

                //if (resto2 == 0)
                //{
                //    result += (minNum - 1 + 2) * div2;
                //}
                //else
                //{
                //    result += ((minNum - 1 + 2) * div2) + div + 1;
                //}

                addTextEscritorio(String.Concat(";", result, IsPrime(result) ? " V" : " X"), "sumParesMayores");


                n += 2;
            }
        }

        public static void sumParesMenores()
        {            
            BigInteger n = 3;
                        
            while (true)
            {
                BigInteger result = n;

                BigInteger minNum = 0;
                for (BigInteger i = n + 2;i < n * n; i+=2)
                {
                    minNum = n - (2 * i);
                    result += minNum;
                }

                BigInteger div = (minNum) / 2;

                BigInteger resto2 = 0;
                BigInteger div2 = BigInteger.DivRem(div, 2, out resto2); ;

                if (resto2 == 0)
                {
                    result += (minNum - 1 + 2) * div2;
                }
                else
                {
                    result += ((minNum - 1 + 2) * div2) + div + 1;
                }

                addTextEscritorio(String.Concat(";", result, IsPrime(result) ? " V" : " X"), "sumParesMenores");
                

                n+=2;                
            }
        }
        public static void piramideCreciente()
        {
            Dictionary<BigInteger, BigInteger> multiplos = new Dictionary<BigInteger, BigInteger>();

            BigInteger n = 1;
            BigInteger hueco = 1; 
            BigInteger resto = 0;

            while (true)
            {
                n+=2;
                

                if (resto < n)
                {
                    hueco += 3;
                    resto = hueco - (n - resto);
                }
                else
                {
                    resto -= n; 
                }

                if(resto == 0)
                {
                    addTextEscritorio(String.Concat(";", n, IsPrime(n) ? " V" : " X"), "piramideCreciente");
                }

                
            }
        }
        public static void crecimientoControlado()
        {
            Dictionary<BigInteger, BigInteger> multiplos = new Dictionary<BigInteger, BigInteger>();

            BigInteger n = 2;

            BigInteger mult = 1;

            while (true)
            {
                if (mult % n != 0)
                {
                    mult *= n;

                    addTextEscritorio(String.Concat(";", n, IsPrime(n) ? " V" : " X"), "primos");
                }
                else
                {
                    BigInteger resto = 0;
                    BigInteger div = BigInteger.DivRem(n, 2, out resto);
                    if (resto == 0)
                    {
                        mult *= div;
                    }
                    else
                    {
                        Boolean enContrado = false;
                        for (BigInteger i = 3; i < n; i += 2)
                        {
                            resto = 0;
                            div = BigInteger.DivRem(n, i, out resto);

                            if (resto == 0)
                            {
                                enContrado = true;
                                mult *= div;
                                break;
                            }

                            if(!enContrado)
                            {
                                mult *= n;
                            }
                        }
                    }
                }

                n++;
            }
        }
        public static void sumSum()
        {
            Dictionary<BigInteger, BigInteger> multiplos = new Dictionary<BigInteger, BigInteger>();

            BigInteger n = 2;
            //BigInteger sum = 2;
            
            while (true)
            {
                BigInteger nM1 = n + 1;

                BigInteger sum = (factorial(2 * n + 1) - factorial(nM1)) / (factorial(nM1) * factorial(n));


                //for (BigInteger i = 1; i < n; i++)
                //{
                //    sum += (factorial(i + n) / factorial(i) / factorial(n));
                //}

                Boolean esPrimo = true;
                KeyValuePair<BigInteger, BigInteger> primerM = multiplos.FirstOrDefault();
                if (primerM.Value == nM1)
                {
                    esPrimo = false;
                    multiplos[primerM.Key] = primerM.Value * primerM.Key;
                    List<KeyValuePair<BigInteger, BigInteger>> orden = multiplos.ToList();
                    orden.Sort((x, y) => x.Value.CompareTo(y.Value));
                    multiplos = orden.ToDictionary(k => k.Key, v => v.Value);

                }

                if (esPrimo && sum % nM1 == 0)
                {
                        multiplos.Add(nM1, nM1 * nM1);
                        List<KeyValuePair<BigInteger, BigInteger>> orden = multiplos.ToList();
                        orden.Sort((x, y) => x.Value.CompareTo(y.Value));
                        multiplos = orden.ToDictionary(k => k.Key, v => v.Value);
                        addTextEscritorio(String.Concat(";", nM1, IsPrime(nM1) ? " V" : " X"), "primos");
                    
                }

                n++;
            }
        }

        public static void conPiramidalSumN()
        {
            List<BigInteger[]> posiblePrimo = new List<BigInteger[]>();

            List<BigInteger> piram = new List<BigInteger> { 7, 21, 35};
                        
            BigInteger i = 7;
            Boolean unaSiOtraNo = true;
            //BigInteger divisor = 3;
            //BigInteger tetaDivisor = 27;
            while (true)
            {
                if (unaSiOtraNo)
                {
                    posiblePrimo.Add(new BigInteger[] { i, (i / 3) });

                    List<BigInteger[]> eliminarPrimo = new List<BigInteger[]>();
                    foreach (BigInteger[] pP in posiblePrimo)
                    {
                        if (piram[piram.Count - 1] % pP[0] != 0)
                        {
                            eliminarPrimo.Add(pP);
                            continue;
                        }

                        pP[1]--;

                        if (pP[1] == 0)
                        {
                            eliminarPrimo.Add(pP);
                            addTextEscritorio(String.Concat(";", pP[0], IsPrime(pP[0]) ? " V" : " X"), "primos");
                            
                        }
                    }
                    posiblePrimo.RemoveAll(x => eliminarPrimo.Contains(x));                                        
                }       

                if(unaSiOtraNo)
                {
                    piram.Add(piram[piram.Count - 1] * 2);
                }
                for (Int32 j = piram.Count - (unaSiOtraNo ? 2 : 1); j > 0; j--)
                {
                    piram[j] += piram[j - 1];
                }
                piram[0]++;

                unaSiOtraNo = !unaSiOtraNo;
                i++;
                //if (i == tetaDivisor)
                //{
                //    divisor ++;
                //    tetaDivisor = Potencia(divisor, divisor);
                //}
            }
        }

        public static void conBiCoeff()
        {
            List<BigInteger[]> posiblePrimo = new List<BigInteger[]>();
                       
            
            BigInteger n = 2;
            BigInteger i = 5;

            while (true)
            {
                BigInteger result = BinCoeff(2 * n + 1, n + 1);

                posiblePrimo.Add(new BigInteger[] { i, i / 2 });

                List<BigInteger[]> eliminarPrimo = new List<BigInteger[]>();
                foreach (BigInteger[] pP in posiblePrimo)
                {
                    if (result % pP[0] != 0)
                    {
                        eliminarPrimo.Add(pP);
                        continue;
                    }

                    pP[1]--;

                    if (pP[1] == 0)
                    {
                        eliminarPrimo.Add(pP);
                        addTextEscritorio(String.Concat(";", pP[0], IsPrime(pP[0]) ? " V" : " X"), "primos");
                    }
                }

                posiblePrimo.RemoveAll(x => eliminarPrimo.Contains(x));
                
                n++;
                i += 2;
            }
        }

        public static BigInteger BinCoeff(BigInteger n, BigInteger k)
        {
            if (k > n) { return 0; }
            if (n == k) { return 1; } // only one way to chose when n == k
            if (k > n - k) { k = n - k; } // Everything is symmetric around n-k, so it is quicker to iterate over a smaller k than a larger one.
            BigInteger c = 1;
            for (BigInteger i = 1; i <= k; i++)
            {
                c *= n--;
                c /= i;
            }
            return c;
        }

        public static void pascalDiagonal()
        {            
            List<BigInteger[]> posiblePrimo = new List<BigInteger[]>();

            BigInteger nFactPor2 = 24;
            BigInteger nFactMenos1 = 2;
            BigInteger n2 = 4;
            BigInteger n = 2;

            while (true)
            {
                BigInteger result = nFactPor2 / (nFactMenos1 * nFactMenos1);

                posiblePrimo.Add(new BigInteger[] { n2 - 1, (n2 - 1) / 3 });

                List<BigInteger[]> eliminarPrimo = new List<BigInteger[]>();
                foreach (BigInteger[] pP in posiblePrimo)
                {
                    if (result % pP[0] != 0)
                    {
                        eliminarPrimo.Add(pP);
                        continue;
                    }

                    pP[1]--;

                    if (pP[1] == 0)
                    {
                        eliminarPrimo.Add(pP);
                        addTextEscritorio(String.Concat(";", pP[0], IsPrime(pP[0]) ? " V" : " X"), "primos");
                    }
                }

                posiblePrimo.RemoveAll(x => eliminarPrimo.Contains(x));

                n++;
                nFactMenos1 *= n;
                n2++;                
                nFactPor2 *= n2;
                n2++;
                nFactPor2 *= n2;
            }
        }
        public static void probatura()
        {
            BigInteger n = 5;
            BigInteger result = 9;
            BigInteger nInferior = 2;
                        
            while (true)
            {                
                result += n + n + 1 - nInferior;
                nInferior++;
                
                if (result % 9 == 0)
                {
                    BigInteger div = result / 9;
                    Char c0 = div.ToString()[0];
                    Boolean iguales = true;
                    foreach (Char c in div.ToString())
                    {
                        if (c0 != c)
                        {
                            iguales = false;
                        }
                        c0 = c;
                    }
                    if (iguales)
                    {
                        addTextEscritorio(String.Concat(";", n + 2, IsPrime(n + 2) ? " V" : " X"), "estamosPreparadosPregunta");
                    }
                }
                n += 2;
            }
        }

        public static void sumatorioAntN()
        {   
            List<BigInteger> compuestos = new List<BigInteger>();
            Dictionary<BigInteger, BigInteger> multiplos = new Dictionary<BigInteger, BigInteger>();
            BigInteger n = 5;
            BigInteger result = 9;
            BigInteger nInferior = 2;
            while (true)
            {

                compuestos.Add(result);

                result += n + n + 1 - nInferior;
                nInferior++;

                Boolean esPrimo = true;

                List<BigInteger> aEliminar = new List<BigInteger>();
                foreach (BigInteger pC in compuestos)
                {
                    if (pC < n)
                    {
                        aEliminar.Add(pC);
                    }

                    if (pC % n == 0)
                    {                        
                        esPrimo = false;
                        break;
                    }
                }
                compuestos.RemoveAll(x => aEliminar.Contains(x));

                KeyValuePair<BigInteger, BigInteger> primerM = multiplos.FirstOrDefault();
                if (esPrimo && primerM.Value == n)
                {
                    esPrimo = false;
                    multiplos[primerM.Key] = primerM.Value * primerM.Key;
                    List<KeyValuePair<BigInteger, BigInteger>> orden = multiplos.ToList();
                    orden.Sort((x, y) => x.Value.CompareTo(y.Value));
                    multiplos = orden.ToDictionary(k => k.Key, v => v.Value);

                }

                if (esPrimo)
                {
                    multiplos.Add(n, n * n );

                    addTextEscritorio(String.Concat(";", n, IsPrime(n) ? " V" : " X"), "paso2");
                }

                n+=2;
            }
        }

        public static void sumatorioImpar()
        {
            SortedSet<BigInteger> pCruz = new SortedSet<BigInteger>() { 3, 5 };
            
            BigInteger n = 3;
            BigInteger sumN = 1;

            while (true)
            {
                sumN += n;

                BigInteger posiblePC = n + sumN;

                if (posiblePC % 2 == 0)
                {
                    posiblePC += n - 2;
                }

                Boolean seguir = false;

                

                do
                {
                    BigInteger maxN = posiblePC / 3;
                    seguir = false;
                    foreach (BigInteger pC in pCruz)
                    {
                        if (pC > maxN)
                        {
                            seguir = false;
                            break;
                        }
                        else
                        {

                            if (posiblePC % pC == 0)
                            {
                                seguir = true;
                                posiblePC = posiblePC / pC;
                                if (posiblePC == 1)
                                {
                                    seguir = false;
                                }
                                break;
                            }
                        }
                    }

                    if (!seguir && posiblePC != 1)
                    {
                        pCruz.Add(posiblePC);
                        addTextEscritorio(String.Concat(";", posiblePC, IsPrime(posiblePC) ? " V" : " X"), "sumatorioImpar");
                    }

                }
                while (seguir);
                
                n+=2;
            }
        }

        public static void conjeturaCruz()
        {
            List<BigInteger> pCruz = new List<BigInteger>() { 3, 5 };

            BigInteger n = 2;

            BigInteger sumN = 0;
            
            
            while (true)
            {
                BigInteger posiblePC = 0;

                if (n % 2 == 0)
                {
                    sumN += n;
                }
                else
                {
                    posiblePC = n + sumN;
                    Boolean esPrimo = true;
                    foreach (BigInteger pC in pCruz)
                    {
                        if (posiblePC % pC == 0)
                        {
                            esPrimo = false;
                            break;
                        }
                    }

                    if (esPrimo)
                    {
                        pCruz.Add(posiblePC);
                        addTextEscritorio(String.Concat(";", posiblePC, IsPrime(posiblePC) ? " V" : ""), "conjeturaCruz");
                    }

                    posiblePC = (n * n) + sumN;
                    esPrimo = true;
                    foreach (BigInteger pC in pCruz)
                    {
                        if (posiblePC % pC == 0)
                        {
                            esPrimo = false;
                            break;
                        }
                    }

                    if (esPrimo)
                    {
                        pCruz.Add(posiblePC);
                        addTextEscritorio(String.Concat(";", posiblePC, IsPrime(posiblePC) ? " V" : ""), "conjeturaCruz");
                    }
                }

                n++;             
            }
        }

        /// <summary>Dedicado a mi abuela Cruz, por su sabiduría y ayuda desde el otro lado ;) nos vemos pronto!</summary>
        public static void conjeturaCruzAnt()
        {
            List<BigInteger> pCruz = new List<BigInteger>();
                        
            BigInteger sumN = 1;
            BigInteger cruz = 1;
            BigInteger n = 1;
            while (true)
            {
                n++; 
                cruz += cruz;

                BigInteger posiblePC = cruz + sumN;

                Boolean esPrimo = true;
                foreach (BigInteger pC in pCruz)
                {
                    if (posiblePC % pC == 0)
                    {
                        esPrimo = false;
                        break;
                    }
                }

                if (esPrimo)
                {
                    pCruz.Add(posiblePC);
                    addTextEscritorio(String.Concat(";", posiblePC, IsPrime(posiblePC) ? " V" : ""), "conjeturaCruz");
                }

                sumN += cruz;

                posiblePC = cruz + sumN;

                esPrimo = true;
                foreach (BigInteger pC in pCruz)
                {
                    if (posiblePC % pC == 0)
                    {
                        esPrimo = false;
                        break;
                    }
                }

                if (esPrimo)
                {
                    pCruz.Add(posiblePC);
                    addTextEscritorio(String.Concat(";", posiblePC, IsPrime(posiblePC) ? " V" : ""), "conjeturaCruz");
                }
            }
        }

        private static void cromosomaPrimo()
        {
            
            //List<BigInteger[]> listado = new List<BigInteger[]>() { new BigInteger[] { 2, 1, 2 } };

            BigInteger n = 4;
                        
            BigInteger orden = 0;

            while (true)
            {
                BigInteger result = n + n - 1 + n - 1;

                if (result % 2 != 0 && result % orden != 0)
                {
                    addTextEscritorio(String.Concat(";", result, IsPrime(result) ? " V" : ""), "cromosomaPrimo");
                }
                n++;
                orden++;
            }

            //if (true)
            //{
            //    foreach (BigInteger[] linea in listado)
            //    {
            //        addTextEscritorio(String.Concat(linea[0]," ", linea[1]," ", linea[2]), "cromosomaPrimo");
            //    }
                
            //}

        }

        private static BigInteger combinacionesNumero(BigInteger numero)
        {
            BigInteger combinaciones = 0;

            for (BigInteger i = 1; i <= numero; i++)
            {
                if (i == 1 || i == numero)
                {
                    combinaciones++;
                }
                else
                {
                    List<BigInteger> posibilidades = new List<BigInteger>();
                    for (BigInteger j = 1; j <= numero ; j++)
                    {
                        posibilidades.Add(j);
                    }

                    List<List<BigInteger>> aValorar = GetCombinations(posibilidades, Convert.ToInt32(i.ToString()));

                    foreach (List<BigInteger> lAV in aValorar)
                    {
                        BigInteger suma = 0;
                        BigInteger nIgual = 0;
                        Boolean iguales = true;
                        foreach (BigInteger n in lAV)
                        {
                            suma += n;

                            if(nIgual == 0 || !iguales)
                            {
                                nIgual = n;
                            }
                            else
                            {
                                if (nIgual != n)
                                {
                                    iguales = false;
                                }
                            }

                        }

                        if (suma == numero)
                        {
                            combinaciones++;

                            if (iguales)
                            {
                                combinaciones++;
                            }
                        }

                        

                    }

                }
            }

            return combinaciones;
        }

        private static List<List<BigInteger>> GetCombinations<BigInteger>(List<BigInteger> list, Int32 length)
        {
            if (length == 1) return list.Select(t => new List<BigInteger> { t }).ToList();

            return GetCombinations(list, length - 1)
                .SelectMany(t => list, (t1, t2) => t1.Concat(new BigInteger[] { t2 }).ToList()).ToList();
        }

        private static List<List<String>> GetCombinationsST<String>(List<String> list, Int32 length)
        {
            if (length == 1) return list.Select(t => new List<String> { t }).ToList();

            return GetCombinations(list, length - 1)
                .SelectMany(t => list, (t1, t2) => t1.Concat(new String[] { t2 }).ToList()).ToList();
        }

        private static void rapid()
        {
            BigInteger elNumero = 761;
                                    
            while (true)
            {
                BigInteger compose = 0;
                                
                foreach (Char c in elNumero.ToString())
                {
                    compose += BigInteger.Parse(c.ToString());
                }

                var sb = new StringBuilder();
                Char cAnt = ' ';
                Boolean isFirst = true;
                Boolean equal = true;
                foreach (var c in compose.ToString().Reverse())
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        cAnt = c;
                    }
                    else
                    {
                        if (cAnt != c)
                        {
                            equal = false;
                        }
                    }

                    sb.Append(c);
                }

                BigInteger equalSum = 0;
                if (equal)                {
                    
                    foreach (var c in compose.ToString().Reverse())
                    {

                        equalSum += BigInteger.Parse(c.ToString());
                        
                    }
                }
                

                BigInteger total = BigInteger.Parse(
                    (compose % 10 == 0 ? compose.ToString() : 
                    equal ? equalSum.ToString() : sb.ToString()) + elNumero.ToString());

                addTextEscritorio(String.Concat("; total:", total, IsPrime(total) ? " V" : ""),
                        "rapid");

                elNumero = total;
            }

        }
        private static void nDiagonalPoligonoRegularParImpar()
        {
            List<BigInteger> primos = new List<BigInteger>();

            BigInteger iPoligono = 4;
            BigInteger i = 1;
            BigInteger iSum = 0;
            BigInteger totalAnt = 0;
            while (true)
            {
                //BigInteger check = resPI + i;

                //if(check % 2 != 0)
                //{
                //    addTextEscritorio(String.Concat(";", i, " + ", resPI, " = ", check,
                //    IsPrime(check) ? " V" : "  "), "nDiagonalPoligonoRegularParImpar");
                //}

                iSum += i;
                BigInteger res = (iPoligono * (iPoligono - 3)) / 2;

                BigInteger total = (res * iSum) + ((res + 1) - (iSum - 1));
                                                
                if (true)
                {
                    
                    addTextEscritorio(String.Concat(";res: ", res, " i: ", iSum, " total ", total, IsPrime(total) ? " V" : ""),
                        "nDiagonalPoligonoRegularParImpar");
                }

                iPoligono++;
                i++;
            }
        }

        private static void iImparMenosCuadrados()
        {
            BigInteger i = 3;
            BigInteger iSumImpar = 1;
            BigInteger iSumSumImpar = 1;
            BigInteger iSumPar = 2;
            BigInteger iSumSumPar = 2;
            List<BigInteger> cuadrados = new List<BigInteger>();

            while (true)
            {
                iSumImpar += i;
                iSumSumImpar += iSumImpar;

                iSumPar += i + 1;
                iSumSumPar += iSumPar;

                cuadrados.Add(iSumImpar);

                if (iSumSumImpar % (i + 2) == 0 && !cuadrados.Contains(i + 2))
                {
                    addTextEscritorio(String.Concat(";", (i + 2), IsPrime(i + 2) ? " V " : "   ", " SumI: " + iSumImpar, " SumSumI: " + iSumSumImpar, " SumP: " + iSumPar, " SumSumP: " + iSumSumPar), "iImparMenosCuadrados");

                }

                i+=2;
            }
        }

        public static void iSumMultAdd()
        {
            BigInteger i = 2;
            BigInteger iMult = 1;
            BigInteger i1;

            while (true)
            {
                iMult *= i;

                i1 = ((iMult + i) * 3) - 1;

                if (true)
                {
                    addTextEscritorio(String.Concat(";", i1, IsPrime(i1) ? " V" : ""), "iSumMultAdd");
                    
                }

                i++;
            }
        }

        public static void iHastaElInfinitoYMasAlla()
        {
            //BigInteger primo = new BigInteger();

            BigInteger i = 1;
            
            List<List<BigInteger>> mults = new List<List<BigInteger>>() { new List<BigInteger>() { i * (i + 1) } };
            List<BigInteger> sums = new List<BigInteger>() { i + i + 1 };

            //primo = mults[0][0] + sums[0];

            i++;

            while (true)
            {
                BigInteger sum = i + i + 1;
                sums.Add(sum);

                BigInteger mult = i * (i + 1);
                mults[0].Add(mult);

                List<List<BigInteger>> multsTemp = mults.ToList();

                Int32 iC = 0;
                BigInteger addM = 1;
                foreach (List<BigInteger> lM in multsTemp)
                {
                    if (addM != 1)
                    {
                        mults[iC].Add(addM);
                        addM = 1;
                    }

                    Int32 jC = 0;
                    BigInteger mAntAnt = 1;
                    BigInteger mAnt = 1;
                    foreach (BigInteger m in lM)
                    {
                        mAntAnt = mAnt;
                        mAnt = m;
                        jC++;
                    }

                    if (mAntAnt == 1 || mAnt == 1) { break; }

                    addM = mAntAnt * mAnt;

                    iC++;
                }

                if (addM != 1)
                {
                    mults.Add(new List<BigInteger>() { addM });
                }

                if (sums.Count % 2 != 0)
                {
                    //BigInteger s = sums[sums.Count / 2];

                    BigInteger total = 0;

                    foreach (List<BigInteger> lM in mults)
                    {
                        foreach (BigInteger m in lM)
                        {
                            total += m;
                        }
                    }

                    foreach (BigInteger s in sums)
                    {
                        total += s;
                    }


                    BigInteger subTotal = 1;
                    List<BigInteger> temp = new List<BigInteger>();
                    BigInteger mAntAnt = 1;
                    BigInteger mAnt = 1;
                    foreach (BigInteger t in sums)
                    {
                        mAntAnt = mAnt;
                        mAnt = t;

                        if (mAntAnt != 1 && mAnt != 1)
                        {
                            subTotal *= mAntAnt + mAnt;
                            temp.Add(mAntAnt + mAnt);
                        }
                    }
                                        
                    List<BigInteger> tempTemp = new List<BigInteger>();
                    while(true)
                    {
                        mAntAnt = 1;
                        mAnt = 1;

                        foreach (BigInteger t in temp)
                        {
                            mAntAnt = mAnt;
                            mAnt = t;

                            if (mAntAnt != 1 && mAnt != 1)
                            {
                                subTotal *= mAntAnt + mAnt;
                                tempTemp.Add(mAntAnt + mAnt);
                            }
                        }

                        if (mAntAnt == 1 || mAnt == 1) { break; }

                        temp = tempTemp.ToList();
                        tempTemp.Clear();
                    }

                    //subTotal *= mAntAnt * mAnt;

                    total += subTotal;

                    if (true)
                    {
                        //primo = total;
                        addTextEscritorio(String.Concat(";", total, IsPrime(total) ? " V" : ""), "iHastaElInfinitoYMasAlla");
                    }
                }
                i++;
            }
        }

        public static void iSumaRara()
        {
            BigInteger i = 22;

            BigInteger[][] matriz = new BigInteger[5][];
            matriz[0] = new BigInteger[5] { 1, 3, 5, 7, 9 };
            matriz[1] = new BigInteger[5] { 2, 4, 6, 8, 10 };
            matriz[2] = new BigInteger[5] { 11, 13, 15, 17, 19 };
            matriz[3] = new BigInteger[5] { 12, 14, 16, 18, 20 };
            matriz[4] = new BigInteger[5] { 21, 23, 25, 27, 29 };

            while (true)
            {
                BigInteger sum = 0;                
                sum += matriz[0][4];
                sum += matriz[1][3];
                sum += matriz[2][2];
                sum += matriz[3][1];
                sum += matriz[4][0];

                addTextEscritorio(String.Concat(sum, IsPrime(sum) ? " V" : ""), "sumaRARA");
                                            
                BigInteger[] UP = new BigInteger[5];
                BigInteger[] DOWN = new BigInteger[5];
                for (Int16 j = 0; j < 5; j++)
                {
                    UP[j] = i;                    
                    DOWN[j] = i + 9;
                    
                    i+=2;
                }

                matriz[0] = matriz[2];
                matriz[1] = matriz[3];
                matriz[2] = matriz[4];
                matriz[3] = UP;
                matriz[4] = DOWN;


            }
        }

        public static void iSumaFina()
        {
            BigInteger numero = 1;
            BigInteger catchN = 2;

            while (true)
            {

                BigInteger sum = 0;

                for (BigInteger i = 0; i < catchN; i++)
                {
                    sum += numero;
                    numero += 105;
                }

                sum += 52772;

                if (!IsPrime(sum))
                {
                    addTextEscritorio(String.Concat(sum), "prri\\_105_FInd" + 52772);
                }
                else
                {
                    addTextEscritorio(String.Concat(sum, " V"), "prri\\_105_FInd" + 52772);
                }
            }
        }

        public static void iSuma123()
        {            
            BigInteger numero = 1;
            BigInteger catchN = 2;
            BigInteger nSum = 1;
            BigInteger nNumero = 2;
            while (true)
            {
                Task.Run(() => honolulu(catchN, numero, nNumero, nSum));

                nSum++;

                if (nSum == 10000)
                {
                    nNumero++;
                    nSum = 1;

                    if(nNumero > 10000)
                    {
                        nNumero = 2;
                        catchN++;
                    }
                }



            }
        }

        public static void honolulu(BigInteger catchN, BigInteger numero, BigInteger nNumero, BigInteger nSum)
        {
            Int32 catchContador = 20;
            Int32 seguirContador = 20;
            Boolean seguir = true;
            while (seguir)
            {

                BigInteger sum = 0;

                for (BigInteger i = 0; i < catchN; i++)
                {
                    sum += numero;
                    numero += nNumero;
                }

                sum += nSum;

                if (!IsPrime(sum))
                {
                    seguirContador--;
                }
                else
                {
                    catchContador--;
                }

                if (seguirContador == 0)
                {
                    numero = 1;
                    seguir = false;
                    break;
                }

                if (catchContador == 0 && seguirContador > 19)
                {
                    FileWriter.WriteDesktop(String.Concat("EUREKA"), "prri\\" + seguirContador + "___nNumero_" + nNumero + "___nSum_" + nSum + "___catch_" + catchN);
                    //addTextEscritorio(String.Concat("EUREKA"), "prri\\" + seguirContador + "___nNumero_" + nNumero + "___nSum_" + nSum + "___catch_" + catchN);
                    seguir = false;
                    break;
                }
            }
        }

        public static void caminos()
        {            
            BigInteger i = 1;
                        
            while (true)
            {
                BigInteger sum = i;

                for (BigInteger j = i -1; j > 0; j--)
                {
                    sum += i * j;
                }

                if (true)
                {
                    addTextEscritorio(String.Concat("= ", i, " > ", sum), "caminos");
                }

                i++;
            }
        }

        public static void sumaCero()
        {            
            List<BigInteger> array = new List<BigInteger>() { 1 };
            List<BigInteger> arrayTemp = new List<BigInteger>() { };
            BigInteger sum2 = 0;
            while (true)
            {
                BigInteger nTemp = 0;
                for (Int32 j = 0; j < array.Count; j++)
                {
                    nTemp = array[j];
                    arrayTemp.Add(nTemp + j + 1);
                }

                arrayTemp.Add(nTemp + array.Count + 1);

                

                if (true) 
                {
                    BigInteger sum = 0;
                    String linea = String.Empty;
                    foreach (BigInteger n in array)
                    {
                        sum += n;
                        linea += String.Concat(n.ToString(), ", ");
                    }
                    sum2 += sum;
                    linea += String.Concat(" = ", sum.ToString());
                    linea += String.Concat(" = ", sum2.ToString());


                    addTextEscritorio(linea, "sumaCero");
                    
                }

                array = arrayTemp.ToList();
                arrayTemp.Clear();
            }
        }

        public static void trianguloPablo()
        {

            List<BigInteger> lista = new List<BigInteger>() { 2, 3 };
            List<BigInteger> listaTemp = new List<BigInteger>();

            BigInteger sum = 6;
            while (true)
            {
                
                BigInteger ant = 0;
                foreach (BigInteger n in lista)
                {
                    if(ant == 0)
                    {
                        listaTemp.Add(n + 2);
                        sum += n + 2;
                    }
                    else
                    {
                        BigInteger result = ant - n;
                        result *= result < 0 ? -1 : 1;
                        sum += result;
                        listaTemp.Add(result);
                    }

                    ant = n;
                }

                listaTemp.Add(ant + 2);
                sum += ant + 2;


                if (true) //(rem != 0)
                {
                    addTextEscritorio(String.Concat(";", sum.ToString(), IsPrime(sum) ? " V" : " X"), "trianguloPablo");
                }


                lista = listaTemp.ToList();
                listaTemp.Clear();

            }
        }


        public static void priConcat()
        {
            BigInteger iDei = 1;            
            
            BigInteger ant = 0;
            while (true)
            {
                String concat = String.Empty;
                for (BigInteger j = 0; j < iDei; j++)
                {
                    concat += iDei.ToString();
                }
                                
                BigInteger num = BigInteger.Parse(concat);
                BigInteger result = (num * iDei * iDei) + ((iDei - 1) * ant);

                
                if (true) //(rem != 0)
                {
                    addTextEscritorio(String.Concat(";", iDei, ";", result.ToString(), IsPrime(result) ? " V" : " X"), "priConcat");                       
                }

                ant = num;
                iDei += 1;
            }
        }

        public static void priEx2()
        {
            List<BigInteger> primos = new List<BigInteger>() { 7 };

            BigInteger iDei = 3;
            
            BigInteger ant = 8;
            while (true)
            {
                BigInteger pot = Potencia(iDei, 3);
                BigInteger result = pot - ant;
                ant = pot;

                Boolean esPrimo = true;
                foreach (BigInteger p in primos)
                {
                    BigInteger rem = 0;
                    BigInteger div = BigInteger.DivRem(result, p, out rem);

                    if (rem == 0)
                    {
                        esPrimo = false;
                        descomposicionFactorialPrimos(result, p, primos);
                        break;
                    }
                }

                if (esPrimo)
                {
                    primos.Add(result);
                    addTextEscritorio(String.Concat(";", result.ToString(), IsPrime(result) ? " V" : " X"), "priEx");                    
                }

                iDei++;
            }
        }

        public static void priEx()
        {
            List<BigInteger> primos = new List<BigInteger>();

            BigInteger iDei = 2;
            //BigInteger iDeImpar = 3;

            //BigInteger sum = 1;

            while (true)
            {
                //sum += (2 * BigInteger.Pow(iDei, 2)) - 1;

                BigInteger result = Potencia(iDei, 2) + iDei - 1;

                Boolean esPrimo = true;
                foreach (BigInteger p in primos)
                {
                    BigInteger rem = 0;
                    BigInteger div = BigInteger.DivRem(result, p, out rem);

                    if (rem == 0)
                    {
                        esPrimo = false;
                        descomposicionFactorialPrimos(result, p, primos);
                        break;
                    }
                }
                                
                if (esPrimo)
                {
                    primos.Add(result);
                    addTextEscritorio(String.Concat(";", result.ToString(), IsPrime(result) ? " V" : " X"), "priEx");
                    //posible.Add(iDeImpar);
                }
                


                //BigInteger rem2 = 0;
                //BigInteger div2 = BigInteger.DivRem(sum, iDei, out rem2);

                //if(rem2 == 0)
                //{
                //    if(posible.Contains(iDei))
                //    {
                //        posible.Remove(iDei);
                        
                //    }
                //}
                
                iDei++;
                //iDeImpar += 2;
            }
        }

        public static void espiralTouche()
        {
            BigInteger i = 1;
                        
            List<BigInteger> divisores = new List<BigInteger>();
                        
            BigInteger iMatrix = 12;
            BigInteger jMatrix = 12;
            BigInteger x = 1;
            Int32 iX = 1;
            Int32 maxIX = 2;
            Int16 direccion = 1;
            
            i += 1;
            while (true)
            {
                try
                {
                    if (direccion == 1)
                    {
                        BigInteger iMatrixMax = iMatrix + x;
                        while (iMatrix < iMatrixMax)
                        {
                            iMatrix++;                            
                            i += 1;
                        }

                        direccion++;
                    }
                    else if (direccion == 2)
                    {
                        BigInteger jMatrixMax = jMatrix + x;
                        while (jMatrix < jMatrixMax)
                        {
                            jMatrix++;
                            
                            i += 1;
                        }

                        BigInteger numAnterior = i - 1;

                        Boolean esPrimo = true;
                        foreach (BigInteger d in divisores)
                        {
                            if (numAnterior % d == 0)
                            {
                                esPrimo = false;
                                //descomposicionFactorialPrimos(numAnterior, d, divisores);
                                break;
                            }
                        }

                        if (esPrimo)
                        {
                            divisores.Add(numAnterior);
                            addTextEscritorio(String.Concat(";", numAnterior.ToString(), IsPrime(numAnterior) ? " V": " X"), "priEspiralDiagonalDecreciente");
                        }

                        direccion++;
                    }
                    else if (direccion == 3)
                    {
                        BigInteger iMatrixMin = iMatrix - x;
                        while (iMatrix > iMatrixMin)
                        {
                            iMatrix--;                            
                            i += 1;
                        }

                        direccion++;
                    }
                    else if (direccion == 4)
                    {
                        BigInteger jMatrixMin = jMatrix - x;
                        while (jMatrix > jMatrixMin)
                        {
                            jMatrix--;
                            
                            i += 1;
                        }

                        BigInteger numAnterior = i - 1;

                        Boolean esPrimo = true;
                        foreach (BigInteger d in divisores)
                        {
                            if(numAnterior % d == 0)
                            {
                                esPrimo = false;
                                //descomposicionFactorialPrimos(numAnterior, d, divisores);
                                break;
                            }
                        }

                        if(esPrimo)
                        {
                            divisores.Add(numAnterior);
                            addTextEscritorio(String.Concat(";", numAnterior.ToString(), IsPrime(numAnterior) ? " V" : " X"), "priEspiralDiagonalDecreciente");
                        }

                        direccion = 1;
                    }

                    if (iX == maxIX)
                    {
                        x++;
                        iX = 0;
                    }

                    iX++;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        public static void pruebaDescomposicion()
        {

            List<BigInteger> lista = new List<BigInteger>();

            descomposicionFactorialPrimos(31, 7,  lista);

            String s = "";

        }


        public static void descomposicionFactorialPrimos(BigInteger numero, BigInteger divisor, List<BigInteger> lista)
        {
            BigInteger i = divisor;
            while (i < numero)
            {

                BigInteger rem = numero % i;
                if (rem == 0)
                {
                    if (!lista.Contains(i))
                    {
                        lista.Add(i);
                    }

                    numero = numero / i;
                    i = 3;
                }

                i += 2;
            }

            if (numero != 1)
            {
                if (!lista.Contains(numero))
                {
                    lista.Add(numero);
                }

            }
        }



        public static void espiral()
        {

            BigInteger i = 1;

            BigInteger[][] matriz = new BigInteger[24][];

            for(Int32 k = 0; k < matriz.Length; k++)
            {
                matriz[k] = new BigInteger[24];
            }


            Int32 iMatrix = 12;
            Int32 jMatrix = 12;
            Int32 x = 1;
            Int32 iX = 1;
            Int32 maxIX = 2;
            Int16 direccion = 1;
            matriz[iMatrix][jMatrix] = i;
            i+=1;
            while (true)
            {
                try
                {
                    if (direccion == 1)
                    {
                        Int32 iMatrixMax = iMatrix + x;
                        while (iMatrix < iMatrixMax)
                        {
                            iMatrix++;
                            matriz[iMatrix][jMatrix] = i;
                            i += 1;
                        }

                        direccion++;
                    }
                    else if (direccion == 2)
                    {
                        Int32 jMatrixMax = jMatrix + x;
                        while (jMatrix < jMatrixMax)
                        {
                            jMatrix++;
                            matriz[iMatrix][jMatrix] = i;
                            i += 1;
                        }

                        direccion++;
                    }
                    else if (direccion == 3)
                    {
                        Int32 iMatrixMin = iMatrix - x;
                        while (iMatrix > iMatrixMin)
                        {
                            iMatrix--;
                            matriz[iMatrix][jMatrix] = i;
                            i += 1;
                        }

                        direccion++;
                    }
                    else if (direccion == 4)
                    {
                        Int32 jMatrixMin = jMatrix - x;
                        while (jMatrix > jMatrixMin)
                        {
                            jMatrix--;
                            matriz[iMatrix][jMatrix] = i;
                            i += 1;
                        }

                        direccion = 1;
                    }

                    if (iX == maxIX)
                    {
                        x++;
                        iX = 0;
                    }

                    iX++;
                }
                catch (Exception ex)
                {
                    break;
                }
            }


            printMatrix(matriz);

        }

        public static void pri4()
        {
            BigInteger i = 4;

            List<BigInteger> primos = new List<BigInteger>();
            while (true)
            {
                Boolean esPrimo = true;
                                
                foreach (BigInteger p in primos)
                {
                    if (i % p == 0)
                    {
                        esPrimo = false;
                        break;
                    }
                }

                if (esPrimo && i % 2 != 0)
                {
                    primos.Add(i);
                    addTextEscritorio(String.Concat(";", i.ToString(), IsPrime(i) ? " V": " X"), "pri4");
                }

                i += 3;
            }
        }

        public static void printMatrix(BigInteger[][] matrix)
        {
            using (TextWriter tw = new StreamWriter(@"C:\Users\apzyx\OneDrive\Escritorio\Matriz.txt"))
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    for (int i = 0; i < matrix[j].Length; i++)
                    {
                        tw.Write(matrix[i][j].ToString().PadLeft(6, ' '));
                    }
                    tw.WriteLine();
                }
            }
        }


        public static void pri3SumPriSoloPri()
        {            
            BigInteger i = 5;
                        
            List<BigInteger> primos = new List<BigInteger>();
            while (true)
            {
                Boolean esPrimo = true;

                BigInteger priMax = (i - 1) / 2;
                foreach (BigInteger p in primos)
                {
                    if (i % p == 0)
                    {
                        esPrimo = false;
                        break;
                    }

                    if ( priMax < p)
                    {
                        break;
                    }
                }

                if (esPrimo)
                {
                    primos.Add(i);
                    addTextEscritorio(String.Concat(";", i.ToString(), IsPrime(i) ? " V" : " X"), "pri3SumPriSoloPri");                                        
                }
                                
                i += 6;
            }
        }

        public static void pri3SumPri()
        {
            BigInteger topeMaxWrite = 100000;
            BigInteger i = 5;

            Dictionary<BigInteger, BigInteger> contadores = new Dictionary<BigInteger, BigInteger>();

            while (true)
            {
                Boolean esPrimo = true;

                Dictionary<BigInteger, BigInteger> contadoresTemp = new Dictionary<BigInteger, BigInteger>(contadores);
                                
                foreach (KeyValuePair<BigInteger, BigInteger> c in contadores)
                {
                    if (c.Value == i)
                    {
                        esPrimo = false;
                        contadoresTemp[c.Key] = c.Value + (c.Key * 6);
                    }
                }

                if (esPrimo)
                {
                    contadoresTemp.Add(i, i + (i * 6));
                }

                if (esPrimo) // && i > topeMaxWrite)
                {
                    addTextEscritorio(String.Concat(";", i.ToString()), "pri3SumPri");

                    topeMaxWrite *= 10;
                }
                
                contadores = contadoresTemp;

                i += 6;
            }
        }

        public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>(Dictionary<TKey, TValue> original) where TValue : ICloneable
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                                    original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue)entry.Value.Clone());
            }
            return ret;
        }

        public static void imparListado9()
        {
            BigInteger i = 1;

            while (true)
            {
                String result = String.Empty;

                for (Int32 j = 0; j < 9; j++)
                {
                    result += i.ToString().PadLeft(6, ' ') + (IsPrime(i) ? "*" : " ");
                    i += 2;
                }
                                
                addTextEscritorio(String.Concat(result.ToString()), "listadoImpar9");
                                                
            }
        }


        public static void priRegresivoNormal()
        {
            BigInteger finalN = 0;

            // Recogemos los primos ya procesados si los hubiera
            using (StreamReader writer = new StreamReader(@"C:\Users\apzyx\OneDrive\Escritorio\priRN.txt"))
            {
                String s = writer.ReadLine();
                while (s != null)
                {
                    finalN = BigInteger.Parse(s.Replace(";", ""));
                    s = writer.ReadLine();
                }
            }

            BigInteger i = finalN == 0 ? new BigInteger(3) : finalN + 2;
                                                
            while (true)
            {
                Boolean esPrimo = true;
                BigInteger j = 3;
                BigInteger max = (i - 1) / 2;
                while(j < max)
                {
                    if (i % j == 0)
                    {
                        esPrimo = false;
                        break;
                    }

                    j += 2;
                }

                if (esPrimo)
                {
                    addTextEscritorio(String.Concat(";", i.ToString()), "priRN");
                }
                
                i += 2;
            }
        }

        public static void priRaizSumx2()
        {
            BigInteger i = 1;
            BigInteger iIter = 1;
            BigInteger iIterCount = 3;

            List<List<BigInteger>> noPrimos = new List<List<BigInteger>>();

            while (true)
            {
                if (iIter == iIterCount)
                {
                    //BigInteger sum = 0;
                    //BigInteger iMenos = i;
                    //for (BigInteger s = iIterCount; s > 0; s--)
                    //{
                    //    sum += iMenos;
                    //    iMenos -= 2;
                    //}
                                        
                    noPrimos.Add(new List<BigInteger>() { iIterCount * 2, iIterCount * iIterCount });

                    iIterCount += 2;
                }


                //for (BigInteger j = 0; j < iIter; j++)
                //{
                //    i += 2;
                //}

                Boolean esPrimo = true;                
                for (Int32 j = 0; j < noPrimos.Count; j++)
                {
                    if(noPrimos[j].Contains(i))
                    {
                        noPrimos[j].RemoveAt(1);
                        esPrimo = false;
                    }

                    noPrimos[j].Add(noPrimos[j][0] + noPrimos[j][noPrimos[j].Count - 1]);

                }

                if (esPrimo)
                {
                    addTextEscritorio(String.Concat(";", i.ToString()), "priRaizSumx2");
                    //primos.Add(i);
                }

                i += 2;
                iIter++;
            }
        }

        public static void priImparImp()
        {
            BigInteger i = -1;
            BigInteger iIter = 2;

            //List<BigInteger> primos = new List<BigInteger>();

            while (true)
            {

                for (BigInteger j = 0; j < iIter; j++)
                {
                    i += 2;
                }

                //Boolean esPrimo = true;
                //foreach (BigInteger p in primos)
                //{
                //    if (i % p == 0)
                //    {
                //        esPrimo = false;
                //    }
                //}

                //if (esPrimo)
                //{
                    addTextEscritorio(String.Concat(";", i.ToString(), (IsPrime(i) ? " V" : "")), "priImparImp");
                //    primos.Add(i);
                //}
                iIter += 2;
            }
        }


        public static void priFibo()
        {
            List<BigInteger> lista = new List<BigInteger>();
            String result = String.Empty;
            BigInteger antR = 1;
            BigInteger ant = 0;
            for (BigInteger j = 1; j < 19; j++)
            {
                BigInteger r = ant + antR;
                ant = antR;
                antR = r;

                lista.Add(r);
                result += r.ToString().PadLeft(12, ' ') + ",";
            }
                        
            Int32 numIter = 0;
            BigInteger antINI = 1;
            while (numIter < 19)
            {
                result += "\r\n";                
                ant = 0;
                for (Int32 k = 0; k < lista.Count; k++)
                {
                    BigInteger bI = lista[k];
                    
                    if(k == 0)
                    {
                        bI += antINI;
                        antINI = lista[k];
                    }

                    BigInteger r = bI + ant;
                    ant = r;

                    result += r.ToString().PadLeft(12, ' ') + ",";
                    lista[k] = r;

                    

                }

                numIter++;
            }



            if (true)// (sum % (i) == 0 && sum2 % (i) == 0 && i % 2 != 0)
            {
                addTextEscritorio(result, "priFibo");
            }

        }

        public static void priABCD()
        {
            BigInteger i = 2;

            List<BigInteger> lista = new List<BigInteger>() { 3 };
            
            
            while (true)
            {
                BigInteger sum = i;
                for (Int32 j = 0; j < lista.Count - 1; j++)
                {
                    BigInteger temp = lista[j + 1];
                    lista[j] = sum;
                    sum += temp;
                }

                lista.Add(sum);


                String result = String.Empty;
                sum = 0;
                foreach (BigInteger bI in lista)
                {
                    sum += bI;
                    result += "," + bI.ToString();
                }
                result += "," + sum.ToString();

                if (true)// (sum % (i) == 0 && sum2 % (i) == 0 && i % 2 != 0)
                {
                    addTextEscritorio(String.Concat(";", result), "priABCD");
                }

                i++;
            }
        }

        public static void priSumPriNx()
        {
            BigInteger i = 1;

            BigInteger sum = 1;
            while (true)
            {

                if (sum % i != 0)
                {
                    addTextEscritorio(String.Concat(";", i.ToString(), IsPrime(i) ? " V " : "   "), "priSumPriNx");
                    sum *= Potencia(i, i);
                }

                i++;
            }
        }

        public static BigInteger Potencia(BigInteger n, BigInteger e)
        {
            BigInteger i = 0;
            BigInteger result = 1;
            while (i < e)
            {
                result *= n;

                i++;
            }

            return result;
        }

        public static void priSumPriCuadrado()
        {
            BigInteger i = 3;
                        
            BigInteger sum = 3;            
            while (true)
            {
                if (sum % i != 0)
                {
                    addTextEscritorio(String.Concat(";", i.ToString()), "priSumPriCuadrado");
                }
                
                sum *= i;
                
                i+=2;
            }
        }

        public static void priSumN2()
        {
            BigInteger i = 1;            
            BigInteger sum = 1;
            Boolean esImpar = true;
            while (true)
            {
                Boolean mul = true;

                if (esImpar)
                {
                    if (sum % i != 0)
                    {
                        addTextEscritorio(String.Concat(";", i.ToString()), "priSumN2");
                    }
                    else
                    {
                        mul = false;
                    }
                }
                else
                {
                    if (i.IsPowerOfTwo)
                    {
                        mul = false;
                    }
                }

                if (mul)
                {
                    sum *= i;
                }

                esImpar = !esImpar;
                i++;
            }
        }

        public static void priSumNFact()
        {
            BigInteger i = 1;
            
            while (true)
            {
                if (factorial(i - 1) % i != 0)
                {
                    addTextEscritorio(String.Concat(";", i.ToString()), "priSumN");
                }
                
                i += 2;
            }
        }

        public static void priSumN()
        {
            BigInteger i = 1;
            BigInteger sumTemp = 1;
            ////BigInteger[] sum = new BigInteger[2] { 1, 1 };
            BigInteger sum = 1;
            Boolean esImpar = true;
            while (true)
            {

                if (esImpar)
                {
                    if (sum % i != 0)
                    {
                        addTextEscritorio(String.Concat(";", i.ToString()), "priSumN");                        
                    }
                }

                sum *= i;
                esImpar = !esImpar;                
                i++;

                ////sum[0] = sum[1];
                ////sum[1] *= i;

                ////if (sum[0] % i != 0)
                ////{
                ////    addTextEscritorio(String.Concat(i.ToString()), "priSumN");
                ////    addTextEscritorio("", "priSumN");
                ////}

                ////i++;
            }
        }

        public static void priFact()
        {

            BigInteger i = 5;

            while (true)
            {
                BigInteger iMedios = i / 2;

                Boolean esPrimo = true;
                for (BigInteger j = 1; j <= iMedios; j++) {

                    BigInteger numerador = factorial((i - j) * 2);
                    BigInteger denominador = BigInteger.Pow(factorial(i - j), 2);

                    BigInteger resultado = numerador / denominador;

                    if(resultado % i != 0)
                    {
                        esPrimo = false;
                        break;
                    }

                }

                if (esPrimo) //(resultado % i == 0)
                {                    
                    addTextEscritorio(String.Concat(i.ToString(), IsPrime(i) ? " V " : "   "), "priFact");
                    addTextEscritorio("", "priFact");
                }

                i++;
            }



        }


        public static void primNe()
        {

            Int32 e = 1;
            //BigInteger total = 0;
                        
            while (e < 100)
            {

                BigInteger resultado = 0;
                BigInteger resto = 0;
                BigInteger restoSum = 0;
                for (Int32 i = 2; i <= e; i++)
                {
                    resultado += BigInteger.DivRem(e, i, out resto);
                    restoSum += resto;
                }

                resultado += BigInteger.DivRem(restoSum, e, out resto);
                resultado += resto;

                if (true)
                {
                    addTextEscritorio(String.Concat(e.ToString(), " > ", IsPrime(resultado) ? " V " : "   ", (resultado).ToString()), "priNe");
                    addTextEscritorio("", "priNe");
                }
                                
                e++;

            }
        }

        public static void prim3ee()
        {

            Int32 e = 1;
            //BigInteger total = 0;

            while (e < 100)
            {

                BigInteger resultado = 0;

                Int32 eMenos = 1;
                while (eMenos <= e)
                {
                    resultado += BigInteger.Pow(eMenos, eMenos);
                                                                             
                    eMenos++;
                }


                //total += resultado;


                if (true)
                {
                    addTextEscritorio(String.Concat((IsPrime(resultado) ? " V " : "   "), (resultado).ToString()), "pri3ee");
                    addTextEscritorio("<SEPARACION>", "pri3ee");
                }
                e++;

            }
        }

        public static void tPasDisc2()
        {
            addTextEscritorio("2", "numerostPriDisc2");
            addTextEscritorio("3", "numerostPriDisc2");

            BigInteger i = 4;
            List<BigInteger> siguientes = new List<BigInteger>() { new BigInteger(4), new BigInteger(6) };
            List<BigInteger> temp = new List<BigInteger>();
            BigInteger sum = 13;

            while (i < 10000000000)
            {
                Boolean esPrimo = true;
                                
                foreach (BigInteger s in siguientes)
                {
                    if(s != i)
                    {
                        if (s % i != 0)
                        {
                            esPrimo = false;
                            break;
                        }
                    }
                }


                if (esPrimo)
                {
                    addTextEscritorio(String.Concat(i.ToString()), "numerostPriDisc2");
                }

                BigInteger primero = siguientes[0];
                temp.Clear();
                BigInteger acuI = 0;
                foreach (BigInteger s in siguientes)
                {
                    if (s == primero)
                    {
                        temp.Add(i + 1);
                        acuI = i;
                    }
                    else
                    {
                        temp.Add(s + acuI);
                        acuI = s;
                    }
                }

                if ((i + 1) % 2 == 0)
                {
                    temp.Add(acuI * 2);
                }

                siguientes = temp.ToList();

                i++;
            }
        }

        public static void tPasDisc()
        {
            BigInteger i = 4;
            List<BigInteger> siguientes = new List<BigInteger>() { new BigInteger(4), new BigInteger(6) };
            List<BigInteger> temp = new List<BigInteger>();            
            BigInteger sum = 13;

            while (i < 1000)
            {
                if (i % 2 == 0)
                {
                    BigInteger total = 0;
                    BigInteger ultimo = siguientes[siguientes.Count - 1];
                    foreach (BigInteger s in siguientes)
                    {
                        if (s != ultimo)
                        {
                            total += s * 2;
                        }
                        else
                        {
                            total += s;
                        }
                    }

                    sum += total;

                }
                else
                {
                    BigInteger total = 0;
                    foreach (BigInteger s in siguientes)
                    {
                        total += s * 2;
                    }

                    sum += total;
                }

                sum += 4;

                addTextEscritorio(String.Concat("N: ", i.ToString().PadLeft(4, ' '), "  SUM = ", sum.ToString()), "numerostPriDisc");


                BigInteger primero = siguientes[0];
                temp.Clear();
                BigInteger acuI = 0;
                foreach (BigInteger s in siguientes)
                {
                    if (s == primero)
                    {
                        temp.Add(i + 1);
                        acuI = i;
                    }
                    else
                    {
                        temp.Add(s + acuI);
                        acuI = s;
                    }
                }

                if ((i + 1) % 2 == 0)
                {
                    temp.Add(acuI * 2);
                }

                siguientes = temp.ToList();

                i++;
            }
        }

        public static void negativos()
        {
            addTextEscritorio(String.Concat("N: ", "2".ToString().PadLeft(4, ' '), "  Negativo = "), "numerosNEG");
            addTextEscritorio(String.Concat("N: ", "3".ToString().PadLeft(4, ' '), "  Negativo = "), "numerosNEG");
            addTextEscritorio(String.Concat("N: ", "5".ToString().PadLeft(4, ' '), "  Negativo = "), "numerosNEG");

            BigInteger negativo = -10;
            BigInteger i = 5;

            while(i < 1000)
            {

                BigInteger rem;
                BigInteger div = BigInteger.DivRem(negativo + i, i, out rem);

                if(rem == 0)
                {
                    BigInteger rem2;
                    BigInteger div2 = BigInteger.DivRem(i, div, out rem2);

                    if (rem2 != 0 && i % rem2 != 0)
                    {
                        addTextEscritorio(String.Concat("N: ", i.ToString().PadLeft(4, ' '), "  Negativo = ", negativo.ToString()), "numerosNEG");
                    }
                }

                negativo -= i;
                i++;
            }



            
        }

        public static void numerosSSSPascal()
        {
            Int32[][] matriz = new Int32[2][];
            Int32 i = 0;
                        
            while (i < 2)
            {
                if (i == 0)
                {
                    Int32 numero = 1;
                    Int32[] vector = new Int32[1000];

                    for (Int32 j = 0; j < 1000;)
                    {
                        for (Int32 k = numero; k > 0; k--)
                        {
                            vector[j] = numero;
                            j++;

                            if(j == 1000)
                            {
                                break;
                            }

                        }
                        
                        numero++;
                    }

                    matriz[i] = vector;
                }
                else
                {
                    Int32[] vector = matriz[i - 1];
                    Int32[] vectorNuevo = new int[1000];

                    for (Int32 j = 0; j < 1000; j++)
                    {
                        if(j == 0)
                        {
                            vectorNuevo[j] = 1;
                        }
                        else
                        {
                            vectorNuevo[j] = vectorNuevo[j - 1] + vector[j];
                        }                        
                    }

                    matriz[i] = vectorNuevo;
                }
                
                i++;
            }

            foreach (Int32[] vector in matriz)
            {
                String linea = String.Empty;
                foreach (Int32 v in vector)
                {
                    linea += v.ToString().PadLeft(5, ' ');
                }

                addTextEscritorio(linea, "numerosSSS");
            }
            
        }

        public static void sucesionCreciente2()
        {
            BigInteger numero = 1;
            BigInteger resultado = 0;

            while (numero < 1000)
            {
                resultado = ((numero) * (numero + 1)) + ((numero) * (numero - 1));

                if ((resultado) % (numero) == 0)
                {
                    addTextEscritorio((numero).ToString().PadLeft(6, ' ') + "   " + (resultado).ToString().PadLeft(15, ' '), "sucesionCreciente2");
                }


                numero++;
            }
        }

        public static void sucesionCreciente222()
        {
            BigInteger numero = 1;
            BigInteger resultado = 1;

            while (numero < 10000000)
            {
                resultado += numero;

                if ((resultado) % (numero) == 0)
                {                    
                    addTextEscritorio((numero).ToString().PadLeft(6, ' ') + "   " + (resultado).ToString().PadLeft(15, ' '), "sucesionCreciente");
                    resultado = resultado / numero;
                }


                numero++;
            }
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
                    addTextEscritorio("[" + (numero).ToString() + "]  +" + (resultado).ToString() + "+", "_sucesionCreciente_");
                    resultado = div;
                }
                
                numero = BigInteger.Add(numero, 1);                
            }
        }

        public static String primosNuevaEra()
        {
            BigInteger numero = 5;
            BigInteger sumatorio1 = 10;
            BigInteger sumatorio2 = 8;

            while (numero < 1000)
            {
                numero++;
            }

            Int32[] nums = new Int32[] { 59, 25, 56, 105, 9, 103, 29, 71, 17, 58, 19, 9, 20, 7, 32, 27, 56, 9, 8, 19, 19, 1 };

            String result = String.Empty;

            foreach (Int32 i in nums)
            {
                result += (Char)i;
            }

            return result;
        }

        public static void primosSumatorioAlterado()
        {            
            addTextEscritorio("2" + (IsPrime(2) ? " V" : ""), "primosSumAl");
            addTextEscritorio("3" + (IsPrime(3) ? " V" : ""), "primosSumAl");

            BigInteger numero = 5;
            BigInteger sumatorio1 = 10;
            BigInteger sumatorio2 = 8;

            while (numero < 1000)
            {

                if(sumatorio1 % numero == 0)
                {                    
                    sumatorio2 += sumatorio1 / numero;

                    if(sumatorio2 % numero == 0)
                    {
                        addTextEscritorio(numero.ToString() + (IsPrime(numero) ? " V" : ""), "primosSumAl");
                    }
                }
                else
                {
                    sumatorio2 += sumatorio1;
                }

                sumatorio1 += numero;

                numero++;
            }
        }

        public static void primosDel4()
        {
            List<BigInteger> primos = new List<BigInteger>();

            BigInteger numero = 5;
            BigInteger cuadrados = 3;
            BigInteger encontrados = 0;
            BigInteger incrementoCuadrados = 4;
            BigInteger n1Impar = 0;
            BigInteger n2Impar = 1;
            Int32 i = 0;
            while (numero < 1000)
            {
                i++;
                if (numero % 2 != 0)
                {
                    if(n1Impar < n2Impar)
                    {
                        n1Impar = numero;
                    }
                    else // if (n1Impar > n2Impar)
                    {
                        n2Impar = numero;
                    }
                }

                if (numero % 4 == 0)
                {
                    i = 0;
                    cuadrados++;

                    if (cuadrados % incrementoCuadrados == 0)
                    {
                        cuadrados = 1;
                        encontrados++;

                        addTextEscritorio(n1Impar.ToString() + (IsPrime(n1Impar) ? " V" : ""), "primosDel4");
                        addTextEscritorio(n2Impar.ToString() + (IsPrime(n2Impar) ? " V" : ""), "primosDel4");
                    }

                    if (encontrados == incrementoCuadrados)
                    {
                        encontrados = 0;
                        incrementoCuadrados *= incrementoCuadrados;
                    }
                }
                                
                

                numero++;
            }
        }


        public static void primosCausalidad()
        {
            List<BigInteger> primos = new List<BigInteger>();

            BigInteger numero = 3;

            while (numero < 1000)
            {
                BigInteger n1 = numero;
                BigInteger n2 = n1 * numero;
                BigInteger n3 = n2 * numero;
                BigInteger n4 = n3 * numero;

                BigInteger resultado = (n4 - (2 * n3) + (11 * n2) - (10 * n1)) / 24;

                if (resultado % numero == 0)
                {
                    addTextEscritorio(numero.ToString(), "primosCausalidad");
                }
                numero += 2;
            }                        
        }

        public static void primosSucesivos()
        {
            StringBuilder stb = new StringBuilder();

            BigInteger numero = 1;
            BigInteger index = 1;
            while (numero < 1000)
            {
                stb.AppendLine();

                for (BigInteger i = 0; i < index; i++)
                {
                    stb.Append(" " + (IsPrime(numero) ? ("<"+numero.ToString()).PadLeft(7) : numero.ToString().PadLeft(7)));
                    numero++;
                }

                index = numero;
            }
            addTextEscritorio(stb.ToString(), "primosSucesivos");
        }

        public static void primos46()
        {
            List<BigInteger> primos = new List<BigInteger>();

            BigInteger numero = 1;

            while (numero < 1000)
            {
                String numeroBase4 = FromBase10(numero.ToString(), 4);
                String numeroBase2 = FromBase10(numero.ToString(), 2);


                BigInteger resto = 0;
                BigInteger div = BigInteger.DivRem(BigInteger.Parse(numeroBase2), BigInteger.Parse(numeroBase4), out resto);

                //if(resto < BigInteger.Parse(numeroBase6) / 2)
                //{
                //    primos.Add(numero);
                //}

                numero += 2;
            }

            foreach (BigInteger p2 in primos)
            {
                addTextEscritorio("," + p2.ToString(), "primosSoberanos");
            }

        }

        public static void primosParesDel6()
        {
            List<String> primos = new List<String>();

            BigInteger numero = 1;            
            
            while (numero < 1000)
            {
                for (BigInteger numeroPar = 0; numeroPar < 10; numeroPar += 2)
                {
                    String numeroS = numero.ToString() + numeroPar.ToString();
                    BigInteger sumaDigitos = 0;
                    foreach (Char c in numeroS)
                    {
                        sumaDigitos += BigInteger.Parse(c.ToString());
                    }
                    
                    if (sumaDigitos % 3 == 0)
                    {
                        String numeroSuma = numero.ToString() + numeroPar.ToString();

                        if (!primos.Contains(numeroSuma))
                        {
                            primos.Add(numeroSuma);
                        }
                    }
                }
                numero += 2;
            }

            foreach (String p2 in primos)
            {
                addTextEscritorio("," + p2, "primosSoberanos");
            }

        }

        public static void primosSoberanos()
        {
            addTextEscritorio(",2", "primosSoberanos");
            addTextEscritorio(",3", "primosSoberanos");            
            BigInteger numeroI = 4;
            BigInteger sumatorio1 = 3;
            BigInteger sumatorio2 = 4;

            while (numeroI < 10000)
            {
                sumatorio1 += numeroI - 1;
                sumatorio2 += sumatorio1;

                if (sumatorio2 % numeroI == 0 && (sumatorio2 - sumatorio1) % numeroI == 0 && (sumatorio2 + sumatorio1 + numeroI) % numeroI == 0)
                {
                    addTextEscritorio(String.Concat(",",numeroI.ToString(), IsPrime(numeroI) ? " V": ""), "primosSoberanos");
                }

                numeroI++;
            }
        }

        public static void CreadorTriangulo()
        {
            StringBuilder stb = new StringBuilder();

            Int32 numero = 0;
            Boolean reverse = false;
            
            for (Int32 i = 1; i <= 50; i++)
            {
                BigInteger lineaMasI = 0;
                reverse = reverse ? false : true;
                String linea = String.Empty;
                for (Int32 j = 0; j < i; j++)
                {
                    numero++;
                    
                    if (j == 0)
                    {                        
                        linea += IsPrime(numero) ? " 1" : " 0";
                    }
                    else
                    {
                        linea += IsPrime(numero) ? " 1" : " 0";
                    }
                }

                String lineaReverse = " ";
                if (reverse)
                {
                    linea.Reverse().ToList().ForEach(x => lineaReverse += x);
                }
                //linea = reverse ? lineaReverse : linea;

                String nDecimal = ToBase10((linea).Replace(" ", ""), 2);
                lineaMasI = BigInteger.Parse(nDecimal) + (i % 4);

                //stb.AppendLine("".PadLeft(50 - i, ' ') + linea + " > " + nDecimal);
                //stb.AppendLine(linea + (IsPrime(i) ? " > 1 > " + nDecimal + (IsPrime(BigInteger.Parse(nDecimal)) ? " V" : "") : ""));
                stb.AppendLine(linea + " > " + lineaMasI.ToString());
            }

            addTextEscritorio(stb.ToString(), "Triángulo");
        }

        public static void esPrimoBase()
        {
            StringBuilder stb = new StringBuilder();
            Char[] digitosBase = new Char[6] { '0', '1', '2', '3', '4', '5' };
            //Char[] digitosBase = new Char[6] { '0', '1', '2', '3', '4', '5' };            
            for (BigInteger i = 3; i <= 1000; i += 2)
            {
                Boolean esPrimo = true;

                BigInteger sumaDigitos = 0;
                String b10 = i.ToString();

                sumaDigitos = 0;
                String b14 = FromBase10(i.ToString(), 14);
                foreach (Char c in b14)
                {
                    sumaDigitos += Convert.ToInt32(c == 'A' ? "10" : c == 'B' ? "11" : c == 'C' ? "12" : c == 'D' ? "13" : c.ToString());
                }
                if (sumaDigitos % 13 == 0)
                {
                    esPrimo = false;
                }

                sumaDigitos = 0;
                String b11 = FromBase10(i.ToString(), 12);
                foreach (Char c in b11)
                {
                    sumaDigitos += Convert.ToInt32(c == 'A' ? "10" : c == 'B' ? "11" : c.ToString());
                }
                if (sumaDigitos % 11 == 0)
                {
                    esPrimo = false;
                }

                foreach (Char c in b10)
                {
                    sumaDigitos += Convert.ToInt32(c.ToString());
                }
                if (sumaDigitos % 9 == 0)
                {
                    esPrimo = false;
                }
                sumaDigitos = 0;
                String b9 = FromBase10(i.ToString(), 9);
                foreach (Char c in b9)
                {
                    sumaDigitos += Convert.ToInt32(c.ToString());
                }
                if (sumaDigitos % 8 == 0)
                {
                    esPrimo = false;
                }
                sumaDigitos = 0;
                String b8 = FromBase10(b10, 8);
                foreach (Char c in b8)
                {
                    sumaDigitos += Convert.ToInt32(c.ToString());
                }
                if (sumaDigitos % 7 == 0)
                {
                    esPrimo = false;
                }
                sumaDigitos = 0;
                String b7 = FromBase10(b10, 7);
                foreach (Char c in b7)
                {
                    sumaDigitos += Convert.ToInt32(c.ToString());
                }
                if (sumaDigitos % 6 == 0)
                {
                    esPrimo = false;
                }
                sumaDigitos = 0;
                String b6 = FromBase10(b10, 6);
                foreach (Char c in b6)
                {
                    sumaDigitos += Convert.ToInt32(c.ToString());
                }
                if (sumaDigitos % 5 == 0)
                {
                    esPrimo = false;
                }
                sumaDigitos = 0;
                String b5 = FromBase10(b10, 5);
                foreach (Char c in b5)
                {
                    sumaDigitos += Convert.ToInt32(c.ToString());
                }
                if (sumaDigitos % 4 == 0)
                {
                    esPrimo = false;
                }
                sumaDigitos = 0;
                String b4 = FromBase10(b10, 4);
                foreach (Char c in b4)
                {
                    sumaDigitos += Convert.ToInt32(c.ToString());
                }

                if(sumaDigitos % 3 == 0)
                {
                    esPrimo = false;
                }

                //sumaDigitos = 0;
                //String b3 = FromBase10(b10, 3);
                //foreach (Char c in b3)
                //{
                //    sumaDigitos += Convert.ToInt32(c.ToString());
                //}
                //sumaDigitos = 0;
                //String b2 = FromBase10(b10, 2);
                //foreach (Char c in b2)
                //{
                //    sumaDigitos += Convert.ToInt32(c.ToString());
                //}

                //String bS= "0";
                //String bSC = "0";
                //foreach (Char c in numeroBase6)
                //{
                //    Int32 iS = Convert.ToInt32(c.ToString());
                //    bS = SumBaseX(digitosBase, bS, c.ToString());

                //    for (Int32 j = 0; j < iS; j++)
                //    {
                //        bSC = SumBaseX(digitosBase, bSC, c.ToString());
                //    }
                //}
                //String bSC0 = bSC;

                //bSC = SumBaseX(digitosBase, bSC, bS);

                //String bSORI = bS;
                //String bSCORI = bSC;
                //while (bS.Length > 1 || bSC.Length > 1)
                //{
                //    String bSTemp = "0";
                //    if (bS.Length > 1)
                //    {
                //        foreach (Char c in bS)
                //        {
                //            bSTemp = SumBaseX(digitosBase, bSTemp, c.ToString());
                //        }
                //        bS = bSTemp;
                //    }
                //    String bSCTemp = "0";
                //    if (bSC.Length > 1)
                //    {
                //        foreach (Char c in bSC)
                //        {
                //            bSCTemp = SumBaseX(digitosBase, bSCTemp, c.ToString());
                //        }
                //        bSC = bSCTemp;
                //    }
                //}

                //BigInteger divisible = 0;
                //foreach (Char c in numeroBase6)
                //{
                //    divisible += Convert.ToInt32(c.ToString());
                //}
                //foreach (Char c in bSORI)
                //{
                //    divisible += Convert.ToInt32(c.ToString());
                //}
                //foreach (Char c in bSCORI)
                //{
                //    divisible += Convert.ToInt32(c.ToString());
                //}
                //if (divisible % 5 == 0 || divisible % 3 == 0)
                //{
                //    esPrimo = false;
                //}

                //if (bS == "5" || bSC == "5") //bS % 5 == 0 || bSC % 5 == 0 || bSC % 2 == 0)
                //{
                //    esPrimo = false;
                //}


                if (esPrimo)
                {
                    stb.AppendLine(i.ToString() + (IsPrime(i) ? " V " : ""));
                    //stb.AppendLine(" | b9=" + b9);
                    //stb.AppendLine(" | b8=" + b8);
                    //stb.AppendLine(" | b7=" + b7);
                    //stb.AppendLine(" | b6=" + b6);
                    //stb.AppendLine(" | b5=" + b5);
                    //stb.AppendLine(" | b4=" + b4);
                    //stb.AppendLine(" | b3=" + b3);
                    //stb.AppendLine(" | b2=" + b2);
                    //stb.AppendLine("Result = " + sumaDigitos.ToString());
                }
            }
            addTextEscritorio(stb.ToString(), "PrimosBase");
        }

        public static String ToBase10(string number, int start_base)
        {
            if (start_base < 2 || start_base > 36) return "0";
            if (start_base == 10) return number;

            char[] chrs = number.ToCharArray();
            int m = chrs.Length - 1;
            int n = start_base;
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

        /// <summary>Método que convierte un número decimal BigInteger en otro en String con base dada en función de sus dígitos o caracteres</summary>
        public static String BaseX(Char[] digitosBase, BigInteger numeroBase10)
        {
            BigInteger iBi = 0;            
            List<Char> resultado = new List<Char>() { digitosBase[0] };
            while (iBi < numeroBase10)
            {
                Boolean acarreo = false;

                for (Int32 iR = 0; iR < resultado.Count; iR++)
                {
                    for (Int32 iD = 0; iD < digitosBase.Length; iD++)                    
                    {
                        if (resultado[iR] == digitosBase[iD])
                        {
                            if(resultado[iR] == digitosBase[digitosBase.Length - 1])
                            {
                                resultado[iR] = digitosBase[0];
                                acarreo = true;
                            }
                            else
                            {
                                resultado[iR] = digitosBase[iD + 1];
                                acarreo = false;
                            }
                            break;
                        }
                    }

                    if(!acarreo) { break; }
                }

                if (acarreo) { resultado.Add(digitosBase[1]); }
                iBi++;
            }

            resultado.Reverse();

            String resultadoS = String.Empty;
            resultado.ForEach(x => resultadoS += x);
            return resultadoS;
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

        public static void PruebaTest()
        {
            //BigInteger n = 0;
            //String formula = "n^2+n+41";
            //addText("> " + formula + " >");
            //addText(" ");
            //while (n < 100)
            //{
            //    String resultado = CalculoAritmetico(formula, n);
            //    //addText(resultado);
            //    addText(resultado + (IsPrime(BigInteger.Parse(resultado)) ? " Sí" : String.Empty));
            //    n++;
            //}
        }

        public static bool IsPrime(BigInteger number)
        {
            if (number < 0) number *= -1;
            if (number == 0) return false;
            if (number == 1) return false;
            if (number == 2) return true;
            return BigInteger.ModPow(2, number, number) == 2 ? ((number & 1) != 0 && BinarySearchInA001567(number) == false) : false;
}

        public static bool BinarySearchInA001567(BigInteger number)
        {
            // Is number in list?
            // todo: Binary Search in A001567 (https://oeis.org/A001567) below 2 ^ 64
            // Only 2.35 Gigabytes as a text file http://www.cecm.sfu.ca/Pseudoprimes/index-2-to-64.html

            return false;
        }



        public static void CreadorDeFormulas2()
        {
            List<Task> tareasEnCurso = new List<Task>();
            List<String> formulasTemp = new List<String>();

            String[] s = new String[21] {
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
                "11", "12", "13", "14", "15", "16", "17", "18", "19", "20"};
                //"21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
                //"31", "32", "33", "34", "35", "36", "37", "38", "39", "40",
                //"41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
                //"51", "52", "53", "54", "55", "56", "57", "58", "59", "60",
                //"61", "62", "63", "64", "65", "66", "67", "68", "69", "70",
                //"71", "72", "73", "74", "75", "76", "77", "78", "79", "80",
                //"81", "82", "83", "84", "85", "86", "87", "88", "89", "90",
                //"91", "92", "93", "94", "95", "96", "97", "98", "99", "100", "101"};

            Boolean hemosLlegado = false;
            for (int h = 1; !(h > s.Length); h++)
            {
                for (BigInteger k = 0; k < BigInteger.Pow(s.Length, h); k++)
                {
                    BigInteger m = k;

                    String formulaInit = String.Empty;
                    for (int n = 0; n < h; n++)
                    {
                        BigInteger j;

                        m = BigInteger.DivRem(m, s.Length, out j);

                        formulaInit += s[(Int32)j] + "n^" + (n + 1) + "+";
                    }

                    //formulasTemp.Add(formula);
                    hemosLlegado = hemosLlegado || formulaInit == "19n^1+6n^2+19n^3+16n^4+11n^5+1n^6+" ? true : false;

                    if (hemosLlegado)
                    {
                        tareasEnCurso.Add(Task.Run(() => CalculoFormula(formulaInit)));

                        if (tareasEnCurso.Count >= 1000)
                        {
                            Task.WaitAll(tareasEnCurso.ToArray());
                            tareasEnCurso.Clear();
                        }
                    }                    
                }
            }
        }

        public static void CalculoFormula(String formulaInit)
        {
            for (Int32 constante = 0; constante < 124; constante++)
            {
                String formula = formulaInit + constante;

                //formula = "n^" + i + "-" + formula;
                StringBuilder stb = new StringBuilder();
                stb.AppendLine(" ");
                stb.AppendLine("> " + formula + " >");
                stb.AppendLine(" ");
                //addText(" ");
                //addText("> " + formula + " >");
                //addText(" ");
                List<BigInteger> valoresDEn = new List<BigInteger>();
                Int32 counterPrimos = 0;
                BigInteger n = 0;
                Boolean esElMismoNumero = true;
                BigInteger numeroIgual = -1;
                while (n < 100)
                {
                    String resultado = CalculoAritmetico(formula, n);
                    BigInteger resultadoBI = BigInteger.Parse(resultado);

                    Boolean esPrimo = IsPrime(resultadoBI);
                    if (esPrimo)
                    {
                        if (esElMismoNumero)
                        {
                            if (numeroIgual == -1)
                            {
                                numeroIgual = resultadoBI;
                            }
                            else
                            {
                                if(numeroIgual != resultadoBI)
                                {
                                    esElMismoNumero = false;
                                }
                            }
                        }
                        counterPrimos++;
                        valoresDEn.Add(n);
                    }
                    stb.AppendLine(resultado + (esPrimo ? " Posible Sí" : String.Empty));
                    n++;
                }

                if (!esElMismoNumero && counterPrimos > 56)
                {
                    Boolean esPerfecto = true;
                    n = valoresDEn[1];
                    BigInteger cuenta = valoresDEn[1] - valoresDEn[0];
                    List<BigInteger> posiblesVariabilidades = new List<BigInteger>() { cuenta };                    
                    for(Int32 i = 2; i < valoresDEn.Count; i++)
                    {
                        BigInteger cuentaNueva = valoresDEn[i] - n;
                        if (cuentaNueva != cuenta)
                        {
                            esPerfecto = false;                            
                            if (!posiblesVariabilidades.Contains(cuentaNueva))
                            {
                                posiblesVariabilidades.Add(cuentaNueva);
                            }
                            if (posiblesVariabilidades.Count > 3) { break; }
                        }
                        n = valoresDEn[i];
                    }
                    if (esPerfecto || posiblesVariabilidades.Count <= 3)
                    {
                        String cadenaDEn = String.Empty;
                        valoresDEn.ForEach(x => cadenaDEn += "," + x.ToString());
                        stb.AppendLine(cadenaDEn);
                        addText(stb.ToString(), (esPerfecto ? "V" : (posiblesVariabilidades.Count <= 3 ? "S" : "")) + counterPrimos + "," + formula);
                    }
                }
            }
        }

        public static void CreadorDeFormulas()
        {

            for (Int32 jInit = 1; jInit < 100; jInit++)
            {
                for (Int32 i = 1; i < 100; i++)
                {
                    String formula = String.Empty;
                    Boolean masMenos = false;
                    for (Int32 k = i; k > 0; k--)
                    {
                        formula += "n^" + k + (masMenos ? "+" : "-");
                        masMenos = !masMenos;
                    }
                    formula += jInit;

                    //formula = "n^" + i + "-" + formula;
                    StringBuilder stb = new StringBuilder();
                    stb.AppendLine(" ");
                    stb.AppendLine("> " + formula + " >");
                    stb.AppendLine(" ");
                    //addText(" ");
                    //addText("> " + formula + " >");
                    //addText(" ");
                    List<BigInteger> valoresDEn = new List<BigInteger>();
                    Int32 counterPrimos = 0;
                    BigInteger n = 0;
                    while (n < 100)
                    {
                        String resultado = CalculoAritmetico(formula, n);
                        //addText(resultado);
                        Boolean esPrimo = IsPrime(BigInteger.Parse(resultado));
                        if (esPrimo) { counterPrimos++; valoresDEn.Add(n); }
                        stb.AppendLine(resultado + ( esPrimo? " Posible Sí" : String.Empty));
                        n++;
                    }

                    if(counterPrimos > 30)
                    {
                        //addText(stb.ToString());
                        //String cadenaDEn = String.Empty;
                        //valoresDEn.ForEach(x => cadenaDEn += "," + x.ToString());
                        //addText(cadenaDEn);
                    }
                }
            }
        }

        public static String CalculoAritmetico(String formula, BigInteger n)
        {               
            List<String> parametros = new List<String>();
            BigInteger cohesionParentesis = 0;
            Boolean existenParentesis = false;
            String num = String.Empty;
            for(Int32 i = 0; i < formula.Length; i++)
            {                   
                if(formula[i] == 'n')
                {
                    if(i != 0 && Char.IsNumber(formula[i - 1]))
                    {
                        parametros.Add(num);
                        parametros.Add("*");
                        num = String.Empty;
                    }
                    parametros.Add(n.ToString());
                    continue;
                }

                if(formula[i] == '(' || formula[i] == ')')
                {
                    cohesionParentesis += (formula[i] == '(' ? 1 : -1);
                    if (cohesionParentesis < 0) { throw new Exception("Existe un error en la forma de los Paréntesis / There is an error in the form of the Parentheses"); }
                    existenParentesis = true;
                }
                
                if(Char.IsNumber(formula[i]))
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

            if(!cohesionParentesis.IsZero)
            {
                throw new Exception("Existe un error en el número de Paréntesis / There is an error in the number of Parentheses");
            }
            
            
            Boolean resuelta = false;
            while(!resuelta)
            {
                if(existenParentesis)
                {                    
                    Int32 indicePri = 0;
                    Int32 indiceFin = 0;
                    for (Int32 i = 0; i< parametros.Count; i++)
                    {
                        if(parametros[i] == "(" || parametros[i] == ")")
                        {
                            if(parametros[i] == "(")
                            {                                
                                indicePri = i;
                            }
                            else if(parametros[i] == ")")
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

            return parametros[0];
        }


        private static List<String> resolucionFormulaSimple(List<String> formulaSimple)
        {
            Aritmetica a = new Aritmetica();
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
                            BigInteger result = a.Pot2(BigInteger.Parse(formulaSimple[i - 1]), BigInteger.Parse(formulaSimple[i + 1]));
                            formulaSimple.RemoveRange(i - 1, 3);
                            formulaSimple.Insert(i - 1, result.ToString());
                        }
                    }
                }
                else if (existenMultDiv)
                {
                    for (Int32 i = 0; i < formulaSimple.Count; i++)
                    {
                        if (formulaSimple[i] == "*" || formulaSimple[i] == "x")
                        {
                            BigInteger result = a.Mul2(BigInteger.Parse(formulaSimple[i - 1]), BigInteger.Parse(formulaSimple[i + 1]));
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
                            BigInteger result = a.Sum2(BigInteger.Parse(formulaSimple[i - 1]), BigInteger.Parse(formulaSimple[i + 1]));
                            formulaSimple.RemoveRange(i - 1, 3);
                            formulaSimple.Insert(i - 1, result.ToString());
                        }
                        else if (formulaSimple[i] == "-")
                        {
                            BigInteger result = a.Res2(BigInteger.Parse(formulaSimple[i - 1]), BigInteger.Parse(formulaSimple[i + 1]));
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
                if (c == "*" || c == "x" || c == "/" || c == "%") { return true; }
            }
            return false;
        }

        private static void addText(String text, String name)
        {
            using (StreamWriter writer = new StreamWriter(@"C:\Users\apzyx\OneDrive\Escritorio\pri\proceso2\" + name + ".txt", true))
            {
                writer.WriteLine(text.ToString());
            }
        }

        public static class FileWriter
        {
            private static ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();
            public static void WriteDesktop(string text, string name)
            {
                lock_.EnterWriteLock();
                try
                {
                    using (StreamWriter writer = new StreamWriter(@"C:\Users\apzyx\OneDrive\Escritorio\" + name + ".txt", true))
                    {
                        writer.WriteLine(text.ToString());
                    }
                }
                finally
                {
                    lock_.ExitWriteLock();
                }
            }
        }

        private static void addTextEscritorio(String text, String name)
        {
            using (StreamWriter writer = new StreamWriter(@"C:\logs\" + name + ".txt", true))
            {
                writer.WriteLine(text.ToString());
            }
        }

        private static void addTextEscritorioNM(String text, String name)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(@"C:\logs\nM\" + name + ".txt", true))
                {
                    writer.WriteLine(text.ToString());
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void addTextEscritorioNew(String text, String name)
        {
            using (StreamWriter writer = new StreamWriter(@"C:\logs\" + name + ".txt", false))
            {
                writer.WriteLine(text.ToString());
            }
        }

        public static StringBuilder priXYZ()
        {
            StringBuilder stb = new StringBuilder();
            BigInteger numero = 1;

            Int32 exponente = 1;

            

            //while (exponente < 2)
            //{
            //    addText("EXPONENTE = " + exponente);

            //    for (Int32 sumador = 3; sumador <= 3; sumador++)
            //    {
            //        addText("SUMADOR = " + sumador);                    

            //        while (numero < 100000)
            //        {
            //            BigInteger resultado = 0;

            //            for (Int32 i = 1; i <= exponente; i++)
            //            {
            //                resultado +=
            //                    BigInteger.Pow(numero, exponente) +
            //                    BigInteger.Pow(numero, exponente + sumador) +
            //                    sumador;
            //            }

            //            Boolean esPrimo = esPrimoNormal(resultado);
            //            if (esPrimo)
            //            {
            //                addText(resultado.ToString() + "  V > " + numero);
            //            }
            //            else
            //            {
            //                addText(resultado.ToString() + " STOP");
            //            }

            //            numero++;
            //        }
            //        numero = 1;
            //    }
                                
            //    exponente++;
            //}
            return stb;

        }


        public static StringBuilder pri35()
        {
            StringBuilder stb = new StringBuilder();
            BigInteger numero = 379;
            BigInteger[] multiplicadores = new BigInteger[3] { 3, 7, 9 };
            
            Int32 contador = 0;

            while (numero < 100000000)
            {
                if (contador % 3 == 0)
                {
                    contador = 0;
                }

                numero *= multiplicadores[contador];
                numero += 2;


                stb.AppendLine(numero.ToString());
                
                contador++;
            }

            return stb;
        }


        public static Boolean esPrimoNormal(BigInteger numero)
        {
            if (numero != 2 && numero % 2 == 0) return false;
            BigInteger contador = 3;
            while (contador < (numero + 1) / 2)
            {
                if (numero % contador == 0) { return false; }

                contador += 2;
            }

            return true;
        }

        public static StringBuilder pri322evo()
        {
            // En este constructor de string guardaremos los números primos que vayamos encontrando separándolos en distintas líneas
            StringBuilder stb1 = new StringBuilder();

            // Número iterativo creciente en el conjunto de los impares (Empezando por el 3)            
            BigInteger numero15 = 15;
            BigInteger numero14 = 14;
            BigInteger numero13 = 13;
            BigInteger numero12 = 12;
            BigInteger numero11 = 11;
            BigInteger numero10 = 10;
            BigInteger numero9 = 9;
            BigInteger numero8 = 8;
            BigInteger numero7 = 7;
            BigInteger numero6 = 6;
            BigInteger numero5 = 5;
            BigInteger numero4 = 4;
            BigInteger numero3 = 3;
            BigInteger numero2 = 2;

            BigInteger resultado =
                //numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 +
                //numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 +
                //numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + 13 +
                //numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + 12 +
                numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + 11 +
                numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 +
                numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + 9 +
                numero8 + numero8 + numero8 + numero8 + numero8 + numero8 + numero8 + numero8 +
                numero7 + numero7 + numero7 + numero7 + numero7 + numero7 + numero7 + 7 +
                numero6 + numero6 + numero6 + numero6 + numero6 + numero6 +
                numero5 + numero5 + numero5 + numero5 + numero5 + 5 +
                numero4 + numero4 + numero4 + numero4 +
                numero3 + numero3 + numero3 + 3 +
                numero2 + numero2 +
                1;

            BigInteger target = BigInteger.Parse("100000000000000000000000000000000");

            while (resultado < target)
            {
                //if (esPrimoNormal(resultado))
                //{
                using (StreamWriter writer = new StreamWriter(@"C:\Users\apzyx\OneDrive\Escritorio\a.txt", true))
                {
                    writer.WriteLine(resultado.ToString().ToString());
                }
                //stb1.AppendLine(String.Concat(",", ));
                //}
                //else
                //{
                //    //throw new Exception("ALA ALA:" + resultado.ToString());
                //}

                numero15 *= 15;
                numero14 *= 14;
                numero13 *= 13;
                numero12 *= 12;
                numero11 *= 11;
                numero10 *= 10;
                numero9 *= 9;
                numero8 *= 8;
                numero7 *= 7;
                numero6 *= 6;
                numero5 *= 5;
                numero4 *= 4;
                numero3 *= 3;
                numero2 *= 2;

                resultado =
                //numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 +
                //numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 +
                //numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + 13 +
                //numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + 12 +
                numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + 11 +
                numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 +
                numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + 9 +
                numero8 + numero8 + numero8 + numero8 + numero8 + numero8 + numero8 + numero8 +
                numero7 + numero7 + numero7 + numero7 + numero7 + numero7 + numero7 + 7 +
                numero6 + numero6 + numero6 + numero6 + numero6 + numero6 +
                numero5 + numero5 + numero5 + numero5 + numero5 + 5 +
                numero4 + numero4 + numero4 + numero4 +
                numero3 + numero3 + numero3 + 3 +
                numero2 + numero2 +
                1;
            }


            // Devolvemos el string builder con los números primos (en teoría)
            return stb1;
        }

        public static StringBuilder pri322()
        {
            // En este constructor de string guardaremos los números primos que vayamos encontrando separándolos en distintas líneas
            StringBuilder stb1 = new StringBuilder();

            // Número iterativo creciente en el conjunto de los impares (Empezando por el 3)            
            BigInteger numero15 = 15;
            BigInteger numero14 = 14;
            BigInteger numero13 = 13;
            BigInteger numero12 = 12;
            BigInteger numero11 = 11;
            BigInteger numero10 = 10;
            BigInteger numero9 = 9;
            BigInteger numero8 = 8;
            BigInteger numero7 = 7;
            BigInteger numero6 = 6;
            BigInteger numero5 = 5;
            BigInteger numero4 = 4;
            BigInteger numero3 = 3;
            BigInteger numero2 = 2;

            BigInteger resultado =
                //numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 +
                //numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 +
                //numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + 13 +
                //numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + 12 +
                numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + 11 +
                numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 +
                numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + 9 +
                numero8 + numero8 + numero8 + numero8 + numero8 + numero8 + numero8 + numero8 +
                numero7 + numero7 + numero7 + numero7 + numero7 + numero7 + numero7 + 7 +
                numero6 + numero6 + numero6 + numero6 + numero6 + numero6 +
                numero5 + numero5 + numero5 + numero5 + numero5 + 5 +
                numero4 + numero4 + numero4 + numero4 +
                numero3 + numero3 + numero3 + 3 +
                numero2 + numero2 +
                1;

            BigInteger target = 10000000000000000;

            while (resultado < target)
            {
                //if (esPrimoNormal(resultado))
                //{
                    using (StreamWriter writer = new StreamWriter(@"C:\Users\apzyx\OneDrive\Escritorio\a.txt", true))
                    {
                        writer.WriteLine(resultado.ToString().ToString());
                    }
                    //stb1.AppendLine(String.Concat(",", ));
                //}
                //else
                //{
                //    //throw new Exception("ALA ALA:" + resultado.ToString());
                //}
                numero15 *= 15;
                numero14 *= 14;
                numero13 *= 13;
                numero12 *= 12;
                numero11 *= 11;
                numero10 *= 10;
                numero9 *= 9;
                numero8 *= 8;
                numero7 *= 7;
                numero6 *= 6;
                numero5 *= 5;
                numero4 *= 4;
                numero3 *= 3;
                numero2 *= 2;

                resultado =
                    //numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 + numero15 +
                    //numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 + numero14 +
                    //numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + numero13 + 13 +
                    //numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + numero12 + 12 +
                    numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + numero11 + 11 +
                    numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 + numero10 +
                    numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + numero9 + 9 +
                    numero8 + numero8 + numero8 + numero8 + numero8 + numero8 + numero8 + numero8 +
                    numero7 + numero7 + numero7 + numero7 + numero7 + numero7 + numero7 + 7 +
                    numero6 + numero6 + numero6 + numero6 + numero6 + numero6 +
                    numero5 + numero5 + numero5 + numero5 + numero5 + 5 +
                    numero4 + numero4 + numero4 + numero4 +
                    numero3 + numero3 + numero3 + 3 +
                    numero2 + numero2 +
                    1;
            }


            // Devolvemos el string builder con los números primos (en teoría)
            return stb1;
        }



        public static StringBuilder pri()
        {
            // En este constructor de string guardaremos los números primos que vayamos encontrando separándolos en distintas líneas
            StringBuilder stb1 = new StringBuilder();
            stb1.AppendLine(",2");
            // Número iterativo creciente en el conjunto de los impares (Empezando por el 3)
            BigInteger numero = 3;

            List<KeyValuePair<BigInteger, BigInteger>> numerosCompuestos = new List<KeyValuePair<BigInteger, BigInteger>>();
            BigInteger target = 100000;

            while (numero < target)
            {                
                KeyValuePair<BigInteger, BigInteger> numComp;
                Boolean encontrado = false;

                List<KeyValuePair<BigInteger, BigInteger>> numerosAñadir = new List<KeyValuePair<BigInteger, BigInteger>>();

                for (Int32 i = 0; i < numerosCompuestos.Count(); i++)
                {                    
                    if (numerosCompuestos[i].Value == numero)
                    {
                        BigInteger suma = numerosCompuestos[i].Key + numerosCompuestos[i].Value;

                        while(suma % 2 == 0)
                        {
                            suma += numerosCompuestos[i].Key;
                        }

                        numerosCompuestos[i] = new KeyValuePair<BigInteger, BigInteger>(numerosCompuestos[i].Key, suma);
                        encontrado = true;
                        break;
                    }
                    else
                    {
                        BigInteger numeroAñadir = numerosCompuestos[i].Key * numero;
                        if (numeroAñadir <= target)
                        {
                            numerosAñadir.Add(new KeyValuePair<BigInteger, BigInteger>(numeroAñadir, numeroAñadir));
                        }
                    }
                }

                if(!encontrado)
                {
                    numerosCompuestos.Add(new KeyValuePair<BigInteger, BigInteger>(numero, numero * numero));
                    numerosCompuestos.InsertRange(numerosCompuestos.Count(),numerosAñadir);
                    numerosCompuestos = numerosCompuestos.OrderBy(x => x.Key).ToList();

                    stb1.AppendLine(String.Concat(",", numero.ToString()));
                }

                numero += 2;
            }


            // Devolvemos el string builder con los números primos (en teoría)
            return stb1;
        }

        public static StringBuilder priFactorialFilas()
        {

            StringBuilder stb1 = new StringBuilder();
            stb1.AppendLine("2");
            stb1.AppendLine("3");
            stb1.AppendLine("5");
            stb1.AppendLine("7");

            Dictionary<BigInteger, BigInteger> factoriales = new Dictionary<BigInteger, BigInteger>();
            factoriales.Add(1, 1);
            factoriales.Add(2, 2);
            factoriales.Add(3, 6);
            factoriales.Add(4, 24);
            factoriales.Add(5, 120);
            factoriales.Add(6, 720);
            factoriales.Add(7, 5040);
            factoriales.Add(8, 40320);

            // Número iterativo impar para verificar si es primo o no
            BigInteger numero = new BigInteger(9);

            // Realizamos el bucle tantas veces como queramos, número iterativo a cascoporro
            // TODO: Realizar bucle infinito para sacar resultados
            for (Int32 i = 0; i < 10000; i++)
            {
                BigInteger factorialN = factoriales[numero - 1] * numero;
                factoriales[numero] = factorialN;

                BigInteger r = new BigInteger(3);

                Boolean esPrimo = true;
                while (r < ((numero / 4) + 2))
                {
                    BigInteger resultado = factorialN / (factoriales[r] * factoriales[numero - r]);

                    if(BigInteger.Remainder(resultado, numero) != 0)
                    {
                        esPrimo = false;
                        break;
                    }
                    r++;
                }

                if(esPrimo)
                {
                    stb1.AppendLine(numero.ToString());
                }

                numero++;
                factoriales[numero] = factorialN * numero;
                numero++;
            }

            return stb1;

        }

        public static StringBuilder priPotencia()
        {
            StringBuilder stb1 = new StringBuilder();

            Int32 numero = 1;
            for (Int32 i = 0; i < 100; i++)
            {
                BigInteger primeraPotencia = BigInteger.Pow(numero, numero);

                BigInteger total = BigInteger.Pow(primeraPotencia, numero) + primeraPotencia + numero;

                stb1.AppendLine(total.ToString());

                numero++;
            }

            return stb1;
        }


        /// <summary>(No funciona, serie del maravilloso 25) Calculo teórico de números primos en base a la sucesión Segner</summary>        
        public static StringBuilder priFactorial()
        {
            StringBuilder stb1 = new StringBuilder();

            // Número iterativo impar para verificar si es primo o no
            BigInteger numero = new BigInteger(7);

            // Realizamos el bucle tantas veces como queramos, número iterativo a cascoporro
            // TODO: Realizar bucle infinito para sacar resultados
            for (Int32 i = 0; i < 10000; i++)
            {
                // Lógica basada en los números Segner
                BigInteger numerador = factorial(2 * (numero - 2));
                BigInteger divisor = (numero - 1) * BigInteger.Pow(factorial(numero - 2), 2);                
                BigInteger division = BigInteger.Divide(numerador, divisor);
                BigInteger divisionRestada = division - (((numero - 1) * ((numero - 1) / 2)) + ((numero - 1) / 2));
                // Si el resto de la operación es 0 el número es primo
                if ( BigInteger.Remainder(divisionRestada, numero) == 0)
                {
                    stb1.AppendLine(numero.ToString());
                }

                numero += 2;
            }

            return stb1;
        }

        /// <summary>Método que calcula el factorial de un número BigInteger</summary>        
        private static BigInteger factorial(BigInteger numero)
        {
            if (facts.ContainsKey(numero))
            {
                return facts[numero];
            }

            if (facts.ContainsKey(numero - 1))
            {
                facts.Add(numero, facts[numero - 1] * numero);
                return facts[numero];
            }

            //Variable de la cuenta del factorial
            BigInteger resultado = new BigInteger(1);
            BigInteger temp = numero;
            // Vamos multiplicando al número sus antecesores
            while (temp > 1)
            {
                resultado *= temp;
                temp--;
            }

            facts.Add(numero, resultado);

            // Devolvemos el factorial
            return resultado;
        }

        private static BigInteger factorialImpar2Siguiente(BigInteger numero)
        {
            if (numero % 2 == 0)
            {
                throw new Exception("Por favor, introduzca un número impar");
            }

            //Variable de la cuenta del factorial
            BigInteger resultado = 1;
            BigInteger i = numero;
            // Vamos multiplicando al número sus antecesores
            while (i > 2)
            {
                resultado *= i;
                i -= 2;
                                
            }
            // Devolvemos el factorial
            return resultado * 2 * (numero + 2);
        }

        /// <summary>
        /// Este problema se deriva de la formulas de los números tetraédricos para conseguir númmeros primos según su "diagonal" de representación,
        /// donde todos ellos son divisibles por ella, siempre y cuando sean primos
        /// </summary>
        public static StringBuilder priTetraedrico()
        {
            // En este constructor de string guardaremos los números primos que vayamos encontrando separándolos en distintas líneas
            StringBuilder stb1 = new StringBuilder();

            // Número iterativo creciente en el conjunto de los impares (Empezando por el 3)
            BigInteger numero = new BigInteger(3);

            // Este será el número de iteraciones con las que haremos las pruebas modificando de momento el número a cascoporro
            // TODO: BUCLE WHILE HASTA EL INFINITO Y MÁS HAYá (De momento estamos en beta)
            for (Int32 i = 0; i < 10000; i++)
            {                
                // Total de la fórmula tetraédrica según nivel de inmersión en ella 
                BigInteger multiplicandos = numero * (numero - 1) * (numero - 2);
                // Divisor creciente según fórmula tetraédrica y su nivel del inmersión en ella
                BigInteger divisor = new BigInteger(6);
                // Contador de inmersión en fórmula tetraédrica
                BigInteger contadorM = 4;

                // Nivel de profundidad máximo que tenemos que alcanzar para comprobarlo como primo
                // La mitad + 1 de la diagonal tetraédrica
                BigInteger finalProceso = (numero / 2) - 1;
                BigInteger contadorProceso = 1;

                Boolean esPrimo = true;
                while (contadorProceso <= finalProceso)
                {
                    // Comprobamos que es divisible exacto por el choriso según teoría
                    if(BigInteger.Remainder((multiplicandos / divisor), numero) != 0)
                    {
                        esPrimo = false;
                        break;
                    }


                    // Preparamos siguiente iteración según teoría
                    multiplicandos *= (numero - contadorM + 1);
                    divisor *= contadorM;

                    // Incrementamos contadores
                    contadorM++;
                    contadorProceso++;
                }

                // Si es un número primo según el cáculo anterior lo añadimos a la lista de primos como una nueva línea al SB
                if(esPrimo)
                {
                    stb1.AppendLine(numero.ToString());
                }

                // Sólo nos interesan los número impares, que son los candidatos a primos, (todo par es divisible entre 2) (A Grosso modo) =P
                numero += 2;
            }

            // Devolvemos el string builder con los números primos (en teoría)
            return stb1;
        }

        public static StringBuilder priMitad1()
        {
            StringBuilder stb1 = new StringBuilder();
            stb1.AppendLine("2");
            stb1.AppendLine("3");

            BigInteger numero = new BigInteger(3);


            for (Int32 i = 0; i < 10000; i++)
            {

                BigInteger nDiv = BigInteger.Divide(numero, 2);
                BigInteger temp = nDiv;
                nDiv += numero + 1;
                

                BigInteger remainder = BigInteger.Remainder(nDiv, 2);
                if(remainder == 0)
                {
                    nDiv -= 1;
                }

                stb1.AppendLine(nDiv.ToString().PadRight(5, ' ') + " - " + temp );

                numero += 2;
            }

            return stb1;
        }

        public static StringBuilder convertirABaseX(Int32 numero, Int32 xBase)
        {
            String resultado = String.Empty;
            Int32 baseTemp = 0;
            for (Int32 i = 0; i < numero; i++)
            {
                if(baseTemp == xBase)
                {

                }
                else
                {

                }
            }
            return null;
        }

        public static StringBuilder priCribaEratostenes()
        {
            StringBuilder stb1 = new StringBuilder();
            stb1.AppendLine("2");

            List<Int32> numerosTemp = new List<Int32>();
            
            for (Int32 i = 3; i < 1000000; i += 2)
            {
                numerosTemp.Add(i);
            }

            List<Int32> deleteNums = new List<Int32>();

            Int32 j = numerosTemp[0];
            while (j < 500000)
            {
                foreach (Int32 n in numerosTemp)
                {
                    if (n % j == 0 && n / j != 1) { deleteNums.Add(n); }
                }

                foreach (Int32 dN in deleteNums)
                {
                    numerosTemp.Remove(dN);
                }
                deleteNums = new List<Int32>();

                foreach (Int32 nJ in numerosTemp)
                {
                    if (nJ > j)
                    {
                        j = nJ;
                        break;
                    }
                }
            }


            foreach (Int32 nF in numerosTemp)
            {
                stb1.AppendLine(nF.ToString());
            }


            return stb1;
        }

        public static StringBuilder priCribaEratostenes10000()
        {
            StringBuilder stb1 = new StringBuilder();
            //stb1.AppendLine("2".PadLeft(5, ' '));

            List<Int32> numerosTemp = new List<Int32>();

            for (Int32 i = 3; i < 1000; i += 2)
            {
                numerosTemp.Add(i);
            }

            List<Int32> deleteNums = new List<Int32>();

            Int32 j = numerosTemp[0];
            while (j < 500)
            {
                foreach (Int32 n in numerosTemp)
                {
                    if (n % j == 0 && n / j != 1) { deleteNums.Add(n); }
                }

                foreach (Int32 dN in deleteNums)
                {
                    numerosTemp.Remove(dN);
                }
                deleteNums = new List<Int32>();

                foreach (Int32 nJ in numerosTemp)
                {
                    if (nJ > j)
                    {
                        j = nJ;
                        break;
                    }
                }
            }

            for (Int32 i = 1; i < 10000; i++)
            {
                Boolean esPrimo = numerosTemp.Contains(i);
                String numero = esPrimo ? ("*" + i.ToString()).PadLeft(7, ' ') : i.ToString().PadLeft(7, ' ');
                stb1.Append(numero);
                if(i % 9 == 0)
                {
                    stb1.AppendLine("");
                }
            }

            //foreach (Int32 nF in numerosTemp)
            //{
            //    stb1.AppendLine(nF.ToString());
            //}


            return stb1;
        }

        public static void priNormalRestaura()
        {
            // Lista en memoria donde tendremos todos los número primos
            StringBuilder stb1 = new StringBuilder();

            // Recogemos los primos ya procesados si los hubiera
            using (StreamReader writer = new StreamReader(@"C:\Users\apzyx\OneDrive\Escritorio\pri.txt"))
            {
                String s = writer.ReadLine();
                String temp = String.Empty;
                while (s != null)
                {
                    if (s != temp)
                    {
                        stb1.AppendLine(s);
                    }
                    temp = s;
                    s = writer.ReadLine();
                }
            }

            using (StreamWriter writer = new StreamWriter(@"C:\Users\apzyx\OneDrive\Escritorio\a.txt"))
            {
                writer.WriteLine(stb1.ToString());
            }


        }

        public static void priNormal()
        {
            // Lista en memoria donde tendremos todos los número primos
            List<BigInteger> primos = new List<BigInteger>();

            // Recogemos los primos ya procesados si los hubiera
            using (StreamReader writer = new StreamReader(@"C:\Users\apzyx\OneDrive\Escritorio\pri.txt"))
            {
                String s = writer.ReadLine();
                while (s != null)
                {
                    primos.Add(BigInteger.Parse(s.Replace(",", "")));
                    s = writer.ReadLine();
                }
            }

            BigInteger numero = primos.Count() == 0 ? new BigInteger(3) : primos.LastOrDefault() + 2;
                                    
            while (true)
            {
                Boolean esPrimo = true;
                BigInteger maxNum = numero / 2;
                foreach (BigInteger primo in primos)
                {
                    if (BigInteger.Remainder(numero, primo) == 0)
                    {
                        esPrimo = false;
                        break;
                    }

                    if(primo > maxNum)
                    {
                        break;
                    }
                }


                // Si el número se considera primo se guarda en memoria y se graba en el archivo de histórico
                if(esPrimo)
                {
                    primos.Add(numero);
                    using (StreamWriter writer = new StreamWriter(@"C:\Users\apzyx\OneDrive\Escritorio\pri.txt", true))
                    {
                        String numString = numero.ToString();
                        writer.WriteLine("," + numString);
                        Console.WriteLine(numString);
                    }                                       
                }

                // Incrementamos a sólo impares (que son los candidatos de primos)
                numero += 2;
            }
        }


        /// <summary>Suma los números enteros en String que se incluyen en el parámetro de entrada</summary>
        public async Task<String> Sum(List<String> numeros, Boolean update)
        {
            // Obtenemos el tamaño del string más largo
            Int32 maxL = 0;
            foreach (String s in numeros)
            {
                if (s.Length > maxL)
                {
                    maxL = s.Length;
                }
            }

            // Proceso de suma de strings
            Int32 acarreo = 0;
            Int32 posicion = 0;
            String resultado = String.Empty;
            for (Int32 i = 0; i < maxL; i++)
            {
                posicion = 0;
                foreach (String s in numeros)
                {
                    Int32 j = s.Length - 1 - i;
                    if (j >= 0)
                    {
                        posicion += Convert.ToInt32(s[j].ToString());
                    }
                }

                posicion = posicion + acarreo;
                acarreo = 0;
                Int32 k = posicion.ToString().Length - 1;
                resultado = posicion.ToString()[k] + resultado;

                if (k >= 1)
                {
                    acarreo = Convert.ToInt32(posicion.ToString().Substring(0, k));
                }
            }

            if (acarreo != 0) { resultado = acarreo.ToString() + resultado; }

            return resultado;
        }

        /// <summary>Suma dos números enteros en String</summary>
        public static String Sum2(String n1, String n2)
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
            Int32 acarreo = 0;
            Int32 posicion = 0;
            String resultado = String.Empty;
            for (Int32 i = 0; i < maxL; i++)
            {
                posicion = 0;
                Int32 j1 = n1.Length - 1 - i;
                if (j1 >= 0)
                {
                    posicion += Convert.ToInt32(n1[j1].ToString());
                }
                Int32 j2 = n2.Length - 1 - i;
                if (j2 >= 0)
                {
                    posicion += Convert.ToInt32(n2[j2].ToString());
                }

                posicion = posicion + acarreo;
                acarreo = 0;
                Int32 k = posicion.ToString().Length - 1;
                resultado = posicion.ToString()[k] + resultado;

                if (k >= 1)
                {
                    acarreo = Convert.ToInt32(posicion.ToString().Substring(0, k));
                }
            }

            if (acarreo != 0) { resultado = acarreo.ToString() + resultado; }

            return resultado;
        }

        /// <summary>Resta dos números enteros en String</summary>
        public static String Res2(String n1, String n2, Boolean update)
        {
            Boolean? initN1N2 = Compare(n1, n2);
            if (!initN1N2.HasValue) { return "0"; }
            if (initN1N2.HasValue && !initN1N2.Value) { return "N1 debe ser mayor o igual a N2 / N1 must be greater than or equal to N2"; }

            String resultado = String.Empty;

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
            Int32 acarreo = 0;
            Int32 p1 = 0;
            Int32 p2 = 0;
            Int32 posicion = 0;
            for (Int32 i = 0; i < maxL; i++)
            {
                p1 = 0;
                Int32 j1 = n1.Length - 1 - i;
                if (j1 >= 0)
                {
                    p1 = Convert.ToInt32(n1[j1].ToString());
                }
                Int32 j2 = n2.Length - 1 - i;
                p2 = 0;
                if (j2 >= 0)
                {
                    p2 = Convert.ToInt32(n2[j2].ToString());
                }

                if (p1 < p2 + acarreo)
                {
                    p1 = p1 + 10;
                    posicion = p1 - (p2 + acarreo);
                    acarreo = 1;
                }
                else
                {
                    posicion = p1 - (p2 + acarreo);
                    acarreo = 0;
                }

                resultado = posicion.ToString() + resultado;
            }

            return resultado.TrimStart('0');
        }

        /// <summary>Método temporal para multiplicar dos números enteros en String</summary>
        public static String Mult2Temp(String numero, String multiplicador)
        {
            String resultado = String.Empty;
            String i = "0";
            while (Compare(i, multiplicador).HasValue)
            {
                resultado = Sum2(resultado, numero);
                i = Sum2(i, "1");
            }
            return resultado;
        }


        /// <summary>Multiplica dos números enteros en String</summary>
        public static String Mult2(String n1, String n2)
        {
            String resultado = String.Empty;
            Int32 num0s = 0;

            Int32 j = 0;
            for (Int32 i = n2.Length - 1; i >= 0; i--)
            {
                String resultTempMult = Mult2Temp(n1, n2[i].ToString());
                resultTempMult = resultTempMult.PadRight(resultTempMult.Length + num0s, '0');
                resultado = Sum2(resultado, resultTempMult);
                num0s++;

                j++;
            }

            return resultado;
        }

        /// <summary>Método temporal que divide dos números enteros en String</summary>
        public static String Div2Temp(String divisor, String dividendo)
        {
            if (dividendo.TrimStart('0') == String.Empty) { return "Infinito / Infinity"; }
            if (divisor.TrimStart('0') == String.Empty) { return "0"; }
            Boolean? initNumDiv = Compare(divisor, dividendo);
            if (initNumDiv.HasValue && !initNumDiv.Value) { return "N1 debe ser mayor o igual a N2 / N1 must be greater than or equal to N2"; }

            String resultado = String.Empty;

            String dividendoTemp = "0";
            String iAnt = String.Empty;
            String i = "0";
            Boolean? continueWhile = true;
            while (continueWhile.HasValue && continueWhile.Value)
            {
                dividendoTemp = Sum2(dividendoTemp, dividendo);
                continueWhile = Compare(divisor, dividendoTemp);
                iAnt = i;
                i = Sum2(i, "1");
            }

            if (!continueWhile.HasValue)
            {
                resultado = i;
            }
            else
            {
                resultado = iAnt;
            }

            return resultado;
        }

        /// <summary>Divide dos números enteros en String</summary>
        public static DivisionEntera Div2(String divisor, String dividendo, Boolean update)
        {
            if (dividendo.TrimStart('0') == String.Empty) { return new DivisionEntera() { Resultado = "Infinito / Infinity", EsExacta = false }; }
            if (divisor.TrimStart('0') == String.Empty) { return new DivisionEntera() { Resultado = "0", EsExacta = true }; }
            Boolean? initNumDiv = Compare(divisor, dividendo);
            if (initNumDiv.HasValue && !initNumDiv.Value) { return new DivisionEntera() { Resultado = "N1 debe ser mayor o igual a N2 / N1 must be greater than or equal to N2", EsExacta = false }; }

            Boolean isFirst = true;
            String resultado = String.Empty;
            String divisorTemp = String.Empty;
            Int32 i = 0;
            while (i < divisor.Length)
            {
                divisorTemp = divisorTemp + divisor[i];

                Boolean? resTemp = Compare(divisorTemp, dividendo);

                if (!resTemp.HasValue || resTemp.Value)
                {
                    isFirst = false;
                    String result = Div2Temp(divisorTemp, dividendo);
                    resultado += result;
                    String resto = Res2(divisorTemp, Mult2(dividendo, result), false);
                    divisorTemp = resto;
                }
                else
                {
                    if (!isFirst)
                    {
                        resultado += "0";
                    }
                }

                i++;
            }

            DivisionEntera dve = new DivisionEntera();
            dve.Resultado = resultado;
            dve.EsExacta = true;

            if (divisorTemp.TrimStart('0') != String.Empty) { dve.EsExacta = false; dve.Resto = divisorTemp.TrimStart('0'); }

            return dve;
        }

        /// <summary>Compara dos números para comprobar si es mayor, menor o igual</summary>
        /// <returns>True si el principal es mayor, False si es menor o Null si son iguales</returns>
        public static Boolean? Compare(String nPrincipal, String nSecundario)
        {
            String nPrin = nPrincipal.TrimStart(new char[1] { '0' });
            String nSec = nSecundario.TrimStart(new char[1] { '0' });

            if (nPrin == nSec)
            {
                return null;
            }
            else
            {
                if (nPrin.Length > nSec.Length)
                {
                    return true;
                }
                else if (nPrin.Length < nSec.Length)
                {
                    return false;
                }
                else
                {
                    for (Int32 i = 0; i < nPrin.Length; i++)
                    {
                        Int32 xPrin = Convert.ToInt32(nPrin[i].ToString());
                        Int32 xSec = Convert.ToInt32(nSec[i].ToString());

                        if (xPrin > xSec)
                        {
                            return true;
                        }
                        else if (xPrin < xSec)
                        {
                            return false;
                        }
                    }

                    return null;
                }
            }
        }
        public async Task<StringBuilder> pri1()
        {
            StringBuilder stb1 = new StringBuilder();
            stb1.AppendLine("2");
            stb1.AppendLine("3");

            String i = "5";
            Boolean? paraBucle = false;
            while (!paraBucle.HasValue || !paraBucle.Value)
            {
                Boolean? paraBucleRetro = false;
                String max = Sum2((Div2(i, "2", false)).Resultado, "1");
                String j = "3";
                Boolean esPrimo = true;
                while (!paraBucleRetro.HasValue || !paraBucleRetro.Value)
                {
                    DivisionEntera dve = Div2(i, j, false);

                    if (dve.EsExacta)
                    {
                        esPrimo = false;
                        break;
                    }

                    j = Sum2(j, "2");
                    paraBucleRetro = Compare(j, max);
                }

                if (esPrimo)
                {
                    stb1.AppendLine(i);
                }

                paraBucle = Compare(i, "999");
                i = Sum2(i, "2");
            }


            return stb1;
        }

        
        public static StringBuilder priCribaEratostenesBigInteger()
        {
            StringBuilder stb1 = new StringBuilder();
            stb1.AppendLine("2");

            List<BigInteger> numerosTemp = new List<BigInteger>();
            BigInteger i = new BigInteger(3);
            Boolean salirBucle = i >= new BigInteger(100000000);
            while (!salirBucle)
            {
                numerosTemp.Add(i);
                i = i + 2;
                salirBucle = i >= new BigInteger(100000000);
            }


            List<BigInteger> deleteNums = new List<BigInteger>();

            BigInteger j = numerosTemp[0];
            Boolean salirBucleRemove = j >= new BigInteger(50000000);
            while (!salirBucleRemove)
            {
                foreach (BigInteger n in numerosTemp)
                {
                    BigInteger reminder = new BigInteger();
                    BigInteger dve = BigInteger.DivRem(n, j, out reminder);
                    if (reminder == 0 && dve != 1) { deleteNums.Add(n); }
                }

                foreach (BigInteger dN in deleteNums)
                {
                    numerosTemp.Remove(dN);
                }
                deleteNums = new List<BigInteger>();

                foreach (BigInteger nJ in numerosTemp)
                {
                    Boolean esMyorNJ = nJ > j;
                    if (esMyorNJ)
                    {
                        j = nJ;
                        break;
                    }
                }

                salirBucleRemove = j >= new BigInteger(50000000);
            }


            foreach (BigInteger nF in numerosTemp)
            {
                stb1.AppendLine(nF.ToString());
            }


            return stb1;
        }

        public static StringBuilder priSucesionCuadratica()
        {
            StringBuilder stb1 = new StringBuilder();
            
            List<BigInteger> primos = new List<BigInteger>();
            BigInteger num = new BigInteger(2);
            BigInteger IntegersPrim =  new BigInteger(30);

            for (Int32 i = 0; i < 10000; i++)
            {
                // INI Aquí comprobamos que el número sera primo según la regla de las sucesiones                                
                BigInteger reminder = BigInteger.Remainder(IntegersPrim, 2);
                if (reminder != 0)
                {
                    Boolean esPrimo = true;
                    foreach (BigInteger pri in primos)
                    {
                        if (BigInteger.Remainder(IntegersPrim, pri) == 0)
                        {
                            esPrimo = false;
                            break;
                        }
                    }

                    if (esPrimo)
                    {
                        primos.Add(IntegersPrim);
                        stb1.AppendLine(IntegersPrim.ToString());
                    }
                    
                }
                // FIN Aquí comprobamos que el número sera primo según la regla de las sucesiones


                // INI MONTAMOS LA SUCESIóN DE LA SIGUIENTE ITERACIóN                
                BigInteger temp = num;
                num = num + 1;                
                IntegersPrim = (temp - 1) + (temp * (temp + 1));
                // FIN MONTAMOS LA SUCESIóN DE LA SIGUIENTE ITERACIóN
            }

            return stb1;
        }

        public static StringBuilder priSucesionErratico()
        {
            StringBuilder stb1 = new StringBuilder();
            stb1.AppendLine("2");
            stb1.AppendLine("3");

            BigInteger num = new BigInteger(5);
            
            List<BigInteger> IntegersPrim = new List<BigInteger>();            
            IntegersPrim.Add(new BigInteger(5));
            IntegersPrim.Add(new BigInteger(10));
            Boolean add = true;
            for (Int32 i = 0; i < 10000; i++)
            {
                // INI Aquí comprobamos que el número sera primo según la regla de las sucesiones
                Boolean esPrimo = true; 
                BigInteger presuntoPrimo = IntegersPrim[0];
                for (Int32 j = 1; j < IntegersPrim.Count(); j++)
                {
                    BigInteger reminder = BigInteger.Remainder(IntegersPrim[j], presuntoPrimo);
                    if (reminder != 0)
                    {
                        esPrimo = false;
                        break;
                    }
                }
                if (esPrimo)
                {
                    stb1.AppendLine(presuntoPrimo.ToString());
                }
                // FIN Aquí comprobamos que el número sera primo según la regla de las sucesiones


                // INI MONTAMOS LA SUCESIóN DE LA SIGUIENTE ITERACIóN                
                BigInteger temp = num;
                IntegersPrim[0] = num + 1;
                BigInteger temp2 = new BigInteger(0);
                for (Int32 j = 1; j < IntegersPrim.Count(); j++)
                {
                    if (temp2 != 0)
                    {
                        temp = temp2;
                    }

                    temp2 = IntegersPrim[j];

                    IntegersPrim[j] = IntegersPrim[j] + temp;

                }

                if (add)
                {
                    IntegersPrim.Add(temp2 * 2);
                    add = false;
                }
                else
                {
                    add = true;
                }

                // FIN MONTAMOS LA SUCESIóN DE LA SIGUIENTE ITERACIóN

                num++;
            }
            
            return stb1;
        }

        public static StringBuilder priSucesionErraticoMenos2()
        {
            StringBuilder stb1 = new StringBuilder();
            stb1.AppendLine("2");
            stb1.AppendLine("3");
            stb1.AppendLine("5");

            BigInteger num = new BigInteger(7);

            List<BigInteger> IntegersPrim = new List<BigInteger>();            
            IntegersPrim.Add(new BigInteger(21));
            IntegersPrim.Add(new BigInteger(35));
            Boolean add = true;
            for (Int32 i = 0; i < 10000; i++)
            {
                if (add)
                {
                    // INI Aquí comprobamos que el número sera primo según la regla de las sucesiones
                    Boolean esPrimo = true;
                    BigInteger presuntoPrimo = num;
                    
                    BigInteger reminder = BigInteger.Remainder(IntegersPrim[IntegersPrim.Count() - 2], presuntoPrimo);
                    if (reminder != 0)
                    {
                        esPrimo = false;
                    }
                    
                    if (esPrimo)
                    {
                        stb1.AppendLine(presuntoPrimo.ToString());
                    }
                    // FIN Aquí comprobamos que el número sera primo según la regla de las sucesiones
                }

                // INI MONTAMOS LA SUCESIóN DE LA SIGUIENTE ITERACIóN                
                BigInteger temp = num;
                num = num + 1;
                BigInteger temp2 = new BigInteger(0);
                for (Int32 j = 0; j < IntegersPrim.Count(); j++)
                {
                    if (temp2 != 0)
                    {
                        temp = temp2;
                    }

                    temp2 = IntegersPrim[j];

                    IntegersPrim[j] = IntegersPrim[j] + temp;

                }

                if (add)
                {
                    IntegersPrim.Add(temp2 * 2);
                    add = false;
                }
                else
                {
                    add = true;
                }
                // FIN MONTAMOS LA SUCESIóN DE LA SIGUIENTE ITERACIóN

            }

            return stb1;
        }

        public static StringBuilder priNormalPrimos2()
        {            
            List<BigInteger> primos = new List<BigInteger>();
            
            BigInteger num = new BigInteger(3);
            
            for (Int32 i = 0; i < 100000; i++)
            {
                // INI Aquí comprobamos que el número sera primo según la regla de las sucesiones                
                BigInteger presuntoPrimo = num;
                if (BigInteger.Remainder(presuntoPrimo, 2) != 0)
                {
                    Boolean esPrimo = true;
                    foreach(BigInteger pri in primos)
                    {
                        if(BigInteger.Remainder(presuntoPrimo, pri) == 0)
                        {
                            esPrimo = false;
                            break;
                        }
                    }

                    if (esPrimo)
                    {                        
                        primos.Add(presuntoPrimo);
                    }
                }

                num = num + 2; ;
            }

            StringBuilder stb1 = new StringBuilder();
            primos.Insert(0, new BigInteger(2));
            foreach(BigInteger primo in primos)
            {
                stb1.AppendLine(primo.ToString());
            }

            return stb1;
        }

        public static StringBuilder priDivision2()
        {
            StringBuilder stb1 = new StringBuilder();
            stb1.AppendLine("2");
            stb1.AppendLine("3");
            stb1.AppendLine("5");

            BigInteger num = new BigInteger(7);
            BigInteger divisor2 = new BigInteger(2);

            for (Int32 i = 0; i < 1000; i++)
            {
                BigInteger divResult = BigInteger.Divide(num, divisor2);

                BigInteger div1 = divResult - 1;
                BigInteger div2 = divResult + 2;


                List<BigInteger> factoresPrimos = new List<BigInteger>();
                BigInteger tempNUM = div1;
                BigInteger k = divisor2;
                BigInteger resi;
                while ((tempNUM != 1))
                {
                    resi = tempNUM % k;
                    if ((resi == 0))
                    {
                        factoresPrimos.Add(k);
                        //textBox2.Text = cadena.Remove(cadena.Length - 2);
                        tempNUM = tempNUM / k;
                    }
                    else
                    {
                        k = k + 1;
                    }
                }

                tempNUM = div2;
                k = divisor2;
                while ((tempNUM != 1))
                {
                    resi = tempNUM % k;
                    if ((resi == 0))
                    {
                        factoresPrimos.Add(k);
                        //textBox2.Text = cadena.Remove(cadena.Length - 2);
                        tempNUM = tempNUM / k;
                    }
                    else
                    {
                        k = k + 1;
                    }
                }

                Boolean esPrimo = true;
                foreach (BigInteger factor in factoresPrimos)
                {
                    
                    if (BigInteger.Remainder(num, factor) == 0)
                    { 
                        esPrimo = false; break;
                    }                    
                }
                if (esPrimo) { stb1.AppendLine(num.ToString()); }

                num = num + 2;
            }

            return stb1;
        }

        public static StringBuilder priSucesion()
        {
            StringBuilder stb1 = new StringBuilder();
            //stb1.AppendLine("2");
            //stb1.AppendLine("3");

            BigInteger num = new BigInteger(1);
            BigInteger[][] matrix = new BigInteger[1000][];
            for (Int32 i = 0; i < 1000; i++)
            {
                BigInteger[] fila = new BigInteger[1000];
                for (Int32 j = 0; j < 1000; j++)
                {                    
                    if (i > 0)
                    {
                        fila[j] = num;
                        if (j + 1 < 1000)
                        {
                            num = BigInteger.Add(num, matrix[i - 1][j + 1]);
                        }
                    }
                    else
                    {
                        fila[j] = num;
                        num = BigInteger.Add(num, new BigInteger(1));
                    }
                }
                num = new BigInteger(1);
                matrix[i] = fila;
            }

            String pausa = "pausa";

            // INI COPIA DE LA MATRIZ A ARCHIVO CSV
            //for (Int32 i = 0; i < 20; i++)
            //{
            //    for (Int32 j = 0; j < 20; j++)
            //    {
            //        stb1.Append(matrix[i][j].ToString().PadLeft(9, ' ') + ",");
            //    }
            //    stb1.AppendLine();
            //    stb1.AppendLine();
            //    stb1.AppendLine();
            //}


            //return stb1;

            // FIN COPIA DE LA MATRIZ A ARCHIVO CSV


            for (Int32 k = 3; k < 1000; k++)
            {
                BigInteger numero = matrix[0][k];

                BigInteger validador = new BigInteger(-1);

                Int32 i = 1;
                for (Int32 j = k - 1; j >= 0; j--)
                {
                    BigInteger numeroSucesion = matrix[i][j];

                    if (numeroSucesion == validador) { stb1.AppendLine(numero.ToString()); break; }
                    BigInteger remainder = new BigInteger(0);
                    BigInteger.DivRem(numeroSucesion, numero, out remainder);
                    if (BigInteger.Compare(remainder, new BigInteger(0)) != 0) { break; }
                    validador = numeroSucesion;
                    i++;
                }
            }

            return stb1;
        }

        /// <summary>Devuelve si es número es par o no</summary>
        public static Boolean EsPar(String n1)
        {
            DivisionEntera dve = Div2(n1, "2", false);
            return dve.EsExacta;
        }

        public async Task<ParPuro> EsParPuro(String n1)
        {
            DivisionEntera dve = null;
            String ordenCalculo = "0";
            while (dve == null || (dve.EsExacta && dve.Resultado != "1"))
            {
                dve = Div2(dve == null ? n1 : dve.Resultado, "2", false);
                ordenCalculo = Sum2(ordenCalculo, "1");
            }

            if (dve != null && dve.EsExacta && dve.Resultado == "1")
            {
                return new ParPuro() { EsParPuro = true, OrdenCalculo = ordenCalculo, Numero = n1 };
            }
            else
            {
                return new ParPuro() { EsParPuro = false };
            }
        }

        private static BigInteger raizCuadradaMasTres(BigInteger elemento, out BigInteger exacta0)
        {
            return raizCuadrada(elemento, out exacta0) + 3;
        }

        private static BigInteger raizCuadradaMasUno(BigInteger elemento, out BigInteger exacta0)
        {
            return raizCuadrada(elemento, out exacta0) + 1;
        }

        private static BigInteger raizCuadrada(BigInteger elemento, out BigInteger exacta0)
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

        public static BigInteger CubeRootRem(BigInteger number, out BigInteger remainder)
        {
            remainder = 0;
            if (number == 0) return 0;

            BigInteger left = BigInteger.One;
            BigInteger right = number;
            while (left < right)
            {
                BigInteger mid = (left + right) / 2;
                BigInteger midCubed = BigInteger.Pow(mid, 3);

                int comparison = midCubed.CompareTo(number);
                if (comparison == 0)
                {
                    remainder = 0;
                    return mid;
                }
                else if (comparison < 0)
                    left = mid + 1;
                else
                    right = mid;
            }

            // Calcula el sobrante.
            BigInteger root = (BigInteger.Pow(left, 3) > number) ? left - 1 : left;
            remainder = number - BigInteger.Pow(root, 3);

            return root;
        }

        public static BigInteger CubeRoot(BigInteger number)
        {
            if (number == 0) return 0;

            BigInteger left = BigInteger.One;
            BigInteger right = number;
            while (left < right)
            {
                BigInteger mid = (left + right) / 2;
                BigInteger midCubed = BigInteger.Pow(mid, 3);

                int comparison = midCubed.CompareTo(number);
                if (comparison == 0)
                    return mid;
                else if (comparison < 0)
                    left = mid + 1;
                else
                    right = mid;
            }

            // If we overshoot and the cube of left is greater than the number,
            // return the previous number.
            return (BigInteger.Pow(left, 3) > number) ? left - 1 : left;
        }


        private static BigInteger raizX(BigInteger elemento, Int32 exp, out BigInteger exacta0)
        {
            String sEle = elemento.ToString();

            Stack<String> aSE = new Stack<String>();

            Int32 iAntL = -1;
            for (Int32 i = sEle.Length - exp; i > -1; i -= exp)
            {
                iAntL = i;
                aSE.Push(sEle.Substring(i, exp));
            }

            
            if (iAntL == -1)
            {
                aSE.Push(sEle.Substring(0));
            }
            else if (iAntL < exp)
            {
                aSE.Push(sEle.Substring(0, iAntL));
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

                BigInteger m = String.IsNullOrEmpty(resultString) ? 1 : BigInteger.Parse((BigInteger.Pow(BigInteger.Parse(resultString), exp - 1) * exp) + "1");
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

        ///// <summary>Suma dos números enteros en String</summary>
        //public String Sum2Temp(String n1, String n2, Boolean update)
        //{
        //    // Obtenemos el tamaño del string más largo
        //    Int32 maxL = 0;
        //    if (n1.Length > n2.Length)
        //    {
        //        maxL = n1.Length;
        //    }
        //    else
        //    {
        //        maxL = n2.Length;
        //    }

        //    if (update)
        //    {
        //        UpdateProgressBarMaximun(maxL - 1);
        //        UpdateProgressBarValue(0);
        //    }

        //    // Proceso de suma de strings
        //    Int32 acarreo = 0;
        //    Int32 posicion = 0;
        //    String resultado = String.Empty;
        //    for (Int32 i = 0; i < maxL; i += 8)
        //    {
        //        if (update)
        //        {
        //            UpdateProgressBarValue(i);
        //        }

        //        posicion = 0;
        //        Int32 j1 = n1.Length - 1 - i;
        //        if (j1 >= 0)
        //        {
        //            posicion += j1 - 8 >= 0 ? Convert.ToInt32(n1.Substring(j1 - 7, 8)) : Convert.ToInt32(n1.Substring(0, j1 + 1));
        //        }
        //        else if (j1 + 8 >= 0 && j1 + 8 < n1.Length - i)
        //        {
        //            posicion += Convert.ToInt32(n1.Substring(0, j1 + 8 - 1));
        //        }
        //        Int32 j2 = n2.Length - 1 - i;
        //        if (j2 >= 0)
        //        {
        //            posicion += j2 - 8 >= 0 ? Convert.ToInt32(n2.Substring(j2 - 7, 8)) : Convert.ToInt32(n2.Substring(0, j2 + 1));
        //        }
        //        else if (j2 + 8 >= 0 && j2 + 8 < n2.Length - i)
        //        {
        //            posicion += Convert.ToInt32(n2.Substring(0, j2 + 8 - 1));
        //        }

        //        posicion += acarreo;
        //        acarreo = 0;
        //        Int32 posLength = posicion.ToString().Length;
        //        Int32 k = posLength > 8 ? 1 : 0;
        //        resultado = posicion.ToString().Substring(k).PadLeft(8, '0') + resultado;

        //        if (k > 0)
        //        {
        //            acarreo = Convert.ToInt32(posicion.ToString().Substring(0, k));
        //        }
        //    }

        //    if (acarreo != 0) { resultado = acarreo.ToString() + resultado; }
        //    if (update)
        //    {
        //        UpdateProgressBarValue(maxL - 1);
        //    }
        //    return resultado.TrimStart('0');
        //}
    }
}
