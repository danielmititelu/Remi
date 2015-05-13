using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server {
    public class HandleClient {
        TcpClient clientSocket;
        String nickname;

        public HandleClient(TcpClient clientSocket, String nickname) {
            this.clientSocket=clientSocket;
            this.nickname=nickname;
            doChat();
        }

        private void doChat() {
            NetworkStream networkStream=clientSocket.GetStream();
            StreamReader read=new StreamReader(networkStream);
            String[] msg=null;
            String message=null;
            while(networkStream.CanRead) { // TODO : MAKE ANOTHER CLASS TO SUPPORT THIS
                message=read.ReadLine();
                if(message!=null) {
                    msg=message.Split(':');
                    Console.WriteLine("From client- "+nickname+": "+message);
                    switch(msg[0]) {
                        case "MESSAGE_CHAT":
                            Server.Broadcast(nickname, message.Substring(message.IndexOf(':')+1, message.Length-message.IndexOf(':')-1));
                            break;
                        case "EXIT_FROM_CHAT":
                            Server.RemoveUserFromChat(nickname);
                            break;
                        case "EXIT_FROM_GAME":
                            Server.RemoveUserFromGame(nickname);
                            break;
                        case "SWITCH_TO_GAME":
                            Server.clientsInGame.Add(nickname, clientSocket);
                            Server.AllUsersInGame();
                            break;
                        case "FOR"://piece1:piece2:piece3:nickname:row+1
                            HandleGame.Formatie(msg[1], msg[2], msg[3], msg[4], msg[5]);
                            break;
                        case "MESSAGE_GAME":
                            Server.BroadcastInGame("MESSAGE_GAME:", nickname, message.Substring(message.IndexOf(':')+1, message.Length-message.IndexOf(':')-1));
                            break;
                        case "ADD_PIECE"://row:image:column:client_to_add
                            //String s=Server.formations[Int32.Parse (msg[1])];

                            String s=Server.formations.Cast<String>().First(e => e.Split(':').ElementAt(3).Equals(msg[4])&&
                                e.Split(':').ElementAt(4).Equals(msg[1]));
                            String[] s1=s.Split(':');//1:405:406:nickname:row
                            if(s1[0].Equals("1")) {
                                if(HandleGame.testInitialNum(Server.pieces[Int32.Parse(msg[2])], Int32.Parse(s1[1]))&&msg[3].Equals("0")) {
                                    Server.BroadcastInGame("ADD_PIECE_ON_FIRST_COL:", msg[4], msg[1]+":"+msg[2]+":"+msg[3]);
                                    int index=Array.IndexOf(Server.formations.ToArray(), s);
                                    Server.formations[index]=s1[0]+":"+Server.pieces[Int32.Parse(msg[2])]+":"+s1[2]+":"+msg[4]+":"+msg[1];
                                } else if(HandleGame.testFinalNum(Int32.Parse(s1[2]), Server.pieces[Int32.Parse(msg[2])])&&!msg[3].Equals("0")) {
                                    Server.BroadcastInGame("ADD_PIECE:", msg[4], msg[1]+":"+msg[2]+":"+msg[3]);
                                    int index=Array.IndexOf(Server.formations.ToArray(), s);
                                    Server.formations[index]=s1[0]+":"+s1[1]+":"+Server.pieces[Int32.Parse(msg[2])]+":"+msg[4]+":"+msg[1];
                                } else {
                                    HandleGame.MsgtoGameClient(nickname, "DONT:Nu se lipeste!");
                                }
                            } else if(s1[0].Equals("2")) {
                                if(Server.pieces[Int32.Parse(msg[2])]==Int32.Parse(s1[1])) {
                                    Server.BroadcastInGame("ADD_PIECE:", msg[4], msg[1]+":"+msg[2]+":"+msg[3]);
                                    int index=Array.IndexOf(Server.formations.ToArray(), s);
                                    Server.formations[index]=s1[0]+":0:0:"+msg[4]+":"+msg[1]; ;
                                } else {
                                    HandleGame.MsgtoGameClient(nickname, "DONT:Nu se lipeste!");
                                }
                            }
                            break;
                        case "DRAW":
                            int i=Server.random.Next();
                            HandleGame.MsgtoGameClient(nickname, "DRAW:"+i);
                            break;
                        case "PUT_PIECE_ON_BORD":
                            Server.pieces_on_board.Add(Server.pieces[Int32.Parse(msg[1])]);
                            Server.BroadcastInGame("PUT_PIECE_ON_BORD:", nickname, msg[1]);
                            break;
                        default:
                            Console.WriteLine("Error 404: Keyword not found");
                            break;
                    }
                }
            }
        }

    }
}
