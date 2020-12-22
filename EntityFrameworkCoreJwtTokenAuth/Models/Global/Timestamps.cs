using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCoreJwtTokenAuth.Models.Global
{
    [Owned]
    public class Timestamps
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
