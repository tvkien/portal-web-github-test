using System;
using System.IO;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Configugration;
using LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement;
using LinkIt.BubbleSheetPortal.Models.DTOs.TeacherReview;
using LinkIt.BubbleSheetPortal.Models.DTOs.ViewAttachment;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Enums;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class AnswerAttachmentService: IAnswerAttachmentService
    {
        private readonly IDocumentManagement _documentManagement;
        private readonly IAnswerAttachmentRepository _answerAttachmentRepository;
        private readonly ItemFeedbackService _itemFeedbackService;
        private readonly IS3Service _s3Service;

        public AnswerAttachmentService(
            IDocumentManagement documentManagement,
            IAnswerAttachmentRepository answerAttachmentRepository,
            ItemFeedbackService itemFeedbackService,
            IS3Service s3Service)
        {
            _documentManagement = documentManagement;
            _answerAttachmentRepository = answerAttachmentRepository;
            _itemFeedbackService = itemFeedbackService;
            _s3Service = s3Service;
        }

        public AttachmentDto ViewAttachment(Guid documentGuid)
        {
            var document = _documentManagement.GetDocumentInfor(documentGuid);
            if (document.CanBeViewedOnline)
            {
                return AttachmentDto.CreateFileViewOnDSSever(document.FileName, BuildArtifactLink(documentGuid));
            }
            else
            {
                var downloadedFile = _documentManagement.DownloadDocument(documentGuid);
                return AttachmentDto.CreateFileDownload(fileName: document.FileName + document.FileType,
                    fileContent: downloadedFile.FileContent);
            }
        }

        public bool CheckUserCanAssessArtifact(int userId, RoleEnum role, Guid documentGuid)
        {
            return _answerAttachmentRepository.CheckUserCanAccessArtifact(userId, role, documentGuid);
        }

        private string BuildArtifactLink(Guid documentGuid)
        {
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            return string.Format(domainName + "/" + Constanst.Artifact.VIEW_ON_DS_SERVER_URL, documentGuid);
        }

        public TeacherFeedbackResponse SaveTeacherFeedback(
            S3Settings s3Settings,
            TeacherFeedbackRequest request,
            HttpPostedFileBase audioFile)
        {
            var response = new TeacherFeedbackResponse();

            if (request.FileDelete != null)
            {
                request.HasChanged = true;
                _documentManagement.DeleteDocument(request.FileDelete.DocumentGuid, request.UserID);
                DeleteTeacherFeedbackAudio(request);
            }

            if (audioFile != null && audioFile.ContentLength > 0)
            {
                request.HasChanged = true;
                response = MappingResponse(audioFile);
                var documentInfo = CreateDocumentInfoAndMeta(request, response);
                _s3Service.UploadFile(s3Settings.S3AssessmentArtifactBucketName, documentInfo.FilePath, audioFile.InputStream);
                response.FilePath = documentInfo.FilePath;
                InsertTeacherFeedbackAudio(request, response);
            }

            var itemFeedbackId = SaveItemFeedback(request, response.DocumentGuid);
            response.ItemFeedbackId = itemFeedbackId;
            response.HasChanged = request.HasChanged;
            return response;
        }

        public byte[] DownloadFile(S3Settings s3Settings, string filePath)
        {
            return _s3Service.DownloadFile(s3Settings.S3AssessmentArtifactBucketName, filePath).ReturnStream;
        }

        private DocumentInforDto CreateDocumentInfoAndMeta(TeacherFeedbackRequest request, TeacherFeedbackResponse response)
        {
            return _documentManagement.CreateDocumentInfo(new CreateDocumentInfoDto
            {
                Author = request.UserID,
                DistrictId = request.DistrictID,
                DocumentGuid = response.DocumentGuid.Value,
                DocumentName = response.FileName,
                DocumentTypeId = (int)DocumentTypeEnum.TeacherFeedback,
                Extension = Path.GetExtension(response.FileName)
            });
        }

        private TeacherFeedbackResponse MappingResponse(HttpPostedFileBase audioFile)
        {
            return new TeacherFeedbackResponse
            {
                DocumentGuid = Guid.NewGuid(),
                FileName = audioFile.FileName,
                FileSize = audioFile.ContentLength
            };
        }

        private void InsertTeacherFeedbackAudio(TeacherFeedbackRequest request, TeacherFeedbackResponse response)
        {
            _answerAttachmentRepository.AddOrUpdateTeacherAttachment(new AnswerAttachmentDto
            {
                AnswerID = request.AnswerId,
                QTIOnlineTestSessionAnswerID = request.QTIOnlineTestSessionAnswerID,
                AttachmentType = (int)AttachmentTypeEnum.TeacherFeedback,
                DocumentGuid = response.DocumentGuid,
                FileName = response.FileName,
                FilePath = response.FilePath,
                FileSize = response.FileSize,
                FileType = Path.GetExtension(response.FilePath)
            });
        }

        private void DeleteTeacherFeedbackAudio(TeacherFeedbackRequest request)
        {
            if (request.AnswerId > 0)
            {
                _answerAttachmentRepository.DeleteAnswerAttachment(request.FileDelete.DocumentGuid);
            }

            if (request.QTIOnlineTestSessionAnswerID > 0)
            {
                _answerAttachmentRepository.DeleteQTIAnswerAttachment(request.FileDelete.DocumentGuid);
            }
        }

        private int SaveItemFeedback(TeacherFeedbackRequest request, Guid? documentGUID)
        {
            var itemFeedback = new ItemFeedback();
            if (request.ItemFeedbackId > 0)
            {
                itemFeedback = _itemFeedbackService.GetItemFeedbackById(request.ItemFeedbackId);
            }
            else if(request.AnswerId > 0)
            {
                itemFeedback = _itemFeedbackService.GetFeedbackOfAnswer(request.AnswerId);
            }
            else if(request.QTIOnlineTestSessionAnswerID > 0)
            {
                itemFeedback = _itemFeedbackService.GetFeedbackOfOnlineSessionAnswer(request.QTIOnlineTestSessionAnswerID);
            }

            if (itemFeedback != null)
            {
                request.HasChanged = request.HasChanged || itemFeedback.Feedback != request.FeedbackContent;

                itemFeedback.DocumentGUID = documentGUID;
                itemFeedback.Feedback = request.FeedbackContent;
                itemFeedback.UserID = request.UserID;
                itemFeedback.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                request.HasChanged = true;

                itemFeedback = new ItemFeedback
                {
                    AnswerID = request.AnswerId,
                    QTIOnlineTestSessionAnswerID = request.QTIOnlineTestSessionAnswerID,
                    Feedback = request.FeedbackContent,
                    UserID = request.UserID,
                    UpdatedDate = DateTime.UtcNow,
                    DocumentGUID = documentGUID
                };
            }

            if (request.HasChanged)
            {
                _itemFeedbackService.Save(itemFeedback);
            }

            return itemFeedback.ItemFeedbackID;
        }
    }
}
