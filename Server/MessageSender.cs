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

        public static void Broadcast(String keyword, String nickname, String msg, List<User> userList) {
            foreach(User user in userList) {
                if(msg.Trim()==""||(TcpClient) user.Client==null)
                    continue;

                user.WriteLine(keyword+nickname+":"+msg);
                Console.WriteLine(keyword+nickname+":"+msg);
            }
        }

        public static void AllUsers(string keyword, List<User> userList, bool room) {
            String allNicknames=null;

            foreach(User item in userList) {
                if(room)
                    allNicknames=allNicknames+":"+item.Nickname+"-"+item.Ready;
                else
                    allNicknames=allNicknames+":"+item.Nickname;
            }
            foreach(User user in userList) {
                user.WriteLine(keyword+allNicknames);
            }
        }

        public static void RemoveUser(string keyword, string nickname, List<User> userList) {
            if(userList.Exists(c => c.Nickname==nickname)) {
                userList.Remove(userList.Single(c => c.Nickname==nickname));
                AllUsers(keyword, userList,false);
            }
        }
        public static void MsgtoClient(String nickname, String msg, List<User> userList) {
            if(userList.Exists(c => c.Nickname==nickname)) {
                userList.Single(c => c.Nickname==nickname).WriteLine(msg);
            }
        }
        public static void AllRooms(List<Room> roomsList, List<User> userList) {
            String allRooms=null;

            foreach(Room room in roomsList) {
                allRooms=allRooms+":"+room.getRoomName();
            }
            foreach(User user in userList) {
                user.WriteLine("NEW_ROOM"+allRooms);
            }
            Console.WriteLine("Camere create"+allRooms);
        }

    }
}
