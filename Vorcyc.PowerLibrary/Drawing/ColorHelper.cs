

using SysMath = System.Math;


/// <summary>
/// 本命名空间包含 2D 图形基础结构
/// </summary>
namespace Vorcyc.PowerLibrary.Drawing
{

    public static class ColorHelper
    {

        /// <summary>
        /// 得到颜色偏移值，正值加深，负值变浅
        /// </summary>
        /// <param name="baseColor">基色</param>
        /// <param name="offset">偏移量，正值加深，负值变浅</param>
        /// <returns></returns>
        public static System.Drawing.Color GetOffsetColor(System.Drawing.Color baseColor, int offset)
        {
            if (offset > 0)
                return System.Drawing.Color.FromArgb(
                    SysMath.Max(baseColor.R - offset, 0),
                    SysMath.Max(baseColor.G - offset, 0),
                    SysMath.Max(baseColor.B - offset, 0));
            else
                return System.Drawing.Color.FromArgb(
                    SysMath.Min(baseColor.R - offset, 255),
                    SysMath.Min(baseColor.G - offset, 255),
                    SysMath.Min(baseColor.B - offset, 255));

        }





        /// <summary>
        /// 返回颜色反色，不改变Alpha
        /// </summary>
        /// <param name="baseColor">基色</param>
        /// <returns></returns>
        public static System.Drawing.Color GetInverseColor(System.Drawing.Color baseColor)
        {
            return System.Drawing.Color.FromArgb((byte)~baseColor.R, (byte)~baseColor.G, (byte)~baseColor.B);
        }


    }
}
