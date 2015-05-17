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
        private Hashtable clientsInRoom;
        private string _roomName;

        public Room(string roomName) {
            _roomName=roomName;
            clientsInRoom=new Hashtable();
        }
        public string getRoomName() {
            return _roomName;
        }

        public void AddClientInRoom(string nickname, TcpClient clientSocket) {
            clientsInRoom.Add(nickname, clientSocket);
        }
        public Hashtable GetClientsInRoom() {
            return clientsInRoom;
        }

    }
}
