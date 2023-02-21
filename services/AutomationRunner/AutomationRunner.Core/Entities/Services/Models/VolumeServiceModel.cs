using AutomationRunner.Core.Entities.Validators;
using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AutomationRunner.Core.Entities.Services.Models
{
    public class VolumeServiceModel : EntityIdService
    {
        [JsonProperty("volume_level")]
        public double Volume { get; }

        public VolumeServiceModel(string entityId, double volume) : base(entityId)
        {
            new VolumeValidator().ValidateAndThrow(volume);
            Volume = volume / 70.0;
        }
    }
}
