using Newtonsoft.Json;

namespace AutomationRunner.Core.Entities.Services.Models;

public class PlayMediaServiceModel : EntityIdService
{
    [JsonProperty("media_content_id")]
    public string ContentId { get; }
        
    [JsonProperty("media_content_type")]
    public string ContentType { get; }

    public PlayMediaServiceModel(string entityId, string contentId, string contentType) : base(entityId)
    {
        ContentId = contentId;
        ContentType = contentType;
    }
}