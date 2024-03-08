using Grand.Core.Data;
using Grand.Core.Domain.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grand.Services.Points
{
    public partial class ProcessService : IProcessService
    {
        private readonly IRepository<Process> _processRepository;

        public ProcessService(IRepository<Process> processRepository)
        {
            this._processRepository = processRepository;
        }

        public virtual Process Insert(Process procs)
        {
            return _processRepository.Insert(procs);
        }

        public virtual Process Update(Process procs)
        {
            return _processRepository.Update(procs);
        }
        public virtual decimal GetCustomerTotalSpend(string username)
        {
            return _processRepository.Table.Where(x => x.Username == username && x.TypeId == (int)ProcessType.Spend && x.TypeId == (int)ProcessType.TransferSpend).Sum(x => x.Point);
        }

        public virtual decimal GetCustomerActualPoint(string username)
        {
            return _processRepository.Table.Where(x => x.Username == username).Sum(x => x.Point);
        }

        public virtual List<Process> GetAllHistory(string username)
        {
            return _processRepository.Table.Where(x => x.Username == username).OrderByDescending(x => x.CreatedOnUtc).ToList();
        }

        public virtual decimal GetCustomerEarnPoint(string username)
        {
            return _processRepository.Table.Where(x => x.Username == username && x.TypeId == (int)ProcessType.Earn).Sum(x => x.Point);
        }

        public virtual decimal GetCustomerSpend(string username)
        {
            return _processRepository.Table.Where(x => x.Username == username && x.TypeId == (int)ProcessType.Spend).Sum(x => x.Point);
        }

        public virtual decimal GetCustomerTransferEarnPoint(string username)
        {
            return _processRepository.Table.Where(x => x.Username == username && x.TypeId == (int)ProcessType.TransferEarn).Sum(x => x.Point);
        }

        public virtual decimal GetCustomerTransferSpendPoint(string username)
        {
            return _processRepository.Table.Where(x => x.Username == username && x.TypeId == (int)ProcessType.TransferSpend).Sum(x => x.Point);
        }

        public virtual decimal GetCustomerCancelPoint(string username)
        {
            return _processRepository.Table.Where(x => x.Username == username && (x.TypeId == (int)ProcessType.Cancel || x.TypeId==(int)ProcessType.Partial_Cancellation)).Sum(x => x.Point);
        }

    }
}
