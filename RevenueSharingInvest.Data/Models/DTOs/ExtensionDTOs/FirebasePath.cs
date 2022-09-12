using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class FirebasePath
    {
        public string? category { get; set; }
        public string? path { get; set; }
        public List<IFormFile> files { get; set; }
    }
}
