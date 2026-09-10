using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Windows.UI.Popups;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace PrimesNumbersSearch
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StreamSocket streamSocket;
        PUser u;

        Stream outputStream;
        StreamWriter streamWriter;

        DataReader reader;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void connect()
        {
            try
            {
                if (streamSocket == null)
                {
                    //btnConnect.IsEnabled = false;
                    //btnConnect.Visibility = Visibility.Collapsed;

                    streamSocket = new StreamSocket();
                    var hostName = new Windows.Networking.HostName("192.168.1.46"); //192.168.1.46
                    await streamSocket.ConnectAsync(hostName, "63495");

                    u = new PUser() { Id = Guid.NewGuid(), Password = Guid.NewGuid() };
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
                //PlayerState = 1;

                //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                //    double xMidPage = cMain.ActualWidth / 2;
                //    double yMidPage = cMain.ActualHeight / 2;

                //    Button btn = new Button();
                //    btn.Content = "Crear mesa";
                //    btn.Width = 200;
                //    btn.Height = 50;
                //    btn.SetValue(Canvas.TopProperty, 0);
                //    btn.SetValue(Canvas.LeftProperty, xMidPage - 100);
                //    btn.Click += AddRoom_ButtonClick;
                //    cMain.Children.Add(btn);
                //});

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
                        if (m is Message)
                        {
                            //txtMessage.Text = ((Message)m).Mess;
                        }
                        else if (m is Error)
                        {
                            throw new Exception(((Error)m).Message);

                        }
                        //else if (m is Room[])
                        //{
                            
                        //}                        
                        else
                        {
                            throw new Exception("La interfaz no usa los parámetros de forma correcta");
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

                    //btnConnect.IsEnabled = true;
                    //btnConnect.Visibility = Visibility.Visible;
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


    [Serializable]
    public class Parameter
    {
        public String Type { get; set; }

        public String ObjectS { get; set; }
    }

    [Serializable]
    public class PUser
    {
        public Guid Id { get; set; }

        public Guid Password { get; set; }

        public Int16 State { get; set; }

        public DateTime LastUpdate { get; set; }
                
    }

    [Serializable]
    public class Error
    {
        public String Message { get; set; }
    }

    [Serializable]
    public class Message
    {
        public PUser User { get; set; }
        public String Mess { get; set; }
    }
}
