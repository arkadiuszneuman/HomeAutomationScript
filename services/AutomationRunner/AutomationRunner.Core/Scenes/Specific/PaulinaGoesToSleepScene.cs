using System;
using System.Threading;
using System.Threading.Tasks;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Scenes.Specific
{
    public class PaulinaGoesToSleepScene : IScene
    {
        private readonly HomeAssistantConnector connector;

        public string Name => "scene.paulina_idzie_spac";

        public PaulinaGoesToSleepScene(HomeAssistantConnector connector)
        {
            this.connector = connector;
        }

        public async Task Activated(CancellationToken cancellationToken = default)
        {
            var spotify = await MediaPlayer.LoadFromEntityId(connector, MediaPlayer.Name.Spotify);
            await spotify.SelectSource("Pokój dziecięcy");
            // await spotify.PlayMedia("https://open.spotify.com/album/1PtHlcG5feBee4IFGgbut0", "album");
            await spotify.SetRepeat(MediaPlayer.RepeatMode.All);
            await spotify.SetRepeat(MediaPlayer.RepeatMode.Off);
            await spotify.SetRepeat(MediaPlayer.RepeatMode.One);
            await spotify.SetRepeat(MediaPlayer.RepeatMode.Off);
            // await spotify.SetVolumeLevel(10);
        }
    }
}