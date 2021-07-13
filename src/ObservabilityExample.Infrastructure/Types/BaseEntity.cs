using System;

namespace ObservabilityExample.Infrastructure.Types
{
    public abstract class BaseEntity
    {
        public Guid Id { get; }
        public DateTime CreatedDate { get; }
        public DateTime UpdatedDate { get; }

        public BaseEntity(Guid id)
        {
            Id = id;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}