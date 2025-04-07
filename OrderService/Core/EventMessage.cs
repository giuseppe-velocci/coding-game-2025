namespace Core
{
    public class EventMessage
    {
        public static readonly string UserNotFoundEvent = "UserNotFound";
        public static readonly string AddressNotFoundEvent = "AddressNotFound";
        public static readonly string ProductNotFoundEvent = "ProductNotFound";

        private EventMessage(string eventType, long eventId)
        {
            EventType = eventType;
            EventId = eventId;
        }

        public string EventType { get; }
        public long EventId { get; }

        public static EventMessage UserNotFound(long userId)
        {
            return new EventMessage(UserNotFoundEvent, userId);
        }

        public static EventMessage AddressNotFound(long userId)
        {
            return new EventMessage(AddressNotFoundEvent, userId);
        }

        public static EventMessage ProductNotFound(long orderId)
        {
            return new EventMessage(ProductNotFoundEvent, orderId);
        }
    }
}
