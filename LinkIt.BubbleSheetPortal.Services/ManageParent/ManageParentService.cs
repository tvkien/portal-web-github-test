using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories.Helper;
using LinkIt.BubbleSheetPortal.Data.Repositories.ManageParent;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Base;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Services.ManageParent
{
    public class ManageParentService : IManageParentService
    {
        private readonly IManageParentRepository _manageParentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IConnectionString _connectionString;
        private readonly IRepository<ParentDto> _parentRepository;

        public ManageParentService(IManageParentRepository manageParentRepository, IRepository<User> userRepository, IConnectionString connectionString,
            IRepository<ParentDto> parentRepository)
        {
            this._manageParentRepository = manageParentRepository;
            this._userRepository = userRepository;
            this._connectionString = connectionString;
            _parentRepository = parentRepository;
        }
        public DatatablesViewModel<ParentGridViewModel> GetParentList(FilterParentRequestModel filterParentRequestModel)
        {
            var parentFilterd = _manageParentRepository.GetParentList(filterParentRequestModel);

            return new DatatablesViewModel<ParentGridViewModel>()
            {
                TotalRecord = parentFilterd?.FirstOrDefault()?.TotalRecord ?? 0,
                Data = parentFilterd?.Select(c => new ParentGridViewModel()
                {
                    UserId = c.UserId ?? 0,
                    ParentFullName = c.ParentFullName,
                    SchoolNames = ConvertToSchoolDetail(c.SchoolNames),
                    RegistrationCode = c.RegistrationCode,
                    Active = c.Active ?? false,
                    EmailLastSent = c.EmailLastSent,
                    Email = c.Email,
                    LastLoginDate = c.LastLoginDate.Value.Year > 1900 ?  c.LastLoginDate : null
                }).ToArray()
            };
        }

        private static IEnumerable<ParentGridSchoolNameViewModel> ConvertToSchoolDetail(string schoolNames)
        {
            var schoolDefinedList = schoolNames?
                .Split(new char[] { '|' })
                .Select((val, ind) => new { val, ind }).ToArray();

            if (schoolDefinedList?.Length < 2)
            {
                return new ParentGridSchoolNameViewModel[0];
            }

            return schoolDefinedList?.Where(c => c.ind % 2 == 0)
                .Select(c => new { SchoolNames = c.val, StudentName = schoolDefinedList[c.ind + 1].val })
                .GroupBy(c => c.SchoolNames)
                .Select(c => new ParentGridSchoolNameViewModel()
                {
                    SchoolName = c.Key,
                    StudentNames = c.Select(cx => cx.StudentName).ToArray()
                });

        }

        public GenericDataTableResponseDTO<ChildrenListViewModel> GetChildrenList(GetChildrenListRequestModel criteria, int userId, int roleId, int? districtId)
        {
            var childrentList = _manageParentRepository.GetChildrenList(criteria, userId, roleId, districtId);
            return new GenericDataTableResponseDTO<ChildrenListViewModel>()
            {
                sEcho = criteria.sEcho,
                sColumns = criteria.sColumns,
                aaData = childrentList.Select(ChildrenModelToViewModel()).ToList(),
                iTotalDisplayRecords = childrentList?.FirstOrDefault()?.TotalRecord ?? 0,
                iTotalRecords = childrentList?.FirstOrDefault()?.TotalRecord ?? 0
            };
        }

        private Func<ManageParentGetChildrenListResult, ChildrenListViewModel> ChildrenModelToViewModel()
        {
            return c => new ChildrenListViewModel()
            {
                StudentFullName = c.StudentFullName,
                GradeName = c.GradeName,
                SchoolName = c.SchoolName,
                StudentId = c.StudentID ?? 0,
                StudentParentId = c.StudentParentID ?? 0,
                Relationship = c.Relationship,
                StudentDataAccess = c.StudentDataAccess.GetValueOrDefault(),
            };
        }

        public AddOrEditParentViewModelDto GetParentUserInfo(int userId)
        {
            var parentInfo = new AddOrEditParentViewModelDto();
            var parent = _parentRepository.Select().FirstOrDefault(o => o.UserID == userId);
            if (parent != null)
            {
                parentInfo.ParentId = parent.ParentID;
                parentInfo.DistrictId = parent.DistrictID;
                parentInfo.FirstName = parent.FirstName;
                parentInfo.LastName = parent.LastName;
                parentInfo.Code = parent.Code;
                parentInfo.UserId = parent.UserID;
                var userParent = _userRepository.Select().Where(c => c.Id == parent.UserID).FirstOrDefault();
                if (userParent != null)
                {
                    parentInfo.StateId = userParent.StateId ?? 0;
                    parentInfo.UserName = userParent.UserName;
                }
            }
            return parentInfo;
        }

        public bool IsParentUser(int parentUserId)
        {
            return _userRepository.Select()
                .Where(c => c.Id == parentUserId)
                .Select(c => c.RoleId == (int)Permissions.Parent)
                .FirstOrDefault();
        }

        public void UpdateParentUserInfo(UpdateParentRequestModel parentModel)
        {
            var currentUser = _userRepository.Select()
                .Where(c => c.Id == parentModel.UserId)
                .FirstOrDefault();
            if (currentUser == null)
            {
                throw new Exception("UserId is invalid!");
            }
            //Update User Infor
            currentUser.FirstName = parentModel.FirstName;
            currentUser.LastName = parentModel.LastName;
            currentUser.LocalCode = parentModel.Code;
            currentUser.UserName = parentModel.UserName;
            currentUser.EmailAddress = parentModel.UserName;
            currentUser.UserStatusId = parentModel.StudentParents?.Count == 0 ? (int)UserStatus.Inactive : (int)UserStatus.Active;
            string strPhone = parentModel.Phone;
            if (!string.IsNullOrEmpty(strPhone))
            {
                currentUser.PhoneNumber = strPhone;
            }
            _userRepository.Save(currentUser);

            var currentParent = _parentRepository.Select().FirstOrDefault(o => o.UserID == parentModel.UserId);
            if (currentParent == null)
            {
                throw new ArgumentException("Parent is invalid!");
            }
            parentModel.ParentId = currentParent.ParentID;
            //Update Parent Infor
            currentParent.FirstName = parentModel.FirstName;
            currentParent.LastName = parentModel.LastName;
            currentParent.Email = parentModel.UserName;
            currentParent.Code = parentModel.Code;
            _parentRepository.Save(currentParent);

        }

        public IEnumerable<int> GetChildrenStudentIdList(int parentId)
        {
            return _manageParentRepository.GetChildrenStudentIdList(parentId);
        }

        public void GenerateRegistrationCode(IEnumerable<int> userIds, int currentUserId, int maxCodeLength)
        {
            if (userIds.Count() <= 0)
            {
                return;
            }
            BulkHelper bulkHelper = new BulkHelper(_connectionString);
            string tempTableName = "#UserIdAndRegistrationCode";
            string tempTableCreateScript = $@"CREATE TABLE [{tempTableName}](UserId int,RegistrationCode varchar({maxCodeLength}))";
            string updateRegistrationCodeProcedureName = "UpdateParentRegistrationCode";



            var randomGuids = ServiceUtil.RandomGuidIds(maxCodeLength, userIds.Count());

            var parentUserIdAndGegistrationCode = (from user in userIds.Select((userid, index) => new { userid, index })
                                                   join guid in randomGuids.Select((guidString, index) => new { guidString, index })
                                                   on user.index equals guid.index
                                                   select new
                                                   {
                                                       UserId = user.userid,
                                                       RegistrationCode = guid.guidString
                                                   }).ToArray();



            using (DataSet dataSetDuplicateRegistrationCode = bulkHelper.BulkCopy(tempTableCreateScript, tempTableName, parentUserIdAndGegistrationCode, updateRegistrationCodeProcedureName, "@UserId", currentUserId, "@Now", DateTime.UtcNow))
            {
                if (dataSetDuplicateRegistrationCode?.Tables.Cast<DataTable>()?.FirstOrDefault() is DataTable userIdTable)
                {
                    string userIdColumnName = "DuplicateUserIdRegistrationCode";
                    if (userIdTable.Columns.Contains(userIdColumnName))
                    {
                        var userIdsNeedreGenerateRegistrationCode = userIdTable
                            .Rows
                            .Cast<DataRow>()
                            .Select(r => r[userIdColumnName])
                            .Where(c => c != null && c != DBNull.Value)
                            .Select(c =>
                            {
                                if (c is int _intVal)
                                {
                                    return _intVal;
                                }
                                int.TryParse(c.ToString(), out int _value);
                                return _value;
                            }).Where(c => c > 0)
                            .ToList();
                        if (userIdsNeedreGenerateRegistrationCode.Count == 0)
                        {
                            return;
                        }
                        GenerateRegistrationCode(userIdsNeedreGenerateRegistrationCode, currentUserId, maxCodeLength);
                    }
                }
            }
        }

        public BaseResponseModel<bool> UnassignStudent(int parentUserId, int studentId)
        {
            try
            {
                return _manageParentRepository.UnassignStudent(parentUserId, studentId);
            }
            catch (Exception ex)
            {
                return BaseResponseModel<bool>.InstanceError(ex.Message);
            }
        }

        public BaseResponseModel<bool> AddStudentsToParent(string studentIds, int parentUserId)
        {
            try
            {

                if (string.IsNullOrEmpty(studentIds))
                {
                    return BaseResponseModel<bool>.InstanceError("Please select at least on student");
                }
                if (!IsParentUser(parentUserId))
                {
                    return BaseResponseModel<bool>.InstanceError($"Invalid Parent user");
                }
                int[] studentIdsSplitted = studentIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Where(c => int.TryParse(c, out _))
                    .Select(c => int.Parse(c))
                    .ToArray();
                _manageParentRepository.AddStudentsToParent(studentIdsSplitted, parentUserId, ContaintUtil.PARENT_RELATIONSHIP_DEFAULT, ContaintUtil.PARENT_STUDENTDATAACCESS_DEFAULT);
                return BaseResponseModel<bool>.InstanceSuccess(true);
            }
            catch (Exception ex)
            {
                return BaseResponseModel<bool>.InstanceError(ex.Message);
            }
        }

        public BaseResponseModel<bool> AddStudentsToParent(string studentIds, int parentUserId, string relationship, bool studentDataAccess)
        {
            try
            {

                if (string.IsNullOrEmpty(studentIds))
                {
                    return BaseResponseModel<bool>.InstanceError("Please select at least on student");
                }
                var parent = _parentRepository.Select().FirstOrDefault(o => o.UserID == parentUserId);
                if (parent == null) //if (!IsParentUser(parentUserId))
                {
                    return BaseResponseModel<bool>.InstanceError($"Invalid Parent user");
                }
                int[] studentIdsSplitted = studentIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Where(c => int.TryParse(c, out _))
                    .Select(c => int.Parse(c))
                    .ToArray();

                _manageParentRepository.AddStudentsToParent(studentIdsSplitted, parent.ParentID, relationship, studentDataAccess);
                return BaseResponseModel<bool>.InstanceSuccess(true);
            }
            catch (Exception ex)
            {
                return BaseResponseModel<bool>.InstanceError(ex.Message);
            }
        }

        public bool ExistsRegistrationCode(string registrationCode)
        {
            return _manageParentRepository.ExistsRegistrationCode(registrationCode);
        }

        public IEnumerable<ParentListForDistributingDto> GetParentListForDistributing(FilterParentRequestModel filterParentRequestModel)
        {
            var parentFilterd = _manageParentRepository.GetParentList(filterParentRequestModel);

            return parentFilterd.Select(c => new ParentListForDistributingDto()
            {
                UserId = c.UserId ?? 0,
                ParentFullName = c.ParentFullName,
                RegistrationCode = c.RegistrationCode,
                TheUserLogedIn = c.TheUserLogedIn ?? false
            }).ToArray();
        }

        public void TrackingLastSendDistributeEmail(int userId)
        {
            _manageParentRepository.TrackingLastSendDistributeEmail(userId);
        }

        public GetParentsInformationForDistributeRegistrationCodeResult GetParentsInformationForDistributeRegistrationCode(int userId)
        {
            return _manageParentRepository.GetParentsInformationForDistributeRegistrationCode(userId);
        }

        public bool CanEditParent(int userId, int roleId, int? districtId, int parentUserId)
        {
            if (parentUserId < 1)
                return false;
            var accessibleParentUserIds = _manageParentRepository.GetAccessibleUserIds(userId, roleId, districtId);
            return accessibleParentUserIds.Contains(-1) || accessibleParentUserIds.Contains(parentUserId);
        }

        public void UpdateStudentParents(int parentId, List<StudentParentCustom> studentParents)
        {
            if (parentId > 0 && studentParents != null && studentParents.Count > 0)
            {
                foreach (var studentParent in studentParents)
                {
                    if (!string.IsNullOrEmpty(studentParent.Relationship))
                    {
                        studentParent.ParentId = parentId;
                    }
                }
                _manageParentRepository.UpdateStudentParents(studentParents);
            }
        }
    }
}
