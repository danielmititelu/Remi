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
        double scale=2;
        Image[] image=new Image[106];
        int[] selectedImages=null;
        String[] formations=new String[100];
        int n=0;
        int row=-1;
        int c;
        int r;
        Image temp_img;

        CroppedBitmap[] objImg=new CroppedBitmap[65];

        public Game () {
            InitializeComponent ();
            CutImage ();
            LoadImage ();
        }

        public Game (String ipAddress, String nickname) {
            InitializeComponent ();
            CutImage ();
            LoadImage ();
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
                while (true) {
                    message=_client.ReadLine ();
                    keyword=message.Substring (0, message.IndexOf (':'));
                    readData=message.Substring (message.IndexOf (':')+1, message.Length-message.IndexOf (':')-1);
                    switch (keyword) {
                        case "MESSAGE":
                            //readData=dataReceived[1]+":"+dataReceived[2];
                            this.Dispatcher.Invoke ((Action) (() => { received.AppendText (readData+"\n"); }));
                            break;
                        case "FORMATION":
                            if (readData=="Numaratoare") {
                                this.Dispatcher.Invoke ((Action) (() => { Formation (image[selectedImages[0]], image[selectedImages[1]], image[selectedImages[2]]); }));
                            } else if (readData=="Pereche") {
                                this.Dispatcher.Invoke ((Action) (() => { Formation (image[selectedImages[0]], image[selectedImages[1]], image[selectedImages[2]]); }));
                            }
                            this.Dispatcher.Invoke ((Action) (() => { error.Content=readData; }));
                            break;
                        case "DRAW":
                            this.Dispatcher.Invoke ((Action) (() => { DrawCard (Int32.Parse (readData)); }));
                            break;
                        case "ADD_PIECE":
                            this.Dispatcher.Invoke ((Action) (() => { Add_piece (); }));
                            break;
                        case "ADD_PIECE_ON_FIRST_COL":
                            this.Dispatcher.Invoke ((Action) (() => { Add_piece_on_first_col (); }));
                            break;
                        case "DONT_ADD_PIECE":
                            this.Dispatcher.Invoke ((Action) (() => { error.Content="Nu se lipeste!"; }));
                            break;
                        case "NEW_USER":
                            this.Dispatcher.Invoke ((Action) (() => { new_user (readData); }));
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

        private void Formation (Image image1, Image image2, Image image3) {

            canvas.Children.Remove (image1);
            canvas.Children.Remove (image2);
            canvas.Children.Remove (image3);
            etalon.RowDefinitions.Add (new RowDefinition ());
            row++;
            image1.SetValue (Grid.ColumnProperty, 0);
            image1.SetValue (Grid.RowProperty, row);
            image2.SetValue (Grid.ColumnProperty, 1);
            image2.SetValue (Grid.RowProperty, row);
            image3.SetValue (Grid.ColumnProperty, 2);
            image3.SetValue (Grid.RowProperty, row);

            etalon.Children.Add (image1);
            etalon.Children.Add (image2);
            etalon.Children.Add (image3);

            removeImgListeners (image1);
            removeImgListeners (image2);
            removeImgListeners (image3);

            addEtalonListener (image1);
            //etalonListener (image2);
            addEtalonListener (image3);
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
                var element=(Image) sender;
                c=Grid.GetColumn (element);
                r=Grid.GetRow (element);

                for (int i=0; i<image.Length; i++) {
                    if (temp_img.Equals (image[i])) {
                        _client.WriteLine ("ADD_PIECE:"+r+":"+i+":"+c);
                    }
                }
            }
        }
        private void Add_piece () {
            canvas.Children.Remove (temp_img);
            removeImgListeners (temp_img);
            etalon.Children.Add (temp_img);
            Grid.SetRow (temp_img, r);
            Grid.SetColumn (temp_img, c+1);
            addEtalonListener (temp_img);
            Image temp_img2=etalon.Children.Cast<Image> ().First (e => Grid.GetRow (e)==r&&Grid.GetColumn (e)==c);
            removeEtalonListener (temp_img2);
            temp_img=null;
        }
        private void Add_piece_on_first_col () {
            canvas.Children.Remove (temp_img);
            removeImgListeners (temp_img);

            IEnumerable<Image> imagesOnRow;
            imagesOnRow=etalon.Children.Cast<Image> ().Where (e => Grid.GetRow (e)==r);

            Image temp_img2=etalon.Children.Cast<Image> ().First (e => Grid.GetRow (e)==r&&Grid.GetColumn (e)==c);
            removeEtalonListener (temp_img2);
            foreach (Image i in imagesOnRow) {
                Grid.SetColumn (i, Grid.GetColumn (i)+1);
            }
            etalon.Children.Add (temp_img);
            addEtalonListener (temp_img);
            Grid.SetRow (temp_img, r);
            Grid.SetColumn (temp_img, c);

            temp_img=null;
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
                canvas.Children.Remove (((Image) sender));
                stack.Children.Add (((Image) sender));
                removeImgListeners (((Image) sender));
            }
            if (etalon.IsMouseOver) {
                temp_img=((Image) sender);
            }
            if (formatie) {
                for (int i=0; i<image.Length; i++) {
                    if (((Image) sender).Equals (image[i])) {
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
        //delete meeeeee
        int a;

        private void DrawCard (int i) {
            a++;
            Canvas.SetLeft (image[a], 0);
            Canvas.SetTop (image[a], 0);
            canvas.Children.Add (image[a]);
            addImgListeners (image[a]);

            //row++;
            //image[i].SetValue (Grid.ColumnProperty, row);
            //image[i].SetValue (Grid.RowProperty, 0);
            //etalon.Children.Add (image[i]);
        }

        private void CutImage () {
            int count=0;

            BitmapImage src=new BitmapImage ();
            src.BeginInit ();
            src.UriSource=new Uri ("pack://application:,,,/Image/Tiles.png", UriKind.Absolute);
            src.CacheOption=BitmapCacheOption.OnLoad;
            src.EndInit ();

            for (int i=0; i<5; i++)
                for (int j=0; j<13; j++)
                    objImg[count++]=new CroppedBitmap (src, new Int32Rect (j*32, i*48, 32, 48));
        }

        private void LoadImage () {
            for (int i=0; i<106; i++) {
                if (i<53) {
                    image[i]=new Image ();
                    image[i].Source=objImg[i];
                    image[i].Width=objImg[i].Width*scale;
                    image[i].Height=objImg[i].Height*scale;
                } else {
                    image[i]=new Image ();
                    image[i].Source=objImg[i-53];
                    image[i].Width=objImg[i-53].Width*scale;
                    image[i].Height=objImg[i-53].Height*scale;
                }
            }
        }
        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            if (_client.ClientConnected ()) {
                _client.WriteLine ("EXIT:Am iesit din joc");
                _client.Close ();
            }
        }

        private void Button_Click_1 (object sender, RoutedEventArgs e) {
            selectedImages=new int[3];
            formatie=true;
            n=0;
        }

    }
}
