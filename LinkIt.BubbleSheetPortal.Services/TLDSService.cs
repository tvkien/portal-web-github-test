using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using LinkIt.BubbleSheetPortal.Data.Repositories.TLDS;
using LinkIt.BubbleSheetPortal.Models.DTOs.TLDS;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TLDSService
    {
        private readonly ITLDSProfileRepository _tldsProfileRepository;
        private readonly IRepository<TLDSAdditionalInformation> _tldsAdditionalInformationRepository;
        private readonly IRepository<TLDSDevelopmentOutcome> _tldsDevelopmentOutcomeRepository;
        private readonly IRepository<TLDSDevelopmentOutcomeProfile> _tldsDevelopmentOutcomeProfileRepository;
        private readonly IRepository<TLDSOtherReportPlan> _tldsOtherReportPlanRepository;
        private readonly IRepository<TLDSProfessionalService> _tldsProfessionalServiceRepository;
        private readonly IRepository<TLDSEarlyABLESReport> _tldsEarlyABLESReportRepository;
        private readonly IRepository<TLDSParentGuardian> _tldsParentGuardianRepository;
        private readonly UserSchoolService _userSchoolService;
        private readonly IRepository<TLDSLevelQualification> _tldsLevelQualification;
        private readonly IRepository<TLDSUploadedDocument> _tldsUploadedDocumentRepository;
        private readonly IRepository<TLDSUserMeta> _tldsUserMetaRepository;
        private readonly IRepository<TLDSProfileMeta> _tldsProfileMetaRepository;
        private readonly ITLDSDownloadQueueRepository _tldsDownloadQueueRepository;
        private readonly ITLDSGroupRepository _tldsGroupRepository;
        private readonly ITLDSProfileTeacherRepository _tldsProfileTeacherRepository;

        public TLDSService(ITLDSProfileRepository tldsProfileRepository,
            IRepository<TLDSAdditionalInformation> tldsAdditionalInformationRepository,
            IRepository<TLDSDevelopmentOutcome> tldsDevelopmentOutcomeRepository,
            IRepository<TLDSDevelopmentOutcomeProfile> tldsDevelopmentOutcomeProfileRepository,
            IRepository<TLDSOtherReportPlan> tldsOtherReportPlanRepository,
            IRepository<TLDSProfessionalService> tldsProfessionalServiceRepository,
            IRepository<TLDSEarlyABLESReport> tldsEarlyABLESReportRepository,
            IRepository<TLDSParentGuardian> tldsParentGuardianRepository,
            UserSchoolService userSchoolService,
            IRepository<TLDSLevelQualification> tldsLevelQualification,
            IRepository<TLDSUploadedDocument> tldsUploadedDocumentRepository,
            IRepository<TLDSUserMeta> tldsUserMetaRepository,
            IRepository<TLDSProfileMeta> tldsProfileMetaRepository,
            ITLDSDownloadQueueRepository tldsDownloadQueueRepository,
            ITLDSGroupRepository tldsGroupRepository,
            ITLDSProfileTeacherRepository tldsProfileTeacherRepository)
        {
            this._tldsProfileRepository = tldsProfileRepository;
            _tldsAdditionalInformationRepository = tldsAdditionalInformationRepository;
            _tldsDevelopmentOutcomeRepository = tldsDevelopmentOutcomeRepository;
            _tldsDevelopmentOutcomeProfileRepository = tldsDevelopmentOutcomeProfileRepository;
            _tldsOtherReportPlanRepository = tldsOtherReportPlanRepository;
            _tldsProfessionalServiceRepository = tldsProfessionalServiceRepository;
            _tldsEarlyABLESReportRepository = tldsEarlyABLESReportRepository;
            _tldsParentGuardianRepository = tldsParentGuardianRepository;
            _userSchoolService = userSchoolService;
            _tldsLevelQualification = tldsLevelQualification;
            _tldsUploadedDocumentRepository = tldsUploadedDocumentRepository;
            _tldsUserMetaRepository = tldsUserMetaRepository;
            _tldsProfileMetaRepository = tldsProfileMetaRepository;
            _tldsDownloadQueueRepository = tldsDownloadQueueRepository;
            _tldsGroupRepository = tldsGroupRepository;
            _tldsProfileTeacherRepository = tldsProfileTeacherRepository;
        }

        public IQueryable<TLDSProfile> SelectTLDSProfile()
        {
            return _tldsProfileRepository.Select();
        }

        public void SaveTLDSProfile(TLDSProfile profile)
        {
            profile.DateUpdated = DateTime.UtcNow;
            _tldsProfileRepository.Save(profile); //only table TLDSProfile
        }

        public TLDSProfile GetTLDSProfile(int profileId)
        {
            return _tldsProfileRepository.Select().FirstOrDefault(x => x.ProfileId == profileId);
        }

        public TLDSProfile GetTLDSProfileIncludeMeta(int profileId)
        {
            var profile = _tldsProfileRepository.Select().FirstOrDefault(x => x.ProfileId == profileId);
            if (profile != null)
            {
                profile.TLDSProfileMetaes = new List<TLDSProfileMeta>();
                profile.TLDSProfileMetaes = _tldsProfileMetaRepository.Select().Where(x => x.TLDSProfileID == profileId).ToList();
                TransferTLDSProfileMetaData(profile, profile.TLDSProfileMetaes);
            }

            return profile;
        }

        public void SaveTLDSProfileMeta(int profileId, string metaName, string metaValue)
        {
            TLDSProfileMeta profileMeta = new TLDSProfileMeta() { TLDSProfileID = profileId, MetaName = metaName, MetaValue = metaValue };
            _tldsProfileMetaRepository.Save(profileMeta);
        }


        private void TransferTLDSProfileMetaData(TLDSProfile profile, List<TLDSProfileMeta> metas)
        {
            var hasTheFamilyIndicatedAboriginal = metas.FirstOrDefault(x => x.MetaName == nameof(profile.HasTheFamilyIndicatedAboriginal));
            profile.HasTheFamilyIndicatedAboriginal = hasTheFamilyIndicatedAboriginal == null ? "Unknown" : hasTheFamilyIndicatedAboriginal.MetaValue;

            var hasProvidedTransitionStatement = metas.FirstOrDefault(x => x.MetaName == nameof(profile.HasProvidedTransitionStatement));
            profile.HasProvidedTransitionStatement = hasProvidedTransitionStatement == null ? "No" : hasProvidedTransitionStatement.MetaValue;

            var isAwareTransitionChildSchoolAndOSHC = metas.FirstOrDefault(x => x.MetaName == nameof(profile.IsAwareTransitionChildSchoolAndOSHC));
            profile.IsAwareTransitionChildSchoolAndOSHC = isAwareTransitionChildSchoolAndOSHC == null ? "No" : isAwareTransitionChildSchoolAndOSHC.MetaValue;

            var isFamilyDidNotCompleteSection3 = metas.FirstOrDefault(x => x.MetaName == nameof(profile.IsFamilyDidNotCompleteSection3));
            profile.IsFamilyDidNotCompleteSection3 = isFamilyDidNotCompleteSection3 == null ? "No" : isFamilyDidNotCompleteSection3.MetaValue;

            var isFamilyDidNotCompleteSection2 = metas.FirstOrDefault(x => x.MetaName == nameof(profile.IsFamilyDidNotCompleteSection2));
            profile.IsFamilyDidNotCompleteSection2 = isFamilyDidNotCompleteSection2 == null ? "No" : isFamilyDidNotCompleteSection2.MetaValue;

            var isfamilyOptedOutTransitionStatement = metas.FirstOrDefault(x => x.MetaName == nameof(profile.IsfamilyOptedOutTransitionStatement));
            profile.IsfamilyOptedOutTransitionStatement = isfamilyOptedOutTransitionStatement == null ? "No" : isfamilyOptedOutTransitionStatement.MetaValue;
        }

        public List<TLDSProfile> GetTLDSProfiles(List<int> profileIdList)
        {
            return _tldsProfileRepository.Select().Where(x => profileIdList.Contains(x.ProfileId)).ToList();
        }
        public List<TLDSProfile> GetTLDSProfileNameOnly(List<int> profileIdList)
        {
            return _tldsProfileRepository.Select().Where(x => profileIdList.Contains(x.ProfileId)).Select(x => new TLDSProfile()
            {
                ProfileId = x.ProfileId,//Select only necessary fileds to optimize perfornamce
                FirstName = x.FirstName,
                LastName = x.LastName,
                Status = x.Status,
                TLDSProfileMetaes = GetTLDSProfileMetas(x.ProfileId)
            }).ToList();
        }
        public IQueryable<TLDSProfile> SelectTLDSProfile(int userId, int roleId)
        {
            var query = _tldsProfileRepository.Select();
            query = query.Where(x => x.UserID == userId);

            if (roleId == (int)Permissions.SchoolAdmin)
            {
                var userSchools = _userSchoolService.GetSchoolsUserHasAccessTo(userId);
                var schoolIdList = userSchools.Select(x => x.SchoolId).ToList();
                query = query.Where(x => schoolIdList.Contains(x.UpcommingSchoolID));
            }
            return query;
        }

        #region TLDSAdditionalInformation

        public IQueryable<TLDSAdditionalInformation> SelectTLDSAdditionalInformation()
        {
            return _tldsAdditionalInformationRepository.Select();
        }

        public void SaveTLDSAdditionalInformation(TLDSAdditionalInformation additionalInformation)
        {
            _tldsAdditionalInformationRepository.Save(additionalInformation);
        }

        public TLDSAdditionalInformation GetTLDSAdditionalInformation(int additionalInformationID)
        {
            return
                _tldsAdditionalInformationRepository.Select()
                    .FirstOrDefault(x => x.AdditionalInformationID == additionalInformationID);
        }

        public void DeleteTLDSAdditionalInformation(int additionalInformationID)
        {
            var item = GetTLDSAdditionalInformation(additionalInformationID);
            _tldsAdditionalInformationRepository.Delete(item);
        }

        public List<TLDSAdditionalInformation> GetTLDSAdditionalInformationOfProfile(int profileId)
        {
            return _tldsAdditionalInformationRepository.Select().Where(x => x.ProfileID == profileId).ToList();
        }

        public void SaveTLDSAdditionalInformation(int profileId,
            List<TLDSAdditionalInformation> additionalInformationList)
        {
            //get the existing first
            var existingProfessionalServiceList = GetTLDSAdditionalInformationOfProfile(profileId);
            if (existingProfessionalServiceList == null || existingProfessionalServiceList.Count == 0)
            {
                //save all
                foreach (var item in additionalInformationList)
                {
                    if (!item.IsEmpty())
                    {
                        SaveTLDSAdditionalInformation(item);
                    }
                }
            }
            else
            {
                //need to find which one has been deleted by user on form
                foreach (var item in additionalInformationList)
                {
                    if (item.IsEmpty())
                    {
                        if (item.AdditionalInformationID > 0)
                        {
                            DeleteTLDSAdditionalInformation(item.AdditionalInformationID);
                        }
                    }
                    else
                    {
                        //save
                        SaveTLDSAdditionalInformation(item);
                    }

                }
            }
        }

        public void DeleteTldsAdditionalInformations(int profileId, List<int> keepIds)
        {
            var existingProfessionalServices = _tldsAdditionalInformationRepository.Select().Where(x => x.ProfileID == profileId && !keepIds.Contains(x.AdditionalInformationID));

            foreach (var item in existingProfessionalServices)
            {
                DeleteTLDSAdditionalInformation(item.AdditionalInformationID);
            }
        }

        #endregion TLDSAdditionalInformation

        #region TLDSDevelopmentOutcome

        public IQueryable<TLDSDevelopmentOutcome> SelectTLDSDevelopmentOutcome()
        {
            return _tldsDevelopmentOutcomeRepository.Select();
        }

        public void SaveTLDSDevelopmentOutcome(TLDSDevelopmentOutcome developmentOutcome)
        {
            _tldsDevelopmentOutcomeRepository.Save(developmentOutcome);
        }

        public TLDSDevelopmentOutcome GetTLDSDevelopmentOutcome(int developmentOutcomeID)
        {
            return
                _tldsDevelopmentOutcomeRepository.Select()
                    .FirstOrDefault(x => x.DevelopmentOutcomeID == developmentOutcomeID);
        }

        #endregion TLDSDevelopmentOutcome

        #region TLDSDevelopmentOutcomeProfile

        public IQueryable<TLDSDevelopmentOutcomeProfile> SelectTLDSDevelopmentOutcomeProfile()
        {
            return _tldsDevelopmentOutcomeProfileRepository.Select();
        }

        public TLDSDevelopmentOutcomeProfile GetTLDSDevelopmentOutcomeProfile(int developmentOutcomeProfileID)
        {
            return
                _tldsDevelopmentOutcomeProfileRepository.Select()
                    .FirstOrDefault(x => x.DevelopmentOutcomeProfileID == developmentOutcomeProfileID);
        }

        public void SaveTLDSDevelopmentOutcomeProfile(TLDSDevelopmentOutcomeProfile developmentOutcomeProfile)
        {
            _tldsDevelopmentOutcomeProfileRepository.Save(developmentOutcomeProfile);
        }

        public void DeleteTLDSDevelopmentOutcomeProfile(TLDSDevelopmentOutcomeProfile developmentOutcomeProfile)
        {
            _tldsDevelopmentOutcomeProfileRepository.Delete(developmentOutcomeProfile);
        }

        public void DeleteTLDSDevelopmentOutcomeProfile(int developmentOutcomeProfileID)
        {
            var item = GetTLDSDevelopmentOutcomeProfile(developmentOutcomeProfileID);
            _tldsDevelopmentOutcomeProfileRepository.Delete(item);
        }

        public List<TLDSDevelopmentOutcomeProfile> GetDevelopmentOutcomeProfileOfProfile(int profileId)
        {
            return _tldsDevelopmentOutcomeProfileRepository.Select().Where(x => x.ProfileID == profileId).ToList();
        }

        public void SaveTLDSDevelopmentOutcomeProfile(List<TLDSDevelopmentOutcomeProfile> developmentOutcomeList)
        {
            foreach (var item in developmentOutcomeList)
            {
                SaveTLDSDevelopmentOutcomeProfile(item);
            }
        }

        #endregion TLDSDevelopmentOutcomeProfile

        #region TLDSOtherReportPlan

        public IQueryable<TLDSOtherReportPlan> SelectTLDSOtherReportPlan()
        {
            return _tldsOtherReportPlanRepository.Select();
        }

        public void SaveTLDSOtherReportPlan(TLDSOtherReportPlan otherReportPlan)
        {
            _tldsOtherReportPlanRepository.Save(otherReportPlan);
        }

        public TLDSOtherReportPlan GetTLDSOtherReportPlan(int otherReportPlanID)
        {
            return _tldsOtherReportPlanRepository.Select().FirstOrDefault(x => x.OtherReportPlanID == otherReportPlanID);
        }

        public void DeleteTLDSOtherReportPlan(int otherReportPlanID)
        {
            var item = GetTLDSOtherReportPlan(otherReportPlanID);
            _tldsOtherReportPlanRepository.Delete(item);
        }

        public List<TLDSOtherReportPlan> GetTLDSOtherReportPlanOfProfile(int profileId)
        {
            return _tldsOtherReportPlanRepository.Select().Where(x => x.ProfileID == profileId).ToList();
        }

        public void SaveTLDSOtherReportPlan(int profileId, List<TLDSOtherReportPlan> otherReportPlanList)
        {
            //get the existing first
            var existingProfessionalServiceList = GetTLDSOtherReportPlanOfProfile(profileId);
            if (existingProfessionalServiceList == null || existingProfessionalServiceList.Count == 0)
            {
                //save all
                foreach (var item in otherReportPlanList)
                {
                    if (!item.IsEmpty())
                    {
                        SaveTLDSOtherReportPlan(item);
                    }
                }
            }
            else
            {
                //need to find which one has been deleted by user on form
                foreach (var item in otherReportPlanList)
                {
                    if (item.IsEmpty())
                    {
                        if (item.OtherReportPlanID > 0)
                        {
                            DeleteTLDSOtherReportPlan(item.OtherReportPlanID);
                        }
                    }
                    else
                    {
                        //save
                        SaveTLDSOtherReportPlan(item);
                    }

                }
            }
        }

        public void DeleteTldsOtherReportPlans(int profileId, List<int> keepIds)
        {
            var existingProfessionalServices = _tldsOtherReportPlanRepository.Select().Where(x => x.ProfileID == profileId && !keepIds.Contains(x.OtherReportPlanID));
            foreach (var item in existingProfessionalServices)
            {
                DeleteTLDSOtherReportPlan(item.OtherReportPlanID);
            }
        }

        #endregion TLDSOtherReportPlan

        #region TLDSProfessionalService

        public IQueryable<TLDSProfessionalService> SelectTLDSProfessionalService()
        {
            return _tldsProfessionalServiceRepository.Select();
        }

        public void SaveTLDSProfessionalService(TLDSProfessionalService professionalService)
        {
            _tldsProfessionalServiceRepository.Save(professionalService);
        }

        public TLDSProfessionalService GetTLDSProfessionalService(int professionalServiceID)
        {
            return
                _tldsProfessionalServiceRepository.Select()
                    .FirstOrDefault(x => x.ProfessionalServiceID == professionalServiceID);
        }

        public void DeleteTLDSProfessionalService(int professionalServiceID)
        {
            var item = GetTLDSProfessionalService(professionalServiceID);
            _tldsProfessionalServiceRepository.Delete(item);
        }

        public List<TLDSProfessionalService> GetTLDSProfessionalServiceOfProfile(int profileId)
        {
            return _tldsProfessionalServiceRepository.Select().Where(x => x.ProfileID == profileId).ToList();
        }

        public void SaveTLDSProfessionalService(int profileId, List<TLDSProfessionalService> professionalServiceList)
        {
            //get the existing first
            var existingProfessionalServiceList = GetTLDSProfessionalServiceOfProfile(profileId);
            if (existingProfessionalServiceList == null || existingProfessionalServiceList.Count == 0)
            {
                //save all
                foreach (var item in professionalServiceList)
                {
                    if (!item.IsEmpty())
                    {
                        SaveTLDSProfessionalService(item);
                    }
                }
            }
            else
            {
                //need to find which one has been deleted by user on form
                foreach (var item in professionalServiceList)
                {
                    if (item.IsEmpty())
                    {
                        if (item.ProfessionalServiceID > 0)
                        {
                            DeleteTLDSProfessionalService(item.ProfessionalServiceID);
                        }
                    }
                    else
                    {
                        //save
                        SaveTLDSProfessionalService(item);
                    }

                }
            }
        }

        public void DeleteTldsProfessionalServices(int profileId, List<int> keepIds)
        {
            var existingProfessionalServices = _tldsProfessionalServiceRepository.Select().Where(x => x.ProfileID == profileId && !keepIds.Contains(x.ProfessionalServiceID));
            foreach (var item in existingProfessionalServices)
            {
                DeleteTLDSProfessionalService(item.ProfessionalServiceID);
            }
        }

        #endregion TLDSProfessionalService

        #region TLDSEarlyABLESReport

        public IQueryable<TLDSEarlyABLESReport> SelectTLDSEarlyABLESReport()
        {
            return _tldsEarlyABLESReportRepository.Select();
        }

        public void SaveTLDSEarlyABLESReport(TLDSEarlyABLESReport earlyABLESReport)
        {
            _tldsEarlyABLESReportRepository.Save(earlyABLESReport);
        }

        public TLDSEarlyABLESReport GetTLDSEarlyABLESReport(int earlyABLESReportID)
        {
            return
                _tldsEarlyABLESReportRepository.Select().FirstOrDefault(x => x.EarlyABLESReportId == earlyABLESReportID);
        }

        public void DeleteTLDSEarlyABLESReport(int earlyABLESReportId)
        {
            var item = GetTLDSEarlyABLESReport(earlyABLESReportId);
            _tldsEarlyABLESReportRepository.Delete(item);
        }

        public List<TLDSEarlyABLESReport> GetTLDSEarlyABLESReportOfProfile(int profileId)
        {
            return _tldsEarlyABLESReportRepository.Select().Where(x => x.ProfileId == profileId).ToList();
        }

        public void SaveTLDSEarlyABLESReport(int profileId, List<TLDSEarlyABLESReport> earlyABLESReportList)
        {
            //get the existing first
            var existingProfessionalServiceList = GetTLDSEarlyABLESReportOfProfile(profileId);
            if (existingProfessionalServiceList == null || existingProfessionalServiceList.Count == 0)
            {
                //save all
                foreach (var item in earlyABLESReportList)
                {
                    if (!item.IsEmpty())
                    {
                        SaveTLDSEarlyABLESReport(item);
                    }
                }
            }
            else
            {
                //need to find which one has been deleted by user on form
                foreach (var item in earlyABLESReportList)
                {
                    if (item.IsEmpty())
                    {
                        if (item.EarlyABLESReportId > 0)
                        {
                            DeleteTLDSEarlyABLESReport(item.EarlyABLESReportId);
                        }
                    }
                    else
                    {
                        //save
                        SaveTLDSEarlyABLESReport(item);
                    }

                }
            }
        }

        public void DeleteTldsEarlyABLESReports(int profileId, List<TLDSEarlyABLESReport> earlyABLESReports)
        {
            var existingProfessionalServices = _tldsEarlyABLESReportRepository.Select().Any(x => x.ProfileId == profileId);
            if (existingProfessionalServices)
            {
                foreach (var item in earlyABLESReports)
                {
                    if (item.EarlyABLESReportId > 0)
                    {
                        DeleteTLDSEarlyABLESReport(item.EarlyABLESReportId);
                    }
                }
            }
        }

        #endregion TLDSEarlyABLESReport

        #region TLDSParentGuardian

        public IQueryable<TLDSParentGuardian> SelectTLDSParentGuardian()
        {
            return _tldsParentGuardianRepository.Select();
        }

        public void SaveTLDSParentGuardian(TLDSParentGuardian parentGuardian)
        {
            _tldsParentGuardianRepository.Save(parentGuardian);
        }

        public void DeleteTLDSParentGuardian(int parentGuardianID)
        {
            var item = GetTLDSParentGuardianById(parentGuardianID);
            _tldsParentGuardianRepository.Delete(item);
        }

        public TLDSParentGuardian GetTLDSParentGuardianById(int parentGuardianID)
        {
            return _tldsParentGuardianRepository.Select()
                .FirstOrDefault(x => x.TLDSParentGuardianID == parentGuardianID);
        }

        public List<TLDSParentGuardian> GetTLDSParentGuardianOfProfile(int profileId)
        {
            return _tldsParentGuardianRepository.Select().Where(x => x.TLDSProfileID == profileId).ToList();
        }

        public void SaveParentGuardian(int profileId, List<TLDSParentGuardian> guardianContactList)
        {
            var tldsParentGuardians = guardianContactList.Where(x => x.IsValid()).ToList();

            //get the existing first
            var existingTLDSParentGuardianList = GetTLDSParentGuardianOfProfile(profileId);

            var tldsParentGuardiansToDelete = existingTLDSParentGuardianList
                .Where(x => !tldsParentGuardians.Any(y => x.TLDSParentGuardianID == y.TLDSParentGuardianID))
                .ToList();

            foreach (var item in tldsParentGuardiansToDelete)
            {
                DeleteTLDSParentGuardian(item.TLDSParentGuardianID);
            }
            foreach (var item in tldsParentGuardians)
            {
                SaveTLDSParentGuardian(item);
            }
        }

        #endregion TLDSParentGuardian

        #region TLDSUploadedDocument

        public IQueryable<TLDSUploadedDocument> GetTLDSUploadedDocumentByProfileId(int profileId)
        {
            return _tldsUploadedDocumentRepository.Select().Where(x => x.ProfileId == profileId);
        }

        public void SaveTLDSUploadedDocument(TLDSUploadedDocument entity)
        {
            _tldsUploadedDocumentRepository.Save(entity);
        }

        public void DeleteTLDSUploadedDocument(int uploadedDocumentId)
        {
            var item = GetTLDSUploadedDocumentById(uploadedDocumentId);
            _tldsUploadedDocumentRepository.Delete(item);
        }

        public TLDSUploadedDocument GetTLDSUploadedDocumentById(int uploadedDocumentId)
        {
            return _tldsUploadedDocumentRepository.Select()
                .FirstOrDefault(x => x.UploadedDocumentId == uploadedDocumentId);
        }
        #endregion TLDSUploadedDocument

        #region TLDSUserMeta        
        public TLDSUserMeta GetTLDSUserMetaByUserId(int userId)
        {
            return _tldsUserMetaRepository.Select().FirstOrDefault(x => x.UserID == userId);
        }

        public void SaveTLDSUserMeta(TLDSUserMeta userMeta)
        {
            if (userMeta.UserID == 0)
            {
                return;
            }
            _tldsUserMetaRepository.Save(userMeta);
        }
        #endregion TLDSUserMeta

        public List<TLDSProfileFilterModel> FilterTLDSProfile(int currentUserId, TLDSFilterParameter p)
        {
            return _tldsProfileRepository.FilterTLDSProfile(currentUserId, p.DistrictId, p.CreatedUserId,
                p.SubmittedSchoolID, p.TldsProfileId, p.EnrollmentYear, p.TldsGroupID);
        }

        public List<TLDSProfileFilterModel> GetTLDSProfileForAssociateToGroup(int currentUserId, TLDSFilterParameter p)
        {
            return _tldsProfileRepository.GetTLDSProfilesForAssiciateToGroup(currentUserId, p.DistrictId, p.CreatedUserId,
                p.SubmittedSchoolID, p.TldsProfileId, p.EnrollmentYear);
        }

        public List<TLDSLevelQualification> GetTldsLevelQualifications()
        {
            return _tldsLevelQualification.Select().ToList();
        }
        public List<TLDSProfileFilterModel> GetTLDSProfilesForSchoolAdmin(int currentUserId, TLDSFilterParameter p)
        {
            return _tldsProfileRepository.GetTLDSProfilesForSchoolAdmin(currentUserId, p.DistrictId, p.CreatedUserId,
                p.SubmittedSchoolID, p.TldsProfileId, p.ShowArchived, p.EnrollmentYear);
        }

        public List<LookupStudent> TLDSStudentLookup(LookupStudentCustom obj, int pageIndex, int pageSize,
            ref int? totalRecords, string sortColumns)
        {
            return _tldsProfileRepository.TLDSStudentLookup(obj, pageIndex, pageSize, ref totalRecords, sortColumns);
        }
        public void AssociateToStudent(int profileId, int studentId)
        {
            var profile = GetTLDSProfile(profileId);
            profile.StudentID = studentId;
            profile.Status = (int)TLDSProfileStatusEnum.AssociatedWithStudent;
            profile.LastStatusDate = DateTime.UtcNow;
            profile.DateUpdated = DateTime.UtcNow;
            profile.ECSCompledDate = DateTime.UtcNow;
            SaveTLDSProfile(profile);

        }
        public void RemoveAssociatedStudent(int profileId)
        {
            var profile = GetTLDSProfileIncludeMeta(profileId);
            profile.StudentID = null;
            if (profile.TLDSProfileMetaes.Any(x => x.MetaName == "IsUploadedStatement" && x.MetaValue.ToLower() == "true"))
                profile.Status = (int)TLDSProfileStatusEnum.UploadedBySchool;
            else
                profile.Status = (int)TLDSProfileStatusEnum.SubmittedToSchool;

            profile.LastStatusDate = DateTime.UtcNow;
            profile.DateUpdated = DateTime.UtcNow;
            profile.ECSCompledDate = DateTime.UtcNow;
            SaveTLDSProfile(profile);
        }

        public TLDSProfile GetProfileOfStudent(int studentId)
        {
            return _tldsProfileRepository.Select().Where(x => x.StudentID == studentId).FirstOrDefault();
        }
        public void DeleteProfile(int currentUserId, int districtId, int profileId)
        {
            _tldsProfileRepository.DeleteProfile(currentUserId, districtId, profileId);
        }
        public void RejectProfile(int currentUserId, int districtId, int profileId, string rejectedReason)
        {
            _tldsProfileRepository.RejectProfile(currentUserId, districtId, profileId, rejectedReason);
        }

        public List<ListItem> GetGradesForFilter(int currentUserId, int? districtId, int roleId)
        {
            return _tldsProfileRepository.GetGradesForFilter(currentUserId, districtId, roleId);
        }

        #region TLDSProfileMeta
        public List<TLDSProfileMeta> GetTLDSProfileMetas(int profileId)
        {
            //return _tldsProfileRepository.Select().FirstOrDefault(x => x.ProfileId == profileId);
            return _tldsProfileMetaRepository.Select().Where(x => x.TLDSProfileID == profileId).ToList();
        }

        public TLDSProfileMeta GetTLDSProfileMeta(int profileId, string metaName)
        {
            //return _tldsProfileRepository.Select().FirstOrDefault(x => x.ProfileId == profileId);
            return _tldsProfileMetaRepository.Select().FirstOrDefault(x => x.TLDSProfileID == profileId
                                                               && x.MetaName.Equals(metaName));
        }

        #endregion


        public TLDSDownloadQueue GetTLDSDownloadQueueByFileName(string fileName)
        {
            var obj = _tldsDownloadQueueRepository.GetByFileName(fileName);
            return obj;
        }

        public void TLDSDownloadQueueInsertOrUpdate(TLDSDownloadQueue model)
        {
            _tldsDownloadQueueRepository.Save(model);
        }

        public List<TLDSGroupDTO> GetAllTldsGroupByUserMetaID(int tldsUserMetaID)
        {
            return _tldsGroupRepository.GetAlllByTldsUserMetaID(tldsUserMetaID);
        }

        public List<TLDSProfileTeacherDTO> GetAllTldsProfileTeachersByUserMetaID(int tldsUserMetaID)
        {
            return _tldsProfileTeacherRepository.GetAllByUserMetaID(tldsUserMetaID);
        }

        public void SaveTldsGroup(TLDSGroupDTO item)
        {
            _tldsGroupRepository.Save(item);
        }

        public void SaveTldsProfileTeacher(TLDSProfileTeacherDTO item)
        {
            _tldsProfileTeacherRepository.Save(item);
        }

        public void AssociateToProfile(int profileId, int groupId)
        {
            var profile = GetTLDSProfile(profileId);
            profile.TldsGroupId = groupId;
            profile.LastStatusDate = DateTime.UtcNow;
            profile.DateUpdated = DateTime.UtcNow;
            SaveTLDSProfile(profile);
        }

        public void RemoveToGroup(int profileId)
        {
            var profile = GetTLDSProfile(profileId);
            profile.TldsGroupId = null;
            profile.LastStatusDate = DateTime.UtcNow;
            profile.DateUpdated = DateTime.UtcNow;
            SaveTLDSProfile(profile);
        }

        public TLDSProfileTeacherDTO GetProfileTeacherById(int tldsProfileTeacherId)
        {
            return _tldsProfileTeacherRepository.Select().FirstOrDefault(x => x.TLDSProfileTeacherID == tldsProfileTeacherId);
        }

        public bool DeactiveTldsGroup(int tldsGroupId)
        {
            return _tldsGroupRepository.DeactiveTLDSGroup(tldsGroupId);
        }

        public bool CheckUniqueGroupName(int userMetaId, string groupName)
        {
            return _tldsGroupRepository.GetAlllByTldsUserMetaID(userMetaId).Any(x => string.Equals(x.GroupName, groupName, StringComparison.OrdinalIgnoreCase));
        }

        public TLDSGroupDTO GetTldsGroupById(int tldsGroupId)
        {
            return _tldsGroupRepository.Select().FirstOrDefault(x => x.TLDSGroupID == tldsGroupId);
        }

        public bool ActiveTldsGroup(int tldsGroupId)
        {
            return _tldsGroupRepository.ActiveTLDSGroup(tldsGroupId);
        }

        public bool RemoveTeacherProfile(int teacherProfileID)
        {
            return _tldsProfileTeacherRepository.Remove(teacherProfileID);
        }
    }
}
