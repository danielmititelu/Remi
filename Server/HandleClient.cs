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
                            String s=null;
                            foreach(int i in Server.random.Next14()) {
                                s=s+":"+i;
                            }
                            HandleGame.MsgtoGameClient(nickname, "DRAW"+s);
                            break;
                        case "FOR"://piece1:piece2:piece3:nickname:row+1
                            HandleGame.Formatie(msg[1], msg[2], msg[3], msg[4], msg[5]);
                            break;
                        case "MESSAGE_GAME":
                            Server.BroadcastInGame("MESSAGE_GAME:", nickname, message.Substring(message.IndexOf(':')+1, message.Length-message.IndexOf(':')-1));
                            break;
                        case "ADD_PIECE"://row:imageIndex:column:clientToAdd
                            HandleGame.AddPiece(msg[1], msg[2], msg[3], msg[4],nickname);
                            break;
                        case "DRAW":
                            HandleGame.MsgtoGameClient(nickname, "DRAW:"+Server.random.Next());
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
