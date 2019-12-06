using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Common
{
    public class AutomationRunnerException : Exception
    {

        public AutomationRunnerException() { }

        public AutomationRunnerException(string? message) : base(message) { }

        public AutomationRunnerException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
