using EMS.BaseLibrary.Responses;

namespace EMS.APILibrary.Repositories.Contracts;

public interface IGenericRepositoryInterface<T>
{
    Task<List<T>> GetAll();
    Task<T> GetById(int id);
    Task<GeneralResponse> Insert(T item);
    Task<GeneralResponse> Update(T item);
    Task<GeneralResponse> DeleteById(int id);
}