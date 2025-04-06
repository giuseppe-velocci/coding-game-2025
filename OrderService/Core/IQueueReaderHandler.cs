namespace Core
{
    public interface IQueueReaderHandler<T>
    {
        public Task Handle(T message, CancellationToken cts);
    }
}