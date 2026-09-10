using FarmsWar.Classes;
using FarmsWar.Sprites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media.Media3D;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace FarmsWar
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StreamSocket streamSocket;        
        Classes.User u;

        Stream outputStream;
        StreamWriter streamWriter;

        DataReader reader;

        public double LastCanvasHeight { get; set; }
        public double LastCanvasWidth { get; set; }

        public List<Room> Rooms { get; set; }

        public Boolean IsMultiplayer { get; set; }
        public Int16 PlayerState { get; set; }

        public Int16[] PlayersPositions { get; set; }
        public Int16 FinalPunt { get; set; }

        public Int16 PuntMyTeam { get; set; }

        public Int16 LastMainPlayerRound { get; set; }

        public Int16 PuntOtherTeam { get; set; }


        private Boolean nuevaPartida = true;

        public Boolean NuevaPartida
        {
            get
            {
                if (nuevaPartida)
                {
                    return true;
                }
                else
                {
                    if (PlayersPositions[PlayerRound] == 3)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            set
            {
                nuevaPartida = value;
            }
        }

        public double LastPosXnumA { get; set; }
        public double LastPosYnumA { get; set; }
        public double LastPosXnumB { get; set; }
        public double LastPosYnumB { get; set; }

        public Button Turn { get; set; }
        public Boolean IsRotateA;
        public Boolean IsRotateB;

        public Int16 DireccionA;
        public Int16 DireccionB;

        public Int16 numDominoTabA { get; set; }
        public Int16 numDominoTabB { get; set; }
        public Image ImagePlayerRound { get; set; }
        public DominoTabServ EmptyTab { get; set; }
        public Int16 PlayerRound { get; set; }

        public TextBlock txtCountNewRound { get; set; }
        public double DominoTabHeigth { get; set; }
        public double DominoTabWidth { get; set; }

        public double ImgMovePlayerHeigth { get; set; }
        public double ImgMovePlayerWidth { get; set; }

        public Boolean IsFirst { get; set; }

        public double PointerX { get; set; }
        public double PointerY { get; set; }

        public DominoTabSprite Pressed { get; set; }


        public Int16 PlayerId { get; set; }

        public Int16[][] AllDominoTabs { get {
                return new Int16[28][] {
                new Int16[2] { 0, 0 }, new Int16[2] { 0, 1 }, new Int16[2] { 0, 2 }, new Int16[2] { 0, 3 }, new Int16[2] { 0, 4 }, new Int16[2] { 0, 5 }, new Int16[2] { 0, 6 },
                new Int16[2] { 1, 1 }, new Int16[2] { 1, 2 }, new Int16[2] { 1, 3 }, new Int16[2] { 1, 4 }, new Int16[2] { 1, 5 }, new Int16[2] { 1, 6 },
                new Int16[2] { 2, 2 }, new Int16[2] { 2, 3 }, new Int16[2] { 2, 4 }, new Int16[2] { 2, 5 }, new Int16[2] { 2, 6 },
                new Int16[2] { 3, 3 }, new Int16[2] { 3, 4 }, new Int16[2] { 3, 5 }, new Int16[2] { 3, 6 }, 
                new Int16[2] { 4, 4 }, new Int16[2] { 4, 5 }, new Int16[2] { 4, 6 }, 
                new Int16[2] { 5, 5 }, new Int16[2] { 5, 6 },
                new Int16[2] { 6, 6 }  };
            } 
        }
                
        public List<DominoTab> PlayerDominoTabs { get; set; }
        public List<DominoTab> OthersDominoTabs { get; set; }
        public List<DominoTab> InGameDominoTabs { get; set; }


        public Int16 nElems = 25;

        public ScaleTransform sT { get; set; }

        public List<gameMain> ElementsAvailable { get; set; }

        public List<gameMain> ElementsInGame { get; set; }

        public gameMain[][] Matrix { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            ElementsAvailable = new List<gameMain>();
            ElementsInGame = new List<gameMain>();
            //MyDominoTabs = new List<DominoTab>();
            PlayerDominoTabs = new List<DominoTab>();
            OthersDominoTabs = new List<DominoTab>();
            InGameDominoTabs = new List<DominoTab>();
            sT = new ScaleTransform();
            cMain.PointerMoved += Canvas_PointerMoved;
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            //setMatrix();          
            //Task.Run(loop);                        
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            connect();
        }

        private async void connect()
        {
            try
            {
                if (streamSocket == null)
                {
                    btnConnect.IsEnabled = false;
                    btnConnect.Visibility = Visibility.Collapsed;

                    streamSocket = new StreamSocket();
                    var hostName = new Windows.Networking.HostName("192.168.1.46"); //192.168.1.46
                    await streamSocket.ConnectAsync(hostName, "63495");

                    u = new Classes.User() { Id = Guid.NewGuid(), Password = Guid.NewGuid() };
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
                    "EN ESTOS MOMENTOS EL MODO MULTIJUGADOR ESTá DESACTIVADO, ESTAMOS TRABAJANDO EN éL, Y ADEMás, NECESITAMOS PATROCINADORES PARA FINANCIAR EL SERVIDOR =) DISCULPEN LAS MOLESTIAS",
                    cmd => { });
                //cmd => { connect(); });
            }
        }                

        private async void gameLoop()
        {
            try
            {
                // Empezamos =)
                PlayerState = 1;

                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    double xMidPage = cMain.ActualWidth / 2;
                    double yMidPage = cMain.ActualHeight / 2;
                
                    Button btn = new Button();
                    btn.Content = "Crear mesa";
                    btn.Width = 200;
                    btn.Height = 50;
                    btn.SetValue(Canvas.TopProperty, 0);
                    btn.SetValue(Canvas.LeftProperty, xMidPage - 100);
                    btn.Click += AddRoom_ButtonClick;
                    cMain.Children.Add(btn);
                });

                while (true)
                {
                    String message = String.Empty;
                    if (reader != null)
                    {
                        uint count = await reader.LoadAsync(1000024);
                        message = reader.ReadString(count);
                    }

                    Object m = Deserializate(Encoding.UTF8.GetBytes(message));

                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        if (m is String)
                        {
                            btnDomino.Content = m.ToString();
                        }
                        else if (m is Message)
                        {
                            //txtMessage.Text = ((Message)m).Mess;
                        }
                        else if (m is Error)
                        {
                            throw new Exception(((Error)m).Message);

                        }
                        else if (m is Room[])
                        {
                            Rooms = ((Room[])m).ToList();

                            if (PlayerState == 1)
                            {
                                addAllRooms();
                            }
                        }
                        else if (m is Room)
                        {
                            if (PlayerState == 1 || PlayerState == 2)
                            {
                                if (Rooms != null)
                                {
                                    Room r = (Room)m;
                                    r.MyIdUsuario = u.Id;

                                    if (r.Add)
                                    {
                                        addRoom(r);
                                    }
                                    else if (r.Update || r.Delete)
                                    {
                                        foreach (UIElement uie in cMain.Children)
                                        {
                                            if (uie is DominoRoomSprite)
                                            {
                                                DominoRoomSprite dRS = (DominoRoomSprite)uie;
                                                Room rInDataContext = (Room)dRS.DataContext;

                                                if (rInDataContext.IdRoom == r.IdRoom)
                                                {
                                                    dRS.DataContext = r;
                                                    break;
                                                }
                                            }
                                        }
                                        //for (Int32 i = 0; i < Rooms.Count; i++)
                                        //{
                                        //    if (Rooms[i].IdRoom == r.IdRoom)
                                        //    {
                                        //        Rooms[i].Users = r.Users;
                                        //        Rooms[i].UsersOcupation = r.UsersOcupation;
                                        //        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                        //        {
                                        //            Rooms[i].NotifyPropertyChanged("Users");
                                        //            Rooms[i].NotifyPropertyChanged("UsersOcupation");
                                        //        });

                                        //    }
                                        //}
                                    }
                                    else if (r.DeleteRoom)
                                    {
                                        DominoRoomSprite dRS = null;

                                        foreach (UIElement uie in cMain.Children)
                                        {
                                            if (uie is DominoRoomSprite)
                                            {
                                                dRS = (DominoRoomSprite)uie;
                                                Room rInDataContext = (Room)dRS.DataContext;

                                                if (rInDataContext.IdRoom == r.IdRoom)
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        if (dRS != null)
                                        {
                                            cMain.Children.Remove(dRS);
                                        }
                                    }
                                    else
                                    {
                                        ShowMessage("Inconsistencia de datos", "Ocurrió una insconsistencia de datos, seguimos trabajando para mejorar la app =)", cmd => { });
                                    }

                                }
                            }
                        }
                        else if (m is Start)
                        {
                            // EMPIEZA LA PARTIDA
                            Start start = (Start)m;

                            if (!start.IsFirst)
                            {

                                if(start.FinalTab != null)
                                {
                                    paintNewDominoTab(start.FinalTab);
                                }

                                foreach(DominoTab dTOther in OthersDominoTabs)
                                {
                                    cMain.Children.Remove(dTOther.Sprite);
                                }
                                OthersDominoTabs.Clear();

                                if (start.FinalPlayerTabs != null && start.FinalPlayerTabs.Length > 0)
                                {
                                    // TODO: Pintar las fichas finales de los contrincantes en cada posición sin pintar la de este usuario

                                    double maxPositionY = 0;
                                    double positionXm = 0;

                                    Int16 playerPostition = -1;
                                    Int16 player = -1;
                                    for (Int16 i = 0; i < 4; i++)
                                    {
                                        if (i != PlayerId)
                                        {
                                            if (i != player)
                                            {
                                                player = i;
                                                playerPostition = PlayersPositions[player];

                                                switch (playerPostition)
                                                {
                                                    case 0:
                                                        maxPositionY = (cMain.ActualHeight / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                                        positionXm = 5 + DominoTabHeigth;
                                                        break;

                                                    case 1:
                                                        maxPositionY = 5;
                                                        positionXm = (cMain.ActualWidth / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                                        break;

                                                    case 2:
                                                        maxPositionY = (cMain.ActualHeight / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                                        positionXm = cMain.ActualWidth - 5;
                                                        break;
                                                }
                                            }

                                            // Recorremos 7 fichas por cada jugador (Estas seran las fichas de los contrincantes)
                                            foreach (DominoTabServ dTS in start.FinalPlayerTabs.Where(x => x.Player == player))
                                            {
                                                DominoTab dT = new DominoTab();
                                                DominoTabSprite r = new DominoTabSprite();                                                
                                                dT.Values = dTS.Values;
                                                r.DataContext = dT;
                                                r.Tag = dT;
                                                dT.Player = player;
                                                dT.SourceX = positionXm;
                                                dT.SourceY = maxPositionY;
                                                dT.Sprite = r;
                                                r.SetValue(Canvas.TopProperty, maxPositionY);
                                                r.SetValue(Canvas.LeftProperty, positionXm);

                                                switch (playerPostition)
                                                {
                                                    case 0:
                                                        r.Rotation = 90;
                                                        dT.IsRotate = true;
                                                        cMain.Children.Add(r);

                                                        maxPositionY += DominoTabWidth + 2;
                                                        break;

                                                    case 1:
                                                        r.Height = DominoTabHeigth;
                                                        r.Width = DominoTabWidth;
                                                        cMain.Children.Add(r);

                                                        positionXm += DominoTabWidth + 2;
                                                        break;

                                                    case 2:
                                                        r.Rotation = 90;
                                                        dT.IsRotate = true;
                                                        cMain.Children.Add(r);

                                                        maxPositionY += DominoTabWidth + 2;
                                                        break;
                                                }
                                            }
                                        }
                                    }

                                }

                                myScore.Text = PlayerId == 0 || PlayerId == 2 ? "Mi Equipo: " + start.PuntTeam2.ToString() : "Mi Equipo: " + start.PuntTeam1.ToString();
                                otherScore.Text = PlayerId == 0 || PlayerId == 2 ? "Equipo 2: " + start.PuntTeam1.ToString() : "Equipo 2: " + start.PuntTeam2.ToString();

                                double posMainMidY = cMain.ActualHeight / 2;
                                double posMainMidX = cMain.ActualWidth / 2;

                                txtCountNewRound = new TextBlock();
                                txtCountNewRound.Text = "15";
                                countNewRound = 15;
                                txtCountNewRound.TextAlignment = TextAlignment.Center;
                                txtCountNewRound.Height = 200;
                                txtCountNewRound.Width = 200;
                                txtCountNewRound.FontSize = 100;
                                txtCountNewRound.Foreground = new SolidColorBrush(Colors.Transparent);
                                txtCountNewRound.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                                cMain.Children.Add(txtCountNewRound);
                                txtCountNewRound.SetValue(Canvas.TopProperty, posMainMidY - (txtCountNewRound.Height / 2));
                                txtCountNewRound.SetValue(Canvas.LeftProperty, posMainMidX - (txtCountNewRound.Width / 2));

                                DispatcherTimer timer = new DispatcherTimer();
                                timer.Tick += Timer_Tick;
                                timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                                timer.Start();

                                await Task.Delay(15000);

                                timer.Stop();

                            }
                            else
                            {
                                myScore.Text = "Mi Equipo: 0";
                                otherScore.Text = "Equipo 2:  0";
                            }

                            PlayerState = 3;

                            cMain.Children.Clear();
                            PlayerDominoTabs = new List<DominoTab>();
                            OthersDominoTabs = new List<DominoTab>();
                            InGameDominoTabs = new List<DominoTab>();
                            DominoTabHeigth = 66;
                            DominoTabWidth = 33;
                            ImgMovePlayerHeigth = 50;
                            ImgMovePlayerWidth = 50;
                            IsRotateA = false;
                            IsRotateB = false;
                            IsMultiplayer = true;
                            DireccionA = 0;
                            DireccionB = 1;                            
                            PlayersPositions = new Int16[4];
                            IsFirst = false;
                            setDominoGame(start);
                        }
                        else if (m is DominoTabServ)
                        {
                            DominoTabServ dTServer = (DominoTabServ)m;

                            if (!dTServer.Turn)
                            {
                                paintNewDominoTab(dTServer);

                            }
                            else
                            {
                                MovePlayer();
                            }
                        }
                        else if (m is StateError)
                        {
                            StateError state = (StateError)m;

                            if (state.IsError)
                            {
                                PlayerState = state.GoToState;
                            }

                            ShowMessage("Aviso de aplicación", state.Message, cmd => { });
                        }
                        else
                        {
                            throw new Exception("La interfaz no usa los parámetros de forma correcta");
                        }
                    });
                }
            }
            catch(Exception ex)
            {
                if(streamSocket != null)
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

                    btnConnect.IsEnabled = true;
                    btnConnect.Visibility = Visibility.Visible;
                });               

            }
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

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerX = e.GetCurrentPoint(cMain).Position.X + 2;
            PointerY = e.GetCurrentPoint(cMain).Position.Y + 2;

            if (Pressed != null)
            {
                Pressed.SetValue(Canvas.TopProperty, PointerY);
                Pressed.SetValue(Canvas.LeftProperty, PointerX);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LastCanvasHeight = cMain.ActualHeight;
            LastCanvasWidth = cMain.ActualWidth;
            NuevaPartida = true;
            cMain.RenderTransform = sT;
            //addMatrix();
            //addPolygon();
        }

        private void setMatrix()
        {
            Matrix = new gameMain[nElems][];

            for (Int32 i = 0; i < Matrix.Length; i++)
            {
                Matrix[i] = new gameMain[nElems];

                for (Int32 j = 0; j < Matrix[i].Length; j++)
                {
                    gameMain gM = new gameMain() { Id =  Guid.NewGuid() };
                    Matrix[i][j] = gM;
                    ElementsInGame.Add(gM);
                }
            }
        }

        private void addMatrix()
        {
            double minSquareY = cMain.ActualHeight / nElems;
            double minSquareX = cMain.ActualWidth / nElems;

            double cSizeY = minSquareY / 2;
            double cSizeX = minSquareX / 2;

            double positionX = 0;
            double positionY = 0;

            for (Int32 i = 0; i < Matrix.Length; i++)
            {
                for (Int32 j = 0; j < Matrix[i].Length; j++)
                {
                    gameMain gM = Matrix[i][j];
                    Rectangle r = new Rectangle();
                    r.PointerPressed += R_PointerPressed;
                    r.Fill = new SolidColorBrush(Colors.Green);
                    r.Height = minSquareY;
                    r.Width = minSquareX;
                    r.SetValue(Canvas.TopProperty, positionY);
                    r.SetValue(Canvas.LeftProperty, positionX);
                    r.Tag = gM.Id;
                    CompositeTransform3D perspective = new CompositeTransform3D();
                    perspective.CenterX = positionX + cSizeX;
                    perspective.CenterY = positionY + cSizeY;                    
                    r.Transform3D = perspective;
                    gM.Perspective = perspective;
                    cMain.Children.Add(r);
                    gM.Sprite = r;
                    positionX += minSquareX;
                }
                positionX = 0;
                positionY += minSquareY;
            }
            cMain.UpdateLayout();
        }

        private void addPolygon()
        {
            double cSizeY = cMain.ActualHeight / 2;
            double cSizeX = cMain.ActualWidth / 2;

            Polygon p = new Polygon();
            p.Points.Add(new Point(cSizeX - 10, cSizeY + 10));
            p.Points.Add(new Point(cSizeX - 10, cSizeY + 110));
            p.Points.Add(new Point(cSizeX - 110, cSizeY + 55));
            p.Fill = new SolidColorBrush(Colors.Red);            
            //p.Transform3D = perspective;
            cMain.Children.Add(p);
        }

        private void R_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("ms-appx:///Images/025Pikachu_OS_anime_4.png", UriKind.Absolute));

            if (sender is Rectangle)
            {
                Rectangle r = (Rectangle)sender;
                Guid id = (Guid)r.Tag;

                foreach (gameMain gM in ElementsInGame)
                {
                    if(gM.Id == id)
                    {
                        if (gM.Sprite is Rectangle)
                        {
                            Rectangle rS = (Rectangle)gM.Sprite;
                            double minSquareY = rS.Height;
                            double minSquareX = rS.Width;                            
                            double positionY = (double)rS.GetValue(Canvas.TopProperty);
                            double positionX = (double)rS.GetValue(Canvas.LeftProperty);

                            img.Height = minSquareY;
                            img.Width = minSquareX;
                            img.SetValue(Canvas.TopProperty, positionY);
                            img.SetValue(Canvas.LeftProperty, positionX);
                            cMain.Children.Remove(gM.Sprite);
                            cMain.Children.Add(img);
                            gM.Sprite = img;
                            break;

                        }
                        else if (gM.Sprite is Image)
                        {
                            //Image r = (Image)gM.Sprite;
                            //r.Height = minSquareY;
                            //r.Width = minSquareX;
                            //r.SetValue(Canvas.TopProperty, positionY);
                            //r.SetValue(Canvas.LeftProperty, positionX);
                            break;
                        }


                    }
                }

            }

            //TextBlock tInit = new TextBlock() { Text = "Hola", Foreground = new SolidColorBrush(Colors.Black), FontSize = 36 };
            //tInit.SetValue(Canvas.TopProperty, 100);
            //tInit.SetValue(Canvas.LeftProperty, 100);

            //cMain.Children.Add(tInit);
            //cMain.UpdateLayout();
        }

        private void main_Click(object sender, RoutedEventArgs e)
        {
            //cMain.Children.Clear();
            //cMain.UpdateLayout();
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == VirtualKey.A)
            {

            }
            else if (e.Key == VirtualKey.D)
            {

            }
            else if (e.Key == VirtualKey.W)
            {

            }
            else if (e.Key == VirtualKey.S)
            {

            }
        }
        
        private void ZoomM_Click(object sender, RoutedEventArgs e)
        {
            sT.ScaleX += 1;
            sT.ScaleY += 1;
        }

        private void Zoomm_Click_m(object sender, RoutedEventArgs e)
        {
            if (sT.ScaleX > 1)
            {
                sT.ScaleX -= 1;
                sT.ScaleY -= 1;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Int16 numA = -1;
            Int16 numB = -1;

            double lastPostionXA = 0;
            double lastPostionYA = 0;
            double lastPostionXB = 0;
            double lastPostionYB = 0;
            Boolean isRotateA = false;
            Boolean isRotateB = false;
            Int16 directionA = 0;
            Int16 directionB = 1;

            for (Int32 i = 0; i < InGameDominoTabs.Count; i++)
            {
                DominoTab dT = InGameDominoTabs[i];
                DominoTabSprite dTS = (DominoTabSprite)dT.Sprite;

                // Para el juego en solitario con las fichas laterales
                if (dTS.RenderTransform is RotateTransform)
                {
                    ((RotateTransform)dTS.RenderTransform).Angle = 0;
                }

                if (i == 0)
                {
                    double xMidPage = cMain.ActualWidth / 2;
                    double yMidPage = cMain.ActualHeight / 2;

                    if (dT.IsDouble)
                    {
                        RotateSprite(dTS, 90);
                        isRotateA = true;
                        isRotateB = true;

                    }

                    numA = dT.Values[0];
                    numB = dT.Values[1];

                    lastPostionXA = xMidPage - (DominoTabWidth / 2);
                    lastPostionYA = dT.IsDouble ? yMidPage - ((DominoTabWidth + ((DominoTabHeigth - DominoTabWidth) / 2)) / 2) : yMidPage - (DominoTabHeigth / 2);

                    lastPostionXB = lastPostionXA;
                    lastPostionYB = lastPostionYA;
                    
                    dT.Sprite.SetValue(Canvas.LeftProperty, lastPostionXA);
                    dT.Sprite.SetValue(Canvas.TopProperty, lastPostionYA);                                        
                }
                else
                {
                    Boolean trueA = dT.Values.Contains(numA);
                    Boolean trueB = dT.Values.Contains(numB);
                    
                    trueA = trueA && trueB ? dT.IsForA : trueA;
                    trueB = trueA && trueB ? dT.IsForB : trueB;
                    

                    if (trueA)
                    {
                        Int16 antDirection = directionA;

                        GetEnabledDirection(lastPostionYA, lastPostionXA, ref directionA);

                        Boolean isDirectionChange = directionA != antDirection;

                        double xPos = 0;
                        double yPos = 0;

                        CalculatePositionByDirection(dT, ref xPos, ref yPos, lastPostionXA, lastPostionYA, isDirectionChange, directionA, isRotateA);
                        RotateIfValueDoubleOrDirection(dT, dTS, numA, directionA, ref isRotateA);

                        numA = dT.Values[0] == numA ? dT.Values[1] : dT.Values[0];

                        lastPostionXA = xPos;
                        lastPostionYA = yPos;

                        dT.Sprite.SetValue(Canvas.LeftProperty, xPos);
                        dT.Sprite.SetValue(Canvas.TopProperty, yPos);
                    }
                    else if (trueB)
                    {
                        Int16 antDirection = directionB;

                        GetEnabledDirection(lastPostionYB, lastPostionXB, ref directionB);

                        Boolean isDirectionChange = antDirection != directionB;

                        double xPos = 0;
                        double yPos = 0;

                        CalculatePositionByDirection(dT, ref xPos, ref yPos, lastPostionXB, lastPostionYB, isDirectionChange, directionB, isRotateB);
                        RotateIfValueDoubleOrDirection(dT, dTS, numB, directionB, ref isRotateB);
                        
                        numB = dT.Values[0] == numB ? dT.Values[1] : dT.Values[0];

                        lastPostionXB = xPos;
                        lastPostionYB = yPos;

                        dT.Sprite.SetValue(Canvas.LeftProperty, xPos);
                        dT.Sprite.SetValue(Canvas.TopProperty, yPos);
                    }
                    else
                    {
                        throw new Exception("Ein? =(");
                    }
                }                                
            }

            LastPosXnumA = lastPostionXA;
            LastPosYnumA = lastPostionYA;
            LastPosXnumB = lastPostionXB;
            LastPosYnumB = lastPostionYB;

            double myPositionY = cMain.ActualHeight - DominoTabHeigth - 5;
            double myPositionX = (cMain.ActualWidth / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;

            double maxPositionY = cMain.ActualHeight - DominoTabHeigth - 5;
            double positionXm = (cMain.ActualWidth / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;

            Int16 playerPostition = -1;
            Int16 player = -1;
            for (Int32 i = 0; i < PlayerDominoTabs.Count; i++)
            {
                DominoTab dT = PlayerDominoTabs[i];
                DominoTabSprite dTS = (DominoTabSprite)dT.Sprite;

                if (dT.Player != PlayerId)
                {
                    if (dT.Player != player)
                    {
                        player = dT.Player;
                        playerPostition = PlayersPositions[player];

                        switch (playerPostition)
                        {
                            case 0:
                                maxPositionY = (cMain.ActualHeight / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                positionXm = 5 + ((DominoTabHeigth / 2) - (DominoTabWidth / 2));
                                break;

                            case 1:
                                maxPositionY = 5;
                                positionXm = (cMain.ActualWidth / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                break;

                            case 2:
                                maxPositionY = (cMain.ActualHeight / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                positionXm = cMain.ActualWidth - 5 - DominoTabHeigth + ((DominoTabHeigth - DominoTabWidth) / 2);
                                break;
                        }
                    }
                                                            
                    dT.SourceX = positionXm;
                    dT.SourceY = maxPositionY;                    
                    dTS.SetValue(Canvas.TopProperty, maxPositionY);
                    dTS.SetValue(Canvas.LeftProperty, positionXm);

                    switch (playerPostition)
                    {
                        case 0:
                            maxPositionY += DominoTabWidth + 2;
                            break;

                        case 1:
                            positionXm += DominoTabWidth + 2;
                            break;

                        case 2:
                            maxPositionY += DominoTabWidth + 2;
                            break;
                    }
                }
                else
                {                    
                    dT.SourceX = myPositionX;
                    dT.SourceY = myPositionY;
                    dTS.SetValue(Canvas.TopProperty, myPositionY);
                    dTS.SetValue(Canvas.LeftProperty, myPositionX);
                    
                    myPositionX += DominoTabWidth + 2;
                }
            }

        }

        private async void SearchRoom_Click(object sender, RoutedEventArgs e)
        {
            cMain.Children.Clear();

            Uri geturi = new Uri("http://apzyxgames.azurewebsites.net/api/DominoLobby");
            using (HttpClient client = new System.Net.Http.HttpClient())
            {
                HttpResponseMessage responseGet = await client.GetAsync(geturi);
                if (responseGet.StatusCode == HttpStatusCode.OK)
                {
                    string response = await responseGet.Content.ReadAsStringAsync();

                    List<DominoLobbyModel> rooms = JsonConvert.DeserializeObject<List<DominoLobbyModel>>(response);

                    double xPosMax = cMain.ActualWidth;

                    double xPos = 0;
                    double yPos = 0;
                    foreach (DominoLobbyModel room in rooms)
                    {
                        DominoRoomSprite dRS = new DominoRoomSprite();
                        dRS.DataContext = room;
                        dRS.SetValue(Canvas.TopProperty, yPos);
                        dRS.SetValue(Canvas.LeftProperty, xPos);

                        cMain.Children.Add(dRS);

                        xPos += 210;

                        if(xPos + 200 > xPosMax)
                        {
                            xPos = 0;
                            yPos += 210;
                        }
                    }
                }
            }
        }

        private void NewDominoGame_Click(object sender, RoutedEventArgs e)
        {
            if (NuevaPartida)
            {
                SelectMaxPoints.Visibility = Visibility.Visible;
            }
            else
            {
                ShowMessage("Garantía de ejecución",
                    "Para empezar una nueva partida debe esperar a que llegue su turno para garantizar la ejecución de todos los procesos pendientes =)",
                    cmd => { });
            }
        }

        private void setDominoGame(Start st)
        {
            // Jugador al que le toca mover
            PlayerRound = st.PlayerRound;

            for (Int16 i = 0; i < 4; i++)
            {
                if (st.Users[i] == u.Id)
                {
                    PlayerId = i;
                }
            }

            // Mis fichas de Dominó
            PlayerDominoTabs = st.MyTabs.Select(x => new DominoTab() { Id = x.Id, IdRoom = x.IdRoom, Player = x.Player, Values = x.Values }).ToList();
            EmptyTab = new DominoTabServ() { IdRoom = PlayerDominoTabs[0].IdRoom, Player = PlayerId, Turn = true };
            
            // MIS FICHAS

            // Posicion menor a la mitad de X de la "pantalla" menos la mitad de las fichas donde empezaremos a pintar las fichas
            // (contado el margen entre ellas que será de 5)

            double myPositionY = cMain.ActualHeight - DominoTabHeigth - 5;
            double myPositionX = (cMain.ActualWidth / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;

            double maxPositionY = cMain.ActualHeight - DominoTabHeigth - 5;
            double positionXm = (cMain.ActualWidth / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;

            PlayersPositions[PlayerId] = 3;
            PlayersPositions[PlayerId == 0 ? 1 : PlayerId == 1 ? 0 : PlayerId == 2 ? 3 : PlayerId == 3 ? 2 : -1] = 2;
            PlayersPositions[PlayerId == 0 ? 2 : PlayerId == 1 ? 3 : PlayerId == 2 ? 0 : PlayerId == 3 ? 1 : -1] = 1;
            PlayersPositions[PlayerId == 0 ? 3 : PlayerId == 1 ? 2 : PlayerId == 2 ? 1 : PlayerId == 3 ? 0 : -1] = 0;

            // FICHAS JUGADORES
            Int32 playerPostition = -1;
            Int16 player = -1;
            foreach (DominoTab dT in PlayerDominoTabs)
            {
                if(dT.Player == PlayerId)
                {
                    DominoTabSprite r = new DominoTabSprite();
                    //r.Fill = new SolidColorBrush(Colors.Red);
                    r.DataContext = dT;
                    r.Tag = dT;
                    r.Height = DominoTabHeigth;
                    r.Width = DominoTabWidth;
                    r.PointerPressed += R_PointerPressedDominoTab;
                    r.PointerReleased += R_PointerReleasedDominoTab;
                    dT.SourceX = myPositionX;
                    dT.SourceY = myPositionY;
                    r.SetValue(Canvas.TopProperty, myPositionY);
                    r.SetValue(Canvas.LeftProperty, myPositionX);
                    dT.Sprite = r;
                    cMain.Children.Add(r);

                    myPositionX += DominoTabWidth + 2;
                }
            }

            playerPostition = -1;
            player = -1;            
            for (Int16 i = 0; i < 4; i++)
            {
                if (i != PlayerId)
                {
                    if (i != player)
                    {
                        player = i;
                        playerPostition = PlayersPositions[player];

                        switch (playerPostition)
                        {
                            case 0:
                                maxPositionY = (cMain.ActualHeight / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                positionXm = 5 + DominoTabHeigth;
                                break;

                            case 1:
                                maxPositionY = 5;
                                positionXm = (cMain.ActualWidth / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                break;

                            case 2:
                                maxPositionY = (cMain.ActualHeight / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                positionXm = cMain.ActualWidth - 5;
                                break;
                        }
                    }

                    // Recorremos 7 fichas por cada jugador (Estas seran las fichas de los contrincantes)
                    for (Int16 j = 0; j < 7; j++)
                    {
                        DominoTab dT = new DominoTab();
                        DominoTabSprite r = new DominoTabSprite();
                        dT.IsBlack = true;
                        r.DataContext = dT;
                        r.Tag = dT;
                        dT.Player = player;
                        dT.SourceX = positionXm;
                        dT.SourceY = maxPositionY;
                        dT.Sprite = r;
                        r.SetValue(Canvas.TopProperty, maxPositionY);
                        r.SetValue(Canvas.LeftProperty, positionXm);
                        OthersDominoTabs.Add(dT);

                        switch (playerPostition)
                        {
                            case 0:
                                r.Rotation = 90;
                                dT.IsRotate = true;
                                cMain.Children.Add(r);

                                maxPositionY += DominoTabWidth + 2;
                                break;

                            case 1:
                                r.Height = DominoTabHeigth;
                                r.Width = DominoTabWidth;
                                cMain.Children.Add(r);

                                positionXm += DominoTabWidth + 2;
                                break;

                            case 2:
                                r.Rotation = 90;
                                dT.IsRotate = true;
                                cMain.Children.Add(r);

                                maxPositionY += DominoTabWidth + 2;
                                break;
                        } 
                    }
                }
            }


            Image img = new Image();
            ImagePlayerRound = img;
            img.Source = new BitmapImage(new Uri("ms-appx:///Images/R.gif", UriKind.Absolute));
            img.Height = 100;
            img.Width = 100;
            //img.PointerReleased += Img_PointerReleased;


            for (Int16 j = 0; j < 4; j++)
            {
                if (j == PlayerRound)
                    //////if (j == PlayerRound)
                {
                    double rPositionY = -1;
                    double rPositionX = -1;
                    //////Int16 position = PlayersPositions[PlayerRound];
                    Int16 position = PlayersPositions[PlayerRound];
                    switch (position)
                    {
                        case 0:
                            rPositionY = (cMain.ActualHeight / 2) - 50;
                            rPositionX = DominoTabHeigth + 10;
                            break;

                        case 1:
                            rPositionY = DominoTabHeigth + 10;
                            rPositionX = (cMain.ActualWidth / 2) - 50;
                            break;

                        case 2:
                            rPositionY = (cMain.ActualHeight / 2) - 50;
                            rPositionX = cMain.ActualWidth - 100 - DominoTabHeigth - 10;
                            break;
                        case 3:
                            rPositionY = (cMain.ActualHeight) - 100 - DominoTabHeigth - 10;
                            rPositionX = (cMain.ActualWidth / 2) - 50;
                            break;

                    }
                    img.SetValue(Canvas.TopProperty, rPositionY);
                    img.SetValue(Canvas.LeftProperty, rPositionX);
                    break;
                }
            }

            cMain.Children.Add(img);
            cMain.UpdateLayout();
        }

        private void Img_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (Pressed != null)
            {
                Image imgR = (Image)sender;
                cMain.Children.Remove(imgR);

            
                DominoTab dT = (DominoTab)Pressed.Tag;
                DominoTabSprite dTS = (DominoTabSprite)dT.Sprite;
                dT.Disabled = true;

                PlayerDominoTabs.Remove(dT);
                InGameDominoTabs.Add(dT);

                double x = (double)dT.Sprite.GetValue(Canvas.LeftProperty);
                double y = (double)dT.Sprite.GetValue(Canvas.TopProperty);
                

                Image img = new Image();
                img.Source = new BitmapImage(new Uri("ms-appx:///Images/R.gif", UriKind.Absolute));
                img.Height = 100;
                img.Width = 100;
                img.PointerReleased += Img_PointerReleased;
                img.SetValue(Canvas.TopProperty, dTS.Height == DominoTabHeigth ? y - 100  : y - (100 / 2) - (DominoTabWidth / 2));
                img.SetValue(Canvas.LeftProperty, dTS.Height == DominoTabHeigth ? x - (100 /2) - (DominoTabWidth / 2) : x - 100);
                cMain.Children.Add(img);

                Image img2 = new Image();
                img2.Source = new BitmapImage(new Uri("ms-appx:///Images/R.gif", UriKind.Absolute));
                img2.Height = 100;
                img2.Width = 100;
                img2.PointerReleased += Img_PointerReleased;
                img2.SetValue(Canvas.TopProperty, dTS.Height == DominoTabHeigth ? y + DominoTabHeigth : y - (100 / 2) - (DominoTabWidth / 2));
                img2.SetValue(Canvas.LeftProperty, dTS.Height == DominoTabHeigth ? x - (100 / 2) - (DominoTabWidth / 2) : x + DominoTabHeigth);
                cMain.Children.Add(img2);


                Pressed = null;
            }
        }

        private void R_PointerReleasedDominoTab(object sender, PointerRoutedEventArgs e)
        {
            if (Pressed != null)
            {
                DominoTab dT = (DominoTab)Pressed.Tag;

                if (dT.Disabled)
                {
                    DropDominoTab(dT);
                }
            }
        }

        private void R_PointerPressedDominoTab(object sender, PointerRoutedEventArgs e)
        {
            DominoTab dT = (DominoTab)((DominoTabSprite)sender).Tag;

            if (!dT.Disabled)
            {
                if (dT.Player == PlayerRound)
                {

                    if (Pressed == null)
                    {
                        Pressed = (DominoTabSprite)sender; ;
                        Pressed.SetValue(Canvas.TopProperty, PointerY);
                        Pressed.SetValue(Canvas.LeftProperty, PointerX);
                    }
                    else
                    {
                        dT = (DominoTab)Pressed.Tag;

                        Pressed.SetValue(Canvas.TopProperty, dT.SourceY);
                        Pressed.SetValue(Canvas.LeftProperty, dT.SourceX);

                        Pressed = null;
                    }
                }
            }
        }

        private void paintNewDominoTab(DominoTabServ dTServer)
        {
            DominoTab dT = new DominoTab();
            dT.Id = dTServer.Id;
            dT.IdRoom = dTServer.IdRoom;
            dT.Player = dTServer.Player;
            dT.Values = dTServer.Values;
            dT.IsForA = dTServer.IsForA;
            dT.IsForB = dTServer.IsForB;            
            InGameDominoTabs.Add(dT);
            DominoTabSprite dTS = new DominoTabSprite();
            dTS.DataContext = dT;
            dT.Sprite = dTS;

            if (InGameDominoTabs.Count == 0)
            {
                double xMidPage = cMain.ActualWidth / 2;
                double yMidPage = cMain.ActualHeight / 2;

                if (dT.IsDouble)
                {
                    RotateSprite(dTS, 90);
                    IsRotateA = true;
                    IsRotateB = true;

                }

                numDominoTabA = dT.Values[0];
                numDominoTabB = dT.Values[1];

                LastPosXnumA = xMidPage - (DominoTabWidth / 2);
                LastPosYnumA = dT.IsDouble ? yMidPage - (DominoTabWidth / 2) : yMidPage - (DominoTabHeigth / 2);

                LastPosXnumB = xMidPage - (DominoTabWidth / 2);
                LastPosYnumB = dT.IsDouble ? yMidPage - (DominoTabWidth / 2) : yMidPage - (DominoTabHeigth / 2);

                dT.Disabled = true;

                PlayerDominoTabs.Remove(dT);
                InGameDominoTabs.Add(dT);

                dT.Sprite.SetValue(Canvas.LeftProperty, LastPosXnumA);
                dT.Sprite.SetValue(Canvas.TopProperty, LastPosYnumA);

                MovePlayer();
            }
            else
            {
                Boolean trueA = dT.Values.Contains(numDominoTabA);
                Boolean trueB = dT.Values.Contains(numDominoTabB);

                trueA = trueA && trueB? dT.IsForA : trueA;
                trueB = trueA && trueB? dT.IsForB : trueB;

                if (trueA)
                {
                    Int16 antDirection = DireccionA;

                    GetEnabledDirection(LastPosYnumA, LastPosXnumA, ref DireccionA);

                    Boolean isDirectionChange = DireccionA != antDirection;

                    double xPos = 0;
                    double yPos = 0;
                    CalculatePositionByDirection(dT, ref xPos, ref yPos, LastPosXnumA, LastPosYnumA, isDirectionChange, DireccionA, IsRotateA);

                    RotateIfValueDoubleOrDirection(dT, dTS, numDominoTabA, DireccionA, ref IsRotateA);

                    numDominoTabA = dT.Values[0] == numDominoTabA ? dT.Values[1] : dT.Values[0];

                    LastPosXnumA = xPos;
                    LastPosYnumA = yPos;

                    dT.Sprite.SetValue(Canvas.LeftProperty, xPos);
                    dT.Sprite.SetValue(Canvas.TopProperty, yPos);

                    dT.Disabled = true;
                    MovePlayer();
                }
                else if (trueB)
                {
                    Int16 antDirection = DireccionB;

                    GetEnabledDirection(LastPosYnumB, LastPosXnumB, ref DireccionB);

                    Boolean isDirectionChange = antDirection != DireccionB;

                    double xPos = 0;
                    double yPos = 0;
                    CalculatePositionByDirection(dT, ref xPos, ref yPos, LastPosXnumB, LastPosYnumB, isDirectionChange, DireccionB, IsRotateB);

                    RotateIfValueDoubleOrDirection(dT, dTS, numDominoTabB, DireccionB, ref IsRotateB);

                    numDominoTabB = dT.Values[0] == numDominoTabB ? dT.Values[1] : dT.Values[0];

                    LastPosXnumB = xPos;
                    LastPosYnumB = yPos;

                    dT.Sprite.SetValue(Canvas.LeftProperty, xPos);
                    dT.Sprite.SetValue(Canvas.TopProperty, yPos);

                    dT.Disabled = true;
                    MovePlayer();
                }
                else
                {
                    dT.Sprite.SetValue(Canvas.LeftProperty, dT.SourceX);
                    dT.Sprite.SetValue(Canvas.TopProperty, dT.SourceY);

                    if (dT.IsRotate)
                    {
                        RotateSprite(dTS, 90);
                    }
                }
            }

            cMain.Children.Add(dTS);
                
        }

        private void GetEnabledDirection(double lastY, double lastX, ref Int16 direction)
        {
            // Tener en cuenta el ancho de la pantalla para dibujar en dirección horizontal o vertical (X)
            Boolean abajo = lastY + DominoTabHeigth + DominoTabHeigth < cMain.ActualHeight - 5 - DominoTabHeigth - ImgMovePlayerHeigth;
            Boolean izquierda = lastX - DominoTabHeigth > 5 + DominoTabHeigth + ImgMovePlayerWidth;
            Boolean arriba = lastY - DominoTabHeigth > 5 + DominoTabHeigth + ImgMovePlayerHeigth;
            Boolean derecha = lastX + DominoTabHeigth + DominoTabHeigth < cMain.ActualWidth - 5 - DominoTabHeigth - ImgMovePlayerWidth;

            if (direction == 1 && !abajo)
            {
                direction = 3;
                if (!izquierda)
                {
                    direction = 0;
                    if (!arriba)
                    {
                        direction = 2;
                    }
                }
            }
            else if (direction == 3 && !izquierda)
            {
                direction = 0;
                if (!arriba)
                {
                    direction = 2;
                    if (!derecha)
                    {
                        direction = 1;
                    }
                }
            }
            else if (direction == 0 && !arriba)
            {
                direction = 2;
                if (!derecha)
                {
                    direction = 1;
                    if (!abajo)
                    {
                        direction = 3;
                    }
                }
            }
            else if (direction == 2 && !derecha)
            {
                direction = 1;
                if (!abajo)
                {
                    direction = 3;
                    if (!izquierda)
                    {
                        direction = 0;
                    }
                }
            }
        }

        private void RotateSprite(DominoTabSprite dTS, double angle)
        {
            if (!(dTS.RenderTransform is RotateTransform))
            {
                dTS.RenderTransform = new RotateTransform() { Angle = angle };
            }
            else
            {
                ((RotateTransform)dTS.RenderTransform).Angle = angle;
            }

        }

        private void DropDominoTab(DominoTab dT)
        {
            DominoTabServ dTClient = new DominoTabServ();
            dTClient.Id = dT.Id;
            dTClient.IdRoom = dT.IdRoom;
            dTClient.Player = dT.Player;
            dTClient.Values = dT.Values;

            DominoTabSprite dTS = (DominoTabSprite)dT.Sprite;

            // Para el juego en solitario con las fichas laterales
            if (dTS.RenderTransform is RotateTransform)
            {
                ((RotateTransform)dTS.RenderTransform).Angle = 0;
            }


            if (InGameDominoTabs.Count == 0)
            {
                double xMidPage = cMain.ActualWidth / 2;
                double yMidPage = cMain.ActualHeight / 2;

                if (dT.IsDouble)
                {
                    RotateSprite(dTS, 90);
                    IsRotateA = true;
                    IsRotateB = true;

                }

                numDominoTabA = dT.Values[0];
                numDominoTabB = dT.Values[1];

                LastPosXnumA = xMidPage - (DominoTabWidth / 2);
                LastPosYnumA = dT.IsDouble ? yMidPage - ((DominoTabWidth + ((DominoTabHeigth - DominoTabWidth) / 2)) / 2) : yMidPage - (DominoTabHeigth / 2);

                LastPosXnumB = LastPosXnumA;
                LastPosYnumB = LastPosYnumA;

                dT.Disabled = true;
                dT.IsBlack = false;

                PlayerDominoTabs.Remove(dT);
                InGameDominoTabs.Add(dT);

                dT.Sprite.SetValue(Canvas.LeftProperty, LastPosXnumA);
                dT.Sprite.SetValue(Canvas.TopProperty, LastPosYnumA);

                MovePlayer();
            }
            else
            {
                Boolean trueA = dT.Values.Contains(numDominoTabA);
                Boolean trueB = dT.Values.Contains(numDominoTabB);


                // Comprobar la ficha que está más cerca para asociar con ese valor en caso de coincidir ambos valores
                if (trueA && trueB)
                {                    
                    double diferenciaXRespectoA = Math.Abs(LastPosXnumA - (double)dTS.GetValue(Canvas.LeftProperty));
                    double diferenciaYRespectoA = Math.Abs(LastPosYnumA - (double)dTS.GetValue(Canvas.TopProperty));
                    double diferenciaXRespectoB = Math.Abs(LastPosXnumB - (double)dTS.GetValue(Canvas.LeftProperty));
                    double diferenciaYRespectoB = Math.Abs(LastPosYnumB - (double)dTS.GetValue(Canvas.TopProperty));

                    if (dT.IsForA || dT.IsForB)
                    {
                        trueA = trueA && trueB ? dT.IsForA : trueA;
                        trueB = trueA && trueB ? dT.IsForB : trueB;
                    }
                    else
                    {
                        if (diferenciaXRespectoA + diferenciaYRespectoA < diferenciaXRespectoB + diferenciaYRespectoB)
                        {
                            trueA = true;
                            trueB = false;
                            dT.IsForA = true;
                            dTClient.IsForA = true;
                        }
                        else
                        {
                            trueA = false;
                            trueB = true;
                            dT.IsForB = true;
                            dTClient.IsForB = true;
                        }
                    }  
                }

                if (trueA)
                {
                    Int16 antDirection = DireccionA;
                                        
                    GetEnabledDirection(LastPosYnumA, LastPosXnumA, ref DireccionA);

                    Boolean isDirectionChange = DireccionA != antDirection;

                    double xPos = 0;
                    double yPos = 0;

                    CalculatePositionByDirection(dT, ref xPos, ref yPos, LastPosXnumA, LastPosYnumA, isDirectionChange, DireccionA, IsRotateA);
                    RotateIfValueDoubleOrDirection(dT, dTS, numDominoTabA, DireccionA, ref IsRotateA);
                                        
                    numDominoTabA = dT.Values[0] == numDominoTabA ? dT.Values[1] : dT.Values[0];

                    LastPosXnumA = xPos;
                    LastPosYnumA = yPos;

                    dT.Sprite.SetValue(Canvas.LeftProperty, xPos);
                    dT.Sprite.SetValue(Canvas.TopProperty, yPos);

                    PlayerDominoTabs.Remove(dT);
                    InGameDominoTabs.Add(dT);
                    
                    dT.IsBlack = false;
                    dT.Disabled = true;
                    MovePlayer();
                }
                else if (trueB)
                {
                    Int16 antDirection = DireccionB;

                    GetEnabledDirection(LastPosYnumB, LastPosXnumB, ref DireccionB);

                    Boolean isDirectionChange = antDirection != DireccionB ;

                    double xPos = 0;
                    double yPos = 0;

                    CalculatePositionByDirection(dT, ref xPos, ref yPos, LastPosXnumB, LastPosYnumB, isDirectionChange, DireccionB, IsRotateB);


                    RotateIfValueDoubleOrDirection(dT, dTS, numDominoTabB, DireccionB, ref IsRotateB);

                    numDominoTabB = dT.Values[0] == numDominoTabB ? dT.Values[1] : dT.Values[0];

                    LastPosXnumB = xPos;
                    LastPosYnumB = yPos;

                    dT.Sprite.SetValue(Canvas.LeftProperty, xPos);
                    dT.Sprite.SetValue(Canvas.TopProperty, yPos);

                    PlayerDominoTabs.Remove(dT);
                    InGameDominoTabs.Add(dT);

                    dT.IsBlack = false;
                    dT.Disabled = true;
                    MovePlayer();
                }
                else
                {
                    dT.Sprite.SetValue(Canvas.LeftProperty, dT.SourceX);
                    dT.Sprite.SetValue(Canvas.TopProperty, dT.SourceY);

                    if (dT.IsRotate)
                    {
                        RotateSprite(dTS, 90);
                    }
                }
            }

            sendToServer(dTClient);

            Pressed = null;
        }

        private void CalculatePositionByDirection(DominoTab dT, ref double xPos, ref double yPos, double lastXPos, double lastYPos, Boolean isDirectionChange, Int16 direction, Boolean isRotate)
        {
            if (direction == 1)
            {
                xPos = lastXPos + (isDirectionChange ? (DominoTabWidth / 2) : 0);
                yPos = lastYPos
                    //+ (dT.IsDouble ? (DominoTabHeigth - DominoTabWidth + (DominoTabHeigth / 2)) : DominoTabHeigth)
                    + (dT.IsDouble ? DominoTabWidth + ((DominoTabHeigth - DominoTabWidth) / 2) : DominoTabHeigth)
                    - (isRotate ? ((DominoTabHeigth / 2) - (DominoTabWidth / 2)) : 0);
            }
            else if (direction == 2)
            {
                xPos = lastXPos
                    //+ (dT.IsDouble ? (DominoTabHeigth - DominoTabWidth + (DominoTabHeigth / 2)) : DominoTabHeigth)
                    + (dT.IsDouble ? DominoTabWidth + ((DominoTabHeigth - DominoTabWidth) / 2) : DominoTabHeigth)
                    - (!isRotate ? ((DominoTabHeigth / 2) - (DominoTabWidth / 2)) : 0);
                yPos = lastYPos - (isDirectionChange ? (DominoTabWidth / 2) : 0);
            }
            else if (direction == 0)
            {
                xPos = lastXPos - (isDirectionChange ? (DominoTabWidth / 2) : 0);
                yPos = lastYPos
                    //- (dT.IsDouble ? (DominoTabHeigth - DominoTabWidth + (DominoTabHeigth / 2)) : DominoTabHeigth)
                    - (dT.IsDouble ? DominoTabWidth + ((DominoTabHeigth - DominoTabWidth) / 2) : DominoTabHeigth)
                    + (isRotate ? ((DominoTabHeigth / 2) - (DominoTabWidth / 2)) : 0);
            }
            else if (direction == 3)
            {
                xPos = lastXPos
                    //- (dT.IsDouble ? (DominoTabHeigth - DominoTabWidth + (DominoTabHeigth / 2)) : DominoTabHeigth)
                    - (dT.IsDouble ? DominoTabWidth + ((DominoTabHeigth - DominoTabWidth) / 2) : DominoTabHeigth)
                    + (!isRotate ? ((DominoTabHeigth / 2) - (DominoTabWidth / 2)) : 0);
                yPos = lastYPos + (isDirectionChange ? (DominoTabWidth / 2) : 0);
            }
            else
            {
                throw new Exception("La ficha no tiene dirección =(");
            }
        }

        private void RotateIfValueDoubleOrDirection(DominoTab dT, DominoTabSprite dTS, Int16 AorB, Int16 direction, ref Boolean isRotate)
        {
            if (direction == 0)
            {
                if (dT.Values[1] == AorB)
                {
                    if (dT.IsDouble)
                    {
                        isRotate = true;
                        RotateSprite(dTS, 90);
                    }
                    else
                    {
                        isRotate = false;
                    }
                }
                else
                {
                    isRotate = false;
                    RotateSprite(dTS, 180);
                }
            }
            else if (direction == 1)
            {
                if (dT.Values[0] == AorB)
                {
                    if (dT.IsDouble)
                    {
                        isRotate = true;
                        RotateSprite(dTS, 90);
                    }
                    else
                    {
                        isRotate = false;
                    }
                }
                else
                {
                    isRotate = false;
                    RotateSprite(dTS, 180);
                }
            }
            else if (direction == 3)
            {
                if (dT.Values[0] == AorB)
                {
                    if (!dT.IsDouble)
                    {
                        isRotate = true;
                        RotateSprite(dTS, 90);
                    }
                    else
                    {
                        isRotate = false;
                    }
                }
                else
                {
                    if (!dT.IsDouble)
                    {
                        isRotate = true;
                        RotateSprite(dTS, 270);
                    }
                    else
                    {
                        isRotate = false;
                    }
                }
            }
            else if (direction == 2)
            {
                if (dT.Values[1] == AorB)
                {
                    if (!dT.IsDouble)
                    {
                        isRotate = true;
                        RotateSprite(dTS, 90);
                    }
                    else
                    {
                        isRotate = false;
                    }
                }
                else
                {
                    if (!dT.IsDouble)
                    {
                        isRotate = true;
                        RotateSprite(dTS, 270);
                    }
                    else
                    {
                        isRotate = false;
                    }
                }
            }
        }

        private void Page_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (Pressed != null)
            {
                DominoTab dT = (DominoTab)Pressed.Tag;

                DropDominoTab(dT);
            }
        }

        private Boolean RotateS(Boolean isRotate, DominoTabSprite s)
        {
            if(isRotate)
            {
                s.Rotation = 0;
                return false;
            }
            else
            {
                s.Rotation = 90;
                return true;
            }
        }

        private void NotTabsInTurn()
        {
            Turn = new Button();
            Turn.Content = "Paso";
            Turn.Click += Turn_Click;
                        
            for (Int16 j = 0; j < 4; j++)
            {
                double rPositionY = (cMain.ActualHeight) - ImgMovePlayerHeigth - DominoTabHeigth - 10;
                double rPositionX = (cMain.ActualWidth / 2) - (ImgMovePlayerWidth / 2);

                Turn.SetValue(Canvas.TopProperty, rPositionY);
                Turn.SetValue(Canvas.LeftProperty, rPositionX);
                break;

            }
            cMain.Children.Add(Turn);

        }

        private void Turn_Click(object sender, RoutedEventArgs e)
        {
            cMain.Children.Remove((Button)sender);
            sendToServer(EmptyTab);
            MovePlayer();
        }

        private async void MovePlayer()
        {
            Int16 antPlayerRound = PlayerRound;

            Int16 position = PlayersPositions[PlayerRound];
            if (position == 0)
            {
                position = 3;
            }
            else
            {
                position--;
            }

            for (Int16 i = 0; i < 4; i++)
            {
                if (PlayersPositions[i] == position)
                {
                    PlayerRound = i;
                    cMain.Children.Remove(ImagePlayerRound);

                    Image img = new Image();
                    ImagePlayerRound = img;
                    img.Source = new BitmapImage(new Uri("ms-appx:///Images/R.gif", UriKind.Absolute));
                    img.Height = ImgMovePlayerHeigth;
                    img.Width = ImgMovePlayerWidth;

                    for (Int16 j = 0; j < 4; j++)
                    {
                        if (j == PlayerRound)
                        {
                            double rPositionY = -1;
                            double rPositionX = -1;

                            switch (position)
                            {
                                case 0:
                                    rPositionY = (cMain.ActualHeight / 2) - (ImgMovePlayerHeigth / 2);
                                    rPositionX = DominoTabHeigth + 10;
                                    break;

                                case 1:
                                    rPositionY = DominoTabHeigth + 10;
                                    rPositionX = (cMain.ActualWidth / 2) - (ImgMovePlayerWidth / 2);
                                    break;

                                case 2:
                                    rPositionY = (cMain.ActualHeight / 2) - (ImgMovePlayerHeigth / 2);
                                    rPositionX = cMain.ActualWidth - ImgMovePlayerWidth - DominoTabHeigth - 10;
                                    break;
                                case 3:
                                    rPositionY = (cMain.ActualHeight) - ImgMovePlayerHeigth - DominoTabHeigth - 10;
                                    rPositionX = (cMain.ActualWidth / 2) - (ImgMovePlayerWidth / 2);
                                    break;

                            }
                            img.SetValue(Canvas.TopProperty, rPositionY);
                            img.SetValue(Canvas.LeftProperty, rPositionX);
                            break;
                        }
                    }
                    cMain.Children.Add(img);
                }
            }

            // Si estamos en nuestra posición comprobamos si vamos o no a pasar
            if (position == 3)
            {
                // Recorremos todas sus fichas
                Boolean nextTurn = true;
                foreach (DominoTab dtSP in PlayerDominoTabs.Where(x => x.Player == PlayerRound))
                {
                    if (dtSP.Values.Contains(numDominoTabA) || dtSP.Values.Contains(numDominoTabB) || numDominoTabA == -1)
                    {
                        nextTurn = false;
                        break;
                    }
                }

                if (nextTurn)
                {
                    NotTabsInTurn();
                }
            }

            if (IsMultiplayer)
            {
                DominoTab dTMultiPlayerEmpty = OthersDominoTabs.Where(x => x.Player == antPlayerRound).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                if (dTMultiPlayerEmpty != null)
                {
                    OthersDominoTabs.Remove(dTMultiPlayerEmpty);
                    cMain.Children.Remove(dTMultiPlayerEmpty.Sprite);
                }
            }
            else
            {
                if(!checkEnd(antPlayerRound))
                {
                    ckeckBotPlayer(position, false);
                }
            }
        }

        private Boolean checkEnd(Int16 antPlayerRound)
        {
            List<DominoTab> playerTabs = PlayerDominoTabs.Where(x => x.Player == antPlayerRound).ToList();

            if (playerTabs != null && playerTabs.Count != 0)
            {
                List<DominoTab> allPlayerTabs = PlayerDominoTabs.ToList();

                Boolean endRound = true;                
                foreach (DominoTab dtSP in allPlayerTabs)
                {
                    if (dtSP.Values.Contains(numDominoTabA) || dtSP.Values.Contains(numDominoTabB) || numDominoTabA == -1)
                    {
                        endRound = false;
                    }
                }

                if (endRound)
                {
                    Int16 partner = (Int16)(PlayerId == 0 ? 2 : PlayerId == 1 ? 3 : PlayerId == 2 ? 0 : PlayerId == 3 ? 1 : -1);

                    Int16 countPartners = (Int16)allPlayerTabs
                        .Where(x => x.Player == PlayerId || x.Player == partner).Sum(x => (x.Values[0] + x.Values[1]));

                    Int16 countOppo = (Int16)allPlayerTabs
                        .Where(x => x.Player != PlayerId && x.Player != partner).Sum(x => (x.Values[0] + x.Values[1]));
                                        
                    if (countPartners < countOppo)
                    {                        
                        PuntMyTeam += (Int16)(countPartners + countOppo);
                        myScore.Text = "Mi Equipo: " + PuntMyTeam.ToString();
                    }
                    else if(countPartners > countOppo)
                    {
                        PuntOtherTeam += (Int16)(countPartners + countOppo);
                        otherScore.Text = "Equipo 2:  " + PuntOtherTeam.ToString();
                    }

                    // Corprobar si hemos llegado al final de la puntuación total para finalizar el juego
                    
                    // Ganamos nosotros (Partida completa)
                    if (PuntMyTeam >= FinalPunt)
                    {
                        END(true);
                    }
                    // Ganan ellos (Partida completa)
                    else if(PuntOtherTeam >= FinalPunt)
                    {
                        END(false);
                    }
                    // Siguiente ronda en caso de que nadie llegue a los puntos
                    else
                    {
                        EndRound();                        
                    }

                    return true;
                }

                return false;
            }
            else
            {
                Int16 partner = (Int16)(PlayerId == 0 ? 2 : PlayerId == 1 ? 3 : PlayerId == 2 ? 0 : PlayerId == 3 ? 1 : -1);

                Int16 countPunts = (Int16)PlayerDominoTabs.Sum(x => (x.Values[0] + x.Values[1]));

                if (antPlayerRound == PlayerId || antPlayerRound == partner)
                {
                    PuntMyTeam += countPunts;
                    myScore.Text =  "Mi Equipo: " + PuntMyTeam.ToString();
                }
                else
                {
                    PuntOtherTeam += countPunts;
                    otherScore.Text = "Equipo 2:  " + PuntOtherTeam.ToString();
                }

                // Corprobar si hemos llegado al final de la puntuación total para finalizar el juego

                // Ganamos nosotros (Partida completa)
                if (PuntMyTeam >= FinalPunt)
                {
                    END(true);
                }
                // Ganan ellos (Partida completa)
                else if (PuntOtherTeam >= FinalPunt)
                {
                    END(false);
                }
                // Siguiente ronda en caso de que nadie llegue a los puntos
                else
                {
                    EndRound();
                }
                                
                return true;
            }
        }

        private void END(Boolean imTheWinner)
        {
            cMain.Children.Remove(ImagePlayerRound);

            foreach (DominoTab dT in PlayerDominoTabs)
            {
                dT.IsBlack = false;
                dT.Disabled = true;
            }

            String result = String.Empty;

            if (imTheWinner)
            {
                result = "GANASTE =)";
            }
            else
            {
                result = "Más suerte la próxima vez =)";
            }

            //PlayersPositions = null;
            NuevaPartida = true;

            ShowMessage("Finalización de partida",
                    result + "\r\n\r\n ¿Desea jugar otra?",
                    cmd => { NewDominoGame_Click(null, null); });
        }

        private async void ckeckBotPlayer(Int16 position, Boolean isFirst)
        {
            // Lógica IA de juego en un solo jugador

            List<DominoTab> playerTabs = PlayerDominoTabs.Where(x => x.Player == PlayerRound).ToList();

            if (playerTabs != null && playerTabs.Count != 0)
            {
                if (position != 3)
                {
                    // IA DEL JUGADOR BOT

                    // Comprobamos las ficha en la mesa con las mias por si están todas las que son y son todas las que están
                    Dictionary<Int16, List<DominoTab>> inGameAndMyTabs = new Dictionary<Int16, List<DominoTab>>();
                    // Inicializamos las listas asociadas para todos los números en juego más los mios
                    for (Int16 i = 0; i < 7; i++) { inGameAndMyTabs.Add(i, new List<DominoTab>()); }

                    foreach (DominoTab dtInGame in InGameDominoTabs)
                    {
                        if (dtInGame.Values.Contains(numDominoTabA))
                        {
                            inGameAndMyTabs[numDominoTabA].Add(dtInGame);

                        }

                        if (dtInGame.Values.Contains(numDominoTabB))
                        {
                            inGameAndMyTabs[numDominoTabB].Add(dtInGame);

                        }
                    }

                    foreach (DominoTab dtSP in playerTabs)
                    {
                        // Para la lógica de encontrar otra ficha
                        if (dtSP.Values.Contains(numDominoTabA))
                        {
                            inGameAndMyTabs[numDominoTabA].Add(dtSP);

                        }
                        if (dtSP.Values.Contains(numDominoTabB))
                        {
                            inGameAndMyTabs[numDominoTabB].Add(dtSP);

                        }
                    }

                    List<Int16> completStrike = new List<Int16>();
                    for (Int16 i = 0; i < 7; i++)
                    {
                        if (inGameAndMyTabs[i].Count == 7)
                        {
                            completStrike.Add(i);
                        }
                    }

                    // Recorremos todas sus fichas
                    List<DominoTab> possibleTabs = new List<DominoTab>();
                    Boolean nextTurn = true;
                    foreach (DominoTab dtSP in playerTabs)
                    {
                        if ((dtSP.Values.Contains(numDominoTabA) && !completStrike.Contains(numDominoTabA)) || 
                            (dtSP.Values.Contains(numDominoTabB) && !completStrike.Contains(numDominoTabB)) || numDominoTabA == -1)
                        {
                            Boolean trueA = dtSP.Values.Contains(numDominoTabA);
                            Boolean trueB = dtSP.Values.Contains(numDominoTabB);
                            if (trueA && trueB && numDominoTabA != numDominoTabB && completStrike.Contains(numDominoTabA) != completStrike.Contains(numDominoTabB))
                            {
                                dtSP.IsForA = !completStrike.Contains(numDominoTabA);
                                dtSP.IsForB = !completStrike.Contains(numDominoTabB);
                            }
                            nextTurn = false;
                            possibleTabs.Add(dtSP);

                        }
                    }
                          
                    if(nextTurn)
                    {
                        foreach (DominoTab dtSP in playerTabs)
                        {
                            if (dtSP.Values.Contains(numDominoTabA) || dtSP.Values.Contains(numDominoTabB) || numDominoTabA == -1)
                            {
                                nextTurn = false;
                                possibleTabs.Add(dtSP);

                            }
                        }
                    }

                    // Si no tenemos pasamos el turno
                    if (nextTurn)
                    {
                        await Task.Delay(1000);
                        MovePlayer();
                    }
                    else
                    {
                        // Si sólo tenemos una la echamos y pasamos rápido el turno (1 seg)
                        if (possibleTabs != null && possibleTabs.Count == 1)
                        {
                            await Task.Delay(1000);
                            DropDominoTab(possibleTabs.FirstOrDefault());
                        }
                        else
                        {
                            // Si es la primera mano de la ronda
                            // Tendremos en cuenta el número de dobles con los asociados que no son dobles pero coinciden con el doble
                            // 
                            ////if(isFirst)
                            ////{
                            // Es la primera mano

                            // Diferenciamos los dobles de los no dobles
                            List<DominoTab> doublesDT = new List<DominoTab>();
                            List<DominoTab> noDoublesDT = new List<DominoTab>();
                            Dictionary<Int16, List<DominoTab>> doublesWithHisNoDoubleTbas = new Dictionary<short, List<DominoTab>>();
                            foreach (DominoTab dtPossible in possibleTabs)
                            {
                                if (dtPossible.IsDouble)
                                {
                                    doublesDT.Add(dtPossible);
                                    // Asociamos el doble que tenemos e
                                    // inicializamos la lista donde introduciremos los asociados al doble que no son dobles
                                    doublesWithHisNoDoubleTbas.Add(dtPossible.Values[0], new List<DominoTab>());
                                }
                                else
                                {
                                    noDoublesDT.Add(dtPossible);
                                }
                            }

                            Boolean thereAreNoDoublesWithDouble = false;
                            // Asociamos los no dobles que coindicen con los dobles que tenemos                                
                            foreach (DominoTab dtNODouble in noDoublesDT)
                            {
                                foreach (DominoTab doubleDT in doublesDT)
                                {
                                    if (dtNODouble.Values.Contains(doubleDT.Values[0]))
                                    {
                                        thereAreNoDoublesWithDouble = true;
                                        doublesWithHisNoDoubleTbas[doubleDT.Values[0]].Add(dtNODouble);
                                    }
                                }
                            }

                            // Si tenemos algún doble
                            if (doublesDT.Count > 2 || (doublesDT.Count > 0 && (thereAreNoDoublesWithDouble || !isFirst)))
                            {

                                // Obtenemos el doble que tiene más fichas no dobles asociadas, con el el mismo número del doble
                                // y dentro de estas en caso de igualar, la mayor
                                Int16 selectedDouble = doublesWithHisNoDoubleTbas.OrderByDescending(x => x.Value.Count)
                                    .ThenByDescending(x => x.Key).FirstOrDefault().Key;
                                foreach (DominoTab doubleDT in doublesDT)
                                {
                                    if (doubleDT.Values[0] == selectedDouble)
                                    {
                                        if (doublesDT.Count > 2)
                                        {
                                            await Task.Delay(10000);
                                        }
                                        else
                                        {
                                            await Task.Delay(5000);
                                        }

                                        DropDominoTab(doubleDT);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // Seleccionaremos de la ficha que más tenemos de ese número //

                                Dictionary<Int16, List<DominoTab>> numWithAssociatedTabs = new Dictionary<short, List<DominoTab>>();

                                // Inicializamos las listas asociadas para todos los números posibles
                                for (Int16 i = 0; i < 7; i++)
                                {
                                    numWithAssociatedTabs.Add(i, new List<DominoTab>());
                                }

                                // Asociamos cada ficha a su lista correspondiente
                                foreach (DominoTab noDouble in noDoublesDT)
                                {
                                    numWithAssociatedTabs[noDouble.Values[0]].Add(noDouble);
                                    numWithAssociatedTabs[noDouble.Values[1]].Add(noDouble);
                                }

                                // Obtenemos la del número que más tenemos o en caso de igualdad el de mayor número
                                KeyValuePair<Int16, List<DominoTab>> selectedsMoreRepeatNODouble = numWithAssociatedTabs
                                    .OrderByDescending(x => x.Value.Count).ThenByDescending(x => x.Key).FirstOrDefault();

                                // Inicilizamos las listas para los siguientes números que más se repiten
                                Dictionary<Int16, List<DominoTab>> sigNumMoreRepeat = new Dictionary<short, List<DominoTab>>();

                                for (Int16 i = 0; i < 7; i++)
                                {
                                    sigNumMoreRepeat.Add(i, new List<DominoTab>());
                                }

                                // Seleccionamos dentro del número que más tenemos, la siguiente de otro número que más tengamos
                                foreach (DominoTab noDoubleInMoreRepeats in selectedsMoreRepeatNODouble.Value)
                                {
                                    Int16 numNonNoDouble = noDoubleInMoreRepeats.Values[0] == selectedsMoreRepeatNODouble.Key ?
                                        noDoubleInMoreRepeats.Values[1] : noDoubleInMoreRepeats.Values[0];

                                    foreach (DominoTab pT in playerTabs)//noDoublesDT)
                                    {
                                        if (pT.Values[0] == numNonNoDouble || pT.Values[1] == numNonNoDouble)
                                        {
                                            sigNumMoreRepeat[numNonNoDouble].Add(pT);
                                        }
                                    }
                                }

                                DominoTab dtFinal = sigNumMoreRepeat.OrderByDescending(x => x.Value.Count)
                                    .ThenByDescending(x => x.Key).FirstOrDefault().Value.Where(x => x.Values.Contains(selectedsMoreRepeatNODouble.Key)).FirstOrDefault();

                                Boolean trueA = dtFinal.Values.Contains(numDominoTabA);
                                Boolean trueB = dtFinal.Values.Contains(numDominoTabB);
                                if (trueA && trueB && numDominoTabA != numDominoTabB)
                                {
                                    dtFinal.IsForA = numDominoTabA == selectedsMoreRepeatNODouble.Key;
                                    dtFinal.IsForB = numDominoTabB == selectedsMoreRepeatNODouble.Key;
                                }

                                if (dtFinal != null)
                                {
                                    await Task.Delay(5000);
                                    DropDominoTab(dtFinal);
                                }
                                else
                                {
                                    throw new Exception("Error al seleccionar ficha =(");
                                }
                            }                           
                        }
                    }
                }
            }
        }

        private async void EndRound()
        {
            cMain.Children.Remove(ImagePlayerRound);

            if(Turn != null && cMain.Children.Contains(Turn))
            {
                cMain.Children.Remove(Turn);
            }

            foreach (DominoTab dT in PlayerDominoTabs)
            {
                dT.IsBlack = false;
                dT.Disabled = true;
            }

            double posMainMidY = cMain.ActualHeight / 2;
            double posMainMidX = cMain.ActualWidth / 2;

            txtCountNewRound = new TextBlock();
            txtCountNewRound.Text = "15";
            countNewRound = 15;
            txtCountNewRound.TextAlignment = TextAlignment.Center;
            txtCountNewRound.Height = 200;
            txtCountNewRound.Width = 200;
            txtCountNewRound.FontSize = 100;
            txtCountNewRound.Foreground = new SolidColorBrush(Colors.Transparent);                
            txtCountNewRound.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            cMain.Children.Add(txtCountNewRound);
            txtCountNewRound.SetValue(Canvas.TopProperty, posMainMidY - (txtCountNewRound.Height / 2));
            txtCountNewRound.SetValue(Canvas.LeftProperty, posMainMidX - (txtCountNewRound.Width / 2));                                       

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();

            await Task.Delay(15000);

            timer.Stop();

            cMain.Children.Clear();
            PlayerDominoTabs = new List<DominoTab>();
            OthersDominoTabs = new List<DominoTab>();
            InGameDominoTabs = new List<DominoTab>();            
            DireccionA = 0;
            numDominoTabA = -1;
            numDominoTabB = -1;
            showTabs.IsChecked = false;
            IsRotateA = false;
            IsRotateB = false;
            DireccionB = 1;
            IsMultiplayer = false;
            IsFirst = false;            
            setDominoLocalGame(PlayerId);
        }

        private bool BlinkOn = false;
        private Int16 countNewRound = 15;
        private void Timer_Tick(object sender, object e)
        {
            if (BlinkOn)
            {
                txtCountNewRound.Foreground = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                txtCountNewRound.Foreground = new SolidColorBrush(Colors.Black);
                countNewRound--;
                txtCountNewRound.Text = countNewRound.ToString();
            }
            BlinkOn = !BlinkOn;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                ////Classes.Message m = new Classes.Message() { IdUser = u, Mess = txtMessage.Text };
                ////sendToServer(m);

                ////if (reader != null)
                ////{                    
                ////    uint count = await reader.LoadAsync(1024);
                ////    //txtMessage.Text = reader.ReadString(count);                        
                ////}
            }
            catch (Exception ex)
            {

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

        private void addAllRooms()
        {
            double xPosMax = cMain.ActualWidth;

            double xPos = 0;
            double yPos = 60;
            foreach (Room r in Rooms)
            {
                r.MyIdUsuario = u.Id;

                DominoRoomSprite dRS = new DominoRoomSprite();
                dRS.ButtonClick += DRS_ButtonClick;
                dRS.DataContext = r;
                dRS.SetValue(Canvas.TopProperty, yPos);
                dRS.SetValue(Canvas.LeftProperty, xPos);

                cMain.Children.Add(dRS);

                xPos += 210;

                if (xPos + 200 > xPosMax)
                {
                    xPos = 0;
                    yPos += 210;
                }
            }
        }

        private void DRS_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlayerState == 1)
            {
                PlayerState = 2;

                Button bt = (Button)sender;
                Room r = (Room)bt.DataContext;
                String t = (String)bt.Tag;
                Int16 i;
                Int16.TryParse(t, out i);
                if(!r.UsersOcupation[i])
                {
                    r.resetStatesRoom();
                    r.Update = true;
                    r.Position = i;
                    sendToServer(r);
                }
                else
                {
                    PlayerState = 1;
                }

            }
            else if(PlayerState == 2)
            {
                PlayerState = 1;

                Button bt = (Button)sender;
                Room r = (Room)bt.DataContext;
                String t = (String)bt.Tag;
                Int16 i;
                Int16.TryParse(t, out i);
                if (r.UsersOcupation[i] && r.Users[i] == u.Id)
                {
                    r.resetStatesRoom();
                    r.Delete = i != 0 ? true : false;
                    r.DeleteRoom = i == 0 ? true : false;
                    r.Position = i;
                    sendToServer(r);
                }
                else
                {
                    PlayerState = 2;
                }
            }

        }

        private void AddRoom_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlayerState == 1)
            {
                PlayerState = 2;
                sendToServer(new Room() { Add = true });
                cMain.Children[0].Visibility = Visibility.Collapsed;
            }
        }

        private void addRoom(Room room)
        {
            double xPosMax = cMain.ActualWidth;

            UIElement uiE = cMain.Children.LastOrDefault();

            if (uiE is DominoRoomSprite)
            {
                if (room.Add)
                {
                    DominoRoomSprite lastDRS = (DominoRoomSprite)uiE;
                    double xPos = (double)lastDRS.GetValue(Canvas.LeftProperty);
                    double yPos = (double)lastDRS.GetValue(Canvas.TopProperty);

                    xPos += 210;

                    if (xPos + 200 > xPosMax)
                    {
                        xPos = 0;
                        yPos += 210;
                    }

                    DominoRoomSprite dRS = new DominoRoomSprite();
                    dRS.ButtonClick += DRS_ButtonClick;
                    dRS.DataContext = room;
                    dRS.SetValue(Canvas.TopProperty, yPos);
                    dRS.SetValue(Canvas.LeftProperty, xPos);

                    cMain.Children.Add(dRS);                    
                } 
            }
            else
            {
                if (room.Add)
                {
                    double xPos = 0;
                    double yPos = 60;

                    if (xPos + 200 > xPosMax)
                    {
                        xPos = 0;
                        yPos += 210;
                    }

                    DominoRoomSprite dRS = new DominoRoomSprite();
                    dRS.ButtonClick += DRS_ButtonClick;
                    dRS.DataContext = room;
                    dRS.SetValue(Canvas.TopProperty, yPos);
                    dRS.SetValue(Canvas.LeftProperty, xPos);

                    cMain.Children.Add(dRS);
                }
            }
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

        private void setDominoLocalGame(Int16 playerId)
        {
            // Crea la ficha en clase y asocia un guid aleatorio para simular "barajar"
            List<DominoTab> dTabs = new List<DominoTab>();
            foreach (Int16[] _dTab in AllDominoTabs)
            {
                DominoTab dTab = new DominoTab();
                dTab.Id = Guid.NewGuid();
                dTab.Values = _dTab;
                dTabs.Add(dTab);
            }

            // Aleatorizar/Barajar
            var dOrderedTabs = dTabs.OrderBy(x => x.Id);

            // Asociacion de jugadores
            Int16 player = -1;
            Int16 i = 0;
            foreach (DominoTab dT in dOrderedTabs)
            {
                if (i % 7 == 0)
                {
                    player = (Int16)(i / 7);
                }

                dT.Player = player;

                i++;
            }

            // Selección de mi jugador
            Random rnd = new Random();
            // Si es la primera partida seleccionamos jugador aleatorio si no el jugador que ya teníamos
            PlayerId = playerId != -1 ? playerId : (Int16)rnd.Next(0, 3);
            // Si es la primera ronda obtenemos turno aleatorio si no damos el turno al siguiente de la mano anterior por la derecha
            if (LastMainPlayerRound == -1)
            {
                PlayerRound = (Int16)rnd.Next(0, 3);
            }
            else
            {
                Int16 lastMainPlayerPosition = PlayersPositions[LastMainPlayerRound];
                lastMainPlayerPosition = (Int16)(lastMainPlayerPosition == 0 ? 3 : lastMainPlayerPosition - 1);
                Int16 newPlayerMainPosition = -1;
                for (Int16 j = 0; j < 4; j++)
                {
                    if(PlayersPositions[j] == lastMainPlayerPosition)
                    {
                        newPlayerMainPosition = j;
                    }
                }

                PlayerRound = newPlayerMainPosition;
            }
            
            // Asociamos el jugador que empieza para valorar en la siguiente ronda, si la hubiera
            LastMainPlayerRound = PlayerRound;


            // Diferenciamos mis fichas de las de los demás jugadores
            PlayerDominoTabs = dOrderedTabs.ToList();
                                                            
            // Asociamos las posiciones de los jugadores, asociando siempre la posición 3 al id que me haya tocado a mí
            // Posicion 1 a mi compañero y 0 y 2 a los contrincantes, recuerde, hablamos de posiciones no de número de id de jugador
            // Las posiciones son fijas en número de jugador puede cambiar aunque se mantiene dentro de la misma ronda
            PlayersPositions[PlayerId] = 3;
            PlayersPositions[PlayerId == 0 ? 1 : PlayerId == 1 ? 0 : PlayerId == 2 ? 3 : PlayerId == 3 ? 2 : -1] = 2;
            PlayersPositions[PlayerId == 0 ? 2 : PlayerId == 1 ? 3 : PlayerId == 2 ? 0 : PlayerId == 3 ? 1 : -1] = 1;
            PlayersPositions[PlayerId == 0 ? 3 : PlayerId == 1 ? 2 : PlayerId == 2 ? 1 : PlayerId == 3 ? 0 : -1] = 0;

            // FICHAS JUGADORES Y TURNO
            PaintPlayersTabsAndTurn(true);
        }

        /// <summary>Pinta las FICHAS JUGADORES Y TURNO</summary>
        /// <param name="isInGame">Si es en juego real (true) o en simulación (false) para el (page size change)</param>
        private void PaintPlayersTabsAndTurn(Boolean isInGame)
        {
            // Posicion menor a la mitad de X de la "pantalla" menos la mitad de las fichas donde empezaremos a pintar las fichas
            // (contado el margen entre ellas que será de 5)

            double myPositionY = cMain.ActualHeight - DominoTabHeigth - 5;
            double myPositionX = (cMain.ActualWidth / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;

            double maxPositionY = cMain.ActualHeight - DominoTabHeigth - 5;
            double positionXm = (cMain.ActualWidth / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;

            Int16 playerPostition = -1;
            Int16 player = -1;
            for (Int32 i = 0; i < PlayerDominoTabs.Count; i++)
            {
                DominoTab dT = PlayerDominoTabs[i];

                if (dT.Player != PlayerId)
                {
                    if (dT.Player != player)
                    {
                        player = dT.Player;
                        playerPostition = PlayersPositions[player];

                        switch (playerPostition)
                        {
                            case 0:
                                maxPositionY = (cMain.ActualHeight / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                positionXm = 5 + ((DominoTabHeigth / 2) - (DominoTabWidth / 2));
                                break;

                            case 1:
                                maxPositionY = 5;
                                positionXm = (cMain.ActualWidth / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                break;

                            case 2:
                                maxPositionY = (cMain.ActualHeight / 2) - (DominoTabWidth * 3) - (DominoTabWidth / 2) - 6;
                                positionXm = cMain.ActualWidth - 5 - DominoTabHeigth + ((DominoTabHeigth - DominoTabWidth) / 2);
                                break;
                        }
                    }

                    dT.IsBlack = true;

                    DominoTabSprite r = new DominoTabSprite();
                    r.DataContext = dT;
                    r.Tag = dT;
                    //r.PointerPressed += R_PointerPressedDominoTab;
                    r.PointerReleased += R_PointerReleasedDominoTab;
                    dT.SourceX = positionXm;
                    dT.SourceY = maxPositionY;
                    dT.Sprite = r;
                    r.SetValue(Canvas.TopProperty, maxPositionY);
                    r.SetValue(Canvas.LeftProperty, positionXm);

                    switch (playerPostition)
                    {
                        case 0:
                            r.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 90 };
                            dT.IsRotate = true;
                            cMain.Children.Add(r);

                            maxPositionY += DominoTabWidth + 2;
                            break;

                        case 1:
                            r.Height = DominoTabHeigth;
                            r.Width = DominoTabWidth;
                            cMain.Children.Add(r);

                            positionXm += DominoTabWidth + 2;
                            break;

                        case 2:
                            r.RenderTransform = new RotateTransform() { CenterX = 0.5, CenterY = 0.5, Angle = 90 };
                            dT.IsRotate = true;
                            cMain.Children.Add(r);

                            maxPositionY += DominoTabWidth + 2;
                            break;
                    }
                }
                else
                {
                    DominoTabSprite r = new DominoTabSprite();
                    //r.Fill = new SolidColorBrush(Colors.Red);
                    r.DataContext = dT;
                    r.Tag = dT;
                    r.Height = DominoTabHeigth;
                    r.Width = DominoTabWidth;
                    r.PointerPressed += R_PointerPressedDominoTab;
                    r.PointerReleased += R_PointerReleasedDominoTab;
                    dT.SourceX = myPositionX;
                    dT.SourceY = myPositionY;
                    r.SetValue(Canvas.TopProperty, myPositionY);
                    r.SetValue(Canvas.LeftProperty, myPositionX);
                    dT.Sprite = r;
                    cMain.Children.Add(r);

                    myPositionX += DominoTabWidth + 2;
                }
            }

            Image img = new Image();
            ImagePlayerRound = img;
            img.Source = new BitmapImage(new Uri("ms-appx:///Images/R.gif", UriKind.Absolute));
            img.Height = ImgMovePlayerHeigth;
            img.Width = ImgMovePlayerWidth;
            //img.PointerReleased += Img_PointerReleased;


            for (Int16 j = 0; j < 4; j++)
            {
                if (j == PlayerRound)
                {
                    double rPositionY = -1;
                    double rPositionX = -1;
                    Int16 position = PlayersPositions[PlayerRound];
                    switch (position)
                    {
                        case 0:
                            rPositionY = (cMain.ActualHeight / 2) - (ImgMovePlayerHeigth / 2);
                            rPositionX = DominoTabHeigth + 10;
                            break;

                        case 1:
                            rPositionY = DominoTabHeigth + 10;
                            rPositionX = (cMain.ActualWidth / 2) - (ImgMovePlayerWidth / 2);
                            break;

                        case 2:
                            rPositionY = (cMain.ActualHeight / 2) - (ImgMovePlayerHeigth / 2);
                            rPositionX = cMain.ActualWidth - ImgMovePlayerWidth - DominoTabHeigth - 10;
                            break;
                        case 3:
                            rPositionY = (cMain.ActualHeight) - ImgMovePlayerHeigth - DominoTabHeigth - 10;
                            rPositionX = (cMain.ActualWidth / 2) - (ImgMovePlayerWidth / 2);
                            break;

                    }
                    img.SetValue(Canvas.TopProperty, rPositionY);
                    img.SetValue(Canvas.LeftProperty, rPositionX);
                    cMain.Children.Add(img);
                    if (isInGame)
                    {
                        if (position != 3)
                        {
                            ckeckBotPlayer(position, true);
                        }
                    }
                    break;
                }
            }
        }

        private void closeNewGame_Click(object sender, RoutedEventArgs e)
        {
            SelectMaxPoints.Visibility = Visibility.Collapsed;
        }
        private void maxPoint_Click(object sender, RoutedEventArgs e)
        {
            SelectMaxPoints.Visibility = Visibility.Collapsed;

            Button btn = (Button)sender;

            cMain.Children.Clear();
            PlayerDominoTabs = new List<DominoTab>();
            OthersDominoTabs = new List<DominoTab>();
            InGameDominoTabs = new List<DominoTab>();
            DominoTabHeigth = 66;
            DominoTabWidth = 33;
            ImgMovePlayerHeigth = 50;
            ImgMovePlayerWidth = 50;
            DireccionA = 0;
            FinalPunt = Convert.ToInt16(btn.Content);
            DireccionB = 1;
            PlayerRound = -1;
            LastMainPlayerRound = -1;
            PuntMyTeam = 0;
            PuntOtherTeam = 0;
            NuevaPartida = false;
            IsMultiplayer = false;
            PlayerRound = 0;
            IsRotateA = false;
            IsRotateB = false;
            showTabs.IsChecked = false;
            numDominoTabA = -1;
            numDominoTabB = -1;
            myScore.Text = "Mi Equipo: 0";
            otherScore.Text = "Equipo 2:  0";
            PlayersPositions = new Int16[4];
            IsFirst = false;
            setDominoLocalGame(-1);
        }

        private void showTabs_Checked(object sender, RoutedEventArgs e)
        {
            if (PlayerDominoTabs != null)
            {
                for (Int32 i = 0; i < PlayerDominoTabs.Count; i++)
                {
                    PlayerDominoTabs[i].IsBlack = false;
                }
            }
        }

        private void showTabs_Unchecked(object sender, RoutedEventArgs e)
        {
            if (PlayerDominoTabs != null)
            {
                for (Int32 i = 0; i < PlayerDominoTabs.Count; i++)
                {
                    if (PlayerDominoTabs[i].Player != PlayerId)
                    {
                        PlayerDominoTabs[i].IsBlack = true;
                    }
                }
            }
        }

        private void PatrionsClose_Click(object sender, RoutedEventArgs e)
        {
            if(Visibility.Visible == Patrions.Visibility)
            {
                Patrions.Visibility = Visibility.Collapsed;
            }
            else
            {
                Patrions.Visibility = Visibility.Visible;
            }
        }
    }
}
