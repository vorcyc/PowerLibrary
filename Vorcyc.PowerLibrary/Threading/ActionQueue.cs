using System;
using System.Collections.Generic;

namespace Vorcyc.PowerLibrary.Threading
{
    using System.Threading;

    public sealed class ActionQueue
    {

        private Queue<Delegate> _queue;

        private Thread _workerThread;

        private AutoResetEvent _resetEvent;

        private bool _running = false;

        public ActionQueue()
        {
            _queue = new Queue<Delegate>(10);
            _resetEvent = new AutoResetEvent(false);

        }

        public void Run()
        {
            _running = true;
            _workerThread = new Thread(ThreadProc);
            _workerThread.IsBackground = true;
            _workerThread.Start();
        }

        private void ThreadProc()
        {
#if debug
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
#endif
            while (_running)
            {
                while (_queue.Count > 0)
                {
                    var item = _queue.Dequeue();
                    if (item is Action action)
                    {
                        action?.Invoke();
                    }
#if debug
                    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
#endif
                }
                _resetEvent.WaitOne();
            }
        }


        public void Post(Action action)
        {
            _queue.Enqueue(action);
            _resetEvent.Set();
        }


        public void Stop()
        {
            _running = false;
            _resetEvent.Set();
            _workerThread.Abort();
        }

    }
}
