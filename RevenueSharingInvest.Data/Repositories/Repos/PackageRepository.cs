using Dapper;
using Microsoft.Extensions.Configuration;
using RevenueSharingInvest.Data.Helpers;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Data;
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
        public async Task<string> CreatePackage(Package packageDTO)
        {
            try
            {
                var query = "INSERT INTO Package ("
                    + "         Name, "
                    + "         ProjectId, "
                    + "         Price, "
                    + "         Image, "
                    + "         Quantity, "
                    + "         RemainingQuantity, "
                    + "         Description, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted ) "
                    + "     OUTPUT "
                    + "         INSERTED.Id "
                    + "     VALUES ( "
                    + "         @Name, "
                    + "         @ProjectId, "
                    + "         @Price, "
                    + "         @Image, "
                    + "         @Quantity, "
                    + "         @RemainingQuantity, "
                    + "         @Description, "
                    + "         @Status, "
                    + "         @CreateDate, "
                    + "         @CreateBy, "
                    + "         @UpdateDate, "
                    + "         @UpdateBy, "
                    + "         0 )";

                var parameters = new DynamicParameters();
                parameters.Add("Name", packageDTO.Name, DbType.String);
                parameters.Add("ProjectId", packageDTO.ProjectId, DbType.Guid);
                parameters.Add("Price", packageDTO.Price, DbType.Double);
                parameters.Add("Image", packageDTO.Image, DbType.String);
                parameters.Add("Quantity", packageDTO.Quantity, DbType.Int16);
                parameters.Add("RemainingQuantity", packageDTO.RemainingQuantity, DbType.Int16);
                parameters.Add("Description", packageDTO.Description, DbType.String);
                parameters.Add("Status", packageDTO.Status, DbType.String);
                parameters.Add("CreateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("CreateBy", packageDTO.CreateBy, DbType.Guid);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", packageDTO.UpdateBy, DbType.Guid);

                using var connection = CreateConnection();
                return ((Guid)connection.ExecuteScalar(query, parameters)).ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //DELETE
        public async Task<int> DeletePackageById(Guid packageId)//thiếu para UpdateBy
        {
            try
            {
                var query = "DELETE FROM Package WHERE Id = @Id";

                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("Id", packageId, DbType.Guid);

                return await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //GET ALL
        public async Task<List<Package>> GetAllPackagesByProjectId(int pageIndex, int pageSize, Guid projectId)
        {
            try
            {
                if (pageIndex != 0 && pageSize != 0)
                {
                    var query = "WITH X AS ( "
                    + "         SELECT "
                    + "             ROW_NUMBER() OVER ( "
                    + "                 ORDER BY "
                    + "                     ProjectId, "
                    + "                     Name ASC ) AS Num, "
                    + "             * "
                    + "         FROM Package "
                    + "         WHERE "
                    + "             ProjectId = @ProjectId "
                    + "             AND IsDeleted = 0 ) "
                    + "     SELECT "
                    + "         Id, "
                    + "         Name, "
                    + "         ProjectId, "
                    + "         Price, "
                    + "         Image, "
                    + "         Quantity, "
                    + "         RemainingQuantity, "
                    + "         Description, "
                    + "         Status, "
                    + "         CreateDate, "
                    + "         CreateBy, "
                    + "         UpdateDate, "
                    + "         UpdateBy, "
                    + "         IsDeleted "
                    + "     FROM "
                    + "         X "
                    + "     WHERE "
                    + "         Num BETWEEN @PageIndex * @PageSize - (@PageSize - 1) "
                    + "         AND @PageIndex * @PageSize";
                    var parameters = new DynamicParameters();
                    parameters.Add("ProjectId", projectId, DbType.Guid);
                    parameters.Add("PageIndex", pageIndex, DbType.Int16);
                    parameters.Add("PageSize", pageSize, DbType.Int16);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Package>(query, parameters)).ToList();
                }
                else
                {
                    var query = "SELECT * FROM Package WHERE ProjectId = @ProjectId AND IsDeleted = 0 ORDER BY ProjectId, Name ASC";
                    var parameters = new DynamicParameters();
                    parameters.Add("ProjectId", projectId, DbType.Guid);
                    using var connection = CreateConnection();
                    return (await connection.QueryAsync<Package>(query, parameters)).ToList();
                }               
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //GET BY ID
        public async Task<Package> GetPackageById(Guid packageId)
        {
            try
            {
                string query = "SELECT * FROM Package WHERE Id = @Id";
                var parameters = new DynamicParameters();
                parameters.Add("Id", packageId, DbType.Guid);
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<Package>(query, parameters);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //UPDATE
        public async Task<int> UpdatePackage(Package packageDTO, Guid packageId)
        {
            try
            {
                var query = "UPDATE Package "
                    + "     SET "
                    + "         Name = @Name, "
                    + "         ProjectId = @ProjectId, "
                    + "         Price = @Price, "
                    + "         Image = @Image, "
                    + "         Quantity = @Quantity, "
                    + "         RemainingQuantity = @RemainingQuantity, "
                    + "         Description = @Description, "
                    + "         UpdateDate = @UpdateDate, "
                    + "         UpdateBy = @UpdateBy, "
                    + "     WHERE "
                    + "         Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Name", packageDTO.Name, DbType.String);
                parameters.Add("ProjectId", packageDTO.ProjectId, DbType.Guid);
                parameters.Add("Price", packageDTO.Price, DbType.Double);
                parameters.Add("Image", packageDTO.Image, DbType.String);
                parameters.Add("Quantity", packageDTO.Quantity, DbType.Int16);
                parameters.Add("RemainingQuantity", packageDTO.RemainingQuantity, DbType.Int16);
                parameters.Add("Description", packageDTO.Description, DbType.String);
                parameters.Add("UpdateDate", DateTime.Now, DbType.DateTime);
                parameters.Add("UpdateBy", packageDTO.UpdateBy, DbType.Guid);
                parameters.Add("Id", packageId, DbType.Guid);

                using (var connection = CreateConnection())
                {
                    return await connection.ExecuteAsync(query, parameters);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        //CLEAR DATA
        public async Task<int> ClearAllPackageData()
        {
            try
            {
                var query = "DELETE FROM Package";
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(query);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> CountPackageByProjectId(Guid projectId)
        {
            try
            {
                var query = "SELECT COUNT(*) FROM Package WHERE ProjectId = @ProjectId AND IsDeleted = 0 ";

                var parameters = new DynamicParameters();
                parameters.Add("ProjectId", projectId, DbType.Guid);
                using var connection = CreateConnection();
                return ((int)connection.ExecuteScalar(query, parameters));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
