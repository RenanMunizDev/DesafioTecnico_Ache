using DesafioTecnico_Ache.Application.DTOs;
using DesafioTecnico_Ache.Domain.Interfaces;

namespace DesafioTecnico_Ache.Application.Queries;

public class GetSalesOrderQueryHandler : IGetSalesOrderQueryHandler
{
    private readonly ISalesOrderRepository _repository;
    
    public GetSalesOrderQueryHandler(ISalesOrderRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task<SalesOrderDto?> HandleAsync(GetSalesOrderQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.OrderId))
            throw new ArgumentException("Order ID cannot be empty", nameof(query));
        
        var order = await _repository.GetByIdAsync(query.OrderId);
        
        if (order == null)
            return null;
        
        return new SalesOrderDto
        {
            OrderId = order.OrderId,
            CustomerCode = order.CustomerCode,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Items = order.Items.Select(item => new SalesOrderItemDto
            {
                ItemId = item.ItemId,
                MaterialCode = item.MaterialCode,
                MaterialDescription = item.MaterialDescription,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            }).ToList()
        };
    }
}
