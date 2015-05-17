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

namespace UserControls {
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RoomUC : UserControl {
        private string _roomName;
        static RoomUC _instance;

        public RoomUC() {
            InitializeComponent();
            _instance=this;
        }

        public RoomUC(string roomName) {
            InitializeComponent();
            _instance=this;
            _roomName=roomName;
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

        public string getRoomName(){
            return _roomName;
        }

        public static bool Exists() {
            if(_instance==null) {
                return false;
            } else
                return true;
        }

        private void ReadyButton(object sender, RoutedEventArgs e) {
            //Client.GetInstance().WriteLine("READY:"+_roomName);
        }

        public static RoomUC GetInstance() {
            return _instance;
        }

    }
}
