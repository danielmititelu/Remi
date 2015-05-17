using Game;
using MessageControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainUC : UserControl {

        static MainUC _instance;

        public MainUC() {
            InitializeComponent();
            _instance=this;
        }


        private void send_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key==Key.Enter) {
                Client.GetInstance().WriteLine("MESSAGE_CHAT:"+send.Text);
                send.Text="";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            new GameWindow();
            MainWindow.GetInstance().Close();
            Client.GetInstance().WriteLine("SWITCH_TO_GAME:"+Client.GetInstance().GetNickname());
        }

        public void SetText(string message) {
            this.Dispatcher.Invoke((Action) ( () => {
                received.AppendText(message+"\n");
                received.ScrollToEnd();
            } ));
        }

        public void AddPlayer(string message) {
            this.Dispatcher.Invoke((Action) ( () => {
                listBox1.Items.Clear();
                foreach(String user in message.Split(':')) {
                    listBox1.Items.Add(user);
                }
            } ));
        }

        public void AddRoom(string message) {
            this.Dispatcher.Invoke((Action) ( () => {
                rooms.Items.Clear();
                if(!message.Equals("")) {
                    foreach(String room in message.Split(':')) {
                        rooms.Items.Add(room);
                    }
                }
            } ));
        }

        private void NewRoom(object sender, RoutedEventArgs e) {
            CreateRoom newRoom=new CreateRoom();
            newRoom.ShowDialog();
            //MainWindow.GetInstance().Switch(new RoomUC(Client.GetInstance().GetNickname()));
            //Client.GetInstance().WriteLine("CREATE_ROOM:"+Client.GetInstance().GetNickname());
        }

        private void JoinRoom(object sender, RoutedEventArgs e) {
            if(!( rooms.SelectedItem==null )) {
                MainWindow.GetInstance().Switch(new RoomUC(""+rooms.SelectedItem));
                Client.GetInstance().WriteLine("JOIN_ROOM:"+rooms.SelectedItem);
            }
        }

        public static MainUC GetInstance() {
            return _instance;
        }


    }
}
