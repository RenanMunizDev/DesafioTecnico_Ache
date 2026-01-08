using DesafioTecnico_Ache.Application.DTOs;

namespace DesafioTecnico_Ache.Application.Queries;

public record GetSalesOrderQuery(string OrderId);

public interface IGetSalesOrderQueryHandler
{
    Task<SalesOrderDto?> HandleAsync(GetSalesOrderQuery query);
}
