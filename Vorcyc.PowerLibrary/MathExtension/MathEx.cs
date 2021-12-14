

using System;


namespace Vorcyc.PowerLibrary.MathExtension
{
    /// <summary>
    /// 数学/数字工具扩展
    /// </summary>
    public static class MathEx
    {

        /// <summary>
        /// 求n的阶乘
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Factorial(int n)
        {
            int num = n;
            if (num == 0) {
                return 1;
            }
            return (num * Factorial(num - 1));
        }

        /// <summary>
        /// highest common factor 最大公约数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Hcf(int a, int b)
        {
            if (a == b) {
                return b;
            }
            if (a < b) {
                return Hcf(a, b - a);
            }
            return Hcf(a - b, b);
        }


        /// <summary>
        /// 求最小公倍数.
        /// 几个数共有的倍数叫做这几个数的公倍数，其中除0以外最小的一个公倍数，叫做这几个数的最小公倍数。
        /// Least Common Multiple 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Lcm(int a, int b)
        {
            return a * b / Hcf(a, b);
        }


        public static int FixRange(int value, int min, int max)
        {
            if (max <= min) throw new ArgumentOutOfRangeException("max ,min.");
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        public static T FixRange<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (max.CompareTo(min) <= 0) throw new ArgumentOutOfRangeException("max ,min.");
            if (value.CompareTo(max) > 0) return max;
            if (value.CompareTo(min) <= 0) return min;
            return value;
        }

        public static bool IsRangeIn(int value, int min, int max)
        {
            return (value >= min) && (value <= max);
        }

        public static bool IsRangeIn<T>(T value, T min, T max) where T : struct, IComparable
        {
            return ((value.CompareTo(min) >= 0) && (value.CompareTo(max) <= 0));
        }


        /*按位与：有0为0
         * 按位或：有1为1
         * 按位异或：同样为0，异样为1
         */


        /// <summary>
        /// Return whether the specified two numbers have same sign.
        /// </summary>
        /// <param name="numberA"></param>
        /// <param name="numberB"></param>
        /// <returns></returns>
        public static bool HasSameSign(int numberA, int numberB)
        {
            return (numberA ^ numberB) >= 0;
        }



        /// <summary>
        /// Round up to the nearest odd number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int RoundUpToNearestOdd(int number)
        {
            return number | 1;
        }


        /// <summary>
        /// Round down to the nearest even number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int RoundDownToNearestEven(int number)
        {
            return number & ~1;
        }

        /// <summary>
        /// 返回徘徊数（我自己起名呢）。
        /// 若是奇数则返回比它小一的数，若偶数则返回比它大一呢数
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int RoundAround(int number)
        {
            return number ^ 1;
        }

        /// <summary>
        /// 判断是否是奇数
        /// </summary>
        public static bool IsOdd(long value)
        {
            //vb: (value And 1) = 1
            return (value & 1L) == 1;
        }

        /// <summary>
        /// 判断是否是偶数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEven(long value)
        {
            //vb: Not (value And 1) = 1
            //return ~(value & 1L) == 1;
            //上面错了，18.7.9发现
            //~ 是按位取反
            //! 才是逻辑非
            return !((value & 1L) == 1);
        }

        /// <summary>
        /// 将数字从一个范围映射至另一个范围
        /// </summary>
        /// <param name="number">输入的数字</param>
        /// <param name="rangeMin">数字所在区间的最小值，开区间（包含）</param>
        /// <param name="rangeMax">数字所在区间的最大值，开区间（包含）</param>
        /// <param name="mappingMin">映射至区间的最小值，开区间（包含）</param>
        /// <param name="mappingMax">映射至区间的最大值，开区间（包含）</param>
        /// <returns></returns>
        public static float NumberMapping(
            float number,
            float rangeMin, float rangeMax,
            float mappingMin, float mappingMax)
        {
            if (number < rangeMin || number > rangeMax)
                throw new ArgumentOutOfRangeException(nameof(number));

            if (rangeMin >= rangeMax)
                throw new ArgumentOutOfRangeException("rangeMin must less than rangeMax.");

            if (mappingMin >= mappingMax)
                throw new ArgumentOutOfRangeException("mappingMin must less than mappingMax.");

            var inputRange = rangeMax - rangeMin;
            var outputRange = mappingMax - mappingMin;
            var r = outputRange / inputRange;
            return mappingMin + r * (number - rangeMin);
        }

        /// <summary>
        /// 将数字从一个范围映射至另一个范围
        /// </summary>
        /// <param name="number">输入的数字</param>
        /// <param name="rangeMin">数字所在区间的最小值，开区间（包含）</param>
        /// <param name="rangeMax">数字所在区间的最大值，开区间（包含）</param>
        /// <param name="mappingMin">映射至区间的最小值，开区间（包含）</param>
        /// <param name="mappingMax">映射至区间的最大值，开区间（包含）</param>
        /// <returns></returns>
        public static double NumberMapping(
            double number,
            double rangeMin, double rangeMax,
            double mappingMin, double mappingMax)
        {
            if (number < rangeMin || number > rangeMax)
                throw new ArgumentOutOfRangeException(nameof(number));

            if (rangeMin >= rangeMax)
                throw new ArgumentOutOfRangeException("rangeMin must less than rangeMax.");

            if (mappingMin >= mappingMax)
                throw new ArgumentOutOfRangeException("mappingMin must less than mappingMax.");

            var inputRange = rangeMax - rangeMin;
            var outputRange = mappingMax - mappingMin;
            var r = outputRange / inputRange;
            return mappingMin + r * (number - rangeMin);
        }

      

        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="a"> An angle, measured in radians.</param>
        /// <returns></returns>
        public static double RadiansToDegrees(double a)
        {
            return 180 * a / Math.PI;
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="a">An angle, measured in degress.</param>
        /// <returns></returns>
        public static double DegreesToRadians(double a)
        {
            return a * Math.PI / 180;
        }
    }
}
