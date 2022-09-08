using Microsoft.AspNetCore.Http;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Data.Models.DTOs;
using System.Collections.Generic;
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
        public Task<List<string>> UploadFilesWithPath(FirebaseRequest request);
    }
}
