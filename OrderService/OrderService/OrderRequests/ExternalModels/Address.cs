﻿namespace OrderService.OrderRequests.ExternalModels
{
    public class Address
    {
        public long AddressId { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }

}
