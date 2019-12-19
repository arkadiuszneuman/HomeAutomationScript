using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutomationRunner.Core.Entities.Services.Models
{
    public class RgbColorService : EntityIdService
    {
        public RgbColorService(string entityId) : base(entityId)
        {
        }

        public int Transition { get; set; }

        [JsonProperty("rgb_color")]
        public List<int> Color { get; set; }

        [JsonProperty("brightness_pct")]
        public byte? BrightnessPercent { get; set; }
    }
}
