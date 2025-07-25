using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSProfileRepository : ITLDSProfileRepository
    {
        private readonly Table<TLDSProfileEntity> table;
        private readonly TLDSContextDataContext _tldsContext;
        public TLDSProfileRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _tldsContext = TLDSContextDataContext.Get(connectionString);
            table = _tldsContext.GetTable<TLDSProfileEntity>();
        }

        public IQueryable<TLDSProfile> Select()
        {
            return table.Select(x => new TLDSProfile
            {
                ProfileId = x.ProfileID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                GenderId = x.GenderID,
                DateOfBirth = x.DateOfBirth,
                PrimarySchool = x.PrimarySchool,
                OutsideSchoolHoursCareService = x.OutsideSchoolHoursCareService,
                PhotoURL = x.PhotoURL,
                ECSName = x.ECSName,
                ECSAddress = x.ECSAddress,
                ECSApprovalNumber = x.ECSApprovalNumber,
                ECSCompletingFormEducatorName = x.ECSCompletingFormEducatorName,
                ECSCompletingFormEducatorPosition = x.ECSCompletingFormEducatorPosition,
                ECSCompletingFormEducatorQualification = x.ECSCompletingFormEducatorQualification,
                ECSCompletingFormEducatorQualificationId = x.ECSCompletingFormEducatorQualificationID,
                ECSCompletingFormEducatorPhone = x.ECSCompletingFormEducatorPhone,
                ECSCompletingFormEducatorEmail = x.ECSCompletingFormEducatorEmail,
                ECSCompledDate = x.ECSCompledDate,
                TLDSInformationHasBeenSaved = x.TLDSInformationHasBeenSaved,
                ContextOfEarlyYearsSetting = x.ContextOfEarlyYearsSetting,
                SpecificInformation = x.SpecificInformation,
                ContextSpecificInforHasBeenSaved = x.ContextSpecificInforHasBeenSaved,
                SectionChildInputFileUrl = x.SectionChildInputFileUrl,
                SectionFamilyFileUrl = x.SectionFamilyFileUrl,
                SectionParentConsentFileUrl = x.SectionParentConsentFileUrl,
                Section102IsNotRequired = x.Section102IsNotRequired,
                NoKWName = x.NoKWName,
                NoKWPosition = x.NoKWPosition,
                NoKWPhone = x.NoKWPhone,
                NoKWEmail = x.NoKWEmail,
                EARReportCompleted = x.EARReportCompleted,
                EARReportDate = x.EARReportDate,
                EARAttachmentURL = x.EARAttachmentURL,
                EARAvailableUponRequest = x.EARAvailableUponRequest,
                DevelopmentOutcomeHasBeenSaved = x.DevelopmentOutcomeHasBeenSaved,
                Section102HasBeenSaved = x.Section102HasBeenSaved,
                UploadedChildFormFileName = x.UploadedChildFormFileName,
                UploadedFamilyFormFileName = x.UploadedFamilyFormFileName,
                ParentConsentIsIncluded = x.ParentConsentIsIncluded,
                SectionChildParentCompleted = x.SectionChildParentCompleted,
                PrintAllSectionsFamily = x.PrintAllSectionsFamily,
                WasAnEarlyABLESReportCompleted = x.WasAnEarlyABLESReportCompleted,
                WillAttendASchoolInVictoria = x.WillAttendASchoolInVictoria,
                HasParentSharedStatementWithSchool = x.HasParentSharedStatementWithSchool,
                SchoolNotListed = x.SchoolNotListed,
                Section2CheckedCompleted = x.Section2CheckedCompleted,
                Section3CheckedCompleted = x.Section3CheckedCompleted,
                Section4CheckedCompleted = x.Section4CheckedCompleted,
                UpcommingSchoolID = x.UpcommingSchoolID,
                Status = x.Status,
                UserID = x.UserID,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated,
                LastStatusDate = x.LastStatusDate,
                StudentID = x.StudentID,
                RejectedBy = x.RejectedBy,
                RejectedDate = x.RejectedDate,
                RejectedReason = x.RejectedReason,
                IsAboriginal = x.IsAboriginal,
                EnrolmentYear = x.EnrolmentYear,
                TldsGroupId = x.TLDSGroupID
            });
        }

        public void Save(TLDSProfile item)
        {
            var entity = table.FirstOrDefault(x => x.ProfileID.Equals(item.ProfileId));

            if (entity == null)
            {
                entity = new TLDSProfileEntity();
                table.InsertOnSubmit(entity);
            }
            entity.FirstName = item.FirstName;
            entity.LastName = item.LastName;
            entity.DateOfBirth = item.DateOfBirth;
            entity.GenderID = item.GenderId;
            entity.PrimarySchool = item.PrimarySchool;
            entity.OutsideSchoolHoursCareService = item.OutsideSchoolHoursCareService;
            entity.PhotoURL = item.PhotoURL;
            entity.ECSName = item.ECSName;
            entity.ECSAddress = item.ECSAddress;
            entity.ECSApprovalNumber = item.ECSApprovalNumber;
            entity.ECSCompletingFormEducatorName = item.ECSCompletingFormEducatorName;
            entity.ECSCompletingFormEducatorPosition = item.ECSCompletingFormEducatorPosition;
            entity.ECSCompletingFormEducatorQualification = item.ECSCompletingFormEducatorQualification;
            entity.ECSCompletingFormEducatorQualificationID = item.ECSCompletingFormEducatorQualificationId;
            entity.ECSCompletingFormEducatorPhone = item.ECSCompletingFormEducatorPhone;
            entity.ECSCompletingFormEducatorEmail = item.ECSCompletingFormEducatorEmail;
            entity.ECSCompledDate = item.ECSCompledDate;
            entity.TLDSInformationHasBeenSaved = item.TLDSInformationHasBeenSaved;
            entity.ContextOfEarlyYearsSetting = item.ContextOfEarlyYearsSetting;
            entity.SpecificInformation = item.SpecificInformation;
            entity.ContextSpecificInforHasBeenSaved = item.ContextSpecificInforHasBeenSaved;
            entity.DevelopmentOutcomeHasBeenSaved = item.DevelopmentOutcomeHasBeenSaved;
            entity.SectionChildInputFileUrl = item.SectionChildInputFileUrl;
            entity.SectionFamilyFileUrl = item.SectionFamilyFileUrl;
            entity.SectionParentConsentFileUrl = item.SectionParentConsentFileUrl;
            entity.Section102IsNotRequired = item.Section102IsNotRequired;
            entity.NoKWName = item.NoKWName;
            entity.NoKWPosition = item.NoKWPosition;
            entity.NoKWPhone = item.NoKWPhone;
            entity.NoKWEmail = item.NoKWEmail;
            entity.EARReportCompleted = item.EARReportCompleted;
            entity.EARReportDate = item.EARReportDate;
            entity.EARAttachmentURL = item.EARAttachmentURL;
            entity.EARAvailableUponRequest = item.EARAvailableUponRequest;
            entity.Section102HasBeenSaved = item.Section102HasBeenSaved;
            entity.UploadedChildFormFileName = item.UploadedChildFormFileName;
            entity.UploadedFamilyFormFileName = item.UploadedFamilyFormFileName;
            entity.UpcommingSchoolID = item.UpcommingSchoolID;
            entity.ParentConsentIsIncluded = item.ParentConsentIsIncluded;
            entity.SectionChildParentCompleted = item.SectionChildParentCompleted;
            entity.PrintAllSectionsFamily = item.PrintAllSectionsFamily;
            entity.WasAnEarlyABLESReportCompleted = item.WasAnEarlyABLESReportCompleted;
            entity.WillAttendASchoolInVictoria = item.WillAttendASchoolInVictoria;
            entity.HasParentSharedStatementWithSchool = item.HasParentSharedStatementWithSchool;
            entity.SchoolNotListed = item.SchoolNotListed;
            entity.Section2CheckedCompleted = item.Section2CheckedCompleted;
            entity.Section3CheckedCompleted = item.Section3CheckedCompleted;
            entity.Section4CheckedCompleted = item.Section4CheckedCompleted;
            entity.DateCreated = item.DateCreated;
            entity.Status = item.Status;
            entity.UserID = item.UserID;
            entity.DateUpdated = item.DateUpdated;
            entity.LastStatusDate = item.LastStatusDate;//The date of the last status
            entity.StudentID = item.StudentID;
            entity.RejectedBy = item.RejectedBy;
            entity.RejectedDate = item.RejectedDate;
            entity.RejectedReason = item.RejectedReason;
            entity.IsAboriginal = item.IsAboriginal;
            entity.EnrolmentYear = item.EnrolmentYear;
            entity.TLDSGroupID = item.TldsGroupId;
            table.Context.SubmitChanges();
            item.ProfileId = entity.ProfileID;
        }

        public void Delete(TLDSProfile item)
        {
            var entity = table.FirstOrDefault(x => x.ProfileID.Equals(item.ProfileId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public List<TLDSProfileFilterModel> FilterTLDSProfile(int currentUserId, int? districtId, int? createdUserId, int? submittedSchoolID, int? tldsProfileId, int? enrollmentYear, int? tldsGroupID)
        {
            return
                _tldsContext.TLDSGetProfiles(currentUserId, districtId, createdUserId, submittedSchoolID, tldsProfileId, enrollmentYear, tldsGroupID)
                    .Select(x => new TLDSProfileFilterModel()
                    {
                        ProfileID = x.ProfileID,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ECSCompletingFormEducatorName = x.ECSCompletingFormEducatorName,
                        Status = x.Status ?? 0,
                        LastStatusDate = x.LastStatusDate,
                        UpcommingSchoolID = x.UpcommingSchoolID,
                        SchoolName = x.NAME,
                        UserId = x.UserID,
                        Viewable = x.Viewable ?? false,
                        Updateable = x.Updateable ?? false,
                        EnrolmentYear = x.EnrolmentYear ?? 0
                    }).ToList();
        }
        public List<TLDSProfileFilterModel> GetTLDSProfilesForSchoolAdmin(int currentUserId, int? districtId, int? createdUserId, int? submittedSchoolID, int? tldsProfileId, bool? showArchived, int? enrollmentYear)
        {
            return
                _tldsContext.GetTLDSProfilesForSchoolAdmin(currentUserId, districtId, createdUserId, submittedSchoolID, tldsProfileId, showArchived, enrollmentYear)
                    .Select(x => new TLDSProfileFilterModel()
                    {
                        ProfileID = x.ProfileID,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Status = x.Status ?? 0,
                        LastStatusDate = x.LastStatusDate,
                        UpcommingSchoolID = x.UpcommingSchoolID,
                        SchoolName = x.NAME,
                        UserId = x.UserID,
                        DateOfBirth = x.DateOfBirth,
                        GenderID = x.GenderID,
                        ECSName = x.ECSName,
                        Section102IsNotRequired = x.Section102IsNotRequired,
                        ECSCompledDate = x.ECSCompledDate,
                        StudentID = x.StudentID,
                        IsUploadedStatement = x.IsUploadedStatement,
                        UploadedStatementPdfFileName = x.UploadedStatementPdfFileName,
                        EnrolmentYear = x.EnrolmentYear,
                        TldsGroupID = x.TLDSGroupID,
                        GroupName = x.GroupName,
                        StatusGroup = x.StatusGroup.GetValueOrDefault()
                    }).ToList();
        }

        public List<LookupStudent> TLDSStudentLookup(LookupStudentCustom obj, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns)
        {
            int? studentStatus = 1;
            if (obj.ShowInactiveStudent)
                studentStatus = null;
            var showAssociatedStudentOnly = false;
            if (!string.IsNullOrEmpty(obj.ShowAssociatedStudent))
            {
                if (obj.ShowAssociatedStudent.ToLower().Equals("on"))
                {
                    showAssociatedStudentOnly = true;//only 
                }
            }
            return _tldsContext.TLDSStudentLookup(obj.ClassId, obj.DistrictId, obj.UserId, obj.RoleId, obj.FirstName, obj.LastName,
                obj.Code, obj.StateCode, obj.SchoolId, obj.GradeId, obj.RaceName, obj.GenderId, obj.TLDSProfileID, studentStatus, showAssociatedStudentOnly, obj.SSearch, pageIndex, pageSize,
                ref totalRecords, sortColumns).ToList().Select(x => new LookupStudent
                {
                    Code = x.Code,
                    FirstName = x.FirstName,
                    GenderCode = x.GenderCode,
                    GradeName = x.GradeName,
                    Id = x.Id,
                    LastName = x.LastName,
                    RaceName = x.RaceName,
                    SchoolName = x.SchoolName,
                    StateCode = x.StateCode,
                    StudentId = x.StudentID,
                    //Status = x.Status,
                    AdminSchoolId = x.AdminSchoolID,
                    DistrictId = x.DistrictID,
                    TLDSProfileID = x.ProfileID,
                    AltCode = x.AltCode
                }).ToList();
        }

        public void DeleteProfile(int curentUserId, int districtId, int profileId)
        {
            _tldsContext.TLDSDeleteProfile(curentUserId, districtId, profileId);

        }
        public void RejectProfile(int currentUserId, int districtId, int profileId, string rejectedReason)
        {
            _tldsContext.TLDSRejectProfile(currentUserId, districtId, profileId, rejectedReason);

        }
        public List<ListItem> GetGradesForFilter(int currentUserId, int? districtId, int roleId)
        {
            return
                _tldsContext.TLDSStudentGetGradesForFilter(districtId, currentUserId, roleId)
                    .Select(x => new ListItem()
                    {
                        Id = x.GradeID ?? 0,
                        Name = x.NAME
                    }).ToList();
        }

        public List<TLDSProfileFilterModel> GetTLDSProfilesForAssiciateToGroup(int currentUserId, int? districtId, int? createdUserId, int? submittedSchoolID, int? tldsProfileId, int? enrollmentYear)
        {
            return
                _tldsContext.TLDSGetProfilesForAssociateToGroup(currentUserId, districtId, createdUserId, submittedSchoolID, tldsProfileId, enrollmentYear)
                    .Select(x => new TLDSProfileFilterModel()
                    {
                        ProfileID = x.ProfileID,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ECSCompletingFormEducatorName = x.ECSCompletingFormEducatorName,
                        Status = x.Status ?? 0,
                        LastStatusDate = x.LastStatusDate,
                        UpcommingSchoolID = x.UpcommingSchoolID,
                        SchoolName = x.NAME,
                        UserId = x.UserID,
                        Viewable = x.Viewable ?? false,
                        Updateable = x.Updateable ?? false,
                        EnrolmentYear = x.EnrolmentYear
                    }).ToList();
        }
    }
}
