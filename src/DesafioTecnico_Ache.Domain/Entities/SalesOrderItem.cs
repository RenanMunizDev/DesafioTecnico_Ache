namespace DesafioTecnico_Ache.Domain.Entities;

public class SalesOrderItem
{
    public string ItemId { get; private set; }
    public string MaterialCode { get; private set; }
    public string MaterialDescription { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    
    public SalesOrderItem(string itemId, string materialCode, string materialDescription, 
        int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            throw new ArgumentException("Item ID cannot be empty", nameof(itemId));
        
        if (string.IsNullOrWhiteSpace(materialCode))
            throw new ArgumentException("Material code cannot be empty", nameof(materialCode));
        
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        
        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero", nameof(unitPrice));
        
        ItemId = itemId;
        MaterialCode = materialCode;
        MaterialDescription = materialDescription;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = quantity * unitPrice;
    }
}
