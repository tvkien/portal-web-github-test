using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using System.Linq;
using System;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ITLDSProfileLinkRepository : IRepository<TLDSProfileLink>
    {
        bool UpdateTLDSProfileLink(Guid tldsProfileLinkId, bool value);
        bool RefreshTLDSProfileLink(Guid tldsProfileLinkId, int day);
        List<TLDSProfileLink> GetTLDSProfileLink(string scheme, int profileId, int userId, int? enrolmentYear, int? tldsGroupID);
        bool CheckTLDSFormSectionSubmitted(int profileId, int sectionType);
        TLDSInformationToSendMail GetTLDSInformationForSection23(Guid tldsProfileLinkId);
        int UpdateLoginFail(System.Guid tldsProfileLinkId, int loginLimit);

        bool LoginTLDSForm(Guid id, DateTime dateOfBirth);

        TLDSProfile GetTLDSProfileByTLDSProfileLinkId(Guid id);

        void ResetLoginFailCount(Guid tldsProfileLinkID);

        void DeleteTldsProfileLink(Guid tldsProfileLinkId);
    }
}
