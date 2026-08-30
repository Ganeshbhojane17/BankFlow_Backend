using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Events
{
    public class UserProvisioned
    {
        public int CustomerId { get; set; }

        public int UserId { get; set; }
    }
}
