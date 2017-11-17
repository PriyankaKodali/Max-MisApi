using MaxMIS.Models;
using MaxMIS.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;


namespace MaxMIS.Controllers
{
    public class AddDataController : ApiController
    {

        [System.Web.Http.HttpPost]
        public IHttpActionResult AddInvoice()
        {
            try
            {
                InvoiceModel invoice = new InvoiceModel();

                var form = HttpContext.Current.Request.Form;
                var clientId = form.Get("Client_Id");
                var companyAddress = form.Get("CompanyAddress");
                var clientAddress = form.Get("ClientAddress");
                var invoiceDate = form.Get("InvoiceDate");
                var invoiceTime = form.Get("InvoiceTime");
                var dueAmount = form.Get("UnpaidBalance");
                if (dueAmount == "null")
                {
                    dueAmount = 0.ToString();
                }
                var CurrentAmount = form.Get("CurrentAmount");
                var totalLines = form.Get("TotalLines");
                var status = form.Get("Status");
                var serviceId = form.Get("ClientService");
                var unitPrice = form.Get("UnitPrice");
                var invoiceId = form.Get("InvoiceId");
                var createdBy = form.Get("CreatedBy");
                var invoiceSer = JsonConvert.DeserializeObject<List<string>>(form.Get("InvoiceServices"));

                using (MaxDbEntities db = new MaxDbEntities())

                {
                    Invoice inData = new Invoice();
                    InvoiceService inService = new InvoiceService();

                    inData.Client_Id = Convert.ToInt32(clientId);
                    inData.Id = invoiceId;
                    inData.CompanyAddress = companyAddress;
                    inData.ClientAddress = clientAddress;
                    inData.InvoiceCreatedBy = (from Emp in db.Employees where Emp.EmployeeNumber == createdBy select Emp.Id).FirstOrDefault();
                    inData.InvoiceCreatedDate = Convert.ToDateTime(invoiceDate);

                    string currency = (from client in db.Clients where client.Id == inData.Client_Id select client.Currency).FirstOrDefault();
                    inData.CurrencyType = currency;

                    inData.CurrentBillAmount = Convert.ToDecimal(CurrentAmount);
                    inData.TotalDueAmount = Convert.ToDecimal(dueAmount);
                    inData.TotalInvoiceAmount = Convert.ToDecimal(CurrentAmount);

                    foreach (string id in invoiceSer)
                    {
                        string unitprice = (from client in db.Clients where client.Id == inData.Client_Id select client.PaymentAmount).FirstOrDefault().ToString();

                        inService.InvoiceId = inData.Id;
                        inService.ClientId = Convert.ToInt32(clientId);
                        inService.ServiceId = Convert.ToInt32(serviceId);
                        inService.UnitPrice = Convert.ToDecimal(unitPrice);
                        inService.TotalLines = Convert.ToInt32(totalLines);
                        inService.BillAmount = Convert.ToDecimal(CurrentAmount);
                        inService.DueAmount = inService.BillAmount;
                    }
                    db.Invoices.Add(inData);
                    db.InvoiceServices.Add(inService);
                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                new Error().logAPIError(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString(), ex.StackTrace);
                return InternalServerError();
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult updateInvoice(string InvoiceId)
        {
            try
            {
                var form = HttpContext.Current.Request.Form;
                var approvedBy = form.Get("ApprovedBy");
                var approvedDate = form.Get("approvedDate");
                using (MaxDbEntities db = new MaxDbEntities())
                {
                    var invoices = db.Invoices.First(x => x.Id == InvoiceId);
                    invoices.InvoiceApprovedBy = (from emp in db.Employees where emp.EmployeeNumber == approvedBy select emp.Id).FirstOrDefault();
                    invoices.ApprovedDate = DateTime.Now;
                    db.Entry(invoices).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return Ok();
            }
            catch(Exception ex)
            {
                new Error().logAPIError(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString(), ex.StackTrace);
                return InternalServerError();
            }
        }
    }
}