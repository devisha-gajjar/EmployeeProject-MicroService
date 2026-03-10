namespace Employee.Shared.Interfaces;

public interface IUnitOfWork
{
    int Save();
    Task<int> SaveAsync();
}
