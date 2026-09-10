using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using Newtonsoft.Json;

namespace PrimesDispacher
{
    internal class PServer
    {
        Socket socket;
        Thread listenThread;
        Dictionary<Guid, Socket> usersSockets = new Dictionary<Guid, Socket>();
        Dictionary<Guid, PUser> users = new Dictionary<Guid, PUser>();

        //Dictionary<Guid, Room> WaitingRooms = new Dictionary<Guid, Room>();
        //Dictionary<Guid, Room> PlayingRooms = new Dictionary<Guid, Room>();

        public PServer()
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
                while (!(reciveO is PUser));

                PUser u = (PUser)reciveO;

                for (Int32 i = 0; i < users.Count; i++)
                {
                    if (users.ContainsKey(u.Id))
                    {
                        // El usuario ya posee una conexión                    
                        sendToClient(socketClient, "Error");
                        socketClient.Close(1);
                        throw new TaskCanceledException();
                        //Thread.CurrentThread.Abort();
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

                //sendToClient(socketClient, WaitingRooms.Values.ToArray());

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
                            ////// El usuario posiblemente se desconectó
                            ////// Si estaba en una mesa lo sacamos de la misma y si la había creado él limpiamos la mesa
                            ////if (u.Room != null)
                            ////{
                            ////    if (u.Room.IdRoom == u.Id)
                            ////    {
                                    
                            ////        //sendNewUserRoomToUpdate(u.Room);
                            ////    }
                            ////    else
                            ////    {
                            ////        for (Int16 i = 0; i < 4; i++)
                            ////        {
                            ////            if (u.Room.UsersOcupation[i])
                            ////            {
                            ////                if (u.Room.Users[i] == u.Id)
                            ////                {
                            ////                    u.Room.UsersOcupation[i] = false;
                            ////                    u.Room.Users[i] = new Guid();

                            ////                    //sendNewUserRoomToUpdate(u.Room);
                            ////                }
                            ////            }
                            ////        }
                            ////    }
                            ////}

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
                        //else if (reciveO is Room)
                        //{
                        //    Room r = (Room)reciveO;
                                                        
                        //}
                        //else if (reciveO is DominoTabServ)
                        //{
                            
                        //}
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

        public class JsonSerialization
        {
            public static String Serializate(Object toSerializate)
            {
                Parameter p = new Parameter() { Type = toSerializate.GetType().FullName, ObjectS = JsonConvert.SerializeObject(toSerializate) };
                string json = JsonConvert.SerializeObject(p);
                return json;
            }

            public static Object Deserializate(byte[] data)
            {
                if (data != null && data.Length > 0)
                {
                    String json = Encoding.UTF8.GetString(data);
                    Parameter p = JsonConvert.DeserializeObject<Parameter>(json);
                    Object o = JsonConvert.DeserializeObject(p.ObjectS, Type.GetType(p.Type));
                    return o;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        [Serializable]
        public class Parameter
        {
            public String Type { get; set; }

            public String ObjectS { get; set; }
        }

        [Serializable]
        public class PUser
        {
            public Guid Id { get; set; }

            public Guid Password { get; set; }

            public Int16 State { get; set; }

            public DateTime LastUpdate { get; set; }

            //public Room Room { get; set; }
        }

        [Serializable]
        public class Error
        {
            public String Message { get; set; }
        }

        [Serializable]
        public class Message
        {
            public PUser User { get; set; }
            public String Mess { get; set; }
        }
    }
}
