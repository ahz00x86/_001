using c666wayServ;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace _666_way
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const uint ST_QR = 0x40000000;

        const string urlJob = "https://reader.datasymbol.com/api/job";
        const string apiKey = "C4rC2IHnPKkw4MpiLjE8";
        const string fileName = "_QR.bmp";

        List<String> qr = new List<string>() {
            "111111101001011100000000000101100000101111111",
            "100000101001010100001111001010101101001000001",
            "101110100010001110101110000010010001001011101",
            "101110101101011101100010100010100101101011101",
            "101110100000110100001111101110011011101011101",
            "100000100000001010111000111011000000001000001",
            "111111101010111110101050101010101010101111111",
            "000000001111011011001000111110110101100000000",
            "011011110001010100001111110110111010011002011",
            "010000010000010001110111001010110101001022001",
            "000111111100011110110011001100110011000000011",
            "011101001101111111001110001001010110000000001",
            "001000110010010110010010110111111010000000011",
            "011001010011101101101110010100101111100000001",
            "110001100001101101010110001011011000000000001",
            "111000001111011000000100010101010101000000010",
            "000001111000000001000100111010000100000000110",
            "111101011110011011101101010100101110000001001",
            "110010100111001110101111011111000000000001100",
            "000010010111001110011011100101100000000110010",
            "101011111110000110101111110011100100111111111",
            "110110001111100011001000110000011100100010111",
            "100010501001101101001050101000010000105010011",
            "101110001111110000111000111000010000100011001",
            "111111111010101010101111101010101010111110100",
            "010000011110100001000110010111110000000111100",
            "001001100110100000010101010010000000000001100",
            "010001010111000001000111001100000000010101111",
            "00100111010100000111011100110000000000100010",
            "100001010100000001000101111100000000001000000",
            "100010110100000001000000110000000000001000000",
            "100101000100000001101001110000000000001000000",
            "100101111100000000010001110000100000000000000",
            "000000010010000100010000010110100000000000000",
            "000010110001011100001010010100100000000000000",
            "011110000100001010011010011100100000000000000",
            "100110100110001000001111100000000000111110000",
            "000000000101000000011000100000000000100010000",
            "111111101010000000001050100000000000105010000",
            "100000101000000000011000100000000000100010000",
            "101110101110000000001111100000000000111110000",
            "101110100010000000000000000000000000000000000",
            "101110101010100001001101010000000000000000000",
            "100000101110101011111101110000000000000000000",
            "111111100011101110000100011000000000000000000"};

        StreamSocket streamSocket;
        c666wayUser u;

        Stream outputStream;
        StreamWriter streamWriter;

        DataReader reader;

        public Rectangle[,] CanScua { get; set; }

        public Rectangle DownScua { get; set; }
        public Rectangle RightScua { get; set; }
                
        public Windows.UI.Xaml.Shapes.Ellipse ActualScua { get; set; }

        public c666way Way { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown; ;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (Way != null)
            {
                if (args.VirtualKey == Windows.System.VirtualKey.S || args.VirtualKey == Windows.System.VirtualKey.Down)
                {
                    ButtonDOWN_Click(null, null);
                }
                else if (args.VirtualKey == Windows.System.VirtualKey.D || args.VirtualKey == Windows.System.VirtualKey.Right)
                {
                    ButtonRIGHT_Click(null, null);
                }
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //connect();

            //////double maxH = cWay.ActualHeight - 20;

            //////double squareH = maxH / 70;

            //////int y = 10;
            //////foreach (String sQR in qr)
            //////{
            //////    int x = 10;
            //////    foreach (Char cQR in sQR)
            //////    {
            //////        String sCQR = cQR.ToString();

            //////        Rectangle r = new Rectangle();
            //////        r.Height = squareH;
            //////        r.Width = squareH;
            //////        r.Fill = new SolidColorBrush(sCQR == "0" ? Colors.White : Colors.Black);
            //////        //r.Stroke = new SolidColorBrush(Colors.Black);
            //////        //r.StrokeThickness = 1;
            //////        r.SetValue(Canvas.LeftProperty, 10 + (x * squareH));
            //////        r.SetValue(Canvas.TopProperty, 10 + (y * squareH));
            //////        cWay.Children.Add(r);
            //////        x++;
            //////    }
            //////    y++;
            //////}
            ///
            //////QRCodeDetector qrD = new QRCodeDetector();
            //////Image<Bgr, Byte> img1 = new Image<Bgr, Byte>("oqr.png");



            ////////using (Mat tmp = new Mat())
            ////////using (OutputArray oaTmp = tmp.GetOutputArray())
            ////////{
            //////VectorOfPoint approxContour = new VectorOfPoint();
            //////    if (qrD.DetectMulti(img1, approxContour))
            //////    {
            //////        String code = qrD.Decode(img1, approxContour);
            //////    }
            //////    else
            //////    {
            //////        Console.WriteLine("A por puas...");
            //////    }
            ////////}

            

            //send job
            string xmlString = Upload(apiKey, urlJob, fileName, ST_QR).Result;
            //xml result
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);

            
        }

        static async Task<String> Upload(string apiKey, string url, string sourceFile, uint barcodeTypes)
        {
            using (HttpClientHandler handler = new HttpClientHandler())
            using (HttpClient client = new HttpClient(handler))
            {
                var request = new MultipartFormDataContent();

                request.Add(new StringContent(apiKey), "key");
                request.Add(new StringContent("barcode-reader"), "target");
                request.Add(new StringContent("xml"), "out_format");
                //request.Add(new StringContent("5"), "lnum");
                request.Add(new StringContent(barcodeTypes.ToString()), "types");
                request.Add(new StreamContent(File.OpenRead(sourceFile)), "source_file", new FileInfo(sourceFile).Name);

                using (HttpResponseMessage response = await client.PostAsync(url, request).ConfigureAwait(false))
                using (HttpContent content = response.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    return data;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            selDiffSP.Visibility = Visibility.Collapsed;
                        
            Button btn = (Button)sender;
            Int32 diff = Convert.ToInt32(btn.Tag);

            CanScua = new Rectangle[diff, diff];

            double maxH = cWay.ActualHeight - 20;
            double maxW = cWay.ActualWidth - 20;

            double squareH = maxH / diff;
            double squareW = maxW / diff;

            for (Int32 x = 0; x < diff; x++)
            {
                for (Int32 y = 0; y < diff; y++)
                {
                    Rectangle r = new Rectangle();
                    r.Height = squareH;
                    r.Width = squareW;                    
                    r.Fill = new SolidColorBrush((x == 0 && y == 0) || x == diff - 1 || y == diff -1 ? Colors.Gold : Colors.White);
                    r.Stroke = new SolidColorBrush(Colors.Black);
                    r.StrokeThickness = 1;
                    r.SetValue(Canvas.LeftProperty, 10 + (x * squareW));
                    r.SetValue(Canvas.TopProperty, 10 + (y * squareH));
                    cWay.Children.Add(r);
                    CanScua[x, y] = r;
                }
            }

            c666way way = new c666way() { Diff = diff, Start = true };
            Way = way;
            sendToServer(way);
        }

        private void ButtonDOWN_Click(object sender, RoutedEventArgs e)
        {
            UpRigthSP.Visibility = Visibility.Collapsed;
            Way.PosY++;
            sendToServer(Way);
        }

        private void ButtonRIGHT_Click(object sender, RoutedEventArgs e)
        {
            UpRigthSP.Visibility = Visibility.Collapsed;
            Way.PosX++;
            sendToServer(Way);
        }

        private async void connect()
        {
            try
            {
                if (streamSocket == null)
                {                    
                    streamSocket = new StreamSocket();
                    var hostName = new Windows.Networking.HostName("localhost"); //192.168.1.46
                    await streamSocket.ConnectAsync(hostName, "63493");

                    u = new c666wayUser() { Password = Guid.NewGuid() };
                    String request = Serializate(u);
                    outputStream = streamSocket.OutputStream.AsStreamForWrite();
                    streamWriter = new StreamWriter(outputStream);
                    streamWriter.Write(request);
                    streamWriter.Flush();

                    reader = new DataReader(streamSocket.InputStream);
                    reader.InputStreamOptions = InputStreamOptions.Partial;

                    Task.Run(gameLoop);
                }
            }
            catch (Exception ex)
            {
                streamSocket = null;
                outputStream = null;
                streamWriter = null;
                reader = null;
                ShowMessage("Error de conexión",
                    ex.Message.ToString() + "\n\r" +
                    //"Ha ocurrido un error durante el intento de conexión, este error puede deberse a la caída de nuestro servidor, o a que no tiene conexión a internet.\n\r\n\r" +
                    //"¿Reintentar la conexión?", 
                    "NO HAY CONEXIóN CON EL SERVIDOR =) DISCULPEN LAS MOLESTIAS",
                    cmd => { });
                //cmd => { connect(); });
            }
        }

        private async void gameLoop()
        {
            try
            {
                while (true)
                {
                    String message = String.Empty;
                    if (reader != null)
                    {
                        uint count = await reader.LoadAsync(10024);
                        message = reader.ReadString(count);
                    }

                    Object m = Deserializate(Encoding.UTF8.GetBytes(message));

                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        if (m is Error)
                        {
                            throw new Exception(((Error)m).Message);

                        }
                        else if (m is c666way)
                        {
                            c666way w = (c666way)m;
                            
                            if (w.Continue)
                            {
                                if(DownScua != null && RightScua != null)
                                {
                                    addNULLAnimation(DownScua);
                                    addNULLAnimation(RightScua);
                                }

                                Way = w;
                                CanScua[w.PosX, w.PosY].Fill = new SolidColorBrush(Colors.Gold);
                                                                
                                DownScua = CanScua[w.PosX, w.PosY + 1];
                                RightScua = CanScua[w.PosX + 1, w.PosY];

                                addAnimation(DownScua);
                                addAnimation(RightScua);

                                // Go Go Go!
                                UpRigthSP.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                // Crash!
                                Way = null;
                                cWay.Children.Clear();
                                selDiffSP.Visibility = Visibility.Visible;
                                UpRigthSP.Visibility = Visibility.Collapsed;
                            }
                        }                        
                        else
                        {
                            throw new Exception("ERROR TRANSCEPTOR");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                if (streamSocket != null)
                {
                    streamSocket.Dispose();
                }

                streamSocket = null;
                outputStream = null;
                streamWriter = null;
                reader = null;
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ShowMessage("Error de servidor",
                    ex.Message.ToString() + "\n\r" +
                    "Ha ocurrido un error en el proceso de comunicación con el servidor",
                    cmd => { });

                    selDiffSP.Visibility = Visibility.Visible;
                    UpRigthSP.Visibility = Visibility.Collapsed;
                });

            }
        }

        private void addAnimation(Rectangle r)
        {
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(TimeSpan.FromSeconds(1.0));
            DoubleAnimation opacityAnimation = new DoubleAnimation()
            {
                From = 1.0,
                To = 0.0,
                BeginTime = TimeSpan.FromSeconds(0.5),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            Storyboard.SetTarget(opacityAnimation, r);            
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            storyboard.Children.Add(opacityAnimation);
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.AutoReverse = true;
            storyboard.Begin();
        }

        private void addNULLAnimation(Rectangle r)
        {
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(TimeSpan.FromSeconds(2.0));
            DoubleAnimation opacityAnimation = new DoubleAnimation()
            {
                From = 1.0,
                To = 1.0,
                BeginTime = TimeSpan.FromSeconds(1.0),
                Duration = new Duration(TimeSpan.FromSeconds(1.0))
            };

            Storyboard.SetTarget(opacityAnimation, r);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            storyboard.Children.Add(opacityAnimation);
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.AutoReverse = true;
            storyboard.Begin();
        }

        private async void ShowMessage(String title, String content, UICommandInvokedHandler yesTo)
        {
            var yesCommand = new UICommand("Sí", yesTo);
            var noCommand = new UICommand("No");
            //var cancelCommand = new UICommand("Cancel", cmd => { ... });

            var dialog = new MessageDialog(content, title);
            dialog.Options = MessageDialogOptions.None;
            dialog.Commands.Add(yesCommand);

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 0;

            if (noCommand != null)
            {
                dialog.Commands.Add(noCommand);
                dialog.CancelCommandIndex = (uint)dialog.Commands.Count - 1;
            }

            //if (cancelCommand != null)
            //{
            //    dialog.Commands.Add(cancelCommand);
            //    dialog.CancelCommandIndex = (uint)dialog.Commands.Count - 1;
            //}

            var command = await dialog.ShowAsync();

            //if (command == yesCommand)
            //{
            //    // handle yes command
            //}
            //else if (command == noCommand)
            //{
            //    // handle no command
            //}
        }
        
        private void sendToServer(Object o)
        {
            if (streamWriter != null)
            {
                String request = Serializate(o);
                streamWriter.Write(request);
                streamWriter.Flush();
            }
        }

        public static String Serializate(Object toSerializate)
        {
            Parameter p = new Parameter() { Type = toSerializate.GetType().FullName, ObjectS = JsonConvert.SerializeObject(toSerializate) };

            string json = JsonConvert.SerializeObject(p);
            return json;
        }

        public static Object Deserializate(byte[] data)
        {
            String json = Encoding.UTF8.GetString(data);
            Parameter p = JsonConvert.DeserializeObject<Parameter>(json);
            if (p != null)
            {
                Object o = JsonConvert.DeserializeObject(p.ObjectS, Type.GetType(p.Type));
                return o;
            }
            else
            {
                return null;
            }
        }

        
    }
}
