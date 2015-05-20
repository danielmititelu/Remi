using CanvasItems;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Handlers {
    class PieceHandler {

        public static int _row1=-1; // TODO: Grid.child.count
        public static int _row2=-1;
        public static int _row3=-1;
        public static int _row4=-1;

        public static void AddPieceToFormation(String readData, bool firstRow) {
            GameWindow.GetInstance().Dispatcher.Invoke((Action) ( () => {
                String[] msg=readData.Split(':');//nickname:row:image.getImage:column
                int r=Int32.Parse(msg[1]);
                int c;
                if(firstRow) {
                    c=Int32.Parse(msg[3]);
                } else {
                    c=Int32.Parse(msg[3])+1;
                }

                Image local_image=CanvasItems.Pieces.GetInstance().getImage(msg[2]);

                if(msg[0].Equals(Client.GetInstance().GetNickname())) {
                    AddPiece(GameWindow.GetInstance().GetGridAt(1), local_image, r, c, firstRow);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(2))) {
                    AddPiece(GameWindow.GetInstance().GetGridAt(2), local_image, r, c, firstRow);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(3))) {
                    AddPiece(GameWindow.GetInstance().GetGridAt(3), local_image, r, c, firstRow);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(4))) {
                    AddPiece(GameWindow.GetInstance().GetGridAt(4), local_image, r, c, firstRow);
                }
                GameWindow.GetInstance().AddEtalonListener(local_image);
                GameWindow.GetInstance().temp_img=null;
            } ));
        }

        private static void AddPiece(Grid etalon, Image local_image, int r, int c, bool first_row) {
            if(first_row) {
                Image local_image2=GameWindow.GetInstance().GetToAdd(etalon, c, r);
                GameWindow.GetInstance().RemoveEtalonListener(local_image2);
                GameWindow.GetInstance().MoveRow(etalon, r);
            } else {
                Image local_image2=GameWindow.GetInstance().GetToAdd(etalon, c-1, r);
                GameWindow.GetInstance().RemoveEtalonListener(local_image2);
            }
            if(GameWindow.GetInstance().MyTableContains(local_image)) {
                GameWindow.GetInstance().RemoveImgListeners(local_image);
                GameWindow.GetInstance().RemoveFromMyTable(local_image);
            }
            GameWindow.GetInstance().AddChildToGrid(etalon, local_image, r, c);
        }

        public static void RemovePieces(string readData) {
            GameWindow.GetInstance().Dispatcher.Invoke((Action) ( () => {
                String[] msg=readData.Split(':');

                if(msg[0].Equals(Client.GetInstance().GetNickname())) {
                    GameWindow.GetInstance().RemovePieces(1, true);
                    _row1=-1;
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(2))) {
                    _row2=-1;
                    GameWindow.GetInstance().RemovePieces(2, false);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(3))) {
                    _row3=-1;
                    GameWindow.GetInstance().RemovePieces(3, false);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(4))) {
                    _row4=-1;
                    GameWindow.GetInstance().RemovePieces(4, false);
                }
            } ));
        }

        public static void AddFormationToCanvas(String readData) {
            GameWindow.GetInstance().Dispatcher.Invoke((Action) ( () => {
                String[] msg=readData.Split(':');
                Image local_image1=CanvasItems.Pieces.GetInstance().getImage(msg[1]);
                Image local_image2=CanvasItems.Pieces.GetInstance().getImage(msg[2]);
                Image local_image3=CanvasItems.Pieces.GetInstance().getImage(msg[3]);
                if(msg[0].Equals(Client.GetInstance().GetNickname())) {
                    GameWindow.GetInstance().RemoveImgListeners(local_image1);
                    GameWindow.GetInstance().RemoveImgListeners(local_image2);
                    GameWindow.GetInstance().RemoveImgListeners(local_image3);

                    GameWindow.GetInstance().RemoveFromMyTable(local_image1);
                    GameWindow.GetInstance().RemoveFromMyTable(local_image2);
                    GameWindow.GetInstance().RemoveFromMyTable(local_image3);
                    _row1++;
                    AddFormation(GameWindow.GetInstance().GetGridAt(1), _row1, local_image1, local_image2, local_image3);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(2))) {
                    _row2++;
                    AddFormation(GameWindow.GetInstance().GetGridAt(2), _row2, local_image1, local_image2, local_image3);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(3))) {
                    _row3++;
                    AddFormation(GameWindow.GetInstance().GetGridAt(3), _row3, local_image1, local_image2, local_image3);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(4))) {
                    _row4++;
                    AddFormation(GameWindow.GetInstance().GetGridAt(4), _row4, local_image1, local_image2, local_image3);
                }
                GameWindow.GetInstance().AddEtalonListener(local_image1);
                GameWindow.GetInstance().AddEtalonListener(local_image3);
            } ));
        }

        private static void AddFormation(Grid etalon, int _row, Image local_image1, Image local_image2, Image local_image3) {
            GameWindow.GetInstance().AddRowToGrid(etalon);
            GameWindow.GetInstance().AddChildToGrid(etalon, local_image1, _row, 0);
            GameWindow.GetInstance().AddChildToGrid(etalon, local_image2, _row, 1);
            GameWindow.GetInstance().AddChildToGrid(etalon, local_image3, _row, 2);
        }

        public static void PutPieceOnBoard(string readData) {
            GameWindow.GetInstance().Dispatcher.Invoke((Action) ( () => {
                String[] mes=readData.Split(':');
                Image piece=CanvasItems.Pieces.GetInstance().getImage(mes[1]);
                if(mes[0].Equals(Client.GetInstance().GetNickname())) {
                    GameWindow.GetInstance().RemoveImgListeners(piece);
                    GameWindow.GetInstance().RemoveFromMyTable(piece);
                }
                GameWindow.GetInstance().StackCanvas.Children.Add(piece);
                GameWindow.GetInstance().AddBoardListener(piece);
            } ));
        }
        public static void DrawFromBoard(string readData) {
            GameWindow.GetInstance().Dispatcher.Invoke((Action) ( () => {
                String[] mes=readData.Split(':');
                String allpieces=null;
                foreach(String i in mes) {
                    if(i.Equals(mes[0]))
                        continue;
                    allpieces=allpieces+":"+i;
                    Image piece=CanvasItems.Pieces.GetInstance().getImage(i);
                    GameWindow.GetInstance().StackCanvas.Children.Remove(piece);
                    GameWindow.GetInstance().RemoveBoardListener(piece);
                }
                if(mes[0].Equals(Client.GetInstance().GetNickname())) {
                    GameWindow.GetInstance().DrawPiece(allpieces.Substring(1));
                }
            } ));
        }

    }
}
