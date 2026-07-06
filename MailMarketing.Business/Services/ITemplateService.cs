using MailMarketing.Business.DTOs.Templates;

namespace MailMarketing.Business.Services;

public interface ITemplateService
{
    Task<List<TemplateDto>> GetAllAsync();

    Task<TemplateDto?> GetByIdAsync(int id);

    Task<TemplateDto> AddAsync(CreateTemplateDto createTemplateDto, int userId);

    Task<bool> UpdateAsync(int id, UpdateTemplateDto updateTemplateDto);

    Task<bool> DeleteAsync(int id);
}
