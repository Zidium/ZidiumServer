using System;
using System.Collections.Generic;
using System.Threading;

namespace Zidium.Api
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

        public ThreadTask Add(ThreadStart threadStart, string name = null)
        {
            ThreadTask item = new ThreadTask(threadStart, name);
            AddTask(item);
            return item;
        }

        public ThreadTask Add(ParameterizedThreadStart threadStart, object obj)
        {
            ThreadTask item = new ThreadTask(threadStart, obj);
            AddTask(item);
            return item;
        }

        public ThreadTask Add<T>(ThreadTaskAction<T> action, T obj, string name = null)
        {
            ThreadStart newThreadStart = delegate() { action.Invoke(obj); };
            return Add(newThreadStart, name);
        }

        public void AddTask(ThreadTask task)
        {
            lock (SyncRoot)
            {
                if (HasFreeThreads)
                {
                    BeginTask(task);
                }
                else
                {
                    _queue.Enqueue(task);
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
            _allTaskCompleted.WaitOne();
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
            var thread = new Thread(() => ExecuteTask(item))
            {
                Name = item.Name
            };
            thread.Start();
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
    }
}