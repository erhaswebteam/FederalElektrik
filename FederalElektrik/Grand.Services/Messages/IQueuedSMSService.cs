using System;
using System.Collections.Generic;
using Grand.Core;
using Grand.Core.Domain.Messages;

namespace Grand.Services.Messages
{
    public partial interface IQueuedSMSService
    {
        void InsertQueuedSMS(QueuedSMS queuedSMS);

        List<QueuedSMS> GetUnSendSMS();

       void UpdateSmsForSend(QueuedSMS queuedSMS);
    }
}
