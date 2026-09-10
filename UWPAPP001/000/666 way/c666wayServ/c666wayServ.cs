using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace c666wayServ
{
    internal class c666wayServ
    {        
        Socket socket;
        Thread listenThread;
        private Int32[] validDiff = new Int32[] { 20, 30, 40 };
        Dictionary<Guid, Socket> usersSockets = new Dictionary<Guid, Socket>();        
        private Dictionary<Guid, Boolean[,]> ways = new Dictionary<Guid, Boolean[,]>();
        private Dictionary<Guid, c666way> waysMeta = new Dictionary<Guid, c666way>();

        public c666wayServ()
        {
            //puerto 
            int PORT = 63493;
            //IP Localhost
            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[1]; //192.168.1.46
            Console.WriteLine("Server " + ipAddress);
            Console.WriteLine("Server Corriendo");

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
                Console.WriteLine("Server escuchando...");


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
                while (!(reciveO is c666wayUser));

                c666wayUser u = (c666wayUser)reciveO;

                u.Id = Guid.NewGuid();
                usersSockets.Add(u.Id, socketClient);
                                                
                try
                {
                    while (true)
                    {
                        //tamaño del búfer 
                        byte[] bytes = new byte[10024];
                        try
                        {
                            //recibe datos y devuelve el número de bytes leídos correctamente
                            int count = socketClient.Receive(bytes);
                            Array.Resize(ref bytes, count);
                        }
                        catch (Exception ex)
                        {
                            // El usuario posiblemente se desconectó
                            // Si tenía camino lo eliminamos
                            if (u.Way != null)
                            {
                                ways.Remove(u.Way.IdWay);
                            }

                            // Eliminamos el usuario de la lista                            
                            usersSockets.Remove(u.Id);
                            Thread.CurrentThread.Abort();

                        }

                        reciveO = JsonSerialization.Deserializate(bytes);

                        if (reciveO is c666way)
                        {
                            c666way w = (c666way)reciveO;

                            // Comprbamos que el camino sea del usuario

                            // Si comenzamos un nuevo camino
                            if (w.Start)
                            {
                                //Eliminamos el anterior camino del usuario en caso de que existiera
                                if (u.Way != null)
                                {
                                    if (ways.ContainsKey(u.Way.IdWay))
                                    {
                                        ways.Remove(u.Way.IdWay);
                                    }
                                    u.Way = null;
                                }

                                if (validDiff.Contains(w.Diff))
                                {
                                    setNewWay(u, w);
                                }
                                else
                                {
                                    throw new Exception("ERROR EN EL TRANSCEPTOR DE COMPLEJIDAD");
                                }
                            }
                            // Si seguimos un camino en curso
                            else
                            {
                                if (ways.ContainsKey(w.IdWay) && waysMeta.ContainsKey(w.IdWay) && u.Way != null)
                                {
                                    c666way meta = waysMeta[w.IdWay];
                                    if (meta.APosX + meta.APosY + 1 == w.PosX + w.PosY)
                                    {
                                        if(ways[w.IdWay][w.PosX, w.PosY])
                                        {
                                            meta.APosX = w.PosX;
                                            meta.APosY = w.PosY;
                                        }
                                        else
                                        {
                                            u.Way = null;
                                            ways.Remove(w.IdWay);
                                            waysMeta.Remove(w.IdWay);
                                            w.Continue = false;                                            
                                        }
                                        sendToClient(socketClient, w);
                                    }
                                    else
                                    {
                                        u.Way = null;
                                        ways.Remove(w.IdWay);
                                        waysMeta.Remove(w.IdWay);
                                        w.Continue = false;
                                        throw new Exception("ERROR EN EL TRANSCEPTOR DEL SALTO CUANTICO");
                                    }
                                }
                                else
                                {
                                    throw new Exception("ERROR EN EL TRANSCEPTOR DEL CAMINO");
                                }
                            }                            
                        }
                        else
                        {
                            throw new Exception("ERROR EN EL TRANSCEPTOR DE CONTINUO");
                        }
                    }
                }
                catch (Exception ex)
                {
                    usersSockets.Remove(u.Id);
                    //Eliminamos el anterior camino del usuario en caso de que existiera
                    if (u.Way != null)
                    {
                        if (ways.ContainsKey(u.Way.IdWay))
                        {
                            ways.Remove(u.Way.IdWay);
                        }
                        u.Way = null;
                    }
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

        private void setNewWay(c666wayUser u, c666way way)
        {
            Guid idWay = Guid.NewGuid();

            Boolean[,] boolsWay = new Boolean[way.Diff, way.Diff];

            boolsWay[0, 0] = true;

            for (Int16 i = 0; i < way.Diff; i++)
            {
                boolsWay[way.Diff - 1, i] = true;
                boolsWay[i, way.Diff - 1] = true;
            }

            Random direccion = new Random();

            Int32 dd = direccion.Next(1, 3);
            Int32 x = 0;
            Int32 y = 0;
            switch (dd)
            {
                case 1:
                    x++;
                    break;
                case 2:
                    y++;
                    break;
            }

            while (!boolsWay[x, y])
            {
                boolsWay[x, y] = true;
                dd = direccion.Next(1, 3);
                switch (dd)
                {
                    case 1:
                        x++;
                        break;
                    case 2:
                        y++;
                        break;
                }
            }

            way.IdWay = idWay;
            way.PosX = 0;
            way.PosY = 0;
            way.APosX = 0;
            way.APosY = 0;
            way.Start = false;
            way.Continue = true;
            u.Way = way;
            ways.Add(idWay, boolsWay);
            waysMeta.Add(idWay, way);
            // Mandamos al usuario finalización del camino realizado
            sendToClient(usersSockets[u.Id], way);
        }

        private void sendToClient(Socket socketClient, Object o)
        {
            socketClient.Send(Encoding.UTF8.GetBytes(JsonSerialization.Serializate(o)));
        }

    }
}
