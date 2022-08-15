using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
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

        private ProjectEntity ParseToProjectentity(FirebaseRequest request, Guid newId, string url)
        {
            ProjectEntity entity = new();

            entity.Id = newId;
            entity.ProjectId = Guid.Parse(request.entityId);
            entity.CreateBy = Guid.Parse(request.createBy);
            entity.UpdateBy = Guid.Parse(request.updateBy);
            entity.Link = url;
            entity.Type = request.type;
            entity.Description = request.description;

            return entity;
        }

        public async Task<List<string>> UploadFilesWithPath(FirebaseRequest request)
        {
            var urls = new List<string>();
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(request.createBy, tokenDescriptor);

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

            string path = null;
            if (request.entityName.ToLower().Equals(StoragePathEnum.Business.ToString().ToLower()))
            {
                foreach(var file in request.files)
                {
                    string newGuid = Guid.NewGuid().ToString();

                    string[] type = file.ContentType.Split("/");

                    if (type[0].ToLower().Equals(CategoryEnum.Images.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Business.ToString() + "/" + request.entityId + "/" + CategoryEnum.Images;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Videos.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Business.ToString() + "/" + request.entityId + "/" + CategoryEnum.Videos;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Applications.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Business.ToString() + "/" + request.entityId + "/" + CategoryEnum.Applications;
                    }

                    string url = await uploadTask.Child(path).Child(newGuid).PutAsync(file.OpenReadStream());

                    urls.Add(url);

                }
            } else if (request.entityName.ToLower().Equals(StoragePathEnum.Project.ToString().ToLower()))
            {
                foreach(var file in request.files)
                {
                    string newGuid = Guid.NewGuid().ToString();

                    string[] type = file.ContentType.Split("/");

                    if (type[0].ToLower().Equals(CategoryEnum.Images.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + CategoryEnum.Images;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Videos.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + CategoryEnum.Videos;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Applications.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + CategoryEnum.Applications;
                    }

                    string url = await uploadTask.Child(path).Child(newGuid).PutAsync(file.OpenReadStream());

                    urls.Add(url);

                }
            } else if (request.entityName.ToLower().Equals(StoragePathEnum.ProjectEntity.ToString().ToLower()))
            {
                foreach(var file in request.files)
                {
                    Guid newGuid = Guid.NewGuid();

                    string[] type = file.ContentType.Split("/");

                    if (type[0].ToLower().Equals(CategoryEnum.Images.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + StoragePathEnum.ProjectEntity.ToString() + "/" + CategoryEnum.Images;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Videos.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + StoragePathEnum.ProjectEntity.ToString() + "/" + CategoryEnum.Videos;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Applications.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + StoragePathEnum.ProjectEntity.ToString() + "/" + CategoryEnum.Applications;
                    }

                    string url = await uploadTask.Child(path).Child(newGuid.ToString()).PutAsync(file.OpenReadStream());

                    ProjectEntity projectEntity = ParseToProjectentity(request, newGuid, url);

                    _ = _projectEntityRepository.CreateProjectEntityFromFirebase(projectEntity);

                    urls.Add(url);

                }
            } else if (request.entityName.ToLower().Equals(StoragePathEnum.User.ToString().ToLower()))
            {
                if(request.files.Count > 1) {
                    throw new FileException("One User Only Have One Avatar!!");
                }

                foreach(var file in request.files)
                {
                    string newGuid = Guid.NewGuid().ToString();

                    string[] type = file.ContentType.Split("/");

                    if (type[0].ToLower().Equals(CategoryEnum.Images.ToString().ToLower()))
                    {
                        path = StoragePathEnum.User.ToString() + request.entityId + "/" + CategoryEnum.Images;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Videos.ToString().ToLower()))
                    {
                        path = StoragePathEnum.User.ToString() + request.entityId + "/" + CategoryEnum.Videos;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Applications.ToString().ToLower()))
                    {
                        path = StoragePathEnum.User.ToString() + request.entityId + "/" + CategoryEnum.Applications;
                    }

                    string url = await uploadTask.Child(path).Child(newGuid).PutAsync(file.OpenReadStream());

                    urls.Add(url);

                }
            }
/*
            foreach (var file in request.files)
            {
                
                string newGuid = Guid.NewGuid().ToString();

                string[] type = file.ContentType.Split("/");

                if (type[0].ToLower().Equals(CategoryEnum.Images.ToString().ToLower()))
                {
                    string url = await uploadTask.Child(StoragePathEnum.ProjectEntity.ToString())
                                                 .Child(request.entityId)
                                                 .Child(CategoryEnum.Images.ToString())
                                                 .Child(newGuid)
                                                 .PutAsync(file.OpenReadStream());
                    urls.Add(newGuid, url);
                } else if (type[0].ToLower().Equals(CategoryEnum.Videos.ToString().ToLower()))
                {
                    string url = await uploadTask.Child(StoragePathEnum.ProjectEntity.ToString())
                                                 .Child(request.entityId)
                                                 .Child(CategoryEnum.Videos.ToString())
                                                 .Child(newGuid)
                                                 .PutAsync(file.OpenReadStream());
                    urls.Add(newGuid, url);
                } else if (type[0].ToLower().Equals(CategoryEnum.Applications.ToString().ToLower()))
                {
                    string url = await uploadTask.Child(StoragePathEnum.ProjectEntity.ToString())
                                                 .Child(request.entityId)
                                                 .Child(CategoryEnum.Applications.ToString())
                                                 .Child(newGuid)
                                                 .PutAsync(file.OpenReadStream());
                    urls.Add(newGuid, url);
                }

                
            }*/



            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(request.createBy);

            return urls;
        }

/*        public async Task DeleteImagesFromFirebase(ProjectEntityDTO firebaseEntity)
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


        }*/

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
