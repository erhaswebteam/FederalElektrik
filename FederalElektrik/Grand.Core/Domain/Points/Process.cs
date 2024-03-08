using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Core.Domain.Points
{
    public partial class Process : BaseEntity
    {
        public string Username { get; set; }
        public int TypeId { get; set; }
        public Guid OrderGuid { get; set; }
        public int OrderNumber { get; set; } = 0;
        public decimal Point { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    }
}
