
using System;
using System.Text;


namespace Vorcyc.PowerLibrary.StringManipulation
{

    /// <summary>
    /// 延续VB6编的CycloneFunction的功能
    /// 属于字符串功能扩展
    /// </summary>
    public static class StringExtension
    {
        public static string[] ToLines(this string text)
        {
            return text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }


        /// <summary>
        /// 返回某个字符在字符串中第一次出现的位置,找不到则返回-1
        /// </summary>
        /// <param name="src">在其中搜索的源串</param>
        /// <param name="target">要获得位置的字符</param>
        /// <returns></returns>
        public static unsafe int GetIndexOf(this string src, char target)
        {
            fixed (char* pSrc = src)
            {
                var ps = pSrc;
                int pos = 0, len = src.Length - 1;

                while (pos <= len)
                {
                    if (target == *ps) return pos;

                    ps++; pos++;
                }

            }
            return -1;

        }


        /// <summary>
        /// 返回某个字符在字符串中第num次出现的位置,找不到则返回-1
        /// </summary>
        /// <param name="src">在其中搜索的源串</param>
        /// <param name="target">要获得位置的字符</param>
        /// <param name="num">第几次出现，最小为1</param>
        /// <returns></returns>
        public static unsafe int GetIndexOf(this string src, char target, int num)
        {
            if (num < 1) throw new ArgumentException("num");

            fixed (char* pSrc = src)
            {
                var ps = pSrc;
                int pos = 0, len = src.Length - 1, nNum = 1;

                while (pos <= len)
                {
                    if (target == *ps)
                    {
                        if (nNum == num) return pos;
                        nNum++;
                    }

                    ps++; pos++;
                }

            }
            return -1;
        }

        //public static int GetIndexOf(string src,string target) 用 String.IndexOf就行,所以不实现了

        public static int GetIndexOf(this string src, string target, int num)
        {
            if (num < 1) throw new ArgumentException("num");
            if (num > GetStringCount(src, target)) return -1;

            int nNum = 1;

            for (int pos = 0; pos <= src.Length - target.Length; pos++)
            {
                //Console.WriteLine(src.Substring (pos, target .Length ));

                if (src.Substring(pos, target.Length) == target)
                {
                    if (nNum == num)
                    {
                        return pos;
                    }
                    nNum++;
                }
            }

            return -1;
        }



        /// <summary>
        /// 返回第一次出现的两个定界字符间的字符串,并包含两个定界字符
        /// </summary>
        /// <param name="src">要查找的源串</param>
        /// <param name="startDelimiter">起始定界字符</param>
        /// <param name="endDelimiter">结束定界字符</param>
        /// <returns></returns>
        public static unsafe string GetStr(this string src, char startDelimiter, char endDelimiter)
        {
            fixed (char* pSrc = src)
            {
                int startPos = GetIndexOf(src, startDelimiter);
                string trailString = GetSubString(src, startPos);
                int endPos = GetIndexOf(trailString, endDelimiter);

                return new string(pSrc, startPos, endPos + 1);
            }
        }


        /// <summary>
        /// 返回第num次出现的两个定界字符间的字符串
        /// </summary>
        /// <param name="src">要查找的源串</param>
        /// <param name="startDelimiter">起始定界字符</param>
        /// <param name="endDelimiter">结束定界字符</param>
        /// <param name="num">第几次出现,最小为1</param>
        /// <param name="includeDelimiter">返回值是否包含2个定界字符</param>
        /// <returns></returns>
        public static string GetStr(this string src, char startDelimiter, char endDelimiter,
            int num, bool includeDelimiter)
        {
            if (num < 1) throw new ArgumentException("num");
            if (num > GetCharCount(src, startDelimiter)) return string.Empty;

            int startPos = includeDelimiter ?
                                    GetIndexOf(src, startDelimiter, num) :
                                    (GetIndexOf(src, startDelimiter, num) + 1);

            string trailString = src.Substring(startPos);

            int endPos = includeDelimiter ?
                                    trailString.IndexOf(endDelimiter) :
                                    (trailString.IndexOf(endDelimiter) - 1);

            return new string(src.ToCharArray(), startPos, endPos + 1);
        }


        /// <summary>
        /// 回第num次出现的一个定界字符串和字符之间的字符串
        /// </summary>
        /// <param name="src">要查找的串源</param>
        /// <param name="startDelimiter">起始定界字符串</param>
        /// <param name="endDelimiter">结束定界字符</param>
        /// <param name="num">第几次出现,最小为1</param>
        /// <param name="includeDelimiter">是否包含定界符</param>
        /// <returns></returns>
        public static string GetStr(this string src, string startDelimiter, char endDelimiter,
            int num, bool includeDelimiter)
        {
            try
            {
                if (num < 1) throw new ArgumentException("num");
                if (num > GetStringCount(src, startDelimiter)) return string.Empty;

                int startPos = includeDelimiter ?
                                        GetIndexOf(src, startDelimiter, num) :
                                        (GetIndexOf(src, startDelimiter, num) + startDelimiter.Length);

                string trialString = src.Substring(startPos);

                int endPos = includeDelimiter ?
                                        trialString.IndexOf(endDelimiter) :
                                        trialString.IndexOf(endDelimiter) - 1;//因为只是一个字符 所以之要减1

                return new string(src.ToCharArray(), startPos, endPos + 1);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }



        /// <summary>
        /// 取子字符串
        /// </summary>
        /// <param name="src">原字符串</param>
        /// <param name="start">起始索引</param>
        /// <returns></returns>
        public static unsafe string GetSubString(this string src, int start)
        {
            fixed (char* pSrc = src)
            {
                var pS = pSrc;
                int pos = 0;

                while (++pos <= start)
                    pS++;//先让指针跑到start处

                return new string(pS);//从start处构造个新个字符串
            }
        }





        /// <summary>
        /// 返回某个字符在字符串中出现了几次
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <param name="find">要活动数量的字符</param>
        /// <returns></returns>
        public static int GetCharCount(this string src, char find)
        {
            int count = 0;

            foreach (char c in src)
                if (c == find) count++;

            return count;
        }




        /// <summary>
        /// 返回某个字符串在另一字符串中出现几次
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <param name="find">要活动数量的字符串</param>
        /// <returns></returns>
        public static int GetStringCount(this string src, string find)
        {
            int count = 0;

            int findLen = find.Length;
            int searchLen = src.Length - findLen;

            for (int i = 0; i <= searchLen; i++)
            {
                if (src.Substring(i, findLen) == find) count++;
            }

            return count;
        }




        /// <summary>
        /// 移除所有空格
        /// </summary>
        /// <param name="src">源字符串</param>
        /// <returns></returns>
        public static string TrimAll(this string src)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < src.Length; i++)
            {
                char current = src[i];
                if (current != ' ')
                    sb.Append(current);
            }

            return sb.ToString();
        }


        /// <summary>
        /// 去除一句英文句子中单词间超过2个的空格
        /// </summary>
        /// <param name="value">源字符串</param>
        /// <returns>返回去除多余空格后的句子</returns>
        public static string MakeStrSingleWhitespace(this string value)
        {

            var srcArray = value.Trim().Split(' ');

            StringBuilder sb = new StringBuilder();

            int aLen = srcArray.Length, aCount = 0;//aCount 数组遍历索引

            foreach (string s in srcArray)
            {
                ++aCount;

                if (s != string.Empty)//如果原来是个空格就会是 Empty
                {
                    sb.Append(s);
                    if (aCount == aLen) break;
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }

    }

}
