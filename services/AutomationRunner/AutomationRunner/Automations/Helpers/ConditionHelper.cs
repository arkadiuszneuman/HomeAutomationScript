﻿using AutomationRunner.Common;
using System;

namespace AutomationRunner.Automations.Helpers
{
    public class ConditionHelper
    {
        private readonly DateTimeHelper dateTimeHelper;
        private TimeSpan forTime;
        private DateTime? firstDate;
        private bool shouldActionBeExecuted = true;

        public ConditionHelper(DateTimeHelper dateTimeHelper)
        {
            this.dateTimeHelper = dateTimeHelper;
        }

        public ConditionHelper For(TimeSpan timeSpan)
        {
            forTime = timeSpan;
            return this;
        }

        public bool CheckFulfilled(bool condition)
        {
            if (condition)
            {
                if (shouldActionBeExecuted)
                {
                    if (firstDate == null)
                    {
                        firstDate = dateTimeHelper.Now;
                    }

                    if (firstDate + forTime <= dateTimeHelper.Now)
                    {
                        shouldActionBeExecuted = false;
                        return true;
                    }
                }
            }
            else
            {
                firstDate = null;
                shouldActionBeExecuted = true;
            }

            return false;
        }
    }
}
