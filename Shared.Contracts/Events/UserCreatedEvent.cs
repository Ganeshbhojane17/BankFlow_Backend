using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Events
{
    public class UserCreatedEvent
    {
        public int UserId { get; set; }

        public int CustomerId { get; set; }

        public string Email { get; set; } = string.Empty;
    }
}
