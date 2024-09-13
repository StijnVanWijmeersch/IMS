using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using Polly;
using IMS.Infrastructure.Resolvers;
using IMS.Infrastructure.Contexts;
using IMS.SharedKernel;

namespace IMS.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob : IJob
{
    private readonly IMSDbContext _context;
    private readonly IPublisher _publisher;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        ContractResolver = new PrivateSetterResolver()
    };

    public ProcessOutboxMessagesJob(
        IMSDbContext context,
        IPublisher publisher,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }


    public async Task Execute(IJobExecutionContext context)
    {
        string? finalError = null;

        var messages = await _context.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .Take(5)
            .ToListAsync(context.CancellationToken);

        foreach (var message in messages)
        {
            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                message.Content,
                JsonSerializerSettings);

            if (domainEvent is null)
            {
                _logger.LogInformation("The deserialization of the message {message} returned a null value for domainevent", message);
                continue;
            }

            var pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new Polly.Retry.RetryStrategyOptions()
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Constant,
                    Delay = TimeSpan.FromSeconds(3),
                    ShouldHandle = new PredicateBuilder()
                        .Handle<Exception>(),
                    OnRetry = retryArgs =>
                    {
                        _logger.LogInformation(
                            "Handling domainevent: {domainEvent} " +
                            "Current attempt: {attemptNumber}, {Outcome}", domainEvent, retryArgs.AttemptNumber, retryArgs.Outcome);

                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            try
            {
                await pipeline.ExecuteAsync(async cancellation =>
                {
                    await _publisher.Publish(domainEvent, cancellation);
                },
                context.CancellationToken);
            }
            catch (Exception ex)
            {
                finalError = ex.Message;
                _logger.LogError(ex, "An error occurred while processing the outbox message {message}", message);
            }

            message.ErrorMessage = finalError;
            message.ProcessedAt = DateTime.Now;

            await _context.SaveChangesAsync(context.CancellationToken);
        }
    }
}
