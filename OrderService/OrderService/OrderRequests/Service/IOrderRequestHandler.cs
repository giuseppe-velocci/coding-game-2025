using Core;
using OrderService.Orders;

namespace OrderService.OrderRequests.Service
{
    public interface IOrderRequestHandler
    {
        Task<OperationResult<long>> Create(OrderRequest value, CancellationToken cts);
        Task<OperationResult<None>> Delete(long id, CancellationToken cts);
        Task<OperationResult<Order[]>> ReadAll(CancellationToken cts);
        Task<OperationResult<Order>> ReadOne(long id, CancellationToken cts);
        Task<OperationResult<None>> Update(long id, OrderRequest value, CancellationToken cts);
    }
}