namespace Vorcyc.PowerLibrary.StringManipulation
{
    /// <summary>
    /// 表示一个字符串的子区域
    /// </summary>
    public sealed class StringPair
    {

        internal StringPair(int beginIndex, int endIndex, string content)
        {
            BeginIndex = beginIndex;
            EndIndex = endIndex;
            Content = content;
        }

        /// <summary>
        /// 表示该子区域在母字符串中的0基起始索引。
        /// </summary>
        public int BeginIndex {
            get;
        }

        /// <summary>
        /// 表示该子区域在母字符串中的0基结束索引。
        /// </summary>
        public int EndIndex {
            get;
        }

        /// <summary>
        /// 表示该子区域的在母字符串中的内容。
        /// </summary>
        public string Content {
            get;
        }

        /// <summary>
        /// 已重载。输出 BeginIndex , EndIndex , Content属性。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("BeginIndex : {0}\nEndIndex : {1}\nContent : {2}\n", BeginIndex, EndIndex, Content);
        }
    }
}
