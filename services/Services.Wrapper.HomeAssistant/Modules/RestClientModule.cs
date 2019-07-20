using Autofac;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Wrapper.HomeAssistant.Modules
{
    public class RestClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<RestClient>()
                .As<IRestClient>();
        }
    }
}
