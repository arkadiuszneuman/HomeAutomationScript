﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System;

namespace Services.BackupCreator.Secrets
{
    public class SecretsConfig
    {
        public string DropboxToken { get; set; }
    }

    public class SecretsLoader
    {
        private readonly ILogger<BackupCreatorService> _logger;

        public SecretsLoader(ILogger<BackupCreatorService> logger)
        {
            _logger = logger;
        }

        public SecretsConfig LoadSecrets()
        {
            try
            {
                _logger.LogDebug("Loading secrets");

                var secretsPath = "secrets.json";
                if (!File.Exists(secretsPath))
                    throw new FileNotFoundException("Secrets file doesn't exist");

                var json = File.ReadAllText(secretsPath);
                var secrets = JsonConvert.DeserializeObject<SecretsConfig>(json);

                return secrets;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
