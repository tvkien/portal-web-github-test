using System;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Models.Interfaces
{
    public interface IRosterUploadService
    {
        void UploadFile(HttpPostedFileBase postedFile, Request request, string filePath);
        void UploadRosterFile(HttpPostedFileBase postedFile, Request request, string destinationFolder);
        void MoveRosterFileBackToUploadedFolder(int requestId, string importedFileName, string uploadFilePath,
            string completedFilePath, Action<string> logErrorAction);

        void UploadTestDataFile(HttpPostedFileBase postedFile, Request request, string filePath);
    }
}
