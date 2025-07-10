using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDefaults.Messaging.Events
{
    public record IntegrationEvent
    {
        public Guid EventId => Guid.NewGuid();
        public DateTime OccurredOn  => DateTime.UtcNow;

        public string EventType => GetType().AssemblyQualifiedName;
    }
}
