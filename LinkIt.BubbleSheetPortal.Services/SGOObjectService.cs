using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using System.Web.Script.Serialization;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGOObjectService
    {
        private readonly ISGORepository _repository;

        public SGOObjectService(ISGORepository repository)
        {
            _repository = repository;
        }

        public SGOObject Save(SGOObject obj)
        {
            if (obj != null)
            {
                _repository.Save(obj);
            }
            return obj;
        }

        public bool CheckOwnerSGO(int sgoId, int userId)
        {
            return _repository.Select().Any(o => o.SGOID == sgoId && o.OwnerUserID == userId);
        }

        public void SaveStudentPopulate(SGOPopulateStudentFilter objFilter)
        {
            _repository.SGOSaveStudentPopulate(objFilter);
        }

        public IQueryable<SGOObject> GetSGOByUserId(int userId)
        {
            return _repository.Select().Where(o => o.OwnerUserID == userId);
        }

        public List<ListItemExtra> GetStudentSelectedBySogId(int sgoId)
        {
            return _repository.GetStudentSelectedBySogId(sgoId);
        }

        public SGOObject GetSGOByID(int id)
        {
            return _repository.Select().FirstOrDefault(o => o.SGOID == id);
        }

        public IQueryable<SGOCustomNew> GetSGOCustom(int districtId, int userId, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns, string searchbox
            , int userRoleId, bool? isArchived, bool? isActive, int? schoolId, int? teacherId, int? reviewerId, int? districtTermId, string sgoStatusIds
            , DateTime? InstructionPeriodFrom, DateTime? InstructionPeriodTo)
        {
            return _repository.GetSGOCustom(districtId, userId, pageIndex, pageSize, ref totalRecords, sortColumns, searchbox, userRoleId
                , isArchived, isActive, schoolId, teacherId, reviewerId, districtTermId, sgoStatusIds, InstructionPeriodFrom, InstructionPeriodTo);
        }

        public SGOCustomReport GetSGOCustomById(int sgoId)
        {
            return _repository.GetSGOCustomById(sgoId);
        }

        public void SaveClassStudent(int sgoId, string studentDataPointXML)
        {
            _repository.SGOSaveStudent(sgoId, studentDataPointXML);
        }

        public IQueryable<ListItem> SGOGetDistictTerm(SGOPopulateStudentFilter obj)
        {
            return _repository.SGOGetDistictTerm(obj);
        }

        public IQueryable<ListItem> SGOGetGender(SGOPopulateStudentFilter obj)
        {
            return _repository.SGOGetGender(obj);
        }

        public IQueryable<ListItem> SGOGetRace(SGOPopulateStudentFilter obj)
        {
            return _repository.SGOGetRace(obj);
        }

        public IQueryable<ListItem> SGOGetProgram(SGOPopulateStudentFilter obj)
        {
            return _repository.SGOGetProgram(obj);
        }

        public IQueryable<ListItem> SGOGetClasses(SGOPopulateStudentFilter obj)
        {
            return _repository.SGOGetClasses(obj);
        }

        public IQueryable<ListItemExtra> SGOGetStudents(SGOPopulateStudentFilter obj)
        {
            return _repository.SGOGetStudents(obj);
        }
        public void ChangeSGOStatus(int sgoID, int approverUserID, int sgoStatusID)
        {
            var sgo = _repository.Select().FirstOrDefault(o => o.SGOID == sgoID);
            if (sgo == null) return;

            sgo.ApproverUserID = approverUserID;
            sgo.SGOStatusID = sgoStatusID;

            _repository.Save(sgo);
        }

        public void PopulateSchoolIdsAndClassIdsBySgoId(int sgoId)
        {
            _repository.PopulateSchoolIdsAndClassIdsBySgoId(sgoId);
        }

        #region Permission
        public List<SGOPermission> GetPermissionforStep(int userId, int sgoId)
        {
            var lst = new List<SGOPermission>();
            var obj = _repository.Select().FirstOrDefault(o => o.SGOID == sgoId);
            if (obj == null)
                return lst;
            for (int i = 1; i <= 8; i++)
            {
                switch (i)
                {
                    case (int)SGOStepEnum.SGOHome:
                        {
                            lst.Add(GetPermissionAccessSgoHome(userId, obj));
                        }
                        break;
                    case (int)SGOStepEnum.StudentPopulation:
                        {
                            lst.Add(GetPermissionAccessSgoDataPoint(userId, obj));
                        }
                        break;
                    case (int)SGOStepEnum.DataPoints:
                        {
                            lst.Add(GetPermissionAccessSgoStudentPopulate(userId, obj));
                        }
                        break;
                    case (int)SGOStepEnum.PreparednessGroups:
                        {
                            lst.Add(GetPermissionAccessSgoPreparednessGroup(userId, obj));
                        }
                        break;
                    case (int)SGOStepEnum.ScoringPlan:
                        {
                            lst.Add(GetPermissionAccessSgoScoringPlan(userId, obj));
                        }
                        break;
                    case (int)SGOStepEnum.AdminReview:
                        {
                            lst.Add(GetPermissionAccessSgoAdminReview(userId, obj));
                        }
                        break;
                    case (int)SGOStepEnum.ProgressMonitoring:
                        {
                            lst.Add(GetPermissionAccessSgoProPressMonitoring(userId, obj));
                        }
                        break;
                    case (int)SGOStepEnum.FinalSignoff:
                        {
                            lst.Add(GetPermissionAccessSgoFinalSignoff(userId, obj));
                        }
                        break;
                }
            }
            return lst;
        }

        public SGOPermission GetPermissionAccessSgoHome(int userId, SGOObject sgo)
        {
            //if (sgo.OwnerUserID == userId || (sgo.ApproverUserID.HasValue && sgo.ApproverUserID.Value == userId))
            if (GetAccessPermission(sgo.DistrictID, userId, sgo.SGOID))
            {
                switch (sgo.SGOStatusID)
                {
                    case (int)SGOStatusType.Draft:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.SGOHome, Status = (int)SGOPermissionEnum.FullUpdate };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.SGOHome, Status = (int)SGOPermissionEnum.NotAvalible };
                        }
                    case (int)SGOStatusType.PreparationSubmittedForApproval:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.SGOHome, Status = (int)SGOPermissionEnum.ReadOnly };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.SGOHome, Status = (int)SGOPermissionEnum.FullUpdate };
                        }
                    case (int)SGOStatusType.PreparationApproved:

                    case (int)SGOStatusType.PreparationDenied:

                    case (int)SGOStatusType.Cancelled:

                    case (int)SGOStatusType.EvaluationSubmittedForApproval:

                    case (int)SGOStatusType.SGOApproved:

                    case (int)SGOStatusType.SGODenied:

                    case (int)SGOStatusType.TeacherAcknowledged:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.SGOHome, Status = (int)SGOPermissionEnum.ReadOnly };
                        }
                }
            }
            return new SGOPermission()
            {
                Status = (int)SGOPermissionEnum.NotAvalible,
                Step = (int)SGOStepEnum.SGOHome
            };
        }

        public SGOPermission GetPermissionAccessAuditTrail(int userId, SGOObject sgo)
        {
            //if (sgo.OwnerUserID == userId || (sgo.ApproverUserID.HasValue && sgo.ApproverUserID.Value == userId))
            if (GetAccessPermission(sgo.DistrictID, userId, sgo.SGOID))
            {
                switch (sgo.SGOStatusID)
                {
                    case (int)SGOStatusType.Draft:
                    case (int)SGOStatusType.PreparationSubmittedForApproval:
                    case (int)SGOStatusType.PreparationApproved:
                    case (int)SGOStatusType.PreparationDenied:
                    case (int)SGOStatusType.Cancelled:
                    case (int)SGOStatusType.EvaluationSubmittedForApproval:
                    case (int)SGOStatusType.SGOApproved:
                    case (int)SGOStatusType.SGODenied:
                    case (int)SGOStatusType.TeacherAcknowledged:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.SGOHome, Status = (int)SGOPermissionEnum.ReadOnly };
                        }
                }
            }
            return new SGOPermission()
            {
                Status = (int)SGOPermissionEnum.NotAvalible,
                Step = (int)SGOStepEnum.SGOHome
            };
        }

        public SGOPermission GetPermissionAccessSgoStudentPopulate(int userId, SGOObject sgo)
        {
            //if (sgo.OwnerUserID == userId || (sgo.ApproverUserID.HasValue && sgo.ApproverUserID.Value == userId))
            if (GetAccessPermission(sgo.DistrictID, userId, sgo.SGOID))
            {
                switch (sgo.SGOStatusID)
                {
                    case (int)SGOStatusType.Draft:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.StudentPopulation, Status = (int)SGOPermissionEnum.FullUpdate };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.StudentPopulation, Status = (int)SGOPermissionEnum.NotAvalible };
                        }
                    case (int)SGOStatusType.PreparationSubmittedForApproval:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.StudentPopulation, Status = (int)SGOPermissionEnum.ReadOnly };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.StudentPopulation, Status = (int)SGOPermissionEnum.FullUpdate };
                        }
                    case (int)SGOStatusType.PreparationApproved:
                    case (int)SGOStatusType.PreparationDenied:
                    case (int)SGOStatusType.Cancelled:
                    case (int)SGOStatusType.EvaluationSubmittedForApproval:
                    case (int)SGOStatusType.SGOApproved:
                    case (int)SGOStatusType.SGODenied:
                    case (int)SGOStatusType.TeacherAcknowledged:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.StudentPopulation, Status = (int)SGOPermissionEnum.ReadOnly };
                        }
                }
            }
            return new SGOPermission()
            {
                Status = (int)SGOPermissionEnum.NotAvalible,
                Step = (int)SGOStepEnum.StudentPopulation
            };
        }
        public SGOPermission GetPermissionAccessSgoDataPoint(int userId, SGOObject sgo)
        {
            //if (sgo.OwnerUserID == userId || (sgo.ApproverUserID.HasValue && sgo.ApproverUserID.Value == userId))
            if (GetAccessPermission(sgo.DistrictID, userId, sgo.SGOID))
            {
                switch (sgo.SGOStatusID)
                {
                    case (int)SGOStatusType.Draft:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.DataPoints, Status = (int)SGOPermissionEnum.FullUpdate };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.DataPoints, Status = (int)SGOPermissionEnum.NotAvalible };
                        }
                    case (int)SGOStatusType.PreparationSubmittedForApproval:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.DataPoints, Status = (int)SGOPermissionEnum.MinorUpdate };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.DataPoints, Status = (int)SGOPermissionEnum.FullUpdate };
                        }
                    case (int)SGOStatusType.PreparationApproved:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.DataPoints, Status = (int)SGOPermissionEnum.MinorUpdate };
                        }
                    case (int)SGOStatusType.PreparationDenied:
                    case (int)SGOStatusType.Cancelled:
                    case (int)SGOStatusType.EvaluationSubmittedForApproval:
                    case (int)SGOStatusType.SGOApproved:
                    case (int)SGOStatusType.SGODenied:
                    case (int)SGOStatusType.TeacherAcknowledged:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.DataPoints, Status = (int)SGOPermissionEnum.ReadOnly };
                        }
                }
            }
            return new SGOPermission()
            {
                Status = (int)SGOPermissionEnum.NotAvalible,
                Step = (int)SGOStepEnum.DataPoints
            };
        }
        public SGOPermission GetPermissionAccessSgoPreparednessGroup(int userId, SGOObject sgo)
        {
            //if (sgo.OwnerUserID == userId || (sgo.ApproverUserID.HasValue && sgo.ApproverUserID.Value == userId))
            if (GetAccessPermission(sgo.DistrictID, userId, sgo.SGOID))
            {
                switch (sgo.SGOStatusID)
                {
                    case (int)SGOStatusType.Draft:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.PreparednessGroups, Status = (int)SGOPermissionEnum.FullUpdate };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.PreparednessGroups, Status = (int)SGOPermissionEnum.NotAvalible };
                        }
                    case (int)SGOStatusType.PreparationSubmittedForApproval:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.PreparednessGroups, Status = (int)SGOPermissionEnum.MinorUpdate };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.PreparednessGroups, Status = (int)SGOPermissionEnum.FullUpdate };
                        }
                    case (int)SGOStatusType.PreparationApproved:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.PreparednessGroups, Status = (int)SGOPermissionEnum.MinorUpdate };
                        }
                    case (int)SGOStatusType.PreparationDenied:
                    case (int)SGOStatusType.Cancelled:
                    case (int)SGOStatusType.EvaluationSubmittedForApproval:
                    case (int)SGOStatusType.SGOApproved:
                    case (int)SGOStatusType.SGODenied:
                    case (int)SGOStatusType.TeacherAcknowledged:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.PreparednessGroups, Status = (int)SGOPermissionEnum.ReadOnly };
                        }
                }
            }
            return new SGOPermission()
            {
                Status = (int)SGOPermissionEnum.NotAvalible,
                Step = (int)SGOStepEnum.PreparednessGroups
            };
        }
        public SGOPermission GetPermissionAccessSgoScoringPlan(int userId, SGOObject sgo)
        {
            //if (sgo.OwnerUserID == userId || (sgo.ApproverUserID.HasValue && sgo.ApproverUserID.Value == userId))
            if (GetAccessPermission(sgo.DistrictID, userId, sgo.SGOID))
            {
                switch (sgo.SGOStatusID)
                {
                    case (int)SGOStatusType.Draft:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.ScoringPlan, Status = (int)SGOPermissionEnum.FullUpdate };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.ScoringPlan, Status = (int)SGOPermissionEnum.NotAvalible };
                        }
                    case (int)SGOStatusType.PreparationSubmittedForApproval:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.ScoringPlan, Status = (int)SGOPermissionEnum.ReadOnly };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.ScoringPlan, Status = (int)SGOPermissionEnum.FullUpdate };
                        }
                    case (int)SGOStatusType.PreparationApproved:
                    case (int)SGOStatusType.PreparationDenied:
                    case (int)SGOStatusType.Cancelled:
                    case (int)SGOStatusType.EvaluationSubmittedForApproval:
                    case (int)SGOStatusType.SGOApproved:
                    case (int)SGOStatusType.SGODenied:
                    case (int)SGOStatusType.TeacherAcknowledged:
                        {

                            return new SGOPermission() { Step = (int)SGOStepEnum.ScoringPlan, Status = (int)SGOPermissionEnum.ReadOnly };
                        }
                }
            }
            return new SGOPermission()
            {
                Status = (int)SGOPermissionEnum.NotAvalible,
                Step = (int)SGOStepEnum.ScoringPlan
            };
        }
        public SGOPermission GetPermissionAccessSgoAdminReview(int userId, SGOObject sgo)
        {
            //if (sgo.OwnerUserID == userId || (sgo.ApproverUserID.HasValue && sgo.ApproverUserID.Value == userId))
            if (GetAccessPermission(sgo.DistrictID, userId, sgo.SGOID))
            {
                switch (sgo.SGOStatusID)
                {
                    case (int)SGOStatusType.Draft:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.AdminReview, Status = (int)SGOPermissionEnum.NotAvalible };
                        }
                    case (int)SGOStatusType.PreparationSubmittedForApproval:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.AdminReview, Status = (int)SGOPermissionEnum.ReadOnly };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.AdminReview, Status = (int)SGOPermissionEnum.FullUpdate };
                        }
                    case (int)SGOStatusType.PreparationApproved:
                    case (int)SGOStatusType.PreparationDenied:
                    case (int)SGOStatusType.Cancelled:
                    case (int)SGOStatusType.EvaluationSubmittedForApproval:
                    case (int)SGOStatusType.SGOApproved:
                    case (int)SGOStatusType.SGODenied:
                    case (int)SGOStatusType.TeacherAcknowledged:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.AdminReview, Status = (int)SGOPermissionEnum.ReadOnly };
                        }
                }
            }
            return new SGOPermission()
            {
                Status = (int)SGOPermissionEnum.NotAvalible,
                Step = (int)SGOStepEnum.AdminReview
            };
        }
        public SGOPermission GetPermissionAccessSgoProPressMonitoring(int userId, SGOObject sgo)
        {
            //if (sgo.OwnerUserID == userId || (sgo.ApproverUserID.HasValue && sgo.ApproverUserID.Value == userId))
            if (GetAccessPermission(sgo.DistrictID, userId, sgo.SGOID))
            {
                switch (sgo.SGOStatusID)
                {
                    case (int)SGOStatusType.Draft:
                    case (int)SGOStatusType.PreparationSubmittedForApproval:
                    case (int)SGOStatusType.PreparationDenied:
                    case (int)SGOStatusType.Cancelled:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.ProgressMonitoring, Status = (int)SGOPermissionEnum.NotAvalible };
                        }
                    case (int)SGOStatusType.PreparationApproved:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.ProgressMonitoring, Status = (int)SGOPermissionEnum.FullUpdate };
                        }
                    case (int)SGOStatusType.EvaluationSubmittedForApproval:
                    case (int)SGOStatusType.SGOApproved:
                    case (int)SGOStatusType.SGODenied:
                    case (int)SGOStatusType.TeacherAcknowledged:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.ProgressMonitoring, Status = (int)SGOPermissionEnum.ReadOnly };
                        }
                }
            }
            return new SGOPermission()
            {
                Status = (int)SGOPermissionEnum.NotAvalible,
                Step = (int)SGOStepEnum.ProgressMonitoring
            };

        }
        public SGOPermission GetPermissionAccessSgoFinalSignoff(int userId, SGOObject sgo)
        {
            //if (sgo.OwnerUserID == userId || (sgo.ApproverUserID.HasValue && sgo.ApproverUserID.Value == userId))
            if (GetAccessPermission(sgo.DistrictID, userId, sgo.SGOID))
            {
                switch (sgo.SGOStatusID)
                {
                    case (int)SGOStatusType.Draft:
                    case (int)SGOStatusType.PreparationSubmittedForApproval:
                    case (int)SGOStatusType.PreparationApproved:
                    case (int)SGOStatusType.PreparationDenied:
                    case (int)SGOStatusType.Cancelled:
                        {
                            return new SGOPermission() { Status = (int)SGOPermissionEnum.NotAvalible, Step = (int)SGOStepEnum.FinalSignoff };
                        }
                    case (int)SGOStatusType.EvaluationSubmittedForApproval:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.FinalSignoff, Status = (int)SGOPermissionEnum.NotAvalible };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.FinalSignoff, Status = (int)SGOPermissionEnum.FullUpdate };
                        }
                    case (int)SGOStatusType.SGOApproved:
                        {
                            if (sgo.OwnerUserID == userId)
                            {
                                return new SGOPermission() { Step = (int)SGOStepEnum.FinalSignoff, Status = (int)SGOPermissionEnum.FullUpdate };
                            }
                            return new SGOPermission() { Step = (int)SGOStepEnum.FinalSignoff, Status = (int)SGOPermissionEnum.ReadOnly };
                        }
                    case (int)SGOStatusType.SGODenied:
                    case (int)SGOStatusType.TeacherAcknowledged:
                        {
                            return new SGOPermission() { Step = (int)SGOStepEnum.FinalSignoff, Status = (int)SGOPermissionEnum.ReadOnly };
                        }
                }
            }
            return new SGOPermission()
            {
                Status = (int)SGOPermissionEnum.NotAvalible,
                Step = (int)SGOStepEnum.FinalSignoff
            };
        }
        #endregion
        public void UpdateDistrictSelected(int sgoid, int districtIdSelected)
        {
            var obj = _repository.Select().FirstOrDefault(o => o.SGOID == sgoid);
            if (obj != null)
            {
                obj.DistrictID = districtIdSelected;
                _repository.Save(obj);
            }
        }

        public List<SGOStepObject> GetListStepBySGOId(int sgoId)
        {
            return _repository.GetCompletedList(sgoId)
                .OrderBy(o => o.Step).ToList();
        }

        public int SGOAuthorizeRevision(int sgoId, int userId, int statusId)
        {
            return _repository.SGOAuthorizeRevision(sgoId, userId, statusId);
        }

        public void SGORelatedDataPoint(int oldDataPointId, int newSgoId)
        {
            _repository.SGORelatedDataPoint(oldDataPointId, newSgoId);
        }

        public List<User> SGOGetAdminsOfUser(int districtID, int userID, int roleID)
        {
            var result = _repository.SGOGetAdminsOfUser(districtID, userID, roleID);
            return result;
        }

        public List<SGOCalculateScoreResult> GetSGOCalculateScoreResult(int sgoId, int sgoDataPointId)
        {
            return _repository.GetSGOCalculateScoreResult(sgoId, sgoDataPointId);
        }

        public void UpdateGenerateResultDate(int sgoid)
        {
            var obj = _repository.Select().FirstOrDefault(o => o.SGOID == sgoid);
            if (obj != null)
            {
                obj.GenerateResultDate = DateTime.UtcNow;
                _repository.Save(obj);
            }
        }
        public List<SGODataPoint> GetDataPointHasNoBand(int sgoId)
        {
            return _repository.GetDataPointHasNoBand(sgoId);
        }

        public List<SGOScoringDetail> GetSgoScoringDetail(int sgoId, int? sgoDataPointId)
        {
            return _repository.GetSgoScoringDetail(sgoId, sgoDataPointId);
        }

        public void PopulateDefaultAttainmentGroup(int sgoId)
        {
            _repository.PopulateDefaultAttainmentGroup(sgoId);
        }

        public List<SGOReportDataPoint> GetSGOReportDataPoint(int sgoId)
        {
            return _repository.GetSGOReportDataPoint(sgoId);
        }

        public List<SGOReportDataPointFilter> GetSGOReportDataPointFilter(int sgoId)
        {
            return _repository.GetSGOReportDataPointFilter(sgoId);
        }

        public List<ListItemExtra> GetAllStudentsBySogId(int sgoId)
        {
            return _repository.GetAllStudentsBySogId(sgoId);
        }

        public bool CheckUserHasFinalAdministrativeSignoffSGO(int userId)
        {
            return _repository.SGOGetFinalAdministrativeSignoffSGO(userId,null,null,null,null, null).Any();
        }
        
        public List<SGOExportData> GetFinalAdministrativeSignoffSGOByUserId(int userId, DateTime? from, DateTime? to, bool? isArchire, bool? isActive, string sgoStatusIDs)
        {
            return _repository.SGOGetFinalAdministrativeSignoffSGO(userId,from,to,isArchire, isActive, sgoStatusIDs).ToList();
        }

        public bool GetAccessPermission(int districtId, int userId, int sgoId)
        {
            return _repository.GetAccessPermission(districtId, userId, sgoId);
        }

        public List<SGOLoggingData> GetFullDataForLogging(int sgoId)
        {
            return _repository.GetFullDataForLogging(sgoId);
        }

        public List<SGOGetCandidateClass> GetCandidateClassForReplacement(int sgoId)
        {
            return _repository.GetCandidateClassForReplacement(sgoId);
        }

        public void ApplyCandidateClassForReplacement(int sgoId, int removedClassId, int candidateClassId)
        {
            _repository.ApplyCandidateClassForReplacement(sgoId, removedClassId, candidateClassId);
        }

        public void SaveNote(int sgoID, string pageName, string sgoNote)
        {
            var sgo = _repository.Select().FirstOrDefault(o => o.SGOID == sgoID);
            if (sgo == null) return;

            var sgoNoteData = ParseSGONoteData(sgo.SGONote);
            switch (pageName)
            {
                case SGOPageNameConstant.StudentPopulation:
                    sgoNoteData.StudentPopulationNote = sgoNote;
                    break;
                case SGOPageNameConstant.DataPoint:
                    sgoNoteData.DataPointNote = sgoNote;
                    break;
                case SGOPageNameConstant.PreparenessGroup:
                    sgoNoteData.PreparenessGroupNote = sgoNote;
                    break;
                case SGOPageNameConstant.ScoringPlan:
                    sgoNoteData.ScoringPlanNote = sgoNote;
                    break;
                case SGOPageNameConstant.AdminReview:
                    sgoNoteData.AdminReviewNote = sgoNote;
                    break;
                case SGOPageNameConstant.ProgressMonitor:
                    sgoNoteData.ProgressMonitorNote = sgoNote;
                    break;
                case SGOPageNameConstant.FinalSignoff:
                    sgoNoteData.FinalSignoffNote = sgoNote;
                    break;
            }

            sgo.SGONote = new JavaScriptSerializer().Serialize(sgoNoteData);
            _repository.Save(sgo);
        }

        public string GetNote(int sgoID, string pageName)
        {
            var sgoNote = string.Empty;
            var sgo = _repository.Select().FirstOrDefault(o => o.SGOID == sgoID);

            if (sgo != null)
            {
                var sgoNoteData = ParseSGONoteData(sgo.SGONote);
                switch (pageName)
                {
                    case SGOPageNameConstant.StudentPopulation:
                        sgoNote = sgoNoteData.StudentPopulationNote;
                        break;
                    case SGOPageNameConstant.DataPoint:
                        sgoNote = sgoNoteData.DataPointNote;
                        break;
                    case SGOPageNameConstant.PreparenessGroup:
                        sgoNote = sgoNoteData.PreparenessGroupNote;
                        break;
                    case SGOPageNameConstant.ScoringPlan:
                        sgoNote = sgoNoteData.ScoringPlanNote;
                        break;
                    case SGOPageNameConstant.AdminReview:
                        sgoNote = sgoNoteData.AdminReviewNote;
                        break;
                    case SGOPageNameConstant.ProgressMonitor:
                        sgoNote = sgoNoteData.ProgressMonitorNote;
                        break;
                    case SGOPageNameConstant.FinalSignoff:
                        sgoNote = sgoNoteData.FinalSignoffNote;
                        break;
                }
            }

            return sgoNote;
        }

        private SGONoteData ParseSGONoteData(string sgoNote)
        {
            var sgoNoteData = new SGONoteData();
            if (!string.IsNullOrEmpty(sgoNote))
            {
                try
                {
                    sgoNoteData = new JavaScriptSerializer().Deserialize<SGONoteData>(sgoNote);
                }
                catch (Exception) { }
            }
            return sgoNoteData;
        }
    }
}
