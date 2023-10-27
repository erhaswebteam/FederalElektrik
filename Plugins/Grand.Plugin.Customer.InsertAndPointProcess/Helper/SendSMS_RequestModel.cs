using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KargoTest.Models
{
    public class SendSMS_RequestModel
    {
        public string token { get; set; }
        public string number { get; set; }
        public string message { get; set; }
    }
}
