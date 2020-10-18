using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Core.Common.Exceptions
{
    public class ValidationException : AutomationRunnerException
    {
        public ValidationException(string? message)
            : base (message)
        {

        }
    }
}
