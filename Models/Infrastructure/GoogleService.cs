using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialData.Models.Infrastructure
{
    public class GoogleService : IUrlBuilderable
    {
        protected string query;
        protected string datefrom;
        protected string dateto;
        protected string format;

        private const string URL = "http://www.google.com/finance/historical?q=";
        protected readonly int PER_PAGE = 10;
        
        protected int current_page;
        protected int Page { get { return this.current_page - 1; } set { this.current_page = value; } }

        protected readonly IDictionary<string, string> FormatDictionary = new Dictionary<string, string> { 
            { "XML",    "text/xml" },
            { "JSON",   "application/json" },
            { "CSV",    "text/plain"}
        };

        public GoogleService(string query, string datefrom, string dateto, string format, int page = 1) {
            this.query = query;
            this.datefrom = datefrom;
            this.dateto = dateto;
            this.format = format;
            this.Page = page;
        }

        public virtual string GetFormat(bool to_web_format)
        {
            if (to_web_format)
                return this.FormatDictionary[this.format];

            return this.format;
        }
        
        public virtual string GetURL()
        {
            DateTime from = new DateTime();
            DateTime to = new DateTime();
            try {
                from = DateTime.Parse(this.datefrom);
                to = DateTime.Parse(this.dateto);
            } catch {
                from = to = DateTime.Now;
            }

            return String.Concat(URL, 
                String.Format(
                    string.Join("&", new List<string> { 
                    this.query,
                    "startdate=" + from.ToString("MMM dd, yyyy"),
                    "enddate=" + to.ToString("MMM dd, yyyy"),
                    "start={0}",
                    "num={1}",
                    "output=csv"
                }), (this.Page * PER_PAGE), PER_PAGE));
        }
    }
}