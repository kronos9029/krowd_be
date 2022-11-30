using RevenueSharingInvest.Data.Helpers.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions.Security
{
    public static class GenerateFileHash
    {
        public static String GetHash(HashingAlgoTypes hashingAlgoType, string filePath)
        {
            try
            {
                using var hasher = System.Security.Cryptography.HashAlgorithm.Create(hashingAlgoType.ToString());
                using var stream = System.IO.File.OpenRead(filePath);
                var hash = hasher.ComputeHash(stream);
                stream.Close();
                return BitConverter.ToString(hash).Replace("-", "");

            }
            catch(Exception ex)
            {
                LoggerService.Logger(ex.ToString());
                throw new Exception(ex.Message);
            }
        }
    }
    public enum HashingAlgoTypes
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }
}
