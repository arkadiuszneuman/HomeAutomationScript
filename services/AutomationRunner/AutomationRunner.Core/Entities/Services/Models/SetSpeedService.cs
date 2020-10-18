using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Core.Entities.Services.Models
{
    public class SetSpeedService : EntityIdService
    {
        public SetSpeedService()
        {
        }

        public SetSpeedService(string entityId, string speedName)
            : base(entityId)
        {
            Speed = speedName;
        }

        public string Speed { get; set; }
    }
}
