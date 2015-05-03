using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class HandleGame {

        
        TcpClient clientSocket;
        String nickname;
        String[] formations=new String[100];
        int row=-1;

        public HandleGame (TcpClient clientSocket, String nickname) {
            this.clientSocket=clientSocket;
            this.nickname=nickname; 
            doChat ();
        }

        private void doChat () {
            NetworkStream networkStream=clientSocket.GetStream ();
            StreamReader read=new StreamReader (networkStream);
            String[] msg=null;
            String message=null;
            
            try {
                while (networkStream.CanRead) {
                    message=read.ReadLine ();
                    msg=message.Split (':');
                    if (message!=null) {
                        Console.WriteLine ("From client in-game- "+nickname+": "+message);
                        switch (msg[0]) {
                            case "FOR":
                                Formatie (msg[1], msg[2], msg[3]);
                                break;
                            case "MESSAGE":
                                Server.BroadcastInGame (nickname, message.Substring (message.IndexOf (':')+1, message.Length-message.IndexOf (':')-1));   
                                break;
                            case "ADD_PIECE":
                                String s = formations[Int32.Parse(msg[1])];
                                String[] s1 = s.Split(':');//1:405:406
                                if(s1[0].Equals("1")){
                                    if (testInitialNum (Server.pieces[Int32.Parse (msg[2])],Int32.Parse (s1[1])) && msg[3].Equals("0")) {
                                        Server.MsgtoGameClient (nickname, "ADD_PIECE_ON_FIRST_COL:"+nickname);
                                        formations[row]=s1[0]+":"+Server.pieces[Int32.Parse (msg[2])]+":"+s1[2];
                                    } else if (testFinalNum (Int32.Parse (s1[2]), Server.pieces[Int32.Parse (msg[2])])&&!msg[3].Equals ("0")) {
                                        Server.MsgtoGameClient (nickname, "ADD_PIECE:"+nickname);
                                        formations[row]=s1[0]+":"+s1[1]+":"+Server.pieces[Int32.Parse (msg[2])];
                                    } else {
                                        Server.MsgtoGameClient (nickname, "DONT_ADD_PIECE:"+nickname);
                                    }
                                }else if(s1[0].Equals("2")){
                                    if (Server.pieces[Int32.Parse (msg[2])]==Int32.Parse (s1[1])) {
                                        Server.MsgtoGameClient (nickname, "ADD_PIECE:"+nickname);
                                        formations[row]="0";
                                    } else {
                                        Server.MsgtoGameClient (nickname, "DONT_ADD_PIECE:"+nickname);
                                    }
                                }
                                break;
                            case "DRAW":
                                int i=Server.random.Next ();                             
                                Server.MsgtoGameClient (nickname, "DRAW:"+i);
                                break;
                            case "EXIT":
                                Server.RemoveUserFromGame (nickname);
                                break;
                            default:
                                Console.WriteLine ("Error 404: Keyword not found");
                                break;
                        }
                    }
                }
            } catch (Exception ) {

            }
        }

        
        private void Formatie (String msg1, String msg2, String msg3) {
            int a=Server.pieces[Int32.Parse (msg1)];
            int b=Server.pieces[Int32.Parse (msg2)];
            int c=Server.pieces[Int32.Parse (msg3)];
            Console.WriteLine (a+":"+b+":"+c);
            String res=testeaza (a, b, c);
            Server.MsgtoGameClient (nickname, "FORMATION:"+res);
            Console.WriteLine (res);
        }

        private string testeaza (int a, int b, int c) {
            if (testInitialNum (a, b)&&testFinalNum (b, c)&&testIntNum (a, c)) {
                row++;
                formations[row]=numCode (a, b, c);  
                return "Numaratoare";
            } else if (testPereche (a, b)&&testPereche (b, c)&&testPereche (a, c)) {
                row++;
                formations[row]=missingPiece (a, b, c);
                return "Pereche"; 
            } else {
                return "Nu este o formatie";
            }
        }

        private string numCode (int a, int b, int c) {     
            return "1:"+a+":"+c;
        }

        private string missingPiece (int a, int b, int c) {
            int a1=Int32.Parse(a.ToString ().Substring(0,1));//102:202:302:402:500
            int b1=Int32.Parse(b.ToString ().Substring(0,1));
            int c1=Int32.Parse(c.ToString ().Substring (0, 1));
            String number=a.ToString ().Substring (1,2);
            int res=10-(a1+b1+c1);
            return "2:"+res+number;
        }
        public bool testInitialNum (int a, int b) {
            if (a==500&&(b==101||b==201||b==301||b==401)) {
                return false;
            } else if ((b-a)==1||a==500||b==500) {
                return true;
            } else {
                return false;
            }
        }
        public bool testFinalNum (int a, int b) {
            if ((b-a)==1||(a-b)==12||a==500||b==500) {
                return true;
            } else {
                return false;
            }
        }
        public bool testIntNum (int a, int b) {
            if ((b-a)==2||a==500||b==500||(a-b)==11) {
                return true;
            } else {
                return false;
            }
        }
        public bool testPereche (int a, int b) {
            if (Math.Abs (a-b)==100||Math.Abs (a-b)==200||Math.Abs (a-b)==300||a==500||b==500) {
                return true;
            } else {
                return false;
            }
        }     
    }
}
