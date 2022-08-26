using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.Constants
{
    public class InvestorTypeDictionary
    {
        public static readonly Dictionary<string, string> investorType = new Dictionary<string, string>
        {
            {"Nhà đầu tư dài hạn", "EC92EF2A-F794-11EC-B939-0242AC120002"},
            {"Nhà đầu tư theo sở thích", "07C55F72-F794-11EC-B939-0242AC120002"},
            {"Nhà đầu tư ngắn hạn", "CA4E68CC-F794-11EC-B939-0242AC120002"},
            {"Nhà đầu tư khôn khéo", "175389B8-F795-11EC-B939-0242AC120002"}
        };
    }
}
