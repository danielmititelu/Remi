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
        StreamReader read;
        public static List<int> pieces=new List<int>();
        public static Hashtable clientsFormation=new Hashtable();
        public static List<String> formations=new List<String>();
        public static UniqueRandom random=new UniqueRandom(Enumerable.Range(0, 105));
        public static List<int> pieces_on_board=new List<int>();

        public Server() {
            TcpClient client=null;
            String nickname=null;
            HandlePieces.GeneratePieces();
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
                        AllUsers();
                    }
                }
            }
        }

        private void ConnectToChat(TcpClient client, string nickname) {
            new HandleClient(client, nickname);
        }
        private void ConnectToGame(TcpClient client, string nickname) {
            new HandleGame(client, nickname);
        }

        public static void Broadcast(String nickname, String msg) {
            TcpClient broadcastSocket=null;
            StreamWriter write=null;
            foreach(DictionaryEntry item in clientsList) { // TODO: Hashtable should use the objects not only tcpconnection
                try {
                    if(msg.Trim()==""||(TcpClient) item.Value==null)
                        continue;

                    broadcastSocket=(TcpClient) item.Value;
                    write=new StreamWriter(broadcastSocket.GetStream());
                    write.WriteLine("MESSAGE_CHAT:"+nickname+":"+msg);
                    write.Flush();
                    write=null;
                } catch(Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static void BroadcastInGame(String keyword, String nickname, String msg) {
            TcpClient broadcastSocket=null;
            StreamWriter write=null;
            foreach(DictionaryEntry item in clientsInGame) {
                try {
                    if(msg.Trim()==""||(TcpClient) item.Value==null)
                        continue;

                    broadcastSocket=(TcpClient) item.Value;
                    write=new StreamWriter(broadcastSocket.GetStream());
                    write.WriteLine(keyword+nickname+":"+msg);
                    write.Flush();
                    write=null;
                    Console.WriteLine(keyword+nickname+":"+msg);
                } catch(Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static void AllUsers() {
            String allNicknames=null;
            TcpClient broadcastSocket=null;
            StreamWriter write=null;

            foreach(DictionaryEntry item1 in clientsList) {
                allNicknames=allNicknames+":"+item1.Key;
            }
            foreach(DictionaryEntry item in clientsList) {
                try {
                    broadcastSocket=(TcpClient) item.Value;
                    write=new StreamWriter(broadcastSocket.GetStream());

                    write.WriteLine("NEW_USER_IN_CHAT"+allNicknames);

                    write.Flush();
                    write=null;
                } catch(Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.WriteLine("Useri conectati in chat"+allNicknames);
        }
        public static void AllUsersInGame() {
            String allNicknames=null;
            TcpClient broadcastSocket=null;
            StreamWriter write=null;

            foreach(DictionaryEntry item1 in clientsInGame) {
                allNicknames=allNicknames+":"+item1.Key;
            }
            foreach(DictionaryEntry item in clientsInGame) {
                try {
                    broadcastSocket=(TcpClient) item.Value;
                    write=new StreamWriter(broadcastSocket.GetStream());

                    write.WriteLine("NEW_USER_IN_GAME"+allNicknames);

                    write.Flush();
                    write=null;
                } catch(Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.WriteLine("Useri conectati in game"+allNicknames);
        }

        public static void RemoveUserFromChat(string nickname) {
            if(clientsList.ContainsKey(nickname)) {
                clientsList.Remove(nickname);
                AllUsers();
            }
        }

        public static void RemoveUserFromGame(string nickname) {
            if(clientsInGame.ContainsKey(nickname)) {
                clientsInGame.Remove(nickname);
                AllUsersInGame();
            }
        }

        static void Main(string[] args) {
            new Server();
        }
    }
}
