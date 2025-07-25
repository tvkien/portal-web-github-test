using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AnswerAttachmentRepository: IAnswerAttachmentRepository
    {
        private readonly TestDataContext _testDataContext;
        private readonly Table<AnswerAttachment> _answerAttachmentTable;
        private readonly Table<QTIOnlineTestSessionAnswerAttachment> _qtiAnswerAttachmentTable;
        private readonly Table<AnswerEntity> _answerTable;

        public AnswerAttachmentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testDataContext = TestDataContext.Get(connectionString);
            var studentDataContext = StudentDataContext.Get(connectionString);
            _answerAttachmentTable = studentDataContext.GetTable<AnswerAttachment>();
            _qtiAnswerAttachmentTable = studentDataContext.GetTable<QTIOnlineTestSessionAnswerAttachment>();
            _answerTable = studentDataContext.GetTable<AnswerEntity>();
        }

        public bool CheckUserCanAccessArtifact(int userId, RoleEnum  roleEnum, Guid documentGuid)
        {
            var result = _testDataContext.CheckUserCanAccessArtifact(userId, (int) roleEnum, documentGuid);
            return result.FirstOrDefault()?.CanAccess ?? false;
        }

        public void DeleteAnswerAttachment(Guid documentGUID)
        {
            var answerAttachment = _answerAttachmentTable.FirstOrDefault(x => x.DocumentGUID == documentGUID);

            if (answerAttachment != null)
            {
                _answerAttachmentTable.DeleteOnSubmit(answerAttachment);
                _answerAttachmentTable.Context.SubmitChanges();
            }
        }

        public void DeleteQTIAnswerAttachment(Guid documentGUID)
        {
            var qtiAnswerAttachment = _qtiAnswerAttachmentTable.FirstOrDefault(x => x.DocumentGUID == documentGUID);

            if (qtiAnswerAttachment != null)
            {
                _qtiAnswerAttachmentTable.DeleteOnSubmit(qtiAnswerAttachment);
                _qtiAnswerAttachmentTable.Context.SubmitChanges();
            }
        }

        public void AddOrUpdateTeacherAttachment(AnswerAttachmentDto teacherAttachmentDto)
        {
            if(teacherAttachmentDto.AnswerID > 0)
            {
                var answerAttachment = _answerAttachmentTable.FirstOrDefault(x =>
                                                x.AnswerID == teacherAttachmentDto.AnswerID
                                             && x.AttachmentType == teacherAttachmentDto.AttachmentType);

                if(answerAttachment != null)
                {
                    answerAttachment.DocumentGUID = teacherAttachmentDto.DocumentGuid;
                    answerAttachment.FileName = teacherAttachmentDto.FileName;
                    answerAttachment.FilePath = teacherAttachmentDto.FilePath;
                    answerAttachment.FileSize = teacherAttachmentDto.FileSize;
                    answerAttachment.FileType = teacherAttachmentDto.FileType;
                }
                else
                {
                    answerAttachment = new AnswerAttachment
                    {
                        AnswerID = teacherAttachmentDto.AnswerID,
                        DocumentGUID = teacherAttachmentDto.DocumentGuid,
                        FileName = teacherAttachmentDto.FileName,
                        FilePath = teacherAttachmentDto.FilePath,
                        FileSize = teacherAttachmentDto.FileSize,
                        FileType = teacherAttachmentDto.FileType,
                        AttachmentType = teacherAttachmentDto.AttachmentType
                    };
                    _answerAttachmentTable.InsertOnSubmit(answerAttachment);
                }
                _answerAttachmentTable.Context.SubmitChanges();
                return;
            }

            if (teacherAttachmentDto.QTIOnlineTestSessionAnswerID > 0)
            {
                var qtiAnswerAttachment = _qtiAnswerAttachmentTable.FirstOrDefault(x =>
                                                x.QTIOnlineTestSessionAnswerID == teacherAttachmentDto.QTIOnlineTestSessionAnswerID
                                             && x.AttachmentType == teacherAttachmentDto.AttachmentType);

                if (qtiAnswerAttachment != null)
                {
                    qtiAnswerAttachment.DocumentGUID = teacherAttachmentDto.DocumentGuid;
                    qtiAnswerAttachment.FileName = teacherAttachmentDto.FileName;
                    qtiAnswerAttachment.FilePath = teacherAttachmentDto.FilePath;
                    qtiAnswerAttachment.FileSize = teacherAttachmentDto.FileSize;
                    qtiAnswerAttachment.FileType = teacherAttachmentDto.FileType;
                }
                else
                {
                    qtiAnswerAttachment = new QTIOnlineTestSessionAnswerAttachment
                    {
                        QTIOnlineTestSessionAnswerID = teacherAttachmentDto.QTIOnlineTestSessionAnswerID,
                        DocumentGUID = teacherAttachmentDto.DocumentGuid,
                        FileName = teacherAttachmentDto.FileName,
                        FilePath = teacherAttachmentDto.FilePath,
                        FileSize = teacherAttachmentDto.FileSize,
                        FileType = teacherAttachmentDto.FileType,
                        AttachmentType = teacherAttachmentDto.AttachmentType
                    };
                    _qtiAnswerAttachmentTable.InsertOnSubmit(qtiAnswerAttachment);
                }
                _qtiAnswerAttachmentTable.Context.SubmitChanges();
            }
        }

        public List<Guid?> GetDocumentGuids(IEnumerable<int> testResultIDs)
        {
            var result = from ans in _answerTable
                         join aa in _answerAttachmentTable on ans.AnswerID equals aa.AnswerID
                         where testResultIDs.Contains(ans.TestResultID)
                         select aa.DocumentGUID;

            return result.ToList();
        }
    }
}
