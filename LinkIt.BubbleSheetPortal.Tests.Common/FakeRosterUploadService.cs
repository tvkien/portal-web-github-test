using System.Web;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Tests.Common
{
    public class FakeRosterUploadService : IRosterUploadService
    {
        public void UploadFile(HttpPostedFileBase postedFile, Request request, string filePath)
        {
        }

        public void MoveRosterFileBackToUploadedFolder(int requestId, string uploadFilePath, string completedFilePath)
        {
        }

        public void UploadTestDataFile(HttpPostedFileBase postedFile, Request request, string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}