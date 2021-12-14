/// <summary>
/// 本命名空间提供一些线程相关类型
/// </summary>
namespace Vorcyc.PowerLibrary.Threading
{
    using System;
    using System.Threading;


    /// <summary>
    /// A simple loop callback with another thread.
    /// </summary>
    public sealed class Looper
    {

        private Thread _loopThread;


        private volatile bool _isWorking = false;

        private Action _procedure;

        private Func<bool> _loopCondition;


        private bool _isUseWaitingLock = false;

        private AutoResetEvent _waitingLock;


        public Action Procedure
        {
            get { return _procedure; }
            set
            {
                if (_isWorking) return;//运行起来就不能再改了
                _procedure = value;
            }
        }


        public Action ForeProcedure { get; set; }

        public Action BehindProcedure { get; set; }


        public Func<bool> LoopCondition
        {
            get { return _loopCondition; }
            set
            {
                if (_isWorking) return;
                _loopCondition = value;
            }
        }

        /// <summary>
        /// 若使用，则阻塞调用线程
        /// </summary>
        public bool IsUseWaitLock
        {
            get { return _isUseWaitingLock; }
            set
            {
                if (_isWorking) return;
                _isUseWaitingLock = value;
            }
        }


        public void Run()
        {

            if (_procedure == null)
                throw new InvalidOperationException("procedure cannot be null.");

            if (_loopCondition == null)
                throw new InvalidOperationException("condition cannot be null.");

            _loopThread = new Thread(
                () =>
                {

                    ForeProcedure?.Invoke();

                    while (_loopCondition())
                    {
                        _procedure();
                    }

                    BehindProcedure?.Invoke();

                    if (_isUseWaitingLock)
                    {
                        _waitingLock.Set();
                        ReleaseAre();
                    }

                });
            _loopThread.Name = "Looper Thread";
            _loopThread.IsBackground = false;
            _loopThread.Priority = ThreadPriority.Highest;
            _isWorking = true;
            _loopThread.Start();


            if (_isUseWaitingLock)
            {
                ReleaseAre();
                _waitingLock = new AutoResetEvent(false);
                _waitingLock.WaitOne();
            }

        }



        private void ReleaseAre()
        {
            if (_waitingLock != null)
            {
                _waitingLock.Dispose();
                _waitingLock = null;
            }
        }

        public void Stop()
        {
            _isWorking = false;
            ReleaseAre();
            if (_loopThread != null)
                _loopThread.Abort();
            _loopThread = null;
        }

    }
}
