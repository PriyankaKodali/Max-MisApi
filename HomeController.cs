using MaxMIS.Models;
using MaxMIS.ViewModels;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MaxMIS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }


        [System.Web.Http.HttpGet]
        public ActionResult GetInvoiceDetails(int ClientId, DateTime? fromdate, DateTime? todate)
        {
            try
            {
                

                using (MaxDbEntities db = new MaxDbEntities())
                {
                    var clientDetails = db.MaxClient_Details(ClientId, fromdate, todate).ToList();
                    var clientDueAmount = db.MaxClientDueDetails(ClientId).FirstOrDefault();
                    InvoiceModel invoice = new InvoiceModel();

                    if (clientDueAmount == null)
                    {
                        invoice.DueAmount=0;
                    }
                    else
                    {
                        invoice.DueAmount = Math.Round(Convert.ToDecimal(clientDueAmount.DueAmount));
                    }

                    List<InvoiceModel> invoices = new List<InvoiceModel>();

                    foreach (var client in clientDetails)
                    {
                        // InvoiceModel invoice = new InvoiceModel();
                        invoice.Client_Id = client.Client_Id;
                        invoice.Client_Name = client.ClientName;
                        invoice.ClientShortName = client.ShortName;
                        invoice.Address = client.AddressLine1;
                        invoice.ServiceType = client.ServiceType;
                        invoice.ServiceId = Convert.ToInt32(client.ClientVertical_Id);
                        invoice.PaymentType = client.PaymentType;
                        invoice.RowNum = Convert.ToInt32(client.RowNum);
                        invoice.LineCount = Convert.ToInt32(client.LineCount);
                        invoice.CountryName = client.Name;
                        if (client.PaymentType == "Per Unit")
                        {
                            invoice.UnitPrice = Convert.ToDecimal(client.PaymentAmount);
                            invoice.UnitPriceforDisplay = (client.PaymentAmount).ToString();
                            invoice.Amount = Math.Round(Convert.ToDecimal(client.LineCount * client.PaymentAmount));
                        }
                        if (client.PaymentType == "Fixed")
                        {
                            invoice.UnitPriceforDisplay = "Fixed";
                            invoice.UnitPrice = Convert.ToDecimal(client.PaymentAmount);
                            invoice.Amount = Math.Round(Convert.ToDecimal(client.PaymentAmount));
                        }

                        invoice.TotalAmount = Math.Round(Convert.ToDecimal(invoice.Amount + invoice.DueAmount));

                        invoice.MonthYear = fromdate.Value.ToString("MMMM yyyy");
                        invoices.Add(invoice);
                    }

                    invoice.InvoiceDate = fromdate.Value.ToShortDateString();
                    invoice.InvoiceDueDate = fromdate.Value.AddDays(10).ToShortDateString();
                    invoice.InvoiceCurrentDate = DateTime.Now.ToShortDateString();

                    var InvoiceCount = db.DeletedInvoices.Where(inv => inv.Client_Id == ClientId && inv.InvoiceCreatedDate.Value.Month == fromdate.Value.Month).Count();
                    int count = Convert.ToInt32(InvoiceCount);

                    invoice.InvoiceNumber = "MAX-" + invoice.Client_Id + (fromdate.Value.Month).ToString().Substring(0,1) + (fromdate.Value.Year).ToString().Substring(2,2) + "_" + ++count;

                    string html = RenderRazorViewToString("GetInvoiceDetails", invoice);
                    string baseUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                    HtmlToPdf converter = new HtmlToPdf();

                    converter.Options.DisplayFooter = true;
                    string footerUrl = Server.MapPath("~/views/home/footer.html");
                    PdfHtmlSection footerHtml = new PdfHtmlSection(footerUrl);
                    footerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                    converter.Footer.Add(footerHtml);

                    PdfDocument doc = converter.ConvertHtmlString(html, baseUrl);

                    // save pdf document
                    byte[] pdf = doc.Save();

                    // close pdf document
                    doc.Close();

                    // return resulted pdf document
                    FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                    fileResult.FileDownloadName = invoice.InvoiceNumber+"_"+ invoice.ClientShortName + ".pdf";
                    return fileResult;
                    //return View(invoice);
                }
            }
            catch (Exception ex)
            {
                new Error().logAPIError(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString(), ex.StackTrace);
                return View();
            }
        }


        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


    }
}
