using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Data;
using ProcessTracker.API.Entities;

namespace ProcessTracker.API.Repositories;

public class ProcessRecordRepository : IProcessRecordRepository
{
    private readonly ProcessTrackerDbContext _context;

    public ProcessRecordRepository(ProcessTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<ProcessRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessRecords.FirstOrDefaultAsync(pr => pr.Id == id, cancellationToken);
    }

    public async Task<ProcessRecord?> GetByServiceItemIdAsync(int serviceItemId, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessRecords.FirstOrDefaultAsync(pr => pr.ServiceItemId == serviceItemId, cancellationToken);
    }

    public async Task<ProcessRecord> AddAsync(ProcessRecord processRecord, CancellationToken cancellationToken = default)
    {
        _context.ProcessRecords.Add(processRecord);
        await _context.SaveChangesAsync(cancellationToken);
        return processRecord;
    }

    public async Task UpdateAsync(ProcessRecord processRecord, CancellationToken cancellationToken = default)
    {
        _context.ProcessRecords.Update(processRecord);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ProcessRecord processRecord, CancellationToken cancellationToken = default)
    {
        _context.ProcessRecords.Remove(processRecord);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
