namespace DesafioTecnico_Ache.Domain.Entities;

public class SalesOrder
{
    public string OrderId { get; private set; }
    public string CustomerCode { get; private set; }
    public string CustomerName { get; private set; }
    public DateTime OrderDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Status { get; private set; }
    public List<SalesOrderItem> Items { get; private set; }
    
    public SalesOrder(string orderId, string customerCode, string customerName, 
        DateTime orderDate, string status)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("Order ID cannot be empty", nameof(orderId));
        
        if (string.IsNullOrWhiteSpace(customerCode))
            throw new ArgumentException("Customer code cannot be empty", nameof(customerCode));
        
        OrderId = orderId;
        CustomerCode = customerCode;
        CustomerName = customerName;
        OrderDate = orderDate;
        Status = status;
        Items = new List<SalesOrderItem>();
    }
    
    public void AddItem(SalesOrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        
        Items.Add(item);
        CalculateTotal();
    }
    
    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status cannot be empty", nameof(newStatus));
        
        Status = newStatus;
    }
    
    private void CalculateTotal()
    {
        TotalAmount = Items.Sum(item => item.TotalPrice);
    }
}
