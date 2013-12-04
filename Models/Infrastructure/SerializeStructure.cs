using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialData.Models.Infrastructure
{
    public class SerializeStructure
    {
        public Query query { get; set; }
    }
    public class Quote
    {
        public String Date { get; set; }
        public String Open { get; set; }
        public String High { get; set; }
        public String Low { get; set; }
        public String Close { get; set; }
        public String Volume { get; set; }
        public String Adj_Close { get; set; }
    }

    public class Results
    {
        public List<Quote> quote { get; set; }
    }

    public class Query
    {
        public int count { get; set; }
        public string created { get; set; }
        public string lang { get; set; }
        public Results results { get; set; }
    }
}