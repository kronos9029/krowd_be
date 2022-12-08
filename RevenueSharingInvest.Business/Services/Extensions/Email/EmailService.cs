using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs;
using RevenueSharingInvest.Business.Services.Extensions.Security;
using System.IO;
using Google.Cloud.Firestore;

namespace RevenueSharingInvest.Business.Services.Extensions.Email
{
    public static class EmailService
    {
        //private static readonly string App_Password = "hchr lwct gcor qtsr";
        private static readonly string APP_PASSWORD = "cxrdzurzsizgzaep";
        private static readonly string SENDER = "krowd.2022@gmail.com";
        public static async Task SendEmail(string filePath, string receiver, string projectName)
        {
            string SendMailSubject = "KROWD - Hợp Đồng Góp Vốn Kinh Doanh";
            string SendMailBody = "Hợp đồng góp vốn kinh doanh của dự án " + projectName;
            try
            {
                SmtpClient SmtpServer = new("smtp.gmail.com", 587)
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 5000,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(SENDER, APP_PASSWORD)
                };

                MailMessage email = new()
                {
                    // START
                    From = new MailAddress(SENDER),
                    Subject = SendMailSubject,
                    Body = SendMailBody
                };

                var fileChecksum = await Task.WhenAll(GenerateFileHash.GetHash(HashingAlgoTypes.SHA256, filePath));

                email.Body += "\nChecksum của file hợp đồng: " + fileChecksum[0];
                email.Body += "\nChecksum (tổng kiểm tra) là kết quả của việc chạy một thuật toán, được gọi là hàm băm mật mã, trên một phần dữ liệu, thường là một tệp duy nhất. So sánh Checksum mà bạn tạo từ phiên bản tệp của mình với phiên bản do nguồn tệp cung cấp, giúp đảm bảo rằng bản sao tệp của bạn là chính hãng và không có lỗi.\r\n\r\nTìm hiểu thêm về Checksum: https://wikimaytinh.com/checksum-la-gi-dung-checksum-de-lam-gi.html";
                email.Attachments.Add(new Attachment(filePath));
                email.To.Add(receiver);
                email.CC.Add(SENDER);

                //END

                SmtpServer.Send(email);
                email.Attachments.ToList().ForEach(x => x.Dispose());
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
            }

        }

        public static async Task<string> SendFancyEmail(string projectName, string investorName, string receiver)
        {
            try
            {
                SmtpClient SmtpServer = new("smtp.gmail.com", 587)
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 5000,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(SENDER, APP_PASSWORD)
                };

                string emailString = GetEmailTemplate();
                emailString.Replace("[InvestorName],", investorName);


            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.ToString());
            }
        }

        private static string GetEmailTemplate()
        {
            try
            {
                var path = Environment.GetEnvironmentVariable("EmailTemplate_Path");
                if (path == null)
                {
                    Environment.SetEnvironmentVariable("EmailTemplate_Path", "C:\\EmailTemplate\\EmailTemplate.html");
                    path = Environment.GetEnvironmentVariable("EmailTemplate_Path");
                    if (!Directory.Exists(path))
                    {
                        path = "C:\\EmailTemplate";
                        Directory.CreateDirectory(path);
                    }
                }
                else
                {
                    StreamReader stream = new(path);
                    string mailTemp = stream.ReadToEnd();
                    return mailTemp;
                }
                return path;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }
    }
}
