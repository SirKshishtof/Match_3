using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match_3
{
    public class Element:ICloneable
    {
        protected Position currentPosition;
        protected Position endPosition;
        protected Position? startPosition;
        protected Position shiftPsition;
        protected int collorID;
        protected int points;

        public bool foo;
        public bool isDeleted;
        public bool onPosition;
        public Position CurrentPosition
        {
            set { currentPosition= value; }
            get { return currentPosition; }
        }
        public int CurrentPositionX
        {
            set { currentPosition.x = value; }
            get { return currentPosition.x; }
        }
        public int CurrentPositionY
        {
            set { currentPosition.y = value; }
            get { return currentPosition.y; }
        }
        public Position EndPosition
        {
            set { endPosition = value; }
            get { return endPosition; }
        }
        public Position? StartPosition
        {
            set { startPosition = value; }
            get { return startPosition; }
        }
        public Position ShiftPosition
        {
            set { shiftPsition = value; }
            get { return shiftPsition; }
        }
        public int CollorID
        {
            set { collorID = value; }
            get { return collorID; }
        }
        public int Points
        {
            set { points = value; }
            get { return points; }
        }
        
        public Element()
        { }
        public Element(Position currentPosition, int collorID)
        {
            this.currentPosition = currentPosition;
            this.collorID = collorID;
            points = 10;
            foo = true;
            onPosition = true;
            isDeleted = false;
        }
        public Element(Element elem)
        {
            this.currentPosition = elem.currentPosition;
            this.collorID = elem.CollorID;
            points = 10;
        }
        public void SetStartPosition() => startPosition = currentPosition;
        public void NullStartPosition() => startPosition = null;
        public void ElementShift() => currentPosition += 5*shiftPsition;
        public object Clone() => new Element(new Position(currentPosition.x, currentPosition.y), collorID);
    }

    public class Bomb : Element
    {
        private int radius;
        public int Radius
        {
            set { radius = value; }
            get { return radius; }
        }
        public Bomb(Position position, int collorID) : base(position, collorID)
        {
            points = 100;
            radius = 1;
        }
        public Bomb(Element element)
        {
            currentPosition = element.CurrentPosition;
            collorID = element.CollorID;
            points = 100;
            radius = 1;
        }
    }
    public class Destroer : Element
    {
        private Direction direction;

        public Direction Direction
        {
            set { direction = value; }
            get { return direction; }
        }
        public Destroer(Position Position, Direction direction, int collorID) : base(Position, collorID)
        {
            points = 80;
            this.direction = direction;
        }

        public Destroer(Element element, Direction direction)
        {
            currentPosition = element.CurrentPosition;
            collorID = element.CollorID;
            points = 100;
            this.direction = direction;
        }
    }
}
