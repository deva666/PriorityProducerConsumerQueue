using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarkoDevcic
{
    public enum Priority
    {
        Idle,
        BelowNormal,
        Normal,
        AboveNormal,
        TimeCritical
    }

    public class ProducerConsumerQueue
    {
        private bool isShutdown = false;
        private readonly object _lock = new object();
        private readonly CancellationTokenSource cancelToken;
        private readonly PriorityQueue<Worker> queue = new PriorityQueue<Worker>();
        private readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        public ProducerConsumerQueue()
        {
            cancelToken = new CancellationTokenSource();
            Task.Factory.StartNew(new Action(DoWork), TaskCreationOptions.LongRunning);
        }

        public Task Enqueue(Action _delegate, Priority priority)
        {
            if (isShutdown)
                throw new InvalidOperationException("Queue is shutdown");

            var worker = new Worker(_delegate, priority);
            lock (_lock)
            {
                queue.Insert(worker);
            }
            waitHandle.Set();
            return worker.TaskCompletionSrc.Task;
        }

        public Task Enqueue(Action _delegate)
        {
            return Enqueue(_delegate, Priority.Normal);
        }

        private void DoWork()
        {
            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    break;
                }

                Worker worker = null;
                lock (_lock)
                {
                    if (queue.Size > 0)
                    {
                        worker = queue.ExtractTopItem();
                    }
                }

                if (worker != null)
                {
                    try
                    {
                        worker.Delegate.Invoke();
                        worker.TaskCompletionSrc.SetResult(null);
                    }
                    catch (Exception fail)
                    {
                        worker.TaskCompletionSrc.SetException(fail);
                    }
                }
                else
                {
                    waitHandle.WaitOne();
                }
            }
        }

        public void Shutdown()
        {
            isShutdown = true;
            cancelToken.Cancel();
            waitHandle.Set();
        }

        private class Worker : IComparable<Worker>
        {
            public Priority Priority { get; private set; }
            public TaskCompletionSource<object> TaskCompletionSrc { get; private set; }
            public Action Delegate { get; private set; }

            public Worker(Action _delegate, Priority priority)
            {
                if (_delegate == null)
                    throw new ArgumentNullException("delegate");

                this.Delegate = _delegate;
                this.Priority = priority;
                this.TaskCompletionSrc = new TaskCompletionSource<object>();
            }

            public int CompareTo(Worker other)
            {
                if (this.Priority > other.Priority)
                    return 1;
                else if (this.Priority < other.Priority)
                    return -1;
                return 0;
            }
        }
    }
}
