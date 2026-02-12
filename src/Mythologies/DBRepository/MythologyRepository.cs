
using Microsoft.EntityFrameworkCore;
using MythApi.Common.Database;
using MythApi.Common.Database.Models;
using MythApi.Common.Models;
using MythApi.Mythologies.Interfaces;

namespace MythApi.Mythologies.DBRepositories;

public class MythologyRepository : IMythologyRepository
{
    private readonly AppDbContext _context;

    public MythologyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Mythology>> GetAllMythologiesAsync()
    {
        return await _context.Mythologies.ToListAsync();
    }

    public async Task<PagedResult<Mythology>> GetAllMythologiesAsync(PaginationParameters pagination)
    {
        var totalCount = await _context.Mythologies.CountAsync();
        
        var mythologies = await _context.Mythologies
            .OrderBy(m => m.Id)
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PagedResult<Mythology>
        {
            Items = mythologies,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }
}
