﻿using CanvasItems;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Handlers {
    class PieceHandler {

        public static int _row1=-1;
        public static int _row2=-1;
        public static int _row3=-1;
        public static int _row4=-1;

        public static void AddPieceToFormation(String readData, bool firstRow) {
            GameWindow.GetInstance().Dispatcher.Invoke((Action) ( () => {
                String[] msg=readData.Split(':');//clientToAdd:row:imageIndex:column:piecesToTake:nickname
                int r=Int32.Parse(msg[1]);
                string piecesToTake=msg[4];
                int c;
                if(firstRow)
                    c=Int32.Parse(msg[3]);
                else
                    c=Int32.Parse(msg[3])+1;

                Image local_image=CanvasItems.Pieces.GetInstance().getImage(msg[2]);

                if(msg[0].Equals(Client.GetInstance().GetNickname())) {
                    AddPiece(GameWindow.GetInstance().GetGridAt(1), local_image, r, c, firstRow, piecesToTake, GameWindow.GetInstance().GetPlayerAt(1), msg[5]);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(2))) {
                    AddPiece(GameWindow.GetInstance().GetGridAt(2), local_image, r, c, firstRow, piecesToTake, GameWindow.GetInstance().GetPlayerAt(2), msg[5]);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(3))) {
                    AddPiece(GameWindow.GetInstance().GetGridAt(3), local_image, r, c, firstRow, piecesToTake, GameWindow.GetInstance().GetPlayerAt(3), msg[5]);
                } else if(msg[0].Equals(GameWindow.GetInstance().GetPlayerAt(4))) {
                    AddPiece(GameWindow.GetInstance().GetGridAt(4), local_image, r, c, firstRow, piecesToTake, GameWindow.GetInstance().GetPlayerAt(4), msg[5]);
                }
                GameWindow.GetInstance().AddEtalonListener(local_image);
                GameWindow.GetInstance().temp_img=null;
            } ));
        }

        private static void AddPiece(Grid etalon, Image pieceToAdd, int r, int c, bool first_row, string piecesToTake, string playerToAdd, string nickname) {
            bool exceptionalCase=false;
            switch(piecesToTake) {
                case "0":
                    if(first_row) {
                        Image pieceOnBoard=GameWindow.GetInstance().GetToAdd(etalon, c, r);
                        GameWindow.GetInstance().RemoveEtalonListener(pieceOnBoard);
                        GameWindow.GetInstance().MoveRow(etalon, r);
                    } else {
                        Image pieceOnBoard=GameWindow.GetInstance().GetToAdd(etalon, c-1, r);
                        GameWindow.GetInstance().RemoveEtalonListener(pieceOnBoard);
                    }
                    break;
                case "1":
                    if(first_row) {
                        Image pieceOnBoard=GameWindow.GetInstance().GetToAdd(etalon, c, r);
                        GameWindow.GetInstance().RemoveEtalonListener(pieceOnBoard);
                        GameWindow.GetInstance().MovePieceToBonus(pieceOnBoard, etalon, nickname);
                    } else {
                        Image pieceOnBoard=GameWindow.GetInstance().GetToAdd(etalon, c-1, r);
                        GameWindow.GetInstance().RemoveEtalonListener(pieceOnBoard);
                        GameWindow.GetInstance().MovePieceToBonus(pieceOnBoard, etalon, nickname);
                        c=c-1;
                    }
                    break;
                case "2":
                    Image pieceOnBoard1=GameWindow.GetInstance().GetToAdd(etalon, c-1, r);
                    Image pieceOnBoard2=GameWindow.GetInstance().GetToAdd(etalon, c-2, r);
                    GameWindow.GetInstance().RemoveEtalonListener(pieceOnBoard1);
                    GameWindow.GetInstance().RemoveEtalonListener(pieceOnBoard2);
                    GameWindow.GetInstance().MovePieceToBonus(pieceOnBoard1, etalon, nickname);
                    GameWindow.GetInstance().MovePieceToBonus(pieceOnBoard2, etalon, nickname);
                    c=c-2;
                    break;
                case "3":
                    if(GameWindow.GetInstance().MyTableContains(pieceToAdd)) {
                        GameWindow.GetInstance().RemoveImgListeners(pieceToAdd);
                        GameWindow.GetInstance().RemoveFromMyTable(pieceToAdd);
                    }
                    GameWindow.GetInstance().MovePieceToBonus(pieceToAdd, etalon, nickname);
                    exceptionalCase=true;
                    break;
            }
            if(GameWindow.GetInstance().MyTableContains(pieceToAdd)) {
                GameWindow.GetInstance().RemoveImgListeners(pieceToAdd);
                GameWindow.GetInstance().RemoveFromMyTable(pieceToAdd);
            }
            if(!exceptionalCase)
                GameWindow.GetInstance().AddChildToGrid(etalon, pieceToAdd, r, c);
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
