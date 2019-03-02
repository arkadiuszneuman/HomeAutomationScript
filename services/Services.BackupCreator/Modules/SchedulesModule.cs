using Autofac;
using Services.BackupCreator.SchedulerJobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.BackupCreator.Modules
{
    public class SchedulesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<ISchedulerJob>()
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}
