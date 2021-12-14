

namespace Vorcyc.PowerLibrary.Threading
{

    using System;
    using System.Threading;

    public sealed class ThreadTimer
        : IDisposable, IThreadTimer
    {

        public delegate void TimerCallbackDelegate();


        private Thread _workerThread;
        private TimerCallbackDelegate _tcb;
        private volatile TimerState _state;
        private AutoResetEvent _are = new AutoResetEvent(false);

        private object objLock = new object();

        /// <summary>
        /// 构建新的线程时序器，无间隔时间
        /// </summary>
        /// <param name="timerCB">时序回调</param>
        public ThreadTimer(TimerCallbackDelegate timerCB)
            : this(timerCB, 0)
        { }

        /// <summary>
        /// 构建新的线程时序器
        /// </summary>
        /// <param name="timerCb">时序回调</param>
        /// <param name="interval">间隔时间（单位：毫秒）</param>
        public ThreadTimer(TimerCallbackDelegate timerCb, int interval)
        {
            this._workerThread = null;
            this._state = TimerState.Stopped;
            this._tcb = timerCb;
            this.Interval = interval;
        }

        /// <summary>
        /// 开始新的时序或恢复暂停的时序
        /// </summary>
        public void Start()
        {
            lock (objLock)
            {
                if (_state == TimerState.Stopped)
                {
                    _workerThread = new Thread(ThreadProc);
                    _workerThread.Priority = ThreadPriority.Normal;
                    _state = TimerState.Running;
                    _workerThread.Start();
                }
                else if (_state == TimerState.Paused)
                {
                    _state = TimerState.Running;//难道改哈这2个执行顺序就可以解决暂停后无法启动你呢问题？
                    _are.Set();
                }
                Console.WriteLine(_state);
            }
        }

        /// <summary>
        /// 暂停时序
        /// </summary>
        public void Pause()
        {
            lock (objLock)
                _state = TimerState.Paused;
        }

        /// <summary>
        /// 停止时序，释放时序线程
        /// </summary>
        public void Stop()
        {
            lock (objLock)
            {
                _state = TimerState.Stopped;
                if (_workerThread != null)
                {
                    _workerThread.Abort();
                    _workerThread = null;
                    _are.Reset();
                }
            }
        }


        private void ThreadProc()
        {
            while (_state != TimerState.Stopped)
            {
                if (_state == TimerState.Running)
                {
                    this._tcb();
                    if (Interval > 0)
                        Thread.Sleep(Interval);
                }
                else if (_state == TimerState.Paused)
                {
                    _are.WaitOne();
                }
            }
        }

        /// <summary>
        /// 时序间隔 毫秒
        /// </summary>
        public int Interval { get; set; }


        #region dispose
        private bool disposedValue = false;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposedValue && disposing)
            {
                this.Stop();
                this._tcb = null;
            }
            this.disposedValue = true;
        }

        ~ThreadTimer()
        {
            this.Dispose(true);
        }

        #endregion

    }


    /// <summary>
    /// 基于线程的时序器
    /// </summary>
    public interface IThreadTimer
    {
        /// <summary>
        /// 启动
        /// </summary>
        void Start();
        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
        /// <summary>
        /// 间隔时间，单位 : 毫秒
        /// </summary>
        int Interval { get; set; }
    }


    /// <summary>
    /// 时序器状态
    /// </summary>
    public enum TimerState
    {
        /// <summary>
        /// 运行中
        /// </summary>
        Running,
        /// <summary>
        /// 暂停中
        /// </summary>
        Paused,
        /// <summary>
        /// 停止了
        /// </summary>
        Stopped,
    }

}
