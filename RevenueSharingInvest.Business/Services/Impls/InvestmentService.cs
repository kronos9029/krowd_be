using AutoMapper;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class InvestmentService : IInvestorService
    {
        //private readonly IInvestorRepository _investorRepository;
        //private readonly IInvestorWalletRepository _investorWalletRepository;
        private readonly IMapper _mapper;

        public InvestmentService(IMapper mapper)
        {
            //_investorRepository = investorRepository;
            //_investorWalletRepository = investorWalletRepository;
            _mapper = mapper;
        }

        public Task<InvestorDTO> GetInvestorById(string ID)
        {
            throw new NotImplementedException();
        }
    }
}
