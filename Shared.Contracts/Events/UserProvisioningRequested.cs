using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Events
{
    public class UserProvisioningRequested
    {
        public int CustomerId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
