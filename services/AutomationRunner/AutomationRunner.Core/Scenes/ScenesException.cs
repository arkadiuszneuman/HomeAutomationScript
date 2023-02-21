using AutomationRunner.Core.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Core.Scenes
{
    public class ScenesException : AutomationRunnerException
    {
        public ScenesException()
        {
        }

        public ScenesException(string message) : base(message)
        {
        }

        public ScenesException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
