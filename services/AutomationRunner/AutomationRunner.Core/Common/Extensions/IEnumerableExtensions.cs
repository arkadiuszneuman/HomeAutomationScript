using AutomationRunner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static async Task TurnOffAll(this IEnumerable<Task<Light>> lights)
        {
            foreach (var light in lights)
                await (await light).TurnOff();
        }

        public static async Task TurnOffAll(this IEnumerable<Task<RgbLight>> lights)
        {
            foreach (var light in lights)
                await (await light).TurnOff();
        }

        public static async Task TurnOffAll(this IEnumerable<Task<Switch>> switches)
        {
            foreach (var @switch in switches)
                await (await @switch).TurnOff();
        }

        public static async Task TurnOnAll(this IEnumerable<Task<Light>> lights)
        {
            foreach (var light in lights)
                await (await light).TurnOn();
        }

        public static async Task TurnOnAll(this IEnumerable<Task<RgbLight>> lights, Color? color = null, int? brightnessPercent = null, TimeSpan? transitionTime = null)
        {
            foreach (var light in lights)
                await (await light).TurnOn(color, brightnessPercent, transitionTime);
        }

        public static async Task TurnOnAll(this IEnumerable<Task<Switch>> switches)
        {
            foreach (var @switch in switches)
                await (await @switch).TurnOn();
        }
    }
}
