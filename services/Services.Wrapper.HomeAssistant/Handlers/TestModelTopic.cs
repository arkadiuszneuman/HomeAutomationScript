using Microsoft.Extensions.Logging;
using Services.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.Handlers
{
    public class TestModelTopic : IHandler<TestModel>
    {
        private readonly ILogger<TestModelTopic> _logger;

        public TestModelTopic(ILogger<TestModelTopic> logger)
        {
            _logger = logger;
        }

        public Task Execute(TestModel model)
        {
            _logger.LogInformation($"Received: {model.Message}");
            return Task.CompletedTask;
        }
    }
}
