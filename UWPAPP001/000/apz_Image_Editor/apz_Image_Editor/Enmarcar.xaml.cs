using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace apz_Image_Editor
{
    public sealed partial class Enmarcar : UserControl
    {
        public WriteableBitmap _wB { get; set; }

        public Enmarcar(WriteableBitmap wB)
        {            
            _wB = wB;

            this.InitializeComponent();
        }

        private async void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog parent = (ContentDialog)this.Parent;
            Int32 frameSize = Convert.ToInt32(txtFrameSize.Text);
            Color frameColor = cpFrameColor.Color;

            Int32 w = _wB.PixelWidth;
            Int32 h = _wB.PixelHeight;

            // Calcular with height a partir de los datos text,
            Int32 widthWF = _wB.PixelWidth + (2 * frameSize);
            Int32 heightWF = _wB.PixelHeight + (2 * frameSize);

            // Obtenemos el hexadecimal little endian para alto y ancho (es una imagen cuadrada)
            String wHex = Convert.ToString(widthWF, 16);
            wHex = wHex.Length % 2 == 0 ? wHex : String.Concat("0", wHex);
            wHex = SDK.LittleEndian(wHex);

            String hHex = Convert.ToString(heightWF, 16);
            hHex = hHex.Length % 2 == 0 ? hHex : String.Concat("0", hHex);
            hHex = SDK.LittleEndian(hHex);

            // Enlazamos y obtenemos los bytes de la cabecera
            String sHex = String.Concat(SDK.sHex32_0, wHex, hHex, SDK.sHex32_1);
            Byte[] bytesH = SDK.HexStringToByteArray(sHex);

            // Estos serán los datos de la imagen a construir
            // El tamaño de estos datos será del tamaño height por with
            // con un padding ajustado a bytes y con un mínimo de 4 bytes
                
            Byte[] bytes = await SDK.WBToBytes(_wB);

            List<Byte> lsBytesD = new List<Byte>();

            Byte R = frameColor.R;
            Byte G = frameColor.G;
            Byte B = frameColor.B;
            Byte A = frameColor.A;

            if (!cbGradiente.IsChecked.HasValue || !cbGradiente.IsChecked.Value)
            {
                // Si sólo tenemos un color

                // Creamos la primera parte del marco
                for (Int32 fila = 0; fila < frameSize; fila++)
                {
                    for (Int32 columna = 0; columna < widthWF; columna++)
                    {
                        lsBytesD.Add(B);
                        lsBytesD.Add(G);
                        lsBytesD.Add(R);
                        lsBytesD.Add(A);
                    }
                }

                Int32 row = 0;
                Int32 col = 0;
                for (Int32 fila = 0; fila < frameSize; fila++)
                {
                    lsBytesD.Add(B);
                    lsBytesD.Add(G);
                    lsBytesD.Add(R);
                    lsBytesD.Add(A);
                }

                for (Int32 j = 54; j < bytes.Length; j += 4)
                {
                    var b = bytes[j];
                    var g = bytes[j + 1];
                    var r = bytes[j + 2];
                    var a = bytes[j + 3];

                    lsBytesD.Add(r);
                    lsBytesD.Add(g);
                    lsBytesD.Add(b);
                    lsBytesD.Add(a);

                    col++;

                    if (col != 0 && col % _wB.PixelWidth == 0)
                    {
                        row++;
                        col = 0;

                        for (Int32 columna = 0; columna < frameSize; columna++)
                        {
                            lsBytesD.Add(B);
                            lsBytesD.Add(G);
                            lsBytesD.Add(R);
                            lsBytesD.Add(A);
                        }

                        for (Int32 columna = 0; columna < frameSize; columna++)
                        {
                            lsBytesD.Add(B);
                            lsBytesD.Add(G);
                            lsBytesD.Add(R);
                            lsBytesD.Add(A);
                        }
                    }
                }

                // Pintamos la parte de arriba del marco
                for (Int32 fila = 0; fila < frameSize; fila++)
                {
                    for (Int32 columna = 0; columna < widthWF; columna++)
                    {
                        lsBytesD.Add(B);
                        lsBytesD.Add(G);
                        lsBytesD.Add(R);
                        lsBytesD.Add(A);
                    }
                }
            }
            else
            {
                // Si tenemos dos colores para GRADIENTE

                Color frameColorGradiente = cpFrameColorGradiente.Color;

                Byte R2 = frameColorGradiente.R;
                Byte G2 = frameColorGradiente.G;
                Byte B2 = frameColorGradiente.B;
                Byte A2 = frameColorGradiente.A;
                
                int stepA = ((A2 - A) / (frameSize - 1));
                int stepR = ((R2 - R) / (frameSize - 1));
                int stepG = ((G2 - G) / (frameSize - 1));
                int stepB = ((B2 - B) / (frameSize - 1));
                                
                // Creamos la primera parte del marco
                for (Int32 fila = 0; fila < frameSize; fila++)
                {
                    // Esquina inferior izquierda                    
                    for (Int32 columna = 0; columna < frameSize; columna++)
                    {
                        if (columna <= fila)
                        {
                            lsBytesD.Add(Convert.ToByte(B + (stepB * (columna))));
                            lsBytesD.Add(Convert.ToByte(G + (stepG * (columna))));
                            lsBytesD.Add(Convert.ToByte(R + (stepR * (columna))));
                            lsBytesD.Add(Convert.ToByte(A + (stepA * (columna))));
                        }
                        else
                        {
                            lsBytesD.Add(Convert.ToByte(B + (stepB * fila)));
                            lsBytesD.Add(Convert.ToByte(G + (stepG * fila)));
                            lsBytesD.Add(Convert.ToByte(R + (stepR * fila)));
                            lsBytesD.Add(Convert.ToByte(A + (stepA * fila)));
                        }                        
                    }

                    // CENTRO 
                    for (Int32 columna = 0; columna < w; columna++)
                    {
                        lsBytesD.Add(Convert.ToByte(B + (stepB * fila)));
                        lsBytesD.Add(Convert.ToByte(G + (stepG * fila)));
                        lsBytesD.Add(Convert.ToByte(R + (stepR * fila)));
                        lsBytesD.Add(Convert.ToByte(A + (stepA * fila)));
                    }

                    // Esquina inferior derecha
                    for (Int32 columna = frameSize - 1; columna >= 0; columna--)
                    {
                        if (columna <= fila)
                        {
                            lsBytesD.Add(Convert.ToByte(B + (stepB * (columna))));
                            lsBytesD.Add(Convert.ToByte(G + (stepG * (columna))));
                            lsBytesD.Add(Convert.ToByte(R + (stepR * (columna))));
                            lsBytesD.Add(Convert.ToByte(A + (stepA * (columna))));
                        }
                        else
                        {
                            lsBytesD.Add(Convert.ToByte(B + (stepB * fila)));
                            lsBytesD.Add(Convert.ToByte(G + (stepG * fila)));
                            lsBytesD.Add(Convert.ToByte(R + (stepR * fila)));
                            lsBytesD.Add(Convert.ToByte(A + (stepA * fila)));
                        }
                    }

                }

                Int32 row = 0;
                Int32 col = 0;
                for (Int32 fila = 0; fila < frameSize; fila++)
                {
                    lsBytesD.Add(Convert.ToByte(B + (stepB * fila)));
                    lsBytesD.Add(Convert.ToByte(G + (stepG * fila)));
                    lsBytesD.Add(Convert.ToByte(R + (stepR * fila)));
                    lsBytesD.Add(Convert.ToByte(A + (stepA * fila)));
                }

                for (Int32 j = 54; j < bytes.Length; j += 4)
                {
                    var b = bytes[j];
                    var g = bytes[j + 1];
                    var r = bytes[j + 2];
                    var a = bytes[j + 3];

                    lsBytesD.Add(r);
                    lsBytesD.Add(g);
                    lsBytesD.Add(b);
                    lsBytesD.Add(a);

                    col++;

                    if (col != 0 && col % _wB.PixelWidth == 0)
                    {
                        row++;
                        col = 0;

                        for (Int32 columna = frameSize - 1; columna >= 0; columna--)
                        {
                            lsBytesD.Add(Convert.ToByte(B + (stepB * columna)));
                            lsBytesD.Add(Convert.ToByte(G + (stepG * columna)));
                            lsBytesD.Add(Convert.ToByte(R + (stepR * columna)));
                            lsBytesD.Add(Convert.ToByte(A + (stepA * columna)));
                        }
                        if (row < h)
                        {
                            for (Int32 columna = 0; columna < frameSize; columna++)
                            {
                                lsBytesD.Add(Convert.ToByte(B + (stepB * columna)));
                                lsBytesD.Add(Convert.ToByte(G + (stepG * columna)));
                                lsBytesD.Add(Convert.ToByte(R + (stepR * columna)));
                                lsBytesD.Add(Convert.ToByte(A + (stepA * columna)));
                            }
                        }                
                    }
                }

                // Pintamos la parte de arriba del marco
                for (Int32 fila = frameSize - 1; fila >= 0; fila--)
                {
                    // Esquina superior izquierda
                    for (Int32 columna = 0; columna < frameSize; columna++)
                    {
                        if (columna > fila)
                        {
                            lsBytesD.Add(Convert.ToByte(B + (stepB * (fila))));
                            lsBytesD.Add(Convert.ToByte(G + (stepG * (fila))));
                            lsBytesD.Add(Convert.ToByte(R + (stepR * (fila))));
                            lsBytesD.Add(Convert.ToByte(A + (stepA * (fila))));
                        }
                        else
                        {
                            lsBytesD.Add(Convert.ToByte(B + (stepB * columna)));
                            lsBytesD.Add(Convert.ToByte(G + (stepG * columna)));
                            lsBytesD.Add(Convert.ToByte(R + (stepR * columna)));
                            lsBytesD.Add(Convert.ToByte(A + (stepA * columna)));
                        }
                    }                    

                    // CENTRO 
                    for (Int32 columna = 0; columna < w; columna++)
                    {
                        lsBytesD.Add(Convert.ToByte(B + (stepB * fila)));
                        lsBytesD.Add(Convert.ToByte(G + (stepG * fila)));
                        lsBytesD.Add(Convert.ToByte(R + (stepR * fila)));
                        lsBytesD.Add(Convert.ToByte(A + (stepA * fila)));
                    }

                    // Esquina superior derecha
                    for (Int32 columna = frameSize - 1; columna >= 0; columna--)
                    {
                        if (columna > fila)
                        {
                            lsBytesD.Add(Convert.ToByte(B + (stepB * (fila))));
                            lsBytesD.Add(Convert.ToByte(G + (stepG * (fila))));
                            lsBytesD.Add(Convert.ToByte(R + (stepR * (fila))));
                            lsBytesD.Add(Convert.ToByte(A + (stepA * (fila))));
                        }
                        else
                        {
                            lsBytesD.Add(Convert.ToByte(B + (stepB * columna)));
                            lsBytesD.Add(Convert.ToByte(G + (stepG * columna)));
                            lsBytesD.Add(Convert.ToByte(R + (stepR * columna)));
                            lsBytesD.Add(Convert.ToByte(A + (stepA * columna)));
                        }
                    }

                }
            }

            Byte[] bytesD = lsBytesD.ToArray();

            Byte[] resultBytes = new Byte[bytesH.Length + bytesD.Length];
            Array.Copy(bytesH, resultBytes, bytesH.Length);
            Array.Copy(bytesD, 0, resultBytes, bytesH.Length, bytesD.Length);

            MemoryStream mS = new MemoryStream(resultBytes);

            WriteableBitmap wb = new WriteableBitmap(widthWF, heightWF);
            await wb.SetSourceAsync(mS.AsRandomAccessStream());
            parent.Tag = wb;

            parent.Hide();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog parent = (ContentDialog)this.Parent;
            parent.Hide();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Int32 cutColumns = Convert.ToInt32(((TextBox)sender).Text);

                if (cutColumns < 2)
                {
                    ((TextBox)sender).Text = "2";
                }
            }
            catch (Exception ex)
            {
                ((TextBox)sender).Text = "2";
            }
        }

        
    }
}
