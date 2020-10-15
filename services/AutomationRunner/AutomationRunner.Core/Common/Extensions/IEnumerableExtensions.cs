using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static async Task TurnOffAll(this IEnumerable<Light> lights)
        {
            foreach (var light in lights)
                await light.TurnOff();
        }

        public static async Task TurnOffAll(this IEnumerable<RgbLight> lights)
        {
            foreach (var light in lights)
                await light.TurnOff();
        }

        public static async Task TurnOffAll(this IEnumerable<MediaPlayer> mediaPlayers)
        {
            foreach (var mediaPlayer in mediaPlayers)
                await mediaPlayer.TurnOff();
        }

        public static async Task TurnOffAll(this IEnumerable<Switch> switches)
        {
            foreach (var @switch in switches)
                await @switch.TurnOff();
        }

        public static async Task TurnOnAll(this IEnumerable<Light> lights)
        {
            foreach (var light in lights)
                await light.TurnOn();
        }

        public static async Task TurnOnAll(this IEnumerable<RgbLight> lights, Color? color = null,
            int? brightnessPercent = null, TimeSpan? transitionTime = null)
        {
            foreach (var light in lights)
                await light.TurnOn(color, brightnessPercent, transitionTime);
        }

        public static async Task TurnOnAll(this IEnumerable<Switch> switches)
        {
            foreach (var @switch in switches)
                await @switch.TurnOn();
        }

        public static async Task TurnOnAll(this IEnumerable<MediaPlayer> mediaPlayers)
        {
            foreach (var mediaPlayer in mediaPlayers)
                await mediaPlayer.TurnOn();
        }
    }
}