using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Core.Entities.Services.Models
{
    public class SourceServiceModel : EntityIdService
    {
        public string Source { get; }

        public SourceServiceModel(string entityId, string source) : base(entityId)
        {
            Source = source;
        }
    }
}
