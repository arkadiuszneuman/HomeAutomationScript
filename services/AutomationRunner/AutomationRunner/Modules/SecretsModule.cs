using Autofac;
using AutomationRunner.Core.Secrets;

namespace AutomationRunner.Modules
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
