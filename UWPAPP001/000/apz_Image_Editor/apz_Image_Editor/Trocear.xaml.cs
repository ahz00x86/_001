using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace apz_Image_Editor
{
    public sealed partial class Trocear : UserControl
    {
        List<WriteableBitmap> imagenesTroceadas = new List<WriteableBitmap>();

        public WriteableBitmap _wB { get; set; }
        public Trocear(WriteableBitmap wb)
        {
            _wB = wb;

            this.InitializeComponent();
        }

        private async void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Int32 cutRows = Convert.ToInt32(txtRows.Text);
            Int32 cutColumns = Convert.ToInt32(txtColumns.Text);
            Int32 totalImgs = cutRows * cutColumns;

            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            String fileType = cbFileType.SelectionBoxItem.ToString();
                        
            if (folder != null)
            {
                //TODO: Calcular with height a partir de los datos text,
                Int32 widthCut = _wB.PixelWidth / cutColumns;
                Int32 heightCut = _wB.PixelHeight / cutRows;
                                
                // Obtenemos el hexadecimal little endian para alto y ancho (es una imagen cuadrada)
                String wHex = Convert.ToString(widthCut, 16);
                wHex = wHex.Length % 2 == 0 ? wHex : String.Concat("0", wHex);
                wHex = SDK.LittleEndian(wHex);

                String hHex = Convert.ToString(heightCut, 16);
                hHex = hHex.Length % 2 == 0 ? hHex : String.Concat("0", hHex);
                hHex = SDK.LittleEndian(hHex);

                // Enlazamos y obtenemos los bytes de la cabecera
                String sHex = String.Concat(SDK.sHex32_0, wHex, hHex, SDK.sHex32_1);
                Byte[] bytesH = SDK.HexStringToByteArray(sHex);

                // Estos serán los datos de la imagen a construir
                // El tamaño de estos datos será del tamaño height por with
                // con un padding ajustado a bytes y con un mínimo de 4 bytes
                List<List<Byte>> cBinarios = new List<List<Byte>>();                
                for (Int32 j = 0; j < totalImgs; j++)
                {
                    cBinarios.Add(new List<Byte>());
                }

                Byte[] bytes = await SDK.WBToBytes(_wB);

                Int32 row = 0;
                Int32 col = 0;
                Int32 iInit = 0;
                Int32 i = 0;
                for (Int32 j = 54; j < bytes.Length; j += 4)
                {
                    var b = bytes[j];
                    var g = bytes[j + 1];
                    var r = bytes[j + 2];
                    var a = bytes[j + 3];

                    if (i < cBinarios.Count && col < widthCut * cutColumns) {

                        cBinarios[i].Add(r);
                        cBinarios[i].Add(g);
                        cBinarios[i].Add(b);
                        cBinarios[i].Add(a);
                    }

                    col++;
                    if (col % widthCut == 0)
                    {
                        i++;
                        if (i % cutColumns == 0)
                        {
                            i = iInit;
                        }
                        
                    }
                    //if(col > widthCut * cutColumns)
                    //{
                    //    col++;
                    //    j+=4;
                    //}

                    if (col != 0 && col % _wB.PixelWidth == 0)
                    {
                        row++;

                        if (row % heightCut == 0)
                        {
                            iInit += cutColumns;
                            i = iInit;
                        }

                        col = 0;
                    }
                }

                foreach (List<Byte> lsBytesD in cBinarios)
                {
                    Byte[] bytesD = lsBytesD.ToArray();

                    Byte[] resultBytes = new Byte[bytesH.Length + bytesD.Length];
                    Array.Copy(bytesH, resultBytes, bytesH.Length);
                    Array.Copy(bytesD, 0, resultBytes, bytesH.Length, bytesD.Length);

                    MemoryStream mS = new MemoryStream(resultBytes);

                    WriteableBitmap wb = new WriteableBitmap(widthCut, heightCut);
                    await wb.SetSourceAsync(mS.AsRandomAccessStream());

                    var fileT = await folder.CreateFileAsync(String.Concat("TrozoImagen", fileType), CreationCollisionOption.GenerateUniqueName);
                    using (IRandomAccessStream stream = await fileT.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(
                            fileType == ".jpg" ? BitmapEncoder.JpegEncoderId :
                            fileType == ".png" ? BitmapEncoder.PngEncoderId :
                            fileType == ".bmp" ? BitmapEncoder.BmpEncoderId : BitmapEncoder.JpegEncoderId
                            , stream);
                        Stream pixelStream = wb.PixelBuffer.AsStream();
                        byte[] pixels = new byte[pixelStream.Length];
                        await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight,
                                            (uint)wb.PixelWidth,
                                            (uint)wb.PixelHeight,
                                            96.0,
                                            96.0,
                                            pixels);
                        await encoder.FlushAsync();
                    }
                }
            }
            else
            {
                //this.textBlock.Text = "Operation cancelled.";
            }

            ContentDialog parent = (ContentDialog)this.Parent;
            parent.Hide();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog parent = (ContentDialog)this.Parent;
            parent.Hide();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Int32 cutColumns = Convert.ToInt32(((TextBox)sender).Text);

                if (cutColumns <= 0)
                {
                    ((TextBox)sender).Text = "1";
                }
                else if (cutColumns > 10)
                {
                    ((TextBox)sender).Text = "10";
                }
            }
            catch (Exception ex)
            {
                ((TextBox)sender).Text = "2";
            }
        }

        
    }
}
