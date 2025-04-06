using Core;
using OrderService.Addresses;
using OrderService.Products;
using OrderService.Users;
using System.Text.Json;

namespace OrderService.Orders.DataSynchronization
{
    public class QueueMessageHandler(
        IHttpClientService _httpClient, 
        ICrudRepository<User> _userRepo,
        ICrudRepository<Address> _addressRepo,
        ICrudRepository<Product> _productRepo,
        ILogger<QueueMessageHandler> _logger
        ) : IQueueReaderHandler<EventMessage>
    {
        private Dictionary<string, string> urlMapping = new()
        {
            { EventMessage.UserNotFoundEvent, "http://localhost:123/users" },
            { EventMessage.AddressNotFoundEvent, "http://localhost:123/addresses" },
            { EventMessage.ProductNotFoundEvent, "http://localhost:123/products" },
        };

        public Task Handle(EventMessage message, CancellationToken cts)
        {
            if (message is not null && urlMapping.TryGetValue(message.EventType, out var url))
            {
                return CallApiAsync(message, url, cts);
            }
            else
            {
                _logger.LogWarning("Invalid EventMessage {@msg}", message);
                return Task.CompletedTask;
            }
        }

        private async Task CallApiAsync(EventMessage message, string url, CancellationToken cts)
        {
            var response = await _httpClient.CallEndpointAsync($"{url}/{message.EventId}");
            if (response == string.Empty)
            {
                _logger.LogWarning("Api call failed for {@msg}", message);
                return;
            }
            else
            {
                if (message.EventType == EventMessage.UserNotFoundEvent)
                {
                    var user = JsonSerializer.Deserialize<SuccessResult<User>>(response);
                    await TryStoreUser(_userRepo, _logger, user?.Value, cts);
                }
                else if (message.EventType == EventMessage.AddressNotFoundEvent)
                {
                    var address = JsonSerializer.Deserialize<SuccessResult<Address>>(response);
                    await TryStoreAddress(_addressRepo, _logger, address?.Value, cts);
                }
                else if (message.EventType == EventMessage.ProductNotFoundEvent)
                {
                    var product = JsonSerializer.Deserialize<SuccessResult<Product>>(response);
                    await TryStoreProduct(_productRepo, _logger, product?.Value, cts);
                }
                else
                {
                    _logger.LogWarning("Invalid message type {@msg}", message);
                }
            }
        }

        private static async Task TryStoreProduct(
            ICrudRepository<Product> repo,
            ILogger logger,
            Product? value,
            CancellationToken cts)
        {
            OperationResult<long> res = null!;

            if (value is not null)
            {
                res = await repo.Create(value, cts);
            }

            if (res is not null && res.Value > 0)
            {
                logger.LogInformation("Created product {id}", res.Value);
            }
            else
            {
                logger.LogWarning("Failed storage for product");
            }
        }

        private static async Task TryStoreAddress(
            ICrudRepository<Address> repo,
            ILogger logger,
            Address? value,
            CancellationToken cts)
        {
            OperationResult<long> res = null!;

            if (value is not null)
            {
                res = await repo.Create(value, cts);
            }

            if (res is not null && res.Value > 0)
            {
                logger.LogInformation("Created address {id}", res.Value);
            }
            else
            {
                logger.LogWarning("Failed storage for address");
            }
        }
        
        private static async Task TryStoreUser(
            ICrudRepository<User> repo,
            ILogger logger,
            User? value,
            CancellationToken cts)
        {
            OperationResult<long> res = null!;

            if (value is not null)
            {
                res = await repo.Create(value, cts);
            }

            if (res is not null && res.Value > 0)
            {
                logger.LogInformation("Created user {id}", res.Value);
            }
            else
            {
                logger.LogWarning("Failed storage for user");
            }
        }

        private static async Task TryStoreUser(ICrudRepository<User> _userRepo, SuccessResult<User>? user, CancellationToken cts)
        {
            await _userRepo.Create(user.Value, cts);
        }
    }
}