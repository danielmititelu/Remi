using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server {
    class Server {
        public static Hashtable clientsList=new Hashtable();
        public static Hashtable clientsInGame=new Hashtable();
        public static List<int> pieces=new List<int>();
        public static Hashtable clientsFormation=new Hashtable();
        public static List<String> formations=new List<String>();
        public static UniqueRandom random=new UniqueRandom(Enumerable.Range(0, 105));
        public static List<int> pieces_on_board=new List<int>();

        public Server() {
            TcpClient client=null;
            String nickname=null;
            StreamReader read;
            LoadPieces.GeneratePieces();
            TcpListener server=new TcpListener(IPAddress.Any, 5150);
            Console.WriteLine("Chat server started");
            server.Start();
            while(true) {
                if(server.Pending()) {
                    client=server.AcceptTcpClient();
                    read=new StreamReader(client.GetStream());
                    nickname=read.ReadLine();

                    if(clientsList.ContainsKey(nickname)) {
                        StreamWriter write=null;
                        write=new StreamWriter(client.GetStream());
                        write.WriteLine("ALR:"+nickname);
                        write.Flush();
                        Console.WriteLine("Client "+nickname+" already exists");
                    } else {
                        clientsList.Add(nickname, client);
                        Thread chatThread=new Thread(() => ConnectToChat(client, nickname));
                        chatThread.Start();
                        Console.WriteLine("A new client has connected to chat "+nickname);
                        MessageSender.AllUsers("NEW_USER_IN_CHAT", clientsList);
                    }
                }
            }
        }

        private void ConnectToChat(TcpClient client, string nickname) {
            new HandleClient(client, nickname);
        }

        static void Main(string[] args) {
            new Server();
        }
    }
}
