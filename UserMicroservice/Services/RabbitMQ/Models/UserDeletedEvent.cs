﻿namespace UserMicroservice.Services.RabbitMQ.Models
{
    public record UserDeletedEvent
    {
        public string UserId { get; set; } = null!;
    }
}
