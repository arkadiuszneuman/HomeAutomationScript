﻿using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public record MediaPlayer : BaseEntity
    {
        public enum Name
        {
            [EntityId("media_player.denon_avr_x1400h")]
            Denon,

            [EntityId("media_player.playstation_4")]
            Playstation4,

            [EntityId("media_player.sony_bravia_tv")]
            Tv,

            [EntityId("media_player.spotify_arkadiusz_neuman")]
            Spotify
        }
        
        public enum RepeatMode
        {
            All,
            One,
            Off
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
        
        public static async Task<IList<MediaPlayer>> LoadAll(HomeAssistantConnector connector, params Name[] except)
        {
            var mediaPlayers = new List<MediaPlayer>();
            foreach (Name lightName in ((Name[])Enum.GetValues(typeof(Name))).Except(except))
                mediaPlayers.Add(await LoadFromEntityId(connector, lightName));

            return mediaPlayers;
        }

        public async Task TurnOn()
        {
            await Connector.SendServiceAsync("media_player.turn_on", new EntityIdService(EntityId));
            State = "on";
        }

        public async Task TurnOff()
        {
            await Connector.SendServiceAsync("media_player.turn_off", new EntityIdService(EntityId));
            State = "off";
        }

        public async Task SelectSource(string source) =>
            await Connector.SendServiceAsync("media_player.select_source", new SourceServiceModel(EntityId, source));

        public async Task SetVolumeLevel(int volume) =>
            await Connector.SendServiceAsync("media_player.volume_set", new VolumeServiceModel(EntityId, volume));

        public async Task Play() => 
            await Connector.SendServiceAsync("media_player.media_play", new EntityIdService(EntityId));
        
        public async Task PlayMedia(string contentId, string contentType) => 
            await Connector.SendServiceAsync("media_player.play_media", new PlayMediaServiceModel(EntityId, contentId, contentType));

        public async Task SetRepeat(RepeatMode repeatMode) => 
            await Connector.SendServiceAsync("media_player.repeat_set", new RepeatModeServiceModel(EntityId, repeatMode));

        public static MediaPlayer CreateBasedOnBaseEntity(BaseEntity state)
        {
            var entity = new MediaPlayer();
            entity.UpdateEntity(state);
            return entity;
        }
    }
}
