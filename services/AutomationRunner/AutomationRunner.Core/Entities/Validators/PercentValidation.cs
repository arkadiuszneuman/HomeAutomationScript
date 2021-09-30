using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Core.Entities.Validators
{
    public class PercentValidation : AbstractValidator<int>
    {
        public PercentValidation()
        {
            RuleFor(x => x).InclusiveBetween(0, 100);
        }
    }
}
