using apzGlib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Path = Windows.UI.Xaml.Shapes.Path;
using Point = Windows.Foundation.Point;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace apzyxGames
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Cámara Cámara { get; set; }
        public Entidad Floor { get; set; }
        public List<Entidad> Entidades { get; set; }
        public Cámara BoarCenter { get; set; }

        public double Alfa = 0;
        public double Beta = 0;
        public double Gamma = 0;
        public double Scale = 0;

        double iterInit = 100;
        double iterAdd = 100;
        Boolean tickBi = false;

        public Timer TimerInit { get; set; }

        public MainPage()
        {
            Entidades = new List<Entidad>();

            this.InitializeComponent();

            KeyDown += MainPage_KeyDown;
        }

        private void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.W || e.Key == Windows.System.VirtualKey.Up)
            {
                foreach (Entidad entidad in Entidades)
                {
                    //entidad.Escale(1.01);
                }

            }
            else if (e.Key == Windows.System.VirtualKey.S || e.Key == Windows.System.VirtualKey.Down)
            {
                foreach (Entidad entidad in Entidades)
                {
                    //entidad.Escale(0.99);
                }
            }
            else if (e.Key == Windows.System.VirtualKey.A || e.Key == Windows.System.VirtualKey.Left)
            {
                foreach (Entidad entidad in Entidades)
                {
                    //entidad.Rotate(0,1,0);
                }
            }
            else if (e.Key == Windows.System.VirtualKey.D || e.Key == Windows.System.VirtualKey.Right)
            {
                foreach (Entidad entidad in Entidades)
                {
                    //entidad.Rotate(0,-1,0);
                }
            }
            UpdateGBoard();

        }

        private async void UpdateGBoard()
        {
            proyectar(Entidades, Cámara, GBoard);

            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //{
            //    GBoard.Children.Clear();

            //    //GBoard.Children.Add(Floor.Malla);

            //    foreach (Entidad entidad in Entidades)
            //    {
            //        //Task task = Task.Factory.StartNew(async () =>
            //        //{
            //        //foreach (Polygon p in entidad.)
            //        //{

            //        //    GBoard.Children.Add(p);
            //        //}

            //        //});
            //    }
            //});
        }

        private async void timerCallback(object state)
        {
            //iterAdd -= 1;


            //if(iterAdd < 0)
            //{
            //    tickBi = !tickBi;
            //    iterAdd = iterInit;
            //}

            //Alfa = 1; // new Random().NextDouble();
            //Beta = 3;
            //Gamma = 1; // new Random().NextDouble();
            //Scale = tickBi ? 1.01 : 0.99;

            //foreach (Entidad entidad in Entidades)
            //{
            //    //entidad.Rotate(Alfa, Beta, Gamma);
            //    //entidad.Escale(Scale);
            //}

            //proyectar();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Crear un cubo con un radio de 10, un ángulo theta de 45 grados, un ángulo phi de 30 grados y un lado de 2
            Entidad cubo = await Entidad.CrearCubo(new Átomo(0, 0, 0), 3, 0.25, null, new Material() { Color = Colors.Red });

            Entidades.Add(cubo);

            // Crear una cámara con un centro en el punto (10, 0, 0), un ancho de 5, un alto de 3 y un sentido en el punto (10, 90, 0)
            Cámara = new Cámara(Convert.ToInt32(GBoard.ActualWidth), Convert.ToInt32(GBoard.ActualHeight), new Átomo(0, 0, -15), new Átomo(0, 0, 0));

            AddPointToCanvas();

            proyectar(Entidades, Cámara, GBoard);


            TimerInit = new Timer(timerCallback, null, 50, 50);
        }

        private void AddPointToCanvas()
        {
            Ellipse point = new Ellipse()
            {
                Width = 5,
                Height = 5,
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Red),
                Fill = new SolidColorBrush(Colors.Red)
            };

            // Set the position of the point
            Canvas.SetLeft(point, 50); // X coordinate
            Canvas.SetTop(point, 50);  // Y coordinate

            // Add the point to the canvas
            GBoard.Children.Add(point);
        }

        private async void proyectar(List<Entidad> entidades, Cámara camara, Canvas GBoard)
        {
            WriteableBitmap wb = await getView(entidades, camara);
            addWB(GBoard, wb);

            GBoard.UpdateLayout();
        }

        private async void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog parent = (ContentDialog)this.Parent;


            
        }


        private async Task<WriteableBitmap> getView(List<Entidad> entidades, Cámara c)
        {
            List<Átomo> fAts = new List<Átomo>();


            int maxDegreeOfParallelism = 100; // Número máximo de tareas en paralelo
            SemaphoreSlim semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
            List<Task> tasks = new List<Task>();
            IEnumerable<Átomo> ats = entidades.SelectMany(a => a.Malla);

            await semaphore.WaitAsync();

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    //// Actualizar la interfaz de usuario
                    ////var ignored = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    ////{
                    //// Código que se ejecuta en el subproceso de la interfaz de usuario
                    //// Por ejemplo, actualizar un control de UI




                    //});
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    semaphore.Release();
                }
            }));


            await Task.WhenAll(tasks);

            WriteableBitmap wB = await CreateBitmapFromColorAsync(fAts, c.Ancho, c.Alto);
            
            return wB;
        }

        private void addWB(Canvas gBoard, WriteableBitmap wb)
        {
            
            //Ellipse eA = new Ellipse()
            //{
            //    Width = 2,
            //    Height = 2,
            //    StrokeThickness = 2,
            //    Stroke = new SolidColorBrush(aS.Material != null ? aS.Material.Color : Colors.Black),
            //    Fill = new SolidColorBrush(aS.Material != null ? aS.Material.Color : Colors.Black)
            //};

            //// Set the position of the ellipse
            //Canvas.SetLeft(eA, aS.X + (xMax / 2)); // X coordinate
            //Canvas.SetTop(eA, aS.Y + (yMax / 2));  // Y coordinate
            Image image = new Image
            {
                Source = wb,
                Width = wb.PixelWidth,
                Height = wb.PixelHeight
            };

            GBoard.Children.Clear();
            GBoard.Children.Add(image);
            //GBoard.Children.Add(eA);

        }

        public async Task SaveBitmapAsJpgAsync(WriteableBitmap bitmap, StorageFile file)
        {
            
        }

        //public async Task<IRandomAccessStream> CreateJpgImageAsync()
        //{
        //    // Crear una imagen en memoria
        //    int width = 100;
        //    int height = 100;
        //    using (Bitmap bitmap = new Bitmap(width, height))
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            for (int x = 0; x < width; x++)
        //            {
        //                // Establecer el color del píxel (rojo, verde, azul, alfa)
        //                Color color = Color.FromArgb(255, x % 256, y % 256, (x + y) % 256);
        //                bitmap.SetPixel(x, y, color);
        //            }
        //        }

        //        // Guardar la imagen en un MemoryStream

        //    }
        //}

        public async Task<WriteableBitmap> CreateBitmapFromColorAsync(List<Átomo> atms, int width, int height)
        {
            int i = 0;
            // Crear un array de píxeles
            byte[] pixels = new byte[width * height * 4]; // 4 bytes por píxel (BGRA)

            foreach (Átomo at in atms)
            {
                for (; i < pixels.Length; i += 4)
                {
                    pixels[i] = at.Material.Color.B;     // Blue
                    pixels[i + 1] = at.Material.Color.G; // Green
                    pixels[i + 2] = at.Material.Color.R; // Red
                    pixels[i + 3] = at.Material.Color.A; // Alpha
                }
            }



            // Convertimos los nuevos bytes a un nuevo WriteableBitmap
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                stream.AsStreamForWrite().Write(pixels, 0, pixels.Length);
                stream.Seek(0);
                WriteableBitmap wb = new WriteableBitmap(width, height);
                wb.SetSource(stream);

                return wb;

            }
        }

            //// Convertir el SoftwareBitmap en WriteableBitmap
            //using (var stream = writeableBitmap.PixelBuffer.AsStream())
            //    {
            //        var buffer = new byte[4 * bitmap.PixelWidth * bitmap.PixelHeight];
            //        bitmap.CopyToBuffer(buffer.AsBuffer());
            //        await stream.WriteAsync(buffer, 0, buffer.Length);
            //    }

        //    return bitmap;
            
        //}





        private void Juegos_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Inventario_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Perfil_Click(object sender, RoutedEventArgs e)
        {

        }

        private void j006_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Creditos_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
