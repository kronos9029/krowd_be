using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.Repos
{
    public class BillRepository : BaseRepository, IBillRepository
    {
        public BillRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> BulkInsertInvoice(InsertBillDTO request)
        {
            var query = "INSERT INTO Bill (InvoiceId, Amount, Description, CreateBy,  CreateDate, ProjectId) VALUES ";

            for(int i = 0; i < request.Bills.Count; i++)
            {
                if(i == request.Bills.Count - 1)
                    query += "(" +"'"+request.Bills[i].InvoiceId +"'"+ "," + "'" + request.Bills[i].Amount + "'" + "," +"'"+ (request.Bills[i].Description ??= "") +"'" + "," + "'" + request.Bills[i].CreateBy + "'" + "," + "'" + request.Bills[i].CreateDate + "'" + "," + "'" + request.Bills[i].ProjectId + "'" + ")";
                else
                    query += "(" +"'"+request.Bills[i].InvoiceId +"'"+ "," + "'" + request.Bills[i].Amount + "'" + "," +"'"+ (request.Bills[i].Description ??= "") +"'" + "," + "'" + request.Bills[i].CreateBy + "'" + "," + "'" + request.Bills[i].CreateDate + "'" + "," + "'" + request.Bills[i].ProjectId + "'" + "),";

            }

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query);
        }
    }
}
