using InccApi.Models;

namespace InccApi.Repositories;

public interface IInccRepository
{
    Task<IEnumerable<InccEntry>> GetAllAsync();
}
