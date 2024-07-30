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
        private List<Arrow> arrows = new List<Arrow>();
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
        public List<Arrow> Arrows
        {
            set { arrows = value; }
            get { return arrows; }
        }
        public Position[,] PositionMatrix
        {
            set { positionMatrix = value; }
            get { return positionMatrix; }
        }
        public Position? SelectElem
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

            elemMatrix[selectX][selectY].ShiftPosition = -elemMatrix[selectX][selectY].ShiftPosition;
            elemMatrix[swichX][swichY].ShiftPosition = -elemMatrix[swichX][swichY].ShiftPosition;

        }
        private void SwipeCoord()
        {
            Position posS = (Position)selectElem;
            Position posT = (Position)trySwichElem;

            elemMatrix[posS.x][posS.y].OnPosition = false;
            elemMatrix[posT.x][posT.y].OnPosition = false;

            if (posS.x == posT.x)
            {
                if (posS.y < posT.y) { ElemMatrix[posS.x].Reverse(posS.y, 2); }
                else { ElemMatrix[posT.x].Reverse(posT.y, 2); }
            }
            else
            {
                ElemMatrix[posS.x].Insert(posS.y+1, ElemMatrix[posT.x][posT.y]);
                ElemMatrix[posT.x].Insert(posT.y+1, ElemMatrix[posS.x][posS.y]);

                ElemMatrix[posS.x].RemoveAt(posS.y);
                ElemMatrix[posT.x].RemoveAt(posT.y);
            }

            elemMatrix[posS.x][posS.y].EndPosition = positionMatrix[posS.x,posS.y];
            elemMatrix[posT.x][posT.y].EndPosition = positionMatrix[posT.x,posT.y];

            if (elemMatrix[posS.x][posS.y].StartPosition == null) elemMatrix[posS.x][posS.y].SetStartPosition();
            else elemMatrix[posS.x][posS.y].NullStartPosition();
            if (elemMatrix[posT.x][posT.y].StartPosition == null) elemMatrix[posT.x][posT.y].SetStartPosition();
            else elemMatrix[posT.x][posT.y].NullStartPosition();
        }
        private void InitElementMatrix()
        {
            for (int i = 0; i < GameSettings.MatrixSizeX; i++)
            {
                for (int j = 0; j < GameSettings.MatrixSizeY; j++)
                {
                    elemMatrix[i].Add(new Element(Random.Shared.Next(5)));
                }
            }
            int overlap;
            do
            {

                overlap = 0;
                for (int i = 0; i < GameSettings.MatrixSizeX; i++)
                {
                    for (int j = 0; j < GameSettings.MatrixSizeY - 1; j++)
                    {
                        if (elemMatrix[i][j].ColorID == elemMatrix[i][j + 1].ColorID)
                        {
                            elemMatrix[i][j].ColorID = Random.Shared.Next(5);
                            overlap++;
                        }
                    }
                }

                for (int j = 0; j < GameSettings.MatrixSizeX; j++)
                {
                    for (int i = 0; i < GameSettings.MatrixSizeY - 1; i++)
                    {
                        if (elemMatrix[i][j].ColorID == elemMatrix[i + 1][j].ColorID)
                        {
                            elemMatrix[i][j].ColorID = Random.Shared.Next(5);
                            overlap++;
                        }
                    }
                }

            } while (overlap != 0);

        }
        private bool CheckBonus(int x, int y)
        {
            if(elemMatrix[x][y] is not Destroer && elemMatrix[x][y] is not Bomb)
            {
                if (checedElem[0].Count() > 1 && checedElem[1].Count() > 1)
                {
                    elemMatrix[x][y] = new Bomb(elemMatrix[x][y]);
                    return false;
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (checedElem[i].Count > 2)
                        {
                            switch (checedElem[i].Count())
                            {
                                case 3:
                                    {
                                        Destroer destroer = new Destroer(elemMatrix[x][y]);

                                        if (i == 0)
                                        {
                                            destroer.Direction = Direction.Horizontal;
                                            destroer.Arrows[0] = new Arrow(Direction.Left);
                                            destroer.Arrows[1] = new Arrow(Direction.Right);
                                        }
                                        else
                                        {
                                            destroer.Direction = Direction.Vertical;
                                            destroer.Arrows[0] = new Arrow(Direction.Up);
                                            destroer.Arrows[1] = new Arrow(Direction.Down);
                                        }

                                        for (int j = 0; j < 2; j++)
                                        {
                                            destroer.Arrows[j].ColorId = destroer.ColorID;
                                            destroer.Arrows[j].SetSpeed();
                                        }

                                        elemMatrix[x][y] = destroer;
                                    }
                                    return false;
                                case > 3: elemMatrix[x][y] = new Bomb(elemMatrix[x][y]); return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        private void СollapseDestroer(int x, int y)
        {
            Destroer destroer = (Destroer)elemMatrix[x][y];

            for (int i = 0; i < 2; i++)
            {
                destroer.Arrows[i].SetPosition(elemMatrix[x][y].CurrentPosition);
                arrows.Add(destroer.Arrows[i]);
            }

            if (destroer.Direction == Direction.Horizontal)
            {
                for (int i = 0; i < GameSettings.MatrixSizeX; i++)
                {
                    if (elemMatrix[i][y].IsDeleted != true && i != x)
                    {
                        elemMatrix[i][y].IsDeleted = true;
                        СollapseBonus(i, y);
                    }
                }
            }
            else
            {
                for (int j = 0; j < GameSettings.MatrixSizeY; j++)
                {
                    if (elemMatrix[x][j].IsDeleted != true && j != y)
                    {
                        elemMatrix[x][j].IsDeleted = true;
                        СollapseBonus(x, j);
                    }
                }
            }
        }
        private void СollapseBomb(int x, int y)
        {
            for (int i = x - Bomb.Radius; i <= x + Bomb.Radius; i++)
            {
                for (int j = y - Bomb.Radius; j <= y + Bomb.Radius; j++)
                {
                    if (i > -1 && i < GameSettings.MatrixSizeX && j > -1 && j < GameSettings.MatrixSizeX)
                    {
                        if (elemMatrix[i][j].IsDeleted != true/* && !(i != x && j != y)*/)
                        {
                            elemMatrix[i][j].IsDeleted = true;
                            СollapseBonus(i, j);
                        }
                    }
                }
            }
        }
        private void СollapseBonus(int x, int y)
        {
            switch (elemMatrix[x][y])
            {
                case Bomb: СollapseBomb(x, y); break;
                case Destroer: СollapseDestroer(x, y); break;
            }
        }
        private void DeleteElements(int x, int y)
        {
            elemMatrix[x][y].IsDeleted = CheckBonus(x, y);//Если элемент становиться бонусом, он не удаляется

            if (checedElem[0].Count < 2) checedElem[0].Clear();
            if (checedElem[1].Count < 2) checedElem[1].Clear();
            if (elemMatrix[x][y].IsDeleted)
            {
                СollapseBonus(x, y);
            }

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < checedElem[i].Count(); j++)
                {
                    Position pos = checedElem[i][j];
                    elemMatrix[pos.x][pos.y].IsDeleted = true;
                    СollapseBonus(pos.x, pos.y);
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
        private void SpawnNewElements()
        {
            for (int i = 0; i < GameSettings.MatrixSizeX; i++)
            {
                int count = 1;
                while (elemMatrix[i].Count < 8)
                {
                    elemMatrix[i].Insert(0, new Element(Random.Shared.Next(5)));
                    elemMatrix[i][0].CurrentPosition = Position.GetPosition(positionMatrix[i,0], positionMatrix[i,1], count);
                    count++;
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
        private void SwipeElement()
        {
            SwipeCoord();
            SetShift();
            ResetSelection();
        }
        private bool CheckMatch(int x0, int y0)
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
                    if (elemMatrix[x0][y0].ColorID == elemMatrix[x][y].ColorID && elemMatrix[x][y].OnPosition)
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
                    if (elemMatrix[x0][y0].ColorID == elemMatrix[x][y].ColorID && elemMatrix[x][y].OnPosition)
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
        private void СollapseElements(int x, int y)
        {
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
        public void ResetGame()
        {
            for (int i = 0; i < GameSettings.MatrixSizeX; i++) elemMatrix[i].Clear();
            List<Position>[] checkedElem = new List<Position>[2];
            arrows.Clear();
            selectElem = null;
            trySwichElem = null;
            score = 0;
            remainingTime = GameSettings.TimeCount;
            isGameStart = false;
            InitElementMatrix();
            SetPositionOnElementsMatrix();
        }
        public void SetPositionOnElementsMatrix()
        {
            for (int i = 0; i < GameSettings.MatrixSizeX; i++)
            {
                for (int j = 0; j < GameSettings.MatrixSizeY; j++)
                {
                    elemMatrix[i][j].CurrentPosition = positionMatrix[i, j];
                    elemMatrix[i][j].EndPosition = elemMatrix[i][j].CurrentPosition;
                    elemMatrix[i][j].OnPosition = true;
                }
            }

        }
        public void SwipeElementCoord(Position SelectElemShift, Position elemTrySwichShift)
        {
            SetShift(SelectElemShift, elemTrySwichShift);
            SwipeCoord();
        }
        public void CheckElementsForCollapse(int x, int y)
        {
            int h = 0;
            if (elemMatrix[x][y].CurrentPosition == elemMatrix[x][y].EndPosition)
            {
                if (selectElem == new Position(x, y)&& trySwichElem!=null)
                {
                    h++;
                }
                elemMatrix[x][y].OnPosition = true;

                if (CheckMatch(x, y))
                {
                    СollapseElements(x, y);
                }
                else
                {
                    if (trySwichElem != null && x == trySwichElem.Value.x && y == trySwichElem.Value.y)
                    {
                        if (elemMatrix[x][y].StartPosition != null)
                        {
                            if (CheckMatch(selectElem.Value.x, selectElem.Value.y))
                            {
                                СollapseElements(selectElem.Value.x, selectElem.Value.y);
                                trySwichElem = null;
                            }
                            else SwipeElement();
                        }
                    }

                }

            }
            else
            {
                ElemMatrix[x][y].ElementShift();
            }
        }

    }
}
