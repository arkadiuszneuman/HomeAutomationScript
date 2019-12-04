using AutomationRunner.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Automations.Helpers
{
    public class AutomationHelpersFactory
    {
        private readonly DateTimeHelper dateTimeHelper;

        public AutomationHelpersFactory(DateTimeHelper dateTimeHelper)
        {
            this.dateTimeHelper = dateTimeHelper;
        }

        public ConditionHelper GetConditionHelper() => new ConditionHelper(dateTimeHelper);
    }
}
