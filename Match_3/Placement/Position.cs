using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Match_3
{
    public struct Position
    {
        public int x;
        public int y;

        private static Position up = new(0, -1);
        private static Position down = new(0, 1);
        private static Position left = new(-1, 0);
        private static Position right = new(1, 0);
        private static Position non = new(0, 0);

        public static Position Up => up;
        public static Position Down => down;
        public static Position Left => left;
        public static Position Right => right;
        public static Position Non => non;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static bool operator <(Position left, Position right) => left.x < right.x && left.y < right.y;
        public static bool operator >(Position left, Position right) => left.x > right.x && left.y > right.y;
        public static bool operator <=(Position left, Position right) => left.x <= right.x && left.y <= right.y;
        public static bool operator >=(Position left, Position right) => left.x >= right.x && left.y >= right.y;
        public static bool operator ==(Position left, Position right) => left.x == right.x && left.y == right.y;
        public static bool operator !=(Position left, Position right) => !(left == right);


        public static Position operator -(Position position) => new(-position.x, -position.y);
        public static Position operator -(Position left, Position right) => new(left.x - right.x, left.y - right.y);
        public static Position operator +(Position left, Position right) => new(left.x + right.x, left.y + right.y);
        public static Position operator *(int left, Position right) => new(left * right.x, left * right.y);


    }
}
