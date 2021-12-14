using System.Text;

namespace Vorcyc.PowerLibrary.StringManipulation
{
    /// <summary>
    /// 汉字拼音功能扩展
    /// </summary>
    public class Pinyin
    {

        //list item 不加 <description> vsdocman 生成的文档显示不出来 item
        //em 是斜体

        /// <summary>
        /// 取<c>汉字</c>拼音的首字母
        /// </summary>
        /// <param name="UnName">汉字</param>
        /// <returns>若是中文，则每个中文字符首字母，否则原样返回</returns>
        /// <remarks>
        /// 若要在.NET Core中使用此类，需要在调用程序中做这些事：
        /// <list type="number">
        /// <item><description><para><em>nuget 安装 <strong>System.Text.Encoding.CodePages </strong>包</em></para></description> </item>
        /// <item><description><para><em>
        /// 程序入口处添加如下代码：
        /// <code>Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);</code>
        /// </em></para></description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <example>
        /// 以下代码演示如何使用本方法以获取汉字的拼音首字母：
        /// <code>
        /// Console.WriteLine(Vorcyc.PowerLibrary.StringManipulation.Pinyin.GetPinyinHead("你好"));
        ///  =>  NH
        ///
        /// Console.WriteLine(Vorcyc.PowerLibrary.StringManipulation.Pinyin.GetPinyinHead("昆明涡旋科技有限公司11aaa"));
        ///  =>  KMWXKJYXGS11aaa
        /// </code>
        /// </example>
        public static string GetPinyinHead(string UnName)
        {
            int i = 0;
            var result = new StringBuilder();

            Encoding unicode = Encoding.Unicode;
            //Encoding gbk = Encoding.GetEncoding(936);
            Encoding gbk = Encoding.GetEncoding("GB2312");


            byte[] unicodeBytes = unicode.GetBytes(UnName);
            byte[] gbkBytes = Encoding.Convert(unicode, gbk, unicodeBytes);

            while (i < gbkBytes.Length) 
            {
                if (gbkBytes[i] <= 127)
                {
                    result.Append((char)gbkBytes[i]);
                    result.Append((char)gbkBytes[i]);
                    i++;
                }
                #region 生成汉字拼音简码,取拼音首字母
                else 
                {
                    ushort key = (ushort)(gbkBytes[i] * 256 + gbkBytes[i + 1]);
                    if (key >= '\uB0A1' && key <= '\uB0C4') {
                        result.Append('A');
                    }
                    else if (key >= '\uB0C5' && key <= '\uB2C0') {
                        result.Append('B');
                    }
                    else if (key >= '\uB2C1' && key <= '\uB4ED') {
                        result.Append('C');
                    }
                    else if (key >= '\uB4EE' && key <= '\uB6E9') {
                        result.Append('D');
                    }
                    else if (key >= '\uB6EA' && key <= '\uB7A1') {
                        result.Append('E');
                    }
                    else if (key >= '\uB7A2' && key <= '\uB8C0') {
                        result.Append('F');
                    }
                    else if (key >= '\uB8C1' && key <= '\uB9FD') {
                        result.Append('G');
                    }
                    else if (key >= '\uB9FE' && key <= '\uBBF6') {
                        result.Append('H');
                    }
                    else if (key >= '\uBBF7' && key <= '\uBFA5') {
                        result.Append('J');
                    }
                    else if (key >= '\uBFA6' && key <= '\uC0AB') {
                        result.Append('K');
                    }
                    else if (key >= '\uC0AC' && key <= '\uC2E7') {
                        result.Append('L');
                    }
                    else if (key >= '\uC2E8' && key <= '\uC4C2') {
                        result.Append('M');
                    }
                    else if (key >= '\uC4C3' && key <= '\uC5B5') {
                        result.Append('N');
                    }
                    else if (key >= '\uC5B6' && key <= '\uC5BD') {
                        result.Append('O');
                    }
                    else if (key >= '\uC5BE' && key <= '\uC6D9') {
                        result.Append('P');
                    }
                    else if (key >= '\uC6DA' && key <= '\uC8BA') {
                        result.Append('Q');
                    }
                    else if (key >= '\uC8BB' && key <= '\uC8F5') {
                        result.Append('R');
                    }
                    else if (key >= '\uC8F6' && key <= '\uCBF9') {
                        result.Append('S');
                    }
                    else if (key >= '\uCBFA' && key <= '\uCDD9') {
                        result.Append('T');
                    }
                    else if (key >= '\uCDDA' && key <= '\uCEF3') {
                        result.Append('W');
                    }
                    else if (key >= '\uCEF4' && key <= '\uD188') {
                        result.Append('X');
                    }
                    else if (key >= '\uD1B9' && key <= '\uD4D0') {
                        result.Append('Y');
                    }
                    else if (key >= '\uD4D1' && key <= '\uD7F9') {
                        result.Append('Z');
                    }
                    else {
                        result.Append('?');
                    }
                    i += 2;
                }
                #endregion
            }


            return result.ToString();
        }
    }
}
