namespace ChaosMonkey.API
{
    public class ShutdownGatewayWorker: BackgroundService
    {
        private readonly ILogger<ShutdownGatewayWorker> _logger;

        public ShutdownGatewayWorker(ILogger<ShutdownGatewayWorker> logger)
        {
            _logger = logger;
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}