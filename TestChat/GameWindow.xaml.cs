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

namespace Game {
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class GameWindow : Window {
        private Point mouseClick;
        private double canvasLeft;
        private double canvasTop;
        bool formatie=false;
        String _nickname;
        int[] selectedImages=null;
        int n=0;
        public Image temp_img;
        String client_to_add;

        static GameWindow _instance;

        public GameWindow() {
            InitializeComponent();
        }

        public GameWindow(String ipAddress, String nickname) {
            InitializeComponent();
            _nickname=nickname;
            _instance=this;
            this.Show();
        }

        public static GameWindow GetInstance() {
            return _instance;
        }

        public static bool Exists() {
            if(_instance==null) {
                return false;
            }
            return true;
        }

        public void SetText(string message) {
            this.Dispatcher.Invoke((Action) ( () => {
                received.AppendText(message+"\n");
                received.ScrollToEnd();
            } ));
        }

        public void ErrorText(string message) {
            this.Dispatcher.Invoke((Action) ( () => { error.Content=message; } ));
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
                    if(users[i].Equals(_nickname)) {
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
                Client.GetInstance().WriteLine("MESSAGE_GAME:"+send.Text);
                send.Text="";
            }
        }

        private void addImgListeners(Image img) {
            img.PreviewMouseDown+=new MouseButtonEventHandler(myimg_MouseDown);
            img.PreviewMouseMove+=new MouseEventHandler(myimg_MouseMove);
            img.PreviewMouseUp+=new MouseButtonEventHandler(myimg_MouseUp);
            img.LostMouseCapture+=new MouseEventHandler(myimg_LostMouseCapture);

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

        public void etalon_MouseUp(object sender, MouseButtonEventArgs e) {
            if(temp_img!=null) {
                Image selectedPiece=(Image) sender;
                int c=Grid.GetColumn(selectedPiece);
                int r=Grid.GetRow(selectedPiece);
                int index=CanvasItems.Pieces.GetInstance().getIndex(temp_img);
                Client.GetInstance().WriteLine("ADD_PIECE:"+r+":"+index+":"+c+":"+client_to_add);
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
                int index=CanvasItems.Pieces.GetInstance().getIndex(selectedPiece);
                Client.GetInstance().WriteLine("PUT_PIECE_ON_BORD:"+index);
            }
            if(etalon1.IsMouseOver) {
                temp_img=selectedPiece;
                client_to_add=_nickname;
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
                int index=CanvasItems.Pieces.GetInstance().getIndex(selectedPiece);
                selectedImages[n]=index;
                n++;
                if(n==3) {
                    formatie=false;
                    Client.GetInstance().WriteLine("FOR:"+selectedImages[0]+":"+selectedImages[1]+":"+selectedImages[2]+":"+_nickname+":"+( PieceHandler._row1+1 ));
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

        private void Button_Click(object sender, RoutedEventArgs e) {
            Client.GetInstance().WriteLine("DRAW:Trage o piesa");
        }

        public void DrawCard(string readData) {
            string[] pieces=readData.Split(':');
            this.Dispatcher.Invoke((Action) ( () => {
                foreach(string index in pieces) {
                    Canvas.SetLeft(CanvasItems.Pieces.GetInstance().getImage(index), 0);
                    Canvas.SetTop(CanvasItems.Pieces.GetInstance().getImage(index), 0);
                    MyTableCanvas.Children.Add(CanvasItems.Pieces.GetInstance().getImage(index));
                    addImgListeners(CanvasItems.Pieces.GetInstance().getImage(index));
                }
            } ));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            selectedImages=new int[3];
            formatie=true;
            n=0;
        }


        public string GetNickName() {
            return _nickname;
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
                etalon.RowDefinitions.Add(new RowDefinition());
            } ));
        }

        public void AddChildToGrid(Grid etalon, Image local_image, int r, int c) {
            etalon.Children.Add(local_image);
            Grid.SetRow(local_image, r);
            Grid.SetColumn(local_image, c);
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if(Client.GetInstance().ClientConnected()) {
                Client.GetInstance().WriteLine("EXIT_FROM_GAME:Am iesit din joc");
                Client.GetInstance().Close();
            }
        }
    }
}
