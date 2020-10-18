using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Logging;
using Quartz;
using Services.BackupCreator.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.BackupCreator.Jobs
{
    public class CreateBackupJob : IJob
    {
        private readonly ILogger _logger;
        private readonly ZipBackupCreator _backupCreator;
        private readonly DropboxSender _dropboxSender;

        public CreateBackupJob(ILogger<BackupCreatorService> logger, ZipBackupCreator backupCreator,
            DropboxSender dropboxSender)
        {
            _logger = logger;
            _backupCreator = backupCreator;
            _dropboxSender = dropboxSender;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Backup job started");
            try
            {
                var backupFile = _backupCreator.CreateBackup();
                await _dropboxSender.BackupToDropbox(backupFile);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Error in backup creation");
            }
        }
    }
}
