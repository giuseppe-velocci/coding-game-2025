namespace Core
{
    public interface IQueueWriter<T>
    {
        void Complete();
        ValueTask Send(T message, CancellationToken cts);
    }
}