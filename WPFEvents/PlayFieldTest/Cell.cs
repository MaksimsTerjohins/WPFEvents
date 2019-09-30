using System;
using System.Collections.Generic;
using System.Text;

namespace PlayFieldTest
{
    class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int F { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public Cell Parent;

        public Cell()
        {

        }

        public Cell(int inX, int inY)
        {
            X = inX;
            Y = inY;
        }

        public void printCell()
        { }

        public int calcH(int startX, int startY, int finishX, int finishY)
        {
            int xA = startX;
            int yA = startY;
            int xB = finishX;
            int yB = finishY;
            double H = Math.Sqrt((xB-xA)^2)+((yB-yA)^2);
            int resultH = Convert.ToInt32(H);
            return resultH;

        }
    }
}
