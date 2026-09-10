using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
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
using Windows.UI.Notifications;
using System.Reflection;
using GOTCHA;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.ApplicationModel.ExtendedExecution.Foreground;
using Windows.Graphics.Display;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace GOTCHA
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MediaCapture _mediaCapture { get; set; }
        public ApplicationTrigger aT { get; set; }

        public Boolean TimerOK { get; set; }

        public System.Timers.Timer _Timer { get; set; }
                
        private Int32 vF1 = -1;

        public Boolean Recording { get; set; }

        public DateTime RecordingMaxTBD { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        public async static Task<BackgroundTaskRegistration> RegisterBackgroundTask(
            string taskEntryPoint,
            string taskName,
            IBackgroundTrigger trigger,
            IBackgroundCondition condition)
        {
            // Comprobar si la tarea ya está registrada
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return (BackgroundTaskRegistration)task.Value;
                }
            }

            // Solicitar el permiso de ejecución en segundo plano
            var requestStatus = await BackgroundExecutionManager.RequestAccessAsync();

            // Crear el objeto BackgroundTaskBuilder
            var builder = new BackgroundTaskBuilder();
            builder.Name = taskName;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            // Agregar una condición si se desea
            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            // Registrar la tarea en segundo plano
            BackgroundTaskRegistration taskR = builder.Register();

            return taskR;
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var newSession = new ExtendedExecutionForegroundSession();
            newSession.Reason = ExtendedExecutionForegroundReason.Unconstrained;
            newSession.Description = "Long Running Processing";
            newSession.Revoked += NewSession_Revoked;
            ExtendedExecutionForegroundResult result = await newSession.RequestExtensionAsync();
            switch (result)
            {
                case ExtendedExecutionForegroundResult.Allowed:

                    break;

                default:
                case ExtendedExecutionForegroundResult.Denied:

                    break;
            }
            

            Recording = false;

            vF1 = -1;
            
            devices.Children.Clear();

            // Obtiene la lista de fuentes de vídeo disponibles
            var videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            if (videoDevices != null && videoDevices.Count > 0)
            {
                foreach (DeviceInformation vD in videoDevices)
                {
                    Button button = new Button();
                    button.Content = vD.Name;
                    button.Tag = vD;
                    button.Click += vDSELECTED_Click;
                    button.Margin = new Thickness(3);
                    

                    devices.Children.Add(button);
                }
            }
        }

        private async void vDSELECTED_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await System.Threading.Tasks.Task.Run(async () =>
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        _mediaCapture = new MediaCapture();

                    DeviceInformation vD = ((Button)sender).Tag as DeviceInformation;

                    // Crea un objeto de configuración para la captura de vídeo
                    var settings = new MediaCaptureInitializationSettings
                    {
                        VideoDeviceId = vD.Id,
                        StreamingCaptureMode = StreamingCaptureMode.Video,
                        //PhotoCaptureSource = PhotoCaptureSource.Photo                
                    };

                    // Inicializa la captura de vídeo con la fuente seleccionada
                    await _mediaCapture.InitializeAsync(settings);

                    
                        // Asigna la fuente de vídeo al elemento CaptureElement de la interfaz de usuario
                        _cE.Source = _mediaCapture;
                    
                    

                    // Inicia la vista previa de la fuente de vídeo
                    await _mediaCapture.StartPreviewAsync();

                        //IsPreviewing = true;
                    });
                });
                
                MovDetection.Visibility = Visibility.Visible;

            }
            catch(Exception ex)
            {

            }
        }

        private async void refreshDevices_Click(object sender, RoutedEventArgs e)
        {
            Page_Loaded(null, null);
            MovDetection.Visibility = Visibility.Collapsed;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;                        
        }

        private void Deregister(String taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                }
            }
        }

        private bool IsRegistered(String taskName)
        {

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsTaskRegistered(string taskName) =>
            BackgroundTaskRegistration.AllTasks.Any(x => x.Value.Name.Equals(taskName));
        public BackgroundTaskRegistration TaskByName(string taskName) =>
            BackgroundTaskRegistration.AllTasks.FirstOrDefault(x => x.Value.Name.Equals(taskName)).Value as BackgroundTaskRegistration;
        
        public async Task ComputeNextMove()
        {
            
        }



        public async void START(object state, ElapsedEventArgs e)
        {
            try
            {
                if (Recording)
                {
                    if (RecordingMaxTBD < DateTime.Now)
                    {
                        var r = await _mediaCapture.StopRecordWithResultAsync();
                        Recording = false;
                        //continuel = false;
                    }
                }
                else
                {

                    //////var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                    //////var _mediaCapture = new MediaCapture();

                    //////// Crea un objeto de configuración para la captura de vídeo
                    //////var settings = new MediaCaptureInitializationSettings
                    //////{
                    //////    VideoDeviceId = (String)localSettings.Values["vD_ID"],
                    //////    StreamingCaptureMode = StreamingCaptureMode.Video,
                    //////    //PhotoCaptureSource = PhotoCaptureSource.Photo                
                    //////};

                    //////// Inicializa la captura de vídeo con la fuente seleccionada
                    //////await _mediaCapture.InitializeAsync(settings);

                    //////await _mediaCapture.StopPreviewAsync();

                    // Get information about the preview
                    var previewProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

                    // Create the video frame to request a SoftwareBitmap preview frame
                    var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)previewProperties.Width, (int)previewProperties.Height);


                    // Capture the preview frame
                    using (var currentFrame = await _mediaCapture.GetPreviewFrameAsync(videoFrame))
                    {
                        // Collect the resulting frame
                        SoftwareBitmap previewFrame = currentFrame.SoftwareBitmap;

                        //Si el frame no es nulo, obtiene el SoftwareBitmap
                        if (previewFrame != null)
                        {
                            if (vF1 == -1)
                            {
                                vF1 = await MainPage.GetBNImage(previewFrame);
                                return;
                            }
                            else
                            {
                                var vF = await MainPage.GetBNImage(previewFrame);

                                Int32 porcentual = (Int32)(previewProperties.Width * previewProperties.Height) / 50;

                                if (Math.Abs(vF1 - vF) > porcentual)
                                {
                                    RecordingMaxTBD = DateTime.Now.AddMinutes(1);

                                    if (!Recording)
                                    {
                                        Recording = true;

                                        // Obtener una referencia a la biblioteca de vídeos
                                        StorageLibrary videoLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);
                                        // Obtener la carpeta donde se guardan los nuevos archivos de forma predeterminada
                                        StorageFolder saveFolder = videoLibrary.SaveFolder;

                                        StorageFolder GOTCHA = (await saveFolder.GetFoldersAsync(Windows.Storage.Search.CommonFolderQuery.DefaultQuery)).Where(x => x.Name == "GOTCHA").SingleOrDefault();

                                        if (GOTCHA == null)
                                        {
                                            // Crear una carpeta llamada GOTCHA en esa carpeta
                                            GOTCHA = await saveFolder.CreateFolderAsync("GOTCHA");
                                        }

                                        // Crear un nuevo archivo en esa carpeta
                                        StorageFile videoFile = await GOTCHA.CreateFileAsync(
                                            String.Concat(
                                                "GOTCHA_"
                                                , DateTime.Now.Year.ToString().PadLeft(4, '0')
                                                , DateTime.Now.Month.ToString().PadLeft(2, '0')
                                                , DateTime.Now.Day.ToString().PadLeft(2, '0')
                                                , "_"
                                                , DateTime.Now.Hour.ToString().PadLeft(2, '0')
                                                , DateTime.Now.Minute.ToString().PadLeft(2, '0')
                                                , DateTime.Now.Second.ToString().PadLeft(2, '0')
                                                , "_"
                                                , DateTime.Now.Millisecond.ToString().PadLeft(2, '0')
                                                , ".mp4"));

                                        MediaEncodingProfile profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD1080p);
                                        await _mediaCapture.StartRecordToStorageFileAsync(profile, videoFile);

                                    }
                                }

                                vF1 = vF;

                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {

            }

            //if (!TimerOK)
            //{
            //TimerOK = true;
            //var r = await aT.RequestAsync();
            //}
        }

        

        private async void MovDetection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ////if (BackgroundTaskHelper.IsBackgroundTaskRegistered("MiTarea"))
                ////{
                ////    BackgroundTaskHelper.Unregister("MiTarea");
                ////}
                ////if (BackgroundTaskHelper.IsBackgroundTaskRegistered("GOTCHA.MiTarea"))
                ////{
                ////    BackgroundTaskHelper.Unregister("GOTCHA.MiTarea");
                ////}

                ////if (BackgroundTaskHelper.IsBackgroundTaskRegistered("MT"))
                ////{
                ////    BackgroundTaskHelper.Unregister("MT");
                ////}

                ////if (BackgroundTaskHelper.IsBackgroundTaskRegistered("GOTCHA.BackGround.MiTarea"))
                ////{
                ////    BackgroundTaskHelper.Unregister("GOTCHA.BackGround.MiTarea");
                ////}

                ////aT = new ApplicationTrigger();                
                //////TimeTrigger tT = new TimeTrigger(15, false);

                //////List<StorageLibrary> sLL = new List<StorageLibrary>();

                //////sLL.Add(await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos));

                //////StorageLibraryContentChangedTrigger sLCT = StorageLibraryContentChangedTrigger.CreateFromLibraries(sLL);

                ////SystemCondition userCondition = new SystemCondition(SystemConditionType.UserPresent);
                //////En el proyecto de la aplicación, llamar a la función para registrar la tarea en segundo plano
                ////// Usar un SystemTrigger como ejemplo de desencadenador
                ////var task = await RegisterBackgroundTask(
                ////    "GOTCHA.MiTarea",
                ////    "GOTCHA.MiTarea",
                ////    aT,
                ////    userCondition);
                                
                _Timer = new System.Timers.Timer(444);

                _Timer.Elapsed += (o, s) => Task.Factory.StartNew(() => START(o, s));
                _Timer.Start();

                MovDetection.Visibility = Visibility.Collapsed;
                IREC.Visibility = Visibility.Visible;
                refresh.IsEnabled = false;
                foreach (UIElement uiE in devices.Children)
                {
                    ((Button)uiE).IsEnabled = false;
                }
            }
            catch(Exception ex)
            {

            }
            
        }

        private void NewSession_Revoked(object sender, ExtendedExecutionForegroundRevokedEventArgs args)
        {
            throw new NotImplementedException();
        }

        public static async Task<Int32> GetBNImage(SoftwareBitmap sftBM)
        {
            Int32 us = 0;

            using (var stream = new InMemoryRandomAccessStream())
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                encoder.SetSoftwareBitmap(sftBM);
                await encoder.FlushAsync();
                var bytes = new byte[stream.Size];
                await stream.AsStream().ReadAsync(bytes, 0, bytes.Length);

                for (Int32 j = 54; j < bytes.Length; j += 4)
                {
                    var b = bytes[j];
                    var g = bytes[j + 1];
                    var r = bytes[j + 2];
                    //var a = bytes[j + 3];

                    // Obtenemos la media para Blanco y Negro
                    var averageValue = ((int)r + (int)b + (int)g) / 3;

                    if(averageValue > 128)
                    {
                        us++;
                    }                    
                }

                return us;
            }
                    
        }

        private async void GOTCHAFOLDER_Click(object sender, RoutedEventArgs e)
        {
            //Thread.Sleep(10000);
            //var r = await aT.RequestAsync();

            // Obtener una referencia a la biblioteca de vídeos
            StorageLibrary videoLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);
            // Obtener la carpeta donde se guardan los nuevos archivos de forma predeterminada
            StorageFolder saveFolder = videoLibrary.SaveFolder;

            StorageFolder GOTCHA = (await saveFolder.GetFoldersAsync(Windows.Storage.Search.CommonFolderQuery.DefaultQuery)).Where(x => x.Name == "GOTCHA").SingleOrDefault();

            if (GOTCHA == null)
            {
                // Crear una carpeta llamada GOTCHA en esa carpeta
                GOTCHA = await saveFolder.CreateFolderAsync("GOTCHA");
            }

            await Launcher.LaunchFolderAsync(GOTCHA);
        }

        private async void IREC_Click(object sender, RoutedEventArgs e)
        {
            _Timer.Dispose();

            if (Recording)
            {
                var r = await _mediaCapture.StopRecordWithResultAsync();
                Recording = false;
                //continuel = false;
                
            }
            
            refresh.IsEnabled = true;
            foreach (UIElement uiE in devices.Children)
            {
                ((Button)uiE).IsEnabled = true;
            }

            MovDetection.Visibility = Visibility.Visible;
            IREC.Visibility = Visibility.Collapsed;
        }
    }
}
