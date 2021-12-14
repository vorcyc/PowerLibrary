namespace Vorcyc.PowerLibrary.StringManipulation
{

    using System.Collections.Generic;

    /// <summary>
    /// 字符串匹配功能扩展
    /// </summary>
    public static class StringPairExtension
    {


        //用不安全的代码并不能优化

        /// <summary>
        /// 返回一个字符串中以 <see cref="char"/> 为起始和结束定界符的第一次出现的子字符串。
        /// </summary>
        /// <param name="text">输入字符串</param>
        /// <param name="begin">起始定界符</param>
        /// <param name="end">结束定界符</param>
        /// <returns>若找到起始和结束定界符则返回子<see cref="StringPair"/>，否则返回 null </returns>
        /// <example>
        /// <code>
        /// var s = "&lt;code&gt;code1&lt;/code&gt;&lt;code&gt;code2&lt;/code&gt;";
        /// var pair2 = s.GetPair('&lt;', '&gt;');
        /// Console.WriteLine(pair2);
        /// //  =>
        /// //  BeginInex: 0
        /// //  EndIndex: 5
        /// //  Content: code
        /// </code>
        /// </example>
        public static StringPair GetPair(this string text, char begin, char end)
        {
            var beginIndex = text.IndexOf(begin);
            if (beginIndex == -1) return null;

            var behindStr = text.Substring(beginIndex + 1);

            var endShortIndex = behindStr.IndexOf(end);
            if (endShortIndex == -1) return null;

            return new StringPair(beginIndex, beginIndex + endShortIndex + 1, behindStr.Substring(0, endShortIndex));
        }



        /// <summary>
        /// 返回一个字符串中以 <see cref="string"/> 为起始和结束定界符的第一次出现的子字符串。
        /// </summary>
        /// <param name="text">输入字符串</param>
        /// <param name="begin">起始定界符</param>
        /// <param name="end">结束定界符</param>
        /// <returns>若找到起始和结束定界符则返回子<see cref="StringPair"/>，否则返回 null </returns>
        /// <example>
        /// <code>
        /// var s = "&lt;code&gt;code1&lt;/code&gt;&lt;code&gt;code2&lt;/code&gt;";
        /// var pair1 = s.GetPair("&lt;code&gt;", "&lt;/code&gt;");
        /// Console.WriteLine(pair1);
        /// //  =>
        /// //  BeginInex : 0
        /// //  EndIndex: 11
        /// //  Content: code1
        /// </code>
        /// </example>
        public static StringPair GetPair(this string text, string begin, string end)
        {
            var beginIndex = text.IndexOf(begin);
            if (beginIndex == -1) return null;

            var behindStr = text.Substring(beginIndex + begin.Length);

            var endShortIndex = behindStr.IndexOf(end);
            if (endShortIndex == -1) return null;

            return new StringPair(beginIndex, beginIndex + endShortIndex + begin.Length, behindStr.Substring(0, endShortIndex));
        }


        /// <summary>
        /// 返回一个字符串中以 <see cref="System.Char"/> 为起始和结束定界符的多组子字符串。
        /// </summary>
        /// <param name="text">输入字符串</param>
        /// <param name="beginDelimiter">起始定界符</param>
        /// <param name="endDelimiter">结束定界符</param>
        /// <returns>返回0或多个子<see cref="StringPair"/>的<see cref="System.Collections.Generic.IList{T}"/>形式</returns>
        /// <example>
        /// <code>
        /// var s = "&lt;code&gt;code1&lt;/code&gt;&lt;code&gt;code2&lt;/code&gt;";
        /// var pairs1 = s.GetPairs("&lt;code&gt;", "&lt;/code&gt;");
        /// foreach (var p in pairs1)
        ///     Console.WriteLine(p);
        ///  =>
        ///  BeginInex: 0
        ///  EndIndex: 11
        ///  Content: code1
        ///
        ///  BeginInex : 18
        ///  EndIndex: 29
        ///  Content: code2
        /// </code>
        /// </example>
        public static IList<StringPair> GetPairs(this string text, char beginDelimiter, char endDelimiter)
        {
            var result = new List<StringPair>();

            int len = text.Length - 1;
            int currentPos = 0;

            while (currentPos <= len) {

                var pair = GetPair(text.Substring(currentPos), beginDelimiter, endDelimiter);

                if (pair != null) {
                    result.Add(pair);
                    currentPos += pair.EndIndex;
                }
                else {
                    break;
                }
            }
            return result;
        }


        /// <summary>
        /// 返回一个字符串中以 <see cref="System.String"/> 为起始和结束定界符的多组子字符串
        /// </summary>
        /// <param name="text">输入字符串</param>
        /// <param name="beginDelimiter">起始定界符</param>
        /// <param name="endDelimiter">结束定界符</param>
        /// <returns>返回0或多个子<see cref="StringPair"/>的<see cref="System.Collections.Generic.IList{T}"/>形式</returns>
        /// <example>
        /// <code>
        /// var pairs2 = s.GetPairs('&lt;', '&gt;');
        /// foreach (var p in pairs2)
        ///     Console.WriteLine(p);
        ///  =>
        ///  BeginInex: 0
        ///  EndIndex: 5
        ///  Content: code
        ///
        ///  BeginInex : 6
        ///  EndIndex: 12
        ///  Content: / code
        ///
        ///  BeginInex: 1
        ///  EndIndex: 6
        ///  Content: code
        ///
        ///  BeginInex : 6
        ///  EndIndex: 12
        ///  Content: / code
        /// </code>
        /// </example>
        public static IList<StringPair> GetPairs(this string text, string beginDelimiter, string endDelimiter)
        {
            var result = new List<StringPair>();

            int len = text.Length - 1;
            int currentPos = 0;

            while (currentPos <= len) {
                var sb = text.Substring(currentPos);
                var pair = GetPair(sb, beginDelimiter, endDelimiter);

                if (pair != null) {
                    result.Add(new StringPair(pair.BeginIndex + currentPos, pair.EndIndex + currentPos, pair.Content));
                    currentPos += pair.EndIndex + endDelimiter.Length;
                }
                else {
                    break;
                }
            }
            return result;
        }


    }

}
