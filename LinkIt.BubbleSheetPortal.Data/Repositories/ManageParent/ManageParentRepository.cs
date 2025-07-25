using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.ManageParent
{
    public class ManageParentRepository : IManageParentRepository
    {
        private readonly ParentDataContext _parentDbContext;
        private readonly StudentDataContext _studentDataContext;

        public ManageParentRepository(IConnectionString connectionString)
        {
            this._parentDbContext = ParentDataContext.Get(connectionString.GetLinkItConnectionString());
            this._studentDataContext = StudentDataContext.Get(connectionString.GetLinkItConnectionString());
        }

        public IEnumerable<ManageParentGetChildrenListResult> GetChildrenList(GetChildrenListRequestModel criteria, int userId, int roleId, int? districtId)
        {
            return _parentDbContext.ManageParentGetChildrenList(userId, roleId, districtId, criteria.ParentUserId, criteria.sSearch, criteria.studentIdsThatBeAddedOnCommit, criteria.iDisplayStart, criteria.iDisplayLength, criteria.SortColumns).ToArray();
        }

        public IEnumerable<GetParentListResult> GetParentList(FilterParentRequestModel filterParentRequestModel)
        {
            DateTime dtDateTimeUTC = new DateTime(1900, 1, 1); //Default LastLoginDate
            if (filterParentRequestModel.DateTimeUTC.Year > 1753) //Limit SQL Server
            {
                dtDateTimeUTC = filterParentRequestModel.DateTimeUTC;
            }
            return _parentDbContext.GetParentList(filterParentRequestModel.DistrictId, filterParentRequestModel.UserId, filterParentRequestModel.RoleId, filterParentRequestModel.ParentName,
                                                  filterParentRequestModel.StudentName, filterParentRequestModel.SchoolId, filterParentRequestModel.GradeId, filterParentRequestModel.ShowInactiveParent,
                                                  filterParentRequestModel.sSearch, filterParentRequestModel.iDisplayStart, filterParentRequestModel.iDisplayLength, filterParentRequestModel.SortColumns,
                                                  filterParentRequestModel.LoginTimeFrame, filterParentRequestModel.HasRegistrationCode, dtDateTimeUTC).ToArray();
        }

        public GetParentByRegistrationCodeResult GetParentByRegistrationCode(int districtId, string registrationCode)
        {
            return _parentDbContext.GetParentByRegistrationCode(registrationCode, districtId).FirstOrDefault();
        }

        public void DisableRegistrationCode(int userId)
        {
            var userRegistrationCode = _parentDbContext.UserRegistrationCodeEntities.FirstOrDefault(x => x.UserId == userId);
            if (userRegistrationCode != null)
            {
                userRegistrationCode.Disabled = true;
                _parentDbContext.SubmitChanges();
            }
        }

        public IEnumerable<int> GetChildrenStudentIdList(int parentId)
        {
            return _parentDbContext.StudentParentEntities
                .Where(c => c.ParentID == parentId)
               .Select(c => c.StudentID).ToArray();
        }

        public BaseResponseModel<bool> UnassignStudent(int parentUserId, int studentId)
        {
            var parent = _parentDbContext.ParentEntities.FirstOrDefault(o => o.UserID == parentUserId);
            if (parent == null)
                return BaseResponseModel<bool>.InstanceError("Relationship not found");

            var currentAssignment = _parentDbContext.StudentParentEntities
                .Where(c => c.ParentID == parent.ParentID && c.StudentID == studentId)
                .FirstOrDefault();
            if (currentAssignment == null)
            {
                return BaseResponseModel<bool>.InstanceError("Relationship not found");

            }
            _parentDbContext.StudentParentEntities.DeleteOnSubmit(currentAssignment);
            _parentDbContext.SubmitChanges();
            return BaseResponseModel<bool>.InstanceSuccess(true);
        }

        public void AddStudentsToParent(int[] studentIdsSplitted, int parentId, string relationship, bool studentDataAccess)
        {
            var studentIdsThatExisting = _parentDbContext.StudentParentEntities
                .Where(c => c.ParentID == parentId && studentIdsSplitted.Contains(c.StudentID))
                .Select(c => c.StudentID).ToArray();
            if (studentIdsThatExisting.Any())
            {
                studentIdsSplitted = (from studentId in studentIdsSplitted
                                      join existing in studentIdsThatExisting
                                      on studentId equals existing
                                      into joined
                                      from j in joined.DefaultIfEmpty()
                                      select new
                                      {
                                          StudentId = studentId,
                                          Existing = j > 0
                                      }
                                       ).Where(c => !c.Existing)
                                       .Select(c => c.StudentId)
                                       .ToArray();
            }
            if (studentIdsSplitted.Any())
            {
                var assignmentNewModels = studentIdsSplitted.Select(c => new StudentParentEntity()
                {
                    ParentID = parentId,
                    StudentID = c,
                    Relationship = relationship,
                    StudentDataAccess = studentDataAccess,
                    CreatedDate = DateTime.UtcNow,
                }).ToArray();
                foreach (var assigment in assignmentNewModels)
                {
                    _parentDbContext.StudentParentEntities.InsertOnSubmit(assigment);
                }
                _parentDbContext.SubmitChanges();
            }
        }

        public bool CheckRegistrationCodeActive(int userId)
        {
            return _parentDbContext.UserRegistrationCodeEntities.Any(x => x.UserId == userId && x.Disabled);
        }

        public bool ExistsRegistrationCode(string registrationCode)
        {
            if (string.IsNullOrEmpty(registrationCode))
                return false;
            return _parentDbContext.UserRegistrationCodeEntities.Any(o => o.RegistrationCode == registrationCode);

        }
        public string GetStudentFirstNameByParentId(int parentId)
        {
            if (parentId > 0)
            {
                var listStudentFirstName = _parentDbContext.GetStudentFirtNameByParentID(parentId)
                    .Select(o => o.FirstName).ToList();
                if (listStudentFirstName.Any())
                {
                    return string.Join(", ", listStudentFirstName);
                }
            }
            return string.Empty;
        }

        public List<GetStudentByNavigatorReportAndParentResult> GetStudentByNavigatorReportAndParent(int navigatorReportId, int parentId)
        {
            return _parentDbContext.GetStudentByNavigatorReportAndParent(navigatorReportId, parentId).ToList();
        }

        public void TrackingLastSendDistributeEmail(int userId)
        {
            var userRegistrationCodeEntry = _parentDbContext
                .UserRegistrationCodeEntities
                .Where(c => c.UserId == userId)
                .FirstOrDefault();

            if (userRegistrationCodeEntry != null)
            {
                userRegistrationCodeEntry.EmailLastSent = DateTime.UtcNow;
                _parentDbContext.SubmitChanges();
            }
        }

        public GetParentsInformationForDistributeRegistrationCodeResult GetParentsInformationForDistributeRegistrationCode(int parentUserId)
        {
            return _parentDbContext.GetParentsInformationForDistributeRegistrationCode(parentUserId).FirstOrDefault();
        }

        public IEnumerable<int> GetAccessibleUserIds(int userId, int roleId, int? districtId)
        {
            return _parentDbContext.GetAccessibleParentUserIds(userId, roleId, districtId)
                .Select(c => c.UserId)
                .Where(c => c.HasValue)
                .Select(c => c.Value)
                .ToArray();
        }

        public void UpdateStudentParents(List<StudentParentCustom> studentParentCustoms)
        {
            if (studentParentCustoms != null && studentParentCustoms.Count > 0)
            {
                foreach (var studentParent in studentParentCustoms)
                {
                    var currentStudentParent = _parentDbContext.StudentParentEntities.FirstOrDefault(o => o.ParentID == studentParent.ParentId && o.StudentID == studentParent.StudentId);
                    if (currentStudentParent != null)
                    {
                        currentStudentParent.Relationship = studentParent.Relationship;
                        currentStudentParent.StudentDataAccess = studentParent.StudentDataAccess;
                    }
                    else if (studentParent.StudentId > 0 && studentParent.ParentId > 0)
                    {
                        currentStudentParent = new StudentParentEntity
                        {
                            ParentID = studentParent.ParentId,
                            StudentID = studentParent.StudentId,
                            Relationship = studentParent.Relationship,
                            StudentDataAccess = studentParent.StudentDataAccess,
                            CreatedDate = DateTime.UtcNow
                        };
                        _parentDbContext.StudentParentEntities.InsertOnSubmit(currentStudentParent);
                    }
                }
                _parentDbContext.SubmitChanges();
            }
        }
    }
}
