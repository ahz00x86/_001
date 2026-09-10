using FarmsWar.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace FarmsWar.Sprites
{
    public sealed partial class DominoRoomSprite : UserControl
    {
        public event RoutedEventHandler ButtonClick;

        public DominoRoomSprite()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            if ((String)bt.Content == "Libre" || (String)bt.Content == "Salir")
            {
                if (this.ButtonClick != null)
                    this.ButtonClick(sender, e);
            }
        }
    }
}
