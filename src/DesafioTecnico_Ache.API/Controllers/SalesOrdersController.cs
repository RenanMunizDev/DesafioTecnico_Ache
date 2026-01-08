using DesafioTecnico_Ache.Application.Commands;
using DesafioTecnico_Ache.Application.DTOs;
using DesafioTecnico_Ache.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DesafioTecnico_Ache.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class SalesOrdersController : ControllerBase
{
    private readonly IGetSalesOrderQueryHandler _queryHandler;
    private readonly ICreateSalesOrderCommandHandler _commandHandler;
    private readonly ILogger<SalesOrdersController> _logger;
    
    public SalesOrdersController(
        IGetSalesOrderQueryHandler queryHandler,
        ICreateSalesOrderCommandHandler commandHandler,
        ILogger<SalesOrdersController> logger)
    {
        _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
        _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Retrieves a sales order from SAP SD module by order ID
    /// Integration Type: SAP S/4HANA OData REST API (API_SALES_ORDER_SRV)
    /// </summary>
    /// <param name="orderId">The SAP Sales Order ID</param>
    /// <returns>Sales order details</returns>
    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(SalesOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSalesOrder([FromRoute] string orderId)
    {
        try
        {
            _logger.LogInformation("Retrieving sales order {OrderId} from SAP SD", orderId);
            
            if (string.IsNullOrWhiteSpace(orderId))
            {
                _logger.LogWarning("Invalid order ID provided");
                return BadRequest(new { error = "Order ID is required" });
            }
            
            var query = new GetSalesOrderQuery(orderId);
            var result = await _queryHandler.HandleAsync(query);
            
            if (result == null)
            {
                _logger.LogWarning("Sales order {OrderId} not found", orderId);
                return NotFound(new { error = $"Sales order {orderId} not found" });
            }
            
            _logger.LogInformation("Successfully retrieved sales order {OrderId}", orderId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error for order {OrderId}", orderId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales order {OrderId}", orderId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }
    
    /// <summary>
    /// Creates a new sales order in SAP SD module
    /// Integration Type: SAP S/4HANA OData REST API (API_SALES_ORDER_SRV)
    /// </summary>
    /// <param name="request">Sales order creation request</param>
    /// <returns>Created sales order</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SalesOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSalesOrder([FromBody] CreateSalesOrderRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new sales order for customer {CustomerCode}", request.CustomerCode);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for sales order creation");
                return BadRequest(ModelState);
            }
            
            var command = new CreateSalesOrderCommand(request);
            var result = await _commandHandler.HandleAsync(command);
            
            _logger.LogInformation("Successfully created sales order {OrderId}", result.OrderId);
            
            return CreatedAtAction(
                nameof(GetSalesOrder),
                new { orderId = result.OrderId },
                result
            );
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error during sales order creation");
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict during sales order creation");
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sales order");
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }
}
