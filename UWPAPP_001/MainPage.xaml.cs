using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Input;
using System.Drawing;
using Windows.Devices.Input;
using Windows.UI.Composition;
using Windows.UI.Popups;
using Windows.UI.Input;

namespace UWPAPP_001
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a <see cref="Frame">.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            List<String> menu = new List<String>() { "WIN NFT #1", "WIN NFT #2", "WIN NFT #3", "WIN NFT #4", "WIN NFT #5", "WIN NFT #6", "WIN NFT #7", "WIN NFT #1", "WIN NFT #2", "WIN NFT #3", "WIN NFT #4", "WIN NFT #5", "WIN NFT #6", "WIN NFT #7", "WIN NFT #1", "WIN NFT #2", "WIN NFT #3", "WIN NFT #4", "WIN NFT #5", "WIN NFT #6", "WIN NFT #7", "WIN NFT #1", "WIN NFT #2", "WIN NFT #3", "WIN NFT #4", "WIN NFT #5", "WIN NFT #6", "WIN NFT #7" };

            foreach(String mOp in menu)
            {
                Button btn = new Button();
                btn.Foreground = new SolidColorBrush(Colors.White);
                btn.Content = mOp;
                btn.HorizontalAlignment = HorizontalAlignment.Center;
                btn.VerticalAlignment = VerticalAlignment.Center;
                btn.Click += Btn_Click;
                btn.PointerMoved += Btn_PointerMoved;
                btn.PointerExited += Btn_PointerExited;
                spMenu.Children.Add(btn);
            }
        }

        private void Btn_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button btn = (Button)sender;
            TextBlock tB = (TextBlock)btn.DataContext;
            gMain.Children.Remove(tB);
            tB = null;
        }

        private void Btn_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.DataContext != null)
            {
                TextBlock tBAn = (TextBlock)btn.DataContext;
                gMain.Children.Remove(tBAn);
            }
            TextBlock tB = new TextBlock();
            tB.Text = (String)btn.Content;
            tB.Foreground = new SolidColorBrush(Colors.White);
            tB.HorizontalAlignment = HorizontalAlignment.Center;
            tB.VerticalAlignment = VerticalAlignment.Center;
            tB.SetValue(Grid.RowProperty, 0);
            tB.SetValue(Grid.ColumnProperty, 1);
            btn.DataContext = tB;
            gMain.Children.Add(tB);
        }

        private void Btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //
        }

    }
}
