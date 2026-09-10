using apzyxGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apzyxGames.Data
{
    public static class DominoEngine
    {
        private static List<DominoLobbyModel> _dominoLobbys = new List<DominoLobbyModel>();
        private static List<DominoMatchModel> _dominoMatchs = new List<DominoMatchModel>();
                
        public static List<DominoLobbyModel> GetDominoLobbys()
        {
            return _dominoLobbys;
        }

        public static DominoLobbyModel GetDominoLobby(Guid id)
        {
            return _dominoLobbys.Where(x => x.Id == id).FirstOrDefault();
        }

        public static void AddDominoLobby(DominoLobbyModel dLM)
        {
            _dominoLobbys.Add(dLM);
        }

    }
}