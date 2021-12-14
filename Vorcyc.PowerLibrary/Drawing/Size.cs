using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Vorcyc.PowerLibrary.Drawing
{
    public struct Size
    {
        public readonly static Size Empty;

        private int width;

        private int height;

        public int Height {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        [Browsable(false)]
        public bool IsEmpty {
            get
            {
                if (this.width != 0) {
                    return false;
                }
                return this.height == 0;
            }
        }

        public int Width {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        static Size()
        {
        }

        public Size(Point pt)
        {
            this.width = pt.X;
            this.height = pt.Y;
        }

        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public static Size Add(Size sz1, Size sz2)
        {
            return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        public static Size Ceiling(SizeF value)
        {
            return new Size((int)Math.Ceiling((double)value.Width), (int)Math.Ceiling((double)value.Height));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size)) {
                return false;
            }
            Size size = (Size)obj;
            if (size.width != this.width) {
                return false;
            }
            return size.height == this.height;
        }

        public override int GetHashCode()
        {
            return this.width ^ this.height;
        }

        public static Size operator +(Size sz1, Size sz2)
        {
            return Size.Add(sz1, sz2);
        }

        public static bool operator ==(Size sz1, Size sz2)
        {
            if (sz1.Width != sz2.Width) {
                return false;
            }
            return sz1.Height == sz2.Height;
        }

        public static explicit operator Point(Size size)
        {
            return new Point(size.Width, size.Height);
        }

        public static implicit operator SizeF(Size p)
        {
            return new SizeF((float)p.Width, (float)p.Height);
        }

        public static bool operator !=(Size sz1, Size sz2)
        {
            return !(sz1 == sz2);
        }

        public static Size operator -(Size sz1, Size sz2)
        {
            return Size.Subtract(sz1, sz2);
        }

        public static Size Round(SizeF value)
        {
            return new Size((int)Math.Round((double)value.Width), (int)Math.Round((double)value.Height));
        }

        public static Size Subtract(Size sz1, Size sz2)
        {
            return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        public override string ToString()
        {
            return string.Concat(new string[] { "{Width=", this.width.ToString(CultureInfo.CurrentCulture), ", Height=", this.height.ToString(CultureInfo.CurrentCulture), "}" });
        }

        public static Size Truncate(SizeF value)
        {
            return new Size((int)value.Width, (int)value.Height);
        }
    }
}
