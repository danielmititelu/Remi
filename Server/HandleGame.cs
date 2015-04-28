using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class HandleGame {

        int[] piese=new int[106];
        TcpClient clientSocket;
        String nickname;
        int i=-1;

        public HandleGame (TcpClient clientSocket, String nickname) {
            this.clientSocket=clientSocket;
            this.nickname=nickname;
            GenereazaPiese ();
            doChat ();
    
        }

        private void doChat () {
            NetworkStream networkStream=clientSocket.GetStream ();
            StreamReader read=new StreamReader (networkStream);
            String[] msg=null;
            String message=null;
            UniqueRandom random=new UniqueRandom (Enumerable.Range (0, 105));
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
                                Server.BroadcastInGame (nickname, msg[1]);
                                break;
                            case "DRAW":
                                i=random.Next ();
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
            } catch (Exception) {


            }
        }

        public void GenereazaPiese () {
            int c=0;
            int n=1;
            string zero="0";
            for (int i=0; i<=104; i++) {
                if (i==52) {
                  
                    c=0;
                }
                if (i<52) {
                    if (i%13!=0) {
                        n++;
                    }
                    if (i%13==0) {
                        c++;
                        n=1;
                    }
                } else {
                    if ((i-1)%13!=0) {
                        n++;
                    }
                    if ((i-1)%13==0) {
                        c++;
                        n=1;
                    }
                }
                if (n<10) {
                    zero="0";
                } else {
                    zero="";
                }
                piese[i]=Int32.Parse (c.ToString ()+zero+n.ToString ());
            }
            piese[52]=500;
            piese[105]=500;
        }
        private void Formatie (String msg1, String msg2, String msg3) {
            //string a1=(piese[Int32.Parse (msg1)].ToString ()).Substring (0, 3);
            //string b1=(piese[Int32.Parse (msg2)].ToString ()).Substring (0, 3);
            //string c1=(piese[Int32.Parse (msg3)].ToString ()).Substring (0, 3);

            int a=piese[Int32.Parse (msg1)];
            int b=piese[Int32.Parse (msg2)];
            int c=piese[Int32.Parse (msg3)];
            Console.WriteLine (a+":"+b+":"+c);
            String res=testeaza (a, b, c);
            Server.MsgtoGameClient (nickname, "FORMATION:"+res);
            Console.WriteLine (res);
        }

        private string testeaza (int a, int b, int c) {
            if (testInitialNum (a, b)&&testFinalNum (b, c)&&testIntNum (a, c)) {
                return "Numaratoare";
            } else if (testPereche (a, b)&&testPereche (b, c)&&testPereche (a, c)) {
                return "Pereche";
            } else {
                return "Nu este o formatie";
            }
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
