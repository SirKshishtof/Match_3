
namespace Match_3
{
    public class Arrow
    {
        private int colorId;
        private Position position;
        private Direction direction;
        private Position speed;


        public static Position LeftArrow;
        public static Position RightArrow;
        public static Position UpArrow;
        public static Position DownArrow;

        public Direction Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public Position Positions
        {
            get { return position; }
            set { position = value; }
        }
        public Position Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        public int ColorId 
        { 
            get { return colorId; }
            set { colorId = value; } 
        }
        public Arrow(Direction direction)
        {
            this.direction = direction;
        }
        public void SetPosition(Position position)
        {
            int x = 0;
            int y= 0;

            switch (direction)
            {
                case Direction.Left:
                    {
                        x = position.x + LeftArrow.x;
                        y = position.y + LeftArrow.y;
                    }
                    break;
                case Direction.Right:
                    {
                        x = position.x + RightArrow.x;
                        y = position.y + RightArrow.y;
                    }
                    break;
                case Direction.Up:
                    {
                        x = position.x + UpArrow.x;
                        y = position.y + UpArrow.y;
                    }
                    break;
                case Direction.Down:
                    {
                        x = position.x + DownArrow.x;
                        y = position.y + DownArrow.y;
                    }
                    break;
            }
            this.position = new Position(x, y);
        }
        public void SetSpeed()
        {
            int x;
            int y;

            switch (direction)
            {
                case Direction.Left: speed = Position.Left; break;
                case Direction.Right:speed = Position.Right; break;
                case Direction.Up: speed = Position.Up; break;
                case Direction.Down:speed = Position.Down; break;
            }
        }
        public void ArrowShift() => position += 30 * speed;
    }
}
