using AutoMapper;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class ProjectWalletService : IProjectWalletService
    {
        private readonly IProjectWalletRepository _projectWalletRepository;
        private readonly IMapper _mapper;


        public ProjectWalletService(IProjectWalletRepository projectWalletRepository, IMapper mapper)
        {
            _projectWalletRepository = projectWalletRepository;
            _mapper = mapper;
        }

        //CREATE
        public async Task<int> CreateProjectWallet(ProjectWalletDTO projectWalletDTO)
        {
            int result;
            try
            {
                ProjectWallet dto = _mapper.Map<ProjectWallet>(projectWalletDTO);
                result = await _projectWalletRepository.CreateProjectWallet(dto);
                if (result == 0)
                    throw new CreateObjectException("Can not create ProjectWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //DELETE
        public async Task<int> DeleteProjectWalletById(Guid projectWalletId)
        {
            int result;
            try
            {

                result = await _projectWalletRepository.DeleteProjectWalletById(projectWalletId);
                if (result == 0)
                    throw new CreateObjectException("Can not delete ProjectWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //GET ALL
        public async Task<List<ProjectWalletDTO>> GetAllProjectWallets()
        {
            List<ProjectWallet> projectWalletList = await _projectWalletRepository.GetAllProjectWallets();
            List<ProjectWalletDTO> list = _mapper.Map<List<ProjectWalletDTO>>(projectWalletList);
            return list;
        }

        //GET BY ID
        public async Task<ProjectWalletDTO> GetProjectWalletById(Guid projectWalletId)
        {
            ProjectWalletDTO result;
            try
            {

                ProjectWallet dto = await _projectWalletRepository.GetProjectWalletById(projectWalletId);
                result = _mapper.Map<ProjectWalletDTO>(dto);
                if (result == null)
                    throw new CreateObjectException("No ProjectWallet Object Found!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }

        //UPDATE
        public async Task<int> UpdateProjectWallet(ProjectWalletDTO projectWalletDTO, Guid projectWalletId)
        {
            int result;
            try
            {
                ProjectWallet dto = _mapper.Map<ProjectWallet>(projectWalletDTO);
                result = await _projectWalletRepository.UpdateProjectWallet(dto, projectWalletId);
                if (result == 0)
                    throw new CreateObjectException("Can not update ProjectWallet Object!");
                return result;
            }
            catch (Exception e)
            {
                throw new NotImplementedException();
            }
        }
    }
}
