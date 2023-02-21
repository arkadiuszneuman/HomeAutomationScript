using Microsoft.Extensions.Logging;
using Services.BackupCreator.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Services.BackupCreator.Jobs
{
    public class ZipBackupCreator
    {
        private readonly ILogger _logger;
        private readonly PathsConfig _pathsConfig;

        public ZipBackupCreator(ILogger<BackupCreatorService> logger, PathsConfig pathsConfig)
        {
            _logger = logger;
            _pathsConfig = pathsConfig;
        }

        public string CreateBackup()
        {
            _logger.LogInformation("Starting to create zip");

            var backupDir = GetBackupDir();
            CopyDirsToBackup(backupDir);
            string zipFilePath = CreateZipFilePath(backupDir);
            CreateZipFile(backupDir, zipFilePath);

            return zipFilePath;
        }

        private void CreateZipFile(DirectoryInfo backupDir, string zipFilePath)
        {
            ZipFile.CreateFromDirectory(Path.Combine(backupDir.FullName, "create"), zipFilePath,
                            CompressionLevel.Optimal, false);

            var fileSize = new System.IO.FileInfo(zipFilePath).Length;
            var fileSizeKB = fileSize / 1024 * 1.0;
            var fileSizeMB = Math.Truncate(fileSizeKB / 1024 * 1000) / 1000;

            _logger.LogInformation("Created zip: {zipFilePath}. File size: {fileSizeMB}", zipFilePath, fileSizeMB);
        }

        private string CreateZipFilePath(DirectoryInfo backupDir)
        {
            var now = DateTime.Now;
            var nowString = now.ToString("yyyy-MM-dd-HHmmss");
            var backupFileName = $"backup_" + nowString + ".zip";

            var zipFilePath = Path.Combine(backupDir.FullName, backupFileName);
            if (File.Exists(zipFilePath))
                File.Delete(zipFilePath);
            return zipFilePath;
        }

        private void CopyDirsToBackup(DirectoryInfo backupDir)
        {
            foreach (var dir in _pathsConfig.DirsToBackup)
            {
                var sourceDir = Path.GetFullPath(dir);

                if (!Directory.Exists(sourceDir))
                {
                    _logger.LogWarning("Dir {sourceDir} does not exist", sourceDir);
                    continue;
                }

                var dirName = Path.GetFileName(sourceDir);

                var destinationDir = Path.Combine(backupDir.FullName, "create", dirName);
                Directory.CreateDirectory(destinationDir);

                PerformDeepCopy(sourceDir, destinationDir);

                _logger.LogInformation("Copied {sourceDir} to {destinationDir}", sourceDir, destinationDir);
            }
        }

        private DirectoryInfo GetBackupDir()
        {
            var path = Path.GetFullPath(_pathsConfig.TempBackupPath);
            var backupDir = Directory.CreateDirectory(path);
            backupDir.Delete(true);

            return backupDir;
        }

        private void PerformDeepCopy(string sourceDirectory, string destinationDirectory)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            var sourceDir = new DirectoryInfo(sourceDirectory);
            var targetDir = new DirectoryInfo(destinationDirectory);
            Array.ForEach(sourceDir.GetFiles(), f => f.CopyTo(Path.Combine(targetDir.ToString(), f.Name), true));

            foreach (DirectoryInfo dir in sourceDir.GetDirectories())
            {
                var subDirectory = targetDir.CreateSubdirectory(dir.Name);
                PerformDeepCopy(dir.FullName, subDirectory.FullName);
            }
        }
    }
}
