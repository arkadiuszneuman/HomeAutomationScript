using AutomationRunner.Core.Entities.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Core.Entities.Services.Models
{
    public class CoverPositionServiceModel : EntityIdService
    {
        public int Position { get; }

        public CoverPositionServiceModel(string entityId, int position) : base(entityId)
        {
            new PercentValidation().ValidateAndThrow(position);
            Position = position;
        }
    }
}
