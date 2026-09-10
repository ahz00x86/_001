using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace TXT_Great_Files
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Dictionary<BigInteger, String> fileSplit = new Dictionary<BigInteger, String>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            fileSplit = new Dictionary<BigInteger, String>();
            //cMain.Children.Clear();
            //cMain.DataContext = fileSplit;
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".txt");
            
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                this.txtFileName.Text = "txt: " + file.Name;
                Stream fileS = await file.OpenStreamForReadAsync();
                using (StreamReader writer = new StreamReader(fileS))
                {
                    BigInteger i = 0;
                    String s = writer.ReadLine();
                    while (s != null)
                    {
                        //fileSplit.Add(i, s);

                        cMain.Text += s;

                        s = writer.ReadLine();
                        i++;
                    }
                }

                
            }
            else
            {
                this.txtFileName.Text = "txt: empty / vacío";
            }
        }
    }
}
