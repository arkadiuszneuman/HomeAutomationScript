using Autofac;
using Services.BackupCreator.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.BackupCreator.Modules
{
    public class TasksModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(a => a.Namespace != null && a.Namespace.StartsWith(typeof(ZipBackupCreator).Namespace))
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}
