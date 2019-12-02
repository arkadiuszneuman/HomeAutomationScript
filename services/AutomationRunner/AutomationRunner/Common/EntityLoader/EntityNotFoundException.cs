﻿namespace AutomationRunner.Common.EntityLoader
{
    public class EntityNotFoundException : AutomationRunnerException
    {
        public string EntityId { get; private set; }

        public EntityNotFoundException(string entityId)
            : base($"Entity not found: {entityId}")
        {
            EntityId = entityId;
        }
    }
}
