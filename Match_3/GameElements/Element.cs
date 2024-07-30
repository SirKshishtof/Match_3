using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Match_3
{
    public class Element
    {
        protected Position currentPosition;
        protected Position endPosition;
        protected Position? startPosition;
        protected Position shiftPsition;
        protected int colorID;
        protected int points;

        protected bool isDeleted;
        protected bool onPosition;
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
        public int ColorID
        {
            set { colorID = value; }
            get { return colorID; }
        }
        public int Points
        {
            set { points = value; }
            get { return points; }
        }
        public bool IsDeleted
        {
            set { isDeleted = value; }
            get { return isDeleted; }
        }
        public bool OnPosition
        {
            set { onPosition = value; }
            get { return onPosition; }
        }
        
        public Element()
        { }
        public Element(int collorID)
        {
            this.colorID = collorID;
            points = 10;
            onPosition = true;
            isDeleted = false;
        }
       
         
        public Element(Element element)
        {
            currentPosition = element.CurrentPosition;
            endPosition = element.EndPosition;
            startPosition = element.StartPosition;
            colorID = element.colorID;
            onPosition = element.OnPosition;
            isDeleted = element.IsDeleted;
            points = 10;
        }
        public void SetStartPosition() => startPosition = currentPosition;
        public void NullStartPosition() => startPosition = null;
        public void ElementShift() => currentPosition += 5*shiftPsition;
    }

    public class Bomb : Element
    {
        private static int radius =1;
        public static int Radius => radius;
        public Bomb(Element element):base(element) 
        {
            points = 100; 
        }
    }
    public class Destroer : Element
    {
        private Direction direction;
        Arrow[] arrows;
        public Arrow[] Arrows 
        {
            get { return arrows; }
            set { arrows = value; }
        }
        public Direction Direction
        {
            set { direction = value; }
            get { return direction; }
        }
        
        public Destroer(Element element) : base(element)
        {
            arrows = new Arrow[2];
            points = 100;
        }
    }
}
