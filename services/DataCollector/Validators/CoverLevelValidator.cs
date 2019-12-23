using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCollector.Validators
{
    public class CoverLevelValidator : AbstractValidator<int>
    {
        public CoverLevelValidator()
        {
            RuleFor(x => x).InclusiveBetween(0, 100);
        }
    }
}
