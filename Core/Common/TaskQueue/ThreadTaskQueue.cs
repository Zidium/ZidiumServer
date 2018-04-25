using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Zidium.Core.Common
{
    public class ThreadTaskQueue
    {
        private int _maxParallelTasks;

        private int _runTasks;

        private readonly Queue<ThreadTask> _queue = new Queue<ThreadTask>();

        private readonly ManualResetEvent _allTaskCompleted = new ManualResetEvent(true);
        
        public ThreadTaskQueue(int maxParallelTasks)
        {
            if (maxParallelTasks < 1)
            {
                throw new ArgumentException("maxParallelTasks < 1");
            }
            _maxParallelTasks = maxParallelTasks;
        }

        public int MaxParallelTasks
        {
            get { return _maxParallelTasks; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("value < 1");
                }
                _maxParallelTasks = value;
                TryBeginWaitTask();
            }
        }

        public int RunTasks
        {
            get { return _runTasks; }
        }

        public int WaitTasks
        {
            get { return _queue.Count; }
        }

        public int AllTasks
        {
            get { return _runTasks + _queue.Count; }
        }

        public ThreadTask Add(ThreadStart threadStart)
        {
            ThreadTask item = new ThreadTask(threadStart);
            AddTask(item);
            return item;
        }

        public ThreadTask Add(ParameterizedThreadStart threadStart, object obj)
        {
            ThreadTask item = new ThreadTask(obj, threadStart);
            AddTask(item);
            return item;
        }

        public ThreadTask Add<T>(ThreadTaskAction<T> action, T obj)
        {
            ThreadStart newThreadStart = delegate() { action.Invoke(obj); };
            return Add(newThreadStart);
        }

        public void AddTask(ThreadTask item)
        {
            lock (SyncRoot)
            {
                if (HasFreeThreads)
                {
                    BeginTask(item);
                }
                else
                {
                    _queue.Enqueue(item);
                }
                _allTaskCompleted.Reset();
            }
        }

        public void AddTasks(List<ThreadTask> items)
        {
            lock (SyncRoot)
            {
                foreach (var item in items)
                {
                    AddTask(item);
                }
            }
        }

        public void WaitForAllTasksCompleted()
        {
            try
            {
                _allTaskCompleted.WaitOne();
            }
            catch (ThreadAbortException)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Есть свободные потоки для выполнения задач
        /// </summary>
        public bool HasFreeThreads
        {
            get { return _runTasks < _maxParallelTasks; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool UseSystemThreadPool { get; set; }

        protected void BeginTask(ThreadTask item)
        {
            if (UseSystemThreadPool)
            {
                BeginTaskBySystemThreadPool(item);
            }
            else
            {
                BeginTaskByPersonalThread(item);
            }
            _runTasks++;
        }

        protected void BeginTaskBySystemThreadPool(ThreadTask item)
        {
            ThreadPool.QueueUserWorkItem(ExecuteTask, item);
        }

        protected void BeginTaskByPersonalThread(ThreadTask item)
        {
            var thread = new Thread(ExecuteTask);
            thread.Start(item);
        }

        /// <summary>
        /// Проверяет можно ли запустить на выполнение задачи, которые ожидают своего выполнения.
        /// Если есть ожидающие задачи и есть свободные потоки, запускает задачи на выполнение.
        /// </summary>
        protected void TryBeginWaitTask()
        {
            lock (SyncRoot)
            {
                while (HasFreeThreads && WaitTasks > 0)
                {
                    var newItem = _queue.Dequeue();
                    BeginTask(newItem);
                }
            }
        }

        protected void ExecuteTask(object item)
        {
            ThreadTask task = item as ThreadTask;
            try
            {
                task.Execute();
            }
            catch (Exception exception)
            {
                task.Exception = exception;
            }
            finally
            {
                lock (SyncRoot)
                {
                    _runTasks--;
                    if (AllTasks == 0)
                    {
                        _allTaskCompleted.Set();
                    }
                    else
                    {
                        TryBeginWaitTask();
                    }
                }
            }
        }

        public void BeginForEach<TSource>(IEnumerable<TSource> sourceList, Action<TSource> body)
        {
            foreach (var source in sourceList)
            {
                var source2 = source;
                Add(() => body(source2));
            }
        }

        public void ForEach<TSource>(IEnumerable<TSource> sourceList, Action<TSource> body)
        {
            if (sourceList != null && sourceList.Count() > 0)
            {
                BeginForEach(sourceList, body);
                WaitForAllTasksCompleted();
            }
        }
    }
}