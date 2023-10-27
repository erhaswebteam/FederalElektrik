using Microsoft.AspNetCore.Mvc;
using FluentValidation.Attributes;
using Grand.Framework;
using Grand.Framework.Mvc.Models;
using Grand.Web.Validators.Common;
using Grand.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using Grand.Core.Domain.Catalog;
using Grand.Web.Models.Common;
using Grand.Core.Domain.Messages;
using System;

namespace Grand.Plugin.ContactFormAnswer.Models
{
    public partial class ContactUsAnsModel : ContactUsModel
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }

        public ContactUsAnsModel()
        {
            ContactUsList = new List<ContactUs>();
            ContactUsAnsList = new List<ContactUsAns>();
            IsAnswered = new List<bool>();
        }

        public List<ContactUs> ContactUsList { get; set; }
        public List<ContactUsAns> ContactUsAnsList { get; set; }
        public List<bool> IsAnswered { get; set; }
    }
}
