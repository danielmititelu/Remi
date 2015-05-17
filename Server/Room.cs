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
        public static Hashtable clientsInRoom=new Hashtable();
        private string p;

        public Room(string p) {
            // TODO: Complete member initialization
            this.p=p;
        }
       

    }
}
