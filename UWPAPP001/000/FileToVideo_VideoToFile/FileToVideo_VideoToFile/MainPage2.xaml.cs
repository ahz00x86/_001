using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Editing;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
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
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
//using ZXing;
//using ZXing.Common;
//using ZXing.QrCode;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace FileToVideo_VideoToFile
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage2 : Page
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        String actualFileName = String.Empty;        
        Int32 iComposition = 0;
        //Int64 iCompositionEnded = 0;
        private readonly SemaphoreSlim mutex = new SemaphoreSlim(1);
        //List<StorageFile> files = new List<StorageFile>();
        String sBytes = String.Empty;        
        Int32 i = 1;
        String gFileName = "erroR.np";
        Boolean isError = false;
        Boolean wait = false;
        Boolean? fileFound = null;
        //private static readonly List<BarcodeFormat> Fmts = new List<BarcodeFormat> { BarcodeFormat.DATA_MATRIX };
        Int32 iMedia3 = -1;
        Int32 iMediaAnt = 1;
        Int32 incremento = 1;
        //long multiplicador = 1;

        TimeSpan actualPosition = new TimeSpan(0,0,0,0,1);
                
        public MainPage2()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
            
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock initTB = new TextBlock();
            initTB.Text = "EN > Experimental application, do not rely on a future favorable decoding to save your files in this format, the application may contain errors. The app uses a separate coding standard. For any questions you can refer to us on apzyxGames.azurewebsites.net \r\n\r\nES > Aplicación experimental, no confíe en una futura decodificación favorable para guardar sus archivos en este formato, la aplicación puede contener errores. La aplicación usa un estándar de codificación independiente. Para cualquier consulta puede referirse a nosotros en apzyxGames.azurewebsites.net";
            initTB.Foreground = new SolidColorBrush(Colors.Red);
            initTB.FontSize = 22;
            initTB.TextWrapping = TextWrapping.Wrap;
            initTB.MaxWidth = 600;
            initTB.Margin = new Thickness(25);
            cMain.Children.Add(initTB);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            btnFileToVideo.IsEnabled = false;
            btnVideoToFile.IsEnabled = false;

            await Windows.Storage.ApplicationData.Current.ClearAsync(ApplicationDataLocality.Temporary);

            btnFileToVideo.IsEnabled = true;
            btnVideoToFile.IsEnabled = true;
        }

        public static String convert(byte b)
        {
            StringBuilder str = new StringBuilder(8);
            int[] bl = new int[8];

            for (int i = 0; i < bl.Length; i++)
            {
                bl[bl.Length - 1 - i] = ((b & (1 << i)) != 0) ? 1 : 0;
            }

            foreach (int num in bl) str.Append(num);

            return str.ToString();
        }

        private async void FileToVideo_Click(object sender, RoutedEventArgs e)
        {
            disableControls();
            cMain.Children.Clear();
            iComposition = 0;
            //btnSaveVideo.Visibility = Visibility.Collapsed;
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add("*");
            Windows.Storage.StorageFile fileP = await picker.PickSingleFileAsync();

            if (fileP != null)
            {              

                this.textBlock.Text = "Processing " + fileP.Name + " ... ";
                //Stream s = await fileP.OpenStreamForReadAsync();
                //Convert.ToBase64String
                //Convert.FromBase64String

                List<String> datalS = new List<String>();

                //IBuffer buffer = await FileIO.ReadBufferAsync(fileP);              

                //// Use a dataReader object to read from the buffer
                //using (DataReader dataReader = DataReader.FromBuffer(buffer))
                //{
                IBuffer buffer = await FileIO.ReadBufferAsync(fileP);
                Byte[] bytes = buffer.ToArray();

                //String temp = await apzAlgPrueba03(bytes);
                //String tempS = await apzAlgPrueba04(bytes);

                //String tempS = await apzAlgPrueba04(bytes);

                //Byte[] bTemp = bytes;
                //Byte[] bAnt = bTemp;
                //while (true)
                //{
                //    bAnt = bTemp;
                //    String tempS = await apzAlgPrueba04(bTemp);
                //    if(String.IsNullOrEmpty(tempS))
                //    {
                //        break;
                //    }
                //    else
                //    {
                //        bTemp = apzCode.GetBytesFromBinaryString(tempS);
                //    }
                //}


                // INI apz algorith compressor

                //String binaryString = await getStringFromByteArray(bytes);
                //String tempS = await apzAlgPrueba02(binaryString);

                //BitArray bA = new BitArray(bytes);
                //String tempS = await apzAlgPrueba02(bA);
                //////String binarioSinComprimir = String.Empty;
                //////foreach (Boolean b in bA)
                //////{
                //////    //binarioSinComprimir += Convert.ToString(bytes[iB], 2).PadLeft(8, '0');
                //////    binarioSinComprimir += b ? "1" : "0";
                //////}

                //Byte[] initBytes = bytes.SubArray(0, 2);
                //Byte[] initBytes2 = bytes.SubArray(0, 2);

                //if (initBytes.SequenceEqual(initBytes2))
                //{
                //    String hola = "hola";
                //}

                //String tempS = await getStringFromByteArray(bytes);                
                //String antS = tempS;
                //while (!String.IsNullOrEmpty(tempS))
                //{
                //    antS = tempS;
                //    tempS = await apzAlgPrueba04(tempS);
                //}
                String sS = await getStringFromByteArray(bytes);
                ////Boolean[] dataBool = await getBoolArrayFromByteArray(bytes, false);                
                //String sp05 = await apzAlgPrueba06(sS);
                String s = await HuffmanConDiccionarioIntegrado(sS);

                String s2 = await HuffmanConDiccionarioIntegrado(s);
                String stop = "";

                //String s02 = await apzHuffmanCompressor(binarioSinComprimir);



                //String apzAlgBinCompr = await apzAlgorithmCompressor01(bA);
                //String DECO = await apzAlgorithmDecompressor04(apzAlgBinCompr);                
                //Boolean[] bools2 = apzAlgBinCompr.Select(x => x == '1' ? true : false).ToArray();
                //BitArray bA2 = new BitArray(bools2);
                //String apzAlgBinCompr2 = await apzAlgorithmCompressor01(bA2);
                //Boolean[] bools3 = apzAlgBinCompr2.Select(x => x == '1' ? true : false).ToArray();
                //BitArray bA3 = new BitArray(bools3);
                //String resultCOM = await apzAlgorithmCompressor01(bA3);                
                //String apzAlgBinDEC0 = await apzAlgorithmDecompressor01(resultCOM);
                //String apzAlgBinDEC1 = await apzAlgorithmDecompressor01(apzAlgBinDEC0);
                //String apzAlgBinDEC2 = await apzAlgorithmDecompressor01(apzAlgBinDEC1);

                //int numOfBytes = apzAlgBinCompr.Length / 8;
                //byte[] dataBytes = new byte[numOfBytes];
                //for (int i = 0; i < numOfBytes; ++i)
                //{
                //    dataBytes[i] = Convert.ToByte(apzAlgBinCompr.Substring(8 * i, 8), 2);
                //}

                //var savePickerTEMP = new Windows.Storage.Pickers.FileSavePicker();
                //savePickerTEMP.SuggestedStartLocation =
                //    Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
                //// Default file name if the user does not type one in or select a file to replace
                //savePickerTEMP.SuggestedFileName = "VideoToFile";

                //savePickerTEMP.FileTypeChoices.Add("apz File", new List<string>() { ".apz" });
                //Windows.Storage.StorageFile fileS = await savePickerTEMP.PickSaveFileAsync();

                //if (fileS != null)
                //{
                //    await FileIO.WriteBytesAsync(fileS, dataBytes);
                //}
                //else
                //{
                //    this.textBlock.Text = "Operation cancelled.";
                //}

                // END apz algorith compressor


                if (bytes.Length < 15677733)
                {
                    string text = Convert.ToBase64String(bytes);
                    //String text = dataReader.ReadString(buffer.Length);

                    Int32 iFinal = 0;
                    for (Int32 i1000 = 0; i1000 <= text.Length - 1000;)
                    {
                        datalS.Add(text.Substring(i1000, 1000));
                        i1000 += 1000;
                        iFinal = i1000;
                    }

                    datalS.Add(text.Substring(iFinal));

                    //}



                    //using (DataReader dataReader = DataReader.FromBuffer(buffer))
                    //{
                    //string fileContent = dataReader.ReadString(buffer.Length);



                    //}

                    MediaComposition composition = new MediaComposition();
                    //StorageFile imgInit = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/GoldenWay.jpg"));
                    //var clipInit = await MediaClip.CreateFromImageFileAsync(imgInit, TimeSpan.FromMilliseconds(3));
                    //composition.Clips.Add(clipInit);
                    var encoding = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD1080p);

                    Int32 i = 1;
                    Int32 iMax = datalS.Count;
                    foreach (String sData in datalS)
                    {
                        textBlock.Text = string.Format("Processing Clips Progress: {0:F0} / " + iMax, i);

                        //QrCodeEncodingOptions options = new QrCodeEncodingOptions();
                        //options.ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H;
                        //options.Height = 1000;
                        //options.Width = 1000;
                        //options.Margin = 2;

                        //var write = new BarcodeWriter();
                        //write.Options = options;
                        //write.Format = ZXing.BarcodeFormat.DATA_MATRIX;
                        String[] extension = fileP.Name.Split('.');
                        var wb = await apzCode.Generate("{\"t\": \"." + extension[1] + "\", \"i\": \"" + i.ToString() + "\", \"iMax\": \"" + iMax.ToString() + "\", \"d\": \"" + sData + "\"}", null);

                        var fileQR = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileP.Name, CreationCollisionOption.GenerateUniqueName);
                        using (IRandomAccessStream stream = await fileQR.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                            Stream pixelStream = wb.PixelBuffer.AsStream();
                            byte[] pixels = new byte[pixelStream.Length];
                            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                            encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Ignore,
                                                (uint)wb.PixelWidth,
                                                (uint)wb.PixelHeight,
                                                96.0,
                                                96.0,
                                                pixels);
                            await encoder.FlushAsync();
                        }

                        ImageBrush iBrush = new ImageBrush();
                        iBrush.ImageSource = wb;
                        cMain.Background = iBrush;

                        var clip = await MediaClip.CreateFromImageFileAsync(fileQR, TimeSpan.FromMilliseconds(100));
                        composition.Clips.Add(clip);
                        i++;
                    }

                    //var clipEnd = await MediaClip.CreateFromImageFileAsync(imgInit, TimeSpan.FromMilliseconds(3));
                    //composition.Clips.Add(clipEnd);

                    var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                    savePicker.SuggestedStartLocation =
                        Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
                    // Dropdown of file types the user can save the file as
                    savePicker.FileTypeChoices.Add("MP4 files", new List<string>() { ".mp4" });
                    // Default file name if the user does not type one in or select a file to replace
                    savePicker.SuggestedFileName = "FileToVideo.mp4";

                    Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

                    if (file != null)
                    {
                        cMain.Background = new SolidColorBrush(Colors.Black);

                        TextBlock initTB = new TextBlock();
                        initTB.Text = "EN > Saving file, please wait \r\n\r\nES > Guardando el archivo, por favor espere";
                        initTB.Foreground = new SolidColorBrush(Colors.Red);
                        initTB.FontSize = 22;
                        initTB.TextWrapping = TextWrapping.Wrap;
                        initTB.MaxWidth = 600;
                        initTB.Margin = new Thickness(25);
                        cMain.Children.Add(initTB);

                        this.textBlock.Text = "Saving " + file.Name + " ... ";
                        await Task.Delay(30);

                        var saveOperation = composition.RenderToFileAsync(file, MediaTrimmingPreference.Precise, encoding);

                        saveOperation.Progress = new AsyncOperationProgressHandler<TranscodeFailureReason, double>(async (info, progress) =>
                        {
                            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                            {
                                textBlock.Text = string.Format("Saving " + file.Name + " ... Progress: {0:F0} %", progress);
                            }));
                        });

                        saveOperation.Completed = new AsyncOperationWithProgressCompletedHandler<TranscodeFailureReason, double>(async (info, statusO) =>
                        {
                            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                            {
                                try
                                {
                                    var results = info.GetResults();
                                    if (results != TranscodeFailureReason.None || statusO != AsyncStatus.Completed)
                                    {
                                        this.textBlock.Text = "ERROR saving file " + file.Name;
                                    }
                                    else
                                    {
                                        this.textBlock.Text = "File " + file.Name + " was saved.";

                                        cMain.Children.Clear();
                                        TextBlock initTB2 = new TextBlock();
                                        initTB2.Text = "EN > File was saved \r\n\r\nES > Se guardó el archivo";
                                        initTB2.Foreground = new SolidColorBrush(Colors.Red);
                                        initTB2.FontSize = 22;
                                        initTB2.TextWrapping = TextWrapping.Wrap;
                                        initTB2.MaxWidth = 600;
                                        initTB2.Margin = new Thickness(25);
                                        cMain.Children.Add(initTB2);
                                    }
                                }
                                finally
                                {
                                    // Update UI whether the operation succeeded or not
                                    enableControls();
                                }

                            }));
                        });
                    }
                    else
                    {
                        this.textBlock.Text = "Operation cancelled.";
                    }
                }
                else
                {
                    this.textBlock.Text = "Files larger than 10 mb are not allowed";
                }

            }
            else
            {
                this.textBlock.Text = "Operation cancelled.";
            }

            enableControls();

        }

        private async void ImagesToVideo_Click(object sender, RoutedEventArgs e)
        {
            disableControls();
            cMain.Children.Clear();
            iComposition = 0;
            //btnSaveVideo.Visibility = Visibility.Collapsed;
            var picker = new Windows.Storage.Pickers.FolderPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            //picker.FileTypeFilter.Add("*");
            Windows.Storage.StorageFolder folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {

                this.textBlock.Text = "Processing " + folder.Name + " ... ";
                //Stream s = await fileP.OpenStreamForReadAsync();
                //Convert.ToBase64String
                //Convert.FromBase64String

                //////List<String> datalS = new List<String>();

                ////////IBuffer buffer = await FileIO.ReadBufferAsync(fileP);              

                ////////// Use a dataReader object to read from the buffer
                ////////using (DataReader dataReader = DataReader.FromBuffer(buffer))
                ////////{
                //////IBuffer buffer = await FileIO.ReadBufferAsync(fileP);
                //////Byte[] bytes = buffer.ToArray();
                //////if (bytes.Length < 15677733)
                //////{
                //////    string text = Convert.ToBase64String(bytes);
                //////    //String text = dataReader.ReadString(buffer.Length);

                //////    Int32 iFinal = -1;
                //////    for (Int32 i1000 = 0; i1000 <= text.Length - 1000;)
                //////    {
                //////        datalS.Add(text.Substring(i1000, 1000));
                //////        i1000 += 1000;
                //////        iFinal = i1000;
                //////    }

                //////    datalS.Add(text.Substring(iFinal));

                    //}



                    //using (DataReader dataReader = DataReader.FromBuffer(buffer))
                    //{
                    //string fileContent = dataReader.ReadString(buffer.Length);



                    //}

                    MediaComposition composition = new MediaComposition();
                    //StorageFile imgInit = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/GoldenWay.jpg"));
                    //var clipInit = await MediaClip.CreateFromImageFileAsync(imgInit, TimeSpan.FromMilliseconds(3));
                    //composition.Clips.Add(clipInit);
                    var encoding = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD1080p);

                    var files = await folder.GetFilesAsync();

                    Int32 i = 1;
                    Int32 iMax = files.Count;
                    foreach (StorageFile fileP in files)
                    {
                        textBlock.Text = string.Format("Processing Clips Progress: {0:F0} / " + iMax, i);

                        var clip = await MediaClip.CreateFromImageFileAsync(fileP, TimeSpan.FromMilliseconds(33));
                        composition.Clips.Add(clip);
                        i++;
                    }

                var pickerMusic = new Windows.Storage.Pickers.FileOpenPicker();
                pickerMusic.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                pickerMusic.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
                pickerMusic.FileTypeFilter.Add("*");
                Windows.Storage.StorageFile mp3 = await pickerMusic.PickSingleFileAsync();
                                
                composition.BackgroundAudioTracks.Add(await BackgroundAudioTrack.CreateFromFileAsync(mp3));

                    //var clipEnd = await MediaClip.CreateFromImageFileAsync(imgInit, TimeSpan.FromMilliseconds(3));
                    //composition.Clips.Add(clipEnd);

                    var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                    savePicker.SuggestedStartLocation =
                        Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
                    // Dropdown of file types the user can save the file as
                    savePicker.FileTypeChoices.Add("MP4 files", new List<string>() { ".mp4" });
                    // Default file name if the user does not type one in or select a file to replace
                    savePicker.SuggestedFileName = "FileToVideo.mp4";

                    Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

                    if (file != null)
                    {
                        cMain.Background = new SolidColorBrush(Colors.Black);

                        TextBlock initTB = new TextBlock();
                        initTB.Text = "EN > Saving file, please wait \r\n\r\nES > Guardando el archivo, por favor espere";
                        initTB.Foreground = new SolidColorBrush(Colors.Red);
                        initTB.FontSize = 22;
                        initTB.TextWrapping = TextWrapping.Wrap;
                        initTB.MaxWidth = 600;
                        initTB.Margin = new Thickness(25);
                        cMain.Children.Add(initTB);

                        this.textBlock.Text = "Saving " + file.Name + " ... ";
                        await Task.Delay(30);

                        var saveOperation = composition.RenderToFileAsync(file, MediaTrimmingPreference.Precise, encoding);

                        saveOperation.Progress = new AsyncOperationProgressHandler<TranscodeFailureReason, double>(async (info, progress) =>
                        {
                            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                            {
                                textBlock.Text = string.Format("Saving " + file.Name + " ... Progress: {0:F0} %", progress);
                            }));
                        });

                        saveOperation.Completed = new AsyncOperationWithProgressCompletedHandler<TranscodeFailureReason, double>(async (info, statusO) =>
                        {
                            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                            {
                                try
                                {
                                    var results = info.GetResults();
                                    if (results != TranscodeFailureReason.None || statusO != AsyncStatus.Completed)
                                    {
                                        this.textBlock.Text = "ERROR saving file " + file.Name;
                                    }
                                    else
                                    {
                                        this.textBlock.Text = "File " + file.Name + " was saved.";

                                        cMain.Children.Clear();
                                        TextBlock initTB2 = new TextBlock();
                                        initTB2.Text = "EN > File was saved \r\n\r\nES > Se guardó el archivo";
                                        initTB2.Foreground = new SolidColorBrush(Colors.Red);
                                        initTB2.FontSize = 22;
                                        initTB2.TextWrapping = TextWrapping.Wrap;
                                        initTB2.MaxWidth = 600;
                                        initTB2.Margin = new Thickness(25);
                                        cMain.Children.Add(initTB2);
                                    }
                                }
                                finally
                                {
                                    // Update UI whether the operation succeeded or not
                                    enableControls();
                                }

                            }));
                        });
                    }
                    else
                    {
                        this.textBlock.Text = "Operation cancelled.";
                    }
                }
                else
                {
                    this.textBlock.Text = "Files larger than 10 mb are not allowed";
                }

            //////}
            //////else
            //////{
            //////    this.textBlock.Text = "Operation cancelled.";
            //////}

            enableControls();

        }


        private async void VideoToFile_Click(object sender, RoutedEventArgs e)
        {
            disableControls();
            iMedia3 = -1;
            iMediaAnt = 1;
            //incremento = 1;
            //multiplicador = 1;
            actualPosition = new TimeSpan(0, 0, 0, 0, 1);

            cMain.Children.Clear();
            fileFound = false;
            sBytes = String.Empty;
            isError = false;
            //files = new List<StorageFile>();
            iComposition = 0;
            i = 1;
            //btnSaveVideo.Visibility = Visibility.Collapsed;
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;            
            picker.FileTypeFilter.Add(".mp4");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                Stream s = await file.OpenStreamForReadAsync();
                textBlock.Text = "Processing video " + file.Name + " ... ";
                actualFileName = file.DisplayName;
                                
                //MediaPlaybackItem moviePlaybackItem =
                //    new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));
                //MediaBreak preRollMediaBreak = new MediaBreak(MediaBreakInsertionMethod.Interrupt);
                //MediaPlaybackItem prerollAd =
                //    new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));
                //prerollAd.CanSkip = false;
                //preRollMediaBreak.PlaybackList.Items.Add(prerollAd);

                //moviePlaybackItem.BreakSchedule.PrerollBreak = preRollMediaBreak;

                //StorageFolder videosFolder = KnownFolders.VideosLibrary;
                //StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
                //await vtfFolder.CreateFolderAsync(actualFileName, CreationCollisionOption.FailIfExists);
                mediaPlayer = new MediaPlayer();
                mediaPlayer.PlaybackSession.NaturalDurationChanged += PlaybackSession_NaturalDurationChanged;
                mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
                //mediaPlayer.MediaPlayer.BufferingEnded += MediaPlayer_BufferingEnded;
                //mediaPlayer.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
                mediaPlayer.SeekCompleted += MediaPlayer_SeekCompleted;
                //mediaPlayer.Source = moviePlaybackItem;
                //_mediaPlayerElement.SetMediaPlayer(mediaPlayer);
                //mediaPlayer.MediaPlayer.VideoFrameAvailable += MediaPlayer_VideoFrameAvailable;                
                //mediaPlayer.MediaPlayer.IsVideoFrameServerEnabled = false;
                //mediaPlayer.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
                //mediaPlayer.MediaPlayer.BufferingEnded += MediaPlayer_BufferingEnded;
                //mediaPlayer.MediaPlayer.PlaybackSession.PlaybackRate = 0.5;                
                //mediaPlayer.MediaPlayer.StepForwardOneFrame();
                //mediaPlayer.MediaPlayer.PlaybackSession.Position = mediaPlayer.MediaPlayer.PlaybackSession.Position + TimeSpan.FromMilliseconds(1);
                //mediaPlayer.MediaPlayer.Play();

                while (fileFound.HasValue && !fileFound.Value)
                {
                    await Task.Delay(300);
                }

                if (fileFound.HasValue && fileFound.Value)
                {
                    String hash = SHA512(sBytes);

                    if (hash != "FC33251A5ED35BEACEEB4F4828A73A13ED596D24E20CF7F9A77F668A13983FFCEA59C3937A4DD0306688CC1AA0AA006D5C688801F5F7DC189557ADA702C5FAFA")
                    {
                        Byte[] dataBytes = Convert.FromBase64String(sBytes);

                        var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                        savePicker.SuggestedStartLocation =
                            Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                        // Default file name if the user does not type one in or select a file to replace
                        savePicker.SuggestedFileName = "VideoToFile" + gFileName;

                        savePicker.FileTypeChoices.Add(gFileName, new List<string>() { gFileName });
                        Windows.Storage.StorageFile fileS = await savePicker.PickSaveFileAsync();

                        if(fileS != null)
                        { 
                            await FileIO.WriteBytesAsync(fileS, dataBytes);
                        }
                        else
                        {
                            this.textBlock.Text = "Operation cancelled.";
                        }
                    }
                    else
                    {
                        this.textBlock.Text = "apzyx Easter Egg 2023 is not that easy... keep looking... =)";
                    }
                }
            }
            else
            {
                this.textBlock.Text = "Operation cancelled.";
            }
            enableControls();
        }

        private static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        //private async void VideoToImage_Click(object sender, RoutedEventArgs e)
        //{
        //    sBytes = String.Empty;
        //    isError = false;
        //    //files = new List<StorageFile>();
        //    iComposition = 0;
        //    i = 1;
        //    //btnSaveVideo.Visibility = Visibility.Collapsed;
        //    var picker = new Windows.Storage.Pickers.FileOpenPicker();
        //    picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
        //    picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
        //    picker.FileTypeFilter.Add(".mp4");
        //    Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

        //    if (file != null)
        //    {
        //        Stream s = await file.OpenStreamForReadAsync();
        //        textBlock.Text = "Processing video " + file.Name + " ... ";
        //        actualFileName = file.DisplayName;

        //        //MediaPlaybackItem moviePlaybackItem =
        //        //    new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));
        //        //MediaBreak preRollMediaBreak = new MediaBreak(MediaBreakInsertionMethod.Interrupt);
        //        //MediaPlaybackItem prerollAd =
        //        //    new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));
        //        //prerollAd.CanSkip = false;
        //        //preRollMediaBreak.PlaybackList.Items.Add(prerollAd);

        //        //moviePlaybackItem.BreakSchedule.PrerollBreak = preRollMediaBreak;

        //        //StorageFolder videosFolder = KnownFolders.VideosLibrary;
        //        //StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
        //        //await vtfFolder.CreateFolderAsync(actualFileName, CreationCollisionOption.FailIfExists);
        //        mediaPlayer = new MediaPlayer();
        //        mediaPlayer.PlaybackSession.NaturalDurationChanged += PlaybackSession_NaturalDurationChanged;
        //        mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
        //        //mediaPlayer.MediaPlayer.BufferingEnded += MediaPlayer_BufferingEnded;
        //        //mediaPlayer.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
        //        mediaPlayer.SeekCompleted += MediaPlayer_SeekCompleted;
        //        //mediaPlayer.Source = moviePlaybackItem;
        //        //_mediaPlayerElement.SetMediaPlayer(mediaPlayer);
        //        //mediaPlayer.MediaPlayer.VideoFrameAvailable += MediaPlayer_VideoFrameAvailable;                
        //        //mediaPlayer.MediaPlayer.IsVideoFrameServerEnabled = false;
        //        //mediaPlayer.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        //        //mediaPlayer.MediaPlayer.BufferingEnded += MediaPlayer_BufferingEnded;
        //        //mediaPlayer.MediaPlayer.PlaybackSession.PlaybackRate = 0.5;                
        //        //mediaPlayer.MediaPlayer.StepForwardOneFrame();
        //        //mediaPlayer.MediaPlayer.PlaybackSession.Position = mediaPlayer.MediaPlayer.PlaybackSession.Position + TimeSpan.FromMilliseconds(1);
        //        //mediaPlayer.MediaPlayer.Play();                               

        //    }
        //    else
        //    {
        //        this.textBlock.Text = "Operation cancelled.";
        //    }
        //}


        private async void PlaybackSession_NaturalDurationChanged(MediaPlaybackSession sender, object args)
        {
            //multiplicador = 1;
            incremento = 1;
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    incremento = Convert.ToInt32(tbIncremento.Text);
            //});
            TimeSpan tS1 = new TimeSpan(0, 0, 0, 0, 1);

            Boolean buscandoOro = false;

            for (TimeSpan iP = actualPosition ; fileFound.HasValue && !fileFound.Value; iP = actualPosition)
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    textBlock.Text = string.Format("Processing Video Progress: " + iP.ToString(@"hh\:mm\:ss\.fff", null) + " / " + mediaPlayer.PlaybackSession.NaturalDuration.ToString(@"hh\:mm\:ss\.fff", null));
                });

                wait = true;

                if(iP < tS1)
                {
                    isError = true;
                    break;
                }

                if (actualPosition > mediaPlayer.PlaybackSession.NaturalDuration)
                {
                    if (!buscandoOro)
                    {
                        buscandoOro = true;
                        incremento = -1;
                        actualPosition = mediaPlayer.PlaybackSession.NaturalDuration;
                        iP = actualPosition;
                    }
                    else
                    {
                        break;
                    }
                }

                mediaPlayer.PlaybackSession.Position = iP;
                                

                while (wait)
                {
                    await Task.Delay(30);
                }

                if (!fileFound.HasValue || fileFound.Value)
                {
                    break;
                }


                //if(isError)
                //{
                //    break;
                //}


                //multiplicador++;
            }

            //wait = true;

            //mediaPlayer.PlaybackSession.Position = mediaPlayer.PlaybackSession.NaturalDuration.Subtract(new TimeSpan(0, 0, 0, 0, 1));

            //while (wait)
            //{
            //    await Task.Delay(30);
            //}

            //if (isError)
            //{
            //    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        this.textBlock.Text = "Reading error..";
            //    });
            //    fileFound = null;
            //}
            //else
            //{
                if (fileFound.HasValue && fileFound.Value)
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.textBlock.Text = "File found";
                    });
                }
                else
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.textBlock.Text = "File NOT found";
                    });
                    fileFound = null;
                }                
            //}
        }

        private async void MediaPlayer_SeekCompleted(MediaPlayer sender, object args)
        {
            CanvasDevice canvasDevice = CanvasDevice.GetSharedDevice();
            string filename = actualFileName + "_VideoToFile_" + iComposition.ToString().PadLeft(50,'0') + ".jpg";
            iComposition++;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                //await mutex.WaitAsync();
                SoftwareBitmap frameServerDest;
                CanvasBitmap canvasBitmap;

                frameServerDest = new SoftwareBitmap(BitmapPixelFormat.Rgba8, 1080, 1080, BitmapAlphaMode.Ignore);
                canvasBitmap = CanvasBitmap.CreateFromSoftwareBitmap(canvasDevice, frameServerDest);
                mediaPlayer.CopyFrameToVideoSurface(canvasBitmap);
                //SoftwareBitmap sfbm = await SoftwareBitmap.CreateCopyFromSurfaceAsync(canvasBitmap);
                
                //ZXing.MultiFormatReader barcodeReader = new MultiFormatReader();
                
                StorageFolder videosFolder = KnownFolders.VideosLibrary;
                StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
                StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync(actualFileName, CreationCollisionOption.OpenIfExists);
                var fileT = await vtfVideoImgsFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                //var fileT = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
                using (var fileStream = await fileT.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await canvasBitmap.SaveAsync(fileStream, CanvasBitmapFileFormat.Jpeg);
                }

                WriteableBitmap wb = new WriteableBitmap(1080, 1080);
                Stream sFileI = await fileT.OpenStreamForReadAsync();
                wb.SetSource(sFileI.AsRandomAccessStream());

                ImageBrush iBrush = new ImageBrush();
                iBrush.ImageSource = wb;
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    cMain.Background = iBrush;
                });
                //fileT.DeleteAsync();
                //String barcodeResult = await apzCode.Decode(wb);

                //if (barcodeResult != null)
                //{
                //    FileOutput fO = JsonConvert.DeserializeObject<FileOutput>(barcodeResult);

                //    if (i == fO.i)
                //    {
                        

                //        gFileName = fO.t;
                //        i++;

                //        sBytes += fO.d;

                //        if(fO.i == fO.iMax)
                //        {
                //            fileFound = true;
                //        }
                //    }
                //    else if (i < fO.i)
                //    {
                //        isError = true;
                //        incremento = -1;
                //        //multiplicador = 1;
                //    }
                //    else if (i > fO.i)
                //    {
                //        incremento += 1;
                //    }

                //}

                frameServerDest.Dispose();
                canvasBitmap.Dispose();
                GC.Collect();
                //iCompositionEnded++;
                actualPosition = actualPosition.Add(TimeSpan.FromMilliseconds(incremento));
                wait = false;
            });
        }

        private async void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object args)
        {
                        
        }

        private void MediaPlayer_BufferingEnded(MediaPlayer sender, object args)
        {
            
        }

        //private void MediaPlayer_BufferingEnded(MediaPlayer sender, object args)
        //{
        //    mediaPlayer.MediaPlayer.PlaybackSession.Position = mediaPlayer.MediaPlayer.PlaybackSession.Position + TimeSpan.FromMilliseconds(50);
        //}

        private async void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            //mediaPlayer = null;
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{ 
            //    btnSaveVideo.Visibility = Visibility.Visible;
            //});
        }
        
        private async void btnSaveVideo_Click(object sender, RoutedEventArgs e)
        {
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            //{            
            //btnSaveVideo.IsEnabled = false;
            btnVideoToFile.IsEnabled = false;
            btnFileToVideo.IsEnabled = false;

            //Creamos la composición con las imágenes que existan en la carpeta actual
            //StorageFolder videosFolder = KnownFolders.VideosLibrary;
            //StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
            //StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync(actualFileName, CreationCollisionOption.OpenIfExists);
            //var files = await vtfVideoImgsFolder.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByDate);                     
            //////if (files != null && files.Count > 0)
            //////{
            //////    var barcodeReader = new BarcodeReader();
            //////    List<FileOutput> lOs = new List<FileOutput>();
            //////    foreach (StorageFile fileImage in files)
            //////    {
            //////        WriteableBitmap wb = new WriteableBitmap(1000, 1000);
            //////        Stream sFileI = await fileImage.OpenStreamForReadAsync();
            //////        wb.SetSource(sFileI.AsRandomAccessStream());

            //////        // decode the barcode from the in memory bitmap
            //////        var barcodeResult = barcodeReader.Decode(wb);

            //////        if (barcodeResult != null)
            //////        {
            //////            lOs.Add(JsonConvert.DeserializeObject<FileOutput>(barcodeResult?.Text));
            //////        }
            //////    }

            //////    i = 1;
            //////    foreach (FileOutput fO in lOs)
            //////    {
            //////        if (i == fO.i)
            //////        {
            //////            fileName = fO.name;
            //////            i++;

            //////            String[] stSplit = fO.data.Split(';', StringSplitOptions.RemoveEmptyEntries);

            //////            foreach (String sByte in stSplit)
            //////            {
            //////                bytes.Add(Convert.ToByte(sByte));
            //////            }

            //////        }
            //////    }

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.FileTypeChoices.Add("Unknown", new List<string>() { "." });
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            savePicker.SuggestedFileName = gFileName;
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {

                this.textBlock.Text = "Saving " + file.Name + " ... ";

                //FileIO.WriteTextAsync(file);

                //Stream fS = await file.OpenStreamForWriteAsync();
                //using (DataWriter dW = new DataWriter(fS.AsOutputStream()))
                //{
                //    dW.WriteString(sBytes);
                //    await dW.StoreAsync();
                //    dW.DetachStream();
                //    await dW.FlushAsync();
                //}

                //this.textBlock.Text = file.Name + " be saved ";

                //////// Prevent updates to the remote version of the file until
                //////// we finish making changes and call CompleteUpdatesAsync.
                //////Windows.Storage.CachedFileManager.DeferUpdates(file);
                //////// write to file
                //////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                //////// Let Windows know that we're finished changing the file so
                //////// the other app can update the remote version of the file.
                //////// Completing updates may require Windows to ask for user input.
                //////Windows.Storage.Provider.FileUpdateStatus status =
                //////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                //////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                //////{
                    
                //////}
                //////else
                //////{
                //////    this.textBlock.Text = "File " + file.Name + " couldn't be saved.";
                //////    enableControls();
                //////}

                await FileIO.WriteBytesAsync(file, Convert.FromBase64String(sBytes));

                this.textBlock.Text = file.Name + " be saved ";
            }
            else
            {
                this.textBlock.Text = "Operation cancelled.";
                enableControls();
            }
            //////}
            //////else
            //////{
            //////    this.textBlock.Text = "Operation Error No Files To Read";
            //////    enableControls();
            //////}

            //});
        }

        private void enableControls()
        {
            //btnSaveVideo.IsEnabled = true;
            btnVideoToFile.IsEnabled = true;
            btnFileToVideo.IsEnabled = true;
        }

        private void disableControls()
        {
            //btnSaveVideo.IsEnabled = false;
            btnVideoToFile.IsEnabled = false;
            btnFileToVideo.IsEnabled = false;
        }

        private async void apzCodeGenerate_Click(object sender, RoutedEventArgs e)
        {
            await apzCode.Generate("Hola caracola", cMain);
        }

        private async void apzCodeDecode_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".jpg");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                ImageProperties properties = await file.Properties.GetImagePropertiesAsync();
                WriteableBitmap wb = new WriteableBitmap((int)properties.Width, (int)properties.Height);
                wb.SetSource(await file.OpenReadAsync());
                await apzCode.Decode(wb);
            }
        }

        private async void imgBase64_Click(object sender, RoutedEventArgs e)
        {
            String img = "00111101011111010010000010100100011100000000001001100000001000000000000101100111110111001111100101010011011011110000111111101010100010001000101110111110001111101101100001111110001010111001111110101110110110111111011010111000011101100000000100000001";

            //Byte[] bytes = Convert.FromBase64String(img);

            Byte[] bytes = apzCode.GetBytesFromBinaryString(img);

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.FileTypeChoices.Add("Unknown", new List<string>() { "." });
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            savePicker.SuggestedFileName = gFileName;
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                await FileIO.WriteBytesAsync(file, bytes);
            }
            
        }

        private async Task<String> apzAlgorithmDecompressor01(String bitString)
        {
            //Boolean[] bools = bitArrayS.Select(x => x == 1 ? true : false).ToArray();
            //BitArray bitArray = new BitArray(bools);

            String decompressorS = String.Empty;

            //String bitString = String.Empty;
            //foreach (Boolean b in bitArray)
            //{
            //    bitString += b ? "1" : "0";
            //}

            Int32 positionLength = apzCode.FromBase10(apzCode.ToBase10(bitString.Substring(0, 32), 2), 2).Length;
            Int32 lengthLength = apzCode.FromBase10(apzCode.ToBase10(bitString.Substring(32, 32), 2), 2).Length;

            Int32 i = 64;

            decompressorS += bitString[i];
            i++;

            while (i < bitString.Length)
            {
                Boolean init = bitString[i] == '1' ? true : false;
                i++;
                Int32 position = Convert.ToInt32(apzCode.ToBase10(bitString.Substring(i, positionLength), 2));
                i += positionLength;
                Int32 length = Convert.ToInt32(apzCode.ToBase10(bitString.Substring(i, lengthLength), 2)) + 1;
                i += lengthLength;
                Boolean invertido = bitString[i] == '1' ? true : false;
                i++;

                String s = init ? decompressorS.Substring(position, length) : decompressorS.Substring(decompressorS.Length - position, length);
                if (invertido)
                {
                    s = new String(s.Select(x => x == '1' ? '0' : '1').ToArray());
                }
                decompressorS += s;
            }

            String result = String.Empty;
            int numOfBytes = decompressorS.Length / 8;
            byte[] dataBytes = new byte[numOfBytes];
            for (int lI = 0; lI < numOfBytes; ++lI)
            {
                result += new String(decompressorS.Substring(8 * lI, 8).Reverse().ToArray());
            }

            return result;
        }

        private async Task<String> apzAlgorithmCompressor01(BitArray bitArray)
        {
            //Boolean[] binaryBool = binaryString.Select(x => x == '1' ? true: false).ToArray();

            String bitString = String.Empty;
            foreach (Boolean b in bitArray)
            {
                bitString += b ? "1" : "0";
            }

            String actualBits = bitArray[0] ? "1" : "0";
            String binaryCompressor = String.Empty;

            List<String[]> sinfonia = new List<String[]>();
                        
            Int32 i = 1;
            while (i < bitArray.Length)
            {
                apzAlgCompSearch selected = new apzAlgCompSearch()
                {
                    Init = false,
                    //Position = mostNearInit ? iInit : i - j, 
                    Position = -1,
                    Length = -1,
                    LengthInvertido = false,
                    BitInvertido = false
                }; ;

                //for (Int32 j = 0; j < i; j++)
                //{
                List<Task> tasks = new List<Task>();

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        sCheck += bitArray[iCheck] ? "1" : "0";
                        Int32 index = actualBits.IndexOf(sCheck);

                        if (index != -1)
                        {                            
                            Boolean mostNearInit = index < i - index ? true : false;

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,                                        
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = false,
                                        BitInvertido = false,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        // INVERTIDO
                        sCheck += bitArray[iCheck] ? "0" : "1";
                        Int32 index = actualBits.IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = false,
                                        BitInvertido = true,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                await Task.WhenAll(tasks);

                if (selected.Length != -1)
                {
                    sinfonia.Add(new string[5] 
                    { 
                        selected.Init ? "1" : "0",
                        selected.Position.ToString(),
                        selected.Length.ToString(),
                        selected.Length > 0 ? selected.LengthInvertido ? "1": "0" : "",
                        selected.BitInvertido ? "1" : "0",
                    });

                    i += selected.Length + 1;
                    actualBits += selected.BitInvertido ? new String(selected.S.Select(x => x == '1' ? '0' : '1').ToArray()) : selected.S;
                }
                else
                {
                    throw new Exception("Algo pasó");
                }

                 

            }

            Int32 maxPosition = -1;
            Int32 maxLength = -1;            
            foreach (String[] aS in sinfonia)
            {
                Int32 actualPosition = Convert.ToInt32(aS[1]);
                if (actualPosition > maxPosition)
                {                    
                    maxPosition = actualPosition;
                }

                Int32 actualLength = Convert.ToInt32(aS[2]);
                if (actualLength > maxLength)
                {
                    maxLength = actualLength;
                }
            }

            String sMaxP = apzCode.FromBase10(maxPosition.ToString(), 2);
            String sMaxL = apzCode.FromBase10(maxLength.ToString(), 2);

            // El inicio marcamos los lengths de los datos position y length según los datos que contengan y añadirmos el bit inicial
            binaryCompressor += sMaxP.PadLeft(32, '0') + sMaxL.PadLeft(32, '0') + (bitArray[0] ? "1" : "0");

            // Recorremos la sinfonia para partiturizar*
            foreach (String[] aS in sinfonia)
            {
                binaryCompressor += aS[0];
                binaryCompressor += apzCode.FromBase10(aS[1], 2).PadLeft(sMaxP.Length, '0');
                binaryCompressor += apzCode.FromBase10(aS[2], 2).PadLeft(sMaxL.Length, '0');
                binaryCompressor += aS[4];
            }


            return binaryCompressor;
        }


        private async Task<String> apzAlgorithmDecompressor02(String bitString)
        {
            //Boolean[] bools = bitArrayS.Select(x => x == 1 ? true : false).ToArray();
            //BitArray bitArray = new BitArray(bools);

            String decompressorS = String.Empty;

            //String bitString = String.Empty;
            //foreach (Boolean b in bitArray)
            //{
            //    bitString += b ? "1" : "0";
            //}

            Int32 positionLength = apzCode.FromBase10(apzCode.ToBase10(bitString.Substring(0, 32), 2), 2).Length;
            Int32 lengthLength = apzCode.FromBase10(apzCode.ToBase10(bitString.Substring(32, 32), 2), 2).Length;

            Int32 i = 64;

            decompressorS += bitString[i];
            i++;

            while (i < bitString.Length)
            {
                Boolean init = bitString[i] == '1' ? true : false;
                i++;
                Int32 position = Convert.ToInt32(apzCode.ToBase10(bitString.Substring(i, positionLength), 2));
                i += positionLength;
                Int32 length = Convert.ToInt32(apzCode.ToBase10(bitString.Substring(i, lengthLength), 2)) + 1;
                i += lengthLength;
                Boolean reverse = false;
                if (length > 1)
                {
                    reverse = bitString[i] == '1' ? true : false;
                    i++;
                }
                Boolean invertido = bitString[i] == '1' ? true : false;
                i++;

                String decompresorTEMP = decompressorS;
                if (reverse)
                {
                    decompresorTEMP = new String(decompresorTEMP.Reverse().ToArray());
                }

                String s = init ? decompresorTEMP.Substring(position, length) : decompresorTEMP.Substring(decompresorTEMP.Length - position, length);
                if (invertido)
                {
                    s = new String(s.Select(x => x == '1' ? '0' : '1').ToArray());
                }
                decompressorS += s;
            }

            String result = String.Empty;
            int numOfBytes = decompressorS.Length / 8;
            byte[] dataBytes = new byte[numOfBytes];
            for (int lI = 0; lI < numOfBytes; ++lI)
            {
                result += new String(decompressorS.Substring(8 * lI, 8).Reverse().ToArray());
            }

            return result;
        }

        private async Task<String> apzAlgorithmCompressor02(BitArray bitArray)
        {
            //Boolean[] binaryBool = binaryString.Select(x => x == '1' ? true: false).ToArray();

            String bitString = String.Empty;
            foreach (Boolean b in bitArray)
            {
                bitString += b ? "1" : "0";
            }

            String actualBits = bitArray[0] ? "1" : "0";
            String binaryCompressor = String.Empty;

            List<String[]> sinfonia = new List<String[]>();

            Int32 i = 1;
            while (i < bitArray.Length)
            {
                apzAlgCompSearch selected = new apzAlgCompSearch()
                {
                    Init = false,
                    //Position = mostNearInit ? iInit : i - j, 
                    Position = -1,
                    Length = -1,
                    LengthInvertido = false,
                    BitInvertido = false
                }; ;

                //for (Int32 j = 0; j < i; j++)
                //{
                List<Task> tasks = new List<Task>();

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        sCheck += bitArray[iCheck] ? "1" : "0";
                        Int32 index = actualBits.IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = false,
                                        BitInvertido = false,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        // INVERTIDO
                        sCheck += bitArray[iCheck] ? "0" : "1";
                        Int32 index = actualBits.IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = false,
                                        BitInvertido = true,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        sCheck += bitArray[iCheck] ? "1" : "0";
                        Int32 index = new String(actualBits.Reverse().ToArray()).IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = true,
                                        BitInvertido = false,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        // INVERTIDO
                        sCheck += bitArray[iCheck] ? "0" : "1";
                        Int32 index = new String(actualBits.Reverse().ToArray()).IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = true,
                                        BitInvertido = true,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                await Task.WhenAll(tasks);

                if (selected.Length != -1)
                {
                    sinfonia.Add(new string[5]
                    {
                        selected.Init ? "1" : "0",
                        selected.Position.ToString(),
                        selected.Length.ToString(),
                        selected.Length > 0 ? selected.LengthInvertido ? "1": "0" : "",
                        selected.BitInvertido ? "1" : "0",
                    });

                    i += selected.Length + 1;
                    actualBits += selected.BitInvertido ? new String(selected.S.Select(x => x == '1' ? '0' : '1').ToArray()) : selected.S;
                }
                else
                {
                    throw new Exception("Algo pasó");
                }



            }

            Int32 maxPosition = -1;
            Int32 maxLength = -1;
            foreach (String[] aS in sinfonia)
            {
                Int32 actualPosition = Convert.ToInt32(aS[1]);
                if (actualPosition > maxPosition)
                {
                    maxPosition = actualPosition;
                }

                Int32 actualLength = Convert.ToInt32(aS[2]);
                if (actualLength > maxLength)
                {
                    maxLength = actualLength;
                }
            }

            String sMaxP = apzCode.FromBase10(maxPosition.ToString(), 2);
            String sMaxL = apzCode.FromBase10(maxLength.ToString(), 2);

            // El inicio marcamos los lengths de los datos position y length según los datos que contengan y añadirmos el bit inicial
            binaryCompressor += sMaxP.PadLeft(32, '0') + sMaxL.PadLeft(32, '0') + (bitArray[0] ? "1" : "0");

            // Recorremos la sinfonia para partiturizar*
            foreach (String[] aS in sinfonia)
            {
                binaryCompressor += aS[0];
                binaryCompressor += apzCode.FromBase10(aS[1], 2).PadLeft(sMaxP.Length, '0');
                binaryCompressor += apzCode.FromBase10(aS[2], 2).PadLeft(sMaxL.Length, '0');
                binaryCompressor += aS[3];
                binaryCompressor += aS[4];
            }


            return binaryCompressor;
        }

        private async Task<String> apzAlgorithmDecompressor03(String bitString)
        {
            //Boolean[] bools = bitArrayS.Select(x => x == 1 ? true : false).ToArray();
            //BitArray bitArray = new BitArray(bools);

            String decompressorS = String.Empty;

            //String bitString = String.Empty;
            //foreach (Boolean b in bitArray)
            //{
            //    bitString += b ? "1" : "0";
            //}

            Int32 positionLength = apzCode.FromBase10(apzCode.ToBase10(bitString.Substring(0, 32), 2), 2).Length;
            Int32 lengthLength = apzCode.FromBase10(apzCode.ToBase10(bitString.Substring(32, 32), 2), 2).Length;

            Int32 i = 64;

            decompressorS += bitString[i];
            i++;

            while (i < bitString.Length)
            {
                Boolean init = bitString[i] == '1' ? true : false;
                i++;
                Int32 position = Convert.ToInt32(apzCode.ToBase10(bitString.Substring(i, positionLength), 2));
                i += positionLength;
                Int32 length = Convert.ToInt32(apzCode.ToBase10(bitString.Substring(i, lengthLength), 2)) + 1;
                i += lengthLength;
                Boolean reverse = false;
                if (length > 1)
                {
                    reverse = bitString[i] == '1' ? true : false;
                    i++;
                }
                Boolean invertido = bitString[i] == '1' ? true : false;
                i++;

                String decompresorTEMP = decompressorS;
                if (reverse)
                {
                    decompresorTEMP = new String(decompresorTEMP.Reverse().ToArray());
                }

                String s = init ? decompresorTEMP.Substring(position, length) : decompresorTEMP.Substring(decompresorTEMP.Length - position, length);
                if (invertido)
                {
                    s = new String(s.Select(x => x == '1' ? '0' : '1').ToArray());
                }
                decompressorS += s;
            }

            String result = String.Empty;
            int numOfBytes = decompressorS.Length / 8;
            byte[] dataBytes = new byte[numOfBytes];
            for (int lI = 0; lI < numOfBytes; ++lI)
            {
                result += new String(decompressorS.Substring(8 * lI, 8).Reverse().ToArray());
            }

            return result;
        }

        private async Task<String> apzAlgorithmCompressor03(BitArray bitArray)
        {
            //Boolean[] binaryBool = binaryString.Select(x => x == '1' ? true: false).ToArray();

            String bitString = String.Empty;
            foreach (Boolean b in bitArray)
            {
                bitString += b ? "1" : "0";
            }

            String actualBits = bitArray[0] ? "1" : "0";
            String binaryCompressor = String.Empty;

            List<String[]> sinfonia = new List<String[]>();

            Int32 i = 1;
            while (i < bitArray.Length)
            {
                apzAlgCompSearch selected = new apzAlgCompSearch()
                {
                    Init = false,
                    //Position = mostNearInit ? iInit : i - j, 
                    Position = -1,
                    Length = -1,
                    LengthInvertido = false,
                    BitInvertido = false
                }; ;

                //for (Int32 j = 0; j < i; j++)
                //{
                List<Task> tasks = new List<Task>();

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        sCheck += bitArray[iCheck] ? "1" : "0";
                        Int32 index = actualBits.IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            if (mostNearInit)
                            {
                                if(index > 4095)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (i - index > 4095)
                                {
                                    break;
                                }
                            }

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = false,
                                        BitInvertido = false,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        // INVERTIDO
                        sCheck += bitArray[iCheck] ? "0" : "1";
                        Int32 index = actualBits.IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            if (mostNearInit)
                            {
                                if (index > 4095)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (i - index > 4095)
                                {
                                    break;
                                }
                            }

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = false,
                                        BitInvertido = true,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        sCheck += bitArray[iCheck] ? "1" : "0";
                        Int32 index = new String(actualBits.Reverse().ToArray()).IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            if (mostNearInit)
                            {
                                if (index > 4095)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (i - index > 4095)
                                {
                                    break;
                                }
                            }

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = true,
                                        BitInvertido = false,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        // INVERTIDO
                        sCheck += bitArray[iCheck] ? "0" : "1";
                        Int32 index = new String(actualBits.Reverse().ToArray()).IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            if (mostNearInit)
                            {
                                if (index > 4095)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (i - index > 4095)
                                {
                                    break;
                                }
                            }

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = true,
                                        BitInvertido = true,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                await Task.WhenAll(tasks);

                if (selected.Length != -1)
                {
                    sinfonia.Add(new string[5]
                    {
                        selected.Init ? "1" : "0",
                        selected.Position.ToString(),
                        selected.Length.ToString(),
                        selected.Length > 0 ? selected.LengthInvertido ? "1": "0" : "",
                        selected.BitInvertido ? "1" : "0",
                    });

                    i += selected.Length + 1;
                    actualBits += selected.BitInvertido ? new String(selected.S.Select(x => x == '1' ? '0' : '1').ToArray()) : selected.S;
                }
                else
                {
                    throw new Exception("Algo pasó");
                }



            }

            Int32 maxPosition = -1;
            Int32 maxLength = -1;
            foreach (String[] aS in sinfonia)
            {
                Int32 actualPosition = Convert.ToInt32(aS[1]);
                if (actualPosition > maxPosition)
                {
                    maxPosition = actualPosition;
                }

                Int32 actualLength = Convert.ToInt32(aS[2]);
                if (actualLength > maxLength)
                {
                    maxLength = actualLength;
                }
            }

            String sMaxP = apzCode.FromBase10(maxPosition.ToString(), 2);
            String sMaxL = apzCode.FromBase10(maxLength.ToString(), 2);

            // El inicio marcamos los lengths de los datos position y length según los datos que contengan y añadirmos el bit inicial
            binaryCompressor += sMaxP.PadLeft(32, '0') + sMaxL.PadLeft(32, '0') + (bitArray[0] ? "1" : "0");

            // Recorremos la sinfonia para partiturizar*
            foreach (String[] aS in sinfonia)
            {
                binaryCompressor += aS[0];
                binaryCompressor += apzCode.FromBase10(aS[1], 2).PadLeft(sMaxP.Length, '0');
                binaryCompressor += apzCode.FromBase10(aS[2], 2).PadLeft(sMaxL.Length, '0');
                binaryCompressor += aS[3];
                binaryCompressor += aS[4];
            }


            return binaryCompressor;
        }

        private async Task<String> apzAlgorithmDecompressor04(String bitString)
        {
            //Boolean[] bools = bitArrayS.Select(x => x == 1 ? true : false).ToArray();
            //BitArray bitArray = new BitArray(bools);

            String decompressorS = String.Empty;

            //String bitString = String.Empty;
            //foreach (Boolean b in bitArray)
            //{
            //    bitString += b ? "1" : "0";
            //}

            Int32 positionLength = apzCode.FromBase10(apzCode.ToBase10(bitString.Substring(0, 32), 2), 2).Length;
            Int32 lengthLength = apzCode.FromBase10(apzCode.ToBase10(bitString.Substring(32, 32), 2), 2).Length;

            Int32 i = 64;

            decompressorS += bitString[i];
            i++;

            Int32 lPostionCreciente = 1;
            while (i < bitString.Length)
            {
                Boolean init = bitString[i] == '1' ? true : false;
                i++;
                if (lPostionCreciente < positionLength)
                {
                    Int32 creceX = Convert.ToInt32(apzCode.ToBase10(bitString.Substring(i, 2), 2));
                    lPostionCreciente += creceX;
                    i += 2;
                }
                Int32 position = Convert.ToInt32(apzCode.ToBase10(bitString.Substring(i, lPostionCreciente), 2));
                i += lPostionCreciente;
                Int32 length = Convert.ToInt32(apzCode.ToBase10(bitString.Substring(i, lengthLength), 2)) + 1;
                i += lengthLength;
                Boolean invertido = bitString[i] == '1' ? true : false;
                i++;

                String s = init ? decompressorS.Substring(position, length) : decompressorS.Substring(decompressorS.Length - position, length);
                if (invertido)
                {
                    s = new String(s.Select(x => x == '1' ? '0' : '1').ToArray());
                }
                decompressorS += s;
            }

            String result = String.Empty;
            int numOfBytes = decompressorS.Length / 8;
            byte[] dataBytes = new byte[numOfBytes];
            for (int lI = 0; lI < numOfBytes; ++lI)
            {
                result += new String(decompressorS.Substring(8 * lI, 8).Reverse().ToArray());
            }

            return result;
        }

        private async Task<String> apzAlgorithmCompressor04(BitArray bitArray)
        {
            //Boolean[] binaryBool = binaryString.Select(x => x == '1' ? true: false).ToArray();

            String bitString = String.Empty;
            foreach (Boolean b in bitArray)
            {
                bitString += b ? "1" : "0";
            }

            String actualBits = bitArray[0] ? "1" : "0";
            String binaryCompressor = String.Empty;

            List<String[]> sinfonia = new List<String[]>();

            Int32 i = 1;
            while (i < bitArray.Length)
            {
                apzAlgCompSearch selected = new apzAlgCompSearch()
                {
                    Init = false,
                    //Position = mostNearInit ? iInit : i - j, 
                    Position = -1,
                    Length = -1,
                    LengthInvertido = false,
                    BitInvertido = false
                }; ;

                //for (Int32 j = 0; j < i; j++)
                //{
                List<Task> tasks = new List<Task>();

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        sCheck += bitArray[iCheck] ? "1" : "0";
                        Int32 index = actualBits.IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = false,
                                        BitInvertido = false,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                tasks.Add(Task.Run(() =>
                {
                    String sCheck = String.Empty;
                    for (Int32 iCheck = i; iCheck < bitArray.Length; iCheck++)
                    {
                        // INVERTIDO
                        sCheck += bitArray[iCheck] ? "0" : "1";
                        Int32 index = actualBits.IndexOf(sCheck);

                        if (index != -1)
                        {
                            Boolean mostNearInit = index < i - index ? true : false;

                            lock (selected)
                            {
                                if (selected.Length < sCheck.Length)
                                {
                                    selected = new apzAlgCompSearch()
                                    {
                                        Init = mostNearInit,
                                        Position = mostNearInit ? index : i - index,
                                        Length = sCheck.Length - 1,
                                        LengthInvertido = false,
                                        BitInvertido = true,
                                        S = sCheck
                                    };
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }));

                await Task.WhenAll(tasks);

                if (selected.Length != -1)
                {
                    sinfonia.Add(new string[5]
                    {
                        selected.Init ? "1" : "0",
                        selected.Position.ToString(),
                        selected.Length.ToString(),
                        selected.Length > 0 ? selected.LengthInvertido ? "1": "0" : "",
                        selected.BitInvertido ? "1" : "0",
                    });

                    i += selected.Length + 1;
                    actualBits += selected.BitInvertido ? new String(selected.S.Select(x => x == '1' ? '0' : '1').ToArray()) : selected.S;
                }
                else
                {
                    throw new Exception("Algo pasó");
                }



            }

            Int32 maxPosition = -1;
            Int32 maxLength = -1;
            foreach (String[] aS in sinfonia)
            {
                Int32 actualPosition = Convert.ToInt32(aS[1]);
                if (actualPosition > maxPosition)
                {
                    maxPosition = actualPosition;
                }

                Int32 actualLength = Convert.ToInt32(aS[2]);
                if (actualLength > maxLength)
                {
                    maxLength = actualLength;
                }
            }

            String sMaxP = apzCode.FromBase10(maxPosition.ToString(), 2);
            String sMaxL = apzCode.FromBase10(maxLength.ToString(), 2);

            // El inicio marcamos los lengths de los datos position y length según los datos que contengan y añadirmos el bit inicial
            binaryCompressor += sMaxP.PadLeft(32, '0') + sMaxL.PadLeft(32, '0') + (bitArray[0] ? "1" : "0");

            Int32 lPostionIter = 1;

            // Recorremos la sinfonia para partiturizar*
            foreach (String[] aS in sinfonia)
            {
                binaryCompressor += aS[0];
                Int32 tempLpostIter = apzCode.FromBase10(aS[1], 2).Length - lPostionIter;
                if (lPostionIter < sMaxP.Length)
                {
                    binaryCompressor += apzCode.FromBase10((tempLpostIter > 0 ? tempLpostIter : 0).ToString(), 2).PadLeft(2, '0');
                }
                lPostionIter += tempLpostIter > 0 ? tempLpostIter : 0;
                binaryCompressor += apzCode.FromBase10(aS[1], 2).PadLeft(lPostionIter, '0');
                binaryCompressor += apzCode.FromBase10(aS[2], 2).PadLeft(sMaxL.Length, '0');
                binaryCompressor += aS[4];
            }


            return binaryCompressor;
        }


        //////private async Task<String> apzHuffmanCompressor(String binartyString)
        //////{
        //////    String sResult = String.Empty;

        //////    Dictionary<Int32, List<HuffmanSearch>> huffmanDictionary = new Dictionary<Int32, List<HuffmanSearch>>();
        //////    for (Int32 k = 0; k < 8; k++)
        //////    {
        //////        huffmanDictionary.Add(k, new List<HuffmanSearch>());
        //////    }

        //////    int numOfBytes = binartyString.Length / 8;
        //////    for (Int32 k = 0; k < 8; k++)
        //////    {
        //////            for (int i = 0; i < numOfBytes - 8; ++i)
        //////            {
        //////                String s8 = binartyString.Substring((8 * i) + k, 8);

        //////                List<HuffmanSearch> lista = huffmanDictionary[k];
        //////                HuffmanSearch hS = lista.Where(x => x.S01 == s8).SingleOrDefault();
        //////                if(hS != null)
        //////                {
        //////                    hS.Count++;
        //////                }
        //////                else
        //////                {
        //////                    lista.Add(new HuffmanSearch() { S01 = s8, Count = 1 });
        //////                }
        //////            }
        //////    }

        //////    KeyValuePair<Int32,List<HuffmanSearch>> listaConMasRepeticionesMenosDatos = huffmanDictionary.OrderBy(x => x.Value.Count).Select(x => x).FirstOrDefault();

        //////    Int32 offset = listaConMasRepeticionesMenosDatos.Key;

        //////    List<HuffmanSearch> listaMenorAMayorRepeticion = listaConMasRepeticionesMenosDatos.Value.OrderBy(x => x.Count).ToList();
                                    
        //////    while (listaMenorAMayorRepeticion.Count > 0)
        //////    {
        //////        HuffmanSearch hfS0 = listaMenorAMayorRepeticion[0];
        //////        HuffmanSearch hfS1 = null;
        //////        if (listaMenorAMayorRepeticion.Count > 1)
        //////        {
        //////            hfS1 = listaMenorAMayorRepeticion[1];
        //////        }

        //////        //NodoArbolHuffman n0 = new NodoArbolHuffman() { Izquierdo = }

        //////    }


        //////    return sResult;
        //////}

        /// <summary>
        /// Algoritmo experimental de compresión made in apzyx
        /// </summary>
        private async Task<String> apzAlgPrueba01(String binaryString)
        {
            // TODO: Mejorar añadiendo datos irrelevantes al final para conseguir un número bsLength con mayores factores, solución al problema derivado,
            // de conjuntos de datos con un bsLength primo, para poder aplicar el algoritmo en cualquier caso

            // Resultado: Datos comprimidos
            String sResult = String.Empty;

            // Obtenemos el tamaño del binario que queremos comprimir
            Int32 bsLength = binaryString.Length;

            // Obtenemos los factores primos de este tamaño, deberemos obtener siempre un mínimo de 2^3 que es 8 siempre y cuando sean archivos codificados en bytes
            List<Int32> factores = obtenerFactoresPrimos(bsLength);

            // Obtenemos todas las combinaciones posibles entre esos factores
            List<IEnumerable<Int32>> combinaciones = GetPowerSet<Int32>(factores);

            List<Int32> combinacionesCalculadas = new List<Int32>();
            foreach (IEnumerable<Int32> nS in combinaciones)
            {
                // Hacemos el cálculo de cada combinación
                Int32 cC = 1;
                foreach (Int32 n in nS)
                {
                    cC *= n;
                }

                // Solo introducimos las que son 1 Bytes o mayores
                if(cC > 7 && cC < bsLength)
                {
                    // Y no aceptamos valores repetidos
                    if (!combinacionesCalculadas.Contains(cC))
                    {
                        combinacionesCalculadas.Add(cC);
                    }
                }
            }

            // Ordenamos las combinaciones de mayor a menor
            combinacionesCalculadas = combinacionesCalculadas.OrderByDescending(x => x).ToList();

            Dictionary<Int32,apzExperimentalCode> repeticionesEnFuncionALasCCs = new Dictionary<Int32, apzExperimentalCode>();

            // Ahora con esas combinaciones creamos los conjuntos de datos aceptados por el conjunto inicial
            // Para ello,
            // Recorremos cada combinación y creamos las estructuras para contar sus repeticiones en los datos originales
            foreach (Int32 cC in combinacionesCalculadas)
            {
                // Si el proceso anterior es correcto esta división debe de ser siempre exacta
                Int32 nLbS = binaryString.Length / cC;

                // Inicializamos para cada cC la lista de repeticiones
                repeticionesEnFuncionALasCCs.Add(cC, new apzExperimentalCode() { lbS = nLbS, apzExpCodSearches = new List<apzExpCodSearch>() });

                // Hacemos un barrido para esta combinación
                for (Int32 i = 0; i < nLbS; i++)
                {
                    // Comprobamos y anotamos las repeticiones

                    String s02 = binaryString.Substring(i * cC, cC);

                    apzExperimentalCode listaRepeticionesDeCcConcreto = repeticionesEnFuncionALasCCs[cC];

                    apzExpCodSearch hS = listaRepeticionesDeCcConcreto.apzExpCodSearches.Where( x => x.S01 == s02).SingleOrDefault();

                    if(hS != null )
                    {
                        hS.Where.Add(i);
                    }
                    else
                    {
                        hS = new apzExpCodSearch() { S01 = s02, Where = new List<Int32>() { i } };
                        listaRepeticionesDeCcConcreto.apzExpCodSearches.Add(hS);
                    }
                }
                                
                // Quitamos los parameters que no tengan repeticiones
                if (repeticionesEnFuncionALasCCs[cC].apzExpCodSearches.Count == nLbS)
                {
                    repeticionesEnFuncionALasCCs.Remove(cC);
                }
            }

            //StringBuilder sB = new StringBuilder();

            //sB.AppendLine("k".PadLeft(5, ' ') + " " + "lbS".PadLeft(5, ' ') + " " + "CountL".PadLeft(5, ' '));

            //foreach (Int32 k in repeticionesEnFuncionALasCCs.Keys)
            //{
            //    apzExperimentalCode apzEC = repeticionesEnFuncionALasCCs[k];

            //    sB.AppendLine(k.ToString().PadLeft(5, ' ') + " " + apzEC.lbS.ToString().PadLeft(5, ' ') + " " + apzEC.apzExpCodSearches.Count.ToString().PadLeft(5, ' '));
            //}

            //FileSavePicker picker = new FileSavePicker();
            //picker.FileTypeChoices.Add(".txt", new string[] { ".txt" });
            //picker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            //picker.SuggestedFileName = "output.txt";
            //StorageFile file = await picker.PickSaveFileAsync();
            //if (file != null)
            //{
            //    await FileIO.WriteTextAsync(file, sB.ToString());
            //}

            List<String> resultados = new List<String>();

            // Hacemos una probatura para todos los datos resultantes
            foreach (Int32 k in repeticionesEnFuncionALasCCs.Keys)
            {
                // Establecemos la cabecera con los datos del fichero y la "k" o tamaño de bloque
                String rTempString = apzCode.FromBase10(bsLength.ToString(),2).PadLeft(32, '0') + apzCode.FromBase10(k.ToString(), 2).PadLeft(16, '0');
                apzExperimentalCode apzEC = repeticionesEnFuncionALasCCs[k];
                Int32 nLbS = binaryString.Length / k;
                String nLbSBin = apzCode.FromBase10(nLbS.ToString() , 2);
                rTempString += apzCode.FromBase10(nLbSBin.Length.ToString(), 2).PadLeft(16, '0');

                foreach (apzExpCodSearch cS in apzEC.apzExpCodSearches)
                {
                    rTempString += cS.S01;
                    rTempString += apzCode.FromBase10(cS.Where.Count.ToString(), 2).PadLeft(8,'0');
                    foreach (Int32 j in cS.Where)
                    {
                        rTempString += apzCode.FromBase10(j.ToString(), 2).PadLeft(nLbSBin.Length, '0');
                    }                                        
                }

                resultados.Add(rTempString);

            }

            return sResult;
        }

        private async Task<String> getStringFromByteArray(Byte[] bytes)
        {                        
            String[] stringArray = new String[bytes.Length];

            List<Task> tasks = new List<Task>();
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                Int32 j = i;

                tasks.Add(Task.Run(() =>
                {
                    String s = Convert.ToString(bytes[j], 2).PadLeft(8, '0');
                    stringArray[j] = s;
                }));
            }

            await Task.WhenAll(tasks);

            return String.Concat(stringArray);
        }

        /// <summary>
        /// Obtiene los 0 y 1s en boolean: 0 = false; 1 = true de un array de bytes en formato Little o Big Endian LE = true; BE = false
        /// </summary>
        private async Task<Boolean[]> getBoolArrayFromByteArray(Byte[] bytes, Boolean LE)
        {
            Boolean[][] booleanArray = new Boolean[bytes.Length][];

            List<Task> tasks = new List<Task>();
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                Int32 j = i;

                tasks.Add(Task.Run(() =>
                {
                    Boolean[] bA;
                    if (LE)
                    {
                        bA = ByteToBoolsLE(bytes[j]);
                    }
                    else
                    {
                        bA = ByteToBoolsBE(bytes[j]);
                    }
                    
                    booleanArray[j] = bA;
                }));
            }

            await Task.WhenAll(tasks);

            return booleanArray.SelectMany(x => x).ToArray();
        }

        /// <summary>
        /// Little Endian
        /// </summary>        
        static bool[] ByteToBoolsLE(byte value)
        {
            var values = new bool[8];

            values[0] = (value & 1) == 0 ? false : true;
            values[1] = (value & 2) == 0 ? false : true;
            values[2] = (value & 4) == 0 ? false : true;
            values[3] = (value & 8) == 0 ? false : true;
            values[4] = (value & 16) == 0 ? false : true;
            values[5] = (value & 32) == 0 ? false : true;
            values[6] = (value & 64) == 0 ? false : true;
            values[7] = (value & 128) == 0 ? false : true;

            return values;
        }

        /// <summary>
        /// Big Endian
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static bool[] ByteToBoolsBE(byte value)
        {
            var values = new bool[8];

            values[0] = (value & 128) == 0 ? false : true;
            values[1] = (value & 64) == 0 ? false : true;
            values[2] = (value & 32) == 0 ? false : true;
            values[3] = (value & 16) == 0 ? false : true;
            values[4] = (value & 8) == 0 ? false : true;
            values[5] = (value & 4) == 0 ? false : true;
            values[6] = (value & 2) == 0 ? false : true;
            values[7] = (value & 1) == 0 ? false : true;

            return values;
        }

        /// <summary>
        /// Algoritmo experimental de compresión made in apzyx
        /// </summary>
        private async Task<String> apzAlgPrueba02(String binaryString)
        {
            // TODO: Mejorar añadiendo datos irrelevantes al final para conseguir un número bsLength con mayores factores, solución al problema derivado,
            // de conjuntos de datos con un bsLength primo, para poder aplicar el algoritmo en cualquier caso
                                    
            // Obtenemos el tamaño del binario que queremos comprimir
            Int32 bsLength = binaryString.Length;
            //List<String> resultadosCaoticos = new List<String>();
            String resultadoCaótico = String.Empty;
            //List<Int32> combinacionesProcesadas = new List<Int32>();
            
            List<Task> tasks = new List<Task>();

            for (Int32 bsChang = 0; bsChang < 13; bsChang++)
            {
                Int32 bsChange = bsChang;

                tasks.Add(Task.Run(() =>
                {

                    // Obtenemos los factores primos de este tamaño, deberemos obtener siempre un mínimo de 2^3 que es 8 siempre y cuando sean archivos codificados en bytes
                    List<Int32> factores = obtenerFactoresPrimos(bsLength - bsChange);

                    // Obtenemos todas las combinaciones posibles entre esos factores
                    List<IEnumerable<Int32>> combinaciones = GetPowerSet<Int32>(factores);

                    List<Int32> combinacionesCalculadas = new List<Int32>();
                    foreach (IEnumerable<Int32> nS in combinaciones)
                    {
                        // Hacemos el cálculo de cada combinación
                        Int32 cC = 1;
                        foreach (Int32 n in nS)
                        {
                            cC *= n;
                        }

                        // Solo introducimos las que son 1 Bytes o mayores
                        if (cC > 8 && cC < 500000 && cC < bsLength - bsChange)
                        {
                            // Y no aceptamos valores repetidos
                            if (!combinacionesCalculadas.Contains(cC))
                            {
                                combinacionesCalculadas.Add(cC);
                            }
                        }
                    }

                    // Ordenamos las combinaciones de mayor a menor
                    combinacionesCalculadas = combinacionesCalculadas.OrderByDescending(x => x).ToList();


                    //// Calculamos sobre las combinaciones que no hemos procesado ya
                    //List<Int32> combinacionesNoProcesadas = new List<Int32>();
                    //foreach (Int32 cCtoP in combinacionesCalculadas)
                    //{
                    //    lock (combinacionesProcesadas)
                    //    {
                    //        if (!combinacionesProcesadas.Contains(cCtoP))
                    //        {
                    //            combinacionesNoProcesadas.Add(cCtoP);
                    //        }
                    //    }
                    //}

                    //combinacionesCalculadas = combinacionesNoProcesadas;

                    //Dictionary<Int32, apzExperimentalCode> repeticionesEnFuncionALasCCs = new Dictionary<Int32, apzExperimentalCode>();

                    // Ahora con esas combinaciones creamos los conjuntos de datos aceptados por el conjunto inicial
                    // Para ello,
                    // Recorremos cada combinación y creamos las estructuras para contar sus repeticiones en los datos originales
                    foreach (Int32 cC in combinacionesCalculadas)
                    {
                        //lock (combinacionesProcesadas)
                        //{
                        //    combinacionesProcesadas.Add(cC);
                        //}

                        String actualLBS = String.Empty;

                        // Si el proceso anterior es correcto esta división debe de ser siempre exacta
                        Int32 nLbS = (bsLength - bsChange) / cC;

                        String rTempString = apzCode.FromBase10((bsLength - bsChange).ToString(), 2).PadLeft(32, '0') + apzCode.FromBase10(cC.ToString(), 2).PadLeft(16, '0');
                        String nLbSBin = apzCode.FromBase10(nLbS.ToString(), 2);
                        rTempString += apzCode.FromBase10(nLbSBin.Length.ToString(), 2).PadLeft(16, '0');

                        // Inicializamos para cada cC la lista de repeticiones
                        //repeticionesEnFuncionALasCCs.Add(cC, new apzExperimentalCode() { lbS = nLbS, apzExpCodSearches = new List<apzExpCodSearch>() });

                        // Hacemos un barrido para esta combinación
                        for (Int32 i = 0; i < nLbS; i++)
                        {
                            // Comprobamos y anotamos las repeticiones
                            String s02 = binaryString.Substring(i * cC, cC);

                            Int32 indexOf = actualLBS.IndexOf(s02);

                            //Boolean encontrado = false;
                            if (indexOf != -1 && indexOf % cC == 0)
                            {
                                //for (Int32 j = 0; j < (actualLBS.Length / cC) - cC; j++)
                                //{
                                    //String s02Search = actualLBS.Substring(j * cC, cC);

                                    //if (s02Search == s02)
                                    //{
                                        //encontrado = true;
                                        rTempString += "1" + apzCode.FromBase10((indexOf / cC).ToString(), 2).PadLeft(nLbSBin.Length, '0');
                                        //break;
                                    //}
                                //}
                            }
                            else
                            {
                                rTempString = String.Concat(rTempString, "0", s02);
                            }
                            //apzExperimentalCode listaRepeticionesDeCcConcreto = repeticionesEnFuncionALasCCs[cC];

                            //apzExpCodSearch hS = listaRepeticionesDeCcConcreto.apzExpCodSearches.Where(x => x.S01 == s02).SingleOrDefault();

                            //if (!encontrado)
                            //{
                            //    rTempString = String.Concat(rTempString, "0", s02);

                            //    //listaRepeticionesDeCcConcreto.apzExpCodSearches.Add(new apzExpCodSearch() { S01 = s02, Where = new List<Int32>() { i } });
                            //}
                            actualLBS = String.Concat(actualLBS, s02);
                            GC.Collect();
                        }



                        // Quitamos los parameters que no tengan repeticiones
                        //if (repeticionesEnFuncionALasCCs[cC].apzExpCodSearches.Count == nLbS)
                        //{
                        //    repeticionesEnFuncionALasCCs.Remove(cC);
                        //}
                        //else
                        //{
                        rTempString += "".PadRight(bsChange, '0');
                        if (rTempString.Length < bsLength)
                        {
                            //lock (resultadosCaoticos)
                            //{

                            //    resultadosCaoticos.Add(rTempString);
                            //}

                            lock (resultadoCaótico)
                            {
                                if(String.IsNullOrEmpty(resultadoCaótico) || resultadoCaótico.Length > rTempString.Length)
                                {
                                    resultadoCaótico  = rTempString;
                                }                                    
                            }
                        }
                        //}
                    }

                }));
            }

            await Task.WhenAll(tasks);

            //StringBuilder sB = new StringBuilder();

            //sB.AppendLine("k".PadLeft(5, ' ') + " " + "lbS".PadLeft(5, ' ') + " " + "CountL".PadLeft(5, ' '));

            //foreach (Int32 k in repeticionesEnFuncionALasCCs.Keys)
            //{
            //    apzExperimentalCode apzEC = repeticionesEnFuncionALasCCs[k];

            //    sB.AppendLine(k.ToString().PadLeft(5, ' ') + " " + apzEC.lbS.ToString().PadLeft(5, ' ') + " " + apzEC.apzExpCodSearches.Count.ToString().PadLeft(5, ' '));
            //}

            //FileSavePicker picker = new FileSavePicker();
            //picker.FileTypeChoices.Add(".txt", new string[] { ".txt" });
            //picker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            //picker.SuggestedFileName = "output.txt";
            //StorageFile file = await picker.PickSaveFileAsync();
            //if (file != null)
            //{
            //    await FileIO.WriteTextAsync(file, sB.ToString());
            //}


            //return resultadosCaoticos.OrderBy(x => x.Length).FirstOrDefault();
            return resultadoCaótico;


            //List<String> resultados = new List<String>();

            //// Hacemos una probatura para todos los datos resultantes
            //foreach (Int32 k in repeticionesEnFuncionALasCCs.Keys)
            //{
            //    // Establecemos la cabecera con los datos del fichero y la "k" o tamaño de bloque
            //    String rTempString = apzCode.FromBase10(bsLength.ToString(), 2).PadLeft(32, '0') + apzCode.FromBase10(k.ToString(), 2).PadLeft(16, '0');
            //    apzExperimentalCode apzEC = repeticionesEnFuncionALasCCs[k];
            //    Int32 nLbS = binaryString.Length / k;
            //    String nLbSBin = apzCode.FromBase10(nLbS.ToString(), 2);
            //    rTempString += apzCode.FromBase10(nLbSBin.Length.ToString(), 2).PadLeft(16, '0');

            //    foreach (apzExpCodSearch cS in apzEC.apzExpCodSearches)
            //    {
            //        rTempString += cS.S01;
            //        rTempString += apzCode.FromBase10(cS.Where.Count.ToString(), 2).PadLeft(8, '0');
            //        foreach (Int32 j in cS.Where)
            //        {
            //            rTempString += apzCode.FromBase10(j.ToString(), 2).PadLeft(nLbSBin.Length, '0');
            //        }
            //    }

            //    resultados.Add(rTempString);

            //}

            //return sResult;
        }

        private async Task<String> apzAlgPrueba03(Byte[] bytes)
        {
            // TODO: Mejorar añadiendo datos irrelevantes al final para conseguir un número bsLength con mayores factores, solución al problema derivado,
            // de conjuntos de datos con un bsLength primo, para poder aplicar el algoritmo en cualquier caso

            String baStringUTF = Convert.ToBase64String(bytes);
            Int32 Nbits = bytes.Length * 8;
            // Obtenemos el tamaño del binario que queremos comprimir
            Int32 bsLength = baStringUTF.Length;
            //List<String> resultadosCaoticos = new List<String>();
            String resultadoCaótico = String.Empty;
            //List<Int32> combinacionesProcesadas = new List<Int32>();
                        
            String r = String.Empty;
            

            for (Int32 cogemos = 1; cogemos < 257; cogemos++)
            {
                Int32 maxICalculado = baStringUTF.Length / cogemos;
                Int32 maxICalculadoLength = apzCode.FromBase10(maxICalculado.ToString(), 2).Length;

                String actualS = String.Empty;
                String rTemp = String.Empty;
                for (Int32 i = 0; i < bsLength - cogemos; i+=cogemos)
                {
                    String s = baStringUTF.Substring(i, cogemos);

                    Int32 indexOf = actualS.IndexOf(s);

                    if (indexOf != -1 && indexOf % cogemos == 0)
                    {
                        rTemp += "1" + apzCode.FromBase10((indexOf / cogemos).ToString(), 2).ToString().PadLeft(maxICalculadoLength, '0');
                    }
                    else
                    {
                        rTemp += "0" + (await getStringFromByteArray(Encoding.UTF8.GetBytes(s)));
                    }

                    actualS += s;
                }

                if (rTemp.Length < Nbits)
                {
                    if (String.IsNullOrEmpty(r) || rTemp.Length < r.Length)
                    {
                        r = rTemp;
                    }
                }
            }
            
            return r;
        }

        private async Task<String> apzAlgPrueba04(Boolean[] s01)
        {
            // TODO: Mejorar añadiendo datos irrelevantes al final para conseguir un número bsLength con mayores factores, solución al problema derivado,
            // de conjuntos de datos con un bsLength primo, para poder aplicar el algoritmo en cualquier caso

            //String baStringUTF = s01;            
            //Int32 Nbits = s01.Length;
            // Obtenemos el tamaño del binario que queremos comprimir
            Int32 bsLength = s01.Length;
            //List<String> resultadosCaoticos = new List<String>();
            String resultadoCaótico = String.Empty;
            //List<Int32> combinacionesProcesadas = new List<Int32>();

            String r = String.Empty;

            List<Task> tasks = new List<Task>();

            BoolArayComparer boolComparer = new BoolArayComparer();

            for (Int32 cog = 8; cog < 257; cog*=2)
            {
                Int32 cogemos = cog;

                //tasks.Add(Task.Run(() =>
                //{

                    Int32 maxICalculado = bsLength / cogemos;
                    Int32 maxICalculadoLength = apzCode.FromBase10(maxICalculado.ToString(), 2).Length;

                    List<apzExpCodSearchBool> códigosRepeticiones = new List<apzExpCodSearchBool>();

                    //String actualS = String.Empty;
                    List<Boolean[]> repetidos = new List<Boolean[]>();
                    for (Int32 i = 0; i < bsLength - cogemos; i += cogemos)
                    {
                        Boolean[] s = s01.SubArray(i, cogemos);

                        //for (Int32 j = 0; j < actualS.Length; j += cogemos)
                        //{
                        //    Int32 indexOf = actualS.IndexOf(s);
                        //}
                        if (repetidos.Contains(s, boolComparer))
                        {
                            apzExpCodSearchBool cod = códigosRepeticiones.Where(x => x.S01.SequenceEqual(s)).SingleOrDefault();
                            cod.Count++;
                        }
                        else
                        {
                            códigosRepeticiones.Add(new apzExpCodSearchBool() { S01 = s, Count = 1 });
                            repetidos.Add(s);
                        }

                        //actualS += s;
                    }

                    códigosRepeticiones = códigosRepeticiones.OrderBy(x => x.Count).ToList();

                    List<apzExpCodSearchBool> dataNodes = new List<apzExpCodSearchBool>();

                    // CREAR ARBOL DE HUFFMAN PARA OBTENER CÓDIGOS NO PREFIJO

                    while (códigosRepeticiones.Count > 1)
                    {
                        apzExpCodSearchBool nDer = códigosRepeticiones[0];
                        apzExpCodSearchBool nIzq = códigosRepeticiones[1];

                        apzExpCodSearchBool newNode = new apzExpCodSearchBool() { Der1 = nDer, Izq0 = nIzq, Count = nDer.Count + nIzq.Count };

                        nDer.Padre = newNode;
                        nIzq.Padre = newNode;

                        if (nDer.S01 != null && nDer.S01.Length > 0)
                        {
                            dataNodes.Add(nDer);
                        }
                        if (nIzq.S01 != null && nIzq.S01.Length > 0)
                        {
                            dataNodes.Add(nIzq);
                        }

                        códigosRepeticiones.RemoveAt(0);
                        códigosRepeticiones.RemoveAt(0);

                        códigosRepeticiones.Add(newNode);

                        códigosRepeticiones = códigosRepeticiones.OrderBy(x => x.Count).ToList();
                    }

                    Dictionary<String, String> dic = new Dictionary<String, String>();
                    Int32 maxLengthPrefixCode = 0;
                    foreach (apzExpCodSearchBool node in dataNodes)
                    {
                        String binaryPrefix = String.Empty;

                        apzExpCodSearchBool padre = node.Padre;
                        apzExpCodSearchBool nodeAnt = node;
                        Boolean continuar = true;
                        do
                        {
                            if (padre.Der1 == nodeAnt)
                            {
                                binaryPrefix = String.Concat("1", binaryPrefix);
                            }
                            else if (padre.Izq0 == nodeAnt)
                            {
                                binaryPrefix = String.Concat("0", binaryPrefix);
                            }

                            if (padre.Padre == null)
                            {
                                continuar = false;
                            }
                            nodeAnt = padre;
                            padre = padre.Padre;
                        }
                        while (continuar);

                        if (maxLengthPrefixCode < binaryPrefix.Length)
                        {
                            maxLengthPrefixCode = binaryPrefix.Length;
                        }

                        dic.Add(new String(node.S01.Select(c => c ? '1' : '0').ToArray()), binaryPrefix);
                    }

                    String rTemp = "000" + apzCode.FromBase10(cogemos.ToString(), 2).PadLeft(8, '0') + apzCode.FromBase10(maxLengthPrefixCode.ToString(), 2).PadLeft(8, '0');
                    List<Boolean[]> yaInsertados = new List<Boolean[]>();
                    Int32 indexFault = -1;
                    for (Int32 i = 0; i < bsLength - cogemos; i += cogemos)
                    {
                        indexFault = i + cogemos;

                        Boolean[] s = s01.SubArray(i, cogemos);
                        String sS = new String(s.Select(c => c ? '1' : '0').ToArray());
                        if (dic.ContainsKey(sS))
                        {
                            if (yaInsertados.Contains(s, boolComparer))
                            {
                                rTemp += "1" + dic[sS];
                            }
                            else
                            {
                                String coPrefixLBinary = apzCode.FromBase10(dic[sS].Length.ToString(), 2);

                                rTemp += "01" + sS + coPrefixLBinary.PadLeft(maxLengthPrefixCode, '0') + dic[sS];
                                yaInsertados.Add(s);
                            }
                        }
                        else
                        {
                            rTemp += "00" + sS;
                        }
                    }

                    // TODO: COMPROBAR
                    rTemp += indexFault < bsLength ? new String(s01.SubArray(indexFault, bsLength - indexFault).Select(c => c ? '1' : '0').ToArray()) : "";

                    if (rTemp.Length < bsLength)
                    {
                        lock (r)
                        {
                            if (String.IsNullOrEmpty(r) || rTemp.Length < r.Length)
                            {
                                r = rTemp;
                            }
                        }
                    }
                //}));

            }

            await Task.WhenAll(tasks);

            return r;
        }

        private async Task<String> HuffmanConDiccionarioIntegrado(String s01)
        {
            // TODO: Mejorar añadiendo datos irrelevantes al final para conseguir un número bsLength con mayores factores, solución al problema derivado,
            // de conjuntos de datos con un bsLength primo, para poder aplicar el algoritmo en cualquier caso

            //String baStringUTF = s01;            
            //Int32 Nbits = s01.Length;
            // Obtenemos el tamaño del binario que queremos comprimir
            Int32 bsLength = s01.Length;
            //List<String> resultadosCaoticos = new List<String>();
            String resultadoCaótico = String.Empty;
            //List<Int32> combinacionesProcesadas = new List<Int32>();

            String r = String.Empty;

            List<Task> tasks = new List<Task>();

            BoolArayComparer boolComparer = new BoolArayComparer();

            for (Int32 cog = 8; cog < 257; cog *= 2)
            {
                Int32 cogemos = cog;

                //tasks.Add(Task.Run(() =>
                //{

                Int32 maxICalculado = bsLength / cogemos;
                Int32 maxICalculadoLength = apzCode.FromBase10(maxICalculado.ToString(), 2).Length;

                List<apzExpCodSearch> códigosRepeticiones = new List<apzExpCodSearch>();

                //String actualS = String.Empty;
                List<String> repetidos = new List<String>();
                for (Int32 i = 0; i < bsLength - cogemos; i += cogemos)
                {
                    String s = s01.Substring(i, cogemos);

                    //for (Int32 j = 0; j < actualS.Length; j += cogemos)
                    //{
                    //    Int32 indexOf = actualS.IndexOf(s);
                    //}
                    if (repetidos.Contains(s))
                    {
                        apzExpCodSearch cod = códigosRepeticiones.Where(x => x.S01 == s).SingleOrDefault();
                        cod.Count++;
                    }
                    else
                    {
                        códigosRepeticiones.Add(new apzExpCodSearch() { S01 = s, Count = 1 });
                        repetidos.Add(s);
                    }

                    //actualS += s;
                }

                códigosRepeticiones = códigosRepeticiones.OrderBy(x => x.Count).ToList();

                StringBuilder sB = new StringBuilder();


                foreach (apzExpCodSearch codS in códigosRepeticiones)
                {
                    sB.AppendLine(codS.S01 + "    "  + codS.Count.ToString().PadLeft(3,' '));
                }

                String s2 = sB.ToString();

                List<apzExpCodSearch> dataNodes = new List<apzExpCodSearch>();

                // CREAR ARBOL DE HUFFMAN PARA OBTENER CÓDIGOS NO PREFIJO

                while (códigosRepeticiones.Count > 1)
                {
                    apzExpCodSearch nDer = códigosRepeticiones[0];
                    apzExpCodSearch nIzq = códigosRepeticiones[1];

                    apzExpCodSearch newNode = new apzExpCodSearch() { Der1 = nDer, Izq0 = nIzq, Count = nDer.Count + nIzq.Count };

                    nDer.Padre = newNode;
                    nIzq.Padre = newNode;

                    if (!String.IsNullOrEmpty(nDer.S01))
                    {
                        dataNodes.Add(nDer);
                    }
                    if (!String.IsNullOrEmpty(nIzq.S01))
                    {
                        dataNodes.Add(nIzq);
                    }

                    códigosRepeticiones.RemoveAt(0);
                    códigosRepeticiones.RemoveAt(0);

                    códigosRepeticiones.Add(newNode);

                    códigosRepeticiones = códigosRepeticiones.OrderBy(x => x.Count).ToList();
                }

                Dictionary<String, String> dic = new Dictionary<String, String>();
                Int32 maxLengthPrefixCode = 0;
                foreach (apzExpCodSearch node in dataNodes)
                {
                    String binaryPrefix = String.Empty;

                    apzExpCodSearch padre = node.Padre;
                    apzExpCodSearch nodeAnt = node;
                    Boolean continuar = true;
                    do
                    {
                        if (padre.Der1 == nodeAnt)
                        {
                            binaryPrefix = String.Concat("1", binaryPrefix);
                        }
                        else if (padre.Izq0 == nodeAnt)
                        {
                            binaryPrefix = String.Concat("0", binaryPrefix);
                        }

                        if (padre.Padre == null)
                        {
                            continuar = false;
                        }
                        nodeAnt = padre;
                        padre = padre.Padre;
                    }
                    while (continuar);

                    if (maxLengthPrefixCode < binaryPrefix.Length)
                    {
                        maxLengthPrefixCode = binaryPrefix.Length;
                    }

                    dic.Add(node.S01, binaryPrefix);
                }

                String rTemp = "000" + apzCode.FromBase10(cogemos.ToString(), 2).PadLeft(8, '0') + apzCode.FromBase10(maxLengthPrefixCode.ToString(), 2).PadLeft(8, '0');
                List<String> yaInsertados = new List<String>();
                Int32 indexFault = -1;
                for (Int32 i = 0; i < bsLength - cogemos; i += cogemos)
                {
                    indexFault = i + cogemos;

                    String s = s01.Substring(i, cogemos);
                    if (dic.ContainsKey(s))
                    {
                        if (yaInsertados.Contains(s))
                        {
                            rTemp += "1" + dic[s];
                        }
                        else
                        {
                            String coPrefixLBinary = apzCode.FromBase10(dic[s].Length.ToString(), 2);

                            rTemp += "01" + s + coPrefixLBinary.PadLeft(maxLengthPrefixCode, '0') + dic[s];
                            yaInsertados.Add(s);
                        }
                    }
                    else
                    {
                        rTemp += "00" + s;
                    }
                }

                // TODO: COMPROBAR
                rTemp += indexFault < bsLength ? s01.Substring(indexFault, bsLength - indexFault) : "";

                if (rTemp.Length < bsLength)
                {
                    lock (r)
                    {
                        if (String.IsNullOrEmpty(r) || rTemp.Length < r.Length)
                        {
                            r = rTemp;
                        }
                    }
                }
                //}));

            }

            await Task.WhenAll(tasks);

            return r;
        }

        private async Task<String> apzChangeSequence(String s01)
        {
            String r = s01[0].ToString();

            Char c = s01[0];
            for (Int32 i = 1; i < s01.Length; i++)
            {
                if (s01[i] == c)
                {
                    r += c == '0' ? "1" : "0";
                    c = c == '0' ? '1' : '0';


                }
                else
                {
                    r += c == '0' ? "0" : "1";
                }

            }

            return "";

        }

        private async Task<String> apzAlgPrueba05(Byte[] bytes)
        {
            // TODO: Mejorar añadiendo datos irrelevantes al final para conseguir un número bsLength con mayores factores, solución al problema derivado,
            // de conjuntos de datos con un bsLength primo, para poder aplicar el algoritmo en cualquier caso

            String sBinary = await getStringFromByteArray(bytes);
            Int32 sBinaryLenthInit = sBinary.Length;
            sBinary = sBinary.TrimStart('0');
            GC.Collect();
            Int32 pérdidas0 = sBinaryLenthInit - sBinary.Length;

            String r = apzCode.FromBase10(pérdidas0.ToString(),2).PadLeft(4, '0');

            if(r.Length > 4)
            {
                throw new Exception("Aqui que ver esto");
            }

            String s = sBinary[0].ToString();
            Boolean continuar = true;            
            List<BigInteger> primos3 = new List<BigInteger>();
            Int32 index = 1;
            Int32 últimoIndex = 1;
            while (continuar)
            {
                s += sBinary[index].ToString();
                BigInteger posiblePri = BigInteger.Parse(apzCode.ToBase10(s, 2));
                if (s[s.Length - 1] == '1')
                {
                    if (posiblePri > 7 && obtenerFactoresPrimosBIModificado(posiblePri).Count == 0)
                    {
                        primos3.Add(posiblePri);
                        s = sBinary[index].ToString();
                    }
                }
                if (primos3.Count == 3)
                {
                    últimoIndex = index;
                    r += apzCode.FromBase10((primos3[0] + primos3[1] + primos3[2]).ToString(), 2);
                    primos3= new List<BigInteger>();
                    s = sBinary[index].ToString();
                }


                index++;
                if (sBinary.Length <= index)
                {
                    continuar = false;
                }
            }

            if(últimoIndex < sBinary.Length)
            {
                r += sBinary.Substring(últimoIndex, sBinary.Length - últimoIndex);
            }

            return r;
        }

        private async Task<String> apzAlgPrueba06(String s01)
        {
            String r = String.Empty;

            List<Int32> menorMayorIgual = new List<Int32>();

            Boolean esMayorOvacio = true;
            Boolean esMenorOvacio = true;
            Boolean esIgualOvacio = true;
            for (Int32 i = 0; i < s01.Length - 3; i += 3)
            {
                String s = s01.Substring(i, 3);

                Int32 nS = Convert.ToInt32(apzCode.ToBase10(s, 2));
                                
                if(isPrime7Max(nS))
                {                    
                    foreach (Int32 pInList in menorMayorIgual)
                    {
                        if (pInList >= nS)
                        {
                            esMayorOvacio = false;
                            break;
                        }
                    }

                    foreach (Int32 pInList in menorMayorIgual)
                    {
                        if (pInList <= nS)
                        {
                            esMenorOvacio = false;
                            break;
                        }
                    }

                    foreach (Int32 pInList in menorMayorIgual)
                    {
                        if (pInList != nS)
                        {
                            esIgualOvacio = false;
                            break;
                        }
                    }

                    if (esMayorOvacio || esMenorOvacio || esIgualOvacio)
                    {
                        menorMayorIgual.Add(nS);

                        if (esMayorOvacio || esMenorOvacio)
                        {

                            if (menorMayorIgual.Count == 3)
                            {
                                Int32 suma = 0;
                                foreach (Int32 pInList in menorMayorIgual)
                                {
                                    suma += pInList;
                                }

                                r += (esMayorOvacio ? "01" : "10") + apzCode.FromBase10(suma.ToString(), 2).PadLeft(4, '0');

                                menorMayorIgual.Clear();
                            }
                        }
                        else if(esIgualOvacio)
                        {
                            if (menorMayorIgual.Count == 3)
                            {
                                Int32 initS = menorMayorIgual[0];

                                r += "11" + apzCode.FromBase10(initS.ToString(), 2).PadLeft(3, '0');
                            }
                            menorMayorIgual.Clear();
                        }
                        else
                        {
                            throw new Exception("Esto no debería estar pasando");
                        }
                    }
                    else
                    {
                        foreach (Int32 pInList in menorMayorIgual)
                        {
                            r += "00" + apzCode.FromBase10(pInList.ToString(), 2).PadLeft(3,'0');
                        }
                        esMayorOvacio = true;
                        esMenorOvacio = true;
                        esIgualOvacio = true;
                        menorMayorIgual.Clear();
                    }

                }
                else
                {
                    foreach (Int32 pInList in menorMayorIgual)
                    {
                        r += "00" + apzCode.FromBase10(pInList.ToString(), 2).PadLeft(3, '0');
                    }
                    esMayorOvacio = true;
                    esMenorOvacio = true;
                    esIgualOvacio = true;
                    menorMayorIgual.Clear();

                    r += "00" + s;
                }


            }

            return r;
        }

        private async Task<String> apzAlgPrueba07(String s01)
        {
            String r = String.Empty;

            List<Int32> menorMayorIgual = new List<Int32>();

            Boolean esMayorOvacio = true;
            Boolean esMenorOvacio = true;
            Boolean esIgualOvacio = true;
            for (Int32 i = 0; i < s01.Length - 8; i += 8)
            {
                String s = s01.Substring(i, 8);

                Int32 nS = Convert.ToInt32(apzCode.ToBase10(s, 2));

                if (isPrime7Max(nS))
                {
                    foreach (Int32 pInList in menorMayorIgual)
                    {
                        if (pInList >= nS)
                        {
                            esMayorOvacio = false;
                            break;
                        }
                    }

                    foreach (Int32 pInList in menorMayorIgual)
                    {
                        if (pInList <= nS)
                        {
                            esMenorOvacio = false;
                            break;
                        }
                    }

                    foreach (Int32 pInList in menorMayorIgual)
                    {
                        if (pInList != nS)
                        {
                            esIgualOvacio = false;
                            break;
                        }
                    }

                    if (esMayorOvacio || esMenorOvacio || esIgualOvacio)
                    {
                        menorMayorIgual.Add(nS);

                        if (esMayorOvacio || esMenorOvacio)
                        {

                            if (menorMayorIgual.Count == 3)
                            {
                                Int32 suma = 0;
                                foreach (Int32 pInList in menorMayorIgual)
                                {
                                    suma += pInList;
                                }

                                r += (esMayorOvacio ? "01" : "10") + apzCode.FromBase10(suma.ToString(), 2).PadLeft(9, '0');

                                menorMayorIgual.Clear();
                            }
                        }
                        else if (esIgualOvacio)
                        {
                            if (menorMayorIgual.Count == 3)
                            {
                                Int32 initS = menorMayorIgual[0];

                                r += "11" + apzCode.FromBase10(initS.ToString(), 2).PadLeft(8, '0');
                            }
                            menorMayorIgual.Clear();
                        }
                        else
                        {
                            throw new Exception("Esto no debería estar pasando");
                        }
                    }
                    else
                    {
                        foreach (Int32 pInList in menorMayorIgual)
                        {
                            r += "00" + apzCode.FromBase10(pInList.ToString(), 2).PadLeft(8, '0');
                        }
                        esMayorOvacio = true;
                        esMenorOvacio = true;
                        esIgualOvacio = true;
                        menorMayorIgual.Clear();
                    }

                }
                else
                {
                    foreach (Int32 pInList in menorMayorIgual)
                    {
                        r += "00" + apzCode.FromBase10(pInList.ToString(), 2).PadLeft(8, '0');
                    }
                    esMayorOvacio = true;
                    esMenorOvacio = true;
                    esIgualOvacio = true;
                    menorMayorIgual.Clear();

                    r += "00" + s;
                }


            }

            return r;
        }

        private Boolean isPrime7Max(Int32 pP)
        {
            if(pP == 1 || pP == 2 || pP == 3 || pP == 5 || pP == 7)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Obteiene los factores primos de un número
        /// </summary>        
        private List<Int32> obtenerFactoresPrimos(Int32 nOriginal)
        {
            List<Int32> factoresUnicosRepetidos = new List<Int32>();
            Boolean continuar = true;
            Int32 elTres = 3;
            Double raizNOriginal = Math.Sqrt(nOriginal);
            while (continuar)
            {
                // Si es divisible entre 2
                if (nOriginal % 2 == 0)
                {
                    factoresUnicosRepetidos.Add(2);
                    nOriginal = nOriginal / 2;
                }

                else if (elTres < raizNOriginal + 2)
                {
                    if (nOriginal % elTres == 0)
                    {
                        factoresUnicosRepetidos.Add(elTres);
                        nOriginal = nOriginal / elTres;
                    }
                    else
                    {
                        elTres += 2;
                    }
                }
                else
                {
                    // Sí, ya sé que esto sería algo redundante xD
                    continuar = false;                    
                    break;
                }
            }

            return factoresUnicosRepetidos;
        }

        private List<BigInteger> obtenerFactoresPrimosBIModificado(BigInteger nOriginal)
        {
            List<BigInteger> factoresUnicosRepetidos = new List<BigInteger>();
            Boolean continuar = true;
            BigInteger elTres = 3;
            BigInteger exacta = -1;
            BigInteger raizNOriginal = SDK.raizCuadradaMasUno(nOriginal, out exacta);
            while (continuar)
            {
                // Si es divisible entre 2
                if (nOriginal % 2 == 0)
                {
                    factoresUnicosRepetidos.Add(2);
                    break;
                    nOriginal = nOriginal / 2;
                }

                else if (elTres < raizNOriginal + 2)
                {
                    if (nOriginal % elTres == 0)
                    {
                        factoresUnicosRepetidos.Add(elTres);
                        break;
                        nOriginal = nOriginal / elTres;
                    }
                    else
                    {
                        elTres += 2;
                    }
                }
                else
                {
                    // Sí, ya sé que esto sería algo redundante xD
                    continuar = false;
                    break;
                }
            }

            return factoresUnicosRepetidos;
        }

        public List<IEnumerable<T>> GetPowerSet<T>(List<T> list)
        {
            return (from m in Enumerable.Range(0, 1 << list.Count)
                   select
                   from i in Enumerable.Range(0, list.Count)
                   where (m & (1 << i)) != 0
                   select list[i]).ToList();
        }

        //private static string FromBase10(String number, int target_base)
        //{

        //    if (target_base < 2 || target_base > 36) return "";
        //    if (target_base == 10) return number.ToString();

        //    int n = target_base;
        //    BigInteger q = BigInteger.Parse(number);
        //    BigInteger r;
        //    string rtn = "";

        //    while (q >= n)
        //    {

        //        r = q % n;
        //        q = q / n;

        //        if (r < 10)
        //            rtn = r.ToString() + rtn;
        //        else
        //            rtn = Convert.ToChar((Int32)(r + 55)).ToString() + rtn;

        //    }

        //    if (q < 10)
        //        rtn = q.ToString() + rtn;
        //    else
        //        rtn = Convert.ToChar((Int32)(q + 55)).ToString() + rtn;

        //    return rtn;

        //}

        //private static String ToBase10(string number, int start_base)
        //{
        //    if (start_base < 2 || start_base > 36) return "0";
        //    if (start_base == 10) return number;

        //    char[] chrs = number.ToCharArray();
        //    int m = chrs.Length - 1;
        //    int n = start_base;
        //    BigInteger x;
        //    BigInteger rtn = 0;

        //    foreach (char c in chrs)
        //    {

        //        if (char.IsNumber(c))
        //            x = int.Parse(c.ToString());
        //        else
        //            x = Convert.ToInt32(c) - 55;

        //        rtn += x * BigInteger.Pow(n, m);

        //        m--;

        //    }

        //    return rtn.ToString();

        //}

        public static BigInteger BinarioADecimal(string binario)
        {
            BigInteger decimalNum = 0;
            int nBits = binario.Length;

            for (int i = 0; i < nBits; i++)
            {
                int bit = int.Parse(binario[i].ToString());
                BigInteger potencia = BigInteger.Pow(2, nBits - i - 1);
                decimalNum += bit * potencia;
            }

            return decimalNum;
        }

        public static string DecimalABinario(BigInteger decimalNum)
        {
            if (decimalNum == 0)
            {
                return "0";
            }

            int nBits = (int)Math.Ceiling(BigInteger.Log(decimalNum, 2));
            BigInteger divisor = BigInteger.Pow(2, nBits - 1);
            string binario = "";

            for (int i = 0; i < nBits; i++)
            {
                if (decimalNum < divisor)
                {
                    binario += "0";
                }
                else
                {
                    binario += "1";
                    decimalNum -= divisor;
                }
                divisor /= 2;
            }

            return binario;
        }


        ///// <summary>
        ///// A high performance BigInteger to binary string converter
        ///// that supports 0 and negative numbers.
        ///// License: MIT / Created by Ryan Scott White, 7/16/2022;
        ///// </summary>
        //public static string BigIntegerToBinaryString(BigInteger x)
        //{
        //    // Setup source
        //    ReadOnlySpan<byte> srcBytes = x.ToByteArray();
        //    int srcLoc = srcBytes.Length - 1;

        //    // Find the first bit set in the first byte so we don't print extra zeros.
        //    int msb = log2(srcBytes[srcLoc]);

        //    // Setup Target
        //    Span<char> dstBytes = stackalloc char[srcLoc * 8 + msb + 2];
        //    int dstLoc = 0;

        //    // Add leading '-' sign if negative.
        //    if (x.Sign < 0)
        //    {
        //        dstBytes[dstLoc++] = '-';
        //    }
        //    //else if (!x.IsZero) dstBytes[dstLoc++] = '0'; // add adding leading '0' (optional)

        //    // The first byte is special because we don't want to print leading zeros.
        //    byte b = srcBytes[srcLoc--];
        //    for (int j = msb; j >= 0; j--)
        //    {
        //        dstBytes[dstLoc++] = (char)('0' + ((b >> j) & 1));
        //    }

        //    // Add the remaining bits.
        //    for (; srcLoc >= 0; srcLoc--)
        //    {
        //        byte b2 = srcBytes[srcLoc];
        //        for (int j = 7; j >= 0; j--)
        //        {
        //            dstBytes[dstLoc++] = (char)('0' + ((b2 >> j) & 1));
        //        }
        //    }

        //    return dstBytes.ToString();
        //}

        public static string ToBinaryString(BigInteger bigint)
        {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[idx], 2);

            // Ensure leading zero exists if value is positive.
            if (binary[0] != '0' && bigint.Sign == 1)
            {
                base2.Append('0');
            }

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--)
            {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.ToString();
        }

        private async void btnNew_Click(object sender, RoutedEventArgs e)
        {
            for (Int32 itxt = 26; itxt < 27; itxt++)
            {
                try
                {
                    StorageFolder assetsFoldertxt = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                    StorageFile filetxt = await assetsFoldertxt.GetFileAsync("_FLATE" + itxt.ToString() + "TEXT.txt");

                    String s01txt = String.Empty;
                    using (var stream = await filetxt.OpenStreamForReadAsync())
                    {
                        byte[] buffer = new byte[(int)stream.Length];
                        await stream.ReadAsync(buffer, 0, buffer.Length);
                        s01txt = string.Join("", buffer.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
                        
                    }

                    String off = String.Empty;

                    for (Int32 i = 0; (i * 7) + 7 < s01txt.Length; i++)
                    {
                        off += s01txt.Substring(i * 7, 7).PadLeft(8, '0');
                    }



                    for (Int32 iBits = 2; iBits < 9; iBits *= 2)
                    {

                        // Calculamos el width en función de los bits elegidos por canal de color            
                        Int32 w = 0;
                        Int32 h = 0;

                        // y la dependepndia de tener seleccionado el ancho en función de la raiz cuadrada o
                        // un ancho específico
                        if (true)
                        {
                            Int32 wh = (Int32)Math.Sqrt(s01txt.Length / (iBits * 4));

                            if (wh < 1) return;

                            w = wh;
                            h = wh;
                        }
                        else
                        {
                            try
                            {
                                w = 552;
                                h = (s01txt.Length / (iBits * 4) / w);

                                if (h < 1) return;
                            }
                            catch (Exception ex) { return; }
                        }


                        Color[,] matrix = null;
                        try
                        {
                            matrix = NORMALMatrixFromBinaryStringXBitsColor(s01txt, iBits, w, h);
                            WriteableBitmap _bitmap = await WBitmapFromMATRIX(matrix);
                        }
                        catch (Exception ex) { return; }
                        try
                        {
                            matrix = NEXTMatrixFromBinaryStringXBitsColor(s01txt, iBits, w, h);
                            WriteableBitmap _bitmap = await WBitmapFromMATRIX(matrix);

                        }
                        catch (Exception ex) { return; }
                        try
                        {
                            matrix = SPIRAL01MatrixFromBinaryStringXBitsColor(s01txt, iBits, w, h);
                            WriteableBitmap _bitmap = await WBitmapFromMATRIX(matrix);
                        }
                        catch (Exception ex) { return; }
                        try
                        {
                            matrix = SPIRAL02MatrixFromBinaryStringXBitsColor(s01txt, iBits, w, h);
                            WriteableBitmap _bitmap = await WBitmapFromMATRIX(matrix);
                        }
                        catch (Exception ex) { return; }
                    }
                }
                catch(Exception ex)
                {
                    
                }
            }

            return;


            //StorageFolder assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            //StorageFile file = await assetsFolder.GetFileAsync("WhatIsTheQuest.pdf");

            //String s01 = String.Empty;
            //using (var stream = await file.OpenStreamForReadAsync())
            //{
            //    byte[] buffer = new byte[(int)stream.Length];
            //    await stream.ReadAsync(buffer, 0, buffer.Length);
            //    s01 = string.Join("", buffer.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
            //    String text = Encoding.UTF8.GetString(buffer);
            //}

            //String s01r = new String(s01.Reverse().ToArray());

            ////BigInteger bgNtemp = "1001".Aggregate(BigInteger.Zero, (s, a) => (s << 1) + a - '0');
            //BigInteger bgN = s01r.Aggregate(BigInteger.Zero, (s, a) => (s << 1) + a - '0');

            


            //for (Int32 div = 1; div < 255; div++)
            //{
            //    BigInteger divbgN = bgN / div;
            //    String rDivS = ToBinaryString(divbgN);

            //    for (Int32 offset = 14; offset < 15; offset++)
            //    {
            //        try
            //        {
            //            //Color[,] matrix0 = GenerateNORMALatrixFromBinaryString1BitPaleta(rDivS, bits);
            //            //await GenerateBitmapFromBinaryString(matrix0, "NORMAL", true);
            //            //Color[,] matrix = GenerateSpiralMatrixFromBinaryStringPaleta(rDivS, bits);
            //            //await GenerateBitmapFromBinaryString(matrix, "SPIRO", true);
            //            Color[,] matrix2 = GenerateCIRCUITOEatrixFromBinaryString1BitPaleta(rDivS, bits);
            //            await GenerateBitmapFromBinaryString(matrix2, "CIRCUITO", true);
            //            await GenerateFRAMESMatrixFromBinaryStringXBitsIndexedColor(rDivS, 192, 2, true, div, offset);
            //            //await GenerateFRAMESMatrixFromBinaryStringXBitsIndexedColor(rDivS, 192, 2, false, div, offset);
            //            await GenerateFRAMESMatrixFromBinaryStringXBitsIndexedColor(rDivS, 192, 7, true, div, offset);
            //            //await GenerateFRAMESMatrixFromBinaryStringXBitsIndexedColor(rDivS, 192, 7, false, div, offset);
            //        }
            //        catch (Exception ex)
            //        {

            //        }
            //    }                

            //}

            //await apzCode.WriteableBitmapFromBitString24BN(s02, 269);

            //await apzCode.GenerateBitmapFromBitString(s02, 2033);
            //await apzCode.GenerateImageFromBitString24BN(s02, 269);
            //await apzCode.GenerateImageFromBitString24BN(s01, 7290);
            //await apzCode.GenerateImageFromBitString8BN(s01, 269);
            //Int32 raiz = (Int32)Math.Sqrt(s01.Length / 24);

            //for (int i = 129; i > 100; i--)
            //{
            //    var matrixCS01 = GenerateCIRCUITOEatrixFromBinaryStringBN(s01, i);
            //    await GenerateBitmapFromBinaryStringBN(matrixCS01);
            //}

            //await GenerateFRAMESMatrixFromBinaryString2BitsIndexedColor(s01);
            //for (Int32 bits = 1; bits < 25; bits++)
            //{
            //await GenerateFRAMESMatrixFromBinaryStringXBitsIndexedColor(s01, 7, false);

            //}
            //var matrix1 = GenerateCIRCUITOEatrixFromBinaryString1Bit(s01);
            //await GenerateBitmapFromBinaryString(matrix1);

            //var matrix2 = GenerateSpiralMatrixFromBinaryStringBN(s01);
            //await GenerateBitmapFromBinaryStringBN(matrix2);

            //var matrix3 = GenerateSpiralMatrixFromBinaryStringBNInvertida(s01);
            //await GenerateBitmapFromBinaryStringBN(matrix3);
            //return;
            //for (int with = 269; with < 3000; with++)
            //{
            //    try
            //    {





            //        //Int32 width24 = 26; //(Int32)Math.Sqrt((s01.Length - offset) / 24 / 72);
            //        WriteableBitmap wb = await GenerateBitmapFromBinaryString(s01, with, 0, 1, true);
            //        wb = null;

            //        //Int32 width8 = (Int32)Math.Sqrt((s01.Length - offset) / 8);
            //        //await GenerateBitmapFromBinaryString(s01, width8, offset, 8, true);
            //        //Int32 width1 = (Int32)Math.Sqrt((s01.Length - offset));
            //        //await GenerateBitmapFromBinaryString(s01, width1, offset, 1, true);
            //        GC.Collect();
            //        await Task.Delay(333);
            //    }
            //    catch (Exception ex)
            //    { 

            //    }
            //}

        }

        public static async Task<Boolean> GenerateBitmapFromBinaryString(Color[,] matrix, String name, Boolean isColor)
        {
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);

            WriteableBitmap bitmap = new WriteableBitmap(width, height);

            using (Stream stream = bitmap.PixelBuffer.AsStream())
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        stream.WriteByte(matrix[x, y].B);
                        stream.WriteByte(matrix[x, y].G);
                        stream.WriteByte(matrix[x, y].R);
                        stream.WriteByte(matrix[x, y].A);
                    }
                }
            }


            StorageFolder videosFolder = KnownFolders.VideosLibrary;
            StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
            StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
            var file = await vtfVideoImgsFolder.CreateFileAsync(name + width.ToString() + "x" + height.ToString() + (isColor ? "COLOR" : "BN") + ".png", CreationCollisionOption.GenerateUniqueName);
            

                if (file != null)
                {
                    ////Windows.Storage.CachedFileManager.DeferUpdates(file);
                    ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                    ////Windows.Storage.Provider.FileUpdateStatus status =
                    ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    ////{

                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                        Stream pixelStream = bitmap.PixelBuffer.AsStream();
                        byte[] pixels2 = new byte[pixelStream.Length];
                        await pixelStream.ReadAsync(pixels2, 0, pixels2.Length);

                        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                            (uint)bitmap.PixelWidth,
                                            (uint)bitmap.PixelHeight,
                                            96.0,
                                            96.0,
                                            pixels2);
                        await encoder.FlushAsync();
                    }
                }
                else
                {
                    throw new Exception("Error");
                }
            file = null;
            bitmap = null;
            GC.Collect();
            return true;
        }

        public static async Task<WriteableBitmap> GenerateBitmapFromBinaryStringBN(byte[,] matrix)
        {
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);

            WriteableBitmap bitmap = new WriteableBitmap(width, height);
            
            using (Stream stream = bitmap.PixelBuffer.AsStream())
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {         
                        byte bn = matrix[x,y];

                        stream.WriteByte(bn);
                        stream.WriteByte(bn);
                        stream.WriteByte(bn);
                        stream.WriteByte(255);  
                    }
                }
            }


            StorageFolder videosFolder = KnownFolders.VideosLibrary;
            StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
            StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
            var file = await vtfVideoImgsFolder.CreateFileAsync(width.ToString() + "x" + height.ToString() + "BN.png", CreationCollisionOption.GenerateUniqueName);

            if (file != null)
            {
                ////Windows.Storage.CachedFileManager.DeferUpdates(file);
                ////await Windows.Storage.FileIO.WriteTextAsync(file, file.Name);
                ////Windows.Storage.Provider.FileUpdateStatus status =
                ////    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                ////if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                ////{

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                    Stream pixelStream = bitmap.PixelBuffer.AsStream();
                    byte[] pixels2 = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels2, 0, pixels2.Length);

                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                        (uint)bitmap.PixelWidth,
                                        (uint)bitmap.PixelHeight,
                                        96.0,
                                        96.0,
                                        pixels2);
                    await encoder.FlushAsync();
                }
                ////}
                ////else
                ////{
                ////    throw new Exception("Error");
                ////}
            }
            else
            {
                throw new Exception("Error");
            }
            
            return bitmap;
        }

        public static byte BitStringToByte(string bitsString)
        {
            byte result = 0;

            for (int i = 0; i < bitsString.Length; i++)
            {
                if (bitsString[i] == '1')
                {
                    result += 255;
                }
            }

            return result;
        }

        public static Color[,] GenerateSpiralMatrixFromBinaryStringInvertida(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 24)); // tamaño de la matriz
            Color[,] matrix = new Color[n, n];

            // posición del centro de la matriz
            int x = 0;
            int y = n - 1;

            Boolean cambio = false;
            int iMas = 0;
            int iMAX = n - 1;
            Int32 direccion = 0;
            for (int i = 0; i < binaryString.Length; i += 24)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = Color.FromArgb(255,
                    Convert.ToByte(binaryString.Substring(i, 8), 2),
                    Convert.ToByte(binaryString.Substring(i + 8, 8), 2),
                    Convert.ToByte(binaryString.Substring(i + 16, 8), 2));

                // asignar el valor a la posición actual

                Int32 xTemp = (x < n) ? (x) : (n - 1);
                Int32 yTemp = (y < n) ? (y) : (n - 1);

                xTemp = (xTemp > -1) ? (xTemp) : 0;
                yTemp = (yTemp > -1) ? (yTemp) : 0;

                matrix[xTemp, yTemp] = value;

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y--;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y++;
                }

                if (iMas == iMAX)
                {
                    direccion++;
                    iMas = 0;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }

                    if (cambio)
                    {
                        iMAX--;
                    }
                    cambio = !cambio;
                }
                iMas++;
                


            }

            return matrix;
        }

        public static Byte[,] GenerateCIRCUITOEatrixFromBinaryStringBN(string binaryString, int with)
        {
            int h = (binaryString.Length / with) + 1;

            Byte[,] matrix = new byte[with, h];

            int x = 0;
            int y = 0;

            Boolean derecha = true;
            for (int i = 0; i < binaryString.Length; i++)
            {
                
                matrix[x, y] = binaryString[i].ToString() == "1" ? (Byte)255 : (Byte)0;

                if (derecha)
                {
                    x++;
                }
                else
                {
                    x--;
                }
                if (x < 0 || x > with - 1)
                {
                    if (derecha)
                    {
                        x--;
                    }
                    else
                    {
                        x++;
                    }

                    derecha = !derecha;
                    y++;
                }

            }
            return matrix;
        }

        public static Color[,] GenerateNORMALatrixFromBinaryString1BitPaleta(string binaryString, Int32 bits)
        {
            Color[] paleta = GenerarPaleta((Int32)Math.Pow(2, bits));

            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / bits));

            Color[,] matrix = new Color[n, n];

            int x = 0;
            int y = 0;
                        
            for (int i = 0; i < binaryString.Length; i += bits)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = paleta[Convert.ToInt32(i + bits < binaryString.Length ? binaryString.Substring(i, bits) : binaryString.Substring(i), 2)];

                matrix[x, y] = value;
                x++;
                
                if (x < 0 || x > n - 1)
                {
                    x = 0;                    
                    y++;

                    if(y > n - 1)
                    {
                        return matrix;
                    }

                }

            }
            return matrix;
        }

        public static Color[,] GenerateCIRCUITOEatrixFromBinaryString1BitPaleta(string binaryString, Int32 bits)
        {
            Color[] paleta = GenerarPaleta((Int32)Math.Pow(2, bits));

            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / bits));

            Color[,] matrix = new Color[n, n];

            int x = 0;
            int y = 0;

            Boolean derecha = true;
            for (int i = 0; i < binaryString.Length; i += bits)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = paleta[Convert.ToInt32(i + bits < binaryString.Length ? binaryString.Substring(i, bits) : binaryString.Substring(i), 2)];

                matrix[x, y] = value;

                if (derecha)
                {
                    x++;
                }
                else
                {
                    x--;
                }
                if (x < 0 || x > n - 1)
                {
                    if (derecha)
                    {
                        x--;
                    }
                    else
                    {
                        x++;
                    }

                    derecha = !derecha;
                    y++;
                }

            }
            return matrix;
        }

        public static Color[,] GenerateCIRCUITOEatrixFromBinaryString1Bit(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 4));

            Color[,] matrix = new Color[n, n];

            int x = 0;
            int y = 0;

            Boolean derecha = true;
            for (int i = 0; i < binaryString.Length; i += 4)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = Color.FromArgb(
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 3, 1), 2) * 255),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i, 1), 2) * 255),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 1, 1), 2) * 255),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 2, 1), 2) * 255));

                matrix[x, y] = value;

                if (derecha)
                {
                    x++;
                }
                else
                {
                    x--;
                }
                if (x < 0 || x > n - 1)
                {
                    if (derecha)
                    {
                        x--;
                    }
                    else
                    {
                        x++;
                    }

                    derecha = !derecha;
                    y++;
                }

            }
            return matrix;
        }

        public static Color[,] GenerateCIRCUITOEatrixFromBinaryString(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 24));

            Color[,] matrix = new Color[n, n];

            int x = 0;
            int y = 0;
                        
            Boolean derecha = true;
            for (int i = 0; i < binaryString.Length; i += 24)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = Color.FromArgb(255,
                    Convert.ToByte(binaryString.Substring(i, 8), 2),
                    Convert.ToByte(binaryString.Substring(i + 8, 8), 2),
                    Convert.ToByte(binaryString.Substring(i + 16, 8), 2));

                matrix[x, y] = value;
               
                if (derecha)
                {
                    x++;
                }
                else
                {
                    x--;
                }
                if (x < 0 || x > n - 1)
                {
                    if(derecha)
                    {
                        x--;
                    }
                    else
                    {
                        x++;
                    }

                    derecha = !derecha;
                    y++;
                }

            }
            return matrix;
        }

        public static Color[,] GeneratePIRAMIDEatrixFromBinaryString(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 24));
            
            Color[,] matrix = new Color[n, n];

            int x = 0;
            int y = 0;

            Int32 limitx = 0;
                        
            for (int i = 0; i < binaryString.Length; i += 24)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = Color.FromArgb(255,
                    Convert.ToByte(binaryString.Substring(i, 8), 2),
                    Convert.ToByte(binaryString.Substring(i + 8, 8), 2),
                    Convert.ToByte(binaryString.Substring(i + 16, 8), 2));

                try
                {
                    matrix[x, y] = value;
                }
                catch { return matrix; }
                x++;

                if (x + limitx > n - 1)
                {
                    limitx++;
                    x = limitx;
                    y++;
                }

            }

            return matrix;
        }

        public static Color[,] GenerateCRECHENTOMatrixFromBinaryString1bit(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 4));

            n = (Int32)((double)n * Math.Sqrt(2)) + 1;

            Color[,] matrix = new Color[n, n];

            int x = 0;
            int y = 0;

            Int32 ultX = x;
            Int32 ultY = y;

            Boolean newLine = true;
            for (int i = 0; i < binaryString.Length; i += 4)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = Color.FromArgb((Byte)(Convert.ToInt32(binaryString.Substring(i, 1), 2) * 255),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 1, 1), 2) * 255),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 2, 1), 2) * 255),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 3, 1), 2)* 255));

                matrix[x, y] = value;
                if (newLine)
                {
                    x++;
                    ultX = x;
                    y = 0;
                    newLine = false;
                }
                else
                {
                    y++;
                    x--;
                    newLine = false;
                }

                if (x < 0 || y >= n)
                {
                    ultX++;
                    x = ultX;
                    y = 0;
                }
            }

            return matrix;
        }

        public static Color[,] GenerateCRECHENTOMatrixFromBinaryString(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 24));

            n = (Int32)((double)n * Math.Sqrt(2)) + 1;

            Color[,] matrix = new Color[n,n];

            int x = 0;
            int y = 0;

            Int32 ultX = x;
            Int32 ultY = y;

            Boolean newLine = true;
            for (int i = 0; i < binaryString.Length; i += 24)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = Color.FromArgb(255,
                    Convert.ToByte(binaryString.Substring(i, 8), 2),
                    Convert.ToByte(binaryString.Substring(i + 8, 8), 2),
                    Convert.ToByte(binaryString.Substring(i + 16, 8), 2));

                matrix[x, y] = value;
                if (newLine)
                {
                    x++;
                    ultX = x;
                    y = 0;
                    newLine = false;
                }
                else
                {
                    y++;
                    x--;
                    newLine = false;
                }       
                
                if(x < 0 || y >= n)
                {
                    ultX++;
                    x = ultX;
                    y = 0;                    
                }       
            }

            return matrix;
        }

        //public static Color[] GenerarPaleta(int nColores)
        //{
        //    Color[] paleta = new Color[nColores];
        //    int valorIncrementalRojo = 256 / (int)Math.Ceiling(Math.Sqrt(nColores));
        //    int valorIncrementalVerde = 256 / (int)Math.Ceiling(Math.Sqrt(nColores));
        //    int valorIncrementalAzul = 256 / (int)Math.Ceiling(Math.Sqrt(nColores));
        //    int contadorColor = 0;
        //    for (int i = 0; i < (int)Math.Ceiling(Math.Sqrt(nColores)); i++)
        //    {
        //        for (int j = 0; j < (int)Math.Ceiling(Math.Sqrt(nColores)); j++)
        //        {
        //            int rojo = i * valorIncrementalRojo;
        //            int verde = j * valorIncrementalVerde;
        //            int azul = contadorColor * valorIncrementalAzul;
        //            paleta[contadorColor] = Color.FromArgb(255, (Byte)rojo, (Byte)verde, (Byte)azul);
        //            contadorColor++;
        //            if (contadorColor >= nColores)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    return paleta;
        //}

        public static Color[] GenerarPaleta(int nColores)
        {
            Color[] paleta = new Color[nColores];
            int valorIncremental = 256 / (int)Math.Ceiling(Math.Sqrt(nColores));
            int contadorColor = 0;
            for (int i = 0; i < (int)Math.Ceiling(Math.Sqrt(nColores)); i++)
            {
                for (int j = 0; j < (int)Math.Ceiling(Math.Sqrt(nColores)); j++)
                {
                    int rojo = i * valorIncremental;
                    int verde = j * valorIncremental;
                    int azul = contadorColor * valorIncremental;
                    if (contadorColor < nColores)
                    {
                        paleta[contadorColor] = Color.FromArgb(255, (Byte)rojo, (Byte)verde, (Byte)azul);
                        contadorColor++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return paleta;
        }

        public static Color[] GenerarPaletaBN(int nColores)
        {
            Color[] paleta = new Color[nColores];
            double valorIncremental = 255 / (nColores - 1);
            int contadorColor = 0;
            for (int i = 0; i < nColores; i++)
            {
                int colorBN = (Int32)(i * valorIncremental);
                paleta[contadorColor] = Color.FromArgb(255, (Byte)colorBN, (Byte)colorBN, (Byte)colorBN);
                contadorColor++;
                
            }
            return paleta;
        }


        public static async Task<Boolean> GenerateFRAMESMatrixFromBinaryStringXBitsIndexedColor(string binaryString, Int32 withd, Int32 bits, Boolean isColors, Int32 div, Int32 offset)
        {
            Color[] paleta = GenerarPaleta(Convert.ToInt32(Math.Pow(2, bits)));
            if (!isColors)
            {
                paleta = GenerarPaletaBN(Convert.ToInt32(Math.Pow(2, bits)));
            }

            Int32 w = 192;
            Int32 h = 160; // (binaryString.Length / bits / w) + 3;
            Color[,] bitmapColors = new Color[w, h];

            // posición del centro de la matriz
            int x = 0;
            int y = 0; ;

            for (int i = offset; i + bits < binaryString.Length; i += bits + offset)
            {
                Int32 cInt = Convert.ToInt32(i + bits < binaryString.Length ? binaryString.Substring(i, bits) : binaryString.Substring(i), 2);
                // convertir el carácter de la cadena binaria a byte
                Color value = paleta[cInt];
                                
                bitmapColors[x, y] = value;
                x++;
                if (x > w - 1)
                {
                    y++;
                    x = 0;
                }
                if (y > h - 1)
                {
                    await GenerateBitmapFromBinaryString(bitmapColors, div.ToString().PadLeft(3, '0') + "_DIV_" + offset.ToString().PadLeft(3, '0') + "offset" + bits + "bits", isColors);
                    bitmapColors = new Color[w, h];
                    x = 0;
                    y = 0;
                }


            }

            await GenerateBitmapFromBinaryString(bitmapColors, div.ToString().PadLeft(3, '0') + "_DIV_" + offset.ToString().PadLeft(3, '0') +"offset" + bits + "bits", isColors);
            return true;
        }

        public static async Task<Boolean> GenerateFRAMESMatrixFromBinaryString2BitsIndexedColor(string binaryString)
        {
            Int32 w = 192;
            Int32 h = 160;
            Color[,] bitmapColors = new Color[w, h];

            // posición del centro de la matriz
            int x = 0;
            int y = 0; ;
                                    
            for (int i = 0; i < binaryString.Length; i += 2)
            {
                Int32 cInt = Convert.ToInt32(binaryString.Substring(i, 2), 2);
                // convertir el carácter de la cadena binaria a byte
                Color value =
                    cInt == 0 ? Colors.White :
                    cInt == 1 ? Colors.Red :
                    cInt == 2 ? Colors.Black :
                    Colors.Cyan;

                // asignar el valor a la posición actual


                bitmapColors[x, y] = value;
                x++;
                if(x > w - 1)
                {
                    y++;
                    x = 0;
                }
                if(y > h - 1)
                {
                    await GenerateBitmapFromBinaryString(bitmapColors, "2Bits", true);
                    bitmapColors = new Color[w, h];
                    x = 0;
                    y = 0;
                }


            }

            await GenerateBitmapFromBinaryString(bitmapColors, "2Bits", true);
            return true;
        }

        public static Color[,] GenerateSpiralMatrixFromBinaryString2BitsIndexedColor(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 2)); // tamaño de la matriz
            Color[,] matrix = new Color[n, n];

            // posición del centro de la matriz
            int x = n / 2;
            int y = n / 2; ;

            Boolean cambio = false;
            int iMas = 0;
            int iMAX = 1;
            Int32 direccion = 0;
            for (int i = 0; i < binaryString.Length; i += 2)
            {
                Int32 cInt = Convert.ToInt32(binaryString.Substring(i, 2), 2);
                // convertir el carácter de la cadena binaria a byte
                Color value = 
                    cInt == 0 ? Colors.White :
                    cInt == 1 ? Colors.Red : 
                    cInt == 2 ? Colors.Black : 
                    Colors.Cyan;

                // asignar el valor a la posición actual

                Int32 xTemp = (x < n) ? (x) : (n - 1);
                Int32 yTemp = (y < n) ? (y) : (n - 1);

                xTemp = (xTemp > -1) ? (xTemp) : 0;
                yTemp = (yTemp > -1) ? (yTemp) : 0;

                matrix[xTemp, yTemp] = value;

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y--;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y++;
                }

                if (iMas == iMAX)
                {
                    direccion++;
                    iMas = 0;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }

                    if (cambio)
                    {
                        iMAX++;
                    }
                    cambio = !cambio;
                }
                iMas++;



            }

            return matrix;
        }

        public static Color[,] GenerateSpiralMatrixFromBinaryString1BitsColor(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 4)); // tamaño de la matriz
            Color[,] matrix = new Color[n, n];

            // posición del centro de la matriz
            int x = n / 2;
            int y = n / 2; ;

            Boolean cambio = false;
            int iMas = 0;
            int iMAX = 1;
            Int32 direccion = 0;
            for (int i = 0; i < binaryString.Length; i += 4)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = Color.FromArgb(
                    (Byte)(Convert.ToInt32(binaryString.Substring(i, 1), 2) * 255),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 1, 1), 2) * 255),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 2, 1), 2) * 255),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 3, 1), 2) * 255)
                    );

                // asignar el valor a la posición actual

                Int32 xTemp = (x < n) ? (x) : (n - 1);
                Int32 yTemp = (y < n) ? (y) : (n - 1);

                xTemp = (xTemp > -1) ? (xTemp) : 0;
                yTemp = (yTemp > -1) ? (yTemp) : 0;

                matrix[xTemp, yTemp] = value;

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y--;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y++;
                }

                if (iMas == iMAX)
                {
                    direccion++;
                    iMas = 0;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }

                    if (cambio)
                    {
                        iMAX++;
                    }
                    cambio = !cambio;
                }
                iMas++;



            }

            return matrix;
        }

        public static Color[,] GenerateSpiralMatrixFromBinaryString2BitsColor(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 8)); // tamaño de la matriz
            Color[,] matrix = new Color[n, n];

            // posición del centro de la matriz
            int x = n / 2;
            int y = n / 2; ;

            Boolean cambio = false;
            int iMas = 0;
            int iMAX = 1;
            Int32 direccion = 0;
            for (int i = 0; i < binaryString.Length; i += 8)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = Color.FromArgb(
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 6, 2), 2) * 85),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i, 2), 2) * 85),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 2, 2), 2) * 85),
                    (Byte)(Convert.ToInt32(binaryString.Substring(i + 4, 2), 2) * 85)
                    );

                // asignar el valor a la posición actual

                Int32 xTemp = (x < n) ? (x) : (n - 1);
                Int32 yTemp = (y < n) ? (y) : (n - 1);

                xTemp = (xTemp > -1) ? (xTemp) : 0;
                yTemp = (yTemp > -1) ? (yTemp) : 0;

                matrix[xTemp, yTemp] = value;

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y--;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y++;
                }

                if (iMas == iMAX)
                {
                    direccion++;
                    iMas = 0;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }

                    if (cambio)
                    {
                        iMAX++;
                    }
                    cambio = !cambio;
                }
                iMas++;



            }

            return matrix;
        }

        public static Color[,] GenerateSpiralMatrixFromBinaryString6BitsColor(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 24)); // tamaño de la matriz
            Color[,] matrix = new Color[n, n];

            // posición del centro de la matriz
            int x = n / 2;
            int y = n / 2; ;

            Boolean cambio = false;
            int iMas = 0;
            int iMAX = 1;
            Int32 direccion = 0;
            for (int i = 0; i < binaryString.Length; i += 24)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = Color.FromArgb(
                    (Byte)Math.Round(Convert.ToInt32(binaryString.Substring(i + 18, 6), 2) * 4.03),
                    (Byte)Math.Round(Convert.ToInt32(binaryString.Substring(i, 6), 2) * 4.03),
                    (Byte)Math.Round(Convert.ToInt32(binaryString.Substring(i + 6, 6), 2) * 4.03),
                    (Byte)Math.Round(Convert.ToInt32(binaryString.Substring(i + 12, 6), 2) * 4.03)
                    );

                // asignar el valor a la posición actual

                Int32 xTemp = (x < n) ? (x) : (n - 1);
                Int32 yTemp = (y < n) ? (y) : (n - 1);

                xTemp = (xTemp > -1) ? (xTemp) : 0;
                yTemp = (yTemp > -1) ? (yTemp) : 0;

                matrix[xTemp, yTemp] = value;

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y--;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y++;
                }

                if (iMas == iMAX)
                {
                    direccion++;
                    iMas = 0;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }

                    if (cambio)
                    {
                        iMAX++;
                    }
                    cambio = !cambio;
                }
                iMas++;



            }

            return matrix;
        }

        public static Color[,] GenerateSpiralMatrixFromBinaryStringPaleta(string binaryString, Int32 bits)
        {
            Color[] paleta = GenerarPaleta((Int32)Math.Pow(2, bits));

            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / bits)); // tamaño de la matriz
            Color[,] matrix = new Color[n, n];

            // posición del centro de la matriz
            int x = n / 2;
            int y = n / 2; ;

            Boolean cambio = false;
            int iMas = 0;
            int iMAX = 1;
            Int32 direccion = 0;
            for (int i = 0; i < binaryString.Length; i += bits)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = paleta[Convert.ToInt32(i + bits < binaryString.Length ? binaryString.Substring(i, bits) : binaryString.Substring(i), 2)];

                // asignar el valor a la posición actual

                Int32 xTemp = (x < n) ? (x) : (n - 1);
                Int32 yTemp = (y < n) ? (y) : (n - 1);

                xTemp = (xTemp > -1) ? (xTemp) : 0;
                yTemp = (yTemp > -1) ? (yTemp) : 0;

                matrix[xTemp, yTemp] = value;

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y--;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y++;
                }

                if (iMas == iMAX)
                {
                    direccion++;
                    iMas = 0;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }

                    if (cambio)
                    {
                        iMAX++;
                    }
                    cambio = !cambio;
                }
                iMas++;



            }

            return matrix;
        }

        public static Color[,] GenerateSpiralMatrixFromBinaryString(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length / 24)); // tamaño de la matriz
            Color[,] matrix = new Color[n, n];

            // posición del centro de la matriz
            int x = n / 2;
            int y = n / 2; ;

            Boolean cambio = false;
            int iMas = 0;
            int iMAX = 1;
            Int32 direccion = 0;
            for (int i = 0; i < binaryString.Length; i+=24)
            {
                // convertir el carácter de la cadena binaria a byte
                Color value = Color.FromArgb(255,
                    Convert.ToByte(binaryString.Substring(i,8), 2),
                    Convert.ToByte(binaryString.Substring(i + 8, 8), 2),
                    Convert.ToByte(binaryString.Substring(i + 16, 8), 2)
                    );

                // asignar el valor a la posición actual

                Int32 xTemp = (x < n) ? (x) : (n - 1);
                Int32 yTemp = (y < n) ? (y) : (n - 1);

                xTemp = (xTemp > -1) ? (xTemp) : 0;
                yTemp = (yTemp > -1) ? (yTemp) : 0;

                matrix[xTemp, yTemp] = value;

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y--;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y++;
                }

                if (iMas == iMAX)
                {
                    direccion++;
                    iMas = 0;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }

                    if (cambio)
                    {
                        iMAX++;
                    }
                    cambio = !cambio;
                }
                iMas++;
                


            }

            return matrix;
        }

        public static byte[,] GenerateSpiralMatrixFromBinaryStringBNInvertida(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length)); // tamaño de la matriz
            byte[,] matrix = new byte[n, n];

            // posición del centro de la matriz
            int x = 0;
            int y = n - 1;

            Boolean cambio = false;
            int iMas = 0;
            int iMAX = n - 1;
            Int32 direccion = 0;
            for (int i = 0; i < binaryString.Length; i++)
            {
                // convertir el carácter de la cadena binaria a byte
                byte value = binaryString[i] == '1' ? (byte)255 : (byte)0;

                // asignar el valor a la posición actual

                Int32 xTemp = (x < n) ? (x) : (n - 1);
                Int32 yTemp = (y < n) ? (y) : (n - 1);

                xTemp = (xTemp > -1) ? (xTemp) : 0;
                yTemp = (yTemp > -1) ? (yTemp) : 0;

                matrix[xTemp, yTemp] = value;

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y--;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y++;
                }

                if (iMas == iMAX)
                {
                    direccion++;
                    iMas = 0;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }

                    if (cambio)
                    {
                        iMAX--;
                    }

                    cambio = !cambio;
                }
                iMas++;


            }

            return matrix;
        }

        public static byte[,] GenerateSpiralMatrixFromBinaryStringBN(string binaryString)
        {
            int n = (int)Math.Ceiling(Math.Sqrt(binaryString.Length)); // tamaño de la matriz
            byte[,] matrix = new byte[n, n];

            // posición del centro de la matriz
            int x = n / 2;
            int y = n / 2; ;

            Boolean cambio = false;
            int iMas = 0;
            int iMAX = 1;
            Int32 direccion = 0;
            for (int i = 0; i < binaryString.Length; i++)
            {
                // convertir el carácter de la cadena binaria a byte
                byte value = binaryString[i] == '1' ? (byte)255 : (byte)0;

                // asignar el valor a la posición actual

                Int32 xTemp = (x < n) ? (x) : (n - 1);
                Int32 yTemp = (y < n) ? (y) : (n - 1);

                xTemp = (xTemp > -1) ? (xTemp) : 0;
                yTemp = (yTemp > -1) ? (yTemp) : 0;

                matrix[xTemp, yTemp] = value;

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y--;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y++;
                }

                if (iMas == iMAX)
                {
                    direccion++;
                    iMas = 0;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }

                    if (cambio)
                    {
                        iMAX++;                        
                    }

                    cambio = !cambio;
                }
                iMas++;


            }

            return matrix;
        }

        private async static Task<WriteableBitmap> WBitmapFromMATRIX(Color[,] matrix)
        {
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);

            WriteableBitmap bitmap = new WriteableBitmap(width, height);

            using (Stream stream = bitmap.PixelBuffer.AsStream())
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        stream.WriteByte(matrix[x, y].B);
                        stream.WriteByte(matrix[x, y].G);
                        stream.WriteByte(matrix[x, y].R);
                        stream.WriteByte(matrix[x, y].A);
                    }
                }
            }

            StorageFolder videosFolder = KnownFolders.VideosLibrary;
            StorageFolder vtfFolder = await videosFolder.CreateFolderAsync("VideoToFile", CreationCollisionOption.OpenIfExists);
            StorageFolder vtfVideoImgsFolder = await vtfFolder.CreateFolderAsync("WhatIsTheQuest", CreationCollisionOption.OpenIfExists);
            var file = await vtfVideoImgsFolder.CreateFileAsync(width.ToString() + "x" + height.ToString() + "COLOR.png", CreationCollisionOption.GenerateUniqueName);

            if (file != null)
            {
                String[] extensionTemp = file.Name.Split('.');

                String fileExtension = extensionTemp[extensionTemp.Length - 1].ToLower();

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(                        
                        BitmapEncoder.PngEncoderId                        
                        , stream);
                    Stream pixelStream = bitmap.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight,
                                        (uint)bitmap.PixelWidth,
                                        (uint)bitmap.PixelHeight,
                                        96.0,
                                        96.0,
                                        pixels);
                    await encoder.FlushAsync();
                }
            }

            return bitmap;
        }

        public static Color[,] NORMALMatrixFromBinaryStringXBitsColor(string binaryString, Int32 bits, Int32 w, Int32 h)
        {
            double increment = 255 / (Math.Pow(2, bits) - 1);
            Color[,] matrix = new Color[w, h];

            int x = 0;
            int y = 0;

            for (int i = 0; i + (bits * 4) <= binaryString.Length; i += (bits * 4))
            {
                CMYK cmyk = new CMYK();

                cmyk.K = (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 3), bits), 2) * increment);
                cmyk.C = (Byte)(Convert.ToInt32(binaryString.Substring(i, bits), 2) * increment);
                cmyk.M = (Byte)(Convert.ToInt32(binaryString.Substring(i + bits, bits), 2) * increment);
                cmyk.Y = (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 2), bits), 2) * increment);

                RGB rgb = apzCode.CMYKToRGB(cmyk);
                Color value = Color.FromArgb(
                    255,
                    rgb.R,
                    rgb.G,
                    rgb.B);

                // convertir el carácter de la cadena binaria a byte
                //Color value = Color.FromArgb(
                //    (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 3), bits), 2) * increment),
                //    (Byte)(Convert.ToInt32(binaryString.Substring(i, bits), 2) * increment),
                //    (Byte)(Convert.ToInt32(binaryString.Substring(i + bits, bits), 2) * increment),
                //    (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 2), bits), 2) * increment));

                matrix[x, y] = value;
                x++;

                if (x < 0 || x > w - 1)
                {
                    x = 0;
                    y++;

                    if (y > h - 1)
                    {
                        return matrix;
                    }

                }

            }
            return matrix;
        }

        public static Color[,] NEXTMatrixFromBinaryStringXBitsColor(string binaryString, Int32 bits, Int32 w, Int32 h)
        {
            double increment = 255 / (Math.Pow(2, bits) - 1);

            Color[,] matrix = new Color[w, h];

            int x = 0;
            int y = 0;

            Boolean derecha = true;
            for (int i = 0; i + (bits * 4) <= binaryString.Length; i += bits * 4)
            {
                CMYK cmyk = new CMYK();

                cmyk.K = (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 3), bits), 2) * increment);
                cmyk.C = (Byte)(Convert.ToInt32(binaryString.Substring(i, bits), 2) * increment);
                cmyk.M = (Byte)(Convert.ToInt32(binaryString.Substring(i + bits, bits), 2) * increment);
                cmyk.Y = (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 2), bits), 2) * increment);

                RGB rgb = apzCode.CMYKToRGB(cmyk);
                Color value = Color.FromArgb(
                    255,
                    rgb.R,
                    rgb.G,
                    rgb.B);

                //// convertir el carácter de la cadena binaria a byte
                //Color value = Color.FromArgb(
                //    (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 3), bits), 2) * increment),
                //    (Byte)(Convert.ToInt32(binaryString.Substring(i, bits), 2) * increment),
                //    (Byte)(Convert.ToInt32(binaryString.Substring(i + bits, bits), 2) * increment),
                //    (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 2), bits), 2) * increment));

                matrix[x, y] = value;

                if (derecha)
                {
                    x++;
                }
                else
                {
                    x--;
                }
                if (x < 0 || x > w - 1)
                {
                    if (derecha)
                    {
                        x--;
                    }
                    else
                    {
                        x++;
                    }

                    derecha = !derecha;
                    y++;

                    if (y > h - 1)
                    {
                        return matrix;
                    }

                }

            }
            return matrix;
        }

        public static Color[,] SPIRAL01MatrixFromBinaryStringXBitsColor(string binaryString, Int32 bits, Int32 w, Int32 h)
        {
            double increment = 255 / (Math.Pow(2, bits) - 1);

            Color[,] matrix = new Color[w, h];

            // posición del centro de la matriz
            int x = (w / 2);
            int y = (h / 2);

            Boolean cambio = false;
            int iMas = 0;
            int iMAX = 1;
            Int32 direccion = 0;
            for (int i = 0; i + (bits * 4) <= binaryString.Length; i += bits * 4)
            {
                CMYK cmyk = new CMYK();

                cmyk.K = (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 3), bits), 2) * increment);
                cmyk.C = (Byte)(Convert.ToInt32(binaryString.Substring(i, bits), 2) * increment);
                cmyk.M = (Byte)(Convert.ToInt32(binaryString.Substring(i + bits, bits), 2) * increment);
                cmyk.Y = (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 2), bits), 2) * increment);

                RGB rgb = apzCode.CMYKToRGB(cmyk);
                Color value = Color.FromArgb(
                    255,
                    rgb.R,
                    rgb.G,
                    rgb.B);

                ////// convertir el carácter de la cadena binaria a byte
                ////Color value = Color.FromArgb(
                ////    (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 3), bits), 2) * increment),
                ////    (Byte)(Convert.ToInt32(binaryString.Substring(i, bits), 2) * increment),
                ////    (Byte)(Convert.ToInt32(binaryString.Substring(i + bits, bits), 2) * increment),
                ////    (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 2), bits), 2) * increment)
                ////    );

                // asignar el valor a la posición actual

                Int32 xTemp = (x < w) ? (x) : (w - 1);
                Int32 yTemp = (y < h) ? (y) : (h - 1);

                xTemp = (xTemp > -1) ? (xTemp) : 0;
                yTemp = (yTemp > -1) ? (yTemp) : 0;

                matrix[xTemp, yTemp] = value;

                if (iMas == iMAX)
                {
                    direccion++;
                    iMas = 0;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }

                    if (cambio)
                    {
                        iMAX++;
                    }
                    cambio = !cambio;
                }

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y--;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y++;
                }


                iMas++;
            }

            return matrix;
        }

        public static Color[,] SPIRAL02MatrixFromBinaryStringXBitsColor(string binaryString, Int32 bits, Int32 w, Int32 h)
        {
            double increment = 255 / (Math.Pow(2, bits) - 1);

            Color[,] matrix = new Color[w, h];

            // posición del centro de la matriz
            int x = 0;
            int y = 0;

            Int32 xInit = 0;
            Int32 yInit = 1;
            Int32 wOffset = w;
            Int32 hOffset = h;
            Int32 direccion = 0;
            for (int i = 0; i + (bits * 4) <= binaryString.Length; i += bits * 4)
            {
                CMYK cmyk = new CMYK();

                cmyk.K = (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 3), bits), 2) * increment);
                cmyk.C = (Byte)(Convert.ToInt32(binaryString.Substring(i, bits), 2) * increment);
                cmyk.M = (Byte)(Convert.ToInt32(binaryString.Substring(i + bits, bits), 2) * increment);
                cmyk.Y = (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 2), bits), 2) * increment);

                RGB rgb = apzCode.CMYKToRGB(cmyk);
                Color value = Color.FromArgb(
                    255,
                    rgb.R,
                    rgb.G,
                    rgb.B);

                ////// convertir el carácter de la cadena binaria a byte
                ////Color value = Color.FromArgb(
                ////    (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 3), bits), 2) * increment),
                ////    (Byte)(Convert.ToInt32(binaryString.Substring(i, bits), 2) * increment),
                ////    (Byte)(Convert.ToInt32(binaryString.Substring(i + bits, bits), 2) * increment),
                ////    (Byte)(Convert.ToInt32(binaryString.Substring(i + (bits * 2), bits), 2) * increment));

                // asignar el valor a la posición actual

                matrix[x, y] = value;

                if (direccion == 0)
                {
                    x++;
                }
                else if (direccion == 1)
                {
                    y++;
                }
                else if (direccion == 2)
                {
                    x--;
                }
                else if (direccion == 3)
                {
                    y--;
                }

                if ((direccion == 2 && x < xInit) || (direccion == 3 && y < yInit) || (direccion == 0 && x >= wOffset) || (direccion == 1 && y >= hOffset))
                {
                    if (direccion == 2 && x < xInit)
                    {
                        x++;
                        y--;
                        xInit++;
                    }
                    else if (direccion == 3 && y < yInit)
                    {
                        y++;
                        x++;
                        yInit++;
                    }
                    else if (direccion == 0 && x >= wOffset)
                    {
                        x--;
                        y++;
                        wOffset--;
                    }
                    else if (direccion == 1 && y >= hOffset)
                    {
                        y--;
                        x--;
                        hOffset--;
                    }

                    direccion++;
                    if (direccion == 4)
                    {
                        direccion = 0;
                    }
                }


            }

            return matrix;
        }
    }
}
