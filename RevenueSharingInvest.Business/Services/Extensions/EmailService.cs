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

namespace RevenueSharingInvest.Business.Services.Extensions
{
    public static class EmailService
    {
        //private static readonly string App_Password = "hchr lwct gcor qtsr";
        private static readonly string APP_PASSWORD = "hchrlwctgcorqtsr";
        private static readonly string SENDER = "krowd.dev.2022@gmail.com";
        public static async Task SendEmail(string filePath, string receiver, string projectName)
        {
            String SendMailSubject = "KROWD - Hợp Đồng Góp Vốn Kinh Doanh";
            String SendMailBody = "Hợp đồng góp vốn kinh doanh của dự án "+projectName;
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
                throw new Exception(e.Message);
            }

        }
    }
}
