using System.Collections.Generic;

/// <summary>
/// 本命名空间提供集合相关扩展功能
/// </summary>
namespace Vorcyc.PowerLibrary.CollectionEx
{
    public static class EnumerableEx
    {

        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// 生成一个指定数量32位浮点数序列，该序列的起始值和结束值在指定范围内。
        /// </summary>
        /// <param name="start">起始值</param>
        /// <param name="end">结束值</param>
        /// <param name="count">序列数量</param>
        /// <returns>生成的序列</returns>
        public static IEnumerable<float> Range(float start, float end, int count)
        {
            var step = (end - start) / (count - 1);
            for (float v = start; v <= end; v += step)
                yield return v;
        }

        /// <summary>
        /// 生成一个指定数量64位浮点数序列，该序列的起始值和结束值在指定范围内。
        /// </summary>
        /// <param name="start">起始值</param>
        /// <param name="end">结束值</param>
        /// <param name="count">序列数量</param>
        /// <returns>生成的序列</returns>
        public static IEnumerable<double> Range(double start, double end, int count)
        {
            var step = (end - start) / (count - 1);
            for (double v = start; v <= end; v += step)
                yield return v;
        }

    }
}
