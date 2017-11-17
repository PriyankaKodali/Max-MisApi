using MaxMIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MaxMIS.Controllers
{
    public class DeleteInvoiceController : ApiController
    {
        [System.Web.Http.HttpPost]
        public IHttpActionResult DeleteInvoice(string InvoiceId)
        {
            try
            {

                var form = HttpContext.Current.Request.Form;
                var description = form.Get("Description");
                var invoiceId = form.Get("InvoiceId");

                using (MaxDbEntities db = new MaxDbEntities())
                {
                    //var invoices = db.Invoices.Where(x => x.Id==InvoiceId).FirstOrDefault();
                    //var invoiceServices = db.InvoiceServices.Where(x => x.InvoiceId == InvoiceId).FirstOrDefault();

                    //  var deleteRecord = db.DeleteClientPayment(InvoiceId);

                    var invoiceSer = db.InvoiceServices.First(x => x.InvoiceId == InvoiceId);
                    var Inv = db.Invoices.First(x => x.Id == InvoiceId);

                    if (InvoiceId == null)
                    {
                        return Content(HttpStatusCode.InternalServerError, "Invalid data, please try again!");
                    }

                    if (invoiceSer.PaymentId == null)
                    {
                        DeletedInvoice Di = new DeletedInvoice();
                        DeletedInvoiceService DInvs = new DeletedInvoiceService();
                     
                        Di.Id = Inv.Id;
                        Di.ApprovedDate = Inv.ApprovedDate;
                        Di.ClientAddress = Inv.ClientAddress;
                        Di.Client_Id = Inv.Client_Id;
                        Di.CompanyAddress = Inv.CompanyAddress;
                        Di.CurrencyType = Inv.CurrencyType;
                        Di.CurrentBillAmount = Inv.CurrentBillAmount;
                        Di.InvoiceApprovedBy = Inv.InvoiceApprovedBy;
                        Di.ApprovedDate = Inv.ApprovedDate;
                        Di.InvoiceCreatedBy =Inv.InvoiceCreatedBy;
                        Di.InvoiceCreatedDate = Inv.InvoiceCreatedDate;
                        Di.InvoiceURL = Inv.InvoiceURL;
                        Di.Status = Inv.Status;
                        Di.TotalDueAmount = Inv.TotalDueAmount;
                        Di.TotalInvoiceAmount = Inv.TotalInvoiceAmount;
                        Di.Description = description;

                        DInvs.AmountPaidDate = invoiceSer.AmountPaidDate;
                        DInvs.BillAmount = invoiceSer.BillAmount;
                        DInvs.ClientId = invoiceSer.ClientId;
                        DInvs.DueAmount = invoiceSer.DueAmount;
                        DInvs.InvoiceId = invoiceSer.InvoiceId;
                        DInvs.PaymentId = invoiceSer.PaymentId;
                        DInvs.ServiceId = invoiceSer.ServiceId;
                        DInvs.Id = invoiceSer.Id;

                        db.DeletedInvoices.Add(Di);
                        db.DeletedInvoiceServices.Add(DInvs);

                        db.InvoiceServices.Remove(invoiceSer);
                        db.Invoices.Remove(Inv);
                        db.SaveChanges();
                    }
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                new Error().logAPIError(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString(), ex.StackTrace);
                return InternalServerError();
            }
        }
    }
}