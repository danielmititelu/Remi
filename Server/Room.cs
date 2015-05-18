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
        private List<User> clientsInRoom;
        private string _roomName;
        public  List<int> pieces=new List<int>();
        public  List<String> formations=new List<String>();
        public  UniqueRandom random=new UniqueRandom(Enumerable.Range(0, 105));
        public  List<int> pieces_on_board=new List<int>();

        public Room(string roomName) {
            _roomName=roomName;
            clientsInRoom=new List<User>();
            GeneratePieces();
        }
        public string getRoomName() {
            return _roomName;
        }

        public void AddClientInRoom(string nickname, TcpClient clientSocket) {
            clientsInRoom.Add(new User(nickname, clientSocket));
        }
        public List<User> GetClientsInRoom() {
            return clientsInRoom;
        }

        public void GeneratePieces() {
            int c=0;
            int n=1;
            string zero="0";
            for(int i=0 ; i<=105 ; i++) {
                if(i==52||i==105) {
                    c=0;
                    pieces.Insert(i, 500);
                    continue;
                }
                if(i<52) {
                    if(i%13!=0) {
                        n++;
                    }
                    if(i%13==0) {
                        c++;
                        n=1;
                    }
                } else {
                    if(( i-1 )%13!=0) {
                        n++;
                    }
                    if(( i-1 )%13==0) {
                        c++;
                        n=1;
                    }
                }
                if(n<10) {
                    zero="0";
                } else {
                    zero="";
                }
                pieces.Insert(i, Int32.Parse(c.ToString()+zero+n.ToString()));
            }
        }

    }
}
