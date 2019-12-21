using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Core.Entities.Validators
{
    public class VolumeValidator : AbstractValidator<double>
    {
        public VolumeValidator()
        {
            RuleFor(x => x).InclusiveBetween(0, 100);
        }
    }
}
