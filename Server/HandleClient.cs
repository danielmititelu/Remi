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
                                if(msg[2].Equals("false")){
                                room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                                MessageSender.RemoveUser("ALL_USERS_IN_ROOM", nickname, room.GetClientsInRoom());
                                if(room.GetClientsInRoom().Count==0) {
                                    Server.roomList.Remove(room);
                                }
                                MessageSender.AllRooms(Server.roomList, Server.clientsList);
                                }
                            }
                            MessageSender.RemoveUser("NEW_USER_IN_CHAT", nickname, Server.clientsList);
                            break;
                        case "EXIT_FROM_GAME"://roomName
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.RemoveUser("NEW_USER_IN_GAME", nickname, room.GetClientsInRoom());
                            break;
                        case "SWITCH_TO_GAME"://roomName
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.AllUsers("NEW_USER_IN_GAME", room.GetClientsInRoom(),false);
                            String s=null;
                            foreach(int i in room.random.Next14()) {
                                s=s+":"+i;
                            }
                            MessageSender.MsgtoClient(nickname, "DRAW"+s, room.GetClientsInRoom());
                            break;
                        case "FOR"://piece1:piece2:piece3:nickname:row+1:roomName
                            HandleFormations.Formatie(msg[1], msg[2], msg[3], msg[4], msg[5],msg[6]);
                            break;
                        case "MESSAGE_GAME"://roomName:message
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.Broadcast("MESSAGE_GAME:", nickname, msg[2], room.GetClientsInRoom());
                            break;
                        case "ADD_PIECE"://row:imageIndex:column:clientToAdd:roomName
                            HandleFormations.AddPiece(msg[1], msg[2], msg[3], msg[4],nickname,msg[5]);
                            break;
                        case "DRAW"://roomName
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.MsgtoClient(nickname, "DRAW:"+room.random.Next(), room.GetClientsInRoom());
                            break;
                        case "PUT_PIECE_ON_BORD"://index:roomName
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[2]));
                            room.pieces_on_board.Add(room.pieces[Int32.Parse(msg[1])]);
                            MessageSender.Broadcast("PUT_PIECE_ON_BORD:", nickname, msg[1],room.GetClientsInRoom());
                           // MessageSender.Broadcast("ENDTURN:",nickname,"end",Server.clientsInGame);
                            break;
                        case "CREATE_ROOM":             
                            MessageSender.RemoveUser("NEW_USER_IN_CHAT", nickname, Server.clientsList);
                            Server.roomList.Add(new Room(msg[1]));
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            room.AddClientInRoom(nickname, clientSocket);
                            MessageSender.AllUsers("ALL_USERS_IN_ROOM",room.GetClientsInRoom(),true);
                            MessageSender.AllRooms(Server.roomList,Server.clientsList);
                            break;
                        case"QUIT_ROOM":
                            room = Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.RemoveUser("ALL_USERS_IN_ROOM", nickname, room.GetClientsInRoom());
                            if(room.GetClientsInRoom().Count==0) {
                                Server.roomList.Remove(room);
                            }
                            Server.clientsList.Add(new User(nickname,clientSocket));
                            MessageSender.AllRooms(Server.roomList, Server.clientsList);
                            MessageSender.AllUsers("NEW_USER_IN_CHAT", Server.clientsList,false);
                            break;
                        case"JOIN_ROOM":
                            room = Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.RemoveUser("NEW_USER_IN_CHAT", nickname, Server.clientsList);
                            room.AddClientInRoom(nickname, clientSocket);
                            MessageSender.AllUsers("ALL_USERS_IN_ROOM", room.GetClientsInRoom(),true);
                            break;
                        case "MESSAGE_ROOM":
                            room = Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.Broadcast("MESSAGE_ROOM:", nickname, msg[2], room.GetClientsInRoom());
                            break;
                        case "READY":
                            User user;
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            user = room.GetClientsInRoom().Single(u => u.Nickname == nickname) ;
                            user.Ready=!user.Ready;
                            MessageSender.Broadcast("READY:", nickname, ""+user.Ready, room.GetClientsInRoom());
                            if(room.GetClientsInRoom().All(u => u.Ready==true))
                                MessageSender.Broadcast("START_GAME:", nickname, "start", room.GetClientsInRoom());
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
