using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialData.Models.Infrastructure
{
    public interface IUrlBuilderable
    {
        string GetURL();
        string GetFormat(bool to_web_format);
    }
}
