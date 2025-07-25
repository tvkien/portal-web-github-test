using System;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.Configugration;
using LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview;
using LinkIt.BubbleSheetPortal.Models.DTOs.ViewAttachment;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Services
{
    public interface IAnswerAttachmentService
    {
        AttachmentDto ViewAttachment(Guid documentGuid);
        bool CheckUserCanAssessArtifact(int userId, RoleEnum role, Guid documentGuid);
        TeacherFeedbackResponse SaveTeacherFeedback(
            S3Settings s3Settings,
            TeacherFeedbackRequest model,
            HttpPostedFileBase audioFile);
        byte[] DownloadFile(S3Settings s3Settings, string filePath);
    }
}
