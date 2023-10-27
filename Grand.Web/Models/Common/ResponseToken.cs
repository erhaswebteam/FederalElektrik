using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grand.Web.Models.Common
{
    public class ResponseToken
    {
        public DateTime challenge_ts { get; set; }
        public float score { get; set; }
        public List<string> ErrorCodes { get; set; }
        public bool success { get; set; }
        public string hostname { get; set; }

    }
}
