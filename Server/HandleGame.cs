﻿using System;
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

        public HandleGame(TcpClient clientSocket, String nickname) {
            this.clientSocket=clientSocket;
            this.nickname=nickname;
            doChat();
        }

        private void doChat() {
            NetworkStream networkStream=clientSocket.GetStream();
            StreamReader read=new StreamReader(networkStream);
            String[] msg=null;
            String message=null;

            while(networkStream.CanRead) {
                message=read.ReadLine();
                if(message!=null) {
                    msg=message.Split(':');
                    Console.WriteLine("From client in-game- "+nickname+": "+message);
                    switch(msg[0]) {

                        default:
                            Console.WriteLine("Error 404: Keyword not found");
                            break;
                    }
                }
            }
        }

        public static void MsgtoGameClient(String nickname, String msg) {
            TcpClient clientSocket=null;
            StreamWriter write=null;
            if(Server.clientsInGame.ContainsKey(nickname)) { // TODO: Initialize StreamWriter at the begining
                clientSocket=(TcpClient) Server.clientsInGame[nickname];
                write=new StreamWriter(clientSocket.GetStream());
                write.WriteLine(msg);
                write.Flush();
                write=null;
                Console.WriteLine(nickname+":"+msg);
            }
        }

        public static void Formatie(String msg1, String msg2, String msg3, String nickname, String row) {
            int a=Server.pieces[Int32.Parse(msg1)];
            int b=Server.pieces[Int32.Parse(msg2)];
            int c=Server.pieces[Int32.Parse(msg3)];
            Console.WriteLine(a+":"+b+":"+c+":"+nickname+":"+row);
            String res=testeaza(a, b, c, nickname, row);

            if(res.Equals("Nu este o formatie")) {
                MsgtoGameClient(nickname, "DONT:"+res);
            } else {
                Server.BroadcastInGame("FORMATION:", nickname, msg1+":"+msg2+":"+msg3);
            }
            Console.WriteLine(res);
        }

        private static string testeaza(int a, int b, int c, String nickname, String row) {
            if(testInitialNum(a, b)&&testFinalNum(b, c)&&testIntNum(a, c)) {
                Server.formations.Add(numCode(a, b, c, nickname, row));
                return "formatie";
            } else if(testPereche(a, b)&&testPereche(b, c)&&testPereche(a, c)) {
                Server.formations.Add(missingPiece(a, b, c, nickname, row));
                return "formatie";
            } else {
                return "Nu este o formatie";
            }
        }

        private static string numCode(int a, int b, int c, String nickname, String row) {
            return "1:"+a+":"+c+":"+nickname+":"+row;
        }

        private static string missingPiece(int a, int b, int c, String nickname, String row) {
            int a1=Int32.Parse(a.ToString().Substring(0, 1));//102:202:302:402:500
            int b1=Int32.Parse(b.ToString().Substring(0, 1));
            int c1=Int32.Parse(c.ToString().Substring(0, 1));
            var list=new List<int>(new[] { 1, 2, 3, 4, 5 });
            var result=list.Except(new[] { a1, b1, c1 });
            int test=result.ElementAt(0);
            int test1=result.ElementAt(1);//modify me
            String number=a.ToString().Substring(1, 2);
            int res=10-( a1+b1+c1 );
            return "2:"+res+number+":999"+":"+nickname+":"+row;
        }
        public static bool testInitialNum(int a, int b) {
            if(a==500&&( b==101||b==201||b==301||b==401 )) {
                return false;
            } else if(( b-a )==1||a==500||b==500) {
                return true;
            } else {
                return false;
            }
        }
        public static bool testFinalNum(int a, int b) {
            if(( b-a )==1||( a-b )==12||a==500||b==500) {
                return true;
            } else {
                return false;
            }
        }
        public static bool testIntNum(int a, int b) {
            if(( b-a )==2||a==500||b==500||( a-b )==11) {
                return true;
            } else {
                return false;
            }
        }
        public static bool testPereche(int a, int b) {
            if(Math.Abs(a-b)==100||Math.Abs(a-b)==200||Math.Abs(a-b)==300||a==500||b==500) {
                return true;
            } else {
                return false;
            }
        }
    }
}
