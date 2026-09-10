using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace BigIntegersCalculator
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class Grafica : Page
    {
        public String DataContextInit { get; set; }

        public double Multiply { get; set; }

        public Grafica()
        {
            this.Multiply = 5;
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {            
            if (this.DataContext is String)
            {
                DataContextInit = (String)this.DataContext;
                String[] data = DataContextInit.Split(new String[1] { ";" }, StringSplitOptions.None);
                Formula.Text = data[0];
                this.DataContext = data[1];
            }
            //this.DataContext = "495,525 496,516 497,509 498,504 499,501 500,500 501,501 502,504 503,509 504,516 505,525";
            //this.DataContext = this;

            Polyline linea = (Polyline)canvasGrafica.Children[0];
            PointCollection newPC = new PointCollection();
            for (Int32 i = 0; i < linea.Points.Count; i++)
            {
                newPC.Add(new Point(linea.Points[i].X * this.Multiply, linea.Points[i].Y * this.Multiply));
            }
            linea.Points = newPC;
            Page_SizeChanged(null, null);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double heightGrid = gridP.ActualHeight;
            double widthGrid = gridP.ActualWidth;

            heightGrid = heightGrid / 2;
            widthGrid = widthGrid / 2;

            UIElement linea = canvasGrafica.Children[0];
            Canvas.SetTop(linea, heightGrid);
            Canvas.SetLeft(linea, widthGrid);
            canvasGrafica.Children.Clear();

            Polyline x = new Polyline();
            x.Stroke = new SolidColorBrush(Colors.Black);
            x.StrokeThickness = 2;
            Canvas.SetTop(x, heightGrid);
            Canvas.SetLeft(x, widthGrid);
            x.Points.Add(new Point(-1 * widthGrid, 0));
            x.Points.Add(new Point(widthGrid, 0));            

            Polyline y = new Polyline();
            y.Stroke = new SolidColorBrush(Colors.Black);
            y.StrokeThickness = 2;
            Canvas.SetTop(y, heightGrid);
            Canvas.SetLeft(y, widthGrid);
            y.Points.Add(new Point(0, -1 * heightGrid));
            y.Points.Add(new Point(0, heightGrid));            

            canvasGrafica.Children.Add(linea);
            canvasGrafica.Children.Add(x);
            canvasGrafica.Children.Add(y);

            showGrid(heightGrid, widthGrid);
        }

        private void showGrid(double heightGrid, double widthGrid)
        {
            for(double i = 1 * Multiply; i < widthGrid;)
            {
                Polyline yR = new Polyline();
                yR.Stroke = new SolidColorBrush(Colors.Black);
                yR.StrokeThickness = 0.2;
                Canvas.SetTop(yR, heightGrid);
                Canvas.SetLeft(yR, widthGrid);
                yR.Points.Add(new Point(0 + i, -1 * heightGrid));
                yR.Points.Add(new Point(0 + i, heightGrid));
                canvasGrafica.Children.Add(yR);
                Polyline yL = new Polyline();
                yL.Stroke = new SolidColorBrush(Colors.Black);
                yL.StrokeThickness = 0.2;
                Canvas.SetTop(yL, heightGrid);
                Canvas.SetLeft(yL, widthGrid);
                yL.Points.Add(new Point(0 - i, -1 * heightGrid));
                yL.Points.Add(new Point(0 - i, heightGrid));
                canvasGrafica.Children.Add(yL);
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
                canvasGrafica.Children.Add(xT);

                Polyline xB = new Polyline();
                xB.Stroke = new SolidColorBrush(Colors.Black);
                xB.StrokeThickness = 0.2;
                Canvas.SetTop(xB, heightGrid);
                Canvas.SetLeft(xB, widthGrid);
                xB.Points.Add(new Point(-1 * widthGrid, 0 - i));
                xB.Points.Add(new Point(widthGrid, 0 - i));
                canvasGrafica.Children.Add(xB);

                i += Multiply;
            }
        }


        private void menosZoom_Click(object sender, RoutedEventArgs e)
        {
            Polyline linea = (Polyline)canvasGrafica.Children[0];
            PointCollection newPC = new PointCollection();
            for(Int32 i = 0; i < linea.Points.Count; i++)
            {
                newPC.Add(new Point(linea.Points[i].X / 2, linea.Points[i].Y / 2));
            }
            linea.Points = newPC;
            this.Multiply /= 2;

            Page_SizeChanged(null, null);
        }

        private void masZoom_Click(object sender, RoutedEventArgs e)
        {
            Polyline linea = (Polyline)canvasGrafica.Children[0];
            PointCollection newPC = new PointCollection();
            for (Int32 i = 0; i < linea.Points.Count; i++)
            {
                newPC.Add(new Point(linea.Points[i].X * 2, linea.Points[i].Y * 2));
            }
            linea.Points = newPC;
            this.Multiply *= 2;

            Page_SizeChanged(null, null);
        }

        async private void OnPrintButtonClick(object sender, RoutedEventArgs e)
        {
            if (Windows.Graphics.Printing.PrintManager.IsSupported())
            {
                try
                {
                    // Show print UI
                    await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();

                }
                catch
                {
                    // Printing cannot proceed at this time
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing error",
                        Content = "\nSorry, printing can' t proceed at this time.",
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                }
            }
            else
            {
                // Printing is not supported on this device
                ContentDialog noPrintingDialog = new ContentDialog()
                {
                    Title = "Printing not supported",
                    Content = "\nSorry, printing is not supported on this device.",
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }
    }
}
