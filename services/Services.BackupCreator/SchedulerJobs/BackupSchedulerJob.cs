using Microsoft.Extensions.Logging;
using Quartz;
using Services.BackupCreator.Config;
using Services.BackupCreator.Jobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.BackupCreator.SchedulerJobs
{
    public class BackupSchedulerJob : ISchedulerJob
    {
        private readonly ILogger<BackupCreatorService> _logger;
        private readonly SchedulersConfig _schedulersConfig;
        private readonly IScheduler _scheduler;

        public BackupSchedulerJob(ILogger<BackupCreatorService> logger,
            SchedulersConfig schedulersConfig,
            IScheduler scheduler)
        {
            _logger = logger;
            _schedulersConfig = schedulersConfig;
            _scheduler = scheduler;
        }

        public async Task ScheduleJobAsync()
        {
            _logger.LogInformation($"Scheduling {nameof(BackupSchedulerJob)}");

            try
            {
                await _scheduler.Start();

                IJobDetail job = JobBuilder.Create<CreateBackupJob>()
                    .WithIdentity(nameof(BackupSchedulerJob), "BackupGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity($"{nameof(BackupSchedulerJob)}Trigger", "BackupGroup")
                    .StartNow()
                    .WithDailyTimeIntervalSchedule(x =>
                        x.WithIntervalInHours(24)
                         .OnEveryDay()
                         .StartingDailyAt(TimeOfDay
                            .HourAndMinuteOfDay(_schedulersConfig.BackupHourOfADay, _schedulersConfig.BackupMinuteOfADay)))
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException se)
            {
                _logger.LogError(se, "Cannot schedule job");
            }
        }

        public async Task ShutdownAsync()
        {
            await _scheduler.Shutdown();
        }
    }
}
