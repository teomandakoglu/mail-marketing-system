using MailMarketing.Business.DTOs.Subscribers;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.Business.Services;

public class SubscriberService : ISubscriberService
{
    private readonly MailMarketingDbContext _context;

    public SubscriberService(MailMarketingDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubscriberDto>> GetAllAsync(int userId)
    {
        return await _context.Subscribers
            .AsNoTracking()
            .Where(subscriber => subscriber.UserId == userId)
            .OrderByDescending(subscriber => subscriber.CreatedAt)
            .Select(subscriber => new SubscriberDto
            {
                Id = subscriber.Id,
                Email = subscriber.Email,
                IsActive = subscriber.IsActive,
                CreatedAt = subscriber.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> AddAsync(CreateSubscriberDto createSubscriberDto, int userId)
    {
        var normalizedEmail = createSubscriberDto.Email.Trim().ToLowerInvariant();
        var exists = await _context.Subscribers
            .AnyAsync(subscriber => subscriber.UserId == userId && subscriber.Email == normalizedEmail);

        if (exists)
        {
            return false;
        }

        await _context.Subscribers.AddAsync(new Subscriber
        {
            UserId = userId,
            Email = normalizedEmail,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddToDefaultTenantAsync(CreateSubscriberDto createSubscriberDto)
    {
        var defaultUserId = await _context.Users
            .OrderBy(user => user.Id)
            .Select(user => (int?)user.Id)
            .FirstOrDefaultAsync();

        return defaultUserId.HasValue && await AddAsync(createSubscriberDto, defaultUserId.Value);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var subscriber = await _context.Subscribers
            .Include(item => item.MailLogs)
            .SingleOrDefaultAsync(item => item.Id == id && item.UserId == userId);

        if (subscriber is null || subscriber.MailLogs.Count > 0)
        {
            return false;
        }

        _context.Subscribers.Remove(subscriber);
        await _context.SaveChangesAsync();

        return true;
    }
}
