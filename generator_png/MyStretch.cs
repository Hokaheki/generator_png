using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Library
{
    class MyStretch
    {
        public static int Length;
        public static Pen pen;
        Point A, B;
        public bool isNew;

        public int ax, ay, bx, by;
        public bool isHorisontal;

        public MyStretch(int x, int y, bool direction)
        {
            isNew = true;
            isHorisontal = direction;
            if (isHorisontal)
            {
                ax = x - Length / 2;
                bx = x + Length / 2;
                ay = y;
                by = y;
            }
            else
            {
                ax = x;
                bx = x;
                ay = y - Length / 2;
                by = y + Length / 2;
            }
            A = new Point(ax, ay);
            B = new Point(bx, by);
        }

        public bool Intersects(int x, int y)
        {
            if (ax == x && ay == y)
                return true;
            else if (bx == x && by == y)
                return true;
            else
                return false;
        }


        public void Draw(DrawingContext cont)
        {
            cont.DrawLine(pen, A, B);
        }

        internal MyStretch CreateA(List<MyStretch> others)
        {
            bool available = true;

            foreach(MyStretch other in others)
            {
                if (other != this && other.Intersects(ax,ay))
                    available = false;
            }

            if (available)
                return new MyStretch(ax, ay, !isHorisontal);

            return null;
        }

        internal MyStretch CreateB(List<MyStretch> others)
        {
            bool available = true;

            foreach (MyStretch other in others)
            {
                if (other != this && other.Intersects(bx, by))
                    available = false;
            }

            if (available)
                return new MyStretch(bx, by, !isHorisontal);

            return null;
        }
    }
}      