using Microsoft.AspNetCore.Http;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Data.Models.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions.Firebase
{
    public interface IFileUploadService
    {
        public Task<string> UploadImageToFirebaseBusiness(IFormFile file, string uid);
        public Task<string> UploadImageToFirebaseField(IFormFile file, string uid);
        public Task<string> UploadImageToFirebaseProject(IFormFile file, string uid);
        public Task<string> UploadImageToFirebaseProjectEntity(IFormFile file, string uid);
        public Task<string> UploadImageToFirebaseUser(IFormFile file, string uid);
        public Task<string> UploadBusinessContract(FirebaseBusinessContract request);
        public Task<List<string>> UploadFilesWithPath(FirebaseRequest request);
        public Task<string> UploadGeneratedContractToFirebase(string userId, string projectId, Stream stream);
        public Task<string> UploadAdminTracsactionReceipt(string withdrawId, IFormFile file, string userId);
    }
}
