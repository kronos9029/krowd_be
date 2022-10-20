using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Repositories.IRepos;
using RevenueSharingInvest.Data.Repositories.Repos;
using System;
using System.IO;
using static iText.Kernel.Font.PdfFontFactory;

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

        private string GetDirectoryPath()
        {
            var path = Environment.GetEnvironmentVariable("BusinessContract_Path");
            if (path == null)
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

        private PdfFont GetFontType(FontType type)
        {
            PdfFont font;
            if (type == FontType.Bold)
                font = PdfFontFactory.CreateFont("C:\\Krowd_Font\\LmromanBold.otf", EmbeddingStrategy.FORCE_EMBEDDED);
            else if (type == FontType.Italic)
                font = PdfFontFactory.CreateFont("C:\\Krowd_Font\\LmromanItalic.otf", EmbeddingStrategy.FORCE_EMBEDDED);
            else if (type == FontType.Bold_Italic)
                font = PdfFontFactory.CreateFont("C:\\Krowd_Font\\LmromanBolditalic.otf", EmbeddingStrategy.FORCE_EMBEDDED);
            else
                font = PdfFontFactory.CreateFont("C:\\Krowd_Font\\LmromanRegular.otf", EmbeddingStrategy.FORCE_EMBEDDED);

            return font;
        }

        public async void GenerateProjectContract(ThisUserObj currentUser, string projectId)
        {
            try
            {
                InvestorName investor = await _investorService.GetInvestorNameByEmail(currentUser.email);
                if (investor == null)
                    throw new NotFoundException("No Investor Found!!");

                string fullName;
                if (investor.FirstName != null)
                    fullName = investor.FirstName + " " + investor.LastName;
                else
                    fullName = investor.LastName;
                string directoryPath = GetDirectoryPath();
                directoryPath += "\\" + fullName + "(" + investor.Id + ")" + "\\";
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                //Initialize PDF writer
                PdfWriter writer = new PdfWriter(directoryPath + projectId + ".pdf");
                //Initialize PDF document
                PdfDocument pdfDoc = new PdfDocument(writer);
                //Initialize Document font
                PdfFont fontBold = GetFontType(FontType.Bold);
                PdfFont fontRegular = GetFontType(FontType.Regular);
                PdfFont fontItalic = GetFontType(FontType.Italic);
                PdfFont fontBoldItalic = GetFontType(FontType.Bold_Italic);

                //Initialize document
                Document document = new Document(pdfDoc);

                Paragraph nationalHeader = new();

                nationalHeader.Add("Cộng Hòa Xã Hội Chủ Nghĩa Việt Nam\n").SetFontSize(20).SetFont(fontBold).SetTextAlignment(TextAlignment.CENTER);
                nationalHeader.Add("Độc Lập-Tự Do-Hạnh Phúc").SetFontSize(15).SetFont(fontBold).SetTextAlignment(TextAlignment.CENTER);

                //Add paragraph to the document
                document.Add(nationalHeader);



















                //Close document
                document.Close();
                writer.Close();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }
    }

    public enum FontType
    {
        Bold,
        Regular,
        Italic,
        Bold_Italic
    }

}
