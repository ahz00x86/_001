using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;

namespace rGOTCHA
{
    internal class Program
    {
        public VideoCaptureDevice _mediaCapture { get; set; }
        
        public Boolean TimerOK { get; set; }

        public System.Timers.Timer _Timer { get; set; }

        private Int32 vF1 = -1;

        public Boolean Recording { get; set; }

        public DateTime RecordingMaxTBD { get; set; }

        public static async Task<Int32> GetBNImage(SoftwareBitmap sftBM)
        {
            Int32 us = 0;

            using (var stream = new InMemoryRandomAccessStream())
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                encoder.SetSoftwareBitmap(sftBM);
                await encoder.FlushAsync();
                var bytes = new byte[stream.Size];
                await stream.ReadAsync(bytes, 0, bytes.Length);

                for (Int32 j = 54; j < bytes.Length; j += 4)
                {
                    var b = bytes[j];
                    var g = bytes[j + 1];
                    var r = bytes[j + 2];
                    //var a = bytes[j + 3];

                    // Obtenemos la media para Blanco y Negro
                    var averageValue = ((int)r + (int)b + (int)g) / 3;

                    if (averageValue > 128)
                    {
                        us++;
                    }
                }

                return us;
            }

        }


        static void Main(string[] args)
        {
            _Timer = new System.Timers.Timer(444);

            _Timer.Elapsed += (o, s) => Task.Factory.StartNew(() => START(o, s));
            _Timer.Start();

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

                                        //MediaEncodingProfile profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD1080p);
                                        //await _mediaCapture.StartRecordToStorageFileAsync(profile, videoFile);
                                        CameraCaptureUI captureUI = new CameraCaptureUI();
                                        captureUI.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;

                                        videoFile = captureUI.CaptureFileAsync(CameraCaptureUIMode.Video);

                                        if (videoFile == null)
                                        {
                                            // User cancelled photo capture
                                            return;
                                        }

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



    }
}
