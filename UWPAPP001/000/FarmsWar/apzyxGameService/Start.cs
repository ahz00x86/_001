using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmsWar.Classes
{
    [Serializable]
    public class Start
    {
        public Int16 PlayerRound { get; set; }

        public Dictionary<Int16, Guid> Users { get; set; }

        public DominoTabServ[] MyTabs { get; set; }

        public DominoTabServ[] FinalPlayerTabs { get; set; }

        public DominoTabServ FinalTab { get; set; }

        public Int16 PuntTeam1 { get; set; }

        public Int16 PuntTeam2 { get; set; }

        public Boolean IsFirst { get; set; }
    }
}
