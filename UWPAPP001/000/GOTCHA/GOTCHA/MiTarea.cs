using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace GOTCHA
{
    // Clase que implementa la interfaz IBackgroundTask
    public sealed class MiTarea : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        private void ShowToastNotification(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(toast);
        }

        // Método Run que se ejecuta cuando se desencadena el evento especificado
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            
            try
            {
                ShowToastNotification("HOLA", "HOLIWA");
                //Debug.WriteLine("=)");
                // Obtener el acceso a la biblioteca de videos
                StorageFolder videoLibrary = KnownFolders.VideosLibrary;

                // Crear la carpeta GOTCHA dentro de la biblioteca de videos
                StorageFolder gotchaFolder = await videoLibrary.CreateFolderAsync("GOTCHA", CreationCollisionOption.OpenIfExists);

                // Crear el archivo de texto dentro de la carpeta GOTCHA
                StorageFile textFile = await gotchaFolder.CreateFileAsync("texto.txt", CreationCollisionOption.ReplaceExisting);

                // Escribir el texto en el archivo
                await FileIO.WriteTextAsync(textFile, "Este es un texto de prueba");

            }
            catch(Exception ex)
            {
                //Debug.WriteLine("=)");
            }


            //try
            //{
            //    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            //    var _mediaCapture = new MediaCapture();

            //    // Crea un objeto de configuración para la captura de vídeo
            //    var settings = new MediaCaptureInitializationSettings
            //    {
            //        VideoDeviceId = (String)localSettings.Values["vD_ID"],
            //        StreamingCaptureMode = StreamingCaptureMode.Video,
            //        //PhotoCaptureSource = PhotoCaptureSource.Photo                
            //    };

            //    // Inicializa la captura de vídeo con la fuente seleccionada
            //    await _mediaCapture.InitializeAsync(settings);

            //    // Get information about the preview
            //    var previewProperties = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;



            //    Boolean continuel = true;
            //    while (continuel)
            //    {
            //        if ((Boolean)localSettings.Values["Recording"])
            //        {
            //            if ((DateTime)localSettings.Values["RecordingMaxTBD"] < DateTime.Now)
            //            {
            //                var r = await _mediaCapture.StopRecordWithResultAsync();
            //                localSettings.Values["Recording"] = false;
            //                continuel = false;
            //            }
            //        }

            //        Thread.Sleep(444);

            //        // Create the video frame to request a SoftwareBitmap preview frame
            //        var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)previewProperties.Width, (int)previewProperties.Height);


            //        // Capture the preview frame
            //        using (var currentFrame = await _mediaCapture.GetPreviewFrameAsync(videoFrame))
            //        {
            //            // Collect the resulting frame
            //            SoftwareBitmap previewFrame = currentFrame.SoftwareBitmap;

            //            //Si el frame no es nulo, obtiene el SoftwareBitmap
            //            if (previewFrame != null)
            //            {
            //                var vF1 = (Int32)localSettings.Values["vF1"];
            //                if (vF1 == -1)
            //                {
            //                    localSettings.Values["vF1"] = await MainPage.GetBNImage(previewFrame);
            //                    return;
            //                }
            //                else
            //                {
            //                    var vF = await MainPage.GetBNImage(previewFrame);

            //                    Int32 porcentual = (Int32)(previewProperties.Width * previewProperties.Height) / 50;

            //                    if (Math.Abs(vF1 - vF) > porcentual)
            //                    {
            //                        localSettings.Values["RecordingMaxTBD"] = DateTime.Now.AddMinutes(1);

            //                        if (!(Boolean)localSettings.Values["Recording"])
            //                        {
            //                            localSettings.Values["Recording"] = true;

            //                            // Obtener una referencia a la biblioteca de vídeos
            //                            StorageLibrary videoLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);
            //                            // Obtener la carpeta donde se guardan los nuevos archivos de forma predeterminada
            //                            StorageFolder saveFolder = videoLibrary.SaveFolder;

            //                            StorageFolder GOTCHA = (await saveFolder.GetFoldersAsync(Windows.Storage.Search.CommonFolderQuery.DefaultQuery)).Where(x => x.Name == "GOTCHA").SingleOrDefault();

            //                            if (GOTCHA == null)
            //                            {
            //                                // Crear una carpeta llamada GOTCHA en esa carpeta
            //                                GOTCHA = await saveFolder.CreateFolderAsync("GOTCHA");
            //                            }

            //                            // Crear un nuevo archivo en esa carpeta
            //                            StorageFile videoFile = await GOTCHA.CreateFileAsync(
            //                                String.Concat(
            //                                    "GOTCHA_"
            //                                    , DateTime.Now.Year.ToString().PadLeft(4, '0')
            //                                    , DateTime.Now.Month.ToString().PadLeft(2, '0')
            //                                    , DateTime.Now.Day.ToString().PadLeft(2, '0')
            //                                    , "_"
            //                                    , DateTime.Now.Hour.ToString().PadLeft(2, '0')
            //                                    , DateTime.Now.Minute.ToString().PadLeft(2, '0')
            //                                    , DateTime.Now.Second.ToString().PadLeft(2, '0')
            //                                    , "_"
            //                                    , DateTime.Now.Millisecond.ToString().PadLeft(2, '0')
            //                                    , ".mp4"));

            //                            MediaEncodingProfile profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD1080p);
            //                            await _mediaCapture.StartRecordToStorageFileAsync(profile, videoFile);

            //                        }
            //                    }

            //                    localSettings.Values["vF1"] = vF;

            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
            _deferral.Complete();
        }

    }
}
