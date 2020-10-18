namespace AutomationRunner.Core.Entities.Services.Models
{
    public class OptionServiceModel : EntityIdService
    {
        public string Option { get; }

        public OptionServiceModel(string entityId, string option) : base(entityId)
        {
            Option = option;
        }
    }
}
