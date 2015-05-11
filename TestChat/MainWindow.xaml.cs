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

namespace TestChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        Client _client;
        String _ipAddress;
        String _nickname;
        public MainWindow () {
            InitializeComponent ();
        }
        public MainWindow (String ipAddress, String nickname) {
            InitializeComponent ();
            _ipAddress=ipAddress;
            _nickname=nickname;
            connect (ipAddress, nickname);
        }
        private void connect (String ipAddress, String nickname) {
            String ip=ipAddress.Trim ();
            _client=new Client (ip);
            Thread chatThread=new Thread (new ThreadStart (getMessage));

            chatThread.Start ();
            _client.WriteLine ("c:"+nickname);
        }

        private void getMessage () {
            String message=null;
            String[] dataReceived=null;
            String readData=null;

            while (_client.ClientConnected ()) {
                message=_client.ReadLine ();
                dataReceived=message.Split (':');
                readData=message.Substring (message.IndexOf (':')+1, message.Length-message.IndexOf (':')-1);
                switch (dataReceived[0]) {
                    case "MESSAGE":
                        this.Dispatcher.Invoke ((Action) (() => {
                            received.AppendText (readData+"\n");
                            received.ScrollToEnd ();
                        }));

                        break;
                    case "ALR":

                        //Login login=new Login ();
                        //login.Show ();
                        //this.Dispatcher.Invoke ((Action) (() => { this.Hide(); }));
                        break;
                    case "NEW_USER":
                        this.Dispatcher.Invoke ((Action) (() => { listBox1.Items.Clear (); }));
                        foreach (String user in dataReceived) {
                            if (user==dataReceived[0])
                                continue;

                            this.Dispatcher.Invoke ((Action) (() => { listBox1.Items.Add (user); }));
                        }
                        break;
                    default:
                        this.Dispatcher.Invoke ((Action) (() => { error.Content="Error 404:Keyword not found"; }));
                        break;
                }
            }
        }

        private void send_KeyDown (object sender, KeyEventArgs e) {
            if (e.Key==Key.Enter) {
                _client.WriteLine ("MESSAGE:"+send.Text);
                send.Text="";
            }
        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            Game game=new Game (_ipAddress, _nickname);
            game.Show ();
            this.Close ();
        }
        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            if (_client.ClientConnected ()) {
                _client.WriteLine ("EXIT:Am iesit din chat server");
                _client.Close ();
            }

        }

    }
}
