using AutomationRunner.Core.Common.Connector.Responses;
using AutomationRunner.Core.Entities;
using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific
{
    public interface IStateAutomation
    {
        string EntityName { get; }

        Task Update(BaseEntity oldState, BaseEntity newState);
    }
}