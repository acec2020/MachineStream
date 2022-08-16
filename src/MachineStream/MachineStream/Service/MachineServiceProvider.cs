namespace MachineStream
{
    public class MachineServiceProvider : AbstractSingleThreadWorkerWithQueue<Machine>, IMachineService, IDisposable
    {
        private readonly ILogger<MachineServiceProvider> _logger;
        private Dictionary<string, Machine> _machines;
        public MachineServiceProvider(ILogger<MachineServiceProvider> logger)
            : base(logger)
        {
            _logger = logger;
            _machines = new Dictionary<string, Machine>();
            Init();
        }

        public override string WorkerName => "MachineWorker";

        public async Task<Machine> GetMachine(string machineId)
        {
            if (_machines.ContainsKey(machineId))
                return await Task.FromResult(_machines[machineId]);
            else
                return await Task.FromResult(new Machine());
        }

        public async Task SaveMachine(string streamMsg)
        {
            var stream = JsonConvert.DeserializeObject<Stream>(streamMsg);
            if (stream != null)
                await AddTask(stream.Payload);
            await Task.CompletedTask;
        }

        protected override void HandleTask(Machine machine)
        {
            _machines[machine.MachineId] = machine;
        }

        public void Dispose()
        {
            ShutdownSystem(TimeSpan.Zero);
        }
    }
}
