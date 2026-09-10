using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmsWar.Classes
{
    [Serializable]
    public class EndDomino
    {
        public Boolean Winner { get; set; }

        public DominoTabServ FinalTab { get; set; }

        public DominoTabServ[] FinalPlayerTabs { get; set; }
    }
}
