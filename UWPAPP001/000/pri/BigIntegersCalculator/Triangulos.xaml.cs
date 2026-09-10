using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace BigIntegersCalculator
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Triangulos : Page
    {
        public double Multiply { get; set; }

        public List<Polyline> LineasTriangulo { get; set; }
        public List<TextBlock> PuntosTriangulo { get; set; }

        public List<Color> Colores { get; set; }
        public Int32 iColores { get; set; }

        public Triangulos()
        {
            this.Multiply = 40;
            this.LineasTriangulo = new List<Polyline>();
            this.PuntosTriangulo = new List<TextBlock>();
            this.Colores = new List<Color>() { Colors.Red, Colors.Green, Colors.Blue, Colors.Orange, Colors.Violet };
            iColores = 0;
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //if (this.DataContext is String)
            //{
            //    DataContextInit = (String)this.DataContext;
            //    String[] data = DataContextInit.Split(new String[1] { ";" }, StringSplitOptions.None);
            //    Formula.Text = data[0];
            //    this.DataContext = data[1];
            //}
            //this.DataContext = "495,525 496,516 497,509 498,504 499,501 500,500 501,501 502,504 503,509 504,516 505,525";
            //this.DataContext = this;

            //Polyline linea = (Polyline)canvasTriangulo.Children[0];
            //PointCollection newPC = new PointCollection();
            //for (Int32 i = 0; i < linea.Points.Count; i++)
            //{
            //    newPC.Add(new Point(linea.Points[i].X * this.Multiply, linea.Points[i].Y * this.Multiply));
            //}
            //linea.Points = newPC;
            Page_SizeChanged(null, null);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double heightGrid = gridP.ActualHeight;
            double widthGrid = gridP.ActualWidth;

            heightGrid = heightGrid / 2;
            widthGrid = widthGrid / 2;

            //UIElement linea = canvasTriangulo.Children[0];
            //Canvas.SetTop(linea, heightGrid);
            //Canvas.SetLeft(linea, widthGrid);
            canvasTriangulo.Children.Clear();

            Polyline x = new Polyline();
            x.Stroke = new SolidColorBrush(Colors.Black);
            x.StrokeThickness = 0.2;
            Canvas.SetTop(x, heightGrid);
            Canvas.SetLeft(x, widthGrid);
            x.Points.Add(new Point(-1 * widthGrid, 0));
            x.Points.Add(new Point(widthGrid, 0));

            Polyline y = new Polyline();
            y.Stroke = new SolidColorBrush(Colors.Black);
            y.StrokeThickness = 0.2;
            Canvas.SetTop(y, heightGrid);
            Canvas.SetLeft(y, widthGrid);
            y.Points.Add(new Point(0, -1 * heightGrid));
            y.Points.Add(new Point(0, heightGrid));

            //canvasTriangulo.Children.Add(linea);
            canvasTriangulo.Children.Add(x);
            canvasTriangulo.Children.Add(y);

            showGrid(heightGrid, widthGrid);
        }

        private void showGrid(double heightGrid, double widthGrid)
        {
            for (double i = 1 * Multiply; i < widthGrid;)
            {
                Polyline yR = new Polyline();
                yR.Stroke = new SolidColorBrush(Colors.Black);
                yR.StrokeThickness = 0.2;
                Canvas.SetTop(yR, heightGrid);
                Canvas.SetLeft(yR, widthGrid);
                yR.Points.Add(new Point(0 + i, -1 * heightGrid));
                yR.Points.Add(new Point(0 + i, heightGrid));
                canvasTriangulo.Children.Add(yR);
                Polyline yL = new Polyline();
                yL.Stroke = new SolidColorBrush(Colors.Black);
                yL.StrokeThickness = 0.2;
                Canvas.SetTop(yL, heightGrid);
                Canvas.SetLeft(yL, widthGrid);
                yL.Points.Add(new Point(0 - i, -1 * heightGrid));
                yL.Points.Add(new Point(0 - i, heightGrid));
                canvasTriangulo.Children.Add(yL);
                i += Multiply;
            }

            for (double i = 1 * Multiply; i < heightGrid;)
            {
                Polyline xT = new Polyline();
                xT.Stroke = new SolidColorBrush(Colors.Black);
                xT.StrokeThickness = 0.2;
                Canvas.SetTop(xT, heightGrid);
                Canvas.SetLeft(xT, widthGrid);
                xT.Points.Add(new Point(-1 * widthGrid, 0 + i));
                xT.Points.Add(new Point(widthGrid, 0 + i));
                canvasTriangulo.Children.Add(xT);

                Polyline xB = new Polyline();
                xB.Stroke = new SolidColorBrush(Colors.Black);
                xB.StrokeThickness = 0.2;
                Canvas.SetTop(xB, heightGrid);
                Canvas.SetLeft(xB, widthGrid);
                xB.Points.Add(new Point(-1 * widthGrid, 0 - i));
                xB.Points.Add(new Point(widthGrid, 0 - i));
                canvasTriangulo.Children.Add(xB);

                i += Multiply;
            }

            foreach (Polyline linea in LineasTriangulo)
            {
                Canvas.SetTop(linea, heightGrid);
                Canvas.SetLeft(linea, widthGrid);
                canvasTriangulo.Children.Add(linea);
            }

            foreach (TextBlock tb in PuntosTriangulo)
            {
                canvasTriangulo.Children.Add(tb);
            }

        }

        private void menosZoom_Click(object sender, RoutedEventArgs e)
        {
            double heightGrid = gridP.ActualHeight;
            double widthGrid = gridP.ActualWidth;

            heightGrid = heightGrid / 2;
            widthGrid = widthGrid / 2;

            foreach (Polyline linea in LineasTriangulo)
            { 
                PointCollection newPC = new PointCollection();
                for (Int32 i = 0; i < linea.Points.Count; i++)
                {
                    newPC.Add(new Point(linea.Points[i].X / 2, linea.Points[i].Y / 2));
                }
                linea.Points = newPC;
            }

            foreach (TextBlock tb in PuntosTriangulo)
            {
                double x = Canvas.GetLeft(tb);
                double y = Canvas.GetTop(tb);
                Canvas.SetLeft(tb, (x  / 2) + (widthGrid / 2));
                Canvas.SetTop(tb, (y / 2) + (heightGrid / 2));
            }

            this.Multiply /= 2;
            Page_SizeChanged(null, null);
        }

        private void masZoom_Click(object sender, RoutedEventArgs e)
        {
            double heightGrid = gridP.ActualHeight;
            double widthGrid = gridP.ActualWidth;

            heightGrid = heightGrid / 2;
            widthGrid = widthGrid / 2;

            foreach (Polyline linea in LineasTriangulo)
            {
                PointCollection newPC = new PointCollection();
                for (Int32 i = 0; i < linea.Points.Count; i++)
                {
                    newPC.Add(new Point(linea.Points[i].X * 2, linea.Points[i].Y * 2));
                }
                linea.Points = newPC;
            }

            foreach (TextBlock tb in PuntosTriangulo)
            {
                double x = Canvas.GetLeft(tb);
                double y = Canvas.GetTop(tb);
                Canvas.SetLeft(tb, (x * 2) - (widthGrid));
                Canvas.SetTop(tb, (y * 2) - (heightGrid));
            }

            this.Multiply *= 2;
            Page_SizeChanged(null, null);
        }

        
        /// <summary>Dibuja un triángulo con la información de dos de sus lados</summary>
        private async void DibujarLLA_Click(object sender, RoutedEventArgs e)
        {
            double heightGrid = gridP.ActualHeight;
            double widthGrid = gridP.ActualWidth;

            heightGrid = heightGrid / 2;
            widthGrid = widthGrid / 2;

            try
            {
                //this.LineasTriangulo.Clear();
                //Page_SizeChanged(null, null);

                double lado1 = Convert.ToDouble(txtLLAL1.Text);
                double lado2 = Convert.ToDouble(txtLLAL2.Text);
                double angulo = Convert.ToDouble(txtLLAA.Text);
                double mitadLado1 = lado1 / 2;
                double mitadLado2 = lado2 / 2;

                Polyline l1 = new Polyline();
                l1.Stroke = new SolidColorBrush(Colores[iColores % 5]);
                l1.StrokeThickness = 2;
                Canvas.SetTop(l1, heightGrid);
                Canvas.SetLeft(l1, widthGrid);
                l1.Points.Add(new Point(-1 * mitadLado1 * Multiply, 0));
                l1.Points.Add(new Point(mitadLado1 * Multiply, 0));
                LineasTriangulo.Add(l1);
                canvasTriangulo.Children.Add(l1);

                TextBlock tbA = new TextBlock() { Text = "A", Foreground = new SolidColorBrush(Colores[iColores % 5]) };
                Canvas.SetLeft(tbA, (widthGrid + (-1 * mitadLado1 * Multiply)) - 5);
                Canvas.SetTop(tbA, heightGrid + 5);
                PuntosTriangulo.Add(tbA);
                canvasTriangulo.Children.Add(tbA);

                TextBlock tbC = new TextBlock() { Text = "C", Foreground = new SolidColorBrush(Colores[iColores % 5]) };
                Canvas.SetLeft(tbC, (widthGrid + (mitadLado1 * Multiply)) + 5);
                Canvas.SetTop(tbC, heightGrid + 5);
                PuntosTriangulo.Add(tbC);
                canvasTriangulo.Children.Add(tbC);

                // *** Calculamos la coordenada derecha de Lado2 en función del punto izquierdo de conexión con lado1 y el ángulo
                // Obtenemos el coseno y seno del ángulo en cuestión y lo multiplicamos por la distacia del lado2 y se lo sumamos x e y del punto de Lado1
                double xL2 = Math.Cos(angulo * Math.PI / 180) * lado2 - mitadLado1 ;
                double yL2 = Math.Sin(angulo * Math.PI / 180) * lado2 + 0;                                                

                Polyline l2 = new Polyline();
                l2.Stroke = new SolidColorBrush(Colores[iColores % 5]);
                l2.StrokeThickness = 2;
                Canvas.SetTop(l2, heightGrid);
                Canvas.SetLeft(l2, widthGrid);
                l2.Points.Add(new Point(-1 * mitadLado1* Multiply, 0));
                l2.Points.Add(new Point(xL2 * Multiply, -1 * (yL2 * Multiply)));
                LineasTriangulo.Add(l2);
                canvasTriangulo.Children.Add(l2);

                TextBlock tbB = new TextBlock() { Text = "B", Foreground = new SolidColorBrush(Colores[iColores % 5]) };
                Canvas.SetLeft(tbB, (widthGrid + (xL2 * Multiply)));
                Canvas.SetTop(tbB, heightGrid + (-1 * (yL2 * Multiply)) - 20);
                PuntosTriangulo.Add(tbB);
                canvasTriangulo.Children.Add(tbB);

                Polyline l3 = new Polyline();
                l3.Stroke = new SolidColorBrush(Colores[iColores % 5]);
                l3.StrokeThickness = 2;
                Canvas.SetTop(l3, heightGrid);
                Canvas.SetLeft(l3, widthGrid);                
                l3.Points.Add(new Point(xL2 * Multiply, -1 * (yL2 * Multiply)));
                l3.Points.Add(new Point(mitadLado1 * Multiply, 0));
                LineasTriangulo.Add(l3);
                canvasTriangulo.Children.Add(l3);

                double longitud3 = Math.Sqrt(Math.Pow(Math.Abs((mitadLado1 * Multiply) - (xL2 * Multiply)), 2) + Math.Pow(Math.Abs(yL2 * Multiply), 2)) / Multiply;
                double h = Math.Abs(yL2);

                // Calculamos el área con la fórmula de Herón, sabiendo la longitud de cada uno de los 3 lados
                // A = ¼ * √ (((AB + BC + CA) * (BC + CA-AB) * (AB + CA-BC) ) * (AB + BC-CA))
                DatosTriangulo.Text = "BC = " + longitud3 + 
                    "u - Área / Area = "  + (Math.Sqrt(((lado1 + lado2 + longitud3) * (lado2 + longitud3 - lado1) * (lado1 + longitud3 - lado2)) * (lado1 + lado2 - longitud3))/4) +
                    "u^2 - Altura / Height = " + h + "u";

                iColores++;

            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "ERROR").ShowAsync();
                
                //MessageDialog dialog = new MessageDialog("Yes or no?");
                //dialog.Commands.Add(new UICommand("Yes", null));
                //dialog.Commands.Add(new UICommand("No", null));
                //dialog.DefaultCommandIndex = 0;
                //dialog.CancelCommandIndex = 1;
                //var cmd = await dialog.ShowAsync();

                //if (cmd.Label == "Yes")
                //{
                //    // do something
                //}
            }
        }

        private async void Dibujar_LAA_Click(object sender, RoutedEventArgs e)
        {
            double heightGrid = gridP.ActualHeight;
            double widthGrid = gridP.ActualWidth;

            heightGrid = heightGrid / 2;
            widthGrid = widthGrid / 2;

            try
            {
                //this.LineasTriangulo.Clear();                
                //Page_SizeChanged(null, null);

                double lado = Convert.ToDouble(txtLAAL.Text);
                double angulo1 = Convert.ToDouble(txtLAAA1.Text);
                double angulo2 = Convert.ToDouble(txtLAAA2.Text);
                double mitadLado = lado / 2;
                double angulo3 = 180 - angulo1 - angulo2;

                //Convertimos a radianes los ángulos
                angulo1 = angulo1 * Math.PI / 180;
                angulo2 = angulo2 * Math.PI / 180;
                angulo3 = angulo3 * Math.PI / 180;

                Polyline l1 = new Polyline();
                l1.Stroke = new SolidColorBrush(Colores[iColores % 5]);
                l1.StrokeThickness = 2;
                Canvas.SetTop(l1, heightGrid);
                Canvas.SetLeft(l1, widthGrid);
                l1.Points.Add(new Point(-1 * mitadLado * Multiply, 0));
                l1.Points.Add(new Point(mitadLado * Multiply, 0));
                LineasTriangulo.Add(l1);
                canvasTriangulo.Children.Add(l1);

                TextBlock tbA = new TextBlock() { Text = "A", Foreground = new SolidColorBrush(Colores[iColores % 5]) };
                Canvas.SetLeft(tbA, (widthGrid + (-1 * mitadLado * Multiply)) - 5);
                Canvas.SetTop(tbA, heightGrid + 5);
                PuntosTriangulo.Add(tbA);
                canvasTriangulo.Children.Add(tbA);

                TextBlock tbC = new TextBlock() { Text = "C", Foreground = new SolidColorBrush(Colores[iColores % 5]) };
                Canvas.SetLeft(tbC, (widthGrid + (mitadLado * Multiply)) + 5);
                Canvas.SetTop(tbC, heightGrid + 5);
                PuntosTriangulo.Add(tbC);
                canvasTriangulo.Children.Add(tbC);


                // Calculamos la longitud del lado 2 según la formula a/Sen(A) = b/Sen(B) donde a = lado B = angulo1 y A = angulo3
                double lado2 = (lado / Math.Sin(angulo3)) * Math.Sin(angulo2);

                // *** Calculamos la coordenada derecha de Lado2 en función del punto izquierdo de conexión con lado1 y el ángulo
                // Obtenemos el coseno y seno del ángulo en cuestión y lo multiplicamos por la distacia del lado2 y se lo sumamos x e y del punto de Lado1
                double xL2 = Math.Cos(angulo1) * lado2 - mitadLado;
                double yL2 = Math.Sin(angulo1) * lado2 + 0;

                Polyline l2 = new Polyline();
                l2.Stroke = new SolidColorBrush(Colores[iColores % 5]);
                l2.StrokeThickness = 2;
                Canvas.SetTop(l2, heightGrid);
                Canvas.SetLeft(l2, widthGrid);
                l2.Points.Add(new Point(-1 * mitadLado * Multiply, 0));
                l2.Points.Add(new Point(xL2 * Multiply, -1 * (yL2 * Multiply)));
                LineasTriangulo.Add(l2);
                canvasTriangulo.Children.Add(l2);

                TextBlock tbB = new TextBlock() { Text = "B", Foreground = new SolidColorBrush(Colores[iColores % 5]) };
                Canvas.SetLeft(tbB, (widthGrid + (xL2 * Multiply)));
                Canvas.SetTop(tbB, heightGrid + (-1 * (yL2 * Multiply)) - 20);
                PuntosTriangulo.Add(tbB);
                canvasTriangulo.Children.Add(tbB);

                Polyline l3 = new Polyline();
                l3.Stroke = new SolidColorBrush(Colores[iColores % 5]);
                l3.StrokeThickness = 2;
                Canvas.SetTop(l3, heightGrid);
                Canvas.SetLeft(l3, widthGrid);
                l3.Points.Add(new Point(xL2 * Multiply, -1 * (yL2 * Multiply)));
                l3.Points.Add(new Point(mitadLado * Multiply, 0));
                LineasTriangulo.Add(l3);
                canvasTriangulo.Children.Add(l3);

                double longitud3 = Math.Sqrt(Math.Pow(Math.Abs((mitadLado * Multiply) - (xL2 * Multiply)), 2) + Math.Pow(Math.Abs(yL2 * Multiply), 2)) / Multiply;
                double h = Math.Abs(yL2);

                // Calculamos el área con la fórmula de Herón, sabiendo la longitud de cada uno de los 3 lados
                // A = ¼ * √ (((AB + BC + CA) * (BC + CA-AB) * (AB + CA-BC) ) * (AB + BC-CA))
                DatosTriangulo.Text = "AB = " + lado2 +  "u - BC = " + longitud3 + 
                    "u - Área / Area = " + (Math.Sqrt(((lado + lado2 + longitud3) * (lado2 + longitud3 - lado) * (lado + longitud3 - lado2)) * (lado + lado2 - longitud3)) / 4) +
                    "u^2 - Altura / Height = " + h + "u";

                iColores++;

            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "ERROR").ShowAsync();
            }
        }
        private void borrarLienzo_Click(object sender, RoutedEventArgs e)
        {
            iColores = 0;
            DatosTriangulo.Text = String.Empty;
            this.LineasTriangulo.Clear();
            this.PuntosTriangulo.Clear();
            Page_SizeChanged(null, null);
        }

        private void txt_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            Boolean cancel = false;

            for (Int32 i = 0; i < args.NewText.Length; i++)
            {
                if (args.NewText[i] == '-' && i != 0)
                {
                    cancel = true;
                    break;
                }
                else if (!Char.IsDigit(args.NewText[i]) && args.NewText[i] != '-' && args.NewText[i] != ',')
                {
                    cancel = true;
                    break;
                }
            }

            args.Cancel = cancel;
        }

        private void txt_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            String text = String.Empty;

            for (Int32 i = 0; i < sender.Text.Length; i++)
            {
                if (i == 0 && sender.Text[i] == '-')
                {
                    text += sender.Text[i];
                }
                else if (Char.IsDigit(sender.Text[i]) || sender.Text[i] == ',')
                {
                    text += sender.Text[i];
                }
            }
            sender.Text = text;
        }
    }
}
