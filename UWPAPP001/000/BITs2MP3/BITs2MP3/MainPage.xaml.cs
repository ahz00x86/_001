using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Editing;
using Windows.Media.Transcoding;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI;
using System.Linq.Expressions;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace BITs2MP3
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Byte[] tempBytes { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Upload_FILE_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add("*");
            Windows.Storage.StorageFile fileP = await picker.PickSingleFileAsync();

            if (fileP != null)
            {
                IBuffer buffer = await FileIO.ReadBufferAsync(fileP);
                Byte[] bytes = buffer.ToArray();
                Boolean r = await BITs2MP3(bytes);
            }
        }

        public async Task<Boolean> BITs2MP3(byte[] audioData8Bit)
        {
            try
            {
                // Crea un array de bytes que representa los datos de audio en formato PCM de 8 bits
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation =
                    Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("MP3 files", new List<string>() { ".mp3" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "BITs2MP3.mp3";

                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    using (BinaryWriter wS = new BinaryWriter(await file.OpenStreamForWriteAsync()))
                    {
                        // Define las propiedades del archivo WAV, como la frecuencia de muestreo, el tamaño de muestra y el número de canales
                        int sampleRate = 15000; // Frecuencia de muestreo
                        short bitsPerSample = 16; // Tamaño de muestra en bits
                        short channels = 1; // canales

                        // Escribe la cabecera del archivo WAV
                        wS.Write("RIFF".ToCharArray()); // Identificador del archivo
                        wS.Write(36 + audioData8Bit.Length); // Tamaño del archivo en bytes
                        wS.Write("WAVE".ToCharArray()); // Formato del archivo

                        // Escribe el chunk "fmt "
                        wS.Write("fmt ".ToCharArray()); // Identificador del chunk
                        wS.Write((int)16); // Tamaño del chunk (16 para PCM)
                        wS.Write((short)1); // Formato de codificación (1 para PCM)
                        wS.Write(channels); // Número de canales
                        wS.Write(sampleRate); // Frecuencia de muestreo
                        wS.Write(sampleRate * channels * bitsPerSample / 8); // Tasa de bits
                        wS.Write((short)(channels * bitsPerSample / 8)); // Tamaño de bloque
                        wS.Write(bitsPerSample); // Tamaño de muestra en bits

                        // Escribe el chunk "data"
                        wS.Write("data".ToCharArray()); // Identificador del chunk
                        wS.Write(audioData8Bit.Length); // Tamaño del chunk de datos
                        wS.Write(audioData8Bit); // Datos de audio

                    }                                                                                
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async void btnToMP3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Byte[] bytes = Convert.(txtBinario.Text;
                BITs2MP3()

            }
            catch(Exception ex)
            {

            }
        }
    }
}
