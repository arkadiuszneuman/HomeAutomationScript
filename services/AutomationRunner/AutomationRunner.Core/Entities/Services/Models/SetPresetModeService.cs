using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AutomationRunner.Core.Entities.Services.Models
{
    public class SetPresetModeService : EntityIdService
    {
        public SetPresetModeService()
        {
        }

        public SetPresetModeService(string entityId, string presetModeName)
            : base(entityId)
        {
            PresetMode = presetModeName;
        }

        [JsonProperty("preset_mode")]
        public string PresetMode { get; set; }
    }
}
