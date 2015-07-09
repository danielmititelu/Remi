using System;
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
        public List<string> formations=new List<String>();
        public List<string> piecesOnFormations=new List<string>();
        public List<string> piecesOnTable=new List<string>();
        int _score=0;
        bool _ready=false;
        bool _myTurn=false;
        bool _firstDraw=false;
        bool _etalat=false;
        bool _winner=false;

        public User(String nickname, TcpClient clientSocket) {
            _nickname=nickname;
            _clientSocket=clientSocket;
            write=new StreamWriter(_clientSocket.GetStream());
        }
        public string Nickname {
            get { return _nickname; }
            set { _nickname=value; }
        }
        public TcpClient Client {
            get { return _clientSocket; }
            set { _clientSocket=value; }
        }
        public bool Ready {
            get { return _ready; }
            set { _ready=value; }
        }
        public bool MyTurn {
            get { return _myTurn; }
            set { _myTurn=value; }
        }
        public bool FirstDraw {
            get { return _firstDraw; }
            set { _firstDraw=value; }
        }
        public bool Etalat {
            get { return _etalat; }
            set { _etalat=value; }
        }
        public bool Winner {
            get { return _winner; }
            set { _winner=value; }
        }
        public int Score {
            get { return CalculatePoints(); }
            set { _score=value; }
        }
        public int CalculatePoints() {
            int points=0;
            foreach(String index in piecesOnFormations) {
                int piece=Pieces.GetInstance().pieces[Int32.Parse(index)];
                int number=Int32.Parse(piece.ToString().Substring(1, 2));
                if(number==1)
                    points=points+25;
                else if(number==0)
                    points=points+50;
                else if(number<10)
                    points=points+5;
                else if(number>=10)
                    points=points+10;
            }
            if(_etalat||_winner) {
                if(_winner) {
                    points=points+100;
                } else {
                    foreach(String index in piecesOnTable) {
                        int piece=Pieces.GetInstance().pieces[Int32.Parse(index)];
                        int number=Int32.Parse(piece.ToString().Substring(1, 2));
                        if(number==1)
                            points=points-25;
                        else if(number==0)
                            points=points-50;
                        else if(number<10)
                            points=points-5;
                        else if(number>=10)
                            points=points-10;
                    }
                }
            } else
                points=-100;

            _score=points;
            return _score;
        }
        public int ScoreOnFormation() {
            int points=0;
            foreach(String index in piecesOnFormations) {
                int piece=Pieces.GetInstance().pieces[Int32.Parse(index)];
                int number=Int32.Parse(piece.ToString().Substring(1, 2));
                if(number==1)
                    points=points+25;
                else if(number==0)
                    points=points+50;
                else if(number<10)
                    points=points+5;
                else if(number>=10)
                    points=points+10;
            }
            return points;
        }
        public void WriteLine(String message) {
            write.WriteLine(message);
            write.Flush();
        }
    }
}
