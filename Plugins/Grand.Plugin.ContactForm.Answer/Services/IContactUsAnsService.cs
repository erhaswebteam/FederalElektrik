using Grand.Core;
using Grand.Core.Domain.Messages;
using Grand.Plugin.ContactFormAnswer.Models;
using Grand.Services.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.ContactFormAnswer.Services
{
    public partial interface IContactUsAnsService : IContactUsService
    {
        IPagedList<ContactUsAns> GetAllContactUsAnswer(string contactUsId);
        void InsertContactUsAnswer(ContactUsAns contactusans);
    }
}
