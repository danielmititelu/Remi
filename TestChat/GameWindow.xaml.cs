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
using Game.MessageControl;

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
        int _row1=-1;
        int _row2=-1;
        int _row3=-1;
        int _row4=-1;
        Image temp_img;
        String client_to_add;
        ImageLoader image=new ImageLoader();

        static GameWindow _instance;

        public GameWindow() {
            InitializeComponent();
        }

        public GameWindow(String ipAddress, String nickname) {
            InitializeComponent();
            //connect( ipAddress, nickname );
            _nickname=nickname;
            _instance=this;
            this.Show();
        }

        public static GameWindow GetInstance() {
            return _instance;
        }

        //private void connect( String ipAddress, String nickname ) {
        //    String ip=ipAddress.Trim();
        //    _client=new Client( ip );
        //    Thread gameThread=new Thread( new ThreadStart( MessageReader.getMessage ) ); //TODO: CHANGE TO LINQ EXPRESION
        //    gameThread.Name="GetMessage";
        //    gameThread.Start();
        //    _client.WriteLine( "g:"+nickname );
        //}

        public void SetText(string message) {
            this.Dispatcher.Invoke((Action) ( () => {
                received.AppendText(message+"\n");
                received.ScrollToEnd();
            } ));
        }

        public void ErrorText(string message) {
            this.Dispatcher.Invoke((Action) ( () => { error.Content=message; } ));
        }

        public void PutPieceOnBoard(string readData) {
            this.Dispatcher.Invoke((Action) ( () => {
                String[] mes=readData.Split(':');
                if(mes[0].Equals(_nickname)) {
                    removeImgListeners(image.getImage[Int32.Parse(mes[1])]);
                    canvas.Children.Remove(image.getImage[Int32.Parse(mes[1])]);
                }
                stack.Children.Add(image.getImage[Int32.Parse(mes[1])]);
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

        public void Formation(String readData) {
            this.Dispatcher.Invoke((Action) ( () => {
                String[] msg=readData.Split(':');
                Image local_image1=image.getImage[Int32.Parse(msg[1])];
                Image local_image2=image.getImage[Int32.Parse(msg[2])];
                Image local_image3=image.getImage[Int32.Parse(msg[3])];
                if(msg[0].Equals(_nickname)) {
                    removeImgListeners(local_image1);
                    removeImgListeners(local_image2);
                    removeImgListeners(local_image3);

                    canvas.Children.Remove(local_image1);
                    canvas.Children.Remove(local_image2);
                    canvas.Children.Remove(local_image3);
                    _row1++;
                    add_formation(etalon1, _row1, local_image1, local_image2, local_image3);
                } else if(msg[0].Equals(player2.Content)) {
                    _row2++;
                    add_formation(etalon2, _row2, local_image1, local_image2, local_image3);
                } else if(msg[0].Equals(player3.Content)) {
                    _row3++;
                    add_formation(etalon3, _row3, local_image1, local_image2, local_image3);
                } else if(msg[0].Equals(player4.Content)) {
                    _row4++;
                    add_formation(etalon4, _row4, local_image1, local_image2, local_image3);
                }
                addEtalonListener(local_image1);
                addEtalonListener(local_image3);
            } ));
        }

        private void add_formation(Grid etalon, int _row, Image local_image1, Image local_image2, Image local_image3) {
            etalon.RowDefinitions.Add(new RowDefinition());
            etalon.Children.Add(local_image1);
            etalon.Children.Add(local_image2);
            etalon.Children.Add(local_image3);

            local_image1.SetValue(Grid.ColumnProperty, 0);
            local_image1.SetValue(Grid.RowProperty, _row);
            local_image2.SetValue(Grid.ColumnProperty, 1);
            local_image2.SetValue(Grid.RowProperty, _row);
            local_image3.SetValue(Grid.ColumnProperty, 2);
            local_image3.SetValue(Grid.RowProperty, _row);
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
        private void removeImgListeners(Image img) {
            img.PreviewMouseDown-=myimg_MouseDown;
            img.PreviewMouseMove-=myimg_MouseMove;
            img.PreviewMouseUp-=myimg_MouseUp;
            img.LostMouseCapture-=myimg_LostMouseCapture;
        }
        private void addEtalonListener(Image img) {
            img.MouseUp+=etalon_MouseUp;
        }
        private void removeEtalonListener(Image img) {
            img.MouseUp-=etalon_MouseUp;
        }

        private void etalon_MouseUp(object sender, MouseButtonEventArgs e) {
            if(temp_img!=null) {
                Image element=(Image) sender;
                int c=Grid.GetColumn(element);
                int r=Grid.GetRow(element);
                int index=Array.IndexOf(image.getImage.ToArray(), temp_img);
                Client.GetInstance().WriteLine("ADD_PIECE:"+r+":"+index+":"+c+":"+client_to_add);
            }
        }

        public void AddPiece(String readData, bool firstRow) {
            this.Dispatcher.Invoke((Action) ( () => {
                String[] msg=readData.Split(':');//nickname:row:image.getImage:column
                int r=Int32.Parse(msg[1]);
                int c;
                if(firstRow) {
                    c=Int32.Parse(msg[3]);
                } else {
                    c=Int32.Parse(msg[3])+1;
                }

                Image local_image=image.getImage[Int32.Parse(msg[2])];

                if(msg[0].Equals(_nickname)) {
                    sub_add_piece(etalon1, local_image, r, c, firstRow);
                } else if(msg[0].Equals(player2.Content)) {
                    sub_add_piece(etalon2, local_image, r, c, firstRow);
                } else if(msg[0].Equals(player3.Content)) {
                    sub_add_piece(etalon3, local_image, r, c, firstRow);
                } else if(msg[0].Equals(player4.Content)) {
                    sub_add_piece(etalon4, local_image, r, c, firstRow);
                }
                addEtalonListener(local_image);
                temp_img=null;
            } ));
        }

        private void sub_add_piece(Grid etalon, Image local_image, int r, int c, bool first_row) {
            if(first_row) {
                Image local_image2=etalon.Children.Cast<Image>().First(e => Grid.GetRow(e)==r&&Grid.GetColumn(e)==c);
                removeEtalonListener(local_image2);
                IEnumerable<Image> imagesOnRow;
                imagesOnRow=etalon.Children.Cast<Image>().Where(e => Grid.GetRow(e)==r);
                foreach(Image i in imagesOnRow) {
                    Grid.SetColumn(i, Grid.GetColumn(i)+1);
                }
            } else {
                Image local_image2=etalon.Children.Cast<Image>().First(e => Grid.GetRow(e)==r&&Grid.GetColumn(e)==c-1);
                removeEtalonListener(local_image2);
            }
            if(canvas.Children.Contains(local_image)) {
                removeImgListeners(local_image);
                canvas.Children.Remove(local_image);
            }
            etalon.Children.Add(local_image);
            Grid.SetRow(local_image, r);
            Grid.SetColumn(local_image, c);
        }

        private void myimg_LostMouseCapture(object sender, MouseEventArgs e) {
            Image selected_image=(Image) sender;
            selected_image.ReleaseMouseCapture();
        }

        private void myimg_MouseUp(object sender, MouseButtonEventArgs e) {
            Image selected_image=(Image) sender;
            mouseClick=e.GetPosition(null);

            canvasLeft=Canvas.GetLeft(selected_image);
            canvasTop=Canvas.GetTop(selected_image);

            if(canvasLeft<0) {
                canvasLeft=0;
            }
            if(canvasTop<0) {
                canvasTop=0;
            }
            if(canvasLeft>canvas.ActualWidth) {
                canvasLeft=canvas.ActualWidth-selected_image.ActualWidth;
            }
            if(canvasTop>canvas.ActualHeight) {
                canvasTop=canvas.ActualHeight-selected_image.ActualHeight;
            }
            selected_image.SetValue(Canvas.LeftProperty, canvasLeft);
            selected_image.SetValue(Canvas.TopProperty, canvasTop);
            selected_image.ReleaseMouseCapture();

            if(stack.IsMouseOver) {
                int index=Array.IndexOf(image.getImage.ToArray(), selected_image);
                Client.GetInstance().WriteLine("PUT_PIECE_ON_BORD:"+index);
            }
            if(etalon1.IsMouseOver) {
                temp_img=selected_image;
                client_to_add=_nickname;
            } else if(etalon2.IsMouseOver) {
                temp_img=selected_image;
                client_to_add=(String) player2.Content;
            } else if(etalon3.IsMouseOver) {
                client_to_add=(String) player3.Content;
                temp_img=selected_image;
            } else if(etalon4.IsMouseOver) {
                client_to_add=(String) player4.Content;
                temp_img=selected_image;
            }
            if(formatie) {
                int index=Array.IndexOf(image.getImage.ToArray(), selected_image);
                selectedImages[n]=index;
                n++;
                if(n==3) {
                    formatie=false;
                    Client.GetInstance().WriteLine("FOR:"+selectedImages[0]+":"+selectedImages[1]+":"+selectedImages[2]+":"+_nickname+":"+( _row1+1 ));
                }
            }
        }

        private void myimg_MouseMove(object sender, MouseEventArgs e) {
            Image selected_image=(Image) sender;
            if(selected_image.IsMouseCaptured) {
                Point mouseCurrent=e.GetPosition(null);
                double Left=mouseCurrent.X-mouseClick.X;
                double Top=mouseCurrent.Y-mouseClick.Y;
                mouseClick=e.GetPosition(null);
                selected_image.SetValue(Canvas.LeftProperty, canvasLeft+Left);
                selected_image.SetValue(Canvas.TopProperty, canvasTop+Top);
                canvasLeft=Canvas.GetLeft(selected_image);
                canvasTop=Canvas.GetTop(selected_image);
            }
        }

        private void myimg_MouseDown(object sender, MouseButtonEventArgs e) {
            Image selected_image=(Image) sender;
            mouseClick=e.GetPosition(null);
            canvasLeft=Canvas.GetLeft(selected_image);
            canvasTop=Canvas.GetTop(selected_image);
            selected_image.CaptureMouse();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Client.GetInstance().WriteLine("DRAW:Trage o piesa");
        }

        public void DrawCard(int i) {
            this.Dispatcher.Invoke((Action) ( () => {
                Canvas.SetLeft(image.getImage[i], 0);
                Canvas.SetTop(image.getImage[i], 0);
                canvas.Children.Add(image.getImage[i]);
                addImgListeners(image.getImage[i]);
            } ));
        }
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            selectedImages=new int[3];
            formatie=true;
            n=0;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if(Client.GetInstance().ClientConnected()) {
                Client.GetInstance().WriteLine("EXIT_FROM_GAME:Am iesit din joc");
                Client.GetInstance().Close();
            }
        }
    }
}
