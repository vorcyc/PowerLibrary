using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 本命名空间提供一些数组相关的功能扩展
/// </summary>
namespace Vorcyc.PowerLibrary.ArrayEx
{
    /// <summary>
    /// 数组扩展
    /// </summary>
    public static class ArrayExtension
    {

        /// <summary>
        /// 取内部一段，并返回迭代集。
        /// </summary>
        /// <remarks>
        /// 可以使用LINQ提供的扩展方法 System.Linq.Enumerable.Skip(start).Take(length)）实现同样功能。
        /// 但本版本性能更高。
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">源数组</param>
        /// <param name="start">起始索引</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static IEnumerable<T> GetInner<T>(this T[] array, int start, int length)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (start < 0 || start > array.Length)
                throw new ArgumentOutOfRangeException("start");

            if (length <= 0)
                throw new ArgumentOutOfRangeException("length");

            int len = System.Math.Min(array.Length, length) + start;
            for (int i = start; i < len; i++) {
                yield return array[i];
            }
        }


        /// <summary>
        /// 取一个数组的内部片段，并返回片段。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">源数组</param>
        /// <param name="start">起始索引</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static T[] GetInnerArray<T>(this T[] array, int start, int length)
        /*where T : struct , IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>*/
        {
            if (array == null)
                throw new ArgumentNullException();
            if (start < 0 || start > array.Length)
                throw new ArgumentOutOfRangeException("start");

            if (length <= 0)
                throw new ArgumentOutOfRangeException("length");

            int arrayBound = System.Math.Min(array.Length - start, length);
            T[] result = new T[arrayBound];
            Array.Copy(array, start, result, 0, arrayBound);
            return result;
        }

        #region 其它对比
        //public unsafe static double[] GetInnerArray(this double[] array, int start, int length)
        //{
        //    if (array == null)
        //        throw new ArgumentNullException();
        //    if (start < 0 || start > array.Length)
        //        throw new ArgumentException("start");
        //    /*下行本来是这样 if (length > src.Length||length <=0),
        //     * 既然循环那点那样灵活了,这里也放宽些限制.可以传入任何值，
        //     * 至于是否有效么又再判断
        //     */
        //    if (length <= 0)
        //        throw new ArgumentException("length");

        //    int arrayBound = System.Math.Min(array.Length - start, length);//src.Length-start是从start到整个数组完的元素数
        //    double[] result = new double[arrayBound];

        //    //按long,一次复制8个byte
        //    fixed (double* pSrc = array, pDst = result)
        //    {
        //        double* ps = pSrc + start;//源的起始值是要加上start的
        //        long* lpSrc = (long*)ps;
        //        long* lpDst = (long*)pDst;

        //        for (int i = 0; i < Math.Min(array.Length, length); i += 1)
        //        {
        //            *(lpDst) = *(lpSrc);
        //            lpSrc++;
        //            lpDst++;
        //        }

        //    }
        //    return result;
        //}
        #endregion


        #region GetSegmentAverage

        //老版本，性能低

        ///// <summary>
        ///// 按指定长度分段，返回每段长度的平均值的数组
        ///// </summary>
        ///// <param name="array">源数组</param>
        ///// <param name="segmentLength">段长</param>
        ///// <returns></returns>
        //public static double[] GetSegmentAverage(this double[] array, int segmentLength)
        //{
        //    if (array == null)
        //        throw new ArgumentNullException("array");

        //    var length = (int)System.Math.Ceiling((double)array.Length / segmentLength);
        //    double[] result = new double[length];
        //    for (int i = 0; i < length; i++) {
        //        result[i] = array.GetInnerArray(i * segmentLength, segmentLength).Average();
        //    }
        //    return result;
        //}


        private static double SumCore(double[] source, int start, int length)
        {
            double result = 0.0;
            //var len =  System.Math.Min(length, source.Length - start) + start;
            for (int i = start; i < length; i++) {
                result += source[i];
            }
            return result;
        }


        private static float SumCore(float[] source, int start, int length)
        {
            float result = 0.0f;
            //var len =  System.Math.Min(length, source.Length - start) + start;
            for (int i = start; i < length; i++) {
                result += source[i];
            }
            return result;
        }


        private static double GetSegmentAverageCore(double[] source, int start, int length)
        {
            var end = System.Math.Min(length, source.Length - start);
            return SumCore(source, start, start + end) / end;
        }

        private static float GetSegmentAverageCore(float[] source, int start, int length)
        {
            var end = System.Math.Min(length, source.Length - start);
            return SumCore(source, start, start + end) / end;
        }

        //性能高，因为不取内部数组了

