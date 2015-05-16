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

namespace Game {
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginWindow : Window {
        public LoginWindow() {
            InitializeComponent();
            ipAddress.Text="127.0.0.1";
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            MainWindow mainWindow=new MainWindow(ipAddress.Text, nickame.Text);
            mainWindow.Show();
            this.Close();
        }
    }
}
