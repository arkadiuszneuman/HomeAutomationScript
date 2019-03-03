using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Logging;
using Services.BackupCreator.Config;
using Services.BackupCreator.Secrets;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services.BackupCreator.Jobs
{
    public class DropboxSender
    {
        private readonly ILogger _logger;
        private readonly SecretsConfig _secrets;
        private readonly DropboxConfig _dropboxConfig;

        public DropboxSender(ILogger<BackupCreatorService> logger, SecretsConfig secrets,
            DropboxConfig dropboxConfig)
        {
            _logger = logger;
            _secrets = secrets;
            _dropboxConfig = dropboxConfig;
        }

        public async Task BackupToDropbox(string file)
        {
            DropboxClient client = GetDropboxClient();
            await BackupFile(file, client);
            await RemoveOldBackups(client);
        }

        private async Task RemoveOldBackups(DropboxClient client)
        {
            var filesList = await client.Files.ListFolderAsync(string.Empty);
            var filesCountToGet = filesList.Entries.Count - _dropboxConfig.MaxFiles;
            if (filesCountToGet > 0)
            {
                var filesToRemove = filesList.Entries.OrderBy(f => f.Name)
                    .Take(filesCountToGet);

                foreach (var fileToRemove in filesToRemove)
                {
                    var res = await client.Files.DeleteAsync("/" + fileToRemove.Name);
                    _logger.LogInformation("Removed old backup file: {removedBackupFileName}", res.Name);
                }
            }
        }

        private async Task BackupFile(string file, DropboxClient client)
        {
            var environment = Environment.GetEnvironmentVariable("HOST") ?? Environment.MachineName;

            var backupFileName = $"{environment}_{Path.GetFileName(file)}";

            using (var stream = new FileStream(file, FileMode.Open))
            {
                _logger.LogInformation("Uploading file {file} to Dropbox", file);

                var response = await client.Files
                    .UploadAsync("/" + backupFileName, WriteMode.Overwrite.Instance, body: stream);

                _logger.LogInformation("Uploaded file {file} to Dropbox", file);
            }
        }

        private DropboxClient GetDropboxClient()
        {
            var config = new DropboxClientConfig("Home_Assistant_Back")
            {
                HttpClient = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(_dropboxConfig.TimeoutSeconds)
                }
            };
            return new DropboxClient(_secrets.DropboxToken, config);
        }
    }
}
