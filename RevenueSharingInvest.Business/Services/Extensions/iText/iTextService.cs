using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using RevenueSharingInvest.Data.Repositories.Repos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions.iText
{
    public class ITextService : IITextService
    {

        private readonly IInvestorRepository _investorService;
        private readonly IProjectRepository _projectService;
        public ITextService(IInvestorRepository investorRepository, IProjectRepository projectRepository)
        {
            _investorService = investorRepository;
            _projectService = projectRepository;
        }

        private string GetPath()
        {
            var path = Environment.GetEnvironmentVariable("BusinessContract_Path");
            if(path == null) 
            {
                Environment.SetEnvironmentVariable("BusinessContract_Path", "C:\\Contracts\\Project-Investor");
                path = Environment.GetEnvironmentVariable("BusinessContract_Path");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                }
            }

            return path;
        }

        public async void GenerateProjectContract(ThisUserObj currentUser, string projectId)
        {
            try
            {
                InvestorName investor = await _investorService.GetInvestorNameByEmail(currentUser.email);
                string path = GetPath();
                if (investor == null)
                {
                    throw new NotFoundException("No Investor Found!!");
                }
                string fullName;
                if(investor.FirstName != null)
                    fullName = investor.FirstName +" "+investor.LastName;
                else
                    fullName = investor.LastName;
                path += "\\" + fullName + "(" + investor.Id + ")" + "\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //Initialize PDF writer
                PdfWriter writer = new PdfWriter(path+projectId+".pdf");
                //Initialize PDF document
                PdfDocument pdfDoc = new PdfDocument(writer);

                //Initialize document
                Document document = new Document(pdfDoc);
                //Add paragraph to the document
                document.Add(new Paragraph("Cộng Hòa Xã Hội Chủ Nghĩa Việt Nam").SetFontSize(20).SetTextAlignment(TextAlignment.CENTER));
                //Close document
                document.Close();
                writer.Close();
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }
    }
}
