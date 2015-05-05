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

        public HandleClient (TcpClient clientSocket, String nickname) {
            this.clientSocket=clientSocket;
            this.nickname=nickname;
            doChat ();
        }

        private void doChat () {
            NetworkStream networkStream=clientSocket.GetStream ();
            StreamReader read=new StreamReader (networkStream);
            String[] msg=null;
            String message=null;
                while (networkStream.CanRead) {
                    message=read.ReadLine ();
                    if (message!=null) {
                    msg=message.Split(':');        
                        Console.WriteLine ("From client- "+nickname+": "+message);
                        switch (msg[0]) {  
                            case "MESSAGE":
                                Server.Broadcast (nickname, message.Substring (message.IndexOf (':')+1, message.Length-message.IndexOf (':')-1));
                                break;
                            case "EXIT":
                                Server.RemoveUser (nickname);
                                break;
                            default:
                                Console.WriteLine ("Error 404: Keyword not found");
                                break;
                        }
                    }
                }
        }
       
    }
}
