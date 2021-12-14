

using System;

/// <summary>
/// 本命名空间提供一些数学方法
/// </summary>
namespace Vorcyc.PowerLibrary.MathExtension
{
    public class Conveter
    {

        /// <summary>
        /// 转换一个正整数到2至36进制的字符串
        /// </summary>
        /// <param name="number">正整数</param>
        /// <param name="baseNum">进制数</param>
        /// <returns>字符串形式的进制数</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <example>
        /// 以下代码演示如何使用本方法进行进制转换：
        /// <code>
        /// var r = Vorcyc.PowerLibrary.MathExtension.Conveter.ToAnyBase(100, 16);
        /// Console.WriteLine(r);   // => 64
        /// r = Vorcyc.PowerLibrary.MathExtension.Conveter.ToAnyBase(100, 8);
        /// Console.WriteLine(r);   //  =>  144 
        /// r = Vorcyc.PowerLibrary.MathExtension.Conveter.ToAnyBase(100, 2);
        /// Console.WriteLine(r);   //  =>  1100100
        /// r = Vorcyc.PowerLibrary.MathExtension.Conveter.ToAnyBase(19, 20);
        /// Console.WriteLine(r);   //  =>  J
        /// r = Vorcyc.PowerLibrary.MathExtension.Conveter.ToAnyBase(36, 36);
        /// Console.WriteLine(r);   //  =>  10
        /// </code>
        /// </example>
        public static string ToAnyBase(long number, short baseNum)
        {
            int digitValue;
            System.Text.StringBuilder res = new System.Text.StringBuilder();

            const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            //检查进制数和源数
            if (number < 0)
                throw new ArgumentOutOfRangeException("值必须为正");
            else if (baseNum < 2 || baseNum > 36)
                throw new ArgumentOutOfRangeException("基数必须在2到36间");


            while (number > 0L) {
                digitValue = (int)(number % ((long)baseNum));
                number /= (long)baseNum;
                res.Insert(0, digits[digitValue]);
            }

            return res.ToString();

        }


    }
}
