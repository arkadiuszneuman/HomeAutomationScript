using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific
{
    public interface IAutomation
    {
        Task Update();
    }
}
