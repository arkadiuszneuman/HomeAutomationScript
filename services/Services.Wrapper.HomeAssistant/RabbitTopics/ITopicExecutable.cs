using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Wrapper.HomeAssistant.RabbitTopics
{
    public interface ITopicExecutable<T>
    {
        void Execute(T model);
    }
}
