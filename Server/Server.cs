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
        public static Hashtable clientsList=new Hashtable ();
        public static Hashtable clientsInGame=new Hashtable ();
        StreamReader read;


        public Server () {
            TcpClient client=null;
            String nickname=null;
            String keyword=null;
            string[] clientMessage;
            TcpListener server=new TcpListener (IPAddress.Any, 5150);
            Console.WriteLine ("Chat server started");
            server.Start ();
            while (true) {
                if (server.Pending ()) {
                    client=server.AcceptTcpClient ();
                    read=new StreamReader (client.GetStream ());
                    clientMessage=read.ReadLine ().Split (':');
                    keyword=clientMessage[0];
                    nickname=clientMessage[1];

                    if (keyword.Equals ("c")) {
                        if (clientsList.ContainsKey (nickname)) {
                            StreamWriter write=null;
                            write=new StreamWriter (client.GetStream ());
                            write.WriteLine ("ALR:"+nickname);
                            write.Flush ();
                            Console.WriteLine ("Client "+nickname+" already exists");
                        } else {
                           
                            clientsList.Add (nickname, client);
                            Thread chatThread=new Thread (() => ConnectToChat (client, nickname));
                            chatThread.Start ();
                            Console.WriteLine ("A new client has connected to chat "+nickname);
                            AllUsers ();
                        }
                        
                    } else if (keyword.Equals ("g")) {
                        clientsInGame.Add (nickname, client);
                        Thread gameThread=new Thread (() => ConnectToGame (client, nickname));
                        gameThread.Start ();
                        Console.WriteLine ("A new client is in game "+nickname);
                    }
                }
            }
        }

        private void ConnectToChat (TcpClient client, string nickname) {
            new HandleClient (client, nickname);
        }
        private void ConnectToGame (TcpClient client, string nickname) {
            new HandleGame (client, nickname);
        }

        public static void Broadcast (String nickname, String msg) {
            TcpClient broadcastSocket=null;
            StreamWriter write=null;
            foreach (DictionaryEntry item in clientsList) {
                try {
                    //if (msg.Trim ()==""||(TcpClient) item.Value==null)
                    //    continue;

                    broadcastSocket=(TcpClient) item.Value;
                    write=new StreamWriter (broadcastSocket.GetStream ());
                    write.WriteLine ("MESSAGE:"+nickname+":"+msg);
                    write.Flush ();
                    write=null;
                } catch (Exception ex) {
                    Console.WriteLine (ex.ToString ());
                }
            }
        }
        public static void BroadcastInGame (String nickname, String msg) {
            TcpClient broadcastSocket=null;
            StreamWriter write=null;
            foreach (DictionaryEntry item in clientsInGame) {
                try {
                    //if (msg.Trim ()==""||(TcpClient) item.Value==null)
                    //    continue;

                    broadcastSocket=(TcpClient) item.Value;
                    write=new StreamWriter (broadcastSocket.GetStream ());
                    write.WriteLine ("MESSAGE:"+nickname+":"+msg);
                    write.Flush ();
                    write=null;
                } catch (Exception ex) {
                    Console.WriteLine (ex.ToString ());
                }
            }
        }
        public static void AllUsers () {
            String allNicknames=null;
            TcpClient broadcastSocket=null;
            StreamWriter write=null;

            foreach (DictionaryEntry item1 in clientsList) {
                allNicknames=allNicknames+":"+item1.Key;
            }
            foreach (DictionaryEntry item in clientsList) {
                try {
                    broadcastSocket=(TcpClient) item.Value;
                    write=new StreamWriter (broadcastSocket.GetStream ());

                    write.WriteLine ("NEW_USER"+allNicknames);
                    Console.WriteLine ("Useri conectati"+allNicknames);
                    write.Flush ();
                    write=null;
                } catch (Exception ex) {
                    Console.WriteLine (ex.ToString ());
                }
            }
        }


        public static void MsgtoChatClient (String nickname, String msg) {
            TcpClient clientSocket=null;
            StreamWriter write=null;
            if (clientsList.ContainsKey (nickname)) {
                    clientSocket=(TcpClient) clientsList[nickname];
                    write=new StreamWriter (clientSocket.GetStream ());
                    write.WriteLine (msg);
                    write.Flush ();
                    write=null;
            }
        }
        public static void MsgtoGameClient (String nickname, String msg) {
            TcpClient clientSocket=null;
            StreamWriter write=null;
            if (clientsInGame.ContainsKey (nickname)) {
                    clientSocket=(TcpClient) clientsInGame[nickname];
                    write=new StreamWriter (clientSocket.GetStream ());
                    write.WriteLine (msg);
                    write.Flush ();
                    write=null;

            }
        }


        public static void RemoveUser (string nickname) {
            if (clientsList.ContainsKey (nickname)) {
                    clientsList.Remove (nickname);
                    AllUsers ();
            }
        }
        public static void RemoveUserFromGame (string nickname) {
            if (clientsList.ContainsKey (nickname)) {
               
                    clientsInGame.Remove (nickname);
            }
        }
        static void Main (string[] args) {
            new Server ();
        }


    }
}
