using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;

namespace AutomationRunner.Core.Secrets
{

    public class SecretsLoader
    {
        private readonly ILogger _logger;

        public SecretsLoader(ILogger<SecretsLoader> logger)
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
                if (secrets is null)
                    throw new ArgumentException("Invalid secrets");

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
