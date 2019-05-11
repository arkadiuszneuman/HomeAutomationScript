using Microsoft.Extensions.Logging;
using Services.Common.Models;
using Services.Wrapper.HomeAssistant.MQTT;
using Services.Wrapper.HomeAssistant.MQTT.Topics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.Handlers
{
    public class TriggeredSensorHandler : IHandler<TriggeredUpperStairSensorModel>, IHandler<TriggeredBottomStairSensorModel>
    {
        private readonly MqttManager _mqttManager;
        private readonly ILogger<TriggeredSensorHandler> _logger;
        private DateTime _lastTimeBottomSensorDetected = DateTime.MinValue;
        private DateTime _lastTimeUpperSensorDetected = DateTime.MinValue;

        public TriggeredSensorHandler(MqttManager mqttManager,
            ILogger<TriggeredSensorHandler> logger)
        {
            _mqttManager = mqttManager;
            _logger = logger;
        }

        public async Task Execute(TriggeredUpperStairSensorModel model)
        {
            _logger.LogInformation("Triggered upper stairs sensor at {time}", model.DateTime);
            await _mqttManager.Publish<UpperMotionDetectorTopic>(c => c.ON);

            var differenceBetweenSensorsTime = DateTime.Now - _lastTimeBottomSensorDetected;

            if (differenceBetweenSensorsTime >= TimeSpan.FromSeconds(3) &&
                differenceBetweenSensorsTime <= TimeSpan.FromSeconds(30))
            {
                _logger.LogInformation("Sending set_going_up on MQTT");
                _lastTimeBottomSensorDetected = DateTime.MinValue;
                await _mqttManager.Publish<SetGoingUpTopic>(c => c.ON);
            }

            _lastTimeUpperSensorDetected = model.DateTime;

            await Task.Delay(5000);

            await _mqttManager.Publish<UpperMotionDetectorTopic>(c => c.OFF);
            await _mqttManager.Publish<SetGoingUpTopic>(c => c.OFF);
        }

        public async Task Execute(TriggeredBottomStairSensorModel model)
        {
            _logger.LogInformation("Triggered bottom stairs sensor at {time}", model.DateTime);

            await _mqttManager.Publish<BottomMotionDetectorTopic>(c => c.ON);

            var differenceBetweenSensorsTime = DateTime.Now - _lastTimeUpperSensorDetected;
            if (differenceBetweenSensorsTime >= TimeSpan.FromSeconds(3) &&
                differenceBetweenSensorsTime <= TimeSpan.FromSeconds(30))
            {
                _logger.LogInformation($"Sending set_going_down on MQTT");
                _lastTimeUpperSensorDetected = DateTime.MinValue;
                await _mqttManager.Publish<SetGoingDownTopic>(c => c.ON);
            }

            _lastTimeBottomSensorDetected = model.DateTime;

            await Task.Delay(5000);

            await _mqttManager.Publish<BottomMotionDetectorTopic>(c => c.OFF);
            await _mqttManager.Publish<SetGoingDownTopic>(c => c.OFF);
        }
    }
}
