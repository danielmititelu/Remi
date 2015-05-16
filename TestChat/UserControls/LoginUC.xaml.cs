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
    /// Interaction logic for LoginUC.xaml
    /// </summary>
    public partial class LoginUC : UserControl{
        public LoginUC() {
            InitializeComponent();
            ipAddress.Text="127.0.0.1";
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            MainWindow.GetInstance().Switch(new MainUC(ipAddress.Text, nickame.Text));
        }
    }
}
