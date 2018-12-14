using Services.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Wrapper.HomeAssistant.RabbitTopics
{
    public class TestModelTopic : ITopicExecutable<TestModel>
    {
        public void Execute(TestModel model)
        {
            
        }
    }
}
