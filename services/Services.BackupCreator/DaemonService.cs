using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Services.BackupCreator.Jobs;
using Services.BackupCreator.SchedulerJobs;
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
        private readonly IEnumerable<ISchedulerJob> _schedulerJobs;

        public BackupCreatorService(IEnumerable<ISchedulerJob> schedulerJobs)
        {
            _schedulerJobs = schedulerJobs;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var job in _schedulerJobs)
            {
                await job.ScheduleJobAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var job in _schedulerJobs)
            {
                await job.ShutdownAsync();
            }
        }
    }
}
