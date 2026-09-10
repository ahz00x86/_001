using FarmsWar.Sprites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;

namespace FarmsWar.Classes
{
    public class DominoLobbyModel
    {
        public Guid Id { get; set; }

        public List<DominoPlayerModel> Players { get; set; }

    }

    public class DominoPlayerModel
    {
        public Guid IdPlayer { get; set; }

        public Guid Password { get; set; }

        public Boolean Turn { get; set; }

        public List<DominoTab> DominoTabs { get; set; }
    }

    public class DominoTab : INotifyPropertyChanged
    {

        public Guid Id { get; set; }

        public Guid IdRoom { get; set; }

        public Int16[] Values { get; set; }

        public UIElement Sprite { get; set; }

        public Int16 Player { get; set; }

        public double SourceX { get; set; }
        public double SourceY { get; set; }

        public Boolean Disabled { get; set; }

        public Boolean IsRotate { get; set; }

        public Boolean IsDouble { get { return this.Values[0] == this.Values[1]; } }

        public Boolean IsForA { get; set; }

        public Boolean IsForB { get; set; }
                
        private Boolean isBlack;        
        public Boolean IsBlack
        {
            get => isBlack;

            set
            {
                isBlack = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBlack)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
