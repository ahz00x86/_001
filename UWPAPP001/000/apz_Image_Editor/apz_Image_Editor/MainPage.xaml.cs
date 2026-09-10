using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
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

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace apz_Image_Editor
{    
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string lang = "es-ES";

        public Button AceptarRecorte { get; set; }

        public Rectangle eUL { get; set; }
        public Rectangle eUR { get; set; }
        public Rectangle eDL { get; set; }
        public Rectangle eDR { get; set; }

        public Polyline polylineUP { get; set; }
        public Polyline polylineLEFT{ get; set; }
        public Polyline polylineRIGTH { get; set; }
        public Polyline polylineDOWN { get; set; }

        public WriteableBitmap _wBOriginal { get; set; }
        public WriteableBitmap _wB { get; set; }

        Boolean IsRecortarUL = false;
        Boolean IsRecortarUR = false;
        Boolean IsRecortarDL = false;
        Boolean IsRecortarDR = false;

        public MainPage()
        {            
            this.InitializeComponent();

            imgMain.SizeChanged += ImgMain_SizeChanged;
            cMain.PointerMoved += CMain_PointerMoved;
            this.PointerReleased += MainPage_PointerReleased;
            grid.PointerReleased += MainPage_PointerReleased;
            imgMain.PointerReleased += MainPage_PointerReleased;
            cMain.PointerExited += MainPage_PointerReleased;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //InterstitialAd.ShowInterstitialAd();
        }

        private void MainPage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            IsRecortarUL = false;
            IsRecortarUR = false;
            IsRecortarDL = false;
            IsRecortarDR = false;

            foreach (UIElement uie in cMain.Children)
            {
                Canvas.SetTop(uie, Convert.ToInt32(Canvas.GetTop(uie)));
                Canvas.SetLeft(uie, Convert.ToInt32(Canvas.GetLeft(uie)));
            }
        }

        private void CMain_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pP = e.GetCurrentPoint(cMain);
            double x = pP.Position.X;
            double y = pP.Position.Y;

            if (IsRecortarUL)
            {
                Canvas.SetTop(AceptarRecorte, y + 20);
                Canvas.SetLeft(AceptarRecorte, x + 20);

                Canvas.SetTop(eUL, y);
                Canvas.SetLeft(eUL, x);

                Canvas.SetTop(eUR, y);

                Canvas.SetLeft(eDL, x);
                
                if (polylineUP != null && polylineUP.Points.Count == 2)
                {
                    polylineUP.Points[0] = new Point(x, y);
                    polylineUP.Points[1] = new Point(polylineUP.Points[1].X, y);

                }
                if (polylineLEFT != null && polylineLEFT.Points.Count == 2)
                {
                    polylineLEFT.Points[0] = new Point(x, y);
                    polylineLEFT.Points[1] = new Point(x, polylineLEFT.Points[1].Y);
                }
                if (polylineRIGTH != null && polylineRIGTH.Points.Count == 2)
                {
                    polylineRIGTH.Points[0] = new Point(polylineRIGTH.Points[0].X, y);                    
                }
                if (polylineDOWN != null && polylineDOWN.Points.Count == 2)
                {
                    polylineDOWN.Points[0] = new Point(x, polylineDOWN.Points[0].Y);                    
                }
            }

            if (IsRecortarUR)
            {
                Canvas.SetTop(AceptarRecorte, y + 20);

                Canvas.SetTop(eUL, y);

                Canvas.SetTop(eUR, y);
                Canvas.SetLeft(eUR, x - eUR.Width);

                Canvas.SetLeft(eDR, x - eUR.Width);

                if (polylineUP != null && polylineUP.Points.Count == 2)
                {                    
                    polylineUP.Points[0] = new Point(polylineUP.Points[0].X, y);
                    polylineUP.Points[1] = new Point(x, y);

                }
                if (polylineLEFT != null && polylineLEFT.Points.Count == 2)
                {
                    polylineLEFT.Points[0] = new Point(polylineLEFT.Points[0].X, y);                    
                }
                if (polylineRIGTH != null && polylineRIGTH.Points.Count == 2)
                {                    
                    polylineRIGTH.Points[0] = new Point(x, y);
                    polylineRIGTH.Points[1] = new Point(x, polylineRIGTH.Points[1].Y);
                }
                if (polylineDOWN != null && polylineDOWN.Points.Count == 2)
                {
                    polylineDOWN.Points[1] = new Point(x, polylineDOWN.Points[0].Y);                    
                }
            }

            if (IsRecortarDL)
            {
                Canvas.SetLeft(AceptarRecorte, x + 20);

                Canvas.SetLeft(eUL, x);

                Canvas.SetTop(eDL, y - eDL.Height);
                Canvas.SetLeft(eDL, x);

                Canvas.SetTop(eDR, y - eUR.Height);

                if (polylineUP != null && polylineUP.Points.Count == 2)
                {
                    polylineUP.Points[0] = new Point(x, polylineUP.Points[0].Y);                    

                }
                if (polylineLEFT != null && polylineLEFT.Points.Count == 2)
                {
                    polylineLEFT.Points[0] = new Point(x, polylineLEFT.Points[0].Y);
                    polylineLEFT.Points[1] = new Point(x, y);
                }
                if (polylineRIGTH != null && polylineRIGTH.Points.Count == 2)
                {                    
                    polylineRIGTH.Points[1] = new Point(polylineRIGTH.Points[1].X, y);
                }
                if (polylineDOWN != null && polylineDOWN.Points.Count == 2)
                {
                    polylineDOWN.Points[0] = new Point(x, y);
                    polylineDOWN.Points[1] = new Point(polylineDOWN.Points[1].X, y);
                }
            }

            if (IsRecortarDR)
            {
                Canvas.SetLeft(eUR, x - eUR.Width);

                Canvas.SetTop(eDR, y - eDR.Height);
                Canvas.SetLeft(eDR, x - eDR.Width);

                Canvas.SetTop(eDL, y - eDL.Height);

                if (polylineUP != null && polylineUP.Points.Count == 2)
                {
                    polylineUP.Points[1] = new Point(x, polylineUP.Points[1].Y);

                }
                if (polylineLEFT != null && polylineLEFT.Points.Count == 2)
                {                    
                    polylineLEFT.Points[1] = new Point(polylineLEFT.Points[1].X, y);
                }
                if (polylineRIGTH != null && polylineRIGTH.Points.Count == 2)
                {
                    polylineRIGTH.Points[0] = new Point(x, polylineRIGTH.Points[0].Y);
                    polylineRIGTH.Points[1] = new Point(x, y); 
                }
                if (polylineDOWN != null && polylineDOWN.Points.Count == 2)
                {
                    polylineDOWN.Points[0] = new Point(polylineDOWN.Points[0].X, y);
                    polylineDOWN.Points[1] = new Point(x, y);
                }
            }

        }

        private void ImgMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            cMain.Height = e.NewSize.Height;
            cMain.Width = e.NewSize.Width;

            if (AceptarRecorte != null)
            {
                Canvas.SetTop(AceptarRecorte, 20);
                Canvas.SetLeft(AceptarRecorte, 20);
            }
            if (eUL != null)
            {
                Canvas.SetTop(eUL, 0);
                Canvas.SetLeft(eUL, 0);
            }
            if (eUR != null)
            {
                Canvas.SetTop(eUR, 0);
                Canvas.SetLeft(eUR, e.NewSize.Width - eUR.Width);
            }
            if (eDL != null)
            {
                Canvas.SetTop(eDL, e.NewSize.Height - eDL.Height);
                Canvas.SetLeft(eDL, 0);
            }
            if (eDR != null)
            {
                Canvas.SetTop(eDR, e.NewSize.Height - eDR.Height);
                Canvas.SetLeft(eDR, e.NewSize.Width - eDR.Width);
            }
            if (polylineUP != null && polylineUP.Points.Count == 2)
            {
                polylineUP.Points[0] = new Point(0, 0);
                polylineUP.Points[1] = new Point(e.NewSize.Width, 0);
            }
            if (polylineLEFT != null && polylineLEFT.Points.Count == 2)
            {
                polylineLEFT.Points[0] = new Point(0, 0);
                polylineLEFT.Points[1] = new Point(0, e.NewSize.Height);
            }
            if (polylineRIGTH != null && polylineRIGTH.Points.Count == 2)
            {
                polylineRIGTH.Points[0] = new Point(e.NewSize.Width, 0);
                polylineRIGTH.Points[1] = new Point(e.NewSize.Width, e.NewSize.Height);
            }
            if (polylineDOWN != null && polylineDOWN.Points.Count == 2)
            {
                polylineDOWN.Points[0] = new Point(0, e.NewSize.Height);
                polylineDOWN.Points[1] = new Point(e.NewSize.Width, e.NewSize.Height);
            }
        }
        
        private async void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                try
                {                    
                    ImageProperties properties = await file.Properties.GetImagePropertiesAsync();
                    _wBOriginal = new WriteableBitmap((int)properties.Width, (int)properties.Height);
                    _wBOriginal.SetSource(await file.OpenReadAsync());

                    //ImageBrush brus = new ImageBrush();
                    //brus.ImageSource = _wBOriginal;

                    imgMain.Source = _wBOriginal;
                    _wB = _wBOriginal;

                    // Mostramos el cuadro de opciones al tener imagen
                    spImageOptions.Visibility = Visibility.Visible;
                    spImageOptions2.Visibility = Visibility.Visible;                    
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void btnOriginal_Click(object sender, RoutedEventArgs e)
        {
            imgMain.Source = _wBOriginal;
            _wB = _wBOriginal;
        }

        private void btnRecortar_Click(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            
            cMain.Children.Clear();

            double cH = cMain.ActualHeight;
            double cW = cMain.ActualWidth;

            double whRectangle = 20;

            AceptarRecorte = new Button();
            AceptarRecorte.Content = resourceLoader.GetString("aceptar");
            AceptarRecorte.Background = new SolidColorBrush(Colors.DarkGreen);
            AceptarRecorte.Foreground = new SolidColorBrush(Colors.White);
            AceptarRecorte.Click += AceptarRecorte_Click;
            Canvas.SetTop(AceptarRecorte, 20);
            Canvas.SetLeft(AceptarRecorte, 20);

            eUL = new Rectangle();
            eUL.Width = whRectangle;
            eUL.Height = whRectangle;
            eUL.Stroke = new SolidColorBrush(Colors.White);
            eUL.StrokeThickness = 1;
            eUL.Fill = new SolidColorBrush(Colors.SkyBlue);
            eUL.PointerPressed += EUL_PointerPressed;            
            Canvas.SetTop(eUL, 0);
            Canvas.SetLeft(eUL, 0);

            eUR = new Rectangle();
            eUR.Width = whRectangle;
            eUR.Height = whRectangle;
            eUR.Stroke = new SolidColorBrush(Colors.White);
            eUR.StrokeThickness = 1;
            eUR.Fill = new SolidColorBrush(Colors.SkyBlue);
            eUR.PointerPressed += EUR_PointerPressed; ;
            Canvas.SetTop(eUR, 0);
            Canvas.SetLeft(eUR, cW - eUR.Width);

            eDL = new Rectangle();
            eDL.Width = whRectangle;
            eDL.Height = whRectangle;
            eDL.Stroke = new SolidColorBrush(Colors.White);
            eDL.StrokeThickness = 1;
            eDL.Fill = new SolidColorBrush(Colors.SkyBlue);
            eDL.PointerPressed += EDL_PointerPressed; ; ;
            Canvas.SetTop(eDL, cH - eDL.Height);
            Canvas.SetLeft(eDL, 0);

            eDR = new Rectangle();
            eDR.Width = whRectangle;
            eDR.Height = whRectangle;
            eDR.Stroke = new SolidColorBrush(Colors.White);
            eDR.StrokeThickness = 1;
            eDR.Fill = new SolidColorBrush(Colors.SkyBlue);
            eDR.PointerPressed += EDR_PointerPressed; ; ; ;
            Canvas.SetTop(eDR, cH - eDR.Height);
            Canvas.SetLeft(eDR, cW - eDR.Width);

            GradientStopCollection gSCUP = new GradientStopCollection();
            GradientStopCollection gSCLEFT = new GradientStopCollection();
            GradientStopCollection gSCRIGTH = new GradientStopCollection();
            GradientStopCollection gSCDOWN = new GradientStopCollection();
            GradientStop gS1UP = new GradientStop() { Color = Colors.SkyBlue };
            GradientStop gS2UP = new GradientStop() { Color = Colors.White };
            GradientStop gS3UP = new GradientStop() { Color = Colors.SkyBlue };
            GradientStop gS1LEFT = new GradientStop() { Color = Colors.SkyBlue };
            GradientStop gS2LEFT = new GradientStop() { Color = Colors.White };
            GradientStop gS3LEFT = new GradientStop() { Color = Colors.SkyBlue };
            GradientStop gS1RIGTH = new GradientStop() { Color = Colors.SkyBlue };
            GradientStop gS2RIGTH = new GradientStop() { Color = Colors.White };
            GradientStop gS3RIGTH = new GradientStop() { Color = Colors.SkyBlue };
            GradientStop gS1DOWN = new GradientStop() { Color = Colors.SkyBlue };
            GradientStop gS2DOWN = new GradientStop() { Color = Colors.White };
            GradientStop gS3DOWN = new GradientStop() { Color = Colors.SkyBlue };
            gSCUP.Add(gS1UP);
            gSCUP.Add(gS2UP);
            gSCUP.Add(gS3UP);
            gSCLEFT.Add(gS1LEFT);
            gSCLEFT.Add(gS2LEFT);
            gSCLEFT.Add(gS3LEFT);
            gSCRIGTH.Add(gS1RIGTH);
            gSCRIGTH.Add(gS2RIGTH);
            gSCRIGTH.Add(gS3RIGTH);
            gSCDOWN.Add(gS1DOWN);
            gSCDOWN.Add(gS2DOWN);
            gSCDOWN.Add(gS3DOWN);

            polylineUP = new Polyline();
            polylineUP.Stroke = new LinearGradientBrush(gSCUP,0);
            polylineUP.StrokeThickness = 3;            
            var pointsUP = new PointCollection();
            pointsUP.Add(new Point(0, 0));
            pointsUP.Add(new Point(imgMain.ActualWidth, 0));
            polylineUP.Points = pointsUP;
            Canvas.SetTop(polylineUP, 0);
            Canvas.SetLeft(polylineUP, 0);
            
            polylineLEFT = new Polyline();
            polylineLEFT.Stroke = polylineUP.Stroke = new LinearGradientBrush(gSCLEFT, 90);
            polylineLEFT.StrokeThickness = 3;            
            var pointsLEFT = new PointCollection();
            pointsLEFT.Add(new Point(0, 0));
            pointsLEFT.Add(new Point(0, imgMain.ActualHeight));
            polylineLEFT.Points = pointsLEFT;
            Canvas.SetTop(polylineLEFT, 0);
            Canvas.SetLeft(polylineLEFT, 0);

            polylineRIGTH = new Polyline();
            polylineRIGTH.Stroke = polylineUP.Stroke = new LinearGradientBrush(gSCRIGTH, 90);
            polylineRIGTH.StrokeThickness = 3;            
            var pointsRIGTH = new PointCollection();
            pointsRIGTH.Add(new Point(imgMain.ActualWidth, 0));
            pointsRIGTH.Add(new Point(imgMain.ActualWidth, imgMain.ActualHeight));
            polylineRIGTH.Points = pointsRIGTH;
            Canvas.SetTop(polylineRIGTH, 0);
            Canvas.SetLeft(polylineRIGTH, 0);

            polylineDOWN = new Polyline();
            polylineDOWN.Stroke = new LinearGradientBrush(gSCDOWN, 0);
            polylineDOWN.StrokeThickness = 3;
            polylineDOWN.StrokeLineJoin = PenLineJoin.Round;
            var pointsDOWN = new PointCollection();
            pointsDOWN.Add(new Point(0, imgMain.ActualHeight));
            pointsDOWN.Add(new Point(imgMain.ActualWidth, imgMain.ActualHeight));
            polylineDOWN.Points = pointsDOWN;
            Canvas.SetTop(polylineDOWN, 0);
            Canvas.SetLeft(polylineDOWN, 0);

            cMain.Children.Add(polylineUP);
            cMain.Children.Add(polylineLEFT);
            cMain.Children.Add(polylineRIGTH);
            cMain.Children.Add(polylineDOWN);
            cMain.Children.Add(eUL);
            cMain.Children.Add(eUR);
            cMain.Children.Add(eDL);
            cMain.Children.Add(eDR);
            cMain.Children.Add(AceptarRecorte);
        }

        private async void AceptarRecorte_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double wI = imgMain.ActualWidth;
                double hI = imgMain.ActualHeight;

                Int32 wWB = _wB.PixelWidth;
                Int32 hWB = _wB.PixelHeight;

                double wPorcentual = wWB / wI;
                double hPorcentual = hWB / hI;

                Int32 xUL = Convert.ToInt32(Canvas.GetLeft(eUL) * wPorcentual);
                Int32 yUL = Convert.ToInt32(Canvas.GetTop(eUL) * hPorcentual);

                Int32 xUR = Convert.ToInt32((wI - (Canvas.GetLeft(eUR) + eUR.ActualWidth)) * wPorcentual);
                Int32 yUR = Convert.ToInt32(Canvas.GetTop(eUR) * hPorcentual);

                Int32 xDL = Convert.ToInt32(Canvas.GetLeft(eDL) * wPorcentual);
                Int32 yDL = Convert.ToInt32((hI - (Canvas.GetTop(eDL) + eDL.ActualHeight)) * hPorcentual);

                Int32 xDR = Convert.ToInt32((wI - (Canvas.GetLeft(eDR) + eDR.ActualWidth)) * wPorcentual); ;
                Int32 yDR = Convert.ToInt32((hI - (Canvas.GetTop(eDR) + eDR.ActualHeight)) * hPorcentual); ;

                Int32 resW = wWB - xUL - xUR;
                Int32 resH = hWB - yDL - yUL;
                
                Int32 maxH = yDL + resH;
                Int32 maxW = xDL + resW;

                // Calcular with height a partir de los datos text,
                Int32 widthWF = maxW > wWB ? resW - 1 : resW;
                Int32 heightWF = maxH > hWB ? resH - 1 : resH;

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

                Int32 row = 0;
                Int32 col = 0;
                

                for (Int32 j = 54; j < bytes.Length; j += 4)
                {
                    if (row >= yDL && col >= xDL && row < (maxH > hWB ? hWB : maxH) && col < (maxW > wWB ? wWB : maxW))
                    {
                        var b = bytes[j];
                        var g = bytes[j + 1];
                        var r = bytes[j + 2];
                        var a = bytes[j + 3];

                        lsBytesD.Add(r);
                        lsBytesD.Add(g);
                        lsBytesD.Add(b);
                        lsBytesD.Add(a);
                    }
                    col++;

                    if (col != 0 && col % _wB.PixelWidth == 0)
                    {
                        row++;
                        col = 0;
                    }
                }

                Byte[] bytesD = lsBytesD.ToArray();

                Byte[] resultBytes = new Byte[bytesH.Length + bytesD.Length];
                Array.Copy(bytesH, resultBytes, bytesH.Length);
                Array.Copy(bytesD, 0, resultBytes, bytesH.Length, bytesD.Length);

                MemoryStream mS = new MemoryStream(resultBytes);

                WriteableBitmap wb = new WriteableBitmap(widthWF, heightWF);
                await wb.SetSourceAsync(mS.AsRandomAccessStream());

                _wB = wb;
                imgMain.Source = _wB;
            }
            catch (Exception ex)
            {
                var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                
                // Create the message dialog and set its content
                var messageDialog = new MessageDialog(resourceLoader.GetString("regionNoValida"));
                messageDialog.Commands.Add(new UICommand(resourceLoader.GetString("close")));
                messageDialog.CancelCommandIndex = 0;
                // Show the message dialog
                await messageDialog.ShowAsync();
            }
            cMain.Children.Clear();

        }

        private void EDR_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            IsRecortarUL = false;
            IsRecortarUR = false;
            IsRecortarDL = false;
            IsRecortarDR = true;
        }

        private void EDL_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            IsRecortarUL = false;
            IsRecortarUR = false;
            IsRecortarDL = true;
            IsRecortarDR = false;
        }

        private void EUR_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            IsRecortarUL = false;
            IsRecortarUR = true;
            IsRecortarDL = false;
            IsRecortarDR = false;
        }

        private void EUL_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            IsRecortarUL = true;
            IsRecortarUR = false;
            IsRecortarDL = false;
            IsRecortarDR = false;
        }

        private async void btnBN_Click(object sender, RoutedEventArgs e)
        {
            Byte[] bytes = await SDK.GetBNImage(_wB);

            // Convertimos los nuevos bytes a un nuevo WriteableBitmap
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                stream.AsStreamForWrite().Write(bytes, 0, bytes.Length);
                stream.Seek(0);
                WriteableBitmap wb = new WriteableBitmap(_wB.PixelWidth, _wB.PixelHeight);                
                wb.SetSource(stream);
                _wB = wb;
                
                imgMain.Source = _wB;
            }
        }

        private async void btnCut_Click(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            
            Trocear tro = new Trocear(_wB);
            ContentDialog ucDialog = new ContentDialog() { Title = resourceLoader.GetString("trocear"), Content = tro, HorizontalAlignment = HorizontalAlignment.Center, 
                Background = new SolidColorBrush(Colors.Black) };
            await ucDialog.ShowAsync();
        }

        private async void btnFrame_Click(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            Enmarcar tro = new Enmarcar(_wB);
            ContentDialog ucDialogEnmarcar = new ContentDialog() { Title = resourceLoader.GetString("enmarcar"), Content = tro, HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Colors.Black)
            };
            ucDialogEnmarcar.Closed += UcDialogEnmarcar_Closed;
            await ucDialogEnmarcar.ShowAsync();
        }

        private void UcDialogEnmarcar_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            if (sender.Tag is WriteableBitmap)
            {
                WriteableBitmap wb = (WriteableBitmap)sender.Tag;

                _wB = wb;
                imgMain.Source = _wB;
            }
        }
        private async void btnBordes_Click(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            Bordes tro = new Bordes(_wB);
            ContentDialog ucDialogBorde = new ContentDialog() { Title = resourceLoader.GetString("pixelar"), Content = tro, HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Colors.Black)
            };
            ucDialogBorde.Closed += UcDialogBorde_Closed;
            await ucDialogBorde.ShowAsync();
        }

        private void UcDialogBorde_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            if (sender.Tag is WriteableBitmap)
            {
                WriteableBitmap wb = (WriteableBitmap)sender.Tag;

                _wB = wb;
                imgMain.Source = _wB;
            }
        }

        private async void btnInvColores_Click(object sender, RoutedEventArgs e)
        {
            Byte[] bytes = await SDK.WBToBytes(_wB);

            for (Int32 j = 54; j < bytes.Length; j += 4)
            {
                var b = Convert.ToInt32(bytes[j]);
                var g = Convert.ToInt32(bytes[j + 1]);
                var r = Convert.ToInt32(bytes[j + 2]);
                
                bytes[j] = Convert.ToByte(255 - r);
                bytes[j + 1] = Convert.ToByte(255 - g);
                bytes[j + 2] = Convert.ToByte(255 - b);
            }

            // Convertimos los nuevos bytes a un nuevo WriteableBitmap
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                stream.AsStreamForWrite().Write(bytes, 0, bytes.Length);
                stream.Seek(0);
                WriteableBitmap wb = new WriteableBitmap(_wB.PixelWidth, _wB.PixelHeight);
                wb.SetSource(stream);
                _wB = wb;

                imgMain.Source = _wB;
            }
        }
                
        private async void slBrillo_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Slider slider = (Slider)sender;            
            cambiarBrillo(Convert.ToInt32(slider.Value));
            slBrillo.Value = 0;
        }

        private async void slContraste_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Slider slider = (Slider)sender;
            cambiarContraste(Convert.ToInt32(slider.Value));
            slContraste.Value = 0;
        }                            

        private void btnVerAnuncio_Click(object sender, RoutedEventArgs e)
        {
            //InterstitialAd.ShowInterstitialAd();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Jpg files", new List<string>() { ".jpg" });
            savePicker.FileTypeChoices.Add("Png files", new List<string>() { ".png" });
            savePicker.FileTypeChoices.Add("Bmp files", new List<string>() { ".bmp" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "NewSavedImage";

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                String[] extensionTemp = file.Name.Split('.');

                String fileExtension = extensionTemp[extensionTemp.Length - 1].ToLower();

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(
                        fileExtension == "jpg" ? BitmapEncoder.JpegEncoderId :
                        fileExtension == "png" ? BitmapEncoder.PngEncoderId :
                        fileExtension == "bmp" ? BitmapEncoder.BmpEncoderId : BitmapEncoder.JpegEncoderId
                        , stream);
                    Stream pixelStream = _wB.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                                        (uint)_wB.PixelWidth,
                                        (uint)_wB.PixelHeight,
                                        96.0,
                                        96.0,
                                        pixels);
                    await encoder.FlushAsync();
                }
            }
        }

        private async void btnContornos_Click(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                        
            Contornos con = new Contornos(_wB);
            ContentDialog cDContor = new ContentDialog() { Title = resourceLoader.GetString("dibujarContornos"), Content = con, HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Colors.Black)
            };
            cDContor.Closed += CDContor_Closed;
            await cDContor.ShowAsync();
        }

        private void CDContor_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            if (sender.Tag is WriteableBitmap)
            {
                WriteableBitmap wb = (WriteableBitmap)sender.Tag;

                _wB = wb;
                imgMain.Source = _wB;
            }
        }

        private async void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            Byte[] bytes = await SDK.WBToBytes(_wB);
            Int32 w = _wB.PixelWidth;
            Int32 h = _wB.PixelHeight;

            Int32 row = 0;
            Int32 col = 0;

            List<Color[]> colorsMatrix = new List<Color[]>();
            Color[] colorRow = new Color[w];
            for (Int32 j = 54; j < bytes.Length; j += 4)
            {
                var b = bytes[j];
                var g = bytes[j + 1];
                var r = bytes[j + 2];
                var a = bytes[j + 3];
                                
                colorRow[col] = Color.FromArgb(a, r, g, b);

                col++;

                if (col != 0 && col % w == 0)
                {
                    Color[] tempCRow = new Color[w];
                    colorRow.CopyTo(tempCRow, 0);
                    colorsMatrix.Add(tempCRow);
                    colorRow = new Color[w];
                    row++;
                    col = 0;
                }
            }

            Dictionary<Int32[], Color> pixelAfectados = new Dictionary<Int32[], Color>();
            for (row = 0; row < h; row++)
            {
                for (col = 0; col < w; col++)
                {
                    List<Color> cSPixel = new List<Color>();
                    // arriba
                    if (row > 0)
                    {
                        cSPixel.Add(colorsMatrix[row - 1][col]);
                    }
                    // abajo
                    if ((row + 1) < h)
                    {
                        cSPixel.Add(colorsMatrix[row + 1][col]);
                    }
                    // izquierda
                    if (col > 0)
                    {
                        cSPixel.Add(colorsMatrix[row][col - 1]);
                    }
                    // derecha
                    if ((col + 1) < w)
                    {
                        cSPixel.Add(colorsMatrix[row][col + 1]);
                    }
                    // diagonal arriba izquierda
                    if (row > 0 && col > 0)
                    {
                        cSPixel.Add(colorsMatrix[row - 1][col - 1]);
                    }
                    // diagonal abajo derecha
                    if ((row + 1) < h && (col + 1) < w)
                    {
                        cSPixel.Add(colorsMatrix[row + 1][col + 1]);
                    }
                    // diagonal arriba derecha
                    if (row > 0 && (col + 1) < w)
                    {
                        cSPixel.Add(colorsMatrix[row - 1][col + 1]);
                    }
                    // diagonal abajo izquierda
                    if ((row + 1) < h && col > 0)
                    {
                        cSPixel.Add(colorsMatrix[row + 1][col - 1]);
                    }

                    //var cSOrder = cSPixel.GroupBy(x => x).Select(x => new { c = x.Key, count = x.Count() }).OrderByDescending(o => o.count).FirstOrDefault();

                    Color? cCambio = SDK.ComprobarListadoDeColoresPorToleranciaLIMPIAR(cSPixel, colorsMatrix[row][col], 80);

                    if (cCambio.HasValue)
                    {
                        //colorsMatrix[row][col] = cCambio.Value;

                        pixelAfectados.Add(new Int32[] { row, col }, cCambio.Value);

                    }
                }
            }

            if (pixelAfectados.Count > 0)
            {
                foreach (Int32[] localizacion in pixelAfectados.Keys)
                {
                    colorsMatrix[localizacion[0]][localizacion[1]] = pixelAfectados[localizacion];
                }
            }

            row = 0;
            col = 0;
            // Componemos el nuevo archivo
            for (Int32 j = 54; j < bytes.Length; j += 4)
            {
                bytes[j] = colorsMatrix[row][col].R;
                bytes[j + 1] = colorsMatrix[row][col].G;
                bytes[j + 2] = colorsMatrix[row][col].B;
                bytes[j + 3] = colorsMatrix[row][col].A;
                
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
                _wB = wb;

                imgMain.Source = _wB;
            }
        }

        private void btnMenosContraste_Click(object sender, RoutedEventArgs e)
        {
            cambiarContraste(-1);
        }

        private void btnMasContraste_Click(object sender, RoutedEventArgs e)
        {
            cambiarContraste(1);
        }

        private async void cambiarBrillo(Int32 value)
        {            
            Byte[] bytes = await SDK.WBToBytes(_wB);

            for (Int32 j = 54; j < bytes.Length; j += 4)
            {
                var b = Convert.ToInt32(bytes[j]);
                var g = Convert.ToInt32(bytes[j + 1]);
                var r = Convert.ToInt32(bytes[j + 2]);
                var a = Convert.ToInt32(bytes[j + 3]);

                b = b + value;
                g = g + value;
                r = r + value;

                bytes[j] = Convert.ToByte(value > 0 ? (r > 255 ? 255 : r) : (r < 0 ? 0 : r));
                bytes[j + 1] = Convert.ToByte(value > 0 ? (g > 255 ? 255 : g) : (g < 0 ? 0 : g));
                bytes[j + 2] = Convert.ToByte(value > 0 ? (b > 255 ? 255 : b) : (b < 0 ? 0 : b));
            }

            // Convertimos los nuevos bytes a un nuevo WriteableBitmap
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                stream.AsStreamForWrite().Write(bytes, 0, bytes.Length);
                stream.Seek(0);
                WriteableBitmap wb = new WriteableBitmap(_wB.PixelWidth, _wB.PixelHeight);
                wb.SetSource(stream);
                _wB = wb;

                imgMain.Source = _wB;
            }
        }

        private async void cambiarContraste(Int32 value)
        {                        
            float sliderValue = (100.0f + value) / 100.0f; ;
            sliderValue *= sliderValue;
            Byte[] bytes = await SDK.WBToBytes(_wB);

            for (Int32 j = 54; j < bytes.Length; j += 4)
            {
                var b = Convert.ToInt32(bytes[j]);
                var g = Convert.ToInt32(bytes[j + 1]);
                var r = Convert.ToInt32(bytes[j + 2]);
                var a = Convert.ToInt32(bytes[j + 3]);

                float Red = r / 255.0f;
                float Green = g / 255.0f;
                float Blue = b / 255.0f;
                Red = (((Red - 0.5f) * sliderValue) + 0.5f) * 255.0f;
                Green = (((Green - 0.5f) * sliderValue) + 0.5f) * 255.0f;
                Blue = (((Blue - 0.5f) * sliderValue) + 0.5f) * 255.0f;

                bytes[j] = Convert.ToByte(Red > 255 ? 255 : Red < 0 ? 0 : Red);
                bytes[j + 1] = Convert.ToByte(Green > 255 ? 255 : Green < 0 ? 0 : Green);
                bytes[j + 2] = Convert.ToByte(Blue > 255 ? 255 : Blue < 0 ? 0 : Blue);
            }

            // Convertimos los nuevos bytes a un nuevo WriteableBitmap
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                stream.AsStreamForWrite().Write(bytes, 0, bytes.Length);
                stream.Seek(0);
                WriteableBitmap wb = new WriteableBitmap(_wB.PixelWidth, _wB.PixelHeight);
                wb.SetSource(stream);
                _wB = wb;

                imgMain.Source = _wB;
            }
        }

        private void btnMasBrillo_Click(object sender, RoutedEventArgs e)
        {
            cambiarBrillo(1);
        }

        private void btnMenosBrillo_Click(object sender, RoutedEventArgs e)
        {
            cambiarBrillo(-1);
        }

        private void btnIdioma_Click(object sender, RoutedEventArgs e)
        {
            

            switch (lang)
            {
                case "es-ES":
                    lang = "en";
                    break;
                case "en":
                    lang = "fr";
                    break;
                case "fr":
                    lang = "es-ES";
                    break;
            }

            ApplicationLanguages.PrimaryLanguageOverride = lang;
            Frame.Navigate(this.GetType());
        }

        private async void btnSpanish_Click(object sender, RoutedEventArgs e)
        {
            ApplicationLanguages.PrimaryLanguageOverride = "es-ES";
            await Task.Delay(100);
            Frame.Navigate(this.GetType());
        }

        private async void btnEnglish_Click(object sender, RoutedEventArgs e)
        {
            ApplicationLanguages.PrimaryLanguageOverride = "en";
            await Task.Delay(100);
            Frame.Navigate(this.GetType());
        }

        private async void btnFrench_Click(object sender, RoutedEventArgs e)
        {
            ApplicationLanguages.PrimaryLanguageOverride = "fr";
            await Task.Delay(100);
            Frame.Navigate(this.GetType());
        }

        private async void btnEncuadre_Click(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            Encuadrar tro = new Encuadrar(_wB);
            ContentDialog ucDialogBorde = new ContentDialog()
            {
                Title = "QR",//resourceLoader.GetString("pixelar")"",
                Content = tro,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Colors.Black)
            };
            ucDialogBorde.Closed += UcDialogEncuadrar_Closed1; ;
            await ucDialogBorde.ShowAsync();
        }

        private void UcDialogEncuadrar_Closed1(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            if (sender.Tag is WriteableBitmap)
            {
                WriteableBitmap wb = (WriteableBitmap)sender.Tag;

                _wB = wb;
                imgMain.Source = _wB;
            }
        }


        //String sHex = String.Empty;
        //foreach (Byte b in bytes)
        //{
        //    sHex += Convert.ToString(b, 16).PadLeft(2, '0');
        //}

        ////var savePicker = new Windows.Storage.Pickers.FileSavePicker();
        ////savePicker.SuggestedStartLocation =
        ////    Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
        ////// Dropdown of file types the user can save the file as
        ////savePicker.FileTypeChoices.Add("Txt files", new List<string>() { ".txt" });
        ////savePicker.SuggestedFileName = "NewSavedImage";

        ////Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

        ////if (file != null)
        ////{
        ////    await FileIO.WriteLinesAsync(file, datos01);

        ////}
    }
}
