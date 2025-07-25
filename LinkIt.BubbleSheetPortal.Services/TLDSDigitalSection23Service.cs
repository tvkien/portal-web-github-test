using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using LinkIt.BubbleSheetPortal.Data.Repositories.TLDS;
using System.Globalization;
using LinkIt.BubbleSheetPortal.Models.Old.TLDS;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TLDSDigitalSection23Service
    {
        private readonly ITLDSProfileLinkRepository _tldsProfileLinkRepository;
        private readonly ITLDSProfileRepository _tldsProfileRepository;
        private readonly ITLDSFormSection2Repository _tldsFormSection2Repository;
        private readonly ITLDSFormSection3Repository _tldsFormSection3Repository;
        private readonly IReadOnlyRepository<Configuration> _configurationRepository;

        public TLDSDigitalSection23Service(ITLDSProfileLinkRepository tldsProfileLinkRepository,
            ITLDSProfileRepository tldsProfileRepository,
            ITLDSFormSection2Repository tldsFormSection2Repository,
            ITLDSFormSection3Repository tldsFormSection3Repository,
            IReadOnlyRepository<Configuration> configurationRepository)
        {
            this._tldsProfileLinkRepository = tldsProfileLinkRepository;
            this._tldsProfileRepository = tldsProfileRepository;
            this._tldsFormSection2Repository = tldsFormSection2Repository;
            this._tldsFormSection3Repository = tldsFormSection3Repository;
            this._configurationRepository = configurationRepository;
        }

        public IQueryable<TLDSProfileLink> SelectTLDSProfileLink()
        {
            return _tldsProfileLinkRepository.Select();
        }

        public void SaveTLDSProfileLink(TLDSProfileLink profileLink)
        {
            _tldsProfileLinkRepository.Save(profileLink); //only table TLDSProfile
        }

        public TLDSProfileLink GetTLDSProfile(Guid profileLinkID)
        {
            return _tldsProfileLinkRepository.Select().FirstOrDefault(x => x.TLDSProfileLinkID == profileLinkID);
        }      

        public bool CheckLinkExpired(Guid profileLinkID)
        {
            var isExpired = false;
            if (!_tldsProfileLinkRepository.Select().Any(x => x.TLDSProfileLinkID == profileLinkID))
            {
                isExpired = true;
            }
            else
            {
                isExpired = _tldsProfileLinkRepository.Select().Any(x => x.TLDSProfileLinkID == profileLinkID && x.ExpiredDate.Date < DateTime.UtcNow.Date);
            }
            return isExpired;
        }

        public bool CheckLinkStatus(Guid profileLinkID)
        {
            var deactive = false;
            if (!_tldsProfileLinkRepository.Select().Any(x => x.TLDSProfileLinkID == profileLinkID))
            {
                deactive = true;
            }
            else
            {
                deactive = _tldsProfileLinkRepository.Select().Any(x => x.TLDSProfileLinkID == profileLinkID && !x.IsActive);
            }
            return deactive;
        }

        public bool LoginTLDSForm(Guid id, DateTime dateOfBirth)
        {
            var isSuccess = _tldsProfileLinkRepository.LoginTLDSForm(id, dateOfBirth);

            return isSuccess;
        }

        public TLDSProfile GetProfile(Guid id)
        {
            return _tldsProfileLinkRepository.GetTLDSProfileByTLDSProfileLinkId(id);
        }

        public TLDSFormSection2 GetFormSections2(Guid id)
        {
            var formSection2 = _tldsFormSection2Repository.Select()
                                                          .FirstOrDefault(x => x.TLDSProfileLinkID == id);

            return formSection2;
        }        

        public TLDSFormSection3 GetFormSections3(Guid id)
        {
            var formSection3 = _tldsFormSection3Repository.Select()
                                                          .FirstOrDefault(x => x.TLDSProfileLinkID == id);

            return formSection3;
        }     

        public bool UpdateTLDSProfileLink(Guid tldsProfileLinkId, bool value)
        {
            return _tldsProfileLinkRepository.UpdateTLDSProfileLink(tldsProfileLinkId, value);
        }

        public bool RefreshTLDSProfileLink(Guid tldsProfileLinkId, int day)
        {
            return _tldsProfileLinkRepository.RefreshTLDSProfileLink(tldsProfileLinkId, day);
        }

        public List<TLDSProfileLink> GetTLDSProfileLink(string scheme, int profileId, int userId, TLDSProfileLinkFilterParameter parameter)
        {
            var tldsProfileLinks = _tldsProfileLinkRepository.GetTLDSProfileLink(scheme, profileId, userId, parameter.EnrollmentYear, parameter.TldsGroupID);
            foreach (var item in tldsProfileLinks)
            {
                item.IsReadOnly = TldsProfileIsReadOnly(item.EnrolmentYear.GetValueOrDefault());
            }
            return tldsProfileLinks;
        }

        public TLDSInformationToSendMail GetTLDSInformationForSection23(Guid tldsProfileLinkId)
        {
            return _tldsProfileLinkRepository.GetTLDSInformationForSection23(tldsProfileLinkId);
        }

        public bool CheckTLDSFormSectionSubmitted(int profileId, int sectionType)
        {
            return _tldsProfileLinkRepository.CheckTLDSFormSectionSubmitted(profileId, sectionType);
        }

	    public void SaveTldsFormSection2(TLDSFormSection2 item)
        {
            _tldsFormSection2Repository.Save(item);
        }

        public bool UpdateTldsFormSection2(TLDSFormSection2 item)
        {
            return _tldsFormSection2Repository.UpdateTldsForm(item);
        }

        public bool SubmittedFormSection2(TLDSFormSection2 item)
        {
            return _tldsFormSection2Repository.SubmittedForm(item);
        }

        public void SaveTldsFormSection3(TLDSFormSection3 item)
        {
            _tldsFormSection3Repository.Save(item);
        }

        public bool UpdateTldsFormSection3(TLDSFormSection3 item)
        {
            return _tldsFormSection3Repository.UpdateTldsForm(item);
        }

        public bool SubmittedFormSection3(TLDSFormSection3 item)
        {
            return _tldsFormSection3Repository.SubmittedForm(item);
        }       

        public List<TLDSProfileLink> GetTLDSProfileLinksByProfileId(int profileId)
        {
            var tldsProfileLinks = _tldsProfileLinkRepository.Select().Where(x => x.ProfileId == profileId).ToList();
            return tldsProfileLinks;
        }

        public TLDSFormSection2 GetTLDSFormSection2ByProfileLink(Guid tldsProfileLinkID)
        {
            var tldsFormSection2 = _tldsFormSection2Repository.Select().FirstOrDefault(x => x.TLDSProfileLinkID == tldsProfileLinkID && x.IsSubmitted);
            return tldsFormSection2;
        }

        public TLDSFormSection3 GetTLDSFormSection3ByProfileLink(Guid tldsProfileLinkID)
        {
            var tldsFormSection3 = _tldsFormSection3Repository.Select().FirstOrDefault(x => x.TLDSProfileLinkID == tldsProfileLinkID && x.IsSubmitted.HasValue && x.IsSubmitted.Value);
            return tldsFormSection3;
        }

        public int UpdateLoginFail(Guid tldsProfileLinkID, int loginLimit)
        {
            return _tldsProfileLinkRepository.UpdateLoginFail(tldsProfileLinkID, loginLimit);
        }

        public void ResetLoginFailCount(Guid tldsProfileLinkID)
        {
            _tldsProfileLinkRepository.ResetLoginFailCount(tldsProfileLinkID);
        }

        public bool DeleteTldsProfileLink(Guid tldsProfileLinkId)
        {
            var tldsProfile = GetProfile(tldsProfileLinkId);
            if(tldsProfile != null)
            {
                if((tldsProfile?.Status != 20 && tldsProfile?.Status != 30) ||
                    !TldsProfileIsReadOnly(tldsProfile.EnrolmentYear.GetValueOrDefault()))
                {
                    _tldsProfileLinkRepository.DeleteTldsProfileLink(tldsProfileLinkId);
                    return true;
                }
            }
            return false;
        }

        public bool TldsProfileIsReadOnly(int enrolmentYear)
        {
            var enrolmentYearConfig = _configurationRepository.Select().FirstOrDefault(o => o.Name.Equals("TLDSReadOnly"));
            //compare enrolmentYear with current year
            if (enrolmentYear < DateTime.UtcNow.Year)
                return true;
            //compare between enrolmentYear and "TLDSReadOnly" in Configuration table
            if (enrolmentYear == DateTime.UtcNow.Year &&
                enrolmentYearConfig != null &&
                !string.IsNullOrEmpty(enrolmentYearConfig.Value))
            {
                DateTime dt = DateTime.ParseExact(enrolmentYearConfig.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (enrolmentYear <= dt.Year)
                {
                    if (DateTime.UtcNow.Month == dt.Month && DateTime.UtcNow.Day >= dt.Day)
                    {
                        return true;
                    }
                    if (DateTime.UtcNow.Month > dt.Month)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
