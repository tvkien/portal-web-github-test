using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Services.DownloadPdf;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using System.Configuration;
using System.Net.Http;

namespace LinkIt.BubbleSheetPortal.Web.ServiceConsumer
{
    public class PdfGeneratorConsumer
    {
        public static string InvokePdfGeneratorService(string html, string testTitle, string folder, string userName)
        {
            using (var client = new HttpClient())
            {
                html = Util.ReplaceTagListByTagOl(html, true);
                html = Util.UpdateMathImageForPrint(html);
                testTitle = testTitle.AdjustFileNameForPDFPrinting();

                var content = new MultipartFormDataContent();
                content.Add(new StringContent(testTitle), "testname");
                content.Add(new StringContent(userName), "username");
                content.Add(new StringContent(folder), "folder");
                content.Add(new StringContent(html), "testdata");

                var url = ConfigurationManager.AppSettings["PDFGeneratorURL"];
                var response = client.PostAsync(url, content).Result;
                var pdfData = response.Content.ReadAsStringAsync().Result;

                var pdfUrl = PdfGeneratorResponseParser.GetPdfUrl(pdfData);

                //System.IO.File.WriteAllText(string.Format("H:\\LinkitDoc\\Task\\PDFDownload\\{0}.html", testTitle), html);

                return pdfUrl;
            }
        }
    }
}
