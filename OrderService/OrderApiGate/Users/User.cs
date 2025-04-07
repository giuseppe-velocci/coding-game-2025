namespace OrderApiGate.Users
{
    public class User : WriteUser
    {
        public long UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
