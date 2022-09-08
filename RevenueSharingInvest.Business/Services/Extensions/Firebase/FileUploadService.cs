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

namespace RevenueSharingInvest.Business.Services.Extensions.Firebase
{
    public class FileUploadService : IFileUploadService
    {
        private readonly FirebaseSettings _firebaseSettings;
        private readonly IProjectEntityRepository _projectEntityRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;

        public FileUploadService(IOptions<FirebaseSettings> firebaseSettings,
            IProjectEntityRepository projectEntityRepository,
            IBusinessRepository businessRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository)
        {
            _firebaseSettings = firebaseSettings.Value;
            _projectEntityRepository = projectEntityRepository;
            _businessRepository = businessRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
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
                if (request.files.Count > 1)
                {
                    throw new FileException("One Business Only Have One Avatar!!");
                }

                foreach (var file in request.files)
                {
                    string newGuid = Guid.NewGuid().ToString();

                    string[] type = file.ContentType.Split("/");

                    if (type[0].ToLower().Equals(CategoryEnum.Image.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Business.ToString() + "/" + request.entityId + "/" + CategoryEnum.Image +"/"+ newGuid;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Video.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Business.ToString() + "/" + request.entityId + "/" + CategoryEnum.Video + "/" + newGuid;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Application.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Business.ToString() + "/" + request.entityId + "/" + CategoryEnum.Application + "/" + newGuid;
                    }

                    string url = await uploadTask.Child(path).PutAsync(file.OpenReadStream());

                    if(type[0].ToLower().Equals(CategoryEnum.Image.ToString().ToLower()))
                        await _businessRepository.UpdateBusinessImage(url, Guid.Parse(request.businessId));

                    urls.Add(url);

                }
            } else if (request.entityName.ToLower().Equals(StoragePathEnum.Project.ToString().ToLower()))
            {
                if (request.files.Count > 1)
                {
                    throw new FileException("One Project Only Have One Avatar!!");
                }

                foreach (var file in request.files)
                {
                    string newGuid = Guid.NewGuid().ToString();

                    string[] type = file.ContentType.Split("/");

                    if (type[0].ToLower().Equals(CategoryEnum.Image.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + CategoryEnum.Image + "/" + newGuid;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Video.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + CategoryEnum.Video + "/" + newGuid;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Application.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + CategoryEnum.Application + "/" + newGuid;
                    }

                    string url = await uploadTask.Child(path).PutAsync(file.OpenReadStream());
                    if (type[0].ToLower().Equals(CategoryEnum.Image.ToString().ToLower()))
                        await _projectRepository.UpdateProjectImage(url, Guid.Parse(request.createBy));

                    urls.Add(url);

                }
            } else if (request.entityName.ToLower().Equals(StoragePathEnum.ProjectEntity.ToString().ToLower()))
            {
                foreach(var file in request.files)
                {
                    string newGuid = Guid.NewGuid().ToString();

                    string[] type = file.ContentType.Split("/");

                    if (type[0].ToLower().Equals(CategoryEnum.Image.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + StoragePathEnum.ProjectEntity.ToString() + "/" + CategoryEnum.Image + "/" + newGuid;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Video.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + StoragePathEnum.ProjectEntity.ToString() + "/" + CategoryEnum.Video + "/" + newGuid;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Application.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + StoragePathEnum.ProjectEntity.ToString() + "/" + CategoryEnum.Application + "/" + newGuid;
                    }

                    string url = await uploadTask.Child(path).PutAsync(file.OpenReadStream());

                    ProjectEntity projectEntity = ParseToProjectentity(request, Guid.Parse(newGuid), url);

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

                    if (type[0].ToLower().Equals(CategoryEnum.Image.ToString().ToLower()))
                    {
                        path = StoragePathEnum.User.ToString() + "/" + request.entityId + "/" + CategoryEnum.Image + "/" + newGuid;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Video.ToString().ToLower()))
                    {
                        path = StoragePathEnum.User.ToString() + "/" + request.entityId + "/" + CategoryEnum.Video + "/" + newGuid;
                    } else if (type[0].ToLower().Equals(CategoryEnum.Application.ToString().ToLower()))
                    {
                        path = StoragePathEnum.User.ToString() + "/" + request.entityId + "/" + CategoryEnum.Application + "/" + newGuid;
                    }

                    string url = await uploadTask.Child(path).PutAsync(file.OpenReadStream());
                    if (type[0].ToLower().Equals(CategoryEnum.Image.ToString().ToLower()))
                        await _userRepository.UpdateUserImage(url, Guid.Parse(request.createBy));

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
