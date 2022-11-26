using AutomationRunner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Core.Common.Connector
{
    public class OldNewState
    {
        public BaseEntity? OldState { get; }
        public BaseEntity? NewState { get; }

        public OldNewState(BaseEntity? oldState, BaseEntity? newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}
