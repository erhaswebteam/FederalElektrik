using Grand.Core.Domain.Points;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Services.Points
{
    public partial interface IProcessService
    {
        Process Insert(Process procs);
        Process Update(Process procs);

        decimal GetCustomerActualPoint(string username);

        decimal GetCustomerTotalSpend(string username);

        List<Process> GetAllHistory(string username);

        decimal GetCustomerEarnPoint(string username);

        decimal GetCustomerSpend(string username);

        decimal GetCustomerTransferEarnPoint(string username);

        decimal GetCustomerTransferSpendPoint(string username);

        decimal GetCustomerCancelPoint(string username);
    }
}
