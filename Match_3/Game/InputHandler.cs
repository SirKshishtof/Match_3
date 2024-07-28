using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match_3
{
    
    internal class InputHandler
    {
        Drawing drawing;
        public InputHandler(Drawing drawing)
        {
            this.drawing = drawing;
        }
        public void HendelMouseClick(MouseEventArgs e)
        {
            if (drawing.gameplay.IsGameStart)
            {
                if (e.X > 300 && e.X < drawing.GetEdge_X() && e.Y > 10 && e.Y < drawing.GetEdge_Y())
                {
                    int x = ((e.X - 300) / drawing.CellSize);
                    int y = ((e.Y - 10) / drawing.CellSize);

                    if (drawing.gameplay.SelectElement is null) drawing.gameplay.SelectElement = new Position(x, y);
                    else
                    {
                        int x0 = drawing.gameplay.SelectElement.Value.x;
                        int y0 = drawing.gameplay.SelectElement.Value.y;

                        Position position = new Position(0,0);

                        switch (GetDirection(x, x0, y, y0))
                        {
                            case Direction.Up: position = Position.Up; break;
                            case Direction.Left: position = Position.Left; break;
                            case Direction.Down: position = Position.Down; break;
                            case Direction.Right: position = Position.Right; break;
                            case Direction.Non: position = Position.Non; break;
                        }
                        
                        if (position != Position.Non)
                        {
                            drawing.gameplay.TrySwichElem = new Position(x, y);
                            drawing.gameplay.SwipeElementCoord(-position,position);                            
                        }
                        else
                        {
                            drawing.gameplay.SelectElement = null;
                        }
                    }
                }
                else
                {
                    drawing.gameplay.SelectElement = null; 
                }
            }
        }
        private Direction GetDirection(int x, int x0, int y, int y0)
        {
            if (x == x0 && y - 1 == y0) { return Direction.Down; }
            if (x == x0 && y + 1 == y0) { return Direction.Up; }
            if (x - 1 == x0 && y == y0) { return Direction.Right; }
            if (x + 1 == x0 && y == y0) { return Direction.Left; }
            return Direction.Non;
        }


    }
}
