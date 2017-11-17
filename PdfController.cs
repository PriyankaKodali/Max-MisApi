using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MaxMIS.Controllers
{
    public class PdfController : ApiController
    {

        [System.Web.Http.HttpPost]
        public HttpResponseMessage ExportPdf()
        {

            string html = HttpContext.Current.Request.Form.Get("html");

            //PdfConverter pdfConverter = new PdfConverter();
            //string url = collection["TxtUrl"];
            //byte[] pdf = pdfConverter.GetPdfBytesFromUrl(url);

            //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            //fileResult.FileDownloadName = "RenderedPage.pdf";

            //return fileResult;


            //using (MemoryStream ms = new MemoryStream())
            //{
            //    StringReader sr = new StringReader(html);
            //    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 0f);

            //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
            //    pdfDoc.Open();
            //    pdfDoc.NewPage();
            //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
            //    pdfDoc.Close();

            //    var result = new HttpResponseMessage(HttpStatusCode.OK)
            //    {
            //        Content = new ByteArrayContent(ms.ToArray())
            //    };
            //    result.Content.Headers.ContentDisposition =
            //        new ContentDispositionHeaderValue("attachment")
            //        {
            //            FileName = "123.pdf"
            //        };
            //    result.Content.Headers.ContentType =
            //        new MediaTypeHeaderValue("application/octet-stream");

            //    return result;
            //}


            //HtmlToPdf converter = new HtmlToPdf();
            //SelectPdf.PdfDocument doc = converter.ConvertHtmlString(html);
            //string pdfDir = HttpContext.Current.Server.MapPath("~/Pdfs");
            //if (!Directory.Exists(pdfDir))
            //{
            //    Directory.CreateDirectory(pdfDir);
            //}
            //doc.Save(Path.Combine(pdfDir, "Sample.pdf"));



            using (MemoryStream ms = new MemoryStream())
            {


                HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
                new HiQPdf.HtmlToPdf().ConvertHtmlToStream(html, "", ms);

                //using (FileStream fs = new FileStream(Path.Combine(pdfDir, "Sample.pdf"), FileMode.Open))
                //{
                //    fs.CopyTo(ms);
                //}



                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(ms.ToArray())
                };
                result.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "123.pdf"
                    };
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");
                return result;
            }


        }
    }
}