using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialData.Models.Infrastructure
{
    public static class EnumToService
    {
        public static IUrlBuilderable ConvertToURLBuilderable(this AvailableServices service, string query, string datefrom, string dateto, string format, int page = 1) {
            IUrlBuilderable result = null;
            switch (service) { 
                case AvailableServices.GOOGLE :
                    result = new GoogleService(query, datefrom, dateto, format, page);
                    break;
                case AvailableServices.YAHOO:
                    result = new YahooService(query, datefrom, dateto, format, page);
                    break;
            }

            return result;
        }
    }
}