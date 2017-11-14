using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotliner.Manager
{
    class NetworkManager
    {
        NetServer server;
        NetClient client;

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
                        Console.WriteLine(message.LengthBytes);
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
                        Console.WriteLine(message.LengthBytes);
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
