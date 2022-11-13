using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.ExtensionDTOs
{
    public class Notification
    {
        public int Total { get; set; }
        public int New { get; set; }
        public List<NotificationDetail> Details { get; set; }
    }
    public class NotificationDetail
    {
        public string Title { get; set; }
        public string EntityId { get; set; }
        public string Image { get; set; }
        public string CreateDate { get; set; }
        public bool Seen { get; set; }
    }    
    public class NotificationDetailDTO
    {
        public string Title { get; set; }
        public string EntityId { get; set; }
        public string Image { get; set; }
    }


}
