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
            String readData=null;
            Room room;
            User user;
            while(networkStream.CanRead) {
                message=read.ReadLine();
                if(message!=null) {
                    readData=message.Substring(message.IndexOf(':')+1);
                    msg=message.Split(':');
                    Console.WriteLine("From client- "+nickname+": "+message);
                    switch(msg[0]) {
                        case "MESSAGE_CHAT":
                            MessageSender.Broadcast("MESSAGE_CHAT:", nickname, readData, Server.clientsList);
                            break;
                        case "EXIT_FROM_CHAT":
                            if(!msg[1].Equals("Am iesit din chat server")) {
                                if(msg[2].Equals("false")) {
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
                            MessageSender.AllUsers("NEW_USER_IN_GAME", room.GetClientsInRoom(), false);
                            String s=null;
                            foreach(int i in room.random.Next14()) {
                                s=s+":"+i;
                            }
                            MessageSender.MsgtoClient(nickname, "DRAW"+s, room.GetClientsInRoom());
                            MessageSender.Broadcast("YOUR_TURN:", room.GetClientsInRoom().ElementAt(0).Nickname, "end", room.GetClientsInRoom());
                            if(room.GetClientsInRoom().ElementAt(0).Nickname==nickname) {
                                MessageSender.MsgtoClient(nickname, "DRAW:"+room.random.Next(), room.GetClientsInRoom());
                                room.GetClientsInRoom().Single(c => c.Nickname==nickname).MyTurn=true;
                            }
                            break;
                        case "FORMATION"://piece1:piece2:piece3:nickname:row+1:roomName
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[6]));
                            user=room.GetClientsInRoom().Single(c => c.Nickname==nickname);
                            if(user.MyTurn) {
                                HandleFormations.Formatie(msg[1], msg[2], msg[3], msg[4], msg[5], msg[6]);
                            } else
                                MessageSender.MsgtoClient(nickname, "DONT:Nu e tura ta!", room.GetClientsInRoom());
                            break;
                        case "MESSAGE_GAME"://roomName:message
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.Broadcast("MESSAGE_GAME:", nickname, msg[2], room.GetClientsInRoom());
                            break;
                        case "ADD_PIECE"://row:imageIndex:column:clientToAdd:roomName
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[5]));
                            user=room.GetClientsInRoom().Single(c => c.Nickname==nickname);
                            bool takePiece=true;
                            if(user.MyTurn&&user.Etalat) {
                                if(msg[4]==nickname)
                                    takePiece=false;
                                HandleFormations.AddPiece(msg[1], msg[2], msg[3], msg[4], nickname, msg[5], takePiece);
                            } else
                                MessageSender.MsgtoClient(nickname, "DONT:Nu e tura ta!", room.GetClientsInRoom());
                            break;
                        case "DRAW"://roomName
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            user=room.GetClientsInRoom().Single(c => c.Nickname==nickname);
                            if(user.FirstDraw&&user.MyTurn) {
                                MessageSender.MsgtoClient(nickname, "DRAW:"+room.random.Next(), room.GetClientsInRoom());
                                user.FirstDraw=false;
                            } else  if(!user.MyTurn)
                                    MessageSender.MsgtoClient(nickname, "DONT:Nu e tura ta!", room.GetClientsInRoom());
                                else if(!user.FirstDraw)
                                    MessageSender.MsgtoClient(nickname, "DONT:Ai tras deja!", room.GetClientsInRoom());
                            break;
                        case "PUT_PIECE_ON_BORD"://index:roomName
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[2]));
                            user=room.GetClientsInRoom().Single(c => c.Nickname==nickname);
                            if(!user.FirstDraw&&user.MyTurn) {
                                room.piecesOnBoard.Add(room.pieces[Int32.Parse(msg[1])]);
                                MessageSender.Broadcast("PUT_PIECE_ON_BORD:", nickname, msg[1], room.GetClientsInRoom());
                                room.EndTurnFor(nickname);
                                MessageSender.Broadcast("YOUR_TURN:", room.GetClientTurn(), "end", room.GetClientsInRoom());
                                room.GetClientsInRoom().Single(c => c.Nickname==room.GetClientTurn()).FirstDraw=true;
                                room.GetClientsInRoom().Single(c => c.Nickname==room.GetClientTurn()).MyTurn=true;
                            } else if(!user.MyTurn)
                                MessageSender.MsgtoClient(nickname, "DONT:Nu e tura ta!", room.GetClientsInRoom());
                            else if(user.FirstDraw)
                                MessageSender.MsgtoClient(nickname, "DONT:Trage o piesa mai intai!", room.GetClientsInRoom());
                            break;
                        case "CREATE_ROOM":
                            MessageSender.RemoveUser("NEW_USER_IN_CHAT", nickname, Server.clientsList);
                            Server.roomList.Add(new Room(msg[1]));
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            room.AddClientInRoom(nickname, clientSocket);
                            MessageSender.AllUsers("ALL_USERS_IN_ROOM", room.GetClientsInRoom(), true);
                            MessageSender.AllRooms(Server.roomList, Server.clientsList);
                            break;
                        case "QUIT_ROOM":
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.RemoveUser("ALL_USERS_IN_ROOM", nickname, room.GetClientsInRoom());
                            if(room.GetClientsInRoom().Count==0) {
                                Server.roomList.Remove(room);
                            }
                            Server.clientsList.Add(new User(nickname, clientSocket));
                            MessageSender.AllRooms(Server.roomList, Server.clientsList);
                            MessageSender.AllUsers("NEW_USER_IN_CHAT", Server.clientsList, false);
                            break;
                        case "JOIN_ROOM":
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.RemoveUser("NEW_USER_IN_CHAT", nickname, Server.clientsList);
                            room.AddClientInRoom(nickname, clientSocket);
                            MessageSender.AllUsers("ALL_USERS_IN_ROOM", room.GetClientsInRoom(), true);
                            break;
                        case "MESSAGE_ROOM":
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            MessageSender.Broadcast("MESSAGE_ROOM:", nickname, msg[2], room.GetClientsInRoom());
                            break;
                        case "READY":
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            user=room.GetClientsInRoom().Single(u => u.Nickname==nickname);
                            user.Ready=!user.Ready;
                            MessageSender.Broadcast("READY:", nickname, ""+user.Ready, room.GetClientsInRoom());
                            if(room.GetClientsInRoom().All(u => u.Ready==true))
                                MessageSender.Broadcast("START_GAME:", nickname, "start", room.GetClientsInRoom());
                            break;
                        case "ETALARE":
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            user=room.GetClientsInRoom().Single(u => u.Nickname==nickname);
                            if(user.formations.Exists(u => u.Split(':').ElementAt(0).Equals("1"))&&
                               user.formations.Exists(u => u.Split(':').ElementAt(0).Equals("2"))&&
                               HandleFormations.CalculatePoints(readData)>45) {
                                MessageSender.MsgtoClient(nickname, "ETALARE:you may", room.GetClientsInRoom());
                                user.Etalat=true;
                            }
                            break;
                        case "REMOVE_PIECES":
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            user=room.GetClientsInRoom().Single(u => u.Nickname==nickname);
                            user.formations.Clear();
                            MessageSender.Broadcast("REMOVE_PIECES:", nickname, "all", room.GetClientsInRoom());
                            break;
                        case "DRAW_FROM_BOARD"://roomName:index
                            room=Server.roomList.Cast<Room>().Single(r => r.getRoomName().Equals(msg[1]));
                            user=room.GetClientsInRoom().Single(u => u.Nickname==nickname);
                            if(user.FirstDraw&&user.MyTurn&&user.Etalat) {
                                List<int> allPieces=room.piecesOnBoard.FindAll(i => room.piecesOnBoard.IndexOf(i)>=room.piecesOnBoard.IndexOf(room.pieces.ElementAt(Int32.Parse(msg[2])))).ToList();
                                String all=null;
                                foreach(int i in allPieces) {
                                    all=all+":"+room.pieces.IndexOf(i);
                                    room.piecesOnBoard.Remove(i);
                                }
                                MessageSender.Broadcast("DRAW_FROM_BOARD:", nickname, all.Substring(1), room.GetClientsInRoom());
                                user.FirstDraw=false;
                            } else if(!user.MyTurn)
                                MessageSender.MsgtoClient(nickname, "DONT:Nu e tura ta!", room.GetClientsInRoom());
                            else if(!user.FirstDraw)
                                MessageSender.MsgtoClient(nickname, "DONT:Ai tras deja!", room.GetClientsInRoom());
                            else if(!user.Etalat)
                                MessageSender.MsgtoClient(nickname, "DONT:Nu te-ai etalat inca!", room.GetClientsInRoom());
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
