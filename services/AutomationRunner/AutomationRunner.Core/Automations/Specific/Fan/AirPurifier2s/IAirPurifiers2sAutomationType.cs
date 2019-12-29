using System.Threading.Tasks;

namespace AutomationRunner.Core.Automations.Specific.Fan.AirPurifier2s
{
    public interface IAirPurifiers2sAutomationType
    {
        public string AutomationType { get; }
        Task Update();
    }
}
