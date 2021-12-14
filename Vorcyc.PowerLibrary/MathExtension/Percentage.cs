/* 17.11.26
 * by Vorcyc 
 * cyclone_dll
 */

namespace Vorcyc.PowerLibrary.MathExtension
{

    using System;

    /// <summary>
    /// Represents a Percentage.
    /// <para>Only floating-point numbers can accept convert from this type.</para>
    /// </summary>
    /// <remarks>
    /// 本类型的值域为 [0%,100%]
    /// </remarks>
    /// <example>
    /// 以下代码创建一个<see cref="Percentage"/>实例，并为其赋值为50%
    /// <code>
    /// var p = new Percentage();
    /// p = 50;
    /// Console.WriteLine(p);
    /// => 50%
    /// </code>
    /// 以下代码直接从负整数创建<see cref="Percentage"/>，但由于本类型的取值不能小余0，所以最终为0%：
    /// <code>
    /// Percentage p2 = -53;
    /// Console.WriteLine(p2);
    /// => 0%
    /// </code>
    /// </example>
    public struct Percentage : IComparable
    {

        private float _actualValue;

        private const float _max = 1.0f;

        private const float _min = 0.0f;

        private Percentage(float actualValue)
        {
            if (actualValue < _min) _actualValue = _min;
            else if (actualValue > _max) _actualValue = _max;
            else _actualValue = actualValue;
        }


        #region IComparable

        public int CompareTo(object obj)
        {
            return _actualValue.CompareTo(obj);
        }

        #endregion


        #region Comparison And Equality Operators

        public static bool operator <(Percentage left, Percentage right)
        {
            return left._actualValue < right._actualValue;
        }

        public static bool operator >(Percentage left, Percentage right)
        {
            return left._actualValue > right._actualValue;
        }

        public static bool operator <=(Percentage left, Percentage right)
        {
            return left._actualValue <= right._actualValue;
        }

        public static bool operator >=(Percentage left, Percentage right)
        {
            return left._actualValue >= right._actualValue;
        }

        public static bool operator ==(Percentage left, Percentage right)
        {
            return left._actualValue == right._actualValue;
        }

        public static bool operator !=(Percentage left, Percentage right)
        {
            return left._actualValue != right._actualValue;
        }


        #endregion


        #region Arithmetic Operators

        public static Percentage operator +(Percentage left, Percentage right)
        {
            return new Percentage(left._actualValue + right._actualValue);
        }

        public static Percentage operator -(Percentage left, Percentage right)
        {
            return new Percentage(left._actualValue - right._actualValue);
        }

        public static Percentage operator *(Percentage left, Percentage right)
        {
            return new Percentage(left._actualValue * right._actualValue);
        }

        public static Percentage operator /(Percentage left, Percentage right)
        {
            return new Percentage(left._actualValue / right._actualValue);
        }

        public static Percentage operator ++(Percentage p)
        {
            return new Percentage(p._actualValue += 0.01f);
        }

        public static Percentage operator --(Percentage p)
        {
            return new Percentage(p._actualValue -= 0.01f);
        }

        #endregion


        #region Type Conversion Operators

        //8 bits

        public static implicit operator Percentage(byte v)
        {
            return new Percentage(v / 100.0f);
        }

        public static implicit operator Percentage(sbyte v)
        {
            return new Percentage(v / 100.0f);
        }

        //16 bits
        public static implicit operator Percentage(ushort v)
        {
            return new Percentage(v / 100.0f);
        }

        public static implicit operator Percentage(short v)
        {
            return new Percentage(v / 100.0f);
        }

        //32 bits
        public static implicit operator Percentage(uint v)
        {
            return new Percentage(v / 100.0f);
        }

        public static implicit operator Percentage(int v)
        {
            return new Percentage(v / 100.0f);
        }

        //64 bits
        public static implicit operator Percentage(ulong v)
        {
            return new Percentage(v / 100.0f);
        }

        public static implicit operator Percentage(long v)
        {
            return new Percentage(v / 100.0f);
        }

        //floating numbers
        //only floating numbers can accept TYPE Converstion.
        public static implicit operator Percentage(float v)
        {
            return new Percentage(v / 100.0f);
        }

        public static explicit operator float(Percentage p)
        {
            return p._actualValue;
        }

        public static implicit operator Percentage(double v)
        {
            return new Percentage((float)(v / 100.0));
        }

        public static explicit operator double(Percentage p)
        {
            return p._actualValue;
        }


        #endregion


        #region overrides Object members

        public override bool Equals(object obj)
        {
            return _actualValue.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _actualValue.GetHashCode() * 100;
        }

        public override string ToString()
        {
            return $"{ _actualValue * 100}%";
        }

        #endregion

    }
}
