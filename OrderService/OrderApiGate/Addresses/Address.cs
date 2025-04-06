using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApiGate.Addresses
{
    public class Address : WriteAddress
    {
        public long AddressId { get; set; }
    }

    public class WriteAddress
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}
