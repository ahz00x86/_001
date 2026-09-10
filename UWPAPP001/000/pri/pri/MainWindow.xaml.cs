using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace pri
{
    /// <summary>Lógica de interacción para MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        /// <summary>Inicialización Main Windows</summary>
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Int32 n = Convert.ToInt32(txtN.Text);
            Int32 r = 500;
            Double x = xCanvas.ActualWidth / 2;
            Double y = xCanvas.ActualHeight / 2;

            Polygon poly = new Polygon();
            poly.Stroke = new SolidColorBrush(Colors.Black);
            poly.StrokeThickness = 1;

            for (Int32 i = 0; i < n; i++)
            {
                Point p = new Point(x + r * Math.Cos(2 * Math.PI * i / n), y + r * Math.Sin(2 * Math.PI * i / n));
                poly.Points.Add(p);
            }
            
            xCanvas.Children.Clear();
            xCanvas.Children.Add(poly);

            txtResult.Text = perimetroPoly(poly).ToString();

            PointCollection s = poly.Points;

            Int32 h = 2;
            for (BigInteger k = 0; k < BigInteger.Pow(s.Count(), h); k++)
            {
                BigInteger m = k;

                PointCollection pC = new PointCollection();
                for (int n2 = 0; n2 < h; n2++)
                {
                    BigInteger j;

                    m = BigInteger.DivRem(m, s.Count, out j);


                    pC.Add(s[(Int32)j]);
                }

                if(pC.Count == 2)
                {
                    Line l = new Line();
                    l.Stroke = new SolidColorBrush(Colors.Black);
                    l.StrokeThickness = 1;

                    l.X1 = pC[0].X;
                    l.Y1 = pC[0].Y;
                    l.X2 = pC[1].X;
                    l.Y2 = pC[1].Y;                    
                    xCanvas.Children.Add(l);
                }
            }            
        }

        private double perimetroPoly(Polygon poly)
        {
            double perimetro = 0;
            Point? primero = null;
            Point? anterior = null;
            Boolean isFirst = true;
            foreach(Point p in poly.Points)
            {
                if(isFirst)
                {
                    primero = p;
                    isFirst = false;
                }

                if(anterior.HasValue)
                {
                    perimetro += distaciaPuntos(anterior.Value, p);
                }
                
                anterior = p;                
            }
            perimetro += distaciaPuntos(anterior.Value, primero.Value);

            return perimetro;
        }


        private double distaciaPuntos(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

    }
}
