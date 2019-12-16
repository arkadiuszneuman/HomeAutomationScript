using AutomationRunner.Core.Common;
using Microsoft.Extensions.Logging;
using System;

namespace AutomationRunner.Core.Automations.Helpers
{
    public class ConditionHelper
    {
        private readonly IDateTimeHelper dateTimeHelper;
        private TimeSpan forTime;
        private DateTime? firstDate;
        private bool shouldActionBeExecuted = true;
        private string name;
        private ILogger logger;

        public ConditionHelper(IDateTimeHelper dateTimeHelper)
        {
            this.dateTimeHelper = dateTimeHelper;
        }

        public ConditionHelper For(TimeSpan timeSpan)
        {
            forTime = timeSpan;
            return this;
        }

        public ConditionHelper Name(ILogger logger, string name)
        {
            this.name = name;
            this.logger = logger;
            return this;
        }

        public bool CheckFulfilled(bool condition)
        {
            Log($"Condition fulfilled: {condition}");
            if (condition)
            {
                Log($"Action should be executed: {shouldActionBeExecuted}");
                if (shouldActionBeExecuted)
                {
                    if (firstDate == null)
                    {
                        firstDate = dateTimeHelper.Now;
                        Log($"Initialized first date: {firstDate}. Action be executed on: {firstDate + forTime}");
                    }

                    if (firstDate + forTime <= dateTimeHelper.Now)
                    {
                        Log($"Executing action. Zeroing action should be executed");
                        shouldActionBeExecuted = false;
                        return true;
                    }
                }
            }
            else
            {
                Log($"Zeroing firstDate. Action should be executed: true");
                firstDate = null;
                shouldActionBeExecuted = true;
            }

            return false;
        }

        private void Log(string msg)
        {
            if (logger != null)
            {
                if (string.IsNullOrEmpty(name))
                    logger.LogDebug("{Message}", msg);
                else
                    logger.LogDebug("{Name}: {Message}", name, msg);
            }
        }

        public void Reset()
        {
            shouldActionBeExecuted = true;
        }
    }
}
