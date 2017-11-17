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
    public class EmployeesController : ApiController
    {
        public IHttpActionResult GetAllDetails()
        {
            try
            {
                using (MaxDbEntities db = new MaxDbEntities())
                {
                    var ClientNames = (from client in db.Clients select new { label = client.ShortName, value = client.Id }).OrderBy(x => x.label).ToList();
                    var DoctorNames = (from doctor in db.Doctors select new { label = doctor.FirstName + " " + doctor.LastName, value = doctor.Id }).OrderBy(x => x.label).ToList();
                    var Employees = (from employ in db.Employees select new { label = employ.FirstName + " " + employ.LastName, value = employ.Id }).OrderBy(x => x.label).ToList();

                    return Content(HttpStatusCode.OK, new { ClientNames, DoctorNames, Employees });
                }
            }
            catch (Exception ex)
            {
                new Error().logAPIError(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString(), ex.StackTrace);
                return Content(HttpStatusCode.InternalServerError, "An error occoured, please try again!");
            }
        }

        public IHttpActionResult GetAllEmployesReport(int? EmpId, int? ClientId, int? DoctorId, string JobWorkLevel, DateTime? fromdate, DateTime? todate, int? page, int? count, string sortCol, string sortDir)
        {
            try
            {
                using (MaxDbEntities db = new MaxDbEntities())
                {
                    var MtDirect = 0;
                    var MT = 0;
                    var AQA = 0;
                    var QA = 0;

                    //TotalCharacterCountDetails totalCharacterCount = new TotalCharacterCountDetails();

                    var empReport = db.MaxEmpReport(EmpId, ClientId, DoctorId, JobWorkLevel, fromdate, todate, page, count, sortCol, sortDir).ToList();

                    foreach (var job in empReport)
                    {
                        switch (job.JobWorkLevel)
                        {
                            case "MT - Direct":
                                MtDirect += Convert.ToInt32(job.TotalCharacters);
                                break;

                            case "MT":
                                MT += Convert.ToInt32(job.TotalCharacters);
                                break;

                            case "AQA":
                                AQA += Convert.ToInt32(job.TotalCharacters);
                                break;

                            case "QA":
                                QA += Convert.ToInt32(job.TotalCharacters);
                                break;
                        }
                    }
                    int totalCount = 0;
                    if (empReport.Count > 0)
                    {
                        totalCount = (int)empReport.FirstOrDefault().TotalCount;
                    }
                    //totalCharacterCount.TotalMtDirect = MtDirect;
                    //totalCharacterCount.TotalMt = MT;
                    //totalCharacterCount.TotalAQA = AQA;
                    //totalCharacterCount.TotalQA = QA;
                    return Content(HttpStatusCode.OK, new { empReport, totalCount });
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