using DesafioTecnico_Ache.Domain.Entities;

namespace DesafioTecnico_Ache.Domain.Interfaces;

public interface ISalesOrderRepository
{
    Task<SalesOrder?> GetByIdAsync(string orderId);
    Task<IEnumerable<SalesOrder>> GetAllAsync();
    Task<SalesOrder> CreateAsync(SalesOrder salesOrder);
    Task<bool> ExistsAsync(string orderId);
    string GenerateOrderId();
}
