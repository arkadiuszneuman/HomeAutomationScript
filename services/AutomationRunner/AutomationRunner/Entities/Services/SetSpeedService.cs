using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Entities.Services
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
