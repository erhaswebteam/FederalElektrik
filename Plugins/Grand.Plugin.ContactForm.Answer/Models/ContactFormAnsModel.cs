using Grand.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.ContactFormAnswer.Models
{
    public partial class ContactFormAnsModel : BaseGrandModel
    {
        public DateTime? CreatedOnUtc { get; set; }
        public string FullName { get; set; }
        public string Answer { get; set; }
    }
}
