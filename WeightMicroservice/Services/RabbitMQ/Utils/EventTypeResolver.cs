﻿using WeightMicroservice.Services.RabbitMQ.Models;

namespace WeightMicroservice.Services.RabbitMQ.Utils
{
    public static class EventTypeResolver
    {
        private static readonly Dictionary<string, Type> EventTypeMappings = new()
        {
            { "User.Deleted", typeof(UserDeletedEvent) },
            { "Files.Deleted", typeof(FilesDeletedEvent) }
        };

        public static Type? Resolve(string routingKey)
        {
            return EventTypeMappings.TryGetValue(routingKey, out var eventType) ? eventType : null;
        }
    }
}
