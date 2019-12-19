using System;

namespace AutomationRunner.Core.Common
{
    public class EntityIdAttribute : Attribute
    {
        public string Text { get; private set; }

        public EntityIdAttribute(string text)
        {
            Text = text;
        }
    }
}
