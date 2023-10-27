using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Core.Domain.Points
{
    public enum ProcessType
    {
        Earn = 10,
        Spend = 20,
        Partial_Cancellation = 30,
        Cancel = 40,
        TransferSpend = 50,
        TransferEarn = 60
    }
}
