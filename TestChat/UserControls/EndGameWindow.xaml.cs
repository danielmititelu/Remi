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
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class EndGameWindow : Window {
        private string _roomName;
        public EndGameWindow() {
            InitializeComponent();
        }

        public EndGameWindow(string winner, string scores, string users,string roomName) {
            InitializeComponent();
            _roomName=roomName;
            string[] user = users.Split(':'); 
            string[] scor = scores.Split(':');
            int i=0;
            foreach(var u in user.Zip(scor,Tuple.Create)){
                RowDefinition row=new RowDefinition();
                row.Height=GridLength.Auto;
                scoreBoard.RowDefinitions.Add(row);

                Label nicknameLabel=new Label();
                nicknameLabel.Content=u.Item1;
                scoreBoard.Children.Add(nicknameLabel);
                Grid.SetRow(nicknameLabel, i);
                Grid.SetColumn(nicknameLabel, 0);

                Label scoreLabel=new Label();
                scoreLabel.Content=u.Item2;
                scoreBoard.Children.Add(scoreLabel);
                Grid.SetRow(scoreLabel, i);
                Grid.SetColumn(scoreLabel, 1);
                i++;
            }    
        }

        private void PlayAgainButtonClick(object sender, RoutedEventArgs e) {
            new MainWindow(_roomName);
            this.Close();
            GameWindow.GetInstance().Close();
        }
    }
}
