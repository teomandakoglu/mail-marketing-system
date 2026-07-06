using MailMarketing.Business.Services;
using MailMarketing.Core.Utilities.Queue;

namespace MailMarketing.API.Workers;

public class MailSendingBackgroundWorker : BackgroundService
{
    private readonly IMailQueue _mailQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MailSendingBackgroundWorker> _logger;

    public MailSendingBackgroundWorker(
        IMailQueue mailQueue,
        IServiceProvider serviceProvider,
        ILogger<MailSendingBackgroundWorker> logger)
    {
        _mailQueue = mailQueue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await _mailQueue.DequeueAsync(stoppingToken);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var mailSenderService = scope.ServiceProvider.GetRequiredService<IMailSenderService>();

                await mailSenderService.SendAsync(message, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Mail queue message could not be processed.");
            }
        }
    }
}
