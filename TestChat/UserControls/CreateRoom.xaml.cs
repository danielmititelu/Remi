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
using System.Windows.Shapes;

namespace UserControls {
    /// <summary>
    /// Interaction logic for CreateRoom.xaml
    /// </summary>
    public partial class CreateRoom : Window {
        public CreateRoom() {
            InitializeComponent();
        }

        private void NewRoom(object sender, RoutedEventArgs e) {
            MainWindow.GetInstance().Switch(new RoomUC(roomName.Text));
            Client.GetInstance().WriteLine("CREATE_ROOM:"+roomName.Text);
            this.Close();
        }
    }
}
