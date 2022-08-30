namespace MachineStream
{
    public class ConsumerHostedService : BackgroundService
    {
        private readonly ILogger<ConsumerHostedService> _logger;
        private IMachineService _machineService { get; }
        private WebsocketClient _client;

        public ConsumerHostedService(IMachineService machineService, ILogger<ConsumerHostedService> logger)
        {
            _logger = logger;
            _machineService = machineService;
            _client = new WebsocketClient(new Uri(""));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Hosted Service running.");
            try
            {
                _client.ReconnectTimeout = TimeSpan.FromSeconds(60);
                _client.ReconnectionHappened.Subscribe(info =>
                {
                    Console.WriteLine("Reconnection happened, type: " + info.Type);
                });
                _client.MessageReceived.Subscribe(msg =>
                {
                    Console.WriteLine("Message received: " + msg);
                    _machineService.SaveMachine(msg.Text);
                });
                await _client.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Hosted Service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}
