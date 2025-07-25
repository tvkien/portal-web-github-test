using System.Xml;

namespace LinkIt.BubbleSheetPortal.Services.DownloadPdf
{
    public class PdfGeneratorResponseParser
    {
        public static string GetPdfUrl(string response)
        {
            if (string.IsNullOrWhiteSpace(response)) return null;
            var doc = new XmlDocument();
            doc.LoadXml(response);
            var pdfUrlNodeList = doc.GetElementsByTagName("pdfurl");
            if (pdfUrlNodeList == null || pdfUrlNodeList.Count == 0) return null;

            var result = pdfUrlNodeList[0].InnerText;

            var downloadPdfPrefixUrl = System.Configuration.ConfigurationManager.AppSettings["DownloadPdfPrefixUrl"];
            if (!string.IsNullOrWhiteSpace(downloadPdfPrefixUrl) && !string.IsNullOrWhiteSpace(result))
            {
                result = result.Replace(downloadPdfPrefixUrl, string.Empty);
            }

            return result;
        }
    }
}
