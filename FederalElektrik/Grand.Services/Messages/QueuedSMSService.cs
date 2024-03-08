using System;
using System.Collections.Generic;
using System.Linq;
using Grand.Core;
using Grand.Core.Data;
using Grand.Core.Domain.Common;
using Grand.Core.Domain.Messages;
using Grand.Services.Events;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Grand.Services.Messages
{
    public partial class QueuedSMSService : IQueuedSMSService
    {
        private readonly IRepository<QueuedSMS> _queuedSMSRepository;


        public QueuedSMSService(IRepository<QueuedSMS> queuedSMSRepository)
        {
            _queuedSMSRepository = queuedSMSRepository;
        }


        public virtual void InsertQueuedSMS(QueuedSMS queuedSMS)
        {
            if (queuedSMS == null)
                throw new ArgumentNullException("queuedSMS");

            _queuedSMSRepository.Insert(queuedSMS);
        }

        public List<QueuedSMS> GetUnSendSMS()
        {
            return _queuedSMSRepository.Table.Where(x => x.IsSend == false).OrderBy(x => x.CreatedOnUtc).ToList();
        }

        public void UpdateSmsForSend(QueuedSMS queuedSMS)
        {
            _queuedSMSRepository.Update(queuedSMS);
        }
    }
}
