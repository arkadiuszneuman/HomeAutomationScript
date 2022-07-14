using System;
using System.Collections.Generic;
using System.Text;

namespace Services.BackupCreator.Config
{
    public class PathsConfig
    {
        public string TempBackupPath { get; set; }
        public IEnumerable<string> DirsToBackup { get; set; }
    }
}
