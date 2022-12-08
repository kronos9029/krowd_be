using Firebase.Auth;
using Google.Api.Gax.ResourceNames;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Services.Extensions.Email;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.Entities;
using RevenueSharingInvest.Data.Repositories.IRepos;
using RevenueSharingInvest.Data.Repositories.Repos;
using StackExchange.Redis;
using System;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;
using static iText.Kernel.Font.PdfFontFactory;
using Document = iText.Layout.Document;

namespace RevenueSharingInvest.Business.Services.Extensions.iText
{
    public class ITextService : IITextService
    {

        private readonly IInvestorRepository _investorRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly String COMPANY_NAME = " Công Ty Nhượng Quyền Thương Mại Điện Tử Krowd - Revenue Sharing Invest.";
        private readonly String COMPANY_ADDRESS = " Chung cư Vinhomes Grand Park Quận 9, Long Thạnh Mỹ, Quận 9, Thành phố Hồ Chí Minh, Việt Nam.";
        private readonly AppSettings _appSettings;
        private readonly IValidationService _validationService;

        public ITextService(IInvestorRepository investorRepository, IProjectRepository projectRepository, IFileUploadService fileUploadService, IOptions<AppSettings> appSettings, IValidationService validationService)
        {
            _investorRepository = investorRepository;
            _projectRepository = projectRepository;
            _fileUploadService = fileUploadService;
            _appSettings = appSettings.Value;
            _validationService = validationService;
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

        public async Task<string> GenerateProjectContract(ThisUserObj currentUser, string projectId, decimal amount, string invesmentId)
        {
            try
            {
                InvestorContractInfo investorInfo = await _investorRepository.GetInvestorNameByEmail(currentUser.email);
                if (investorInfo == null)
                    throw new NotFoundException("No Investor Found!!");
                string projectName = await _projectRepository.GetProjectNameForContractById(Guid.Parse(projectId));

                string directoryPath = GetDirectoryPath();
                directoryPath += "\\"+ investorInfo.Id + "\\";
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                string fullPath = directoryPath + invesmentId + ".pdf";

                ManipulateContract(fullPath, investorInfo, projectName, amount, invesmentId);

                string downloadLink = await GetFileToUploadAsync(fullPath, currentUser, projectName, invesmentId);

                return downloadLink;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }


        public async void ManipulateContract(string fullPath, InvestorContractInfo investor, string projectName, decimal amount, string invesmentId)
        {

            string fullName;
            if (investor.FirstName != null)
                fullName = investor.FirstName + " " + investor.LastName;
            else
                fullName = investor.LastName;

            //Initialize PDF writer
            //PdfWriter writer = new PdfWriter(fullPath);
            var userPassword = Encoding.UTF8.GetBytes(investor.SecretKey);
            var ownerPassword = Encoding.UTF8.GetBytes(_appSettings.Secret);

            PdfWriter writer = new(fullPath, new WriterProperties().SetStandardEncryption(userPassword, ownerPassword, EncryptionConstants.ALLOW_SCREENREADERS, EncryptionConstants.ENCRYPTION_AES_256));

            //Initialize PDF document
            PdfDocument pdfDoc = new(writer);
            //Initialize Document font
            PdfFont fontBold = GetFontType(FontType.Bold);
            PdfFont fontRegular = GetFontType(FontType.Regular);
            PdfFont fontItalic = GetFontType(FontType.Italic);
            PdfFont fontBoldItalic = GetFontType(FontType.Bold_Italic);
            string hour = DateTimePicker.GetDateTimeByTimeZone().Hour.ToString();
            string minute = DateTimePicker.GetDateTimeByTimeZone().Minute.ToString();
            string date = DateTimePicker.GetDateTimeByTimeZone().Day.ToString();
            string month = DateTimePicker.GetDateTimeByTimeZone().Month.ToString();
            string year = DateTimePicker.GetDateTimeByTimeZone().Year.ToString();

            string parsedAmount = await _validationService.FormatMoney(amount.ToString());

            //Initialize document
            Document document = new(pdfDoc);

            Paragraph nationalHeader = new();

            nationalHeader.Add("Cộng Hòa Xã Hội Chủ Nghĩa Việt Nam")
                .SetFontSize(20)
                .SetFont(fontBold)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetWordSpacing(-1);
            document.Add(nationalHeader);
            nationalHeader = new();
            nationalHeader.Add("Độc lập - Tự do - Hạnh phúc")
                .SetFontSize(15)
                .SetFont(fontBoldItalic)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingTop(-15);
            document.Add(nationalHeader);
            nationalHeader = new();
            nationalHeader.Add(hour + " giờ" + minute + " phút" + " , Ngày " + date + " Tháng " + month + " Năm " + year)
                .SetFontSize(10)
                .SetFont(fontRegular)
                .SetTextAlignment(TextAlignment.RIGHT);
            document.Add(nationalHeader);
            nationalHeader = new();
            nationalHeader.Add("HỢP ĐỒNG GÓP VỐN KINH DOANH")
                .SetFontSize(13)
                .SetFont(fontBold)
                .SetTextAlignment(TextAlignment.CENTER);
            document.Add(nationalHeader);
            nationalHeader = new();
            nationalHeader.Add("Mã Số: "+invesmentId+" HĐGVKD")
                .SetFontSize(12)
                .SetFont(fontRegular)
                .SetTextAlignment(TextAlignment.CENTER);
            document.Add(nationalHeader);

            Paragraph contractBody = new();
            contractBody.Add("Căn cứ Bộ luật dân sự năm 2015;\r\nCăn cứ vào nhu cầu kinh doanh và năng lực của các bên.\r\nChúng tôi gồm:")
                .SetFontSize(10)
                .SetFont(fontRegular);
            document.Add(contractBody);
            contractBody = new();
            contractBody.Add("BÊN NHẬN GÓP VỐN ( BÊN A):")
                .SetFontSize(12)
                .SetFont(fontBold)
                .SetWordSpacing(-3);
            document.Add(contractBody);
            contractBody = new();
            contractBody.Add("Tên tổ chức:" + COMPANY_NAME +
                "\nTrụ sở chính:" + COMPANY_ADDRESS +
                "\nMã số thuế: 3500806648 do Bộ Tài Chính cấp ngày 18/03/2018" +
                "\nĐại diện bởi: Ông: Đỗ Dương Tâm Khánh. Chức vụ: Tổng Giám Đốc Điều Hành").SetFontSize(10).SetFont(fontRegular);
            document.Add(contractBody);
            contractBody = new();
            contractBody.Add("BÊN GÓP VỐN ( BÊN B):").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
            document.Add(contractBody);
            contractBody = new();
            contractBody.Add("Ông/bà : " + fullName + ".   Sinh năm: " + investor.DateOfBirth +
                "\nChứng minh nhân dân số: " + investor.IdCard +
                "\nThường trú : " + investor.Address + ", " + investor.District + ", " + investor.City + "." +
                "\nSau khi bàn bạc thỏa thuận, hai bên đi đến thống nhất và đồng ý ký kết Hợp đồng góp vốn kinh doanh mã số: "+invesmentId+" HĐGVKD với các điều khoản sau:")
                .SetFontSize(10)
                .SetFont(fontRegular);
            document.Add(contractBody);

            Paragraph conditionTitle = new();
            Paragraph conditionBody = new();
            conditionTitle.Add("ĐIỀU 1: ĐỐI TƯỢNG HỢP ĐỒNG:").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
            conditionBody.Add("Bên B đồng ý góp vốn cho Bên A và cùng với đối tác của Bên A để triển khai dự án " + projectName + ".").SetFontSize(10).SetFont(fontRegular);
            document.Add(conditionTitle);
            document.Add(conditionBody);

            conditionTitle = new();
            conditionBody = new();
            conditionTitle.Add("ĐIỀU 2: TỔNG GIÁ TRỊ VỐN GÓP VÀ PHƯƠNG THỨC GÓP VỐN").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
            conditionBody.Add("Tổng giá trị đầu tư Bên B đầu tư cho bên A để thực hiện nội dung nêu tại Điều 1 là: " + parsedAmount + " VNĐ (Bằng chữ: " + NumberToTextVN(amount) + " ).")
                .SetFontSize(10)
                .SetFont(fontRegular);
            document.Add(conditionTitle);
            document.Add(conditionBody);

            document.Add(new AreaBreak());
            conditionTitle = new();
            conditionBody = new();
            conditionTitle.Add("ĐIỀU 3: PHÂN CHIA LỢI ÍCH VÀ THUA LỖ").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
            conditionBody.Add("Lợi ích được hiểu là phần trăm doanh thu hằng kỳ của dự án mà bên B đầu tư vào nhân cho phần trăm tổng đầu tư của bên B trên tổng số tiền kêu gọi cho dự án bên A" +
                "\nTức là số tiền bên B nhận được hằng kỳ phân chia theo công thức sau:" +
                "\n- Tổng doanh thu của dự án bên A * Phần trăm chia sẻ * (Tổng số tiền bên B đầu tư vào / Tổng số tiền bên A kêu gọi)")
                .SetFontSize(10)
                .SetFont(fontRegular);
            document.Add(conditionTitle);
            document.Add(conditionBody);

            conditionTitle = new();
            conditionBody = new();
            conditionTitle.Add("ĐIỀU 4: QUYỀN VÀ NGHĨA VỤ CỦA BÊN A").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
            conditionBody.Add("4.1 Quyền của Bên A:" +
                "\n- Các quyền khác theo Hợp đồng này hoặc do pháp luật quy định." +
                "\n4.2 Nghĩa vụ của Bên A:" +
                "\n- Trả lại số tiền tương đương với phần vốn góp của Bên B cho Bên B trong trường hợp đơn phương chấm dứt hợp đồng." +
                "\n- Báo cáo việc thay đổi, bổ sung thành viên góp vốn cho bên A" +
                "\n- Thông báo cho Bên A về việc đầu tư, xây dựng, khai thác tài sản góp vốn." +
                "\n- Các nghĩa vụ khác theo Hợp đồng này hoặc do pháp luật quy định.")
                .SetFontSize(10)
                .SetFont(fontRegular);
            document.Add(conditionTitle);
            document.Add(conditionBody);

            conditionTitle = new();
            conditionBody = new();
            conditionTitle.Add("ĐIỀU 5: QUYỀN VÀ NGHĨA VỤ CỦA BÊN B").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
            conditionBody.Add("5.1 Quyền của Bên B:" +
                "\n- Được hưởng lợi ích tương đương với phần vốn góp của mình." +
                "\n- Yêu cầu bên A cùng thanh toán số tiền đã đầu tư ban đầu trong trường hợp có không đạt được cam kết thanh toán ban đầu tối thiểu." +
                "\n- Được quyền đơn phương chấm dứt thực hiện hợp đồng trong trường hợp Bên A không thanh toán lợi nhuận cho mình và cùng chịu rủi ro với mình hoặc vi phạm nghĩa vụ của mình theo quy định tại Điều 4.2. Trong trường hợp này, Bên A phải thanh toán lại toàn bộ giá trị vốn góp cho Bên B và phải chịu phạt vi phạm  theo quy định tại Điều 7 cùng với bồi thường thiệt hại cho Bên B theo thiệt hại thực tế đã xảy ra mà Bên B phải gánh chịu." +
                "\n- Ưu tiên nhận chuyển nhượng phần vốn góp trong trường hợp Bên A có nhu cầu chuyển nhượng phần vốn góp." +
                "\n- Các quyền khác theo Hợp đồng này hoặc do pháp luật quy định." +
                "\n5.2 Nghĩa vụ của Bên B:" +
                "\n- Chịu lỗ tương ứng một phần hoặc toàn phần đầu tư ban đầu nếu xảy ra rủi ro ở dự án đầu tư" +
                "\n- Hỗ trợ cho Bên A để thực hiện các giao dịch liên quan đến phần vốn góp hoặc việc quản lý, khai thác tài sản tại Điều 1 nếu Bên A có yêu cầu." +
                "\n- Cung cấp cho Bên A đầy đủ các giấy tờ cần thiết để hoàn tất thủ tục pháp lý có liên quan nếu Bên A yêu cầu." +
                "\n- Các nghĩa vụ khác theo Hợp đồng này hoặc do pháp luật quy định.")
                .SetFontSize(10)
                .SetFont(fontRegular);
            document.Add(conditionTitle);
            document.Add(conditionBody);

            document.Add(new AreaBreak());
            conditionTitle = new();
            conditionBody = new();
            conditionTitle.Add("ĐIỀU 6: ĐIỀU KHOẢN CUỐI").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
            conditionBody.Add("- Các bên cam kết thực hiện đúng và đầy đủ các thỏa thuận tại Hợp đồng này." +
                "\n- Việc ký kết Hợp đồng này giữa các bên là hoàn toàn tự nguyện, không bị ép buộc, lừa dối. Trong quá trình thực hiện Hợp đồng, nếu cần thay đổi hoặc bổ sung nội dung của Hợp đồng này thì các bên thỏa thuận lập thêm Phụ lục Hợp đồng. Phụ lục hợp đồng là một phần không thể tách rời của Hợp đồng và có giá trị pháp lý như Hợp đồng." +
                "\n- Văn bản này được hiểu và chịu sự điều chỉnh của Pháp luật nước Cộng hoà xã hội chủ nghĩa Việt Nam." +
                "\n- Hai bên cam kết thực hiện tất cả những điều khoản đã cam kết trong văn bản. Bên nào vi phạm những cam kết trong văn bản này gây thiệt hại cho bên kia (trừ trong trường hợp bất khả kháng) thì phải bồi thường toàn." +
                "\n- Trong quá trình thực hiện công việc thỏa thuận trong văn bản nếu bên nào có khó khăn trở ngại thì phải báo cho bên kia trong vòng 1 (một) tháng kể từ ngày có khó khăn trở ngại." +
                "\n- Các bên có trách nhiệm thông tin kịp thời cho nhau tiến độ thực hiện công việc. Đảm bảo bí mật mọi thông tin liên quan tới quá trình sản xuất kinh doanh." +
                "\n- Mọi sửa đổi, bổ sung hợp đồng này đều phải được làm bằng văn bản tạo thành phụ lục và có chữ ký của hai bên. Các phụ lục là phần không tách rời của văn bản thỏa thuận này." +
                "\n- Mọi tranh chấp phát sinh trong quá trình thực hiện cam kết nêu trong văn bản này được giải quyết trước hết qua thương lượng, hoà giải, nếu hoà giải không thành việc tranh chấp sẽ được giải quyết tại Toà án có thẩm quyền.")
                .SetFontSize(10)
                .SetFont(fontRegular);
            document.Add(conditionTitle);
            document.Add(conditionBody);

            conditionTitle = new();
            conditionBody = new();
            conditionTitle.Add("ĐIỀU 7: HIỆU LỰC CỦA HỢP ĐỒNG").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
            conditionBody.Add("Hợp đồng này có hiệu lực kể từ ngày ký và được lập thành 02 (hai) bản và có giá trị pháp lý như nhau, Bên A giữ 01 (một) bản, Bên B giữ 01 (một) bản. Các bên đã đọc kỹ, hiểu rõ nội dung Hợp đồng và đồng ý ký tên.")
                .SetFontSize(10)
                .SetFont(fontRegular);
            document.Add(conditionTitle);
            document.Add(conditionBody);

            Paragraph signatureSpace = new();
            Paragraph fullnamePlease = new();
            signatureSpace.Add("BÊN A\t\t\t\t\t\t\t\t\t\t\t\t\t\tBÊN B").SetFontSize(12).SetFont(fontBold).SetPaddingTop(50).SetTextAlignment(TextAlignment.CENTER);
            fullnamePlease.Add("Đỗ Dương Tâm Khánh\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t" + fullName).SetFontSize(10).SetFont(fontItalic).SetTextAlignment(TextAlignment.CENTER);
            document.Add(signatureSpace);
            document.Add(fullnamePlease);

            

            //Close document
            document.Close();
            writer.Close();
        }

        public async Task<string> GetFileToUploadAsync(string path, ThisUserObj currentUser, string projectName, string invesmentId)
        {
            try
            {
                StreamReader sr = new(path);

                Task<string> downloadLink = _fileUploadService.UploadGeneratedContractToFirebase(currentUser.userId, invesmentId, sr.BaseStream);
                await Task.WhenAll(downloadLink);
                sr.Close();

                await Task.WhenAll(EmailService.SendEmail(path, currentUser.email, projectName));
                
                File.Delete(path);
                return downloadLink.Result;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }

        public static string NumberToTextVN(decimal total)
        {
            try
            {
                string rs = "";
                total = Math.Round(total, 0);
                string[] ch = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
                string[] rch = { "lẻ", "mốt", "", "", "", "lăm" };
                string[] u = { "", "mươi", "trăm", "ngàn", "", "", "triệu", "", "", "tỷ", "", "", "ngàn", "", "", "triệu" };
                string nstr = total.ToString();

                int[] n = new int[nstr.Length];
                int len = n.Length;
                for (int i = 0; i < len; i++)
                {
                    n[len - 1 - i] = Convert.ToInt32(nstr.Substring(i, 1));
                }

                for (int i = len - 1; i >= 0; i--)
                {
                    if (i % 3 == 2)// số 0 ở hàng trăm
                    {
                        if (n[i] == 0 && n[i - 1] == 0 && n[i - 2] == 0) continue;//nếu cả 3 số là 0 thì bỏ qua không đọc
                    }
                    else if (i % 3 == 1) // số ở hàng chục
                    {
                        if (n[i] == 0)
                        {
                            if (n[i - 1] == 0) { continue; }// nếu hàng chục và hàng đơn vị đều là 0 thì bỏ qua.
                            else
                            {
                                rs += " " + rch[n[i]]; continue;// hàng chục là 0 thì bỏ qua, đọc số hàng đơn vị
                            }
                        }
                        if (n[i] == 1)//nếu số hàng chục là 1 thì đọc là mười
                        {
                            rs += " mười"; continue;
                        }
                    }
                    else if (i != len - 1)// số ở hàng đơn vị (không phải là số đầu tiên)
                    {
                        if (n[i] == 0)// số hàng đơn vị là 0 thì chỉ đọc đơn vị
                        {
                            if (i + 2 <= len - 1 && n[i + 2] == 0 && n[i + 1] == 0) continue;
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 1)// nếu là 1 thì tùy vào số hàng chục mà đọc: 0,1: một / còn lại: mốt
                        {
                            rs += " " + ((n[i + 1] == 1 || n[i + 1] == 0) ? ch[n[i]] : rch[n[i]]);
                            rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);
                            continue;
                        }
                        if (n[i] == 5) // cách đọc số 5
                        {
                            if (n[i + 1] != 0) //nếu số hàng chục khác 0 thì đọc số 5 là lăm
                            {
                                rs += " " + rch[n[i]];// đọc số 
                                rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                                continue;
                            }
                        }
                    }

                    rs += (rs == "" ? " " : ", ") + ch[n[i]];// đọc số
                    rs += " " + (i % 3 == 0 ? u[i] : u[i % 3]);// đọc đơn vị
                }
                if (rs[rs.Length - 1] != ' ')
                    rs += " đồng";
                else
                    rs += "đồng";

                if (rs.Length > 2)
                {
                    string rs1 = rs.Substring(0, 2);
                    rs1 = rs1.ToUpper();
                    rs = rs.Substring(2);
                    rs = rs1 + rs;
                }
                return rs.Trim().Replace("lẻ,", "lẻ").Replace("mươi,", "mươi").Replace("trăm,", "trăm").Replace("mười,", "mười");
            }
            catch
            {
                return "";
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
