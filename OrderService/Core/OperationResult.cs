namespace Core
{
    public abstract class OperationResult<T>
    {
        protected OperationResult(bool success)
        {
            Success = success;
        }

        public T? Value { get; protected set; }
        public string Message { get; protected set; } = string.Empty;
        public bool Success { get; }
    }

    public class SuccessResult<T> : OperationResult<T>
    {
        public SuccessResult(T? value) : base(true)
        {
            Value = value;
        }
    }

    public class CriticalFailureResult<T> : OperationResult<T>
    {
        public CriticalFailureResult(string message) : base(false)
        {
            Message = message;
        }
    }
    
    public class NotFoundResult<T> : OperationResult<T>
    {
        public NotFoundResult(string message) : base(false)
        {
            Message = message;
        }
    }

    public sealed record class None
    {
        private static None _instance = new();
        private None()
        { }

        public static None Instance() => _instance;
    }
}
