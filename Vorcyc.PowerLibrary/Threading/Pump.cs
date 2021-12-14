using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vorcyc.PowerLibrary.Threading
{
    internal sealed class Pump
    {


        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PeekMessage(
          out NativeMessage lpMsg,
          HandleRef hWnd, uint wMsgFilterMin,
          uint wMsgFilterMax, uint wRemoveMsg);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpMsg">指向MSG结构的指针，该结构从线程的消息队列里接收消息信息</param>
        /// <param name="hwnd">取得其消息的窗口的句柄。这是一个有特殊含义的值（NULL）。GetMessage为任何属于调用线程的窗口检索消息</param>
        /// <param name="wMsgFilterMin">指定被检索的最小消息值的整数</param>
        /// <param name="wMsgFilterMax">指定被检索的最大消息值的整数</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int GetMessage(
            ref NativeMessage lpMsg,
            int hwnd,
            int wMsgFilterMin,
            int wMsgFilterMax);



        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadId">线程标识</param>
        /// <param name="msg">消息标识符</param>
        /// <param name="wParam">具体由消息决定</param>
        /// <param name="lParam">具体由消息决定</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int PostThreadMessage(
                    int threadId, uint msg,
                    IntPtr wParam, IntPtr lParam);


        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();


        public const uint WM_APP = 0x8000;

        public const uint MessageIsAction = WM_APP + 1;

        private int _threadId;

        private Thread _workerThread;

        private Queue<Action> _queue;

        public Pump()
        {
            _queue = new Queue<Action>(1000);
            _workerThread = new Thread(ThreadCallback);
            _workerThread.IsBackground = true;
            _workerThread.Start();
        }

        //贴线程就可以创建消息队列
        private void ThreadCallback()
        {
            //要获得线程ID，不是托管线程ID                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
            _threadId = GetCurrentThreadId();

            NativeMessage msg = new NativeMessage();

            while (GetMessage(ref msg, 0, 0, 0) != 0)
            {
                if (msg.msg == MessageIsAction)
                {
                    while (_queue.Count > 0)
                    {
                        var action = _queue.Dequeue();
                        action?.Invoke();
                    }
                }
                else
                {
                 
                }
            }
        }




        public void Post(uint message)
        {
            PostThreadMessage(_threadId, message, IntPtr.Zero, IntPtr.Zero);
        }

        public void Post(Action action)
        {
            _queue.Enqueue(action);
            PostThreadMessage(_threadId, MessageIsAction, IntPtr.Zero, IntPtr.Zero);
        }

    }
}
