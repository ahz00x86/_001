using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace BIT_IMG
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ViewPage : Page
    {
        WriteableBitmap _bitmap;

        public async Task PixelBufferToWriteableBitmap(WriteableBitmap wb, byte[] bgra)
        {
            using (Stream stream = wb.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(bgra, 0, bgra.Length);
            }
        }

        public async Task<WriteableBitmap> PixelBufferToWriteableBitmap(byte[] bgra, int width, int height)
        {
            var wb = new WriteableBitmap(width, height);
            await PixelBufferToWriteableBitmap(wb, bgra);
            return wb;
        }

        public ViewPage()
        {
            this.InitializeComponent();                                    
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TransferWBitmap tnfWB = (TransferWBitmap)this.DataContext;
            imgVIEW.Source = await PixelBufferToWriteableBitmap(tnfWB.Data, tnfWB.Width, tnfWB.Heiht);
        }
    }
}
