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
        private readonly PathsConfig _pathsConfig;

        public CreateBackupJob(ILogger<BackupCreatorService> logger, PathsConfig pathsConfig)
        {
            _logger = logger;
            _pathsConfig = pathsConfig;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogTrace("Job started");
            await Pack();
        }

        public async Task Pack()
        {
           

            //var accessToken = "xxx";

            //var httpClient = new HttpClient()
            //{
            //    // Specify request level timeout which decides maximum time that can be spent on
            //    // download/upload files.
            //    Timeout = TimeSpan.FromMinutes(20)
            //};

            //var config = new DropboxClientConfig("Home_Assistant_Back")
            //{
            //    HttpClient = httpClient
            //};
            //var client = new DropboxClient(accessToken, config);
            //using (var stream = new FileStream(zipFilePath, FileMode.Open))
            //{
            //    var response = client.Files
            //        .UploadAsync("/" + backupFileName, WriteMode.Overwrite.Instance, body: stream)
            //        .Result;

            //    Console.WriteLine("Uploaded Id {0}", response.Id);
            //}

            //backupDir.Delete(true);

            //var filesList = client.Files.ListFolderAsync(string.Empty).Result;
            //var maxFiles = 5;
            //var filesCountToGet = filesList.Entries.Count - maxFiles;
            //if (filesCountToGet > 0)
            //{
            //    var filesToRemove = filesList.Entries.OrderBy(f => f.Name)
            //        .Take(filesCountToGet);

            //    foreach (var fileToRemove in filesToRemove)
            //    {
            //        var res = await client.Files.DeleteAsync("/" + fileToRemove.Name);
            //        Console.WriteLine("Removed {0}", res.Name);
            //    }
            //}
        }
    }
}
