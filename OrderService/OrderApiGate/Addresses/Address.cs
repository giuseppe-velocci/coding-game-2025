namespace OrderApiGate.Addresses
{
    public class Address : WriteAddress
    {
        public long AddressId { get; set; }
        public bool IsActive { get; set; }
    }
}
