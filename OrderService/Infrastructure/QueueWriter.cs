using Core;
using System.Threading.Channels;

namespace Infrastructure
{
    public class QueueWriter<T> : IQueueWriter<T>
    {
        private readonly Channel<T> _channel;
        private readonly ChannelWriter<T> writer;

        public QueueWriter(Channel<T> channel)
        {
            _channel = channel;
            writer = _channel.Writer;
        }

        public async ValueTask Send(T message, CancellationToken cts)
        {
            if (await writer.WaitToWriteAsync(cts))
            {
                await writer.WriteAsync(message, cts);
            }
        }

        public void Complete()
        {
            writer.Complete();
        }
    }
}
