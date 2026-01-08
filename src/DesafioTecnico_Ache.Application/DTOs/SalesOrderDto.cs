namespace DesafioTecnico_Ache.Application.DTOs;

public class SalesOrderDto
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<SalesOrderItemDto> Items { get; set; } = new();
}

public class SalesOrderItemDto
{
    public string ItemId { get; set; } = string.Empty;
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
