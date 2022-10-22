using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IValidationService _validationService;

        public FileUploadService(IOptions<FirebaseSettings> firebaseSettings,
            IProjectEntityRepository projectEntityRepository,
            IBusinessRepository businessRepository,
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            IValidationService validationService)
        {
            _firebaseSettings = firebaseSettings.Value;
            _projectEntityRepository = projectEntityRepository;
            _businessRepository = businessRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _validationService = validationService;
        }

        private ProjectEntity ParseToProjectentity(FirebaseRequest request, Guid newId, string url)
        {
            ProjectEntity entity = new();
            DateTime localDate = DateTime.Now;
            entity.Id = newId;
            entity.ProjectId = Guid.Parse(request.entityId);
            entity.CreateBy = Guid.Parse(request.createBy ?? "00000000-0000-0000-0000-000000000000");
            if(entity.CreateBy != Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                entity.CreateDate = localDate;
            } else
            {
                entity.CreateDate = null;
            }
            entity.UpdateBy = Guid.Parse(request.updateBy ?? "00000000-0000-0000-0000-000000000000");
            if (entity.UpdateBy != Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                entity.UpdateDate = localDate;
            }
            else
            {
                entity.UpdateDate = null;
            }
            entity.Link = url ?? "";
            entity.Type = request.type ?? "";
            entity.Description = request.description ?? "";
            entity.Title = request.title ?? "";
            entity.Content = request.content ?? "";
            if(request.type != ProjectEntityEnum.PITCH.ToString())
            {
                entity.Priority = 0;
            }

            
            

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
                        await _businessRepository.UpdateBusinessImage(url, Guid.Parse(request.entityId));

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
                        await _projectRepository.UpdateProjectImage(url, Guid.Parse(request.entityId));

                    urls.Add(url);

                }
            }
            else if (request.entityName.ToLower().Equals(StoragePathEnum.ProjectEntity.ToString().ToLower()) && request.type.Equals("PRESS", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var file in request.files)
                {
                    string newGuid = Guid.NewGuid().ToString();

                    string[] type = file.ContentType.Split("/");

                    if (type[0].ToLower().Equals(CategoryEnum.Image.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + StoragePathEnum.ProjectEntity.ToString() + "/" + CategoryEnum.Image + "/" + newGuid;
                    }
                    else if (type[0].ToLower().Equals(CategoryEnum.Video.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + StoragePathEnum.ProjectEntity.ToString() + "/" + CategoryEnum.Video + "/" + newGuid;
                    }
                    else if (type[0].ToLower().Equals(CategoryEnum.Application.ToString().ToLower()))
                    {
                        path = StoragePathEnum.Project.ToString() + "/" + request.entityId + "/" + StoragePathEnum.ProjectEntity.ToString() + "/" + CategoryEnum.Application + "/" + newGuid;
                    }

                    string url = await uploadTask.Child(path).PutAsync(file.OpenReadStream());

                    ProjectEntity projectEntity = ParseToProjectentity(request, Guid.Parse(newGuid), url);

                    string insertPE = await _projectEntityRepository.CreateProjectEntity(projectEntity);
                    if (insertPE == null)
                    {
                        throw new CreateProjectEntityException("Can Not Create Project Entity!!");
                    }

                    urls.Add(url);

                }
            } else if (request.entityName.ToLower().Equals(StoragePathEnum.ProjectEntity.ToString().ToLower()) && !request.type.Equals("PRESS", StringComparison.InvariantCultureIgnoreCase))
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

                    string insertPE = await _projectEntityRepository.CreateProjectEntity(projectEntity);
                    if(insertPE == null)
                    {
                        throw new CreateProjectEntityException("Can Not Create Project Entity!!");
                    }

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

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(request.createBy);

            return urls;
        }

        public async Task<string> UploadBusinessContract(FirebaseBusinessContract request)
        {
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(request.businessId, tokenDescriptor);

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

            string path = "Contract/"+request.businessId+"/"+request.projectOwnerId+"/"+request.businessName+request.projectOwnerEmail;

            var url = await uploadTask.Child(path).PutAsync(request.file.OpenReadStream());
            return url;
            
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


        public async Task<string> UploadGeneratedContractToFirebase(string userId, string projectId, Stream stream)
        {
            var tokenDescriptor = new Dictionary<string, object>()
            {
                {"permission", "allow" }
            };

            string storageToken = await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(userId, tokenDescriptor);

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
                    .Child("Contracts")
                    .Child(projectId)
                    .Child(userId)
                    .Child(projectId + ".pdf")
                    .PutAsync(stream);


            var downloadUrl = await task;

            await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(userId);

            return downloadUrl.ToString();
        }

    }
}
