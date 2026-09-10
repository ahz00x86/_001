using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GWService
{

    [Serializable]
    public class DominoTabServ
    {
        public Guid Id { get; set; }

        public Guid IdRoom { get; set; }

        public Int16[] Values { get; set; }

        public Int16 Player { get; set; }

        public Boolean Disabled { get; set; }

        public Boolean IsRotate { get; set; }

        public Boolean IsForA { get; set; }

        public Boolean IsForB { get; set; }

        public Boolean Turn { get; set; }

    }

    [Serializable]
    public class Room
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
        public void resetStatesRoom()
        {
            Add = false;
            Update = false;
            Delete = false;
            DeleteRoom = false;
        }

        [NonSerialized]
        public Int16 PlayerRound;

        [NonSerialized]
        public List<DominoTabServ> PlayersDominoTabs;

        [NonSerialized]
        public List<DominoTabServ> InGameDominoTabs;

        [NonSerialized]
        public Int16 A;

        [NonSerialized]
        public Int16 B;

    }

    [Serializable]
    public class User
    {
        public Guid Id { get; set; }

        public Guid Password { get; set; }

        public Int16 State { get; set; }

        public DateTime LastUpdate { get; set; }

        public Room Room { get; set; }

    }

    [Serializable]
    public class Message
    {
        public User IdUser { get; set; }
        public String Mess { get; set; }
    }

    public class StateError
    {
        public Int16 GoToState { get; set; }

        public Boolean IsError { get; set; }

        public String Message { get; set; }
    }

    public class GWServer
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
        Dictionary<Guid, GWUser> users = new Dictionary<Guid, GWUser>();

        Dictionary<Guid, Room> WaitingRooms = new Dictionary<Guid, Room>();
        Dictionary<Guid, Room> PlayingRooms = new Dictionary<Guid, Room>();

        public GWServer()
        {
            //puerto 
            int PORT = 63495;
            //IP Localhost
            IPAddress ipAddress = Dns.GetHostEntry("192.168.1.46").AddressList[1];
            Console.WriteLine("Server> " + ipAddress);
            Console.WriteLine("Server> Corriendo");

            //para la comunicacion de red
            //Inicializa una nueva instancia con la familia de direcciones, tipo de socket y protocolo de la clase.
            socket = new Socket(AddressFamily.InterNetwork,//Dirección para IP versión 4
                                        SocketType.Stream, //Admite secuencias de bytes bidireccionales, usa el protocolo TCP 
                                        ProtocolType.Tcp);//Protocolo de Control de Transmisión.                
            //Contiene información de puerto local o remoto y host que necesita una aplicación 
            //para conectarse a un servicio en un host. La clase IPEndPoint forma un punto de conexión a un servicio
            IPEndPoint myAdress = new IPEndPoint(ipAddress, PORT);

            try
            {
                socket.Bind(myAdress);//Asocia un Socket con un extremo local
                socket.Listen(1000);//colocal el socket en estado de escucha. 1: numero de conexiones entrantes
                Console.WriteLine("Server> escuchando...");


                listenThread = new Thread(this.Listen);
                listenThread.Start();

            }
            catch (Exception ex)
            {

                Console.WriteLine("Server> {0}", ex.ToString());
                Console.WriteLine("Press any key to finish");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        public void Listen()
        {
            while (true)
            {
                //Crea un nuevo Socket para una conexión recién creada.
                Socket socketClient = socket.Accept();
                Console.WriteLine("Nueva conexión");
                listenThread = new Thread(this.ListenClient);
                listenThread.Start(socketClient);
            }
        }

        public void ListenClient(Object o)
        {
            try
            {
                Socket socketClient = (Socket)o;
                Object reciveO;

                do
                {
                    //tamaño del búfer 
                    byte[] bytes = new byte[10024];
                    //recibe datos y devuelve el número de bytes leídos correctamente
                    int count = socketClient.Receive(bytes);
                    Array.Resize(ref bytes, count);
                    reciveO = JsonSerialization.Deserializate(bytes);
                }
                while (!(reciveO is GWUser));

                GWUser u = (GWUser)reciveO;

                for (Int32 i = 0; i < users.Count; i++)
                {
                    if (users.ContainsKey(u.Id))
                    {
                        // El usuario ya posee una conexión                    
                        sendToClient(socketClient, "Error");
                        socketClient.Close(1);
                        Thread.CurrentThread.Abort();
                    }
                }

                u.State = 1;

                //////Guid tempGuid = Guid.NewGuid();

                //////Room nRoom = new Room();
                //////nRoom.Add = true;
                //////nRoom.IdRoom = tempGuid;
                //////nRoom.Users = new Dictionary<Int16, Guid>();
                //////nRoom.UsersOcupation = new Dictionary<Int16, Boolean>();
                //////nRoom.Users[0] = tempGuid;
                //////nRoom.Users[1] = Guid.NewGuid();
                //////nRoom.Users[2] = Guid.NewGuid();
                //////nRoom.Users[3] = new Guid();
                //////nRoom.UsersOcupation[0] = true;
                //////nRoom.UsersOcupation[1] = true;
                //////nRoom.UsersOcupation[2] = true;
                //////nRoom.UsersOcupation[3] = false;
                //////WaitingRooms.Add(tempGuid, nRoom);

                sendToClient(socketClient, WaitingRooms.Values.ToArray());

                usersSockets.Add(u.Id, socketClient);
                users.Add(u.Id, u);

                try
                {
                    while (true)
                    {
                        //tamaño del búfer 
                        byte[] bytes = new byte[1000024];
                        try
                        {
                            //recibe datos y devuelve el número de bytes leídos correctamente
                            int count = socketClient.Receive(bytes);
                            Array.Resize(ref bytes, count);
                        }
                        catch (Exception ex)
                        {
                            // El usuario posiblemente se desconectó
                            // Si estaba en una mesa lo sacamos de la misma y si la había creado él limpiamos la mesa
                            if (u.Room != null)
                            {
                                if (u.Room.IdRoom == u.Id)
                                {
                                    WaitingRooms.Remove(u.Room.IdRoom);
                                    u.Room.resetStatesRoom();
                                    u.Room.DeleteRoom = true;

                                    for (Int16 i = 0; i < 4; i++)
                                    {
                                        if (u.Room.UsersOcupation[i])
                                        {
                                            try
                                            {
                                                //sendToClient(usersSockets[u.Room.Users[i]], u.Room);
                                                if (i != 0)
                                                {
                                                    users[u.Room.Users[i]].Room = null;
                                                    users[u.Room.Users[i]].State = 1;

                                                    sendToClient(usersSockets[u.Room.Users[i]], new StateError() { GoToState = 1, IsError = true, Message = "El anfitrión eliminó la mesa en la que estabas =(" });
                                                }
                                            }
                                            catch (Exception exAnfitrion)
                                            {

                                            }
                                        }
                                    }

                                    sendNewUserRoomToUpdate(u.Room);
                                }
                                else
                                {
                                    for (Int16 i = 0; i < 4; i++)
                                    {
                                        if (u.Room.UsersOcupation[i])
                                        {
                                            if (u.Room.Users[i] == u.Id)
                                            {
                                                u.Room.UsersOcupation[i] = false;
                                                u.Room.Users[i] = new Guid();

                                                sendNewUserRoomToUpdate(u.Room);
                                            }
                                        }
                                    }
                                }
                            }

                            // Eliminamos el usuario de la lista
                            users.Remove(u.Id);
                            usersSockets.Remove(u.Id);
                            Thread.CurrentThread.Abort();

                        }

                        reciveO = JsonSerialization.Deserializate(bytes);

                        if (reciveO is Message)
                        {
                            Console.WriteLine(((Message)reciveO).Mess);
                        }
                        else if (reciveO is Room)
                        {
                            Room r = (Room)reciveO;

                            // Comprobamos la existencia de la mesa que nos manda
                            if (!r.Add && !WaitingRooms.ContainsKey(r.IdRoom))
                            {
                                sendToClient(socketClient, new StateError() { GoToState = 1, IsError = true, Message = "La mesa ya no está disponible." });
                            }
                            else
                            {

                                if (r.Add)
                                {
                                    if (u.State == 1)
                                    {
                                        u.State = 2;
                                        // EL jugador quiere crear una sala
                                        Room createdRoom = new Room();
                                        createdRoom.Add = true;
                                        createdRoom.IdRoom = u.Id;
                                        createdRoom.Users = new Dictionary<Int16, Guid>();
                                        createdRoom.UsersOcupation = new Dictionary<Int16, Boolean>();
                                        createdRoom.Users[0] = u.Id;
                                        createdRoom.Users[1] = new Guid();
                                        createdRoom.Users[2] = new Guid();
                                        createdRoom.Users[3] = new Guid();
                                        createdRoom.UsersOcupation[0] = true;
                                        createdRoom.UsersOcupation[1] = false;
                                        createdRoom.UsersOcupation[2] = false;
                                        createdRoom.UsersOcupation[3] = false;
                                        WaitingRooms.Add(u.Id, createdRoom);
                                        u.Room = createdRoom;

                                        sendNewUserRoomToUpdate(createdRoom);
                                    }
                                }
                                else if (r.Update)
                                {
                                    if (u.State == 1 && u.Room == null)
                                    {
                                        // El jugador quiere unirse a una sala creada
                                        Room waitingRoom = WaitingRooms[r.IdRoom];
                                        if (!waitingRoom.UsersOcupation[r.Position])
                                        {
                                            waitingRoom.UsersOcupation[r.Position] = true;
                                            waitingRoom.Users[r.Position] = u.Id;
                                            u.State = 2;
                                            u.Room = waitingRoom;
                                            waitingRoom.resetStatesRoom();
                                            waitingRoom.Update = true;
                                            sendNewUserRoomToUpdate(waitingRoom);
                                        }
                                        else
                                        {
                                            sendToClient(socketClient, new StateError() { GoToState = 1, IsError = true, Message = "Alguien se te adelantó, inténtalo de nuevo." });
                                        }

                                        Boolean estaCompleta = true;
                                        // Comprobar sala completa
                                        for (Int16 j = 0; j < 4; j++)
                                        {
                                            if (!waitingRoom.UsersOcupation[j])
                                            {
                                                estaCompleta = false;
                                                break;
                                            }
                                        }

                                        if (estaCompleta)
                                        {
                                            WaitingRooms.Remove(waitingRoom.IdRoom);
                                            PlayingRooms.Add(waitingRoom.IdRoom, waitingRoom);

                                            for (Int16 k = 0; k < 4; k++)
                                            {
                                                users[waitingRoom.Users[k]].State = 3;
                                            }

                                            setDominoGame(waitingRoom, null, null, true);
                                        }
                                    }
                                }
                                else if (r.Delete)
                                {
                                    if (u.State == 2 && u.Room != null)
                                    {
                                        if (u.Room.UsersOcupation[r.Position] && u.Room.Users[r.Position] == u.Id)
                                        {
                                            u.Room.UsersOcupation[r.Position] = false;
                                            u.Room.Users[r.Position] = new Guid();
                                            u.State = 1;
                                            u.Room.resetStatesRoom();
                                            u.Room.Delete = true;
                                            sendNewUserRoomToUpdate(u.Room);
                                            u.Room = null;
                                        }
                                        else
                                        {
                                            sendToClient(socketClient, new StateError() { GoToState = 2, IsError = true, Message = "Ha ocurrido un error =) Seguimos trabajando. 0x227126" });
                                        }
                                    }
                                }
                                else if (r.DeleteRoom)
                                {
                                    if (u.State == 2 && r.Position == 0)
                                    {
                                        if (u.Room != null && u.Room.IdRoom == u.Id)
                                        {
                                            u.Room.resetStatesRoom();
                                            u.Room.DeleteRoom = true;
                                            WaitingRooms.Remove(u.Id);

                                            for (Int16 i = 0; i < 4; i++)
                                            {
                                                if (u.Room.UsersOcupation[i])
                                                {
                                                    try
                                                    {
                                                        // Sólo informamos a los que estaban sentados en esa mesa
                                                        if (i != 0)
                                                        {
                                                            users[u.Room.Users[i]].Room = null;
                                                            users[u.Room.Users[i]].State = 1;
                                                            sendToClient(usersSockets[u.Room.Users[i]], new StateError() { GoToState = 1, IsError = true, Message = "El anfitrión eliminó la mesa en la que estabas =(" });
                                                        }
                                                    }
                                                    catch (Exception exAnfitrion)
                                                    {

                                                    }
                                                }
                                            }

                                            sendNewUserRoomToUpdate(u.Room);

                                            u.State = 1;
                                            u.Room = null;
                                        }
                                    }
                                }
                            }
                        }
                        else if (reciveO is DominoTabServ)
                        {
                            if (u.State == 3)
                            {
                                DominoTabServ dtClient = (DominoTabServ)reciveO;

                                // La sala debe de existir
                                if (PlayingRooms.ContainsKey(dtClient.IdRoom))
                                {
                                    Room room = PlayingRooms[dtClient.IdRoom];

                                    // La ficha enviada debe der del cliente del que es el turno
                                    if (room.PlayerRound == dtClient.Player)
                                    {
                                        // El usuario pide pasar turno
                                        if (dtClient.Turn)
                                        {
                                            // Comprobamos que efectivamente no tiene fichas
                                            DominoTabServ[] dominoTabsPlayer = room.PlayersDominoTabs.Where(x => x.Player == dtClient.Player).ToArray();

                                            // Recorremos todas sus fichas
                                            Boolean nextTurn = true;
                                            foreach (DominoTabServ dtSP in dominoTabsPlayer)
                                            {
                                                if (dtSP.Values.Contains(room.A) || dtSP.Values.Contains(room.B))
                                                {
                                                    nextTurn = false;
                                                    break;
                                                }
                                            }

                                            if (nextTurn)
                                            {
                                                room.PlayerRound--;
                                                room.PlayerRound = room.PlayerRound < 0 ? (Int16)3 : room.PlayerRound;
                                            }
                                        }
                                        else
                                        {
                                            DominoTabServ dTServer = room.PlayersDominoTabs.Where(x => x.Player == dtClient.Player && x.Id == dtClient.Id).FirstOrDefault();

                                            // Quitando la posibilidad de error el usuario es posible que esté intentando suplantar a otro
                                            if (dTServer != null && dTServer.Values[0] == dtClient.Values[0] && dTServer.Values[1] == dtClient.Values[1])
                                            {
                                                // No existen fichas aún en la mesa
                                                if (room.InGameDominoTabs.Count == 0)
                                                {
                                                    room.A = dTServer.Values[0];
                                                    room.B = dTServer.Values[1];

                                                    room.PlayersDominoTabs.Remove(dTServer);
                                                    room.InGameDominoTabs.Add(dTServer);
                                                    room.PlayerRound--;
                                                    room.PlayerRound = room.PlayerRound < 0 ? (Int16)3 : room.PlayerRound;

                                                    sendNewDominoTabToUpdate(dtClient, room);

                                                }
                                                // Existe almenos una ficha o más en la mesa
                                                else
                                                {
                                                    Boolean trueA = dTServer.Values.Contains(room.A);
                                                    Boolean trueB = dTServer.Values.Contains(room.B);

                                                    trueA = trueA && trueB ? dtClient.IsForA : trueA;
                                                    trueB = trueA && trueB ? dtClient.IsForB : trueB;

                                                    if (trueA)
                                                    {
                                                        room.A = dTServer.Values[0] == room.A ? dTServer.Values[1] : dTServer.Values[0];
                                                    }
                                                    else if (trueB)
                                                    {
                                                        room.B = dTServer.Values[0] == room.B ? dTServer.Values[1] : dTServer.Values[0];
                                                    }

                                                    room.PlayersDominoTabs.Remove(dtClient);
                                                    room.InGameDominoTabs.Remove(dtClient);
                                                    Int16 antPlayer = room.PlayerRound;
                                                    room.PlayerRound--;
                                                    room.PlayerRound = room.PlayerRound < 0 ? (Int16)3 : room.PlayerRound;

                                                    // Checamos el final de la ronda
                                                    if (!checkEnd(antPlayer, room, dtClient))
                                                    {
                                                        sendNewDominoTabToUpdate(dtClient, room);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                sendToClient(usersSockets[u.Id], new StateError() { GoToState = 1, IsError = true, Message = "No se permiten trampas en el servidor =(" });
                                            }
                                        }

                                    }
                                    else
                                    {
                                        sendToClient(usersSockets[u.Id], new StateError() { GoToState = 0, IsError = false, Message = "No es tu turno =(" });
                                    }
                                }
                                else
                                {
                                    sendToClient(usersSockets[u.Id], new StateError() { GoToState = 1, IsError = true, Message = "No se econtró la mesa en la que estabas jugando =(" });
                                }
                            }
                            else
                            {
                                sendToClient(usersSockets[u.Id], new StateError() { GoToState = 1, IsError = true, Message = "Primero debe elegir una mesa para jugar =)" });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    users.Remove(u.Id);
                    usersSockets.Remove(u.Id);
                    // Ocurrio un error esperado o inesperado
                    sendToClient(socketClient, new Error() { Message = ex.Message });
                    socketClient.Close(1);
                }

                //socketClient.Shutdown(SocketShutdown.Both); //Deshabilita los envíos y recepciones en un Socket
                //socket.Close();//Cierra el Socket conexión y libera todos los asociados los recursos.
            }
            catch (Exception ex)
            {
                // Bye bye =)
            }
        }

        private void sendToClient(Socket socketClient, Object o)
        {
            socketClient.Send(Encoding.UTF8.GetBytes(JsonSerialization.Serializate(o)));
        }

        private Boolean checkEnd(Int16 antPlayerRound, Room r, DominoTabServ dtFinal)
        {
            List<DominoTabServ> playerTabs = r.PlayersDominoTabs.Where(x => x.Player == antPlayerRound).ToList();

            if (playerTabs != null && playerTabs.Count != 0)
            {
                List<DominoTabServ> allPlayerTabs = r.PlayersDominoTabs.ToList();

                Boolean endRound = true;
                foreach (DominoTabServ dtSP in allPlayerTabs)
                {
                    if (dtSP.Values.Contains(r.A) || dtSP.Values.Contains(r.B) || r.A == -1)
                    {
                        endRound = false;
                    }
                }

                if (endRound)
                {
                    Int16 countTeam1 = (Int16)allPlayerTabs
                        .Where(x => x.Player == 0 || x.Player == 2).Sum(x => (x.Values[0] + x.Values[1]));

                    Int16 countTeam2 = (Int16)allPlayerTabs
                        .Where(x => x.Player != 0 && x.Player != 2).Sum(x => (x.Values[0] + x.Values[1]));

                    if (countTeam1 < countTeam2)
                    {
                        r.PuntTeam1 += (Int16)(countTeam1 + countTeam2);
                    }
                    else if (countTeam1 > countTeam2)
                    {
                        r.PuntTeam2 += (Int16)(countTeam1 + countTeam2);
                    }

                    // Corprobar si hemos llegado al final de la puntuación total para finalizar el juego

                    // Ganamos nosotros (Partida completa)
                    if (r.PuntTeam1 >= r.FinalPunt)
                    {
                        //END(true);
                    }
                    // Ganan ellos (Partida completa)
                    else if (r.PuntTeam2 >= r.FinalPunt)
                    {
                        //END(false);
                    }
                    // Siguiente ronda en caso de que nadie llegue a los puntos
                    else
                    {
                        EndRound(r, dtFinal);
                    }

                    return true;
                }

                return false;
            }
            else
            {
                Int16 partnerTeam1 = 0;

                Int16 countPunts = (Int16)r.PlayersDominoTabs.Sum(x => x.Values[0] + x.Values[1]);

                if (antPlayerRound == 0 || antPlayerRound == 1)
                {
                    r.PuntTeam2 += countPunts;
                }
                else
                {
                    r.PuntTeam1 += countPunts;
                }

                // Corprobar si hemos llegado al final de la puntuación total para finalizar el juego

                // Ganamos nosotros (Partida completa)
                if (r.PuntTeam1 >= r.FinalPunt)
                {
                    //END(true);
                }
                // Ganan ellos (Partida completa)
                else if (r.PuntTeam2 >= r.FinalPunt)
                {
                    //END(false);
                }
                // Siguiente ronda en caso de que nadie llegue a los puntos
                else
                {
                    EndRound(r, dtFinal);
                }

                return true;
            }
        }

        private void sendNewDominoTabToUpdate(DominoTabServ updateDominoTab, Room room)
        {

            for (Int16 i = 0; i < 4; i++)
            {
                try
                {
                    // Sólo actualizamos a los clientes que no són los que enviaron la ficha
                    if (updateDominoTab.Player != i)
                    {
                        GWUser u = users[room.Users[i]];

                        // Solamente a los usuarios que están es estado de búsqueda activa de sala (Estados 1 y 2)
                        if (u.State == 3)
                        {
                            try
                            {
                                sendToClient(usersSockets[u.Id], updateDominoTab);
                            }
                            catch (Exception ex)
                            {
                                // Algún susodicho haciendo algo malvado =)
                            }
                        }
                        else
                        {
                            sendToClient(usersSockets[u.Id], new StateError() { GoToState = 1, IsError = true, Message = "Existe un error en tu estado =(" });
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void sendNewUserRoomToUpdate(Room updateRoom)
        {
            for (int i = 0; i < users.Keys.Count; i++)
            {
                try
                {
                    GWUser u = users[users.Keys.ElementAt(i)];
                    // Solamente a los usuarios que están es estado de búsqueda activa de sala (Estados 1 y 2)
                    if (u.State == 1 || u.State == 2)
                    {
                        try
                        {
                            sendToClient(usersSockets[u.Id], updateRoom);
                        }
                        catch (Exception ex)
                        {
                            // Algún susodicho haciendo algo malvado =)
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void setDominoGame(Room room, DominoTabServ finalTab, DominoTabServ[] finalPlayerTabs, Boolean isFirst)
        {
            room.InGameDominoTabs = new List<DominoTabServ>();

            // Crea la ficha en clase y asocia un guid aleatorio para simular "barajar"
            List<DominoTabServ> dTabs = new List<DominoTabServ>();
            foreach (Int16[] _dTab in AllDominoTabs)
            {
                DominoTabServ dTab = new DominoTabServ();
                dTab.IdRoom = room.IdRoom;
                dTab.Id = Guid.NewGuid();
                dTab.Values = _dTab;
                dTabs.Add(dTab);
            }

            // Aleatorizar/Barajar
            var dOrderedTabs = dTabs.OrderBy(x => x.Id);

            // Asociacion de jugadores
            Int16 player = -1;
            Int16 i = 0;
            foreach (DominoTabServ dT in dOrderedTabs)
            {
                if (i % 7 == 0)
                {
                    player = (Int16)(i / 7);
                }

                dT.Player = player;

                i++;
            }

            // Selección de jugador que comienza
            Random rnd = new Random();
            room.PlayerRound = (Int16)rnd.Next(0, 3);
            room.A = -1;
            room.B = -1;
            // Guardamos las fichas
            room.PlayersDominoTabs = dOrderedTabs.ToList();

            for (Int16 j = 0; j < 4; j++)
            {
                // Mandamos a cada uno la señal de salida y a cada usuario sus fichas,
                // además de la lista de usuario para que lleven el progreso del turno

                Start st = new Start();
                st.PlayerRound = room.PlayerRound;
                st.Users = room.Users;
                st.MyTabs = room.PlayersDominoTabs.Where(x => x.Player == j).ToArray();
                st.PuntTeam1 = room.PuntTeam1;
                st.PuntTeam2 = room.PuntTeam2;
                st.FinalTab = finalTab;
                st.FinalPlayerTabs = finalPlayerTabs;
                st.IsFirst = isFirst;
                try
                {
                    sendToClient(usersSockets[room.Users[j]], st);
                }
                catch (Exception ex)
                {

                }
            }

        }
        private async void EndRound(Room room, DominoTabServ finalTab)
        {
            DominoTabServ[] finalPlayersTabs = room.PlayersDominoTabs.ToArray();
            room.PlayersDominoTabs = new List<DominoTabServ>();
            room.InGameDominoTabs = new List<DominoTabServ>();
            room.A = -1;
            room.B = -1;
            setDominoGame(room, finalTab, finalPlayersTabs, false);
        }

        public class Error
        {
            public String Message { get; set; }
        }

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
}
