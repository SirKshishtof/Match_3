using Match_3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Match_3
{
    public class Gameplay
    {
        private List<Element>[] elemMatrix = new List<Element>[GameSettings.MatrixSizeX];
        private Position[,] positionMatrix = new Position[GameSettings.MatrixSizeX, GameSettings.MatrixSizeY];
        private System.Windows.Forms.Timer gameTimer;
        private List<Position>[] checedElem = new List<Position>[2];
        private Position? selectElem;
        private Position? trySwichElem;
        private int score;
        private int remainingTime;
        private bool isGameStart = false;



        public List<Element>[] ElemMatrix
        {
            set { elemMatrix = value; }
            get { return elemMatrix; }
        }
        public Position[,] PositionMatrix
        {
            set { positionMatrix = value; }
            get { return positionMatrix; }
        }
        public Position? SelectElement
        {
            set { selectElem = value; }
            get { return selectElem; }
        }
        public Position? TrySwichElem
        {
            set { trySwichElem = value; }
            get { return trySwichElem; }
        }
        public bool IsGameStart
        {
            set { isGameStart = value; }
            get { return isGameStart; }
        }
        public int Score
        {
            set { score = value; }
            get { return score; } 
        }
        public int RemainingTime => remainingTime;
        public Gameplay() 
        {
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = GameSettings.GameInterval;
            gameTimer.Tick += new EventHandler(UpdateTimer);
            score = 0;
            remainingTime = GameSettings.TimeCount;
            Random random = new Random();
            checedElem[0] = new List<Position>();
            checedElem[1] = new List<Position>();

            for (int i = 0; i < GameSettings.MatrixSizeX; i++) elemMatrix[i] = new List<Element>();
            
            InitElementMatrix();
        }
        private void UpdateTimer(object? sender, EventArgs e) => remainingTime--;
        private void SetShift(Position SelectElemShift, Position elemTrySwichShift)
        {
            int selectX = selectElem.Value.x;
            int selectY = selectElem.Value.y;

            int swichX = trySwichElem.Value.x;
            int swichY = trySwichElem.Value.y;
            elemMatrix[selectX][ selectY].ShiftPosition = SelectElemShift;
            elemMatrix[swichX][swichY].ShiftPosition = elemTrySwichShift;
            
        }
        private void SetShift()
        {
            int selectX = selectElem.Value.x;
            int selectY = selectElem.Value.y;

            int swichX = trySwichElem.Value.x;
            int swichY = trySwichElem.Value.y;
            Position bufferPos = elemMatrix[selectX][ selectY].ShiftPosition;
            elemMatrix[selectX][ selectY].ShiftPosition = elemMatrix[swichX][swichY].ShiftPosition;
            elemMatrix[swichX][swichY].ShiftPosition = bufferPos;
            //elemMatrix[selectX, selectY].ShiftPosition = -elemMatrix[selectX, selectY].ShiftPosition;
            //elemMatrix[swichX][swichY].ShiftPosition = -elemMatrix[swichX][swichY].ShiftPosition;
        }
        private void SwipeCoord()
        {
            int selectX = selectElem.Value.x;
            int selectY = selectElem.Value.y;

            int swichX = trySwichElem.Value.x;
            int swichY = trySwichElem.Value.y;

            elemMatrix[swichX][swichY].OnPosition = false;
            elemMatrix[selectX][ selectY].OnPosition = false;

            SwipeElementInMatrix();

            elemMatrix[swichX][swichY].EndPosition = elemMatrix[swichX][swichY].CurrentPosition;
            elemMatrix[selectX][ selectY].EndPosition = elemMatrix[selectX][ selectY].CurrentPosition;

            elemMatrix[swichX][swichY].CurrentPosition = elemMatrix[selectX][ selectY].EndPosition;
            elemMatrix[selectX][ selectY].CurrentPosition = elemMatrix[swichX][swichY].EndPosition;

            if (elemMatrix[selectX][ selectY].StartPosition == null) elemMatrix[selectX][ selectY].SetStartPosition();
            else elemMatrix[selectX][ selectY].NullStartPosition();
            if (elemMatrix[swichX][swichY].StartPosition == null) elemMatrix[swichX][swichY].SetStartPosition();
            else elemMatrix[swichX][swichY].NullStartPosition();
        }
        private void SwipeCoord(Position movingElem, Position staticElem)
        {
            elemMatrix[staticElem.x][staticElem.y].OnPosition = false;

            elemMatrix[staticElem.x][staticElem.y].CollorID = elemMatrix[movingElem.x][movingElem.y].CollorID;

            elemMatrix[staticElem.x][staticElem.y].EndPosition = elemMatrix[staticElem.x][staticElem.y].CurrentPosition;
            elemMatrix[staticElem.x][staticElem.y].CurrentPosition = elemMatrix[movingElem.x][movingElem.y].CurrentPosition;
        }
        private void InitElementMatrix()
        {
            for (int i = 0; i < GameSettings.MatrixSizeX; i++)
            {
                for (int j = 0; j < GameSettings.MatrixSizeY; j++)
                {
                    ElemMatrix[i].Add(new Element(Random.Shared.Next(5)));
                }
            }
            int overlap;
            do
            {
                overlap = 0;
                for (int i = 0; i < GameSettings.MatrixSizeX; i++)
                {
                    for (int j = 0; j < GameSettings.MatrixSizeY-1; j++)
                    {
                        if (ElemMatrix[i][j].CollorID == elemMatrix[i][ j + 1].CollorID)
                        {
                            ElemMatrix[i][j].CollorID = Random.Shared.Next(5);
                            overlap++;
                        }
                    }
                }

                for (int j = 0; j < GameSettings.MatrixSizeX; j++)
                {
                    for (int i = 0; i < GameSettings.MatrixSizeY-1; i++)
                    {
                        if (ElemMatrix[i][j].CollorID == elemMatrix[i+1][ j].CollorID)
                        {
                            ElemMatrix[i][j].CollorID = Random.Shared.Next(5);
                            overlap++;
                        }
                    }
                }

            } while (overlap != 0);
        }
        private bool CheckBonus(int x, int y)
        {

            if (checedElem[0].Count() > 1 && checedElem[1].Count() > 1)
            {
                elemMatrix[x][ x] = new Bomb(elemMatrix[x][y]);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (checedElem[i].Count < 1) { checedElem[i].Clear(); }
                    else
                    {
                        elemMatrix[0][ 0] = new Destroer(elemMatrix[x][ y], Direction.Vertical);
                        switch (checedElem[i].Count())
                        {
                            case 3:
                                {
                                    if (i == 0) { elemMatrix[x][ y] = new Destroer(elemMatrix[x][ y], Direction.Vertical); }
                                    else { elemMatrix[x][ y] = new Destroer(elemMatrix[x][y], Direction.Horizontal); }
                                }break;
                            case >4: elemMatrix[x][y] = new Bomb(elemMatrix[x][y]); return true;
                        }
                    }
                }
            }
            return false;
        }
        private void DeleteElements(int x, int y)
        {
            if (elemMatrix[x][y] is not Destroer && elemMatrix[x][y] is not Bomb)
            {
                elemMatrix[x][y].IsDeleted = true;
            }

            elemMatrix[x][y].IsDeleted = true;

            if (checedElem[0].Count < 2) checedElem[0].Clear();
            if (checedElem[1].Count < 2) checedElem[1].Clear();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < checedElem[i].Count(); j++)
                {
                    Position pos = checedElem[i][j];
                    elemMatrix[pos.x][pos.y].IsDeleted = true ;
                }
            }

            for (int i = 0; i < GameSettings.MatrixSizeX; i++)
            {
                int index = 0;
                while (index < elemMatrix[i].Count())
                {
                    if (elemMatrix[i][index].IsDeleted)
                    {
                        score += elemMatrix[i][index].Points;
                        elemMatrix[i].RemoveAt(index); 
                    }
                    else index++;
                        
                }
            }

            checedElem[0].Clear();
            checedElem[1].Clear();
            selectElem = null;
            trySwichElem = null;
        }
        private void ResetSelection()
        {
            selectElem = null;
            trySwichElem = null;
        }
        private void SwipeElementInMatrix()
        {
            int buf = elemMatrix[selectElem.Value.x][selectElem.Value.y].CollorID;
            elemMatrix[selectElem.Value.x][selectElem.Value.y].CollorID = elemMatrix[trySwichElem.Value.x][trySwichElem.Value.y].CollorID;
            elemMatrix[trySwichElem.Value.x][trySwichElem.Value.y].CollorID = buf;
        }
        private void CheckLine(int x0, int y0, int index, bool secondPart)
        {
            int partLine;
            if (secondPart)partLine = 0;
            else partLine = 2;
            List<Position> posDirect = [Position.Up, Position.Left, Position.Down, Position.Right];

            int x = x0 + posDirect[index].x;
            int y = y0 + posDirect[index].y;
            while (x > -1 && x < GameSettings.MatrixSizeX && y > -1 && y < GameSettings.MatrixSizeX)
            {
                if (elemMatrix[x0][y0].CollorID == elemMatrix[x][y].CollorID && elemMatrix[x][y].OnPosition)
                {
                    checedElem[index].Add(new Position(x, y));
                    x += posDirect[index + partLine].x;
                    y += posDirect[index + partLine].y;
                }
                else break;
            }
        }
        private void SpawnNewElements()
        {
            for (int i = 0; i < GameSettings.MatrixSizeX; i++)
            {
                int count = 1;
                while (elemMatrix[i].Count < 8)
                {
                    elemMatrix[i].Insert(0, new Element(Random.Shared.Next(5)));
                    elemMatrix[i][0].CurrentPosition = Position.GetPosition(positionMatrix[i,0], positionMatrix[i,1], count);
                }
            }
        }
        private void UpdetePositions()
        {
            for (int i = 0; i < GameSettings.MatrixSizeX; i++)
            {
                for (int j = 0; j < GameSettings.MatrixSizeY; j++)
                {
                    elemMatrix[i][j].EndPosition = positionMatrix[i, j];
                    elemMatrix[i][j].StartPosition = null;
                    if (elemMatrix[i][j].EndPosition != elemMatrix[i][j].CurrentPosition)
                    {
                        elemMatrix[i][j].ShiftPosition = Position.Down;
                        elemMatrix[i][j].OnPosition = false;
                    }
                }
            }
        }

        public void SetPositionOnElementsMatrix()
        {
            for (int i = 0; i < GameSettings.MatrixSizeX; i++)
            {
                for (int j = 0; j < GameSettings.MatrixSizeY; j++)
                {
                    elemMatrix[i][j].CurrentPosition = positionMatrix[i, j];
                    elemMatrix[i][j].EndPosition = elemMatrix[i][j].CurrentPosition;
                }
            }

        }
        public void SwipeElementCoord(Position SelectElemShift, Position elemTrySwichShift)
        {
            SetShift(SelectElemShift, elemTrySwichShift);
            SwipeCoord();
        }
        public void SwipeElementCoord()
        {
            SwipeCoord();
            ResetSelection();
        }
        public bool CheckMatch(int x0, int y0)
        {
            List<Position> posDirect = [Position.Up, Position.Left, Position.Down, Position.Right];
            int x;
            int y;
            for (int i = 0; i < 2; i++)
            {
                x = x0 + posDirect[i].x;
                y = y0 + posDirect[i].y;
                while (x > -1 && x < GameSettings.MatrixSizeX && y > -1 && y < GameSettings.MatrixSizeX)
                {
                    if (elemMatrix[x0][y0].CollorID == elemMatrix[x][y].CollorID && elemMatrix[x][y].OnPosition)
                    {
                        checedElem[i].Add(new Position(x, y));
                        x += posDirect[i].x;
                        y += posDirect[i].y;
                    }
                    else break;
                }

                x = x0 + posDirect[i + 2].x;
                y = y0 + posDirect[i + 2].y;
                while (x > -1 && x < GameSettings.MatrixSizeX && y > -1 && y < GameSettings.MatrixSizeX)
                {
                    if (elemMatrix[x0][y0].CollorID == elemMatrix[x][y].CollorID && elemMatrix[x][y].OnPosition)
                    {
                        checedElem[i].Add(new Position(x, y));
                        x += posDirect[i + 2].x;
                        y += posDirect[i + 2].y;
                    }
                    else break;
                }
                //CheckLine(x0, y0,i,false);
                //CheckLine(x0, y0, i, true);
            }

            if (checedElem[0].Count() > 1 || checedElem[1].Count() > 1) return true;
            else
            {
                //selectElem = null;
                //trySwichElem = null;
                checedElem[0].Clear();
                checedElem[1].Clear();
                return false;
            }
        }
        public void СollapseElements(int x, int y)
        {
            //CheckBonus();
            DeleteElements(x, y);
            SpawnNewElements();
            UpdetePositions();
        }
        public void StartTimer()
        {
            isGameStart = true;
            gameTimer.Start();
        }
        public void StopTimer() => gameTimer.Stop();
        
    }
}


