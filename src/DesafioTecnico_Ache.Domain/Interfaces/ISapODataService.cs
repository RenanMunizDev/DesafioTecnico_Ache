namespace DesafioTecnico_Ache.Domain.Interfaces;

public interface ISapODataService
{
    Task<T?> GetAsync<T>(string endpoint, string id) where T : class;
    Task<IEnumerable<T>> GetAllAsync<T>(string endpoint) where T : class;
    Task<T> PostAsync<T>(string endpoint, T entity) where T : class;
}
