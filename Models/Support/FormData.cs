using FinancialData.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialData.Models.Support
{
    public class FormData
    {
        #region form-data
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Quote { get; set; }
        public string Format { get; set; }
        public bool PlainText { get; set; }
        public AvailableServices Service { get; set; }
        #endregion
              
        public FormData() {
            this.Format = string.Empty;
            this.Quote  = string.Empty;
        }
    }
}