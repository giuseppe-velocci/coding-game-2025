using Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Infrastructure
{
    public class QueueReaderService<T>(
        Channel<T> _channel,
        IQueueReaderHandler<T> _handler,
        ILogger<QueueReaderService<T>> _logger
    ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var reader = _channel.Reader;

            try
            {
                await foreach (var message in reader.ReadAllAsync(stoppingToken))
                {
                    _logger.LogDebug("Reader: Read value {@message}", message);
                    await _handler.Handle(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reader exception");
            }
        }
    }
}
