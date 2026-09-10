using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmsWar.Classes
{
    [Serializable]
    public class Room : INotifyPropertyChanged
    {
        public Guid IdRoom { get; set; }

        public Guid MyIdUsuario { get; set; }

        public Int16 Position { get; set; }

        public Dictionary<Int16, Guid> Users { get; set; }

        public Dictionary<Int16, Boolean> UsersOcupation { get; set; }

        public Boolean Add { get; set; }

        public Boolean Update { get; set; }

        public Boolean Delete { get; set; }

        public Boolean DeleteRoom { get; set; }

        public Boolean Start { get; set; }

        public Int16 PuntTeam1 { get; set; }
        public Int16 PuntTeam2 { get; set; }
        public Int16 FinalPunt { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        public void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void resetStatesRoom()
        {
            Add = false;
            Update = false;
            Delete = false;
            DeleteRoom = false;
        }
        //public Int16 PlayerRound { get; set; }
        //public List<DominoTabServ> PlayersDominoTabs { get; set; }
        //public List<DominoTabServ> InGameDominoTabs { get; set; }
    }
}
