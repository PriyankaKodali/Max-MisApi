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
    public class PaymentController : ApiController
    {
        public IHttpActionResult GetClientsCurrency(int ClientId)
        {
            try
            {
                using (MaxDbEntities db = new MaxDbEntities())
                {
                    var client = db.MaxClientGet(ClientId).FirstOrDefault();
                    return Content(HttpStatusCode.OK, new { client });
                }
            }
            catch (Exception ex)
            {
                new Error().logAPIError(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString(), ex.StackTrace);
                return InternalServerError();
            }
        }
        public IHttpActionResult GetPaymentDetails(int client_id)
        {
            try
            {
                using (MaxDbEntities db = new MaxDbEntities())
                {
                    var paymentDetails = db.ClientPayments(client_id).ToList();
                    return Content(HttpStatusCode.OK, new { paymentDetails });
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