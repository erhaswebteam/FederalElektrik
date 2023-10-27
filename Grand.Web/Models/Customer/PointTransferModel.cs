using System;

namespace Grand.Web.Models.Customer
{
    public class PointTransferModel
    {
        public PointTransferModel()
        {
            Result = new PointTransferResult();
        }

        public string Email { get; set; }
        public decimal Point { get; set; }
        public PointTransferResult Result { get; set; }
        public Guid CheckPage { get; set; }
    }

    public class PointTransferResult
    {
        public string Message { get; set; }
        public bool Success { get; set; }

    }
}
