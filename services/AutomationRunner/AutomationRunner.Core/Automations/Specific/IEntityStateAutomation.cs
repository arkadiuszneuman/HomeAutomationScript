using AutomationRunner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific
{
    public interface IStateUpdate
    {
        Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity);
    }

    public interface IShouldUpdate
    {
        Task<bool> ShouldUpdate(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity);
    }

    public interface ITimeUpdate
    {
        TimeSpan UpdateEvery { get; }
        Task Update();
    }

    public interface IEntityStateAutomation : IStateUpdate
    {
        string EntityName { get; }
    }

    public interface IEntitiesStateAutomation : IStateUpdate
    {
        IEnumerable<string> EntityNames { get; }
    }
}