using Autofac;
using RawRabbit;
using RawRabbit.Configuration;
using RawRabbit.vNext;
using Services.Common.Models;
using Services.Wrapper.HomeAssistant.Config;
using Services.Wrapper.HomeAssistant.Handlers;
using Services.Wrapper.HomeAssistant.RabbitMq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Services.Wrapper.HomeAssistant.Modules
{
    public class RabbitMqModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<RabbitManager>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .Where(type => type.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<>)) && type.IsClass)
                .AsImplementedInterfaces();

            //RegisterBusClient(builder);

            //var topicTypes = from x in ThisAssembly.GetTypes()
            //                 from z in x.GetInterfaces()
            //                 let y = x.BaseType
            //                 where
            //                 (y != null && y.IsGenericType &&
            //                 typeof(ITopicExecutable<>).IsAssignableFrom(y.GetGenericTypeDefinition())) ||
            //                 (z.IsGenericType &&
            //                 typeof(ITopicExecutable<>).IsAssignableFrom(z.GetGenericTypeDefinition()))
            //                 select x;

            //foreach (var topicType in topicTypes)
            //{
            //    var genericTypes = topicTypes.SelectMany(t => t.GetInterfaces())
            //       .Where(i => i.GetGenericTypeDefinition() == typeof(ITopicExecutable<>))
            //       .SelectMany(i => i.GetGenericArguments());

            //    var topicTypeMethod = topicType.GetMethod("Execute");

                //foreach (var genericType in genericTypes)
                //{
                //    builder.Register(c =>
                //    {
                //        var busClient = c.Resolve<IBusClient>();

                //        var method = typeof(IBusClient).GetMethod("SubscribeAsync");
                //        var genericMethod = method.MakeGenericMethod(genericType);
                        
                //        Expression.Lambda()
                //        new Func<>
                //        asdas
                //        return new TestModelTopic();
                //        //var x = new[] {  };
                //        genericMethod.Invoke(method, new[] { (msg, context) => topicTypeMethod.Invoke(topicType, new[] { msg }) });
                //});
                //}
            //}
        }

        private void RegisterBusClient(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var rabbitMqConfig = c.Resolve<RabbitMqConfiguration>();

                return BusClientFactory.CreateDefault(new RawRabbitConfiguration
                {
                    Hostnames = { rabbitMqConfig.Hostname },
                    Port = rabbitMqConfig.Port
                });
            })
            .AsImplementedInterfaces()
            .SingleInstance();
        }
    }
}
