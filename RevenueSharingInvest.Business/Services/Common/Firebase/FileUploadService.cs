using Firebase.Auth;
using FirebaseAdmin.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Services.Common.Firebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenueSharingInvest.Business.Models.Constant;

namespace RevenueSharingInvest.Business.Services.Common.Firebase
{
    public class FileUploadService : IFileUploadService
    {
        private readonly FirebaseSettings _firebaseSettings;

        public FileUploadService(IOptions<FirebaseSettings> firebaseSettings)
        {
            _firebaseSettings = firebaseSettings.Value;
        }

        public async Task<string> UploadImageToFirebaseBusiness(IFormFile file, string uid)
        {
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid, tokenDescriptor);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));

            //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
            var token = await auth.SignInWithCustomTokenAsync(storageToken);

            var task = new FirebaseStorage(
                    _firebaseSettings.Bucket,
                     new FirebaseStorageOptions
                     {
                         AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                         ThrowOnCancel = true,
                     })
                    .Child(StoragePathEnum.Business.ToString())
                    .Child(file.FileName)
                    .PutAsync(file.OpenReadStream());

            var downloadUrl = await task;

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);

            return downloadUrl.ToString();

        }
        public async Task<string> UploadImageToFirebaseField(IFormFile file, string uid)
        {
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid, tokenDescriptor);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));

            //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
            var token = await auth.SignInWithCustomTokenAsync(storageToken);

            var task = new FirebaseStorage(
                    _firebaseSettings.Bucket,
                     new FirebaseStorageOptions
                     {
                         AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                         ThrowOnCancel = true,
                     })
                    .Child(StoragePathEnum.Field.ToString())
                    .Child(file.FileName)
                    .PutAsync(file.OpenReadStream());


            var downloadUrl = await task;

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);

            return downloadUrl.ToString();

        }
        public async Task<string> UploadImageToFirebaseProject(IFormFile file, string uid)
        {
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid, tokenDescriptor);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));

            //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
            var token = await auth.SignInWithCustomTokenAsync(storageToken);

            var task = new FirebaseStorage(
                    _firebaseSettings.Bucket,
                     new FirebaseStorageOptions
                     {
                         AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                         ThrowOnCancel = true,
                     })
                    .Child(StoragePathEnum.Project.ToString())
                    .Child(file.FileName)
                    .PutAsync(file.OpenReadStream());



            var downloadUrl = await task;

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);

            return downloadUrl.ToString();

        }
        public async Task<string> UploadImageToFirebaseProjectEntity(IFormFile file, string uid)
        {
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid, tokenDescriptor);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));

            //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
            var token = await auth.SignInWithCustomTokenAsync(storageToken);

            var task = new FirebaseStorage(
                    _firebaseSettings.Bucket,
                     new FirebaseStorageOptions
                     {
                         AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                         ThrowOnCancel = true,
                     })
                    .Child(StoragePathEnum.ProjectEntity.ToString())
                    .Child(file.FileName)
                    .PutAsync(file.OpenReadStream());



            var downloadUrl = await task;

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);

            return downloadUrl.ToString();

        }
        public async Task<string> UploadImageToFirebaseUser(IFormFile file, string uid)
        {
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid, tokenDescriptor);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));

            //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
            var token = await auth.SignInWithCustomTokenAsync(storageToken);

            var task = new FirebaseStorage(
                    _firebaseSettings.Bucket,
                     new FirebaseStorageOptions
                     {
                         AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                         ThrowOnCancel = true,
                     })
                    .Child(StoragePathEnum.User.ToString())
                    .Child(file.FileName)
                    .PutAsync(file.OpenReadStream());

            
            var downloadUrl = await task;

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);

            return downloadUrl.ToString();

        }
    }
}
