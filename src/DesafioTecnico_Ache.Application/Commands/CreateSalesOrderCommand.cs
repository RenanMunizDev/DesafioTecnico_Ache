using DesafioTecnico_Ache.Application.DTOs;

namespace DesafioTecnico_Ache.Application.Commands;

public record CreateSalesOrderCommand(CreateSalesOrderRequest Request);

public interface ICreateSalesOrderCommandHandler
{
    Task<SalesOrderDto> HandleAsync(CreateSalesOrderCommand command);
}
