/// <summary>
/// 本命名空间包含操作字符串的方法扩展。
/// </summary>
namespace Vorcyc.PowerLibrary.StringManipulation
{
    /// <summary>
    /// 字符扩展
    /// </summary>
    public static class CharExtension
    {
        /// <summary>
        /// 解析一个字符到数字，前提是该字符属于数字字符
        /// </summary>
        /// <param name="c">输入数字字符</param>
        /// <returns></returns>
        public static int Parse(this char c)
        {
            if (c >= 49 && c <= 57) return c - 48;
            return 0;
        }
    }

}
