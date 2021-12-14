using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Vorcyc.PowerLibrary.Drawing
{
    public struct PointF
    {
        public readonly static PointF Empty;

        private float x;

        private float y;

        [Browsable(false)]
        public bool IsEmpty {
            get
            {
                if (this.x != 0f) {
                    return false;
                }
                return this.y == 0f;
            }
        }

        public float X {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public float Y {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        static PointF()
        {
        }

        public PointF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static PointF Add(PointF pt, Size sz)
        {
            return new PointF(pt.X + (float)sz.Width, pt.Y + (float)sz.Height);
        }

        public static PointF Add(PointF pt, SizeF sz)
        {
            return new PointF(pt.X + sz.Width, pt.Y + sz.Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PointF)) {
                return false;
            }
            PointF pointF = (PointF)obj;
            if (pointF.X != this.X || pointF.Y != this.Y) {
                return false;
            }
            return pointF.GetType().Equals(this.GetType());
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        public static PointF operator +(PointF pt, Size sz)
        {
            return PointF.Add(pt, sz);
        }

        public static PointF operator +(PointF pt, SizeF sz)
        {
            return PointF.Add(pt, sz);
        }

        public static bool operator ==(PointF left, PointF right)
        {
            if (left.X != right.X) {
                return false;
            }
            return left.Y == right.Y;
        }

        public static bool operator !=(PointF left, PointF right)
        {
            return !(left == right);
        }

        public static PointF operator -(PointF pt, Size sz)
        {
            return PointF.Subtract(pt, sz);
        }

        public static PointF operator -(PointF pt, SizeF sz)
        {
            return PointF.Subtract(pt, sz);
        }

        public static PointF Subtract(PointF pt, Size sz)
        {
            return new PointF(pt.X - (float)sz.Width, pt.Y - (float)sz.Height);
        }

        public static PointF Subtract(PointF pt, SizeF sz)
        {
            return new PointF(pt.X - sz.Width, pt.Y - sz.Height);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", new object[] { this.x, this.y });
        }
    }

}
