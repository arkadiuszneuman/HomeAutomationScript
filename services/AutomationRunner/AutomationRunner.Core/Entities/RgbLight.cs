using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using AutomationRunner.Core.Entities.Validators;
using FluentValidation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public record RgbLight : BaseEntity
    {
        public enum Name
        {
            [EntityId("light.grzybek")]
            Mushroom,
            [EntityId("light.salon_led")]
            TvLEDs,
            [EntityId("light.halogen_5")]
            Halogen5,
            [EntityId("light.halogen_6")]
            Halogen6,
            [EntityId("light.lampa_gabinet")]
            OfficeBigLight,
            [EntityId("light.lampka_gabinet_1")]
            OfficeSmallLight
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

        public static async Task<IList<RgbLight>> LoadAllLights(HomeAssistantConnector connector, params Name[] except)
        {
            var tasks = new List<RgbLight>();
            foreach (Name lightName in ((Name[])Enum.GetValues(typeof(Name))).Except(except))
                tasks.Add(await LoadFromEntityId(connector, lightName));

            return tasks;
        }

        public async Task TurnOn(Color? color = null, int? brightnessPercent = null, TimeSpan? transitionTime = null)
        {
            var service = new RgbColorService(EntityId);

            if (brightnessPercent.HasValue)
            {
                await new PercentValidation().ValidateAndThrowAsync(brightnessPercent.Value);
                service.BrightnessPercent = brightnessPercent;
            }

            if (color != null)
            {
                service.Color = new List<int>(3)
                {
                    color.Value.R,
                    color.Value.G,
                    color.Value.B
                };
            }

            if (transitionTime != null)
                service.Transition = Convert.ToInt32(transitionTime.Value.TotalSeconds);


            await Connector.SendServiceAsync("light.turn_on", service);
            State = "on";
        }
        
        public async Task Turn(bool on, Color? color = null, int? brightnessPercent = null, TimeSpan? transitionTime = null)
        {
            if (on)
                await TurnOn(color, brightnessPercent, transitionTime);
            else
                await TurnOff();
        }

        public Task TurnOnStandardWhite(int brightnessPercent = 100) => 
            TurnOn(Color.FromArgb(255, 255, 163, 72), brightnessPercent);

        public Task TurnOnWithRandomColor()
        {
            var random = new Random();
            return TurnOn(Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)));
        }

        public async Task TurnOff()
        {
            await Connector.SendServiceAsync("light.turn_off", new EntityIdService(EntityId));
            State = "off";
        }
    }
}
