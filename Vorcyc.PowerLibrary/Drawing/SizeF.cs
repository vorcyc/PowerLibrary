using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Vorcyc.PowerLibrary.Drawing
{
    public struct SizeF
    {
        public readonly static SizeF Empty;

        private float width;

        private float height;

        public float Height {
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
                if (this.width != 0f) {
                    return false;
                }
                return this.height == 0f;
            }
        }

        public float Width {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        static SizeF()
        {
        }

        public SizeF(SizeF size)
        {
            this.width = size.width;
            this.height = size.height;
        }

        public SizeF(PointF pt)
        {
            this.width = pt.X;
            this.height = pt.Y;
        }

        public SizeF(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        public static SizeF Add(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SizeF)) {
                return false;
            }
            SizeF sizeF = (SizeF)obj;
            if (sizeF.Width != this.Width || sizeF.Height != this.Height) {
                return false;
            }
            return sizeF.GetType().Equals(this.GetType());
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        public static SizeF operator +(SizeF sz1, SizeF sz2)
        {
            return SizeF.Add(sz1, sz2);
        }

        public static bool operator ==(SizeF sz1, SizeF sz2)
        {
            if (sz1.Width != sz2.Width) {
                return false;
            }
            return sz1.Height == sz2.Height;
        }

        public static explicit operator PointF(SizeF size)
        {
            return new PointF(size.Width, size.Height);
        }

        public static bool operator !=(SizeF sz1, SizeF sz2)
        {
            return !(sz1 == sz2);
        }

        public static SizeF operator -(SizeF sz1, SizeF sz2)
        {
            return SizeF.Subtract(sz1, sz2);
        }

        public static SizeF Subtract(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        public PointF ToPointF()
        {
            return (PointF)this;
        }

        public Size ToSize()
        {
            return Size.Truncate(this);
        }

        public override string ToString()
        {
            return string.Concat(new string[] { "{Width=", this.width.ToString(CultureInfo.CurrentCulture), ", Height=", this.height.ToString(CultureInfo.CurrentCulture), "}" });
        }
    }
}
