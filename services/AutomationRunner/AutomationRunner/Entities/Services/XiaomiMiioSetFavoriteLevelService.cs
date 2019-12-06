using AutomationRunner.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Entities.Services
{
    public class XiaomiMiioSetFavoriteLevelService : EntityIdService
    {
        public XiaomiMiioSetFavoriteLevelService(string entityId, int level)
            : base(entityId)
        {
            if (level < 0 || level > 16)
                throw new ValidationException("Level should be between 0 and 16");

            Level = level;
        }

        public int Level { get; private set; }
    }
}
