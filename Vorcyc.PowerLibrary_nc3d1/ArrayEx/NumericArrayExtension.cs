namespace Vorcyc.PowerLibrary.ArrayEx
{
    public static partial class NumericArrayExtension
    {

        /// <summary>
        /// 同时返回最大值和最小值
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static (float max, float min) FindMaximumAndMinimum(
            System.Span<float> span)
        {
            var returnMin = float.MaxValue;
            var returnMax = float.MinValue;


            for (int i = 0; i < span.Length; i++) {
                float value = span[i];
                returnMin = (value < returnMin) ? value : returnMin;
                returnMax = (value > returnMax) ? value : returnMax;
            }

            return (returnMax, returnMin);
        }

    }
}
