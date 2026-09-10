using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace apz_Image_Editor
{
    public sealed partial class Contornos : UserControl
    {
        public WriteableBitmap _wB { get; set; }        
        Boolean codeFinded = false;

        public Contornos(WriteableBitmap wB)
        {
            _wB = wB;
            this.InitializeComponent();
        }


        private async void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            codeFinded = false;

            Int32 w = _wB.PixelWidth;
            Int32 h = _wB.PixelHeight;
            
            pB.Minimum = 0;
            pB.Maximum = h;
            tbTolerancia.Visibility = Visibility.Collapsed;
            txtTolerancia.Visibility = Visibility.Collapsed;
            spButtons.Visibility = Visibility.Collapsed;
            cpColor.Visibility = Visibility.Collapsed;
            tbColor.Visibility = Visibility.Collapsed;
            spPB.Visibility = Visibility.Visible;
            ContentDialog parent = (ContentDialog)this.Parent;

            Int32 diferenciaC = Convert.ToInt32(txtTolerancia.Text);
            Windows.UI.Color borderColor = cpColor.Color;
            Byte[] originalB = await SDK.WBToBytes(_wB);
            Byte[] bytes = await SDK.GetBNImage(_wB);

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

            List<String> datos01 = new List<String>();
            for (row = 0; row < h; row++)
            {
                String s = String.Empty;
                for (col = 0; col < w; col++)
                {
                    s += bordesMatrix[row, col] ? "1" : "0";
                }
                datos01.Add(s);
            }

            if (bordesMatrix != null)
            {
                row = 0;
                col = 0;
                
                //Boolean salir = false;
                Boolean[,] coordenadasMatrix = new Boolean[h, w];
                List<Polygon> polygons = new List<Polygon>();
                Canvas c = new Canvas();
                c.Width = w;
                c.Height = h;
                GeneralTransform gT = c.TransformToVisual(c);
                //while (!codeFinded && row < h)
                //{
                // Buscamos un borde que podrá ser o no ser del cuadrado
                for (; row < h; row++)
                {
                    pB.Value = row;
                    
                    for (; col < w; col++)
                    {
                        if (bordesMatrix[row, col])
                        {
                            //if (!SDK.IsPointInPolygon(new Int32[] { row, col }, polygons, h, w))                                                                
                            Point canvasPoint = gT.TransformPoint(new Point(col, row));
                            IEnumerable<UIElement> elementos = VisualTreeHelper.FindElementsInHostCoordinates(canvasPoint, c, false);

                            if (polygons.Count > 1000)
                            {
                                codeFinded = true;
                                break;
                            }
                            if (col % 10 == 0)
                            {
                                await Task.Delay(1);
                            }
                            if (polygons.Count == 0 || elementos.Count() == 0)
                            {
                                Polygon coordenadasContorno = new Polygon();
                                coordenadasContorno.Fill = new SolidColorBrush(Colors.White);
                                coordenadasContorno.Stroke = new SolidColorBrush(Colors.White);
                                coordenadasContorno.StrokeThickness = 1;
                                coordenadasContorno.Points.Add(new Windows.Foundation.Point(col, row));
                                coordenadasMatrix[row, col] = true;
                                Int32 rowEdge = row;
                                Int32 colEdge = col;
                                Boolean posibleCode = true;
                                Boolean edgeFinded = false;
                                Int32 direccion = 2;
                                while (posibleCode && !edgeFinded)
                                {                                    
                                    if (codeFinded)
                                    {
                                        break;
                                    }

                                    // Tendremos primeramente en cuenta unas coordenadas según la dirección a la que vayamos
                                    switch (direccion)
                                    {
                                        // Si vamos hacia izquierda 
                                        case 0:
                                            // abajo
                                            if ((rowEdge + 1) < h && bordesMatrix[rowEdge + 1, colEdge])
                                            {
                                                direccion = 1;
                                                rowEdge++;
                                            }
                                            // a diagonal abajo izquierda
                                            else if ((rowEdge + 1) < h && colEdge > 0 && bordesMatrix[rowEdge + 1, colEdge - 1])
                                            {
                                                rowEdge++;
                                                colEdge--;
                                            }
                                            // Izquierda
                                            else if (colEdge > 0 && bordesMatrix[rowEdge, colEdge - 1])
                                            {
                                                direccion = 0;
                                                colEdge--;
                                            }
                                            // a diagonal arriba izquierda
                                            else if (rowEdge > 0 && colEdge > 0 && bordesMatrix[rowEdge - 1, colEdge - 1])
                                            {
                                                rowEdge--;
                                                colEdge--;
                                            }
                                            // arriba
                                            else if (rowEdge > 0 && bordesMatrix[rowEdge - 1, colEdge])
                                            {
                                                direccion = 3;
                                                rowEdge--;
                                            }
                                            else
                                            {
                                                posibleCode = false;
                                            }
                                            break;

                                        // Si vamos hacia abajo
                                        case 1:
                                            // derecha
                                            if ((colEdge + 1) < w && bordesMatrix[rowEdge, colEdge + 1])
                                            {
                                                direccion = 2;
                                                colEdge++;
                                            }
                                            // a diagonal abajo derecha
                                            else if ((rowEdge + 1) < h && (colEdge + 1) < w && bordesMatrix[rowEdge + 1, colEdge + 1])
                                            {
                                                rowEdge++;
                                                colEdge++;
                                            }
                                            // abajo
                                            else if ((rowEdge + 1) < h && bordesMatrix[rowEdge + 1, colEdge])
                                            {
                                                direccion = 1;
                                                rowEdge++;
                                            }
                                            // a diagonal abajo izquierda
                                            else if ((rowEdge + 1) < h && colEdge > 0 && bordesMatrix[rowEdge + 1, colEdge - 1])
                                            {
                                                rowEdge++;
                                                colEdge--;
                                            }
                                            // Izquierda
                                            else if (colEdge > 0 && bordesMatrix[rowEdge, colEdge - 1])
                                            {
                                                direccion = 0;
                                                colEdge--;
                                            }
                                            else
                                            {
                                                posibleCode = false;
                                            }
                                            break;

                                        // Si vamos hacia la derecha
                                        case 2:
                                            // arriba
                                            if (rowEdge > 0 && bordesMatrix[rowEdge - 1, colEdge])
                                            {
                                                direccion = 3;
                                                rowEdge--;
                                            }
                                            // a diagonal arriba derecha
                                            else if (rowEdge > 0 && (colEdge + 1) < w && bordesMatrix[rowEdge - 1, colEdge + 1])
                                            {
                                                rowEdge--;
                                                colEdge++;
                                            }
                                            // derecha
                                            else if ((colEdge + 1) < w && bordesMatrix[rowEdge, colEdge + 1])
                                            {
                                                direccion = 2;
                                                colEdge++;
                                            }
                                            // a diagonal abajo derecha
                                            else if ((rowEdge + 1) < h && (colEdge + 1) < w && bordesMatrix[rowEdge + 1, colEdge + 1])
                                            {
                                                rowEdge++;
                                                colEdge++;
                                            }
                                            // abajo
                                            else if ((rowEdge + 1) < h && bordesMatrix[rowEdge + 1, colEdge])
                                            {
                                                direccion = 1;
                                                rowEdge++;
                                            }
                                            else
                                            {
                                                posibleCode = false;
                                            }
                                            break;

                                        // Vamos hacia arriba
                                        case 3:
                                            // Izquierda
                                            if (colEdge > 0 && bordesMatrix[rowEdge, colEdge - 1])
                                            {
                                                direccion = 0;
                                                colEdge--;
                                            }
                                            // a diagonal arriba izquierda
                                            else if (rowEdge > 0 && colEdge > 0 && bordesMatrix[rowEdge - 1, colEdge - 1])
                                            {
                                                rowEdge--;
                                                colEdge--;
                                            }
                                            // arriba
                                            else if (rowEdge > 0 && bordesMatrix[rowEdge - 1, colEdge])
                                            {
                                                direccion = 3;
                                                rowEdge--;
                                            }
                                            // a diagonal arriba derecha
                                            else if (rowEdge > 0 && (colEdge + 1) < w && bordesMatrix[rowEdge - 1, colEdge + 1])
                                            {
                                                rowEdge--;
                                                colEdge++;
                                            }
                                            // derecha
                                            else if ((colEdge + 1) < w && bordesMatrix[rowEdge, colEdge + 1])
                                            {
                                                direccion = 2;
                                                colEdge++;
                                            }
                                            else
                                            {
                                                posibleCode = false;
                                            }

                                            break;
                                    }

                                    Boolean contieneCoordenada = false;
                                    for (int i = 0; i < coordenadasContorno.Points.Count; i++)
                                    {
                                        if (coordenadasContorno.Points[i].X == colEdge && coordenadasContorno.Points[i].Y == rowEdge)// SequenceEqual(new Int32[] { rowEdge, colEdge }))
                                        {
                                            contieneCoordenada = true;
                                        }
                                    }

                                    if (contieneCoordenada)
                                    {
                                        edgeFinded = true;
                                        if (coordenadasContorno.Points.Count > 50)
                                        {
                                            polygons.Add(coordenadasContorno);
                                            c.Children.Add(coordenadasContorno);
                                        }
                                    }
                                    else
                                    {
                                        coordenadasContorno.Points.Add(new Windows.Foundation.Point(colEdge, rowEdge));

                                    }
                                }

                            }
                        }
                    }

                    if (codeFinded)
                    {
                        break;
                    }
                    col = 0;

                }

                if (!codeFinded)
                {

                    // Componemos las coordenadasMatrix con los poligonos que tenemos como finalizados
                    foreach (Polygon polygon in polygons)
                    {
                        foreach (Windows.Foundation.Point pPoly in polygon.Points)
                        {
                            Int32 y = Convert.ToInt32(pPoly.Y);
                            Int32 x = Convert.ToInt32(pPoly.X);

                            coordenadasMatrix[y, x] = true;
                            // arriba
                            if (y > 0)
                            {
                                coordenadasMatrix[y - 1, x] = true;
                            }
                            // abajo
                            if ((y + 1) < h)
                            {
                                coordenadasMatrix[y + 1, x] = true;
                            }
                            // izquierda
                            if (x > 0)
                            {
                                coordenadasMatrix[y, x - 1] = true;
                            }
                            // derecha
                            if ((x + 1) < w)
                            {
                                coordenadasMatrix[y, x + 1] = true;
                            }
                            // diagonal arriba izquierda
                            if (y > 0 && x > 0)
                            {
                                coordenadasMatrix[y - 1, x - 1] = true;
                            }
                            // diagonal abajo derecha
                            if ((y + 1) < h && (x + 1) < w)
                            {
                                coordenadasMatrix[y + 1, x + 1] = true;
                            }
                            // diagonal arriba derecha
                            if (y > 0 && (x + 1) < w)
                            {
                                coordenadasMatrix[y - 1, x + 1] = true;
                            }
                            // diagonal abajo izquierda
                            if ((y + 1) < h && x > 0)
                            {
                                coordenadasMatrix[y + 1, x - 1] = true;
                            }
                        }
                    }

                    row = 0;
                    col = 0;
                    // Componemos el nuevo archivo
                    for (Int32 j = 54; j < originalB.Length; j += 4)
                    {
                        if (coordenadasMatrix[row, col])
                        {
                            originalB[j] = borderColor.B;
                            originalB[j + 1] = borderColor.G;
                            originalB[j + 2] = borderColor.R;
                            originalB[j + 3] = borderColor.A;
                        }
                        else
                        {
                            Byte tempB = originalB[j];
                            originalB[j] = originalB[j + 2];
                            //originalB[j + 1] = Convert.ToByte(0);
                            originalB[j + 2] = tempB;
                        }

                        col++;

                        if (col != 0 && col % w == 0)
                        {
                            row++;
                            col = 0;
                        }
                    }

                    //// Convertimos los nuevos bytes a un nuevo WriteableBitmap
                    //using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                    //{
                    //    stream.AsStreamForWrite().Write(bytes, 0, bytes.Length);
                    //    stream.Seek(0);
                    //    WriteableBitmap wb = new WriteableBitmap(_wB.PixelWidth, _wB.PixelHeight);
                    //    wb.SetSource(stream);

                    //    _wB = wb;
                    //    imgMain.Source = _wB;
                    //}

                    MemoryStream mS = new MemoryStream(originalB);
                    WriteableBitmap wb = new WriteableBitmap(w, h);
                    await wb.SetSourceAsync(mS.AsRandomAccessStream());
                    parent.Tag = wb;
                }
                else
                {
                    var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                    
                    // Create the message dialog and set its content
                    var messageDialog = new MessageDialog(resourceLoader.GetString("errorContornos"));
                    messageDialog.Commands.Add(new UICommand(resourceLoader.GetString("close")));
                    messageDialog.CancelCommandIndex = 0;
                    // Show the message dialog
                    await messageDialog.ShowAsync();
                }
            }
            tbTolerancia.Visibility = Visibility.Visible;
            txtTolerancia.Visibility = Visibility.Visible;
            spButtons.Visibility = Visibility.Visible;
            cpColor.Visibility = Visibility.Visible;
            tbColor.Visibility = Visibility.Visible;
            spPB.Visibility = Visibility.Collapsed;
            parent.Hide();
            
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog parent = (ContentDialog)this.Parent;
            parent.Hide();
        }

        private void btnCancelarP_Click(object sender, RoutedEventArgs e)
        {
            codeFinded = true;
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Int32 cutColumns = Convert.ToInt32(((TextBox)sender).Text);

                if (cutColumns < 1)
                {
                    ((TextBox)sender).Text = "1";
                }
                else if(cutColumns > 255)
                {
                    ((TextBox)sender).Text = "255";
                }
            }
            catch (Exception ex)
            {
                ((TextBox)sender).Text = "15";
            }
        }
    }
}
