using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class MessageSender {

        public static void Broadcast(String keyword, String nickname, String msg, Hashtable userList) {
            TcpClient broadcastSocket=null;
            StreamWriter write=null;
            foreach(DictionaryEntry item in userList) {
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
        public static void AllUsers(string keyword, Hashtable userList) {
            String allNicknames=null;
            TcpClient broadcastSocket=null;
            StreamWriter write=null;

            foreach(DictionaryEntry item1 in userList) {
                allNicknames=allNicknames+":"+item1.Key;
            }
            foreach(DictionaryEntry item in userList) {
                try {
                    broadcastSocket=(TcpClient) item.Value;
                    write=new StreamWriter(broadcastSocket.GetStream());

                    write.WriteLine(keyword+allNicknames);

                    write.Flush();
                    write=null;
                } catch(Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static void RemoveUser(string keyword, string nickname, Hashtable userList) {
            if(userList.ContainsKey(nickname)) {
                userList.Remove(nickname);
                AllUsers(keyword, userList);
            }
        }
        public static void MsgtoClient(String nickname, String msg, Hashtable UserList) {
            TcpClient clientSocket=null;
            StreamWriter write=null;
            if(UserList.ContainsKey(nickname)) { // TODO: Initialize StreamWriter at the begining
                clientSocket=(TcpClient) UserList[nickname];
                write=new StreamWriter(clientSocket.GetStream());
                write.WriteLine(msg);
                write.Flush();
                write=null;
                //Console.WriteLine(nickname+":"+msg);
            }
        }
        public static void AllRooms(List<Room> roomsList, Hashtable userList) {
            String allRooms=null;
            TcpClient broadcastSocket=null;
            StreamWriter write=null;

            foreach(Room room in roomsList) {
                allRooms=allRooms+":"+room.getRoomName();
            }
            foreach(DictionaryEntry item in userList) {
                try {
                    broadcastSocket=(TcpClient) item.Value;
                    write=new StreamWriter(broadcastSocket.GetStream());

                    write.WriteLine("NEW_ROOM"+allRooms);

                    write.Flush();
                    write=null;
                } catch(Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.WriteLine("Camere create"+allRooms);
        }

    }
}
