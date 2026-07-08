using MailMarketing.Business.DTOs.Templates;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.Business.Services;

public class TemplateService : ITemplateService
{
    private readonly MailMarketingDbContext _context;

    public TemplateService(MailMarketingDbContext context)
    {
        _context = context;
    }

    public async Task<List<TemplateDto>> GetAllAsync(int userId)
    {
        return await _context.Templates
            .AsNoTracking()
            .Where(template => template.CreatedByUserId == userId)
            .OrderByDescending(template => template.CreatedAt)
            .Select(template => MapToDto(template))
            .ToListAsync();
    }

    public async Task<TemplateDto?> GetByIdAsync(int id, int userId)
    {
        return await _context.Templates
            .AsNoTracking()
            .Where(template => template.Id == id && template.CreatedByUserId == userId)
            .Select(template => MapToDto(template))
            .SingleOrDefaultAsync();
    }

    public async Task<TemplateDto> AddAsync(CreateTemplateDto createTemplateDto, int userId)
    {
        var template = new Template
        {
            Title = createTemplateDto.Title.Trim(),
            Content = createTemplateDto.Content,
            CreatedByUserId = userId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Templates.AddAsync(template);
        await _context.SaveChangesAsync();

        return MapToDto(template);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTemplateDto updateTemplateDto, int userId)
    {
        var template = await _context.Templates
            .SingleOrDefaultAsync(item => item.Id == id && item.CreatedByUserId == userId);

        if (template is null)
        {
            return false;
        }

        template.Title = updateTemplateDto.Title.Trim();
        template.Content = updateTemplateDto.Content;
        template.IsActive = updateTemplateDto.IsActive;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var template = await _context.Templates
            .Include(item => item.MailLogs)
            .SingleOrDefaultAsync(item => item.Id == id && item.CreatedByUserId == userId);

        if (template is null || template.MailLogs.Count > 0)
        {
            return false;
        }

        _context.Templates.Remove(template);
        await _context.SaveChangesAsync();

        return true;
    }

    private static TemplateDto MapToDto(Template template)
    {
        return new TemplateDto
        {
            Id = template.Id,
            Title = template.Title,
            Content = template.Content,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            CreatedByUserId = template.CreatedByUserId
        };
    }
}
