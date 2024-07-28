﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Match_3
{
    public class Drawing
    {
        public Gameplay gameplay;
        private readonly List<Bitmap> elementImage;
        private readonly List<Bitmap> bombImage;
        private readonly List<Bitmap> destroyerDownImage;
        private readonly List<Bitmap> destroyerUpImage;
        private readonly List<Bitmap> destroyerRightImage;
        private readonly List<Bitmap> destroyerLeftImage;
        private System.Windows.Forms.Timer frameTimer;


        private BufferedGraphics bufferedGraphics;
        Point matrixStart;
        private int elemSize;
        private int destroerSize;
        public int cellSize;
        private int gapBetweenCell;
        private int gap;

        private Bitmap[, ] arr;
        public BufferedGraphics BufferedGraphics
        {
            set
            {
                bufferedGraphics = value; 
            }
        }

        public int CellSize
        {
            get {  return cellSize; }
        }
        public Drawing(Gameplay gameplay)
        {
            elementImage = [] ;
            bombImage = [];
            destroyerDownImage = [];
            destroyerUpImage = [];
            destroyerRightImage = [];
            destroyerLeftImage = [];

            this.gameplay = gameplay;
            matrixStart = new Point(300, 10);
            frameTimer = new System.Windows.Forms.Timer();
            frameTimer.Interval = GameSettings.DrawInterval;
            elemSize = 100;
            destroerSize = 40;
            cellSize = 120;
            gapBetweenCell = 5;
            arr = new Bitmap[8, 8];
            gap = (cellSize - elemSize) / 2;

            for (int i = 0; i < gameplay.ElemMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < gameplay.ElemMatrix.GetLength(1); j++)
                {
                    gameplay.ElemMatrix[i, j].CurrentPositionX = gameplay.ElemMatrix[i, j].CurrentPositionX * cellSize + matrixStart.X + gap;
                    gameplay.ElemMatrix[i, j].CurrentPositionY = gameplay.ElemMatrix[i, j].CurrentPositionY * cellSize + matrixStart.Y + gap;
                    gameplay.ElemMatrix[i, j].EndPosition = gameplay.ElemMatrix[i, j].CurrentPosition;
                }
            }
        }

        public void InitBufferedGraphics(BufferedGraphics bufferedGraphics)
        {
            this.bufferedGraphics = bufferedGraphics;
        }

        public void DrawScoreAndTime(int score, int timer)
        {
            int x = 1300;
            int y = 30;
            Font FontNumShip = new Font("Arial", 12);
            SolidBrush brush = new SolidBrush(Color.Black);

            bufferedGraphics.Graphics.DrawString("Score\n"+score, new Font("Arial", 36), brush, x, y);
            bufferedGraphics.Graphics.DrawString(" Time\n   "+timer, new Font("Arial", 36), brush, x+350, y);
        }
        public int GetEdge_Y()
        {
            return (8 * cellSize + matrixStart.Y);
        }

        public int GetEdge_X()
        {
            return (8 * cellSize + matrixStart.X);
        }
        
        private Bitmap ResizeImage(string pash, Size size)
        {
            Bitmap atlas = new (pash);
            return new Bitmap(atlas, size) ;
        }

        public void LoadImage()
        {
            string pash = "..\\..\\..\\Images\\";
            
            for (int i = 0; i < 5; i++)
            {
                elementImage.Add(ResizeImage($"{pash}Element_{i}.png", new Size(elemSize, elemSize)));
                bombImage.Add(ResizeImage($"{pash}Bomb_{i}.png", new Size(elemSize, elemSize)));
                destroyerDownImage.Add(ResizeImage($"{pash}DestroyerDown_{i}.png", new Size(destroerSize, destroerSize)));
                destroyerUpImage.Add(ResizeImage($"{pash}DestroyerUp_{i}.png", new Size(destroerSize, destroerSize)));
                destroyerRightImage.Add(ResizeImage($"{pash}DestroyerRight_{i}.png", new Size(destroerSize, destroerSize)));
                destroyerLeftImage.Add(ResizeImage($"{pash}DestroyerLeft_{i}.png", new Size(destroerSize, destroerSize)));
            }
        }

        private void DrawElement(Element element)
        {
            Point point = new Point(element.CurrentPositionX, element.CurrentPositionY);
            switch (element)
            {
                case Bomb: bufferedGraphics.Graphics.DrawImage(bombImage[element.CollorID], point); break;
                case Destroer destroer:
                    {
                        bufferedGraphics.Graphics.DrawImage(elementImage[element.CollorID], point);
                        destroer = (Destroer)element;
                        int x = 0;
                        int y = 0;
                        switch (destroer.Direction)
                        {
                            case Direction.Horizontal:
                                {
                                    x = point.X;
                                    y = point.Y + elemSize / 2 - destroerSize / 2;
                                    bufferedGraphics.Graphics.DrawImage(destroyerLeftImage[element.CollorID], new Point(x, y));
                                    x = point.X + elemSize - destroerSize;
                                    bufferedGraphics.Graphics.DrawImage(destroyerRightImage[element.CollorID], new Point(x, y));
                                } break;
                            case Direction.Vertical:
                                {
                                    x = point.X + elemSize / 2 - destroerSize / 2; ;
                                    y = point.Y;
                                    bufferedGraphics.Graphics.DrawImage(destroyerUpImage[element.CollorID], new Point(x, y));
                                    y = point.Y + elemSize - destroerSize;
                                    bufferedGraphics.Graphics.DrawImage(destroyerDownImage[element.CollorID], new Point(x, y));
                                } break;
                                
                        }
                    } break;

                default: bufferedGraphics.Graphics.DrawImage(elementImage[element.CollorID], point); break;
            }
        }
       
        private void DrawElements()
        {
            for (int x = 0; x < gameplay.ElemMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < gameplay.ElemMatrix.GetLength(1); y++)
                {
                    if (gameplay.ElemMatrix[x, y].foo)
                    {
                        DrawElement(gameplay.ElemMatrix[x, y]);
                        if (gameplay.ElemMatrix[x, y].CurrentPosition == gameplay.ElemMatrix[x, y].EndPosition)
                        {
                            gameplay.ElemMatrix[x, y].onPosition = true;

                            if (gameplay.CheckMatch(gameplay.ElemMatrix[x, y], x, y))
                            {
                                gameplay.СollapseElements(x, y);
                            }
                            else
                            {
                                if (gameplay.TrySwichElem != null && x == gameplay.TrySwichElem.Value.x&& y == gameplay.TrySwichElem.Value.y) 
                                {
                                    if (gameplay.ElemMatrix[x, y].StartPosition != null)
                                    {
                                        if (gameplay.CheckMatch(gameplay.ElemMatrix[gameplay.SelectElement.Value.x,gameplay.SelectElement.Value.y], 
                                                                gameplay.SelectElement.Value.x, 
                                                                gameplay.SelectElement.Value.y))
                                        {
                                            gameplay.TrySwichElem = null;
                                        }
                                        else gameplay.SwipeElementCoord();
                                    }
                                }
                                
                            }

                        }
                        else
                        {
                            gameplay.ElemMatrix[x, y].ElementShift();
                        }
                    }
                }
            }
        }
        public void Draw()
        {
            bufferedGraphics.Graphics.Clear(Color.White);
            DrawScoreAndTime(gameplay.Score, gameplay.RemainingTime);
           
            DrawMatrix();
            DrawElements();
            bufferedGraphics.Render();
        }
        
        public void DrawMatrix()
        {      
            Random random = new Random(); 
            Rectangle rect = new Rectangle(0,0, cellSize, cellSize);
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    rect.Location = new Point(x * cellSize + matrixStart.X, y * cellSize + matrixStart.Y);
                    bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.LightGray), rect);
                    bufferedGraphics.Graphics.DrawRectangle(new Pen(Color.White, 5), rect);
                }
            }

            if (gameplay.SelectElement != null)
            {
                rect.Location = new Point(gameplay.SelectElement.Value.x * cellSize + matrixStart.X, gameplay.SelectElement.Value.y * cellSize + matrixStart.Y);
                bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.LightSeaGreen), rect);
                bufferedGraphics.Graphics.DrawRectangle(new Pen(Color.White, 5), rect);
            }
        }
        public Position GetPixelPos(int x, int y) => new Position(x * cellSize + matrixStart.X + gap, y * cellSize + matrixStart.Y + gap);
        
        public void Clean() { bufferedGraphics.Graphics.Clear(Color.White); bufferedGraphics.Render(); }
        public void StartTimer() => frameTimer.Start();
        public void StopTimer() => frameTimer.Stop();
        public void SetMetodInTick(EventHandler eventHandler) => frameTimer.Tick += eventHandler;
    }
}