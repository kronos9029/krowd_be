using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Common.Firebase
{
    public class FileUploadService : IFileUploadService
    {
        private readonly FirebaseSettings _firebaseSettings;
        private readonly IProjectEntityRepository _projectEntityRepository;

        public FileUploadService(IOptions<FirebaseSettings> firebaseSettings, IProjectEntityRepository projectEntityRepository)
        {
            _firebaseSettings = firebaseSettings.Value;
            _projectEntityRepository = projectEntityRepository;
        }

        public async Task<Dictionary<string, string>> UploadFilesWithPath(ProjectEntityDTO projectEntity)
        {
            var urls = new Dictionary<string, string>();
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(projectEntity.createBy, tokenDescriptor);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));

            //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
            var token = await auth.SignInWithCustomTokenAsync(storageToken);

            var uploadTask = new FirebaseStorage(
                                 _firebaseSettings.Bucket,
                                 new FirebaseStorageOptions
                                 {
                                     AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                                     ThrowOnCancel = true,
                                 });

            foreach (var image in projectEntity.files)
            {
                
                string newGuid = Guid.NewGuid().ToString();

                string[] type = image.ContentType.Split("/");

                if (type[0].ToLower().Equals(CategoryEnum.Images.ToString().ToLower()))
                {
                    string url = await uploadTask.Child(CategoryEnum.Images.ToString())
                                                 .Child(StoragePathEnum.ProjectEntity.ToString())
                                                 .Child(projectEntity.id)
                                                 .Child(newGuid)
                                                 .PutAsync(image.OpenReadStream());
                    urls.Add(newGuid, url);
                } else if (type[0].ToLower().Equals(CategoryEnum.Videos.ToString().ToLower()))
                {
                    string url = await uploadTask.Child(CategoryEnum.Videos.ToString())
                                                 .Child(StoragePathEnum.ProjectEntity.ToString())
                                                 .Child(projectEntity.id)
                                                 .Child(newGuid)
                                                 .PutAsync(image.OpenReadStream());
                    urls.Add(newGuid, url);
                } else if (type[0].ToLower().Equals(CategoryEnum.Applications.ToString().ToLower()))
                {
                    string url = await uploadTask.Child(CategoryEnum.Applications.ToString())
                                                 .Child(StoragePathEnum.ProjectEntity.ToString())
                                                 .Child(projectEntity.id)
                                                 .Child(newGuid)
                                                 .PutAsync(image.OpenReadStream());
                    urls.Add(newGuid, url);
                }

                
            }

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(projectEntity.createBy);

            return urls;
        }
        public async Task<Dictionary<string, string>> UploadFilesWithPath(BusinessDTO projectEntity)
        {
            var urls = new Dictionary<string, string>();
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(projectEntity.createBy, tokenDescriptor);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));

            //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
            var token = await auth.SignInWithCustomTokenAsync(storageToken);

            var uploadTask = new FirebaseStorage(
                                 _firebaseSettings.Bucket,
                                 new FirebaseStorageOptions
                                 {
                                     AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                                     ThrowOnCancel = true,
                                 });

            foreach (var image in projectEntity.files)
            {

                string newGuid = Guid.NewGuid().ToString();

                string[] type = image.ContentType.Split("/");

                if (type[0].ToLower().Equals(CategoryEnum.Images.ToString().ToLower()))
                {
                    string url = await uploadTask.Child(CategoryEnum.Images.ToString())
                                                 .Child(StoragePathEnum.ProjectEntity.ToString())
                                                 .Child(projectEntity.id)
                                                 .Child(newGuid)
                                                 .PutAsync(image.OpenReadStream());
                    urls.Add(newGuid, url);
                }
                else if (type[0].ToLower().Equals(CategoryEnum.Videos.ToString().ToLower()))
                {
                    string url = await uploadTask.Child(CategoryEnum.Videos.ToString())
                                                 .Child(StoragePathEnum.ProjectEntity.ToString())
                                                 .Child(projectEntity.id)
                                                 .Child(newGuid)
                                                 .PutAsync(image.OpenReadStream());
                    urls.Add(newGuid, url);
                }
                else if (type[0].ToLower().Equals(CategoryEnum.Applications.ToString().ToLower()))
                {
                    string url = await uploadTask.Child(CategoryEnum.Applications.ToString())
                                                 .Child(StoragePathEnum.ProjectEntity.ToString())
                                                 .Child(projectEntity.id)
                                                 .Child(newGuid)
                                                 .PutAsync(image.OpenReadStream());
                    urls.Add(newGuid, url);
                }


            }

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(projectEntity.createBy);

            return urls;
        }

        public async Task DeleteImagesFromFirebase(ProjectEntityDTO firebaseEntity)
        {
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(firebaseEntity.createBy, tokenDescriptor);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseSettings.ApiKey));

            //var token = await auth.SignInWithEmailAndPasswordAsync(_firebaseSettings.Email, _firebaseSettings.Password);
            var token = await auth.SignInWithCustomTokenAsync(storageToken);

            var uploadTask = new FirebaseStorage(
                                 _firebaseSettings.Bucket,
                                 new FirebaseStorageOptions
                                 {
                                     AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken),
                                     ThrowOnCancel = true,
                                 });

            string fileName = firebaseEntity.id + ".jpg";

            string newGuid = Guid.NewGuid().ToString();
            await uploadTask.Child(firebaseEntity.type)
                            .Child(firebaseEntity.path)
                            .Child(firebaseEntity.projectId)
                            .Child(fileName)
                            .DeleteAsync();


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
                    .Child("Images")
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
                .Child("Images")
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
                .Child("Images")
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
                .Child("Images")
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
                .Child("Images")
                    .Child(StoragePathEnum.User.ToString())
                    .Child(file.FileName)
                    .PutAsync(file.OpenReadStream());


            var downloadUrl = await task;

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);

            return downloadUrl.ToString();

        }
    }
}
