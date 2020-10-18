using System;
using System.Collections.Generic;
using System.Text;

namespace Services.BackupCreator.Config
{
    public class SchedulersConfig
    {
        public int BackupHourOfADay { get; set; }
        public int BackupMinuteOfADay { get; set; }
    }
}
