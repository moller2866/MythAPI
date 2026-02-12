
using MythApi.Common.Database.Models;
using MythApi.Common.Models;

namespace MythApi.Mythologies.Interfaces;

public interface IMythologyRepository
{
    public Task<IList<Mythology>> GetAllMythologiesAsync();
    
    public Task<PagedResult<Mythology>> GetAllMythologiesAsync(PaginationParameters pagination);
}
