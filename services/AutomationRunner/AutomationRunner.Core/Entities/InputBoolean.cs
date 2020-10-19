using AutomationRunner.Core.Common;

namespace AutomationRunner.Core.Entities
{
    public class InputBoolean : BaseEntity
    {
        public enum Name
        {
            [EntityId("input_boolean.auto_office_light")]
            AutomaticOfficeLight
        }

        public bool IsSwitchedOn()
        {
            return State == "on";
        }
    }
}