using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Core.Entities.Services.Models
{
    public class ValueServiceModel : EntityIdService
    {
        public int Value { get; set; }

        public ValueServiceModel(string entityId, int value) : base(entityId)
        {
            Value = value;
        }
    }
}
