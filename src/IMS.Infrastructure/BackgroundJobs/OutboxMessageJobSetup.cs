using Microsoft.Extensions.Options;
using Quartz;

namespace IMS.Infrastructure.BackgroundJobs;

internal sealed class OutboxMessageJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(ProcessOutboxMessagesJob));

        options
            .AddJob<ProcessOutboxMessagesJob>(jb => jb.WithIdentity(jobKey))
            .AddTrigger(trigger =>
            {
                trigger.ForJob(jobKey)
                    .WithIdentity($"{jobKey.Name}.trigger")
                    .WithSimpleSchedule(schedule =>
                       schedule.WithIntervalInSeconds(10)
                               .RepeatForever())
                    .StartNow();
            });

    }
}
