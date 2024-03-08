using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grand.Web.Areas.Admin.Models.Customers
{
    public class PointHistoryModel
    {
        public decimal Point { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
