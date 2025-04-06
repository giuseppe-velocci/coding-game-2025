namespace OrderApiGate
{
    public class SerializableResult<T>
    {
        public T? Value { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
