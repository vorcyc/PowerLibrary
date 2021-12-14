using System;

namespace Vorcyc.PowerLibrary.ArrayEx
{
    /// <summary>
    /// 与数字数组相关功能的扩展
    /// </summary>
    public static partial class NumericArrayExtension
    {

        /// <summary>
        /// 返回一组数字中的最大值
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static float Max(this float[] array)
        {
            var result = array[0];
            for (int i = 0; i < array.Length; i++) {
                if (array[i] > result) result = array[i];
            }
            return result;
        }

        /// <summary>
        /// 返回一组数字中的最大值
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double Max(this double[] array)
        {
            var result = array[0];
            for (int i = 0; i < array.Length; i++) {
                if (array[i] > result) result = array[i];
            }
            return result;
        }

        /// <summary>
        /// 返回实现了 <see cref="IComparable"/> 和 <see cref="IComparable{T}"/> 接口的类型的数组的最大值
        /// </summary>
        /// <typeparam name="T">类型参数，约束为<see cref="IComparable"/> , <see cref="IComparable{T}"/></typeparam>
        /// <param name="array">数组</param>
        /// <returns></returns>
        public static T Max<T>(this T[] array) where T : IComparable, IComparable<T>
        {
            var result = array[0];
            for (int i = 0; i < array.Length; i++) {
                if (array[i].CompareTo(result) == 1) result = array[i];
            }
            return result;
        }

        /// <summary>
        /// 返回一组数字中的最小值
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static float Min(this float[] array)
        {
            var result = array[0];
            for (int i = 0; i < array.Length; i++) {
                if (array[i] < result) result = array[i];
            }
            return result;
        }

        /// <summary>
        /// 返回一组数字中的最小值
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double Min(this double[] array)
        {
            var result = array[0];
            for (int i = 0; i < array.Length; i++) {
                if (array[i] < result) result = array[i];
            }
            return result;
        }


        /// <summary>
        /// 返回实现了 <see cref="IComparable"/> 和 <see cref="IComparable{T}"/> 接口的类型的数组的最小值
        /// </summary>
        /// <typeparam name="T">类型参数，约束为<see cref="IComparable"/> , <see cref="IComparable{T}"/></typeparam>
        /// <param name="array">数组</param>
        /// <returns></returns>
        public static T Min<T>(this T[] array) where T : IComparable, IComparable<T>
        {
            var result = array[0];
            for (int i = 0; i < array.Length; i++) {
                if (array[i].CompareTo(result) == -1) result = array[i];
            }
            return result;
        }


        /// <summary>
        /// 找最大值和最小值，并返回TupleValue&lt;T1,T2&gt;
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static (float max, float min) FindMaximumAndMinimum(
            this float[] array,
            int start, int length)
        {
            var returnMin = float.MaxValue;
            var returnMax = float.MinValue;

            var end = Math.Min(start + length, array.Length);

            for (int i = start; i < end; i++) {
                float value = array[i];
                returnMin = (value < returnMin) ? value : returnMin;
                returnMax = (value > returnMax) ? value : returnMax;
            }

            return (returnMax, returnMin);
        }




        /// <summary>
        /// 同时返回最大值和最小值
        /// </summary>
        /// <param name="arraySegment"></param>
        /// <returns></returns>
        public static (float max, float min) FindMaximumAndMinimum(
            this ArraySegment<float> arraySegment)
        {
            var returnMin = float.MaxValue;
            var returnMax = float.MinValue;

            for (int i = arraySegment.Offset; i < (arraySegment.Offset + arraySegment.Count); i++) {
                float value = arraySegment.Array[i];
                returnMin = (value < returnMin) ? value : returnMin;
                returnMax = (value > returnMax) ? value : returnMax;
            }

            return (returnMax, returnMin);
        }




        /// <summary>
        /// 返回数组的最大值和它在数组中的0基索引
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static (float max, int index)
            FindMaxAndIndex(this float[] array)
        {
            var retMax = float.MinValue;
            var retIndex = 0;

            for (int i = 0; i < array.Length; i++) {
                if (array[i] > retMax) {
                    retMax = array[i];
                    retIndex = i;
                }
            }

            return (retMax, retIndex);
        }

        /// <summary>
        /// 找最大值和最小值，并返回TupleValue<T>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static (float max, int maxIndex, float min, int minIndex) FindMaximumAndMinimumWithIndex(
            this float[] array,
            int start, int length)
        {
            var returnMin = float.MaxValue;
            var vMinIndex = 0;

            var returnMax = float.MinValue;
            var vMaxIndex = 0;

            var end = Math.Min(start + length, array.Length);

            for (int i = start; i < end; i++) {
                float value = array[i];

                if (value < returnMin) {
                    returnMin = value;
                    vMinIndex = i;
                }

                if (value > returnMax) {
                    returnMax = value;
                    vMaxIndex = i;
                }
            }

            return (returnMax, vMaxIndex, returnMin, vMinIndex);
        }



        /// <summary>
        /// 返回数组的最小值和它在数组中的0基索引
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static (float min, int index)
            FindMinAndIndex(this float[] array)
        {
            var retMin = float.MaxValue;
            var retIndex = 0;

            for (int i = 0; i < array.Length; i++) {
                if (array[i] < retMin) {
                    retMin = array[i];
                    retIndex = i;
                }
            }

            return (retMin, retIndex);
        }


        /// <summary>
        /// Returns the minimum value in an array with zero-based index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static (T min, int index) FindMinAndIndex<T>(this T[] array) where T : IComparable, IComparable<T>
        {
            var retMin = array[0];
            var retIndex = 0;

            for (int i = 0; i < array.Length; i++) {
                if (array[i].CompareTo(retMin) == -1) {
                    retMin = array[i];
                    retIndex = i;
                }
            }

            return (retMin, retIndex);
        }



        /// <summary>
        /// 求数组的均值
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static float GetAverage(this float[] array)
        {
            float result = 0.0f;

            for (int i = 0; i < array.Length; i++) {
                result += array[i];
            }
            return result / array.Length;
        }

    }
}
