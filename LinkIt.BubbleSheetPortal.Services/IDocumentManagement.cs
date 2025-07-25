using System;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement;

namespace LinkIt.BubbleSheetPortal.Services
{
    public interface IDocumentManagement
    {
        DocumentDownloadDto DownloadDocument(Guid documentGuid);
        DocumentInforDto GetDocumentInfor(Guid documentGuid);
        bool DeleteDocument(Guid documentGuid, int userId);
        DocumentInforDto CreateDocumentInfo(CreateDocumentInfoDto request);
        PresignedLinkResponseDto CreatePresignedLinkAsync(UploadRequestDto dto, int userId, string folderDistrictId);
        string GetPresignedLinkAsync(Guid documentGuid);
        bool AliveConfirm(AliveConfirmDto dto);
        bool CancelUploadMultiPart(CancelUploadDto dto);
        bool UpdatePathEtags(UpdatePathEtagsDto dto);
        DocumentInforDto CreateDocument(CreateDocumentDto dto);
        List<DocumentInforDto> GetDocumentInfoList(IEnumerable<Guid?> documentIds);
        bool DeleteDocuments(List<Guid?> documentGuids);
    }
}