//x = x0 + posDirect[i].x;
//y = y0 + posDirect[i].y;
//while (x > -1 && x < GameSettings.MatrixSizeX && y > -1 && y < GameSettings.MatrixSizeX)
//{
//    if (elemMatrix[x0][y0].CollorID == elemMatrix[x][y].CollorID && elemMatrix[x][y].onPosition)
//    {
//        checedElem[i].Add(new Position(x, y));
//        x += posDirect[i].x;
//        y += posDirect[i].y;
//    }
//    else break;
//}

//x = x0 + posDirect[i + 2].x;
//y = y0 + posDirect[i + 2].y;
//while (x > -1 && x < GameSettings.MatrixSizeX && y > -1 && y < GameSettings.MatrixSizeX)
//{
//    if (elemMatrix[x0][y0].CollorID == elemMatrix[x][y].CollorID && elemMatrix[x][y].onPosition)
//    {
//        checedElem[i].Add(new Position(x, y));
//        x += posDirect[i + 2].x;
//        y += posDirect[i + 2].y;
//    }
//    else break;
//}

////x0 = selectElem.Value.x;
////y0 = selectElem.Value.y;

////if (checedElem[0].Count() > 2 && checedElem[1].Count() > 2)
////{
////    itemMatrix[x0, y0] = new Bomb(itemMatrix[x0, y0]);
////    //бомба на пересечинии  
////}
////else
////{
////    for (int i = 0; i < 2; i++)
////    {
////        if (checedElem[i].Count < 2) { checedElem[i].Clear(); }
////        else
////        {
////            itemMatrix[0, 0] = new Destroer(itemMatrix[x0, y0], Direction.Vertical);
////            switch (checedElem[i].Count())
////            {
////                case 3: /*простое уничтожение элементов*/ break;
////                case 4:
////                    {
////                        if (i == 0) { itemMatrix[x0, y0] = new Destroer(itemMatrix[x0, y0], Direction.Vertical); }
////                        else { itemMatrix[x0, y0] = new Destroer(itemMatrix[x0, y0], Direction.Horizontal); }
////                    }
////                    break;
////                default: itemMatrix[x0, y0] = new Bomb(itemMatrix[x0, y0]); break;
////            }
////        }
////    }
////}
