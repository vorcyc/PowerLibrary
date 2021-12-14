using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 本命名空间提供语言相关扩展功能
/// </summary>
namespace Vorcyc.PowerLibrary.LanguageExtension
{

    /// <summary>
    /// 循环功能扩展
    /// </summary>
    public static class Loops
    {

        /// <summary>
        /// Traversing a sequence and provide its index and item self.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<int, T> action)
        {
            int index = 0;
            foreach (var item in sequence)
            {
                action?.Invoke(index, item);
                index++;
            }
        }

    }
}
