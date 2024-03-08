using Grand.Core.Domain.Messages;
using Grand.Core.Domain.Tasks;
using Grand.Services.Messages;
using Grand.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Grand.Plugin.Codec.SendSms.Controllers
{
    public partial class SMSSendController : ScheduleTask, IScheduleTask
    {
        private readonly IQueuedSMSService _queuedSMSService;

        public SMSSendController(IQueuedSMSService queuedSMSService)
        {
            this._queuedSMSService = queuedSMSService;
        }

        public void Execute()
        {
            List<QueuedSMS> smsList = _queuedSMSService.GetUnSendSMS();
            if (smsList.Count > 0)
            {
                string username = "PANASONICPORTAL";
                string pass = "9Pc-_i8M";
                string sender = "PEWTR";

                foreach (var sms in smsList)
                {
                    CodecSMS.SoapSoapClient client = new CodecSMS.SoapSoapClient(CodecSMS.SoapSoapClient.EndpointConfiguration.SoapSoap);
                    var result = client.SendSmsAsync(username, pass, sender, sms.GSM, sms.Message, sms.CreatedOnUtc.ToString(), true, "1000455", 2, "", "", "BIREYSEL", "BILGILENDIRME").GetAwaiter().GetResult();
                    var document = XDocument.Parse(result);
                    string code = document.Descendants().Where(x => x.Name.LocalName == "ErrorCode").Select(x => x.Value).FirstOrDefault();
                    if (code == "0")
                    {
                        sms.IsSend = true;
                        sms.SendOnUtc = DateTime.UtcNow;

                        _queuedSMSService.UpdateSmsForSend(sms);
                    }
                }
            }
        }
    }
}
