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

namespace TestChat {
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window {
        private Point mouseClick;
        private double canvasLeft;
        private double canvasTop;
        public Client _client;
        bool formatie=false;
        String _nickname;
        int[] selectedImages=null;
        String[] formations=new String[100];
        int n=0;
        int _row1=-1;
        int _row2=-1;
        int _row3=-1;
        int _row4=-1;
        Image temp_img;
        String client_to_add;
        ImageLoader image=new ImageLoader ();

        public Game () {
            InitializeComponent ();
        }

        public Game (String ipAddress, String nickname) {
            InitializeComponent ();
            connect (ipAddress, nickname);
            _nickname=nickname;
        }
        private void connect (String ipAddress, String nickname) {
            String ip=ipAddress.Trim ();
            _client=new Client (ip);
            Thread gameThread=new Thread (new ThreadStart (getMessage));
            gameThread.Name="GetMessage";
            gameThread.Start ();
            _client.WriteLine ("g:"+nickname);
        }
        private void getMessage () {
            String message=null;
            String keyword=null;
            String readData=null;

            try {
                while (_client.ClientConnected ()) {
                    message=_client.ReadLine ();
                    keyword=message.Substring (0, message.IndexOf (':'));
                    readData=message.Substring (message.IndexOf (':')+1, message.Length-message.IndexOf (':')-1);
                    switch (keyword) {
                        case "MESSAGE":
                            this.Dispatcher.Invoke ((Action) (() => { received.AppendText (readData+"\n"); }));
                            break;
                        case "FORMATION":
                            this.Dispatcher.Invoke ((Action) (() => { formation (readData); }));
                            break;
                        case "DRAW":
                            this.Dispatcher.Invoke ((Action) (() => { DrawCard (Int32.Parse (readData)); }));
                            break;
                        case "ADD_PIECE":
                            this.Dispatcher.Invoke ((Action) (() => { Add_piece (readData); }));
                            break;
                        case "ADD_PIECE_ON_FIRST_COL":
                            this.Dispatcher.Invoke ((Action) (() => { Add_piece_on_first_col (); }));
                            break;
                        case "DONT":
                            this.Dispatcher.Invoke ((Action) (() => { error.Content=readData; }));
                            break;
                        case "NEW_USER":
                            this.Dispatcher.Invoke ((Action) (() => { new_user (readData); }));
                            break;
                        case "PUT_PIECE_ON_BORD":
                            this.Dispatcher.Invoke ((Action) (() => { put_piece_on_board (readData); }));
                            break;
                        default:
                            this.Dispatcher.Invoke ((Action) (() => { error.Content="Error 404:Keyword not found"; }));
                            break;
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine (ex.ToString ());
            }
        }

        private void put_piece_on_board (string readData) {
            String[] mes=readData.Split (':');
            if (mes[0].Equals (_nickname)) {
                removeImgListeners (image.getImage[Int32.Parse (mes[1])]);
                canvas.Children.Remove (image.getImage[Int32.Parse (mes[1])]);
            }
            stack.Children.Add (image.getImage[Int32.Parse (mes[1])]);
        }

        private void new_user (string readData) {
            String[] users=readData.Split (':');
            player1.Content="[empty]";
            player2.Content="[empty]";
            player3.Content="[empty]";
            player4.Content="[empty]";
            int pos=0;
            for (int i=0; i<users.Count (); i++) {
                if (users[i].Equals (_nickname)) {
                    pos=i;
                }
            }
            for (int i=pos; i<users.Count (); i++) {

                if (i==pos) {
                    player1.Content=users[i];
                } else if (i==pos+1) {
                    player2.Content=users[i];
                } else if (i==pos+2) {
                    player3.Content=users[i];
                } else if (i==pos+3) {
                    player4.Content=users[i];
                }
            }
            for (int i=pos; i>=0; i--) {

                if (i==pos) {
                    player1.Content=users[i];
                } else if (i==pos-1) {
                    player4.Content=users[i];
                } else if (i==pos-2) {
                    player3.Content=users[i];
                } else if (i==pos-3) {
                    player2.Content=users[i];
                }

            }
        }

        private void formation (String readData) {
            String[] msg=readData.Split (':');
            Image local_image1=image.getImage[Int32.Parse (msg[1])];
            Image local_image2=image.getImage[Int32.Parse (msg[2])];
            Image local_image3=image.getImage[Int32.Parse (msg[3])];
            if (msg[0].Equals (_nickname)) {
                removeImgListeners (local_image1);
                removeImgListeners (local_image2);
                removeImgListeners (local_image3);

                canvas.Children.Remove (local_image1);
                canvas.Children.Remove (local_image2);
                canvas.Children.Remove (local_image3);
                _row1++;
                add_formation (etalon1, _row1, local_image1, local_image2, local_image3);
            } else if (msg[0].Equals (player2.Content)) {
                _row2++;
                add_formation (etalon2, _row2, local_image1, local_image2, local_image3);
            } else if (msg[0].Equals (player3.Content)) {
                _row3++;
                add_formation (etalon3, _row3, local_image1, local_image2, local_image3);
            } else if (msg[0].Equals (player4.Content)) {
                _row4++;
                add_formation (etalon4, _row4, local_image1, local_image2, local_image3);
            }
            addEtalonListener (local_image1);
            addEtalonListener (local_image3);
        }

        private void add_formation (Grid etalon, int _row, Image local_image1, Image local_image2, Image local_image3) {
            etalon.RowDefinitions.Add (new RowDefinition ());
            etalon.Children.Add (local_image1);
            etalon.Children.Add (local_image2);
            etalon.Children.Add (local_image3);

            local_image1.SetValue (Grid.ColumnProperty, 0);
            local_image1.SetValue (Grid.RowProperty, _row);
            local_image2.SetValue (Grid.ColumnProperty, 1);
            local_image2.SetValue (Grid.RowProperty, _row);
            local_image3.SetValue (Grid.ColumnProperty, 2);
            local_image3.SetValue (Grid.RowProperty, _row);
        }

        private void send_KeyDown (object sender, KeyEventArgs e) {
            if (e.Key==Key.Enter) {
                _client.WriteLine ("MESSAGE:"+send.Text);
                send.Text="";
            }
        }

        private void addImgListeners (Image img) {

            img.PreviewMouseDown+=new MouseButtonEventHandler (myimg_MouseDown);
            img.PreviewMouseMove+=new MouseEventHandler (myimg_MouseMove);
            img.PreviewMouseUp+=new MouseButtonEventHandler (myimg_MouseUp);
            //img.TextInput+=new TextCompositionEventHandler (myimg_TextInput);
            img.LostMouseCapture+=new MouseEventHandler (myimg_LostMouseCapture);

        }
        private void removeImgListeners (Image img) {

            img.PreviewMouseDown-=myimg_MouseDown;
            img.PreviewMouseMove-=myimg_MouseMove;
            img.PreviewMouseUp-=myimg_MouseUp;
            //img.TextInput-= myimg_TextInput;
            img.LostMouseCapture-=myimg_LostMouseCapture;
        }
        private void addEtalonListener (Image img) {
            img.MouseUp+=etalon_MouseUp;
        }
        private void removeEtalonListener (Image img) {
            img.MouseUp-=etalon_MouseUp;
        }

        private void etalon_MouseUp (object sender, MouseButtonEventArgs e) {
            if (temp_img!=null) {
                Image element=(Image) sender;
                int c=Grid.GetColumn (element);
                int r=Grid.GetRow (element);

                for (int i=0; i<image.getImage.Length; i++) {
                    if (temp_img.Equals (image.getImage[i])) {
                        _client.WriteLine ("ADD_PIECE:"+r+":"+i+":"+c+":"+client_to_add);
                    }
                }
            }
        }
        private void Add_piece (String readData) {
            String[] msg=readData.Split (':');//nickname:row:image.getImage:column
            int r=Int32.Parse (msg[1]);
            int c=Int32.Parse (msg[3]);
            Image local_image=image.getImage[Int32.Parse (msg[2])];

            if (msg[0].Equals (_nickname)) {
                sub_add_piece (etalon1, local_image, r, c);
            } else if (msg[0].Equals (player2.Content)) {
                sub_add_piece (etalon2, local_image, r, c);
            } else if (msg[0].Equals (player3.Content)) {
                sub_add_piece (etalon3, local_image, r, c);
            } else if (msg[0].Equals (player4.Content)) {
                sub_add_piece (etalon4, local_image, r, c);
            }
            addEtalonListener (local_image);
            temp_img=null;
        }

        private void sub_add_piece (Grid etalon, Image local_image, int r, int c) {
            Image local_image2=etalon.Children.Cast<Image> ().First (e => Grid.GetRow (e)==r&&Grid.GetColumn (e)==c);
            removeEtalonListener (local_image2);

            if (canvas.Children.Contains (local_image)) {
                removeImgListeners (local_image);
                canvas.Children.Remove (local_image);
            }
            etalon.Children.Add (local_image);
            Grid.SetRow (local_image, r);
            Grid.SetColumn (local_image, c+1);
        }
        private void Add_piece_on_first_col () {
            //canvas.Children.Remove (temp_img);
            //removeImgListeners (temp_img);

            //IEnumerable<Image> imagesOnRow;
            //imagesOnRow=etalon1.Children.Cast<Image> ().Where (e => Grid.GetRow (e)==r);

            //Image temp_img2=etalon1.Children.Cast<Image> ().First (e => Grid.GetRow (e)==r&&Grid.GetColumn (e)==c);
            //removeEtalonListener (temp_img2);
            //foreach (Image i in imagesOnRow) {
            //    Grid.SetColumn (i, Grid.GetColumn (i)+1);
            //}
            //etalon1.Children.Add (temp_img);
            //addEtalonListener (temp_img);
            //Grid.SetRow (temp_img, r);
            //Grid.SetColumn (temp_img, c);

            //temp_img=null;
        }

        private void myimg_LostMouseCapture (object sender, MouseEventArgs e) {
            ((Image) sender).ReleaseMouseCapture ();
        }

        private void myimg_TextInput (object sender, TextCompositionEventArgs e) {
            ((Image) sender).ReleaseMouseCapture ();
        }

        private void myimg_MouseUp (object sender, MouseButtonEventArgs e) {
            mouseClick=e.GetPosition (null);

            canvasLeft=Canvas.GetLeft ((Image) sender);
            canvasTop=Canvas.GetTop ((Image) sender);

            if (canvasLeft<0) {
                canvasLeft=0;
            }
            if (canvasTop<0) {
                canvasTop=0;
            }
            if (canvasLeft>canvas.ActualWidth) {
                canvasLeft=canvas.ActualWidth-((Image) sender).ActualWidth;
            }
            if (canvasTop>canvas.ActualHeight) {
                canvasTop=canvas.ActualHeight-((Image) sender).ActualHeight;
            }
            ((Image) sender).SetValue (Canvas.LeftProperty, canvasLeft);
            ((Image) sender).SetValue (Canvas.TopProperty, canvasTop);
            ((Image) sender).ReleaseMouseCapture ();

            if (stack.IsMouseOver) {
                for (int i=0; i<image.getImage.Length; i++) {
                    if (((Image) sender).Equals (image.getImage[i])) {
                        _client.WriteLine ("PUT_PIECE_ON_BORD:"+i);
                    }
                }
            }
            if (etalon1.IsMouseOver) {
                temp_img=((Image) sender);
                client_to_add=_nickname;
            } else if (etalon2.IsMouseOver) {
                temp_img=((Image) sender);
                client_to_add=(String) player2.Content;
            } else if (etalon3.IsMouseOver) {
                client_to_add=(String) player3.Content;
                temp_img=((Image) sender);
            } else if (etalon4.IsMouseOver) {
                client_to_add=(String) player4.Content;
                temp_img=((Image) sender);
            }
            if (formatie) {
                for (int i=0; i<image.getImage.Length; i++) {
                    if (((Image) sender).Equals (image.getImage[i])) {
                        selectedImages[n]=i;
                        n++;
                        if (n==3) {
                            formatie=false;
                            _client.WriteLine ("FOR:"+selectedImages[0]+":"+selectedImages[1]+":"+selectedImages[2]);
                        }
                    }
                }
            }
        }

        private void myimg_MouseMove (object sender, MouseEventArgs e) {
            if (((Image) sender).IsMouseCaptured) {
                Point mouseCurrent=e.GetPosition (null);
                double Left=mouseCurrent.X-mouseClick.X;
                double Top=mouseCurrent.Y-mouseClick.Y;
                mouseClick=e.GetPosition (null);
                ((Image) sender).SetValue (Canvas.LeftProperty, canvasLeft+Left);
                ((Image) sender).SetValue (Canvas.TopProperty, canvasTop+Top);
                canvasLeft=Canvas.GetLeft (((Image) sender));
                canvasTop=Canvas.GetTop (((Image) sender));
            }
        }

        private void myimg_MouseDown (object sender, MouseButtonEventArgs e) {
            mouseClick=e.GetPosition (null);
            canvasLeft=Canvas.GetLeft ((Image) sender);
            canvasTop=Canvas.GetTop ((Image) sender);
            ((Image) sender).CaptureMouse ();
        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            _client.WriteLine ("DRAW:Trage o piesa");
        }

        private void DrawCard (int i) {
            Canvas.SetLeft (image.getImage[i], 0);
            Canvas.SetTop (image.getImage[i], 0);
            canvas.Children.Add (image.getImage[i]);
            addImgListeners (image.getImage[i]);
        }
        private void Button_Click_1 (object sender, RoutedEventArgs e) {
            selectedImages=new int[3];
            formatie=true;
            n=0;
        }
        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            if (_client.ClientConnected ()) {
                _client.WriteLine ("EXIT:Am iesit din joc");
                _client.Close ();
            }
        }
    }
}
