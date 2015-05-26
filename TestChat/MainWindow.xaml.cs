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
using System.Windows.Shapes;
using UserControls;

namespace Game {
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public bool _inGame=false;
        static MainWindow _instance;

        public MainWindow() {
            InitializeComponent();
            Switch(new LoginUC());
            _instance=this;
        }

        public MainWindow(string roomName) {
            InitializeComponent();
            Switch(new RoomUC(roomName));
            _instance=this;
            this.Show();
            Client.GetInstance().WriteLine("REJOIN_ROOM:"+roomName);
        }


        public void Switch(UserControl content) {
            this.Content=content;
        }

        public static MainWindow GetInstance() {
            return _instance;
        }

        public static bool Exists() {
            if(_instance==null) {
                return false;
            } else
                return true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

            if(Client.Exists()) {
                if(!RoomUC.Exists()) {
                    Client.GetInstance().WriteLine("EXIT_FROM_CHAT:Am iesit din chat server");
                } else {
                    Client.GetInstance().WriteLine("EXIT_FROM_CHAT:"+RoomUC.GetInstance().getRoomName()+":"+_inGame);
                }

                if(!GameWindow.Exists())
                    Client.GetInstance().Close();
            }
        }

    }
}
