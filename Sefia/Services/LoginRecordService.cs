using Sefia.Data;
using Sefia.Entities;

namespace Sefia.Services;

public class LoginRecordService
{
    private readonly AppDbContext _context;

    public LoginRecordService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveLoginRecordAsync(LoginHistory record)
    {
        _context.LoginHistorys.Add(record);
        await _context.SaveChangesAsync();
    }
}