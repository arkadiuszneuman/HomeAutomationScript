﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.MQTT.Topics.SubscribedTopics
{
    public interface ISubscribedTopic<T> : ITopic
    {
        Task Handle(T message);
    }
}
