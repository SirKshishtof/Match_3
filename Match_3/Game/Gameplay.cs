using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Match_3
{ 
    public class Gameplay
    {
        //private Element[,] elemMatrix = new Element[GameSettings.MatrixSizeX, GameSettings.MatrixSizeY];
        private List<Element>[] elemMatrix = new List<Element>[GameSettings.MatrixSizeX];
        private System.Windows.Forms.Timer gameTimer;
        List<Position>[] checedElem = new List<Position>[2];
        private Position? selectElem;
        private Position? trySwichElem;
        private int score;
        private int remainingTime;
        private bool isGameStart = false;


        public Element[,] ElemMatrix
        {
            set { elemMatrix = value; }
            get { return elemMatrix; }
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

            InitElementMatrix();
        }
        private void UpdateTimer(object? sender, EventArgs e) => remainingTime--;
        private void SetShift(Position SelectElemShift, Position elemTrySwichShift)
        {
            int selectX = selectElem.Value.x;
            int selectY = selectElem.Value.y;

            int swichX = trySwichElem.Value.x;
            int swichY = trySwichElem.Value.y;
            elemMatrix[selectX, selectY].ShiftPosition = SelectElemShift;
            elemMatrix[swichX, swichY].ShiftPosition = elemTrySwichShift;
            
        }
        private void SetShift()
        {
            int selectX = selectElem.Value.x;
            int selectY = selectElem.Value.y;

            int swichX = trySwichElem.Value.x;
            int swichY = trySwichElem.Value.y;
            Position bufferPos = elemMatrix[selectX, selectY].ShiftPosition;
            elemMatrix[selectX, selectY].ShiftPosition = elemMatrix[swichX, swichY].ShiftPosition;
            elemMatrix[swichX, swichY].ShiftPosition = bufferPos;
            //elemMatrix[selectX, selectY].ShiftPosition = -elemMatrix[selectX, selectY].ShiftPosition;
            //elemMatrix[swichX, swichY].ShiftPosition = -elemMatrix[swichX, swichY].ShiftPosition;
        }
        private void SwipeCoord()
        {
            int selectX = selectElem.Value.x;
            int selectY = selectElem.Value.y;

            int swichX = trySwichElem.Value.x;
            int swichY = trySwichElem.Value.y;

            elemMatrix[swichX, swichY].onPosition = false;
            elemMatrix[selectX, selectY].onPosition = false;

            SwipeElementInMatrix((Position)selectElem, (Position)trySwichElem);

            elemMatrix[swichX, swichY].EndPosition = elemMatrix[swichX, swichY].CurrentPosition;
            elemMatrix[selectX, selectY].EndPosition = elemMatrix[selectX, selectY].CurrentPosition;

            elemMatrix[swichX, swichY].CurrentPosition = elemMatrix[selectX, selectY].EndPosition;
            elemMatrix[selectX, selectY].CurrentPosition = elemMatrix[swichX, swichY].EndPosition;

            if (elemMatrix[selectX, selectY].StartPosition == null) elemMatrix[selectX, selectY].SetStartPosition();
            else elemMatrix[selectX, selectY].NullStartPosition();
            if (elemMatrix[swichX, swichY].StartPosition == null) elemMatrix[swichX, swichY].SetStartPosition();
            else elemMatrix[swichX, swichY].NullStartPosition();
        }

        private void SwipeCoord(Position movingElem, Position staticElem)
        {
            elemMatrix[staticElem.x, staticElem.y].onPosition = false;

            elemMatrix[staticElem.x, staticElem.y].CollorID = elemMatrix[movingElem.x, movingElem.y].CollorID;

            elemMatrix[staticElem.x, staticElem.y].EndPosition = elemMatrix[staticElem.x, staticElem.y].CurrentPosition;
            elemMatrix[staticElem.x, staticElem.y].CurrentPosition = elemMatrix[movingElem.x, movingElem.y].CurrentPosition;
        }
        private void InitElementMatrix()
        {
            Random random = new Random();
            for (int i = 0; i < elemMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < elemMatrix.GetLength(1); j++)
                {
                    random.Next(5);
                    elemMatrix[i, j] = new Element(new Position(i, j), random.Next(5));
                }
            }
            int overlap;
            do
            {
                overlap = 0;
                for (int i = 0; i < elemMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < elemMatrix.GetLength(1)-1; j++)
                    {
                        if (elemMatrix[i, j].CollorID == elemMatrix[i, j + 1].CollorID)
                        {
                            elemMatrix[i, j].CollorID = random.Next(5);
                            overlap++;
                        }
                    }
                }

                for (int j = 0; j < elemMatrix.GetLength(0); j++)
                {
                    for (int i = 0; i < elemMatrix.GetLength(1)-1; i++)
                    {
                        if (elemMatrix[i, j].CollorID == elemMatrix[i+1, j].CollorID)
                        {
                            elemMatrix[i, j].CollorID = random.Next(5);
                            overlap++;
                        }
                    }
                }

            } while (overlap != 0);
        }
        private bool CheckBonus(int x, int y)
        {
            int x0 = selectElem.Value.x;
            int y0 = selectElem.Value.y;

            if (checedElem[0].Count() > 1 && checedElem[1].Count() > 1)
            {
                elemMatrix[x0, y0] = new Bomb(elemMatrix[x0, y0]);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (checedElem[i].Count < 2) { checedElem[i].Clear(); }
                    else
                    {
                        elemMatrix[0, 0] = new Destroer(elemMatrix[x0, y0], Direction.Vertical);
                        switch (checedElem[i].Count())
                        {
                            case 3: return false;
                            case 4:
                                {
                                    if (i == 0) { elemMatrix[x0, y0] = new Destroer(elemMatrix[x0, y0], Direction.Vertical); }
                                    else { elemMatrix[x0, y0] = new Destroer(elemMatrix[x0, y0], Direction.Horizontal); }
                                    return true;
                                }
                            default: elemMatrix[x0, y0] = new Bomb(elemMatrix[x0, y0]); return true;
                        }
                    }
                }
            }
            return false;
        }
        private void DeleteElements(int x, int y)
        {
            elemMatrix[x, y].isDeleted = true;
            score += elemMatrix[x, y].Points;
            elemMatrix[x, y].foo = false;
            elemMatrix[x, y].StartPosition = null;

            if (checedElem[0].Count > 1)
            {
                foreach (var element in checedElem[0])
                {
                    elemMatrix[element.x, element.y].isDeleted = true;
                    elemMatrix[element.x, element.y].foo = false;
                    elemMatrix[element.x, element.y].StartPosition = null;
                    score += elemMatrix[element.x, element.y].Points;

                }
            }
            if (checedElem[1].Count > 1)
            {
                foreach (var element in checedElem[1])
                {
                    elemMatrix[element.x, element.y].isDeleted = true;
                    elemMatrix[element.x, element.y].foo = false;
                    score += elemMatrix[element.x, element.y].Points;
                    elemMatrix[element.x, element.y].StartPosition = null;
                }
            }

            //for (int i = 0; i < GameSettings.MatrixSizeX; i++)
            //{
            //    int index = 1;
            //    for (int j = GameSettings.MatrixSizeY-2; j > -1; j--)
            //    {
            //        if (elemMatrix[i, j].isDeleted)
            //        {
            //            while (j + index < GameSettings.MatrixSizeY)
            //            {
            //                if (elemMatrix[i, j + index].isDeleted)
            //                {
            //                    index++;
            //                }
            //                else break;
            //            }
            //            if (j + index == 8)
            //            {
            //                int hh = index;
            //            }
            //            SwipeCoord(new Position(i,j), new Position(i, j + index));
            //            elemMatrix[i, j + index].isDeleted = false;
            //            elemMatrix[i, j + index].onPosition = false;
            //        }
            //    }
            //}
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
        private void SwipeElementInMatrix(Position movingElem, Position staticElem)
        {
            int buf = elemMatrix[selectElem.Value.x, selectElem.Value.y].CollorID;
            elemMatrix[selectElem.Value.x, selectElem.Value.y].CollorID = elemMatrix[trySwichElem.Value.x, trySwichElem.Value.y].CollorID;
            elemMatrix[trySwichElem.Value.x, trySwichElem.Value.y].CollorID = buf;
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
        public bool CheckMatch()
        {
            Position[] posDirect = [Position.Up, Position.Left, Position.Down, Position.Right];
            
            int selectX = selectElem.Value.x;
            int selectY = selectElem.Value.y;

            int swichX;
            int swichY;

            for (int i = 0;i < 2;i++)
            {
                swichX = trySwichElem.Value.x + posDirect[i].x;
                swichY = trySwichElem.Value.y + posDirect[i].y;

                while (swichX >= 0 && swichX < GameSettings.MatrixSizeX && swichY >= 0 && swichY < GameSettings.MatrixSizeX) 
                {
                    if (elemMatrix[selectX, selectY].CollorID == elemMatrix[swichX, swichY].CollorID)
                    {
                        swichX += posDirect[i].x;
                        swichY += posDirect[i].y;
                        checedElem[i].Add(new Position(swichX, swichY));
                    }
                    else break;
                }

                swichX = trySwichElem.Value.x + posDirect[i+2].x;
                swichY = trySwichElem.Value.y + posDirect[i+2].y;

                while (swichX >= 0 && swichX < GameSettings.MatrixSizeX && swichY >= 0 && 0 < GameSettings.MatrixSizeX)
                {
                    if (elemMatrix[selectX, selectY].CollorID == elemMatrix[swichX, swichY].CollorID)
                    {
                        swichX += posDirect[i+2].x;
                        swichY += posDirect[i+2].y;
                        checedElem[i].Add(new Position(swichX, swichY));
                    }
                    else break;
                }
            }

            if (checedElem[0].Count() >= 2 || checedElem[1].Count() >= 2) return true;
            else
            {
                checedElem[0].Clear();
                checedElem[1].Clear();
                return false;
            }
        }
        public bool CheckMatch(Element element, int x0, int y0)
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
                    if (elemMatrix[x0, y0].CollorID == elemMatrix[x, y].CollorID && elemMatrix[x, y].onPosition)
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
                    if (elemMatrix[x0, y0].CollorID == elemMatrix[x, y].CollorID && elemMatrix[x, y].onPosition)
                    {
                        checedElem[i].Add(new Position(x, y));
                        x += posDirect[i + 2].x;
                        y += posDirect[i + 2].y;
                    }
                    else break;
                }
            }

            if (checedElem[0].Count() > 1 || checedElem[1].Count() > 1) return true;
            else
            {
                checedElem[0].Clear();
                checedElem[1].Clear();
                return false;
            }
        }

        public void СollapseElements(int x, int y)
        {
            //CheckBonus();
            DeleteElements(x, y);
        }
        public void StartTimer()
        {
            isGameStart = true;
            gameTimer.Start();
        }
        public void StopTimer() => gameTimer.Stop();
        
    }
}

//x0 = selectElem.Value.x;
//y0 = selectElem.Value.y;

//if (checedElem[0].Count() > 2 && checedElem[1].Count() > 2)
//{
//    itemMatrix[x0, y0] = new Bomb(itemMatrix[x0, y0]);
//    //бомба на пересечинии  
//}
//else
//{
//    for (int i = 0; i < 2; i++)
//    {
//        if (checedElem[i].Count < 2) { checedElem[i].Clear(); }
//        else
//        {
//            itemMatrix[0, 0] = new Destroer(itemMatrix[x0, y0], Direction.Vertical);
//            switch (checedElem[i].Count())
//            {
//                case 3: /*простое уничтожение элементов*/ break;
//                case 4:
//                    {
//                        if (i == 0) { itemMatrix[x0, y0] = new Destroer(itemMatrix[x0, y0], Direction.Vertical); }
//                        else { itemMatrix[x0, y0] = new Destroer(itemMatrix[x0, y0], Direction.Horizontal); }
//                    }
//                    break;
//                default: itemMatrix[x0, y0] = new Bomb(itemMatrix[x0, y0]); break;
//            }
//        }
//    }
//}
