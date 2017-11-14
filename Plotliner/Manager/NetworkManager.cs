using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Manager
{
    /*
     * -Message types-
     * 
     * 0 = Int, Int
     * */
    class NetworkManager
    {
        PlotlineManager plotline;

        NetServer server;
        NetClient client;

        public NetworkManager(PlotlineManager plotline)
        {
            this.plotline = plotline;
        }

        public void update()
        {
            if(server != null)
            {
                updateServer();
            }

            if(client != null)
            {
                updateClient();
            }
        }

        void updateServer()
        {
            NetIncomingMessage message;
            while((message = server.ReadMessage()) != null)
            {
                switch(message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        switch(message.ReadByte())
                        {
                            case 0:
                                messageClients(0, message.ReadInt32(), message.ReadInt32());
                                break;
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        Console.WriteLine(message.SenderConnection.Status);
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        Console.WriteLine(message.ReadString());
                        break;
                    default:
                        Console.WriteLine("Unhandled message type");
                        break;
                }
            }
        }

        void updateClient()
        {
            NetIncomingMessage message;
            while((message = client.ReadMessage()) != null)
            {
                switch(message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        switch(message.ReadByte())
                        {
                            case 0:
                                plotline.createTextBox(message.ReadInt32(), message.ReadInt32());
                                break;
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        Console.WriteLine(message.SenderConnection.Status);
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        Console.WriteLine(message.ReadString());
                        break;
                    default:
                        Console.WriteLine("Unhandled message type");
                        break;
                }
            }
        }

        public void sendMessage(byte msg, int arg1, int arg2)
        {
            var message = client.CreateMessage();
            message.Write(msg);
            message.Write(arg1);
            message.Write(arg2);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }

        void messageClients(byte msg, int arg1, int arg2)
        {
            var message = server.CreateMessage();
            message.Write(msg);
            message.Write(arg1);
            message.Write(arg2);
            server.SendMessage(message, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void createServer(int port = 12345)
        {
            Console.WriteLine("Creating Server...");

            var config = new NetPeerConfiguration("Plotliner") { Port = port, EnableUPnP = true };
            server = new NetServer(config);
            server.Start();
            server.UPnP.ForwardPort(port, "Plotliner");

            Console.WriteLine("Created Server");
        }

        public void createClient(string ip, int port = 12345)
        {
            Console.WriteLine("Creating Client...");

            var config = new NetPeerConfiguration("Plotliner");
            client = new NetClient(config);
            client.Start();
            client.Connect(host: ip, port: port);

            Console.WriteLine("Created Client");
        }
    }
}
