using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DownloadPdfService
    {
        private readonly IRepository<DownloadPdfData> _downloadPdfDataRepository;
        public DownloadPdfService(IRepository<DownloadPdfData> downloadPdfDataRepository)
        {
            _downloadPdfDataRepository = downloadPdfDataRepository;
        }

        public bool CanDownload(DownloadPdfData downloadPDF, int userID)
        {
            if (downloadPDF == null) return false;
            return downloadPDF.UserID == userID;
        }

        public DownloadPdfData GetDownloadPdf(Guid fileID)
        {
            var result = _downloadPdfDataRepository.Select().FirstOrDefault(o => o.DownloadPdfID == fileID);
            return result;
        }

        public void SaveDownloadPdfData(DownloadPdfData data)
        {
            _downloadPdfDataRepository.Save(data);
        }
    }
}
