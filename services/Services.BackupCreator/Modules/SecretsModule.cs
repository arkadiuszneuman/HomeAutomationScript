using Autofac;
using Services.BackupCreator.Secrets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.BackupCreator.Modules
{
    public class SecretsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SecretsLoader>()
                .AsSelf();

            builder.Register(c =>
            {
                var secretLoader = c.Resolve<SecretsLoader>();
                return secretLoader.LoadSecrets();
            })
            .AsSelf()
            .SingleInstance();
        }
    }
}
