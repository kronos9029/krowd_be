using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authentication.Cookies;
using RevenueSharingInvest.API;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Data.Extensions;
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
                string hour = DateTimePicker.GetDateTimeByTimeZone().Hour.ToString();
                string minute = DateTimePicker.GetDateTimeByTimeZone().Minute.ToString();
                string date = DateTimePicker.GetDateTimeByTimeZone().Day.ToString();
                string month = DateTimePicker.GetDateTimeByTimeZone().Month.ToString();
                string year = DateTimePicker.GetDateTimeByTimeZone().Year.ToString();
                //Initialize document
                Document document = new Document(pdfDoc);

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
                nationalHeader.Add(hour + ":" + minute+", Ngày " + date + " Tháng " + month + " Năm " + year)
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
                nationalHeader=new();
                nationalHeader.Add("Số:…/…/HĐGVKD")
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
                contractBody.Add("Tên tổ chức: ………………………………………………………………………." +
                    "\nTrụ sở chính: ……………………………………………………………………….." +
                    "\nMã số thuế: …do … cấp ngày …/…/…" +
                    "\nĐại diện bởi: Ông/bà:……………….Chức vụ: ……………………………………").SetFontSize(10).SetFont(fontRegular);
                document.Add(contractBody);
                contractBody = new();
                contractBody.Add("BÊN NHẬN GÓP VỐN ( BÊN A):").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
                document.Add(contractBody);                
                contractBody = new();
                contractBody.Add("Ông/bà : …………………………. Sinh năm: ……………………………………" +
                    "\nChứng minh nhân dân số: …  Ngày cấp: …/…/….   Nơi cấp: ……………………" +
                    "\nThường trú : ……………………………………………………………………….." +
                    "\nSau khi bàn bạc thỏa thuận, hai bên đi đến thống nhất và đồng ý ký kết Hợp đồng góp vốn kinh doanh số:…/…/HĐGVKD với các điều khoản sau:")
                    .SetFontSize(10)
                    .SetFont(fontRegular);
                document.Add(contractBody);

                Paragraph conditionTitle = new();
                Paragraph conditionBody = new();
                conditionTitle.Add("ĐIỀU 1: ĐỐI TƯỢNG HỢP ĐỒNG:").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
                conditionBody.Add("Bên B đồng ý góp vốn cho Bên A và cùng với đối tác của Bên A để: …………..").SetFontSize(10).SetFont(fontRegular);
                document.Add(conditionTitle);
                document.Add(conditionBody);

                conditionTitle = new();
                conditionBody = new();
                conditionTitle.Add("ĐIỀU 2: TỔNG GIÁ TRỊ VỐN GÓP VÀ PHƯƠNG THỨC GÓP VỐN").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
                conditionBody.Add("Tổng giá trị vốn góp Bên A và Bên B góp để thực hiện nội dung nêu tại Điều 1 là:… Nay Bên B góp vốn cho Bên A với số tiền: … VNĐ (Bằng chữ:…) tương đương …% tổng giá trị vốn góp nêu trên.")
                    .SetFontSize(10)
                    .SetFont(fontRegular);
                document.Add(conditionTitle);
                document.Add(conditionBody);

                document.Add(new AreaBreak());
                conditionTitle = new();
                conditionBody = new();
                conditionTitle.Add("ĐIỀU 3: PHÂN CHIA LỢI NHUẬN VÀ THUA LỖ").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
                conditionBody.Add("Lợi nhuận được hiểu và khoản tiền còn dư ra sau khi trừ đi các chi phí cho việc đầu tư, quản lý tài sản góp vốn." +
                    "\nLợi nhuận được phân chia theo tỷ lệ sau:" +
                    "\n- Bên A được hưởng …% lợi nhuận trong tổng giá trị lợi nhuận thu được từ tài sản góp vốn." +
                    "\n- Bên B được hưởng …% lợi nhuận trong tổng giá trị lợi nhuận thu được từ tài sản góp vốn." +
                    "\n- Lợi nhuận chỉ được chia khi trừ hết mọi chi phí mà vẫn còn lợi nhuận. Nếu kinh doanh thua lỗ thì các bên có trách nhiệm chịu lỗ theo phần vốn góp của mình tương tự như phân chia lợi nhuận." +
                    "\n- Trường hợp các bên cần huy động vốn thêm từ Ngân hàng để đầu tư thực hiện dự án trên đất thì số lãi phải đóng cho Ngân hàng cũng được chia theo tỷ lệ vốn góp.")
                    .SetFontSize(10)
                    .SetFont(fontRegular);
                document.Add(conditionTitle);
                document.Add(conditionBody);                
                
                conditionTitle = new();
                conditionBody = new();
                conditionTitle.Add("ĐIỀU 4: QUYỀN VÀ NGHĨA VỤ CỦA BÊN A").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
                conditionBody.Add("4.1 Quyền của Bên A:" +
                    "\n- Yêu cầu Bên B góp vốn đúng thời điểm và số tiền theo thỏa thuận trong hợp đồng này." +
                    "\n- Được quyền đơn phương chấm dứt thực hiện hợp đồng trong trường hợp Bên B không góp đủ vốn hoặc góp vốn không đúng thời hạn." +
                    "\n- Được hưởng lợi nhuận tương đương với phần vốn góp của mình." +
                    "\n- Yêu cầu bên B thanh toán lỗ trong trường hợp có thua lỗ." +
                    "\n- Ưu tiên nhận chuyển nhượng phần vốn góp trong trường hợp Bên B có nhu cầu chuyển nhượng phần vốn góp." +
                    "\n- Các quyền khác theo Hợp đồng này hoặc do pháp luật quy định." +
                    "\n4.2 Nghĩa vụ của Bên A:" +
                    "\n- Trả lại số tiền tương đương với phần vốn góp của Bên B cho Bên B trong trường hợp đơn phương chấm dứt hợp đồng." +
                    "\n- Báo cáo việc thay đổi, bổ sung thành viên góp vốn cho bên A" +
                    "\n- Thông báo cho Bên A về việc đầu tư, xây dựng, khai thác tài sản góp vốn." +
                    "\n- Hỗ trợ cho Bên B để thực hiện các giao dịch chuyển nhượng phần vốn góp này khi có yêu cầu từ Bên B cho bên thứ ba và thực hiện các thủ tục có liên quan cho bên B hoặc bên thứ ba;" +
                    "\n- Các nghĩa vụ khác theo Hợp đồng này hoặc do pháp luật quy định.")
                    .SetFontSize(10)
                    .SetFont(fontRegular);
                document.Add(conditionTitle);
                document.Add(conditionBody);

                document.Add(new AreaBreak());
                conditionTitle = new();
                conditionBody = new();
                conditionTitle.Add("ĐIỀU 5: QUYỀN VÀ NGHĨA VỤ CỦA BÊN B").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
                conditionBody.Add("5.1 Quyền của Bên B:" +
                    "\n- Được hưởng lợi nhuận tương đương với phần vốn góp của mình." +
                    "\n- Yêu cầu bên A cùng thanh toán lỗ trong trường hợp có thua lỗ." +
                    "\n- Chuyển nhượng phần vốn góp cho Bên thứ ba nếu được Bên B đồng ý bằng văn bản." +
                    "\n- Được quyền đơn phương chấm dứt thực hiện hợp đồng trong trường hợp Bên A không thanh toán lợi nhuận cho mình và cùng chịu rủi ro với mình hoặc vi phạm nghĩa vụ của mình theo quy định tại Điều 4.2. Trong trường hợp này, Bên A phải thanh toán lại toàn bộ giá trị vốn góp cho Bên B và phải chịu phạt vi phạm  theo quy định tại Điều 7 cùng với bồi thường thiệt hại cho Bên B theo thiệt hại thực tế đã xảy ra mà Bên B phải gánh chịu." +
                    "\n- Ưu tiên nhận chuyển nhượng phần vốn góp trong trường hợp Bên A có nhu cầu chuyển nhượng phần vốn góp." +
                    "\n- Các quyền khác theo Hợp đồng này hoặc do pháp luật quy định." +
                    "\n5.2 Nghĩa vụ của Bên B:" +
                    "\n- Góp vốn vào đúng thời điểm và giá trị theo các thỏa thuận của Hợp đồng này." +
                    "\n- Chịu lỗ tương ứng với phần vốn góp của mình theo thỏa thuận trong hợp đồng này." +
                    "\n- Hỗ trợ cho Bên A để thực hiện các giao dịch liên quan đến phần vốn góp hoặc việc quản lý, khai thác tài sản tại Điều 1 nếu Bên A có yêu cầu." +
                    "\n- Cung cấp cho Bên A đầy đủ các giấy tờ cần thiết để hoàn tất thủ tục pháp lý có liên quan nếu Bên A yêu cầu." +
                    "\n- Thông báo trước 01 tháng cho Bên A biết việc chuyển nhượng phần vốn góp của mình cho Bên thứ ba." +
                    "\n- Các nghĩa vụ khác theo Hợp đồng này hoặc do pháp luật quy định.")
                    .SetFontSize(10)
                    .SetFont(fontRegular);
                document.Add(conditionTitle);
                document.Add(conditionBody);                
                
                conditionTitle = new();
                conditionBody = new();
                conditionTitle.Add("ĐIỀU 6: CHUYỂN NHƯỢNG HỢP ĐỒNG").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
                conditionBody.Add("- Trong quá trình thực hiện hợp đồng này, Bên B có quyền đề nghị chuyển nhượng toàn bộ quyền và nghĩa vụ của hợp đồng này cho bên thứ ba. Đề nghị chuyển nhượng phải được lập thành văn bản và được sự chấp thuận của bên A." +
                    "\n- Trước khi ký kết thỏa thuận chuyển nhượng hợp đồng thì bên B phải thanh toán cho bên A các khoản tiền còn thiếu (nếu có)." +
                    "\n- Thỏa thuận chuyển nhượng giữa ba bên sẽ được lập thành văn bản. Bên B sẽ chuyển giao toàn bộ quyền và nghĩa vụ và bên thứ ba chấp thuận, cam kết nhận chuyển giao toàn bộ quyền và nghĩa vụ từ bên B." +
                    "\n- Phí chuyển nhượng hợp đồng này cho bên thứ ba do Bên B chịu.")
                    .SetFontSize(10)
                    .SetFont(fontRegular);
                document.Add(conditionTitle);
                document.Add(conditionBody);

                document.Add(new AreaBreak());
                conditionTitle = new();
                conditionBody = new();
                conditionTitle.Add("ĐIỀU 7: ĐIỀU KHOẢN CUỐI").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
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
                conditionTitle.Add("ĐIỀU 8: HIỆU LỰC CỦA HỢP ĐỒNG").SetFontSize(12).SetFont(fontBold).SetWordSpacing(-3);
                conditionBody.Add("Hợp đồng này có hiệu lực kể từ ngày ký và được lập thành 02 (hai) bản và có giá trị pháp lý như nhau, Bên A giữ 01 (một) bản, Bên B giữ 01 (một) bản. Các bên đã đọc kỹ, hiểu rõ nội dung Hợp đồng và đồng ý ký tên.")
                    .SetFontSize(10)
                    .SetFont(fontRegular);
                document.Add(conditionTitle);
                document.Add(conditionBody);

                Paragraph signatureSpace = new();
                Paragraph fullnamePlease = new();
                signatureSpace.Add("BÊN A\t\t\t\t\t\t\t\t\t\t\t\t\t\tBÊN B").SetFontSize(12).SetFont(fontBold).SetPaddingTop(50).SetTextAlignment(TextAlignment.CENTER);
                fullnamePlease.Add("Ký và ghi rõ họ tên\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tKý và ghi rõ họ tên").SetFontSize(10).SetFont(fontItalic).SetTextAlignment(TextAlignment.CENTER);
                document.Add(signatureSpace);                
                document.Add(fullnamePlease);                
                
                






















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
