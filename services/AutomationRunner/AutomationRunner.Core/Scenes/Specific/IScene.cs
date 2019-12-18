using System.Threading.Tasks;

namespace AutomationRunner.Core.Scenes.Specific
{
    public interface IScene
    {
        public string Name { get; }

        public Task Activated();
    }
}
