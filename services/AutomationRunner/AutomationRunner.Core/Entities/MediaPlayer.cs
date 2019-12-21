using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public class MediaPlayer : BaseEntity
    {
        public enum Name
        {
            [EntityId("media_player.denon")]
            Denon,

            [EntityId("media_player.playstation_4")]
            Playstation4,

            [EntityId("media_player.sony_bravia")]
            Tv,

            [EntityId("media_player.spotify")]
            Spotify
        }

        public static async Task<MediaPlayer> LoadFromEntityId(HomeAssistantConnector entityLoader, Name lightName)
        {
            return await entityLoader.LoadEntityFromStates<MediaPlayer>(lightName.GetEntityId());
        }

        public static IEnumerable<Task<MediaPlayer>> LoadFromEntitiesId(HomeAssistantConnector connector, params Name[] lightNames)
        {
            foreach (var lightName in lightNames)
                yield return LoadFromEntityId(connector, lightName);
        }

        public static IEnumerable<Task<MediaPlayer>> LoadAll(HomeAssistantConnector connector, params Name[] except)
        {
            foreach (Name lightName in ((Name[])Enum.GetValues(typeof(Name))).Except(except))
                yield return LoadFromEntityId(connector, lightName);
        }

        public async Task TurnOn()
        {
            await Connector.SendService("media_player.turn_on", new EntityIdService(EntityId));
            State = "on";
        }

        public async Task TurnOff()
        {
            await Connector.SendService("media_player.turn_off", new EntityIdService(EntityId));
            State = "off";
        }

        public async Task SelectSource(string source)
        {
            await Connector.SendService("media_player.select_source", new SourceServiceModel(EntityId, source));
        }

        public async Task SetVolumeLevel(int volume)
        {
            await Connector.SendService("media_player.volume_set", new VolumeServiceModel(EntityId, volume));
        }
    }
}
