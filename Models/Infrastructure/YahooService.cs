using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialData.Models.Infrastructure
{
    public class YahooService : GoogleService
    {
        private const string URL = "http://query.yahooapis.com/v1/public/yql?q=";

        public YahooService(string query, string datefrom, string dateto, string format, int page = 1) 
            : base(query, datefrom, dateto, format, page) {
        }

        public override string GetURL()
        {
            DateTime from = new DateTime();
            DateTime to = new DateTime();
            try
            {
                from = DateTime.Parse(this.datefrom);
                to = DateTime.Parse(this.dateto);
            }
            catch
            {
                from = to = DateTime.Now;
            }

            List<string> url_params = this.query.Split(';').ToList<string>();
            for (int step = 0; step < url_params.Count; step++)
                url_params[step] = string.Format("\"{0}\"", url_params[step]);
            var request_params = string.Join(", ", url_params);
            var yql = "select * from yahoo.finance.historicaldata where symbol in ({0}) and startDate = \"{1}\" and endDate = \"{2}\" ";
            yql = string.Format(yql, request_params, from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"));
            yql += "LIMIT {0} OFFSET {1}";
            yql += "&env=http://datatables.org/alltables.env";
            yql += "&format=" + this.format.ToLower();
            var complete_url = String.Concat(URL, String.Format(yql, PER_PAGE, (this.Page * PER_PAGE)));

            return complete_url;
        }
    }
}