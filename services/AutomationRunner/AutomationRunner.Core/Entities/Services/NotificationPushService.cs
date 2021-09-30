using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Entities.Services.Models;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities.Services
{
    public class NotificationPushService
    {
        private readonly HomeAssistantConnector connector;

        public NotificationPushService(HomeAssistantConnector connector)
        {
            this.connector = connector;
        }

        public async Task PushNotification(string text)
        {
            await connector.SendService("notify.push", new NotificationServiceModel(text));
        }
    }
}
