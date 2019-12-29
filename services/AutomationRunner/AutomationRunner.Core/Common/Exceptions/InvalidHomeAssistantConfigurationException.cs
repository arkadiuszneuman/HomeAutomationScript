namespace AutomationRunner.Core.Common.Exceptions
{
    public class InvalidHomeAssistantConfigurationException : AutomationRunnerException
    {
        public InvalidHomeAssistantConfigurationException(string message) : base(message)
        {
        }
    }
}
