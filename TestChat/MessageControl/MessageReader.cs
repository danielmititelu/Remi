using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handlers;
using Game;
using UserControls;

namespace MessageControl {
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
                if(message.Contains(':')) {
                    keyword=message.Substring(0, message.IndexOf(':'));
                    readData=message.Substring(message.IndexOf(':')+1, message.Length-message.IndexOf(':')-1);
                } else {
                    keyword=message;
                    readData="";
                }

                switch(keyword) {
                    case "MESSAGE_GAME":
                        GameWindow.GetInstance().SetText(readData);
                        break;
                    case "FORMATION":
                        PieceHandler.AddFormationToCanvas(readData);
                        break;
                    case "DRAW":
                        GameWindow.GetInstance().DrawPiece(readData);
                        break;
                    case "ADD_PIECE":
                        PieceHandler.AddPieceToFormation(readData, false);
                        break;
                    case "ADD_PIECE_ON_FIRST_COL":
                        PieceHandler.AddPieceToFormation(readData, true);
                        break;
                    case "DONT":
                        GameWindow.GetInstance().ErrorText(readData);
                        break;
                    case "PUT_PIECE_ON_BORD":
                        PieceHandler.PutPieceOnBoard(readData);
                        break;
                    case "MESSAGE_CHAT":
                        MainUC.GetInstance().SetText(readData);
                        break;
                    case "ALR":
                        //Login login=new Login ();
                        //login.Show ();
                        //this.Dispatcher.Invoke ((Action) (() => { this.Hide(); }));
                        break;
                    case "NEW_USER_IN_CHAT":
                        MainUC.GetInstance().AddPlayer(readData);
                        break;
                    case "NEW_USER_IN_GAME":
                        GameWindow.GetInstance().NewUser(readData);
                        break;
                    case "ALL_USERS_IN_ROOM":
                        RoomUC.GetInstance().AddPlayer(readData);
                        break;
                    case "NEW_ROOM":
                        MainUC.GetInstance().AddRoom(readData);
                        break;
                    case "MESSAGE_ROOM":
                        RoomUC.GetInstance().SetText(readData);
                        break;
                    case "READY":
                        RoomUC.GetInstance().SetReadyStatus(readData);
                        break;
                    case "START_GAME":
                        RoomUC.GetInstance().StartTimer();
                        break;
                    case "YOUR_TURN":
                        GameWindow.GetInstance().SetTurn(readData);
                        break;
                    case"REMOVE_PIECES":
                        PieceHandler.RemovePieces(readData);
                        break;
                    case"DRAW_FROM_BOARD":
                        PieceHandler.DrawFromBoard(readData);
                        break;
                    default:
                        GameWindow.GetInstance().ErrorText("Error 404:Keyword not found");
                        break;
                }
            }
        }
    }
}
