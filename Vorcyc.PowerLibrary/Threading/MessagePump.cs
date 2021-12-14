/* 18.3.8
 * Vorcyc
 * cyclone_dll
 */

namespace Vorcyc.PowerLibrary.Threading
{
    using System;
    using System.Threading;
    using System.Collections.Concurrent;

    /// <summary>
    /// The callback function of <see cref="MessagePump{TMessageBody}"/>
    /// </summary>
    /// <typeparam name="TMessageBody">The type of parameter objects</typeparam>
    /// <param name="messageCode">The message code definded by your own system</param>
    /// <param name="messageBody">A user-definded object that to pass though.</param>
    public delegate void MessagePumpHandlerDelegate<TMessageBody>(int messageCode, TMessageBody messageBody) where TMessageBody : class, new();

    /// <summary>
    /// 实现一个线程消息队列
    /// Represents a message queue。
    /// <para>很多时候需要在一个线程上顺序的完成工作，以防止跨线程冲突。本类型可以提供高效单线程工作机制，从而简化线程的开发工作。</para>
    /// </summary>
    /// <typeparam name="TMessageBody">消息体</typeparam>
    /// <example>
    /// 以下代码演示如何使用消息泵
    /// <code>
    /// using Vorcyc.PowerLibrary.Threading;
    ///
    /// //定义消息体，可以根据你的需求自定义
    /// public sealed class MessageBody
    /// {
    ///    //消息名称
    ///    public string Name { get; set; }
    ///    //消息时间
    ///    public DateTime Time { get; set; }
    /// }
    ///
    /// public static class ThreadingTest
    /// {
    ///    public static void MsgPumpTest()
    ///    {
    ///        var MsgPump = new MessagePump&lt;MessageBody&gt;(Handler);//实例化消息泵，指定消息体，指定回调方法
    ///        MsgPump.Start();//启动消息泵
    ///
    ///        //指定消息码，该码用作特定消息的唯一值。
    ///        //发送异步消息至消息泵，该方法不会阻止调用线程。
    ///        MsgPump.Post(1, new MessageBody { Name = "post", Time = DateTime.Now });
    ///        //发送同步消息至消息泵，该方法会阻止调用线程直到本消息被处理。
    ///        MsgPump.Send(2, new MessageBody { Name = "send", Time = DateTime.Now });
    ///        
    ///        //结束消息泵，并释放资源
    ///        MsgPump.Close();
    ///    }
    ///
    ///    public static void Handler(int messageCode, MessageBody body)
    ///    {
    ///        Console.WriteLine($"{body.Time}  {body.Name} |  {messageCode}");
    ///    }
    /// }
    /// </code>
    /// </example>
    public sealed class MessagePump<TMessageBody> where TMessageBody : class, new()
    {

        /// <summary>
        /// 内部使用
        /// </summary>
        /// <typeparam name="TMsgBody"></typeparam>
        private class Message<TMsgBody>
        {
            public int messageCode;
            public TMsgBody body;
            public volatile bool isSyncMessage = false;

            public override string ToString()
                => $"Message : Code:{messageCode} , Argument objects:{body} , IsSyncMessage:{isSyncMessage}";
        }


        private Thread _messageProcessor;

        private ConcurrentQueue<Message<TMessageBody>> _messages;

        private volatile bool _running = false;

        private MessagePumpHandlerDelegate<TMessageBody> _handler;

        //用于让执行线程休息
        private AutoResetEvent _procWaitEvent;

        private volatile int _syncMessageCount = 0;

        //Send()的时候用，用于阻止调用线程
        private AutoResetEvent _syncMessageWaitEvent;

        //不能用CountdownEvent，它只往下计数。且在初始化计数为0的时候就无法增加计数。

        private volatile int _queueLength = 0;

        /// <summary>
        /// Creates an instance of <see cref="MessagePump{T}"/>
        /// </summary>
        /// <param name="handler">The callback function used to handle the message loop.</param>
        public MessagePump(MessagePumpHandlerDelegate<TMessageBody> handler)
        {
            //18.4.24 加个检查
            /*该语法类似于
             * if (handler == null) throw new ArgumentNullException(nameof(handler));
             * _handler = handler;
             */
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _procWaitEvent = new AutoResetEvent(false);
            _syncMessageWaitEvent = new AutoResetEvent(false);
        }


        /// <summary>
        /// Start the message loop.
        /// </summary>
        public void Start()
        {
            //18.4.24从构造器中移到这里，并在Stop()中加置空
            _messages = new ConcurrentQueue<Message<TMessageBody>>();

            _messageProcessor = new Thread(ThreadProc)
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true,
                Name = "Message Pump Thread"
            };
            _running = true;
            _messageProcessor.Start();
        }


        private void ThreadProc()
        {
            while (_running) {
                while (_messages.Count > 0) {
                    if (_messages.TryDequeue(out Message<TMessageBody> message)) {
                        Interlocked.Decrement(ref _queueLength);

                        //var message = _messages.Dequeue();
                        _handler?.Invoke(
                            message.messageCode,
                            message.body);


                        if (message.isSyncMessage) {
                            //_syncMessageCount--;
                            Interlocked.Decrement(ref _syncMessageCount);
                            if (_syncMessageCount == 0) _syncMessageWaitEvent.Set();
                        }
                    }
                }

                _procWaitEvent.WaitOne();
            }
        }



        /// <summary>
        /// Dispatches an asynchronous message to <see cref="MessagePump{TMessageBody}"/>
        /// </summary>
        public void Post(int messageCode, TMessageBody body)
        {
            _messages.Enqueue(new Message<TMessageBody>
            {
                messageCode = messageCode,
                body = body,
                isSyncMessage = false,
            });

            Interlocked.Increment(ref _queueLength);
            _procWaitEvent.Set();
        }


        /// <summary>
        /// Dispatches an synchronous message to <see cref="MessagePump{TMessageBody}"/>.
        /// This method blocks the caller thread until the message been processed.
        /// </summary>
        public void Send(int messageCode, TMessageBody body)
        {
            _messages.Enqueue(new Message<TMessageBody>
            {
                messageCode = messageCode,
                body = body,
                isSyncMessage = true,
            });

            Interlocked.Increment(ref _queueLength);
            Interlocked.Increment(ref _syncMessageCount);

            _procWaitEvent.Set();
            _syncMessageWaitEvent.WaitOne();
        }

        /// <summary>
        /// Gets the length of message queue.
        /// </summary>
        public int QueueLength => _queueLength;

        /// <summary>
        /// Stop the message loop, reset the states of current <see cref="MessagePump{TMessageBody}"/>.
        /// </summary>
        public void Stop()
        {
            _running = false;
            _messageProcessor.Abort();
            //_messages.Clear();
            _messages = null;
            _syncMessageCount = 0;
            _queueLength = 0;
        }

        /// <summary>
        /// Do <see cref="Stop"/>, then release all resources held by the current <see cref="MessagePump{TMessageBody}"/>.
        /// </summary>
        public void Close()
        {
            Stop();
            _procWaitEvent.Close();
            _syncMessageWaitEvent.Close();
        }
    }
}