        /// <summary>
        /// 对源数组的全部元素按指定长度分段，返回每段长度的平均值的数组
        /// </summary>
        /// <param name="array"></param>
        /// <param name="segmentLength"></param>
        /// <returns></returns>
        public static double[] GetSegmentAverage(this double[] array, int segmentLength)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var length = (int)System.Math.Ceiling((double)array.Length / segmentLength);
            double[] result = new double[length];
            for (int i = 0; i < length; i++) {
                result[i] = GetSegmentAverageCore(array, i * segmentLength, segmentLength);
            }
            return result;
        }

        /// <summary>
        /// 对源数组的部分元素按指定长度分段，返回每段长度的平均值的数组
        /// </summary>
        /// <param name="source">源数组</param>
        /// <param name="start">子段的起始索引</param>
        /// <param name="length">子段的长度</param>
        /// <param name="segmentLength">段长</param>
        /// <returns></returns>
        public static double[] GetSegmentAverage(this double[] source, int start, int length, int segmentLength)
        {
            var len = System.Math.Min(length, source.Length - start);
            var retLlength = (int)System.Math.Ceiling((double)len / segmentLength);
            double[] result = new double[retLlength];
            for (int i = 0; i < retLlength; i++) {
                result[i] = GetSegmentAverageCore(source, start + i * segmentLength, segmentLength);
            }
            return result;
        }

        /// <summary>
        /// 对源数组的全部元素按指定长度分段，返回每段长度的平均值的数组
        /// </summary>
        /// <param name="array">源数组</param>
        /// <param name="segmentLength">段长</param>
        /// <returns></returns>
        public static float[] GetSegmentAverage(this float[] array, int segmentLength)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var length = (int)System.Math.Ceiling((double)array.Length / segmentLength);
            float[] result = new float[length];
            for (int i = 0; i < length; i++) {
                result[i] = GetSegmentAverageCore(array, i * segmentLength, segmentLength);
            }
            return result;
        }

        /// <summary>
        /// 对源数组的部分元素按指定长度分段，返回每段长度的平均值的数组
        /// </summary>
        /// <param name="source">源数组</param>
        /// <param name="start">子段的起始索引</param>
        /// <param name="length">子段的长度</param>
        /// <param name="segmentLength">段长</param>
        /// <returns></returns>
        public static float[] GetSegmentAverage(this float[] source, int start, int length, int segmentLength)
        {
            var len = System.Math.Min(length, source.Length - start);
            var retLlength = (int)System.Math.Ceiling((double)len / segmentLength);
            float[] result = new float[retLlength];
            for (int i = 0; i < retLlength; i++) {
                result[i] = GetSegmentAverageCore(source, start + i * segmentLength, segmentLength);
            }
            return result;
        }

        #endregion


        #region 其它对比
        //public static double[] GetSegmentAverage2(this double[] array, int segmentLength)
        //{
        //    if (array == null)
        //        throw new ArgumentNullException("array");

        //    var length = (int)Math.Ceiling((double)array.Length / segmentLength);
        //    double[] result = new double[length];
        //    int i = 0;
        //    int outIndex = 0;
        //    while (i < array.Length)
        //    {
        //        result[outIndex] = array.GetInnerArray(i, segmentLength).Average();
        //        i += segmentLength;
        //        outIndex++;
        //    }
        //    return result;
        //}

        //public static double[] GetSegmentAverage3(this double[] array, int segmentLength)
        //{
        //    if (array == null)
        //        throw new ArgumentNullException("array");

        //    var length = (int)Math.Ceiling((double)array.Length / segmentLength);
        //    double[] result = new double[length];
        //    int outIndex = 0;
        //    for (int i = 0; i < array.Length; i += segmentLength)
        //    {
        //        result[outIndex] = array.GetInnerArray(i, segmentLength).Average();
        //        outIndex++;
        //    }
        //    return result;
        //}
        #endregion


        #region RemoveSegment

        /// <summary>
        /// 移出数组中的一部分，并返回移出后的新数组。
        /// </summary>
        /// <remarks>
        /// 本方法会进行安全检查，因此即使索引越界也不会抛出异常。
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">源数组</param>
        /// <param name="start">待移出部分的起始索引</param>
        /// <param name="length">待移出部分的长度</param>
        /// <returns>返回移出指定段后的数组</returns>
        public static T[] RemoveSegment<T>(this T[] array, int start, int length)
        {
            var result = new T[array.Length - length];
            Array.Copy(array, 0, result, 0, start);
            Array.Copy(array, start + length, result, start, array.Length - start - length);
            return result;
        }

        //public static T[] RemoveSegment<T>(this T[] array,
        //    int start, int length)
        //{
        //    if (array == null)
        //        throw new ArgumentNullException("array");

        //    var result = new T[array.Length - length];
        //    T[] behind;
        //    if (start == 0)
        //        behind = new T[0];
        //    else
        //        behind = array.GetInnerArray(0, start);

        //    if (length >= array.Length) return new T[0];

        //    var len = array.Length - start - length;
        //    if (len <= 0)
        //        throw new ArgumentOutOfRangeException("length");

        //    var after = array.GetInnerArray(start + length, len);

        //    return behind.Combine(after);

        //}


        //public static double[] RemoveSegment(this double[] array,
        //    int start,
        //    int length)
        //{
        //    if (array == null)
        //        throw new ArgumentNullException("array");

        //    var result = new double[array.Length - length];
        //    double[] behind;
        //    if (start == 0)
        //        behind = new double[0];
        //    else
        //        behind = array.GetInnerArray(0, start);

        //    if (length >= array.Length) return new double[0];

        //    var len = array.Length - start - length;
        //    if (len <= 0)
        //        throw new ArgumentOutOfRangeException("length");

        //    var after = array.GetInnerArray(start + length, len);

        //    return behind.Combine(after);
        //}

        #endregion


        /// <summary>
        /// 联接两个数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="leading">前置数组</param>
        /// <param name="following">后置数组</param>
        /// <returns>返回联接后的新数组</returns>
        public static T[] Combine<T>(this T[] leading, T[] following)
        {
            var result = new T[leading.Length + following.Length];
            Array.Copy(leading, result, leading.Length);
            Array.Copy(following, 0, result, leading.Length, following.Length);
            return result;
        }


    }


}
