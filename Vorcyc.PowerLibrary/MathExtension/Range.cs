using System;

namespace Vorcyc.PowerLibrary.MathExtension
{
    /// <summary>
    /// 表示一个范围的开区间
    /// </summary>
    /// <typeparam name="T">值</typeparam>
    public sealed class Range<T>
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        private T _value;

        /// <summary>
        /// 初始化并设置最小最大值。
        /// </summary>
        /// <param name="minimum">指定区间的最小值</param>
        /// <param name="maximum">指定区间的最大值</param>
        public Range(T minimum, T maximum)
        {
            if (maximum.CompareTo(minimum) < 0) throw new ArgumentException("Maximum must greater than minimun.");

            Minimum = minimum;
            Maximum = maximum;
        }

        /// <summary>
        /// 初始化并设置最小、最大和实际值。
        /// </summary>
        /// <param name="minimun">最小值</param>
        /// <param name="maximum">最大值</param>
        /// <param name="value">当前值</param>
        public Range(T minimun, T maximum, T value)
        {
            if (maximum.CompareTo(minimun) < 0) throw new ArgumentException("Maximum must greater than minimun.");

            Minimum = minimun;
            Maximum = maximum;
            Value = value;
        }


        /// <summary>
        /// 获取或设置值，设置值时会限定在依极值所限定的区间内.
        /// </summary>
        public T Value
        {
            get { return _value; }
            set
            {
                if (value.CompareTo(Maximum) > 0)
                {
                    _value = Maximum;
                    return;
                }

                if (value.CompareTo(Minimum) <= 0)
                {
                    _value = Minimum;
                    return;
                }

                _value = value;
            }
        }

        /// <summary>
        /// 获取在构造时设置的最大值
        /// </summary>
        public T Maximum { get; }

        /// <summary>
        /// 获取在构造时设置的最小值
        /// </summary>
        public T Minimum { get; }

    }
}
