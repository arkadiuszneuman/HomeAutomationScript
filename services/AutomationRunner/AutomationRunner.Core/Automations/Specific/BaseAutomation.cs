using System.Threading.Tasks;
using AutomationRunner.Core.Entities;

namespace AutomationRunner.Core.Automations.Specific
{
    public abstract class BaseAutomation : IStateUpdate, IShouldUpdate
    {
        public abstract Task UpdateAsync(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity);
        public virtual bool Enabled => true;

        public virtual Task<bool> ShouldUpdate(BaseEntity oldStateBaseEntity, BaseEntity newStateBaseEntity) =>
            Task.FromResult(true);
    }
}