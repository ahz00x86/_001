using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apzyxGames.Models
{
    public class DominoPlayerModel
    {
        public Guid IdPlayer { get; set; }

        public Guid Password { get; set; }

        public Boolean Turn { get; set; }

        public List<DominoTabModel> DominoTabs { get; set; }
    }
}