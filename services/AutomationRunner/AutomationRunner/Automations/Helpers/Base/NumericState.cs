using AutomationRunner.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutomationRunner.Automations.Helpers.Base
{
    public abstract class NumericState<T>
        where T: BaseEntity
    {
        public abstract Task<T> Entity { get; }

    }
}
