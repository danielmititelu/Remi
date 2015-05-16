using Game.MessageControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Game {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        String _ipAddress;
        String _nickname;

        static MainWindow instance;

        public MainWindow() {
            InitializeComponent();
        }

        public MainWindow(String ipAddress, String nickname) {
            InitializeComponent();
            _ipAddress=ipAddress;
            _nickname=nickname;
            connect(ipAddress, nickname);

            instance=this;
        }

        private void connect(String ipAddress, String nickname) {
            String ip=ipAddress.Trim();

            new Client(ip);
            Client.GetInstance().SetNickName(_nickname);

            Thread messageReader=new Thread(() => MessageReader.getMessage());
            messageReader.SetApartmentState(ApartmentState.STA);
            messageReader.Name="MessageReader";

            messageReader.Start();

            Client.GetInstance().WriteLine(nickname);
        }

        private void send_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key==Key.Enter) {
                Client.GetInstance().WriteLine("MESSAGE_CHAT:"+send.Text);
                send.Text="";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            new GameWindow(_ipAddress, _nickname);
            this.Close();
            Client.GetInstance().WriteLine("SWITCH_TO_GAME:"+_nickname);
        }

        public void SetText(string message) {
            this.Dispatcher.Invoke((Action) ( () => {
                received.AppendText(message+"\n");
                received.ScrollToEnd();
            } ));
        }

        public void AddPlayer(string message) {
            this.Dispatcher.Invoke((Action) ( () => { listBox1.Items.Clear(); } ));
            foreach(String user in message.Split(':')) {
                if(user==Client.GetInstance().GetName())
                    continue;

                this.Dispatcher.Invoke((Action) ( () => { listBox1.Items.Add(user); } ));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if(Client.GetInstance().ClientConnected()) {
                Client.GetInstance().WriteLine("EXIT_FROM_CHAT:Am iesit din chat server");
            }
        }
        public static MainWindow GetInstance() {
            return instance;
        }
    }
}
