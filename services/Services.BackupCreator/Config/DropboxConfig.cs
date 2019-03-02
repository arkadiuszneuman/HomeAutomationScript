using System;
using System.Collections.Generic;
using System.Text;

namespace Services.BackupCreator.Config
{
    public class DropboxConfig
    {
        public int MaxFiles { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
