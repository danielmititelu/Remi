using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class Room {
        private String _roomName;
        private List<User> clientsInRoom;
        public  UniqueRandom random=new UniqueRandom(Enumerable.Range(0, 105));
        public  List<int> piecesOnBoard=new List<int>();

        public Room(string roomName) {
            _roomName=roomName;
            clientsInRoom=new List<User>();
        }

        public string getRoomName() {
            return _roomName;
        }

        public void AddClientInRoom(String nickname, TcpClient clientSocket) {
            clientsInRoom.Add(new User(nickname, clientSocket));
        }

        public List<User> GetClientsInRoom() {
            return clientsInRoom;
        }

        public void EndTurnFor(string nickname) {  
            int index=clientsInRoom.FindIndex(c => c.Nickname==nickname);
            clientsInRoom.ElementAt(index).MyTurn=false;
            if(index+1==clientsInRoom.Count())
                clientsInRoom.ElementAt(0).MyTurn=true;
            else
            clientsInRoom.ElementAt(index+1).MyTurn=true;
        }
        public string GetClientTurn() {
            return clientsInRoom.Single(c=> c.MyTurn==true).Nickname;
        }
    }
}
