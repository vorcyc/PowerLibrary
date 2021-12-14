

using System;
using System.Text;
using System.Collections.Generic;

namespace Vorcyc.PowerLibrary.Buffer
{
    internal class BufferOperation
    {


        public unsafe static byte[] GetInnerBuffer(byte[] src, int start, int length)
        {
            if (src == null)
                throw new NullReferenceException();
            if (start < 0 || start > src.Length)
                throw new ArgumentException("start");
            /*下行本来是这样 if (length > src.Length||length <=0),
             * 既然循环那点那样灵活了,这里也放宽些限制.可以传入任何值，
             * 至于是否有效么又再判断
            */
            if (length <= 0)
                throw new ArgumentException("length");

            int arrayBound = System.Math.Min(src.Length - start, length);//src.Length-start是从start到整个数组完的元素数
            byte[] result = new byte[arrayBound];

            //按long,一次复制8个byte
            fixed (byte* pSrc = src, pDst = result)
            {
                byte* ps = pSrc + start;//源的起始值是要加上start的

                long* lpSrc = (long*)ps;
                long* lpDst = (long*)pDst;

                for (int i = 0; i < System.Math.Min(src.Length, length); i += 8)
                {
                    *(lpDst) = *(lpSrc);
                    lpSrc++;
                    lpDst++;
                }

            }
            return result;
        }


        /// <summary>
        /// 最初呢概念，10.3.12写了新版本，请用GetInnerBuffer代替
        /// </summary>
        /// <param name="srcArray"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [Obsolete]
        public static byte[] GetInnerBuffer_idea(byte[] srcArray, int startIndex, int length)
        {
            byte[] result = new byte[length];//在VB中则是length-1 ，因为定义的是数组上限，而C中定义的是容量
            int start = startIndex, current = 0;

            while (current < length)
            {
                result[current] = srcArray[start];
                current++;
                start++;
            }
            return result;
        }

        //[System.Runtime.CompilerServices.MethodImpl (MethodImplOptions.InternalCall )]
        //public static extern byte[] GetInnerArray2(this byte[] srcArray, int startIndex, int length);


        //internal static unsafe byte[] GetInnerArray2Impl(this byte[] srcArray, int startIndex, int length)
        //{
        //    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();

        //    byte[] result = new byte[length];//在VB中则是length-1 ，因为定义的是数组上限，而C中定义的是容量
        //    int l = startIndex, i = 0;



        //    fixed (byte* pSrc = srcArray)
        //    {
        //        while (i < length)
        //        {
        //            result[i] =  *(pSrc +l);
        //            i++;
        //            l++;
        //        }


        //    }
        //    sw.Stop();
        //    Console.WriteLine(sw.ElapsedTicks);

        //    return result;
        //}




        // 下面的示例输出斐波那契数列的 100 个数字，其中每个数字都是前两个数字之和。
        //在代码中，大小足够容纳 100 个 int 类型元素的内存块是在堆栈上分配的，而不是在堆上分配的。
        //该块的地址存储在 fib 指针中。此内存不受垃圾回收的制约，因此不必将其钉住（通过使用 fixed）。
        //内存块的生存期受限于定义它的方法的生存期。不能在方法返回之前释放内存。
        //stackalloc 仅在局部变量的初始值设定项中有效。
        //由于涉及指针类型，因此 stackalloc 要求不安全上下文。有关更多信息，请参见 不安全代码和指针（C# 编程指南）。
        //stackalloc 类似于 C 运行时中的 _alloca。

        private unsafe static void xx()
        {

            int* fib = stackalloc int[100];
            int* p = fib;
            //让指针先前进2个 int32,8字节
            //并且都设置为1，为算数列做初始化
            *p++ = *p++ = 1;

            for (int i = 2; i < 100; ++i, ++p)
                *p = p[-1] + p[-2];
            for (int i = 0; i < 10; ++i)
                Console.WriteLine(fib[i]);


            Console.Read();
        }




        public unsafe static byte[] CombineArray(byte[] leadingArray, byte[] followingArray)
        {
            int laLen = leadingArray.Length;
            int faLen = followingArray.Length;

            byte[] result = new byte[laLen + faLen];

            fixed (byte* pla = leadingArray, pfa = followingArray, pResult = result)
            {
                for (int i = 0; i < laLen; i++)
                {
                    pResult[i] = pla[i];
                }

                for (int i = 0; i < faLen; i++)
                {
                    pResult[i + laLen] = pfa[i];
                }
            }

            return result;
        }

        public static unsafe void Copy(byte[] src, int srcIndex, byte[] dst, int dstIndex, int count)
        {
            if (src == null || srcIndex < 0 ||
                dst == null || dstIndex < 0 || count < 0)
            {
                throw new System.ArgumentException();
            }

            int srcLen = src.Length;
            int dstLen = dst.Length;
            if (srcLen - srcIndex < count || dstLen - dstIndex < count)
            {
                throw new System.ArgumentException();
            }

            // The following fixed statement pins the location of the src and dst objects
            // in memory so that they will not be moved by garbage collection.
            fixed (byte* pSrc = src, pDst = dst)
            {
                byte* ps = pSrc;
                byte* pd = pDst;

                // Loop over the count in blocks of 4 bytes, copying an integer (4 bytes) at a time:
                for (int i = 0; i < count / 4; i++)
                {
                    *((int*)pd) = *((int*)ps);
                    pd += 4;
                    ps += 4;
                }

                // Complete the copy by moving any bytes that weren't moved in blocks of 4:
                for (int i = 0; i < count % 4; i++)
                {
                    *pd = *ps;
                    pd++;
                    ps++;
                }
            }


        }

        public static string BytesToString(byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(string.Format("{0:X2}", data[i]));
            }
            return builder.ToString();
        }

        public static byte[] StringToBytes(string data)
        {
            var result = new List<byte>();
            for (int i = 0; i <= data.Length - 1; i += 2)
            {
                var t = data.Substring(i, 2);
                result.Add(byte.Parse(t, System.Globalization.NumberStyles.HexNumber));
            }
            return result.ToArray();
        }


    }
}
