using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Common.Firebase
{
    public interface IFileUploadService
    {
        public Task<string> UploadImageToFirebaseBusiness(IFormFile file, string uid);
        public Task<string> UploadImageToFirebaseField(IFormFile file, string uid);
        public Task<string> UploadImageToFirebaseProject(IFormFile file, string uid);
        public Task<string> UploadImageToFirebaseProjectEntity(IFormFile file, string uid);
        public Task<string> UploadImageToFirebaseUser(IFormFile file, string uid);
    }
}
