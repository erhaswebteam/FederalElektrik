using System;

namespace Grand.Core.Domain.Messages
{
    /// <summary>
    /// Represents an email item
    /// </summary>
    public partial class QueuedSMS : BaseEntity
    {
        public string GSM { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool IsSend { get; set; }
        public DateTime? SendOnUtc { get; set; }

    }
}
