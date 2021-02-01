using System.Threading.Tasks;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Entities.Services.Models;

namespace AutomationRunner.Core.Entities
{
    public class InputBoolean : BaseEntity
    {
        public enum Name
        {
            [EntityId("input_boolean.auto_office_light")]
            AutomaticOfficeLight
        }

        public bool IsSwitchedOn() => State == "on";

        public async Task TurnOn()
        {
            await Connector.SendService("input_boolean.turn_on", new EntityIdService(EntityId));
            State = "on";
        }
        
        public async Task TurnOff()
        {
            await Connector.SendService("input_boolean.turn_off", new EntityIdService(EntityId));
            State = "off";
        }
    }
}