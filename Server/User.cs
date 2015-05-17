﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class User {
        String _nickname;
        TcpClient _clientSocket;
        StreamWriter write=null;

        public User(String nickname, TcpClient clientSocket) {
            _nickname=nickname;
            _clientSocket=clientSocket;
            write=new StreamWriter(_clientSocket.GetStream());
        }
        public string Nickname{
            get { return _nickname; }
            set { _nickname=value; }
        }
        public TcpClient Client {
            get { return _clientSocket; }
            set { _clientSocket=value; }
        }
        public void WriteLine(String message) {
            write.WriteLine(message);
            write.Flush();
        }

    }
}