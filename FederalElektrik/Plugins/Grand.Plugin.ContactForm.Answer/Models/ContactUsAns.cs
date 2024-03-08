using Grand.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.ContactFormAnswer.Models
{
    public partial class ContactUsAns : BaseEntity
    {
        public string CustomerId { get; set; }

        public string ContactFormUsId { get; set; }

        public DateTime? CreatedOnUtc { get; set; }

        public string FullName { get; set; }

        public string Answer { get; set; }
    }
}
