using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grand.Core.Domain.Messages;
using Grand.Services.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Controllers
{
    public class SmsController : Controller
    {
        private readonly IQueuedSMSService _queuedSMSService;

        public SmsController(IQueuedSMSService queuedSMSService)
        {
            this._queuedSMSService = queuedSMSService;
        }

        public IActionResult SendSms()
        {
            return Json("1");
        }

        [HttpPost]
        public IActionResult SendSms(string p, string m)
        {
            if (!string.IsNullOrEmpty(p) && !string.IsNullOrEmpty(m))
            {
                QueuedSMS sms = new QueuedSMS();
                sms.CreatedOnUtc = DateTime.UtcNow;
                sms.GSM = p.Trim().Replace(" ", "");
                sms.IsSend = false;
                sms.Message = m;
                sms.SendOnUtc = null;

                _queuedSMSService.InsertQueuedSMS(sms);
            }
            return Json("1");
        }
    }
}