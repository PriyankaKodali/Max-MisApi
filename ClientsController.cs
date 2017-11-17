using MaxMIS.Models;
using MaxMIS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;


namespace MaxMIS.Controllers
{
    public class ClientsController : ApiController
    {
        public IHttpActionResult GetAllClients()
        {
            try
            {
                using (MaxDbEntities db = new MaxDbEntities())
                {
                    var ClientNames = (from client in db.Clients select new { label = client.ShortName, value = client.Id }).OrderBy(x => x.label).ToList();

                    return Content(HttpStatusCode.OK, new { ClientNames });
                }
            }
            catch (Exception ex)
            {
                new Error().logAPIError(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString(), ex.StackTrace);
                return Content(HttpStatusCode.InternalServerError, "An error occoured, please try again!");
            }
        }

        public IHttpActionResult GetClientInvoice(int ClientId, DateTime fromDate)
        {
            try
            {
                using (MaxDbEntities db = new MaxDbEntities())
                {
                    var InvoiceCount = db.DeletedInvoices.Where(inv => inv.Client_Id == ClientId && inv.InvoiceCreatedDate.Value.Month == fromDate.Month).Count();
                    return Content(HttpStatusCode.OK, new { InvoiceCount });
                }
            }
            catch (Exception ex)
            {
                new Error().logAPIError(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString(), ex.StackTrace);
                return Content(HttpStatusCode.InternalServerError, "An error occoured, please try again!");
            }
        }

        public IHttpActionResult GetClientReport(int? clientId, int? page, int? count, DateTime? fromdate, DateTime? todate, string SortCol, string SortDir)
        {
            try
            {
                using (MaxDbEntities db = new MaxDbEntities())
                {

                    int totalCount = 0;
                    var clientReport = db.MaxClientReport(clientId, page, count, fromdate, todate, SortCol, SortDir).ToList();

                    if (clientReport.Count > 0)
                    {
                        totalCount = (int)clientReport.FirstOrDefault().TotalCount;
                    }
                    List<ClientsViewModel> clients = new List<ClientsViewModel>();
                    TotalCountOfCharacters totalChar = new TotalCountOfCharacters();
                    int clientCount = clientReport.Count();

                    foreach (var client in clientReport)
                    {
                        ClientsViewModel clientView = new ClientsViewModel();

                        clientView.ClientId = client.Id;
                        clientView.ShortName = client.ShortName;
                        clientView.NumberOfCharactersPerLine = Convert.ToInt32(client.NumberOfCharactersPerLine);
                        clientView.AmountPerUnit = Convert.ToDecimal(client.PaymentAmount);
                        clientView.Currency = client.Currency;

                        clientView.RowNum = Convert.ToInt32(client.RowNum);
                        clientView.LineCount_55 = Convert.ToInt32(client.LineCount_55);
                        totalChar.TotalCharCount_55 += clientView.LineCount_55;

                        clientView.LineCount_60 = Convert.ToInt32(client.LineCount_60);
                        totalChar.TotalCharCount_60 += clientView.LineCount_60;

                        clientView.LineCount_65 = Convert.ToInt32(client.LineCount_65);
                        totalChar.TotalCharCount_65 += clientView.LineCount_65;

                        clientView.ClientLineCount = Convert.ToInt32(client.ActualLineCount);
                        clientView.TotalJobs = Convert.ToInt32(client.TotalJobs);
                        clientView.EmptyJobs = Convert.ToInt32(client.EmptyJobs);
                        clientView.PaymentType = client.PaymentType;

                        bool InvoiceGenerated = db.Invoices.Any(x => x.Client_Id == client.Id && x.InvoiceCreatedDate.Value.Month == fromdate.Value.Month);
                        clientView.InvoiceGenerated = InvoiceGenerated;

                        clientView.TotalMinutes = new TimeSpan(0, Convert.ToInt32(client.TotalMinutes), 0);
                        totalChar.TotalMinutes += clientView.TotalMinutes;

                        if (clientView.PaymentType == "Per Unit")
                        {

                            clientView.PaymentAmount = Convert.ToDecimal(client.PaymentAmount * client.ActualLineCount);
                        }
                        else if (clientView.PaymentType == "Fixed")
                        {

                            clientView.PaymentAmount = Convert.ToDecimal(client.PaymentAmount);
                        }
                        clients.Add(clientView);

                    }
                    return Content(HttpStatusCode.OK, new { clients, totalCount, totalChar.TotalCharCount_60, totalChar.TotalCharCount_55, totalChar.TotalCharCount_65, totalChar.TotalMinutes });
                }
            }
            catch (Exception ex)
            {
                new Error().logAPIError(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString(), ex.StackTrace);
                return Content(HttpStatusCode.InternalServerError, "An error occoured, please try again!");
            }
        }
        
    }
}