using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.BackupCreator.SchedulerJobs
{
    public interface ISchedulerJob
    {
        Task ScheduleJobAsync();
        Task ShutdownAsync();
    }
}
