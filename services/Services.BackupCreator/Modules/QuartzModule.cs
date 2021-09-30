using Autofac;
using Autofac.Extras.Quartz;
using Services.BackupCreator.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.BackupCreator.Modules
{
    public class QuartzModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // 1) Register IScheduler
            builder.RegisterModule(new QuartzAutofacFactoryModule());
            // 2) Register jobs
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(CreateBackupJob).Assembly));
        }
    }
}
