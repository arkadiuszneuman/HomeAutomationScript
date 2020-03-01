using Microsoft.Extensions.Logging;
using RestSharp;
using Services.Wrapper.HomeAssistant.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.MQTT.Topics.SubscribedTopics
{
    [DebuggerDisplay("MinLevelModel: {Level}")]
    public class MinLevelModel
    {
        public decimal Level { get; set; }
    }

    [DebuggerDisplay("MaxLeveModel: {Level}")]
    public class MaxLevelModel
    {
        public decimal Level { get; set; }
    }

    public class StairsParametersHandler : ISubscribedTopic<MinLevelModel>, ISubscribedTopic<MaxLevelModel>
    {
        private readonly ILogger<StairsParametersHandler> _logger;
        private readonly IRestClient _restClient;
        private readonly StairsConfiguration _stairsConfiguration;

        string ISubscribedTopic<MinLevelModel>.TopicName => "home/stairs/minlevel/set";
        string ISubscribedTopic<MaxLevelModel>.TopicName => "home/stairs/maxlevel/set";

        public StairsParametersHandler(ILogger<StairsParametersHandler> logger,
            IRestClient restClient,
            StairsConfiguration stairsConfiguration)
        {
            _logger = logger;
            _restClient = restClient;
            _stairsConfiguration = stairsConfiguration;
        }

        public async Task Handle(MinLevelModel message)
        {
            _restClient.BaseUrl = new Uri(_stairsConfiguration.Hostname);
            var request = new RestRequest(_stairsConfiguration.Resource);
            request.AddParameter("minlevel", message.Level);

            _logger.LogInformation("Sending stairs minimum level {level}", message.Level);
            var response = await _restClient.ExecuteTaskAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
                _logger.LogWarning("Invalid response code for request");
        }

        public async Task Handle(MaxLevelModel message)
        {
            _restClient.BaseUrl = new Uri(_stairsConfiguration.Hostname);
            var request = new RestRequest(_stairsConfiguration.Resource);
            request.AddParameter("maxlevel", message.Level);

            _logger.LogInformation("Sending stairs maximum level {level}", message.Level);
            var response = await _restClient.ExecuteTaskAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
                _logger.LogWarning("Invalid response code for request");
        }
    }
}
