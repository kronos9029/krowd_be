using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Business.Services.Common.Firebase;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/upload-files")]
    [EnableCors]
    public class UploadFileController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;

        public UploadFileController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageToFirebase([FromForm] FirebaseEntity firebaseEntity)
        {
            var result = await _fileUploadService.UploadImageWithPath(firebaseEntity);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteImagesFromFirebase(FirebaseEntity firebaseEntity)
        {
            return Ok(_fileUploadService.DeleteImagesFromFirebase(firebaseEntity));
        }
    }
}
