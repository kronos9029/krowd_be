using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        public BillService(IBillRepository billRepository)
        {
            _billRepository = billRepository;
        }
        public Task<int> BulkInsertBills(InsertBillDTO bills, string projectId)
        {
            try
            {
                Guid currentProjectId = Guid.Parse(projectId);

                for(int i = 0; i < bills.Bills.Count; i++)
                {
                    bills.Bills[i].ProjectId = currentProjectId;
                }

                var result = _billRepository.BulkInsertInvoice(bills);
                return result;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }
    }
}
