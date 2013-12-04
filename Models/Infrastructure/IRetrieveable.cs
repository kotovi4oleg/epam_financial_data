using FinancialData.Models.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialData.Models.Infrastructure
{
    public interface IRetrieveable<out U>
    {
        U GetResponse(IUrlBuilderable _builder, bool need_serialization);
    }
}
