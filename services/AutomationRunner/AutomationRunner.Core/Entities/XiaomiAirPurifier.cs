using System;
using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public enum AirPurifierPresetMode
    {
        Auto = 1,
        Favorite,
        Silent
    }

    public class XiaomiAirPurifier : BaseEntity
    {
        public enum Name
        {
            [EntityId("fan.filtr_powietrza")]
            AirPurifierPro,

            [EntityId("fan.mi_air_purifier_2s")]
            AirPurifier2S
        }

        public int Aqi => int.TryParse(pm25Sensor.State, out var value) ? value : 0;

        public int FavoriteLevel => Convert.ToInt32(favoriteLevelNumber.State);
        public AirPurifierPresetMode PresetMode => GetAttributeValue<AirPurifierPresetMode>("preset_mode");

        private BaseEntity pm25Sensor;
        private BaseEntity favoriteLevelNumber;

        public static async Task<XiaomiAirPurifier> LoadFromEntityId(HomeAssistantConnector connector, Name entityId)
        {
            var purifier =  await connector.LoadEntityFromStates<XiaomiAirPurifier>(entityId.GetEntityId());
            purifier.pm25Sensor = await connector.LoadEntityFromStates<BaseEntity>($"sensor.{entityId.GetEntityId().Replace(".", "")}_pm2_5");
            purifier.favoriteLevelNumber = await connector.LoadEntityFromStates<BaseEntity>($"number.{entityId.GetEntityId().Replace(".", "")}_favorite_level");

            return purifier;
        }

        public async Task TurnOn()
        {
            await Connector.SendService("fan.turn_on", new EntityIdService(EntityId));
            State = "on";
        }

        public async Task TurnOff()
        {
            await Connector.SendService("fan.turn_off", new EntityIdService(EntityId));
            State = "off";
        }

        public async Task SetPresetMode(AirPurifierPresetMode presetMode)
        {
            await Connector.SendService("fan.set_preset_mode", new SetPresetModeService(EntityId, presetMode.ToString()));
        }

        public async Task SetFavoriteLevel(int level)
        {
            await Connector.SendService("number.set_value",
                new XiaomiMiioSetFavoriteLevelService($"number.{EntityId.Replace(".", "")}_favorite_level", level));
        }
    }
}
