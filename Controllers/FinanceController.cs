using FinancialData.Models.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using FinancialData.Models.Infrastructure;

namespace FinancialData.Controllers
{
    public class FinanceController : Controller
    {
        //
        // GET: /Finance/
        private IRetrieveable<string> _retrieve;
        
        public FinanceController(IRetrieveable<string> _retrieve) {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");  
            this._retrieve = _retrieve;
        }

        [HttpGet]
        public ActionResult Finance()
        {
            return View(new FormData());
        }

        [HttpGet]
        public ActionResult FinanceList(
            AvailableServices Service, 
            bool PlainText, 
            string Quote, 
            string DateFrom, 
            string DateTo,
            string Format,
            int page = 1
            )
        {
            try
            {
                var url_builder = Service.ConvertToURLBuilderable(Quote, DateFrom, DateTo, Format, page);
                var answer = this._retrieve.GetResponse(url_builder, need_serialization: PlainText);
                if (answer.Equals(string.Empty))
                    throw new InvalidOperationException(string.Format("Nothing found for quote {0}", Quote));
                if (!PlainText)
                    return Content(answer, "application/json");
                else
                    return Content(answer, url_builder.GetFormat(to_web_format: true));
            }
            catch (InvalidOperationException e)
            {
                return Json(new { errorMessage = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception) {
                return Json(new { errorMessage = "Sorry, error is occured. Check your form data and try again." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
