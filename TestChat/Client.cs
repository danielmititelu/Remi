using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Game {
    public class Client {
        static Client _instance;

        TcpClient _tcpClient=new TcpClient();
        NetworkStream _serverStream=default(NetworkStream);
        StreamWriter _writer=null;
        StreamReader _reader=null;
        string _nickName=null;

        public Client(String ip) {
            _tcpClient.Connect(ip, 5150);
            _serverStream=_tcpClient.GetStream();
            _writer=new StreamWriter(_serverStream);
            _reader=new StreamReader(_serverStream);
            _instance=this;
        }

        public void WriteLine(String message) {
            _writer.WriteLine(message);
            _writer.Flush();
        }

        public void Close() {
            _tcpClient.Close();
            _serverStream.Close();
            _writer.Close();
            _reader.Close();
        }

        public string ReadLine() {
            try {
                return _reader.ReadLine();
            } catch(Exception e) {
                Console.WriteLine(e.StackTrace);
            }
            return "";
        }

        public bool ClientConnected() {
            return _tcpClient.Connected;
        }

        public static Client GetInstance() {
            return _instance;
        }

        public static bool Exists() {
            if(_instance==null) {
                return false;
            }
            return true;
        }

        public void SetNickname(string name) {
            _nickName=name;
        }

        public string GetNickname() {
            return _nickName;
        }
    }
}
