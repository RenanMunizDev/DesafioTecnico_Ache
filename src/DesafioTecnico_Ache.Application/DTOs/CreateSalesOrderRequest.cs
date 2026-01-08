using System.ComponentModel.DataAnnotations;

namespace DesafioTecnico_Ache.Application.DTOs;

public class CreateSalesOrderRequest
{
    [Required(ErrorMessage = "Customer code is required")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "Customer code must be between 1 and 10 characters")]
    public string CustomerCode { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Customer name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Customer name must be between 3 and 100 characters")]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "At least one item is required")]
    [MinLength(1, ErrorMessage = "At least one item is required")]
    public List<CreateSalesOrderItemRequest> Items { get; set; } = new();
}

public class CreateSalesOrderItemRequest
{
    [Required(ErrorMessage = "Material code is required")]
    [StringLength(18, MinimumLength = 1, ErrorMessage = "Material code must be between 1 and 18 characters")]
    public string MaterialCode { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Material description is required")]
    public string MaterialDescription { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }
}
