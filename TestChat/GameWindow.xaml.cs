﻿using CanvasItems;
using Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using UserControls;

namespace Game {
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class GameWindow : Window {
        private Point mouseClick;
        private double canvasLeft;
        private double canvasTop;
        bool formatie=false;
        int[] selectedImages=null;
        bool drawFromBoard=false;
        int n=0;
        public Image temp_img;
        String client_to_add;
        private string _roomName;
        DispatcherTimer timer=new DispatcherTimer();
        int time=3;
        bool etalat=false;
        static GameWindow _instance;

        public GameWindow() {
            InitializeComponent();
            _instance=this;
            this.Show();
            timer.Interval=new TimeSpan(0, 0, 1);
            timer.Tick+=timerTick;
        }

        public GameWindow(string roomName) {
            InitializeComponent();
            _instance=this;
            this.Show();
            _roomName=roomName;
            timer.Interval=new TimeSpan(0, 0, 1);
            timer.Tick+=timerTick;
        }

        private void timerTick(object sender, EventArgs e) {
            time--;
            if(time==0) {
                timer.Stop();
                this.Dispatcher.Invoke((Action) ( () => { error.Content=""; } ));
            }
        }

        public static GameWindow GetInstance() {
            return _instance;
        }

        public static bool Exists() {
            if(_instance==null) 
                return false;
             else
                return true;
        }

        public void SetText(string message) {
            this.Dispatcher.Invoke((Action) ( () => {
                received.AppendText(message+"\n");
                received.ScrollToEnd();
            } ));
        }

        public void ErrorText(string message) {
            this.Dispatcher.Invoke((Action) ( () => {
                error.Content=message;
                time=3;
                timer.Start();
            } ));
        }

        public void NewUser(string readData) {
            this.Dispatcher.Invoke((Action) ( () => {
                String[] users=readData.Split(':');
                player1.Content="[empty]";
                player2.Content="[empty]";
                player3.Content="[empty]";
                player4.Content="[empty]";
                int pos=0;
                for(int i=0 ; i<users.Count() ; i++) {
                    if(users[i].Equals(Client.GetInstance().GetNickname())) {
                        pos=i;
                    }
                }
                for(int i=pos ; i<users.Count() ; i++) {

                    if(i==pos) {
                        player1.Content=users[i];
                    } else if(i==pos+1) {
                        player2.Content=users[i];
                    } else if(i==pos+2) {
                        player3.Content=users[i];
                    } else if(i==pos+3) {
                        player4.Content=users[i];
                    }
                }
                for(int i=pos ; i>=0 ; i--) {

                    if(i==pos) {
                        player1.Content=users[i];
                    } else if(i==pos-1) {
                        player4.Content=users[i];
                    } else if(i==pos-2) {
                        player3.Content=users[i];
                    } else if(i==pos-3) {
                        player2.Content=users[i];
                    }
                }
            } ));
        }

        private void send_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key==Key.Enter) {
                Client.GetInstance().WriteLine("MESSAGE_GAME:"+_roomName+":"+send.Text);
                send.Text="";
            }
        }

        private void addImgListeners(Image img) {
            img.PreviewMouseDown+=myimg_MouseDown;
            img.PreviewMouseMove+=myimg_MouseMove;
            img.PreviewMouseUp+=myimg_MouseUp;
            img.LostMouseCapture+=myimg_LostMouseCapture;
        }

        public void RemoveImgListeners(Image img) {
            img.PreviewMouseDown-=myimg_MouseDown;
            img.PreviewMouseMove-=myimg_MouseMove;
            img.PreviewMouseUp-=myimg_MouseUp;
            img.LostMouseCapture-=myimg_LostMouseCapture;
        }

        public void AddEtalonListener(Image img) {
            img.MouseUp+=etalon_MouseUp;
        }

        public void RemoveEtalonListener(Image img) {
            img.MouseUp-=etalon_MouseUp;
        }
        public void AddBoardListener(Image img) {
            img.MouseDown+=boardMouseDown;
        }
        public void RemoveBoardListener(Image img) {
            img.MouseDown-=boardMouseDown;
        }

        private void boardMouseDown(object sender, MouseButtonEventArgs e) {
            Image selectedPiece=(Image) sender;
            int index=Pieces.GetInstance().getIndex(selectedPiece);
            Client.GetInstance().WriteLine("DRAW_FROM_BOARD:"+_roomName+":"+index);
        }

        public void etalon_MouseUp(object sender, MouseButtonEventArgs e) {
            if(temp_img!=null) {
                Image selectedPiece=(Image) sender;
                int c=Grid.GetColumn(selectedPiece);
                int r=Grid.GetRow(selectedPiece);
                int index=Pieces.GetInstance().getIndex(temp_img);
                Client.GetInstance().WriteLine("ADD_PIECE:"+r+":"+index+":"+c+":"+client_to_add+":"+_roomName);
            }
        }

        private void myimg_LostMouseCapture(object sender, MouseEventArgs e) {
            Image selectedPiece=(Image) sender;
            selectedPiece.ReleaseMouseCapture();
        }

        private void myimg_MouseUp(object sender, MouseButtonEventArgs e) {
            Image selectedPiece=(Image) sender;
            mouseClick=e.GetPosition(null);

            canvasLeft=Canvas.GetLeft(selectedPiece);
            canvasTop=Canvas.GetTop(selectedPiece);

            if(canvasLeft<0) {
                canvasLeft=0;
            }
            if(canvasTop<0) {
                canvasTop=0;
            }
            if(canvasLeft>MyTableCanvas.ActualWidth) {
                canvasLeft=MyTableCanvas.ActualWidth-selectedPiece.ActualWidth;
            }
            if(canvasTop>MyTableCanvas.ActualHeight) {
                canvasTop=MyTableCanvas.ActualHeight-selectedPiece.ActualHeight;
            }
            selectedPiece.SetValue(Canvas.LeftProperty, canvasLeft);
            selectedPiece.SetValue(Canvas.TopProperty, canvasTop);
            selectedPiece.ReleaseMouseCapture();
            if(StackCanvas.IsMouseOver) {
                if(!( etalon1.Children.Count==0 )&&!etalat)
                    Client.GetInstance().WriteLine("ETALARE:"+_roomName);

                int index=Pieces.GetInstance().getIndex(selectedPiece);
                if(!( MyTableCanvas.Children.Count==1 ))
                    Client.GetInstance().WriteLine("PUT_PIECE_ON_BORD:"+index+":"+_roomName+":"+"NotAWinner");
                else
                    Client.GetInstance().WriteLine("PUT_PIECE_ON_BORD:"+index+":"+_roomName+":"+"Winner");
            }
            if(etalon1.IsMouseOver) {
                temp_img=selectedPiece;
                client_to_add=Client.GetInstance().GetNickname();
            } else if(etalon2.IsMouseOver) {
                temp_img=selectedPiece;
                client_to_add=(String) player2.Content;
            } else if(etalon3.IsMouseOver) {
                client_to_add=(String) player3.Content;
                temp_img=selectedPiece;
            } else if(etalon4.IsMouseOver) {
                client_to_add=(String) player4.Content;
                temp_img=selectedPiece;
            }

            if(formatie) {
                int index=Pieces.GetInstance().getIndex(selectedPiece);
                selectedImages[n]=index;
                n++;
                if(n==3) {
                    formatie=false;
                    Client.GetInstance().WriteLine("FORMATION:"+selectedImages[0]+":"+selectedImages[1]+":"+selectedImages[2]+":"+Client.GetInstance().GetNickname()+":"+( PieceHandler._row1+1 )+":"+_roomName+":"+drawFromBoard);
                    drawFromBoard=false;
                }
            }
        }

        private void myimg_MouseMove(object sender, MouseEventArgs e) {
            Image selectedPiece=(Image) sender;
            if(selectedPiece.IsMouseCaptured) {
                Point mouseCurrent=e.GetPosition(null);
                double Left=mouseCurrent.X-mouseClick.X;
                double Top=mouseCurrent.Y-mouseClick.Y;
                mouseClick=e.GetPosition(null);
                selectedPiece.SetValue(Canvas.LeftProperty, canvasLeft+Left);
                selectedPiece.SetValue(Canvas.TopProperty, canvasTop+Top);
                canvasLeft=Canvas.GetLeft(selectedPiece);
                canvasTop=Canvas.GetTop(selectedPiece);
            }
        }

        private void myimg_MouseDown(object sender, MouseButtonEventArgs e) {
            Image selectedPiece=(Image) sender;
            mouseClick=e.GetPosition(null);
            canvasLeft=Canvas.GetLeft(selectedPiece);
            canvasTop=Canvas.GetTop(selectedPiece);
            selectedPiece.CaptureMouse();
        }

        private void DrawButtonClick(object sender, RoutedEventArgs e) {
            Client.GetInstance().WriteLine("DRAW:"+_roomName);
        }

        public void DrawPiece(string readData) {
            string[] pieces=readData.Split(':');
            this.Dispatcher.Invoke((Action) ( () => {
                foreach(string index in pieces) {
                    Canvas.SetLeft(Pieces.GetInstance().getImage(index), 0);
                    Canvas.SetTop(Pieces.GetInstance().getImage(index), 0);
                    MyTableCanvas.Children.Add(Pieces.GetInstance().getImage(index));
                    addImgListeners(Pieces.GetInstance().getImage(index));
                }
            } ));
        }

        public void RemovePieces(int grid, bool myTable) {
            String allPieces=null;
            foreach(Image piece in GetGridAt(grid).Children) {
                int index=Pieces.GetInstance().getIndex(piece);
                allPieces=allPieces+":"+index;
            }
            if(allPieces!=null ) {
                this.Dispatcher.Invoke((Action) ( () => {
                    foreach(string index in allPieces.Substring(1).Split(':')) {
                        RemoveImgListeners(Pieces.GetInstance().getImage(index));
                        RemoveFromMyTable(Pieces.GetInstance().getImage(index));
                        RemoveChildFromGrid(GetGridAt(grid), Pieces.GetInstance().getImage(index));
                    }
                } ));
                if(myTable)
                    DrawPiece(allPieces.Substring(1));
            }
        }

        private void FormationButtonClick(object sender, RoutedEventArgs e) {
            selectedImages=new int[3];
            formatie=true;
            n=0;
        }

        public string GetPlayerAt(int p) {
            string player2a=null;
            string player3a=null;
            string player4a=null;
            this.Dispatcher.Invoke((Action) ( () => {
                player2a=""+player2.Content;
                player3a=""+player3.Content;
                player4a=""+player4.Content;
            } ));
            switch(p) {
                case 2:
                    return player2a;
                case 3:
                    return player3a;
                case 4:
                    return player4a;
                default:
                    return "";
            }
        }

        public Grid GetGridAt(int p) {
            switch(p) {
                case 1:
                    return etalon1;
                case 2:
                    return etalon2;
                case 3:
                    return etalon3;
                case 4:
                    return etalon4;
                default:
                    return null;
            }
        }

        public void RemoveFromMyTable(Image local_image1) {
            MyTableCanvas.Children.Remove(local_image1);
        }

        public void AddRowToGrid(Grid etalon) {
            this.Dispatcher.Invoke((Action) ( () => {
                RowDefinition row=new RowDefinition();
                row.Height=GridLength.Auto;
                etalon.RowDefinitions.Add(row);
            } ));
        }

        public void AddChildToGrid(Grid etalon, Image local_image, int r, int c) {
            etalon.Children.Add(local_image);
            Grid.SetRow(local_image, r);
            Grid.SetColumn(local_image, c);
        }
        public void RemoveChildFromGrid(Grid etalon, Image local_image) {
            etalon.Children.Remove(local_image);
        }

        public void SetImageValue(Image local_image, DependencyProperty dependencyProperty, int p) {
            this.Dispatcher.Invoke((Action) ( () => {
                local_image.SetValue(dependencyProperty, p);
            } ));
        }

        public Image GetToAdd(Grid etalon, int c, int r) {
            Image img=null;
            this.Dispatcher.Invoke((Action) ( () => {
                img=etalon.Children.Cast<Image>().First(e => Grid.GetRow(e)==r&&Grid.GetColumn(e)==c);
            } ));
            return img;
        }

        public void MoveRow(Grid etalon, int r) {
            this.Dispatcher.Invoke((Action) ( () => {
                foreach(Image i in etalon.Children.Cast<Image>().Where(e => Grid.GetRow(e)==r)) {
                    Grid.SetColumn(i, Grid.GetColumn(i)+1);
                }
            } ));
        }

        public bool MyTableContains(UIElement element) {
            bool exists=false;
            this.Dispatcher.Invoke((Action) ( () => {
                exists=MyTableCanvas.Children.Contains(element);
            } ));
            return exists;
        }

        public void SetTurn(string readData) {
            String[] msg=readData.Split(':');
            this.Dispatcher.Invoke((Action) ( () => {
                if(msg[0].Equals(Client.GetInstance().GetNickname())) {
                    player1Turn.Content="X";
                    player2Turn.Content="";
                    player3Turn.Content="";
                    player4Turn.Content="";
                } else if(msg[0].Equals(player2.Content)) {
                    player1Turn.Content="";
                    player2Turn.Content="X";
                    player3Turn.Content="";
                    player4Turn.Content="";
                } else if(msg[0].Equals(player3.Content)) {
                    player1Turn.Content="";
                    player2Turn.Content="";
                    player3Turn.Content="X";
                    player4Turn.Content="";
                } else if(msg[0].Equals(player4.Content)) {
                    player1Turn.Content="";
                    player2Turn.Content="";
                    player3Turn.Content="";
                    player4Turn.Content="X";
                }
            } ));
        }

        public void Etalat(string readData) {
            this.Dispatcher.Invoke((Action) ( () => {
                etalat=true;
            } ));
        }

        public void MovePieceToBonus(Image local_image, Grid etalon, string nickname) {
            etalon.Children.Remove(local_image);
            if(nickname==player1.Content.ToString())
                stack1.Children.Add(local_image);
            else if(nickname==player2.Content.ToString())
                stack2.Children.Add(local_image);
            else if(nickname==player3.Content.ToString())
                stack3.Children.Add(local_image);
            else if(nickname==player4.Content.ToString())
                stack4.Children.Add(local_image);
        }

        public void EndGame(string readData) {
            string[] msg=readData.Split(',');
            string winner=msg[0].Split(':').ElementAt(0);
            string scores=msg[0].Substring(msg[0].IndexOf(':')+1);
            string users=msg[1];
            this.Dispatcher.Invoke((Action) ( () => {
                EndGameWindow endGame=new EndGameWindow(winner, scores, users, _roomName);
                endGame.Owner=this;
                endGame.ShowDialog();
            } ));
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            if(Client.Exists()) {
                if(!MainWindow.Exists()) {
                    Client.GetInstance().WriteLine("EXIT_FROM_GAME:"+_roomName);
                    Client.GetInstance().Close();
                }
            }
        }

        private void ButtonQuitGameClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
