namespace MachineStream
{
    public abstract class AbstractSingleThreadWorker
    {
        private readonly ILogger<AbstractSingleThreadWorker> _logger;

        #region Fields and properties

        [JsonIgnore]
        public abstract string WorkerName { get; }

        [JsonIgnore]
        public bool IsWorking { get; protected set; }

        protected Thread _threadWorker = null;

        #endregion

        #region Constructors

        protected AbstractSingleThreadWorker(ILogger<AbstractSingleThreadWorker> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Init/Shutdown

        public virtual void Init()
        {
            IsWorking = true;

            PreInit();

            if (_threadWorker == null)
            {
                try
                {
                    _logger.LogInformation("Init() {0} start.", WorkerName);

                    _threadWorker = new Thread(new ThreadStart(DoWork)) { IsBackground = true, Name = string.Format("TW{0}", WorkerName) };
                    _threadWorker.Start();

                    _logger.LogInformation("Init() {0} done.", WorkerName);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Init() {0} Error, {1}.", WorkerName, ex.Message);
                }
            }
        }

        public virtual void ShutdownSystem(TimeSpan timeSpan)
        {
            _logger.LogInformation("ShutdownSystem() enter.");

            StopWorker(timeSpan);

            _logger.LogInformation("ShutdownSystem() leave.");
        }

        #endregion

        #region Acceleration for Shutdown

        public static bool IsGlobalExiting { get; private set; }

        protected TimeSpan _tsEach = TimeSpan.FromMilliseconds(500);
        protected int _targetSleepCount = 2;

        public static void SetGlobalExiting()
        {
            IsGlobalExiting = true;
        }

        protected virtual void SetSleep(TimeSpan tsEach, int targetSleepCount)
        {
            _tsEach = tsEach;
            _targetSleepCount = targetSleepCount;
        }

        protected virtual void SleepAfterDoWork()
        {
            int i = 0;
            while (IsWorking && !IsGlobalExiting && i++ < _targetSleepCount)
                Thread.Sleep(_tsEach);
        }

        protected virtual void StopWorker(TimeSpan timeSpan)
        {
            IsWorking = false;
            if (_threadWorker != null)
            {
                try
                {
                    _logger.LogInformation("StopWorker() {0} start.", WorkerName);

                    _threadWorker.Join(timeSpan);
                    _threadWorker = null;

                    _logger.LogInformation("StopWorker() {0} done.", WorkerName);
                }
                catch (Exception ex)
                {
                    _logger.LogError("StopWorker() {0} Error, {1}.", WorkerName, ex.Message);
                }
            }
        }

        #endregion

        #region Pre/Post

        protected virtual void PreInit()
        {
        }

        #endregion

        #region DoWork method

        protected abstract void DoWork();

        #endregion
    }
}
