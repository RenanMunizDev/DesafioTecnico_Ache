using DesafioTecnico_Ache.Application.DTOs;
using DesafioTecnico_Ache.Domain.Entities;
using DesafioTecnico_Ache.Domain.Interfaces;

namespace DesafioTecnico_Ache.Application.Commands;

public class CreateSalesOrderCommandHandler : ICreateSalesOrderCommandHandler
{
    private readonly ISalesOrderRepository _repository;
    
    public CreateSalesOrderCommandHandler(ISalesOrderRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task<SalesOrderDto> HandleAsync(CreateSalesOrderCommand command)
    {
        var request = command.Request;
        
        var orderId = _repository.GenerateOrderId();
        
        if (await _repository.ExistsAsync(orderId))
            throw new InvalidOperationException($"Order with ID {orderId} already exists");
        
        var salesOrder = new SalesOrder(
            orderId,
            request.CustomerCode,
            request.CustomerName,
            DateTime.UtcNow,
            request.Status
        );
        
        foreach (var itemRequest in request.Items)
        {
            var itemId = GenerateItemId();
            var item = new SalesOrderItem(
                itemId,
                itemRequest.MaterialCode,
                itemRequest.MaterialDescription,
                itemRequest.Quantity,
                itemRequest.UnitPrice
            );
            salesOrder.AddItem(item);
        }
        
        var createdOrder = await _repository.CreateAsync(salesOrder);
        
        return new SalesOrderDto
        {
            OrderId = createdOrder.OrderId,
            CustomerCode = createdOrder.CustomerCode,
            CustomerName = createdOrder.CustomerName,
            OrderDate = createdOrder.OrderDate,
            TotalAmount = createdOrder.TotalAmount,
            Status = createdOrder.Status,
            Items = createdOrder.Items.Select(item => new SalesOrderItemDto
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
    
    private static string GenerateItemId()
    {
        return $"ITM{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}
