using System.Net.Sockets;

namespace TokensWay.Data.pDomino
{
    public class pDomino
    {
        public Int16[][] AllDominoTabs
        {
            get
            {
                return new Int16[28][] {
                new Int16[2] { 0, 0 }, new Int16[2] { 0, 1 }, new Int16[2] { 0, 2 }, new Int16[2] { 0, 3 }, new Int16[2] { 0, 4 }, new Int16[2] { 0, 5 }, new Int16[2] { 0, 6 },
                new Int16[2] { 1, 1 }, new Int16[2] { 1, 2 }, new Int16[2] { 1, 3 }, new Int16[2] { 1, 4 }, new Int16[2] { 1, 5 }, new Int16[2] { 1, 6 },
                new Int16[2] { 2, 2 }, new Int16[2] { 2, 3 }, new Int16[2] { 2, 4 }, new Int16[2] { 2, 5 }, new Int16[2] { 2, 6 },
                new Int16[2] { 3, 3 }, new Int16[2] { 3, 4 }, new Int16[2] { 3, 5 }, new Int16[2] { 3, 6 },
                new Int16[2] { 4, 4 }, new Int16[2] { 4, 5 }, new Int16[2] { 4, 6 },
                new Int16[2] { 5, 5 }, new Int16[2] { 5, 6 },
                new Int16[2] { 6, 6 }  };
            }
        }

        Socket socket;
        Thread listenThread;
        Dictionary<Guid, Socket> usersSockets = new Dictionary<Guid, Socket>();
        Dictionary<Guid, dUser> users = new Dictionary<Guid, dUser>();

        Dictionary<Guid, dRoom> WaitingRooms = new Dictionary<Guid, dRoom>();
        Dictionary<Guid, dRoom> PlayingRooms = new Dictionary<Guid, dRoom>();
    }
}
