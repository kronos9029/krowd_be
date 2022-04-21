using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Repositories.Repos
{
    public class PackageRepository : BaseRepository, IPackageRepository
    {
        public PackageRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //CREATE
        //public Task<int> CreatePackage(Package packageDTO)
        //{
        //    try
        //    {
        //        var query = "INSERT INTO Package ("
        //            + "         Name, "
        //            + "         ProjectId, "
        //            + "         Description, "
        //            + "         Percents, "
        //            + "         OpenMonth, "
        //            + "         CloseMonth, "
        //            + "         Status, "
        //            + "         CreateDate, "
        //            + "         CreateBy, " //Id Business Manager
        //            + "         UpdateDate, "
        //            + "         UpdateBy, "
        //            + "         IsDeleted ) "
        //            + "     VALUES ( "
        //            + "         @Name, "
        //            + "         @ProjectId, "
        //            + "         @Description, "
        //            + "         @Percents, "
        //            + "         @OpenMonth, "
        //            + "         @CloseMonth, "
        //            + "         @Status, "
        //            + "         @CreateDate, "
        //            + "         @CreateBy, "
        //            + "         @UpdateDate, "
        //            + "         @UpdateBy, "
        //            + "         0 )";

        //        var parameters = new DynamicParameters();
        //        parameters.Add("Name", stageDTO.Name, DbType.String);
        //        parameters.Add("ProjectId", stageDTO.ProjectId, DbType.String);
        //        parameters.Add("Description", stageDTO.Description, DbType.String);
        //        parameters.Add("Percents", stageDTO.Percents, DbType.Int16);
        //        parameters.Add("OpenMonth", stageDTO.OpenMonth, DbType.String);
        //        parameters.Add("CloseMonth", stageDTO.CloseMonth, DbType.Guid);
        //        parameters.Add("Status", stageDTO.Status, DbType.Int16);
        //        parameters.Add("CreateDate", stageDTO.CreateDate, DbType.DateTime);
        //        parameters.Add("CreateBy", stageDTO.CreateBy, DbType.Guid);
        //        parameters.Add("UpdateDate", stageDTO.UpdateDate, DbType.DateTime);
        //        parameters.Add("UpdateBy", stageDTO.UpdateBy, DbType.Guid);

        //        using var connection = CreateConnection();
        //        return await connection.ExecuteAsync(query, parameters);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message, e);
        //    }
        //}
    }
}