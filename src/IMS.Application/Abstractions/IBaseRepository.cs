namespace IMS.Application.Abstractions;

public interface IBaseRepository<T> where T : class
{
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> ExistsByIdAsync(ulong id, CancellationToken cancellationToken);
    Task<T?> FindByIdAsync(ulong id, CancellationToken cancellationToken);
}
