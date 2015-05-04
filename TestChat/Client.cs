using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestChat {
    public class Client {
        TcpClient _tcpClient=new TcpClient ();
        NetworkStream _serverStream=default (NetworkStream);
        StreamWriter _writer=null;
        StreamReader _reader=null;

        public Client (String ip) {
            _tcpClient.Connect (ip, 5150);
            _serverStream=_tcpClient.GetStream ();
            _writer=new StreamWriter (_serverStream);
            _reader=new StreamReader (_serverStream);
        }

        public Client () {

        }
        public Client (Client client) {
            _tcpClient=client._tcpClient;
            _serverStream=client._serverStream;
            _writer=client._writer;
            _reader=client._reader;
        }

        public void WriteLine (String message) {
            _writer.WriteLine (message);
            _writer.Flush ();
        }
        public void Close () {
            _tcpClient.Close ();
            _serverStream.Close ();
            _writer.Close ();
            _reader.Close ();
        }

        public string ReadLine () {
            try {
                return _reader.ReadLine ();
            } catch(Exception e) {
               Console.WriteLine(e.StackTrace);
               return "DONT:eroare" ;
            }     
        }
        public bool ClientConnected () {
            return _tcpClient.Connected;
        }
    }
}
