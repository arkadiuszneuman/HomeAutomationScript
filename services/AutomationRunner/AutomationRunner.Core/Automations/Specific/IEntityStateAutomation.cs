using AutomationRunner.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific
{
    public interface IStateUpdate
    {
        Task Update(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity);
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