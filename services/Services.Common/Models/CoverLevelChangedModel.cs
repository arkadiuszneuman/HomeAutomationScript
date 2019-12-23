using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Common.Models
{
    public class CoverLevelChangedModel
    {
        public int Level { get; }

        public CoverLevelChangedModel(int level)
        {
            Level = level;
        }
    }
}
