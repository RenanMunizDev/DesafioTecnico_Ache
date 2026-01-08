using DesafioTecnico_Ache.Domain.Interfaces;

namespace DesafioTecnico_Ache.Infrastructure.SAP;

public class SapODataService : ISapODataService
{
    private readonly Dictionary<string, object> _mockData = new();
    
    public Task<T?> GetAsync<T>(string endpoint, string id) where T : class
    {
        var key = $"{endpoint}/{id}";
        if (_mockData.TryGetValue(key, out var data))
        {
            return Task.FromResult(data as T);
        }
        return Task.FromResult<T?>(null);
    }
    
    public Task<IEnumerable<T>> GetAllAsync<T>(string endpoint) where T : class
    {
        var results = _mockData
            .Where(kvp => kvp.Key.StartsWith(endpoint + "/"))
            .Select(kvp => kvp.Value)
            .OfType<T>();
        
        return Task.FromResult(results);
    }
    
    public Task<T> PostAsync<T>(string endpoint, T entity) where T : class
    {
        var id = Guid.NewGuid().ToString();
        var key = $"{endpoint}/{id}";
        _mockData[key] = entity;
        
        return Task.FromResult(entity);
    }
}
