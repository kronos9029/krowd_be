using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Extensions.iText
{
    public class iTextService
    {
        public void GenerateBusinessContract()
        {
            PdfWriter writer = new PdfWriter("C:");
        }
    }
}
