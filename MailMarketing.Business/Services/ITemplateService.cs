using MailMarketing.Business.DTOs.Templates;

namespace MailMarketing.Business.Services;

public interface ITemplateService
{
    Task<List<TemplateDto>> GetAllAsync(int userId);

    Task<TemplateDto?> GetByIdAsync(int id, int userId);

    Task<TemplateDto> AddAsync(CreateTemplateDto createTemplateDto, int userId);

    Task<bool> UpdateAsync(int id, UpdateTemplateDto updateTemplateDto, int userId);

    Task<bool> DeleteAsync(int id, int userId);
}
