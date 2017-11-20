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
        PlotlineManager plotline;

        NetServer server;
        NetClient client;

        public NetworkManager(PlotlineManager plotline)
        {
            this.plotline = plotline;

            createServer();
            createClient("127.0.0.1");
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

        /* -Message types-
         * 
         * 0 = Int32, Int32
         *      Create TextBox
         * 1 = Int32, string
         *      Update the text of a TextBox
         * 2 = Int32, Int32, Int32
         *      Update the position of a TextBox
         * 3 = Int32, Int32
         *      Create a BoxConnection
         * 4 = Int32
         *      Delete a Box
         * 5 = string
         *      Load a Plotline
         * 6 = Int32
         *      Delete BoxConnection
         * 7 = Int32, Byte, Byte, Byte
         *      Change TexBox Color
        */
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
                                Console.WriteLine("Server Recieved: New Text Box");
                                messageClients(0, message.ReadInt32(), message.ReadInt32());
                                break;
                            case 1:
                                Console.WriteLine("Server Recieved: TextBox Text Update");
                                messageClients(1, message.ReadInt32(), message.ReadString());
                                break;
                            case 2:
                                Console.WriteLine("Server Recieved: Update TextBox Position");
                                messageClients(2, message.ReadInt32(), message.ReadInt32(), message.ReadInt32());
                                break;
                            case 3:
                                Console.WriteLine("Server Recieved: Create Box Connection");
                                messageClients(3, message.ReadInt32(), message.ReadInt32());
                                break;
                            case 4:
                                Console.WriteLine("Server Recieved: Delete Text Box");
                                messageClients(4, message.ReadInt32());
                                break;
                            case 5:
                                Console.WriteLine("Server Recieved: Load Plotline");
                                messageClients(5, message.ReadString());
                                break;
                            case 6:
                                Console.WriteLine("Server Recieved: Delete Box Connection");
                                messageClients(6, message.ReadInt32());
                                break;
                            case 7:
                                Console.WriteLine("Server Recieved: Change TextBox Color");
                                messageClients(7, message.ReadInt32(), message.ReadByte(), message.ReadByte(), message.ReadByte());
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
                                Console.WriteLine("Client Recieved: New Text Box");
                                plotline.createTextBox(message.ReadInt32(), message.ReadInt32());
                                break;
                            case 1:
                                Console.WriteLine("Client Recieved: TextBox Text Update");
                                plotline.updateTextBox(message.ReadInt32(), message.ReadString());
                                break;
                            case 2:
                                Console.WriteLine("Client Recieved: Update TextBox Position");
                                plotline.updateTextBox(message.ReadInt32(), message.ReadInt32(), message.ReadInt32());
                                break;
                            case 3:
                                Console.WriteLine("Client Recieved: Create Box Connection");
                                plotline.createBoxConnect(message.ReadInt32(), message.ReadInt32());
                                break;
                            case 4:
                                Console.WriteLine("Client Recieved: Delete Text Box");
                                plotline.deleteTextBox(message.ReadInt32());
                                break;
                            case 5:
                                Console.WriteLine("Client Recieved: Load Plotline");
                                plotline.loadPlotline(message.ReadString());
                                break;
                            case 6:
                                Console.WriteLine("Client Recieved: Deleting Box Connection");
                                plotline.deleteBoxConnection(message.ReadInt32());
                                break;
                            case 7:
                                Console.WriteLine("Client Recieved: Changing Text Box Color");
                                plotline.updateTextBox(message.ReadInt32(), message.ReadByte(), message.ReadByte(), message.ReadByte());
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

        public void sendMessage(byte msg, int index, string character)
        {
            var message = client.CreateMessage();
            message.Write(msg);
            message.Write(index);
            message.Write(character);
            //message.Write(uppercase);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void sendMessage(byte msg, int index, int arg1, int arg2)
        {
            var message = client.CreateMessage();
            message.Write(msg);
            message.Write(index);
            message.Write(arg1);
            message.Write(arg2);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void sendMessage(byte msg, int index)
        {
            var message = client.CreateMessage();
            message.Write(msg);
            message.Write(index);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void sendMessage(byte msg, string load)
        {
            var message = client.CreateMessage();
            message.Write(msg);
            message.Write(load);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void sendMessage(byte msg, int index, byte r, byte g, byte b)
        {
            var message = client.CreateMessage();
            message.Write(msg);
            message.Write(index);
            message.Write(r);
            message.Write(g);
            message.Write(b);
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

        void messageClients(byte msg, int index, string character)
        {
            var message = server.CreateMessage();
            message.Write(msg);
            message.Write(index);
            message.Write(character);
            server.SendMessage(message, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        void messageClients(byte msg, int index, int arg1, int arg2)
        {
            var message = server.CreateMessage();
            message.Write(msg);
            message.Write(index);
            message.Write(arg1);
            message.Write(arg2);
            server.SendMessage(message, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        void messageClients(byte msg, int index)
        {
            var message = server.CreateMessage();
            message.Write(msg);
            message.Write(index);
            server.SendMessage(message, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        void messageClients(byte msg, string load)
        {
            var message = server.CreateMessage();
            message.Write(msg);
            message.Write(load);
            server.SendMessage(message, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        void messageClients(byte msg, int index, byte r, byte g, byte b)
        {
            var message = server.CreateMessage();
            message.Write(msg);
            message.Write(index);
            message.Write(r);
            message.Write(g);
            message.Write(b);
            server.SendMessage(message, server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void createServer(int port = 12345)
        {
            Console.WriteLine("Creating Server...");

            if(server != null)
            {
                server.Shutdown("Remaking");
                server = null;
            }

            var config = new NetPeerConfiguration("Plotliner") { Port = port, EnableUPnP = true };
            server = new NetServer(config);
            server.Start();
            server.UPnP.ForwardPort(port, "Plotliner");

            Console.WriteLine("Created Server");
        }

        // TODO: Check what happens if you connect to a server?
        public void createClient(string ip, int port = 12345)
        {
            Console.WriteLine("Creating Client...");

            if(client != null)
            {
                client.Shutdown("Remaking");
                client = null;
            }

            string[] split = ip.Split(':');
            if(split.Length == 2)
            {
                ip = split[0];
                port = int.Parse(split[1]);
            }

            var config = new NetPeerConfiguration("Plotliner");
            client = new NetClient(config);
            client.Start();
            client.Connect(host: ip, port: port);

            Console.WriteLine("Created Client");
        }
    }
}
