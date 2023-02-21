namespace AutomationRunner.Core.Entities.Services.Models;

public class RepeatModeServiceModel : EntityIdService
{
    public string Repeat { get; }

    public RepeatModeServiceModel(string entityId, MediaPlayer.RepeatMode repeatMode) : base(entityId)
    {
        Repeat = repeatMode.ToString().ToLowerInvariant();
    }
}