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

        public abstract TOut Accept<TOut>(IOperationResultVisitor<T, TOut> visitor);
    }

    public class SuccessResult<T> : OperationResult<T>
    {
        public SuccessResult(T? value) : base(true)
        {
            Value = value;
        }

        public override TOut Accept<TOut>(IOperationResultVisitor<T, TOut> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class ValidationFailureResult<T> : OperationResult<T>
    {
        public ValidationFailureResult(string message) : base(false)
        {
            Message = message;
        }

        public override TOut Accept<TOut>(IOperationResultVisitor<T, TOut> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class InvalidRequestResult<T> : OperationResult<T>
    {
        public InvalidRequestResult(string message) : base(false)
        {
            Message = message;
        }

        public override TOut Accept<TOut>(IOperationResultVisitor<T, TOut> visitor)
        {
            return visitor.Visit(this);
        }
    }
    
    public class CriticalFailureResult<T> : OperationResult<T>
    {
        public CriticalFailureResult(string message) : base(false)
        {
            Message = message;
        }

        public override TOut Accept<TOut>(IOperationResultVisitor<T, TOut> visitor)
        {
            return visitor.Visit(this);
        }
    }
    
    public class NotFoundResult<T> : OperationResult<T>
    {
        public NotFoundResult(string message) : base(false)
        {
            Message = message;
        }

        public override TOut Accept<TOut>(IOperationResultVisitor<T, TOut> visitor)
        {
            return visitor.Visit(this);
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
