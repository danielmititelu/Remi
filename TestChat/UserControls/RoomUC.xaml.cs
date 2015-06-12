using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UserControls {
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RoomUC : UserControl {
        private string _roomName;
        static RoomUC _instance;
        DispatcherTimer timer;
        int time=3;
        public RoomUC() {
            InitializeComponent();
            _instance=this;
        }

        public RoomUC(string roomName) {
            InitializeComponent();
            _instance=this;
            _roomName=roomName;
            timer=new DispatcherTimer();
            timer.Interval=new TimeSpan(0, 0, 1);
            timer.Tick+=timerTick;
        }
        public RoomUC(string roomName,bool spectator) {
            InitializeComponent();
            _instance=this;
            _roomName=roomName;
            timer=new DispatcherTimer();
            timer.Interval=new TimeSpan(0, 0, 1);
            timer.Tick+=timerTick;
            ready.IsEnabled=false;
        }


        public void AddPlayer(string message) {
            this.Dispatcher.Invoke((Action) ( () => {
                playerList.Items.Clear();
                foreach(String user in message.Split(':')) {
                    playerList.Items.Add(user);
                }
            } ));
        }

        private void QuitRomm(object sender, RoutedEventArgs e) {
            MainWindow.GetInstance().Switch(new MainUC());
            Client.GetInstance().WriteLine("QUIT_ROOM:"+_roomName);
            _instance=null;
        }


        public void SetText(string message) {
            this.Dispatcher.Invoke((Action) ( () => {
                received.AppendText(message+"\n");
                received.ScrollToEnd();
            } ));
        }

        private void send_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key==Key.Enter) {
                Client.GetInstance().WriteLine("MESSAGE_ROOM:"+_roomName+":"+send.Text);
                send.Text="";
            }
        }

        public string getRoomName() {
            return _roomName;
        }

        public static bool Exists() {
            if(_instance==null) {
                return false;
            } else
                return true;
        }

        private void ReadyButton(object sender, RoutedEventArgs e) {
            Client.GetInstance().WriteLine("READY:"+_roomName);
        }

        public void StartTimer(){
            timer.Start();
        }

        private void timerTick(object sender,EventArgs e) {
            if(time>0) {
                received.Foreground=Brushes.Red;
                received.AppendText(time+"\n");
                time--;   
            } else {
                timer.Stop();
                new GameWindow(_roomName);
                MainWindow.GetInstance()._inGame=true;
                MainWindow.GetInstance().Close();
                Client.GetInstance().WriteLine("SWITCH_TO_GAME:"+_roomName);
            }
        }

        public void SetReadyStatus(string readData) {
            string[] msg=readData.Split(':');//nickname-ready

            int index=0;
            foreach(String user in playerList.Items) {
                if(user.Split('-').ElementAt(0)==msg[0]) {
                    index=playerList.Items.IndexOf(user);
                    break;
                }
            }
            this.Dispatcher.Invoke((Action) ( () => {
                playerList.Items.RemoveAt(index);
                playerList.Items.Insert(index, msg[0]+"-"+msg[1]);
            } ));
        }

        public static RoomUC GetInstance() {
            return _instance;
        }

    }
}
