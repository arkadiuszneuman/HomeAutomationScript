using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AutomationRunner.Core.Entities.Services.Models
{
    public class SetModeService : EntityIdService
    {
        public SetModeService()
        {
        }

        public SetModeService(string entityId, string modeName)
            : base(entityId)
        {
            Mode = modeName;
        }

        [JsonProperty("mode")]
        public string Mode { get; set; }
    }
}
