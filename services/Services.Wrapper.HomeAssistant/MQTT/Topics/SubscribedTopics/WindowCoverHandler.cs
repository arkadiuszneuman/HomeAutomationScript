﻿using Microsoft.Extensions.Logging;
using RestSharp;
using Services.Wrapper.HomeAssistant.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.MQTT.Topics.SubscribedTopics
{
    public class WindowCoverStateModel
    {
        public string State { get; set; }
    }

    public class WindowCoverSetPositionModel : IMessageModel
    {
        public string Message { get; set; }
    }

    public class WindowCoverHandler : ISubscribedTopic<WindowCoverStateModel>, 
        ISubscribedTopic<WindowCoverSetPositionModel>
    {
        private bool isFirstRun = true;
        private readonly ILogger<WindowCoverHandler> logger;
        private readonly IRestClient restClient;
        private readonly WindowCoverConfiguration windowCoverConfiguration;

        string ISubscribedTopic<WindowCoverStateModel>.TopicName => "home/cover/set";
        string ISubscribedTopic<WindowCoverSetPositionModel>.TopicName => "home/cover/set_position";

        public WindowCoverHandler(ILogger<WindowCoverHandler> logger,
            IRestClient restClient,
            WindowCoverConfiguration windowCoverConfiguration)
        {
            this.logger = logger;
            this.restClient = restClient;
            this.windowCoverConfiguration = windowCoverConfiguration;
        }

        public async Task Handle(WindowCoverStateModel message)
        {
            if (isFirstRun)
            {
                isFirstRun = false;
                return;
            }

            logger.LogInformation("Got message {message}", message.State);

            restClient.BaseUrl = new Uri(windowCoverConfiguration.Hostname);
            var request = new RestRequest(windowCoverConfiguration.Resource);

            int levelToSet = message.State switch
            {
                "open" => 0,
                "close" => 100,
                "stop" => 0,
                _ => throw new ArgumentException("Invalid level")
            };

            request.AddParameter("level", levelToSet);

            await restClient.ExecuteTaskAsync(request);
        }

        public async Task Handle(WindowCoverSetPositionModel message)
        {
            restClient.BaseUrl = new Uri(windowCoverConfiguration.Hostname);
            var request = new RestRequest(windowCoverConfiguration.Resource);
            request.AddParameter("level", message.Message);

            await restClient.ExecuteTaskAsync(request);
        }
    }
}
