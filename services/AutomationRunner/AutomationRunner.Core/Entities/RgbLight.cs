using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public class RgbLight : BaseEntity
    {
        public enum Name
        {
            [EntityId("light.grzybek")]
            Mushroom,

            [EntityId("light.salon_led")]
            TvLEDs
        }

        public Color Color
        {
            get
            {
                if (!Attributes.ContainsKey("rgb_color"))
                    return default;

                var attributeValue = (JArray)Attributes["rgb_color"];

                return Color.FromArgb(attributeValue[0].Value<int>(),
                    attributeValue[1].Value<int>(), attributeValue[2].Value<int>());
            }
        }

        public byte BrightnessPercent => Convert.ToByte(GetAttributeValue<byte>("brightness") / 2.55);

        public static async Task<RgbLight> LoadFromEntityId(HomeAssistantConnector entityLoader, Name lightName)
        {
            return await entityLoader.LoadEntityFromStates<RgbLight>(lightName.GetEntityId());
        }

        public async Task TurnOn(Color? color = null, byte? brightnessPercent = null, TimeSpan? transitionTime = null)
        {
            var service = new RgbColorService(EntityId);
            if (color != null)
            {
                service.Color = new List<int>(3);
                service.Color.Add(color.Value.R);
                service.Color.Add(color.Value.G);
                service.Color.Add(color.Value.B);
            }

            if (transitionTime != null)
                service.Transition = Convert.ToInt32(transitionTime.Value.TotalSeconds);

            service.BrightnessPercent = brightnessPercent;

            await Connector.SendService("light.turn_on", service);
            State = "on";
        }

        public async Task TurnOff()
        {
            await Connector.SendService("light.turn_off", new EntityIdService(EntityId));
            State = "off";
        }
    }
}
