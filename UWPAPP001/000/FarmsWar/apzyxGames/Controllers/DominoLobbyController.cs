using apzyxGames.Data;
using apzyxGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace apzyxGames.Controllers
{
    public class DominoLobbyController : ApiController
    {

        public IHttpActionResult Get()
        {
            List<DominoLobbyModel> dLobbys = DominoEngine.GetDominoLobbys();

            dLobbys.Add(new DominoLobbyModel() { Id = Guid.NewGuid(), Players = new List<DominoPlayerModel>() { new DominoPlayerModel() { IdPlayer = new Guid() } } });

            if(dLobbys == null || dLobbys.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(dLobbys);
            }
        }

        public IHttpActionResult Get(Int32 id, DominoLobbyModel player)
        {
            DominoLobbyModel dLobby = DominoEngine.GetDominoLobby(player.Id);
                                    
            if (dLobby == null)
            {
                return NotFound();
            }
            else
            {
                if (player.Players != null && player.Players.Count == 1)
                {
                    DominoPlayerModel dPM = new DominoPlayerModel();
                    dPM.IdPlayer = player.Players[0].IdPlayer;
                    dPM.Password = player.Players[0].Password;
                    dLobby.Players.Add(dPM);

                    return Ok(dLobby);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        public Boolean Put(Guid player)
        {

            return true;

        }
    }
}
