using System;
using System.Collections.Generic;
using System.Configuration;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement;

namespace LinkIt.BubbleSheetPortal.Services.EDM
{
    public class DocumentManagement: IDocumentManagement
    {
        private readonly string _edmUrl;
        private readonly IEDMHttpRequest _edmHttpRequest;

        public DocumentManagement(IEDMHttpRequest edmHttpRequest)
        {
            _edmHttpRequest = edmHttpRequest;
            _edmUrl = ConfigurationManager.AppSettings[Constanst.EDM.Configuration.EDM_URL];
        }

        public DocumentInforDto CreateDocumentInfo(CreateDocumentInfoDto request)
        {
            var requestUri = $"{_edmUrl}{Constanst.EDM.Endpoints.CREATE_DOCUMENT_INFO}";
            return _edmHttpRequest.SendPostRequest<DocumentInforDto>(requestUri, request);
        }

        public bool DeleteDocument(Guid documentGuid, int userId)
        {
            var requestUri = $"{_edmUrl}{Constanst.EDM.Endpoints.DELETE_DOCUMENT}/{documentGuid}/{userId}";
            return _edmHttpRequest.SendDeleteRequest<bool>(requestUri);
        }

        public DocumentDownloadDto DownloadDocument(Guid documentGuid)
        {
            string requestUri = string.Format(_edmUrl + Constanst.EDM.Endpoints.DOWNLOAD_DOCUMENT, documentGuid);
            return _edmHttpRequest.SendGetRequest<DocumentDownloadDto>(requestUri);
        }

        public DocumentInforDto GetDocumentInfor(Guid documentGuid)
        {
            string requestUri = string.Format(_edmUrl + Constanst.EDM.Endpoints.GET_DOCUMENT_INFO, documentGuid);
            return _edmHttpRequest.SendPostRequest<DocumentInforDto>(requestUri, new { includeMeta = false, includeAssociations = false });
        }

        public PresignedLinkResponseDto CreatePresignedLinkAsync(UploadRequestDto dto, int userId, string folderDistrictId)
        {
            int documentTypeId;
            if (!int.TryParse(ConfigurationManager.AppSettings[Constanst.Artifact.Configuration.DocumentTypeId], out documentTypeId))
            {
                documentTypeId = (int)DocumentTypeEnum.DataLocker;
            }

            var data = new PresignedLinkRequestDto
            {
                Author = userId,
                DistrictId = folderDistrictId,
                DocumentTypeId = documentTypeId,
                FileName = dto.FileName,
                PrevETags = dto.PrevETags,
                UploadId = dto.UploadId,
                PartNumber = dto.PartNumber,
                IsFinished = dto.IsFinished,
                IsMultiPart = dto.IsMultiPart,
                DocumentGuid = dto.DocumentGuid
            };

            string requestUri = string.Format(_edmUrl + Constanst.EDM.Endpoints.CREATE_UPLOAD_LINK);

            var result = _edmHttpRequest.SendPostRequest<PresignedLinkResponseDto>(requestUri, data);
            return result;
        }

        public string GetPresignedLinkAsync(Guid documentGuid)
        {
            string requestUri = string.Format(_edmUrl + Constanst.EDM.Endpoints.GET_DOWNLOAD_LINK, documentGuid);
            return _edmHttpRequest.SendGetRequest<string>(requestUri);
        }
        public bool AliveConfirm(AliveConfirmDto dto)
        {
            var documentId = dto.documentGuid != Guid.Empty ? dto.documentGuid.ToString() : dto.DocumentRawId.ToString();
            string requestUri = string.Format(_edmUrl + Constanst.EDM.Endpoints.ALIVE_CONFIRM_DOCUMENT, documentId);
            var result = _edmHttpRequest.SendPostRequest<bool>(requestUri, dto);

            return result;
        }

        public bool CancelUploadMultiPart(CancelUploadDto dto)
        {
            var requestUri = $"{_edmUrl}{Constanst.EDM.Endpoints.CANCEL_UPLOAD_MULTI_PART}";
            var result = _edmHttpRequest.SendPostRequest<bool>(requestUri, dto);
            return result;
        }
        public bool UpdatePathEtags(UpdatePathEtagsDto dto)
        {
            var requestUri = $"{_edmUrl}{Constanst.EDM.Endpoints.UPDATE_PATH_ETAGS}";
            var result = _edmHttpRequest.SendPostRequest<bool>(requestUri, dto);
            return result;
        }
        public DocumentInforDto CreateDocument(CreateDocumentDto dto)
        {
            dto.DocumentTypeId = (int)DocumentTypeEnum.DataLocker;
            var requestUri = $"{_edmUrl}{Constanst.EDM.Endpoints.CREATE_DOCUMENT_INFO}";
            var result = _edmHttpRequest.SendPostRequest<DocumentInforDto>(requestUri, dto);
            return result;
        }

        public List<DocumentInforDto> GetDocumentInfoList(IEnumerable<Guid?> documentIds)
        {
            var requestData = new DocumentByIdsRequestDto
            {
                DocumentIds = documentIds
            };

            var requestUri = $"{_edmUrl}{Constanst.EDM.Endpoints.GET_DOCUMENT_INFO_LIST}";
            var result = _edmHttpRequest.SendPostRequest<List<DocumentInforDto>>(requestUri, requestData);
            return result;
        }

        public bool DeleteDocuments(List<Guid?> documentGuids)
        {
            try
            {
                return _edmHttpRequest.SendDeleteRequest<bool>(_edmUrl + Constanst.EDM.Endpoints.DELETE_DOCUMENT, documentGuids);
            }
            catch
            {
                return false;
            }
        }
    }
}
