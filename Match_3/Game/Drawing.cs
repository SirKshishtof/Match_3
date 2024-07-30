
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
        private Point matrixStart;
        private int elemSize;
        private int destroerSize;
        private int cellSize;
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

            for (int i = 0; i < GameSettings.MatrixSizeX; i++)
            {
                for (int j = 0; j < GameSettings.MatrixSizeY; j++)
                {
                    gameplay.PositionMatrix[i,j].x = i * cellSize + matrixStart.X + gap;
                    gameplay.PositionMatrix[i,j].y = j * cellSize + matrixStart.Y + gap;
                }
            }
            gameplay.SetPositionOnElementsMatrix();

            Arrow.LeftArrow = new Position(0, cellSize / 2 - destroerSize / 2);
            Arrow.RightArrow = new Position(cellSize-destroerSize, cellSize / 2 - destroerSize / 2);
            Arrow.UpArrow = new Position(cellSize / 2 - destroerSize / 2,0);
            Arrow.DownArrow = new Position(cellSize / 2 - destroerSize / 2, cellSize - destroerSize);
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
                case Bomb: bufferedGraphics.Graphics.DrawImage(bombImage[element.ColorID], point); break;
                case Destroer destroer:
                    {
                        bufferedGraphics.Graphics.DrawImage(elementImage[element.ColorID], point);
                        destroer = (Destroer)element;
                        int x = 0;
                        int y = 0;
                        switch (destroer.Direction)
                        {
                            case Direction.Horizontal:
                                {
                                    x = point.X;
                                    y = point.Y + elemSize / 2 - destroerSize / 2;
                                    bufferedGraphics.Graphics.DrawImage(destroyerLeftImage[element.ColorID], new Point(x, y));
                                    x = point.X + elemSize - destroerSize;
                                    bufferedGraphics.Graphics.DrawImage(destroyerRightImage[element.ColorID], new Point(x, y));
                                } break;
                            case Direction.Vertical:
                                {
                                    x = point.X + elemSize / 2 - destroerSize / 2; ;
                                    y = point.Y;
                                    bufferedGraphics.Graphics.DrawImage(destroyerUpImage[element.ColorID], new Point(x, y));
                                    y = point.Y + elemSize - destroerSize;
                                    bufferedGraphics.Graphics.DrawImage(destroyerDownImage[element.ColorID], new Point(x, y));
                                } break;
                                
                        }
                    } break;

                default: bufferedGraphics.Graphics.DrawImage(elementImage[element.ColorID], point); break;
            }
        }
       
        private void DrawElements()
        {
            for (int x = 0; x < GameSettings.MatrixSizeX; x++)
            {
                for (int y = 0; y < GameSettings.MatrixSizeY; y++)
                {
                    DrawElement(gameplay.ElemMatrix[x][y]);
                    gameplay.CheckElementsForCollapse(x, y);
                }
            }
        }
        public void Draw()
        {
            bufferedGraphics.Graphics.Clear(Color.White);
            DrawScoreAndTime(gameplay.Score, gameplay.RemainingTime);
            DrawMatrix();
            DrawElements();
            DrawArrows();
            bufferedGraphics.Render();
        }

        private void DrawArrows()
        {
            if (gameplay.Arrows.Count > 0) 
            {
                int index = 0;
                while (index< gameplay.Arrows.Count())
                {
                    gameplay.Arrows[index].ArrowShift();
                    Point point = new Point(gameplay.Arrows[index].Positions.x, gameplay.Arrows[index].Positions.y);
                    switch (gameplay.Arrows[index].Direction)
                    {
                        case Direction.Left:
                            {
                                bufferedGraphics.Graphics.DrawImage(destroyerLeftImage[gameplay.Arrows[index].ColorId], point);
                                if (gameplay.Arrows[index].Positions.x < matrixStart.X) { gameplay.Arrows.RemoveAt(index); }
                                else index++;
                            }
                            break;
                        case Direction.Right:
                            {
                                bufferedGraphics.Graphics.DrawImage(destroyerRightImage[gameplay.Arrows[index].ColorId], point);
                                if (gameplay.Arrows[index].Positions.x > GetEdge_X()) { gameplay.Arrows.RemoveAt(index); }
                                else index++;
                            }
                            break;
                        case Direction.Up:
                            {
                                bufferedGraphics.Graphics.DrawImage(destroyerUpImage[gameplay.Arrows[index].ColorId], point);
                                if (gameplay.Arrows[index].Positions.y < matrixStart.Y) { gameplay.Arrows.RemoveAt(index); }
                                else index++;
                            }
                            break;
                        case Direction.Down:
                            {
                                bufferedGraphics.Graphics.DrawImage(destroyerDownImage[gameplay.Arrows[index].ColorId], point);
                                if (gameplay.Arrows[index].Positions.y > GetEdge_Y()) { gameplay.Arrows.RemoveAt(index); }
                                else index++;
                            }
                            break;
                    }

                }
            }
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

            if (gameplay.SelectElem != null)
            {
                rect.Location = new Point(gameplay.SelectElem.Value.x * cellSize + matrixStart.X, gameplay.SelectElem.Value.y * cellSize + matrixStart.Y);
                bufferedGraphics.Graphics.FillRectangle(new SolidBrush(Color.LightSeaGreen), rect);
                bufferedGraphics.Graphics.DrawRectangle(new Pen(Color.White, 5), rect);
            }
        }
        
        public void Clean() { bufferedGraphics.Graphics.Clear(Color.White); bufferedGraphics.Render(); }
        public void StartTimer() => frameTimer.Start();
        public void SetMetodInTick(EventHandler eventHandler) => frameTimer.Tick += eventHandler;
    }
}
