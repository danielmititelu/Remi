using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace Game.MessageControl {
    class MessageReader {

        public static void getMessage() {
            String message=null;
            String keyword=null;
            String readData=null;

            while(Client.GetInstance().ClientConnected()) {
                message=Client.GetInstance().ReadLine();
                if("".Equals(message.Trim())) {
                    continue;
                }
                keyword=message.Substring(0, message.IndexOf(':'));
                readData=message.Substring(message.IndexOf(':')+1, message.Length-message.IndexOf(':')-1);

                switch(keyword) {
                    case "MESSAGE_GAME":
                        GameWindow.GetInstance().SetText(readData);
                        break;
                    case "FORMATION":
                        GameWindow.GetInstance().Formation(readData);
                        break;
                    case "DRAW":
                        GameWindow.GetInstance().DrawCard(Int32.Parse(readData));
                        break;
                    case "ADD_PIECE":
                        GameWindow.GetInstance().AddPiece(readData, false);
                        break;
                    case "ADD_PIECE_ON_FIRST_COL":
                        GameWindow.GetInstance().AddPiece(readData, true);
                        break;
                    case "DONT":
                        GameWindow.GetInstance().ErrorText(readData);
                        break;
                    case "PUT_PIECE_ON_BORD":
                        GameWindow.GetInstance().PutPieceOnBoard(readData);
                        break;
                    case "MESSAGE_CHAT":
                        MainWindow.GetInstance().SetText(readData);
                        break;
                    case "ALR":
                        //Login login=new Login ();
                        //login.Show ();
                        //this.Dispatcher.Invoke ((Action) (() => { this.Hide(); }));
                        break;
                    case "NEW_USER_IN_CHAT":
                        MainWindow.GetInstance().AddPlayer(readData);
                        break;
                    case "NEW_USER_IN_GAME":
                        GameWindow.GetInstance().NewUser(readData);
                        break;
                    default:
                        GameWindow.GetInstance().ErrorText("Error 404:Keyword not found");
                        break;
                }
            }
        }
    }
}
