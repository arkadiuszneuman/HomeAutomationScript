using AutomationRunner.Core.Common.Exceptions;

namespace AutomationRunner.Core.Entities.Services.Models
{
    public class XiaomiMiioSetFavoriteLevelService : EntityIdService
    {
        public XiaomiMiioSetFavoriteLevelService(string entityId, int value)
            : base(entityId)
        {
            if (value < 0 || value > 16)
                throw new ValidationException("Level should be between 0 and 16");

            Value = value;
        }

        public int Value { get; private set; }
    }
}
