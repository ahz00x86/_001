using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apzyxGames.Models
{
    public class DominoLobbyModel
    {
        public Guid Id { get; set; }

        public List<DominoPlayerModel> Players { get; set; }

    }
}