using System.Collections.Concurrent;

namespace MachineStream
{
    public abstract class AbstractSingleThreadWorkerWithQueue<T> : AbstractSingleThreadWorker
    {
        private readonly ILogger<AbstractSingleThreadWorkerWithQueue<T>> _logger;

        protected AutoResetEvent _eventTask = new AutoResetEvent(false);

        protected ConcurrentQueue<T> _queueTask = new ConcurrentQueue<T>();
        public ConcurrentQueue<T> QueueTask
        {
            get { return _queueTask; }
        }

        protected AbstractSingleThreadWorkerWithQueue(ILogger<AbstractSingleThreadWorkerWithQueue<T>> logger)
            : base(logger)
        {
            _logger = logger;
        }

        #region Init/Shutdown

        public override void ShutdownSystem(TimeSpan timeSpan)
        {
            IsWorking = false;
            _eventTask.Set();
            base.ShutdownSystem(timeSpan);
        }

        #endregion

        #region Public methods

        public virtual async Task AddTask(T task)
        {
            QueueTask.Enqueue(task);
            _eventTask.Set();
            await Task.CompletedTask;
        }

        #endregion

        #region DoWork method

        protected override void DoWork()
        {
            while (IsWorking && !AbstractSingleThreadWorker.IsGlobalExiting)
            {
                if (QueueTask.Count > 0)
                {
                    try
                    {
                        T task;
                        QueueTask.TryDequeue(out task);

                        if (task != null)
                            HandleTask(task);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("AbstractSingleThreadWorkerWithQueue() {0}", ex.Message);
                    }
                    SleepAfterDoWork();
                }
                else
                    _eventTask.WaitOne();
            }
        }

        protected abstract void HandleTask(T task);

        #endregion
    }
}
