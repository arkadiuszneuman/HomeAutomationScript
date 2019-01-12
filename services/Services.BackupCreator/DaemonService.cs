using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Services.BackupCreator.Jobs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.BackupCreator
{
    public class BackupCreatorService : IHostedService
    {
        private readonly IScheduler _scheduler;
        private readonly CreateBackupJob _createBackupJob;
        private readonly ZipBackupCreator _zipBackupCreator;

        public BackupCreatorService(IScheduler scheduler, CreateBackupJob createBackupJob, ZipBackupCreator zipBackupCreator)
        {
            _scheduler = scheduler;
            _createBackupJob = createBackupJob;
            _zipBackupCreator = zipBackupCreator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _zipBackupCreator.CreateBackup();

            try
            {
                await _scheduler.Start();

                IJobDetail job = JobBuilder.Create<CreateBackupJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithDailyTimeIntervalSchedule(x =>
                        x.WithIntervalInHours(24)
                         .OnEveryDay()
                         .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(21, 00)))
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);

                //await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
