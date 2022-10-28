using AutoMapper;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
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
        private readonly IWalletTypeRepository _walletTypeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;


        public ProjectWalletService(
            IProjectWalletRepository projectWalletRepository, 
            IWalletTypeRepository walletTypeRepository, 
            IProjectRepository projectRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IValidationService validationService, 
            IMapper mapper)
        {
            _projectWalletRepository = projectWalletRepository;
            _walletTypeRepository = walletTypeRepository;
            _projectRepository = projectRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        //GET ALL
        public async Task<UserWalletsDTO> GetAllProjectWallets(ThisUserObj currentUser)
        {
            try
            {
                UserWalletsDTO result = new UserWalletsDTO();
                result.listOfProjectWallet = new AllProjectManagerWalletDTO();
                result.listOfProjectWallet.p3List = new List<GetProjectWalletDTO>();
                result.listOfProjectWallet.p4List = new List<GetProjectWalletDTO>();
                List<ProjectWallet> projectWalletList = await _projectWalletRepository.GetProjectWalletsByProjectManagerId(Guid.Parse(currentUser.userId));
                List<MappedProjectWalletDTO> list = _mapper.Map<List<MappedProjectWalletDTO>>(projectWalletList);
                GetProjectWalletDTO dto = new GetProjectWalletDTO();
                Project project = new Project();

                foreach (MappedProjectWalletDTO item in list)
                {
                    item.createDate = await _validationService.FormatDateOutput(item.createDate);
                    item.updateDate = await _validationService.FormatDateOutput(item.updateDate);

                    dto = _mapper.Map<GetProjectWalletDTO>(item);
                    dto.walletType = _mapper.Map<GetWalletTypeForWalletDTO>(await _walletTypeRepository.GetWalletTypeById(Guid.Parse(item.walletTypeId)));

                    project = dto.projectId == null ? null : await _projectRepository.GetProjectById(Guid.Parse(dto.projectId));
                    dto.projectName = project == null ? null : project.Name;
                    dto.projectImage = project == null ? null : project.Image;

                    result.totalAsset += (float)item.balance;

                    if (Guid.Parse(item.walletTypeId).Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P1"))))
                        result.listOfProjectWallet.p1 = dto;
                    else if (Guid.Parse(item.walletTypeId).Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P2"))))
                        result.listOfProjectWallet.p2 = dto;
                    else if (Guid.Parse(item.walletTypeId).Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P3"))))                  
                        result.listOfProjectWallet.p3List.Add(dto);                     
                    else if (Guid.Parse(item.walletTypeId).Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P4"))))
                        result.listOfProjectWallet.p4List.Add(dto);                     
                    else if (Guid.Parse(item.walletTypeId).Equals(Guid.Parse(WalletTypeDictionary.walletTypes.GetValueOrDefault("P5"))))
                        result.listOfProjectWallet.p5 = dto;
                }
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //GET BY ID
        public async Task<GetProjectWalletDTO> GetAllProjectWalletById(Guid id, ThisUserObj currentUser)
        {
            GetProjectWalletDTO result;
            try
            {
                Project project = new Project();
                ProjectWallet projectWallet = await _projectWalletRepository.GetProjectWalletById(id);
                if (projectWallet == null)
                    throw new NotFoundException("No ProjectWallet Object Found!");
                if (!currentUser.userId.Equals(projectWallet.ProjectManagerId.ToString()))
                    throw new InvalidFieldException("This id is not belong to your wallets!!!");

                result = _mapper.Map<GetProjectWalletDTO>(projectWallet);
                result.walletType = _mapper.Map<GetWalletTypeForWalletDTO>(await _walletTypeRepository.GetWalletTypeById((Guid)projectWallet.WalletTypeId));

                project = result.projectId == null ? null : await _projectRepository.GetProjectById(Guid.Parse(result.projectId));
                result.projectName = project == null ? null : project.Name;
                result.projectImage = project == null ? null : project.Image;

                result.createDate = await _validationService.FormatDateOutput(result.createDate);
                result.updateDate = await _validationService.FormatDateOutput(result.updateDate);

                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        //TRANSFER MONEY
        public async Task<TransferResponseDTO> TransferBetweenProjectWallet(TransferDTO transferDTO, ThisUserObj currentUser)
        {
            TransferResponseDTO result = new TransferResponseDTO();
            try
            {
                ProjectWallet fromWallet = await _projectWalletRepository.GetProjectWalletById(transferDTO.fromWalletId);
                WalletType fromWalletType = await _walletTypeRepository.GetWalletTypeById((Guid)fromWallet.WalletTypeId);
                ProjectWallet toWallet = await _projectWalletRepository.GetProjectWalletById(transferDTO.toWalletId);
                WalletType toWalletType = await _walletTypeRepository.GetWalletTypeById((Guid)toWallet.WalletTypeId);

                if (fromWallet.WalletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("P3"))
                    && !fromWallet.WalletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("P5")))
                    throw new InvalidFieldException("You can transfer money from P3 to P5 only!!!");
                else if (fromWallet.WalletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("P5"))
                    && !fromWallet.WalletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("P2")))
                    throw new InvalidFieldException("You can transfer money from P5 to P2 only!!!");
                else if (fromWallet.WalletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("P2"))
                    && !fromWallet.WalletTypeId.Equals(WalletTypeDictionary.walletTypes.GetValueOrDefault("P4")))
                    throw new InvalidFieldException("You can transfer money from P2 to P4 only!!!");

                if (fromWallet.Balance < transferDTO.amount) throw new InvalidFieldException("Your wallet balance is not enough to transfer!!!");

                //Subtract fromWallet balance
                ProjectWallet projectWallet = new ProjectWallet();
                projectWallet.ProjectManagerId = Guid.Parse(currentUser.userId);
                projectWallet.WalletTypeId = fromWallet.WalletTypeId;
                projectWallet.Balance = -transferDTO.amount;
                projectWallet.UpdateBy = Guid.Parse(currentUser.userId);
                await _projectWalletRepository.UpdateProjectWalletBalance(projectWallet);

                //Create CASH_OUT WalletTransaction from fromWallet to toWallet
                WalletTransaction walletTransaction = new WalletTransaction();
                walletTransaction.Amount = transferDTO.amount;
                walletTransaction.Fee = 0;
                walletTransaction.Description = "Transfer money from " + fromWalletType.Type + " wallet to " + toWalletType.Type + " wallet";
                walletTransaction.FromWalletId = transferDTO.fromWalletId;
                walletTransaction.ToWalletId = transferDTO.toWalletId;
                walletTransaction.Type = WalletTransactionTypeEnum.CASH_OUT.ToString();
                walletTransaction.CreateBy = Guid.Parse(currentUser.userId);
                await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                //Add toWallet balance
                projectWallet.WalletTypeId = toWallet.WalletTypeId;
                projectWallet.Balance = transferDTO.amount;
                await _projectWalletRepository.UpdateProjectWalletBalance(projectWallet);

                //Create CASH_IN WalletTransaction fromWallet to toWallet
                walletTransaction.Description = "Receive money from " + fromWalletType.Type + " wallet to " + toWalletType.Type + " wallet";
                walletTransaction.Type = WalletTransactionTypeEnum.CASH_IN.ToString();
                await _walletTransactionRepository.CreateWalletTransaction(walletTransaction);

                //Mapping response
                result.fromWalletId = fromWallet.Id;
                result.fromWalletName = fromWalletType.Name;
                result.fromWalletType = fromWalletType.Type;
                result.toWalletId = toWallet.Id;
                result.toWalletName = toWalletType.Name;
                result.toWalletType = toWalletType.Type;
                result.amount = transferDTO.amount;

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
