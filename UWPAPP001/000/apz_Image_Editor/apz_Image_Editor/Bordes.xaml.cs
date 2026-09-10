using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
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
    public sealed partial class Bordes : UserControl
    {
        public WriteableBitmap _wB { get; set; }

        public Bordes(WriteableBitmap wB)
        {
            _wB = wB;

            this.InitializeComponent();
        }

        private async void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog parent = (ContentDialog)this.Parent;

            Int32 diferenciaC = Convert.ToInt32(txtSizePixel.Text);
            Color borderColor = cpBorderColor.Color;
            Color fondoColor = cpFondoColor.Color;

            Byte[] bytes = await SDK.GetBNImage(_wB);

            Int32 w = _wB.PixelWidth;
            Int32 h = _wB.PixelHeight;

            Int32 row = 0;
            Int32 col = 0;

            // Estando la imagen en blanco y negro r, g y b son iguales
            // Obtnemos la componente green del color (ó red ó blue en su defecto)
            List<Int32[]> colorsMatrix = new List<Int32[]>();
            Int32[] colorRow = new Int32[w];
            for (Int32 j = 54; j < bytes.Length; j += 4)
            {
                //var b = bytes[j];
                var g = Convert.ToInt32(bytes[j + 1]);
                //var r = bytes[j + 2];
                //var a = bytes[j + 3];

                Int32 c = g;
                colorRow[col] = c;

                col++;

                if (col != 0 && col % w == 0)
                {
                    Int32[] tempCRow = new Int32[w];
                    colorRow.CopyTo(tempCRow, 0);
                    colorsMatrix.Add(tempCRow);
                    colorRow = new Int32[w];
                    row++;
                    col = 0;
                }
            }

            Boolean[,] bordesMatrix = SDK.GetBorderMatrix(h, w, colorsMatrix, diferenciaC);

            if (bordesMatrix != null)
            {
                row = 0;
                col = 0;
                // Componemos el nuevo archivo
                for (Int32 j = 54; j < bytes.Length; j += 4)
                {
                    if (bordesMatrix[row, col])
                    {
                        bytes[j] = borderColor.B;
                        bytes[j + 1] = borderColor.G;
                        bytes[j + 2] = borderColor.R;
                        //bytes[j + 3] = borderColor.A;
                    }
                    else
                    {
                        bytes[j] = fondoColor.B;
                        bytes[j + 1] = fondoColor.G;
                        bytes[j + 2] = fondoColor.R;
                        //bytes[j + 3] = fondoColor.A;
                    }

                    col++;

                    if (col != 0 && col % w == 0)
                    {
                        row++;
                        col = 0;
                    }
                }

                // Convertimos los nuevos bytes a un nuevo WriteableBitmap
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    stream.AsStreamForWrite().Write(bytes, 0, bytes.Length);
                    stream.Seek(0);
                    WriteableBitmap wb = new WriteableBitmap(_wB.PixelWidth, _wB.PixelHeight);
                    wb.SetSource(stream);

                    parent.Tag = wb;
                    parent.Hide();
                }
            }
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
                Int32 diffPixel = Convert.ToInt32(((TextBox)sender).Text);

                if (diffPixel < 1)
                {
                    ((TextBox)sender).Text = "1";
                }
                else if (diffPixel > 255)
                {
                    ((TextBox)sender).Text = "255";
                }
            }
            catch (Exception ex)
            {
                ((TextBox)sender).Text = "10";
            }
        }

    }
}
