using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using AutomationRunner.Core.Entities.Validators;
using FluentValidation;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public class InputNumber : BaseEntity
    {
        public enum Name
        {
            [EntityId("input_number.denon_volume")]
            DenonVolume,

            [EntityId("input_number.stairs_min_brightness")]
            StairsMinimumBrightness,

            [EntityId("input_number.stairs_max_brightness")]
            StairsMaximumBrightness
        }

        public static async Task<InputNumber> LoadFromEntityId(HomeAssistantConnector connector, Name inputNumberName)
        {
            return await connector.LoadEntityFromStates<InputNumber>(inputNumberName.GetEntityId());
        }

        public async Task SetValue(int value)
        {
            new PercentValidation().ValidateAndThrow(value);

            await Connector.SendService("input_number.set_value", new ValueServiceModel(EntityId, value));
        }

        public async Task SetValueBasedOnTvState()
        {
            var tv = await MediaPlayer.LoadFromEntityId(Connector, MediaPlayer.Name.Tv);
            await (tv.State == "on" ? SetValue(5) : SetValue(20));
        }
    }
}
