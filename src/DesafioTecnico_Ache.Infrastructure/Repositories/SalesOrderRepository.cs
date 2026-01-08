using DesafioTecnico_Ache.Domain.Entities;
using DesafioTecnico_Ache.Domain.Interfaces;

namespace DesafioTecnico_Ache.Infrastructure.Repositories;

public class SalesOrderRepository : ISalesOrderRepository
{
    private readonly ISapODataService _sapService;
    private static readonly Dictionary<string, SalesOrder> _inMemoryStorage = new();
    private const string SapEndpoint = "API_SALES_ORDER_SRV/A_SalesOrder";
    private static int _orderCounter = 3;
    private static readonly object _counterLock = new();
    private static bool _isInitialized = false;
    
    public SalesOrderRepository(ISapODataService sapService)
    {
        _sapService = sapService ?? throw new ArgumentNullException(nameof(sapService));
        
        if (!_isInitialized)
        {
            lock (_counterLock)
            {
                if (!_isInitialized)
                {
                    InitializeMockData();
                    _isInitialized = true;
                }
            }
        }
    }
    
    public Task<SalesOrder?> GetByIdAsync(string orderId)
    {
        lock (_counterLock)
        {
            _inMemoryStorage.TryGetValue(orderId, out var order);
            return Task.FromResult(order);
        }
    }
    
    public Task<IEnumerable<SalesOrder>> GetAllAsync()
    {
        lock (_counterLock)
        {
            return Task.FromResult<IEnumerable<SalesOrder>>(_inMemoryStorage.Values.ToList());
        }
    }
    
    public Task<SalesOrder> CreateAsync(SalesOrder salesOrder)
    {
        if (salesOrder == null)
            throw new ArgumentNullException(nameof(salesOrder));
        
        lock (_counterLock)
        {
            _inMemoryStorage[salesOrder.OrderId] = salesOrder;
        }
        
        return Task.FromResult(salesOrder);
    }
    
    public Task<bool> ExistsAsync(string orderId)
    {
        lock (_counterLock)
        {
            return Task.FromResult(_inMemoryStorage.ContainsKey(orderId));
        }
    }
    
    public string GenerateOrderId()
    {
        lock (_counterLock)
        {
            _orderCounter++;
            return $"SO{DateTime.UtcNow:yyyyMMdd}{_orderCounter:D3}";
        }
    }
    
    private static void InitializeMockData()
    {
        var order1 = new SalesOrder(
            "SO20260108001",
            "CUST001",
            "Farmácia Popular Ltda",
            DateTime.UtcNow.AddDays(-5),
            "CONFIRMED"
        );
        order1.AddItem(new SalesOrderItem("ITM001", "MAT001", "Paracetamol 500mg", 100, 2.50m));
        order1.AddItem(new SalesOrderItem("ITM002", "MAT002", "Dipirona 1g", 50, 3.75m));
        
        var order2 = new SalesOrder(
            "SO20260108002",
            "CUST002",
            "Drogaria Moderna S.A.",
            DateTime.UtcNow.AddDays(-3),
            "PROCESSING"
        );
        order2.AddItem(new SalesOrderItem("ITM003", "MAT003", "Ibuprofeno 600mg", 200, 4.20m));
        
        var order3 = new SalesOrder(
            "SO20260108003",
            "CUST003",
            "Rede Saúde Plus",
            DateTime.UtcNow.AddDays(-1),
            "PENDING"
        );
        order3.AddItem(new SalesOrderItem("ITM004", "MAT001", "Paracetamol 500mg", 150, 2.50m));
        order3.AddItem(new SalesOrderItem("ITM005", "MAT004", "Amoxicilina 500mg", 80, 8.90m));
        order3.AddItem(new SalesOrderItem("ITM006", "MAT005", "Omeprazol 20mg", 120, 5.60m));
        
        _inMemoryStorage[order1.OrderId] = order1;
        _inMemoryStorage[order2.OrderId] = order2;
        _inMemoryStorage[order3.OrderId] = order3;
    }
}
