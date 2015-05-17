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
            Room room;
            while(networkStream.CanRead) { // TODO : MAKE ANOTHER CLASS TO SUPPORT THIS
                message=read.ReadLine();
                if(message!=null) {
                    msg=message.Split(':');
                    Console.WriteLine("From client- "+nickname+": "+message);
                    switch(msg[0]) {
                        case "MESSAGE_CHAT":
                            MessageSender.Broadcast("MESSAGE_CHAT:",nickname, message.Substring(message.IndexOf(':')+1, message.Length-message.IndexOf(':')-1),Server.clientsList);
                            break;
                        case "EXIT_FROM_CHAT":
                            if(!msg[1].Equals("Am iesit din chat server")){
                                room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                                MessageSender.RemoveUser("ALL_USERS_IN_ROOM", nickname, room.GetClientsInRoom());
                                if(room.GetClientsInRoom().Count==0) {
                                    Server.roomList.Remove(room);
                                }
                                MessageSender.AllRooms(Server.roomList, Server.clientsList);
                            }
                            MessageSender.RemoveUser("NEW_USER_IN_CHAT", nickname, Server.clientsList);
                            break;
                        case "EXIT_FROM_GAME":
                            MessageSender.RemoveUser("NEW_USER_IN_GAME", nickname, Server.clientsInGame);
                            break;
                        case "SWITCH_TO_GAME":
                            Server.clientsInGame.Add(nickname, clientSocket);
                            MessageSender.AllUsers("NEW_USER_IN_GAME", Server.clientsInGame);
                            String s=null;
                            foreach(int i in Server.random.Next14()) {
                                s=s+":"+i;
                            }
                            MessageSender.MsgtoClient(nickname, "DRAW"+s, Server.clientsInGame);
                            break;
                        case "FOR"://piece1:piece2:piece3:nickname:row+1
                            HandleGame.Formatie(msg[1], msg[2], msg[3], msg[4], msg[5]);
                            break;
                        case "MESSAGE_GAME":
                            MessageSender.Broadcast("MESSAGE_GAME:", nickname, message.Substring(message.IndexOf(':')+1, message.Length-message.IndexOf(':')-1),Server.clientsInGame);
                            break;
                        case "ADD_PIECE"://row:imageIndex:column:clientToAdd
                            HandleGame.AddPiece(msg[1], msg[2], msg[3], msg[4],nickname);
                            break;
                        case "DRAW":
                            MessageSender.MsgtoClient(nickname, "DRAW:"+Server.random.Next(), Server.clientsInGame);
                            break;
                        case "PUT_PIECE_ON_BORD":
                            Server.pieces_on_board.Add(Server.pieces[Int32.Parse(msg[1])]);
                            MessageSender.Broadcast("PUT_PIECE_ON_BORD:", nickname, msg[1],Server.clientsInGame);
                           // Server.BroadcastInGame("TURN:",nickname,);
                            break;
                        case "CREATE_ROOM":             
                            MessageSender.RemoveUser("NEW_USER_IN_CHAT", nickname, Server.clientsList);
                            Server.roomList.Add(new Room(msg[1]));
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            room.AddClientInRoom(nickname, clientSocket);
                            MessageSender.AllUsers("ALL_USERS_IN_ROOM",room.GetClientsInRoom());
                            MessageSender.AllRooms(Server.roomList,Server.clientsList);
                            break;
                        case"QUIT_ROOM":
                            room = Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.RemoveUser("ALL_USERS_IN_ROOM", nickname, room.GetClientsInRoom());
                            if(room.GetClientsInRoom().Count==0) {
                                Server.roomList.Remove(room);
                            }
                            Server.clientsList.Add(nickname,clientSocket);
                            MessageSender.AllRooms(Server.roomList, Server.clientsList);
                            MessageSender.AllUsers("NEW_USER_IN_CHAT", Server.clientsList);
                            break;
                        case"JOIN_ROOM":
                            room = Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.RemoveUser("NEW_USER_IN_CHAT", nickname, Server.clientsList);
                            room.AddClientInRoom(nickname, clientSocket);
                            MessageSender.AllUsers("ALL_USERS_IN_ROOM", room.GetClientsInRoom());
                            break;
                        case "MESSAGE_ROOM":
                            room = Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.Broadcast("MESSAGE_ROOM:", nickname, msg[2], room.GetClientsInRoom());
                            break;
                        //case"READY":
                        //    room = Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                        //    MessageSender.Broadcast("READY:", nickname, "", room.GetClientsInRoom());
                        //    break;
                        default:
                            Console.WriteLine("Error 404: Keyword not found");
                            break;
                    }
                }
            }
        }
    }
}
