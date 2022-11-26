using AutomationRunner.Core.Common;
using AutomationRunner.Core.Common.Connector;
using AutomationRunner.Core.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Entities
{
    public record DeviceTracker : BaseEntity
    {
        public enum Name
        {
            [EntityId("device_tracker.arek")]
            Arek,

            [EntityId("device_tracker.patrycja")]
            Patrycja
        }

        public int BatteryLevel => GetAttributeValue<int>("battery");
        public bool BatteryCharging => GetAttributeValue<bool>("battery_charging");
        public decimal Latitude => GetAttributeValue<decimal>("latitude");
        public decimal Longitude => GetAttributeValue<decimal>("longitude");


        public static async Task<DeviceTracker> LoadFromEntityId(HomeAssistantConnector connector, Name deviceTrackerName)
        {
            return await connector.LoadEntityFromStates<DeviceTracker>(deviceTrackerName.GetEntityId());
        }

        public static IEnumerable<Task<DeviceTracker>> LoadFromEntitiesId(HomeAssistantConnector connector, params Name[] deviceTrackerNames)
        {
            foreach (var lightName in deviceTrackerNames)
                yield return LoadFromEntityId(connector, lightName);
        }

        public static IEnumerable<Task<DeviceTracker>> LoadAllDeviceTrackers(HomeAssistantConnector connector, params Name[] except)
        {
            foreach (Name deviceTrackerName in ((Name[])Enum.GetValues(typeof(Name))).Except(except))
                yield return LoadFromEntityId(connector, deviceTrackerName);
        }

        public static async Task<bool> IsAnyoneHome(HomeAssistantConnector connector, params Name[] except)
        {
            var usersToFind = new[] { Name.Arek, Name.Patrycja }.Except(except);
            var deviceTrackersTasks = LoadFromEntitiesId(connector, usersToFind.ToArray());

            var deviceTrackers = new List<DeviceTracker>();
            foreach (var deviceTrackerTask in deviceTrackersTasks)
                deviceTrackers.Add(await deviceTrackerTask);

            return deviceTrackers.Any(t => t.State == "home");
        }
    }
}
