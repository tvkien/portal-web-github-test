using System;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IAnswerAttachmentRepository
    {
        bool CheckUserCanAccessArtifact(int userId, RoleEnum  roleEnum, Guid documentGuid);
        void AddOrUpdateTeacherAttachment(AnswerAttachmentDto teacherAttachmentDto);
        void DeleteAnswerAttachment(Guid documentGUID);
        void DeleteQTIAnswerAttachment(Guid documentGUID);
        List<Guid?> GetDocumentGuids(IEnumerable<int> testResultIDs);
    }
}
