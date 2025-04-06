namespace Infrastructure
{
    public interface IQueueRedeaderHandler<T>
    {
        public Task Handle(T message);
    }
}