using FinancialData.Models.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FinancialData.Models.Infrastructure
{
    public class FinanceRetrieve : IRetrieveable<string>
    {
        #region FORMATS
        private const string PLAIN = "plain/text";
        private const string XML = "text/xml";
        private const string JSON = "application/json";
        #endregion

        public string GetResponse(IUrlBuilderable _arg, bool need_serialization)
        {
            if (_arg == null)
                throw new ArgumentException("[FinanceRetrieve] : FormData is required");
            SerializeStructure structure = new SerializeStructure
            {
                query = new Query { 
                    results = new Results()
                }
            };
            StringBuilder _builder = new StringBuilder();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_arg.GetURL());
            request.Method = "GET";
            request.Timeout = 10000;
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var body = new StreamReader(response.GetResponseStream()))
                    {
                        if (need_serialization)
                            return body.ReadToEnd();

                        switch (_arg.GetFormat(to_web_format:false).ToLower())
                        {
                            case "xml":
                                {
                                    XDocument xdoc = XDocument.Load(body);
                                    var quotes = xdoc.Descendants(XName.Get("quote"));
                                    structure.query.results.quote = quotes.Select(item => 
                                        new Quote
                                    {
                                        Date = item.Element(XName.Get("Date")).Value,
                                        Open = item.Element(XName.Get("Open")).Value,
                                        High = item.Element(XName.Get("High")).Value,
                                        Low = item.Element(XName.Get("Low")).Value,
                                        Close = item.Element(XName.Get("Close")).Value,
                                        Volume = item.Element(XName.Get("Volume")).Value
                                    }).ToList<Quote>();

                                    break;
                                }
                            case "csv":
                                structure.query.results.quote = this.CSVSerializer(body.ReadToEnd()).ToList();
                                break;
                            case "json":
                                structure = new JavaScriptSerializer().Deserialize<SerializeStructure>(body.ReadToEnd());
                                break;
                        }
                    }
                }
            }
            catch (WebException)
            {
                throw new InvalidOperationException("Sorry, error is occurred during request. Please try again.");
            }
            catch (Exception)
            {
                throw new InvalidOperationException(string.Format("Error while getting response. This service probably does not supported format {0} ", _arg.GetFormat(to_web_format:false)));
            }

            if (structure.query.results != null && structure.query.results.quote.Count > 0)
            {
                _builder.Append(
                    new JavaScriptSerializer().Serialize(
                        structure.query.results.quote.Select(quote =>
                            new
                            {
                                _Date = quote.Date,
                                _Open = quote.Open,
                                _High = quote.High,
                                _Low = quote.Low,
                                _Close = quote.Close,
                                _Volume = quote.Volume
                            })));
            }
            return _builder.ToString();
        }


        private IEnumerable<Quote> CSVSerializer(string body)
        {
            List<string> rows = body.Split('\n').ToList();
            List<string> columns = rows[0].Split(',').Select(item => item.Trim()).ToList();
            List<Quote> result_list = new List<Quote>();
            for (int row = 1; row < rows.Count; row++)
            {
                String[] one_row = rows[row].Split(',');
                IDictionary<string, string> row_values = new Dictionary<string, string>();
                if (one_row.Length == columns.Count)
                {
                    for (int column = 0; column < columns.Count; column++)
                    {
                        row_values[columns[column]] = one_row[column];
                    }

                    yield return new Quote
                    {
                        Date = row_values["Date"],
                        Open = row_values["Open"],
                        High = row_values["High"],
                        Low = row_values["Low"],
                        Close = row_values["Close"],
                        Volume = row_values["Volume"]
                    };
                }

                row_values = null;
            }

        }
    }
}