using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Vorcyc.PowerLibrary.Drawing
{
    public struct Point
    {
        public readonly static Point Empty;

        private int x;

        private int y;

        [Browsable(false)]
        public bool IsEmpty {
            get
            {
                if (this.x != 0) {
                    return false;
                }
                return this.y == 0;
            }
        }

        public int X {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public int Y {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        static Point()
        {
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(Size sz)
        {
            this.x = sz.Width;
            this.y = sz.Height;
        }

        public Point(int dw)
        {
            this.x = (short)Point.LOWORD(dw);
            this.y = (short)Point.HIWORD(dw);
        }

        public static Point Add(Point pt, Size sz)
        {
            return new Point(pt.X + sz.Width, pt.Y + sz.Height);
        }

        public static Point Ceiling(PointF value)
        {
            return new Point((int)Math.Ceiling((double)value.X), (int)Math.Ceiling((double)value.Y));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point)) {
                return false;
            }
            Point point = (Point)obj;
            if (point.X != this.X) {
                return false;
            }
            return point.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return this.x ^ this.y;
        }

        private static int HIWORD(int n)
        {
            return n >> 16 & 65535;
        }

        private static int LOWORD(int n)
        {
            return n & 65535;
        }

        public void Offset(int dx, int dy)
        {
            this.X = this.X + dx;
            this.Y = this.Y + dy;
        }

        public void Offset(Point p)
        {
            this.Offset(p.X, p.Y);
        }

        public static Point operator +(Point pt, Size sz)
        {
            return Point.Add(pt, sz);
        }

        public static bool operator ==(Point left, Point right)
        {
            if (left.X != right.X) {
                return false;
            }
            return left.Y == right.Y;
        }

        public static explicit operator Size(Point p)
        {
            return new Size(p.X, p.Y);
        }

        public static implicit operator PointF(Point p)
        {
            return new PointF((float)p.X, (float)p.Y);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

        public static Point operator -(Point pt, Size sz)
        {
            return Point.Subtract(pt, sz);
        }

        public static Point Round(PointF value)
        {
            return new Point((int)Math.Round((double)value.X), (int)Math.Round((double)value.Y));
        }

        public static Point Subtract(Point pt, Size sz)
        {
            return new Point(pt.X - sz.Width, pt.Y - sz.Height);
        }

        public override string ToString()
        {
            string[] str = new string[] { "{X=", null, null, null, null };
            int x = this.X;
            str[1] = x.ToString(CultureInfo.CurrentCulture);
            str[2] = ",Y=";
            x = this.Y;
            str[3] = x.ToString(CultureInfo.CurrentCulture);
            str[4] = "}";
            return string.Concat(str);
        }

        public static Point Truncate(PointF value)
        {
            return new Point((int)value.X, (int)value.Y);
        }
    }
}
