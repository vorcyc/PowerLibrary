

using System.Collections.Generic;


namespace Vorcyc.PowerLibrary.Buffer
{
    public class Super
    {

        /// <summary>
        /// 返回一个十进制数字从高位起的每个位所组成的数组.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static byte[] IntegerToBytes(long number)
        {
            var ts = number.ToString();

            List<byte> result = new List<byte>();

            for (int i = 0; i <= ts.Length - 1; i++)
            {
                char c = ts.Substring(i, 1)[0];
                if (char.IsDigit(c))
                    result.Add(System.Convert.ToByte(c.ToString()));

            }
            return result.ToArray();

        }

        /// <summary>
        /// 将一个数字转成英文字母描述的文本
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        ///    Console.WriteLine(Vorcyc.PowerLibrary.Buffer.Super.NumberToText(65536));
        ///    //  =>  Sixty Five Thousands Five Hundreds Thirty Six
        /// </code>
        /// </example>
        public static string NumberToText(long n)
        {
            if (n == 0)
            {
                return "";
            }
            if ((n >= 1) && (n <= 19))
            {
                string[] arr = new string[] {
                "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                "Seventeen", "Eighteen", "Nineteen"
             };
                return (arr[n - 1] + " ");
            }
            if ((n >= 20) && (n <= 99))
            {
                string[] arr = new string[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
                return (arr[(n / 10) - 2] + " " + NumberToText(n % 10));
            }
            if ((n >= 100) && (n <= 199))
            {
                return ("One Hundred " + NumberToText(n % 100));
            }
            if ((n >= 200) && (n <= 999))
            {
                return (NumberToText(n / 100) + "Hundreds " + NumberToText(n % 100));
            }
            if ((n >= 100) && (n <= 1999))
            {
                return ("One Thousand " + NumberToText(n % 1000));
            }
            if ((n >= 2000) && (n <= 999999))
            {
                return (NumberToText(n / 1000) + "Thousands " + NumberToText(n % 1000));
            }
            if ((n >= 1000000) && (n <= 1999999))
            {
                return ("One Million " + NumberToText(n % 1000000));
            }
            if ((n >= 1000000) && (n <= 999999999))
            {
                return (NumberToText(n / 1000000) + "Millions " + NumberToText(n % 1000000));
            }
            if ((n >= 1000000000) && (n <= 1999999999))
            {
                return ("One Billion " + NumberToText(n % 1000000000));
            }
            return (NumberToText(n / 1000000000) + "Billions " + NumberToText(n % 1000000000));
        }



    }
}
