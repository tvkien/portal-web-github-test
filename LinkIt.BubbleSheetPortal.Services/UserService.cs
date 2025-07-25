using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Data.Repositories.ManageParent;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.LTI;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using LinkIt.BubbleSheetPortal.Models.DTOs.SSO;
using LinkIt.BubbleSheetPortal.Models.DTOs.Users;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup;
using LinkIt.BubbleSheetPortal.Models.SSO;
using LinkIt.BubbleSheetPortal.Services.CodeGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using LinkIt.BubbleSheetPortal.Data.Repositories.StudentData;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;
using System.Reflection;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class UserService
    {
        private readonly IUserRepository repository;
        private readonly IRepository<Class> _classRepository;
        private readonly IRepository<ClassUser> _classUserRepository;
        private readonly IRepository<ASPSession> aSPsessionRepository;
        private readonly IValidator<User> userValidator;
        private readonly IRepository<AuthorGroupUser> authorGroupRepository;
        private readonly IUserManageRepository userManageRepository;
        private readonly IRepository<Student> studentRepository;
        private readonly IReadOnlyRepository<District> districtRepository;
        private readonly IRepository<StudentMeta> studentMetaRepository;
        private readonly ISSODistrictGroupRepository _ssoDistrictGroupRepository;
        private readonly ISSOUserMappingRepository _ssoUserMappingRepository;
        private readonly TestCodeGenerator _testCodeGenerator;
        private readonly StudentMetaService _studentMetaService;
        public readonly DistrictDecodeService _districtDecodeService;
        public readonly ISSOInformationRepository _ssoInformationRepository;
        private readonly IManageParentRepository _manageParentRepository;
        private readonly IStudentUserRepository _studentUserRepository;
        private readonly IRepository<ParentDto> _parentRepository;
        private readonly IReadOnlyRepository<InteroperabilityDto> _interoperabilityRepository;
        private readonly IReadOnlyRepository<Role> _roleRepository;

        public UserService(
            IUserRepository repository,
            IValidator<User> userValidator,
            IRepository<AuthorGroupUser> authorGroupRepository,
            IRepository<ASPSession> aSPsessionRepository,
            IUserManageRepository userManageRepository,
            IRepository<Student> studentRepository,
            IReadOnlyRepository<District> districtRepository,
            IRepository<StudentMeta> studentMetaRepository,
            ISSODistrictGroupRepository ssoDistrictGroupRepository,
            ISSOUserMappingRepository ssoUserMappingRepository,
            TestCodeGenerator testCodeGenerator,
            StudentMetaService studentMetaService,
            DistrictDecodeService districtDecodeService,
            ISSOInformationRepository ssoInformationRepository,
            IManageParentRepository manageParentRepository,
            IStudentUserRepository studentUserRepository,
            IRepository<ParentDto> parentRepository,
            IReadOnlyRepository<InteroperabilityDto> interoperabilityRepository,
            IReadOnlyRepository<Role> roleRepository,
            IRepository<Class> classRepository,
            IRepository<ClassUser> classUserRepository)            
        {
            this.repository = repository;
            this.userValidator = userValidator;
            this.authorGroupRepository = authorGroupRepository;
            this.aSPsessionRepository = aSPsessionRepository;
            this.userManageRepository = userManageRepository;
            this.studentRepository = studentRepository;
            this.districtRepository = districtRepository;
            this.studentMetaRepository = studentMetaRepository;
            _ssoDistrictGroupRepository = ssoDistrictGroupRepository;
            _ssoUserMappingRepository = ssoUserMappingRepository;
            _testCodeGenerator = testCodeGenerator;
            _studentMetaService = studentMetaService;
            _districtDecodeService = districtDecodeService;
            _ssoInformationRepository = ssoInformationRepository;
            _manageParentRepository = manageParentRepository;
            this._studentUserRepository = studentUserRepository;
            _parentRepository = parentRepository;
            _interoperabilityRepository = interoperabilityRepository;
            _roleRepository = roleRepository;
            _classRepository = classRepository;
            _classUserRepository = classUserRepository;
        }
        public User GetUserInfoByStudentId(int id)
        {
            var userId = _studentUserRepository.GetUserIDViaStudentUser(id);
            if (userId > 0)
            {
                return repository.Select().FirstOrDefault(x => x.Id.Equals(userId));
            }
            return null;
        }
        public void SaveTokenSQL(ASPSession session)
        {
            aSPsessionRepository.Save(session);
        }

        public ASPSession GetTokenSQL(string session)
        {
            if (string.IsNullOrEmpty(session))
            {
                return null;
            }

            return aSPsessionRepository.Select().FirstOrDefault(x => x.SessionToken == session);
        }

        public User GetUserById(int id)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(id));
        }

        public IQueryable<User> GetUsersByIds(List<int> userIds)
        {
            return repository.Select().Where(x => userIds.Contains(x.Id));
        }

        public User GetUserByUsernameAndDistrict(string username, int districtId)
        {
            return repository.Select().FirstOrDefault(x => x.UserName.Equals(username) && x.DistrictId.Equals(districtId));
        }

        public User GetUserByUsernameAndDistrict(string username, int districtId, IEnumerable<int> roleIds)
        {
            return repository.Select()
                .FirstOrDefault(x => x.UserName.Equals(username) && x.DistrictId.Equals(districtId) && roleIds.Contains(x.RoleId));
        }

        public List<User> GetListUsers(string username, int districtId)
        {
            return repository.Select().Where(x => x.UserName.Equals(username) && x.DistrictId.Equals(districtId)).ToList();
        }

        public string GetSecurityQuestionByUserId(int id)
        {
            var user = GetUserById(id);
            return user.IsNull() ? string.Empty : user.PasswordQuestion;
        }

        public User GetAdminByUsername(string username)
        {
            return repository.Select().FirstOrDefault(x => x.UserName.Equals(username)
            && (x.RoleId.Equals(5) || x.RoleId.Equals(15) || x.RoleId.Equals(3)));
        }

        public User GetAdminByUsernameAndDistrict(string username, int districtId)
        {
            return repository.Select().FirstOrDefault(x => x.DistrictId == districtId
            && x.UserName.Equals(username)
            && (x.RoleId.Equals(5) || x.RoleId.Equals(15) || x.RoleId.Equals(3)));
        }

        public User DoesNotRequireDistrictToBeSelected(string username)
        {
            return repository.Select().FirstOrDefault(x => x.UserName.Equals(username) && (x.RoleId.Equals(5) || x.RoleId.Equals(15)));
        }

        public User GetSchoolAdminByUserId(int userId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(userId) && x.RoleId.Equals(8));
        }

        public IQueryable<User> GetAllPublishers()
        {
            return repository.Select().Where(x => x.RoleId == (int)Permissions.Publisher);
        }

        public IQueryable<User> Select()
        {
            return repository.Select();
        }

        public bool IsValidUser(User user, string password)
        {
            // return true;
            return !user.IsNull() && Crypto.VerifyHashedPassword(user.HashedPassword, password);
        }

        public void ResetUsersPassword(User user, string newPassword, bool updateSecret = false)
        {
            user.SetValidator(userValidator);
            user.HashedPassword = Crypto.HashPassword(newPassword);
            user.PasswordLastSetDate = DateTime.UtcNow.Date;
            SaveUser(user);

            if (user.IsStudent)
                UpdateStudentSecret(user.Id, updateSecret ? newPassword : null);
        }

        public string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }

        public bool ChangePassword(User currentUser, string providedPassword, string newPassword)
        {
            if (Crypto.VerifyHashedPassword(currentUser.HashedPassword, providedPassword))
            {
                currentUser.HashedPassword = Crypto.HashPassword(newPassword);
                currentUser.PasswordLastSetDate = DateTime.UtcNow.Date;
                currentUser.SetValidator(userValidator);
                SaveUser(currentUser);
                return true;
            }
            return false;
        }

        public bool SetPassword(User currentUser, string newPassword)
        {
            currentUser.HashedPassword = Crypto.HashPassword(newPassword);
            currentUser.PasswordLastSetDate = DateTime.UtcNow.Date;
            currentUser.SetValidator(userValidator);
            currentUser.HasTemporaryPassword = false;
            SaveUser(currentUser);

            if (currentUser.IsStudent)
                UpdateStudentSecret(currentUser.Id, null);

            return true;
        }

        public bool ChangeQuestionAnswer(User currentUser, string password, string question, string answer)
        {
            if (Crypto.VerifyHashedPassword(currentUser.HashedPassword, password))
            {
                currentUser.PasswordQuestion = question;
                currentUser.PasswordAnswer = Crypto.HashPassword(answer);
                currentUser.SetValidator(userValidator);
                SaveUser(currentUser);
                return true;
            }

            return false;
        }

        public bool IsCorrectPasswordAnswer(User user, string passwordAnswer)
        {
            return Crypto.VerifyHashedPassword(user.PasswordAnswer, passwordAnswer);
        }

        public void SaveUser(User user)
        {
            if (user.IsNull())
            {
                throw new ArgumentNullException();
            }
            if (user.Id > 0 && user.UserStatusId == (int)UserStatus.Active)
            {
                user.DateConfirmedActive = DateTime.UtcNow;
            }
            repository.Save(user);
        }

        public IQueryable<User> GetUserNotAssociatedWithSchool(int districtId, IEnumerable<int> listUserId)
        {
            return repository.Select().Where(o => o.DistrictId == districtId && !listUserId.Contains(o.Id));
        }

        public IQueryable<Role> GetRoles()
        {
            return _roleRepository.Select();
        }

        public string SetTemporaryPassword(User user)
        {
            var temporaryPassword = CreateRandomPassword();

            user.HashedPassword = Crypto.HashPassword(temporaryPassword);
            user.HasTemporaryPassword = true;
            user.PasswordLastSetDate = DateTime.UtcNow.Date;
            repository.Save(user);

            return temporaryPassword;
        }

        private static string CreateRandomPassword()
        {
            var allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            var chars = new char[10];
            var rd = new Random();

            for (var i = 0; i < 10; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public void UpdateLastLogin(int userId)
        {
            var user = repository.Select().FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                repository.Save(user);
            }
        }

        public IQueryable<User> GetUsersByDistrictId(int districtId)
        {
            return repository.Select().Where(x => x.DistrictId.Equals(districtId)).OrderBy(x => x.UserName);
        }

        public IQueryable<User> GetManagedUsersByDistrictId(int districtId)
        {
            return repository.Select().Where(x => x.DistrictId.Equals(districtId)
                && (x.RoleId == (int)Permissions.Teacher || x.RoleId == (int)Permissions.SchoolAdmin || x.RoleId == (int)Permissions.DistrictAdmin || x.RoleId == (int)Permissions.Publisher || x.RoleId == (int)Permissions.NetworkAdmin || x.RoleId == (int)Permissions.StateAdministrator)).OrderBy(x => x.UserName);
        }

        public IQueryable<User> GetActiveManagedUsersByDistrictId(int districtId)
        {
            var activeStatus = (int)UserStatus.Active;
            return repository.Select().Where(x => x.DistrictId.Equals(districtId) && x.UserStatusId == activeStatus
                && (x.RoleId == (int)Permissions.Teacher || x.RoleId == (int)Permissions.SchoolAdmin || x.RoleId == (int)Permissions.DistrictAdmin || x.RoleId == (int)Permissions.Publisher || x.RoleId == (int)Permissions.NetworkAdmin || x.RoleId == (int)Permissions.StateAdministrator)).OrderBy(x => x.UserName);
        }

        public IQueryable<User> GetUsersByDistrictAndRole(int districtId, int roleId)
        {
            return GetUsersByDistrictId(districtId).Where(x => x.RoleId.Equals(roleId));
        }

        public IQueryable<User> GetDistrictAdmins(int? districtID, IEnumerable<int> excludeIds)
        {
            var activeStatus = (int)UserStatus.Active;
            var districtAdminRoleID = (int)Permissions.DistrictAdmin;
            var query = repository.Select()
                .Where(x => x.UserStatusId == activeStatus &&
                            x.DistrictId == districtID &&
                            x.RoleId == districtAdminRoleID &&
                            !excludeIds.Contains(x.Id));           
            return query;
        }

        public IQueryable<User> GetDistrictAndSchoolAdmins(int? districtID)
        {
            var activeStatus = (int)UserStatus.Active;
            var districtAdminRoleID = (int)Permissions.DistrictAdmin;
            var schoolAdminRoleID = (int)Permissions.SchoolAdmin;
            var query =
                repository.Select()
                    .Where(
                        x =>
                            x.UserStatusId == activeStatus &&
                            (x.RoleId == districtAdminRoleID || x.RoleId == schoolAdminRoleID));
            if (districtID.HasValue)
            {
                query = query.Where(o => o.DistrictId == districtID.Value);
            }

            return query;
        }

        public List<User> GetUserByNames(IEnumerable<string> lstUserName, int districtId)
        {
            return repository.Select().Where(o => o.DistrictId == districtId && lstUserName.Contains(o.UserName)).ToList();
        }

        public void Delete(User item)
        {
            repository.Delete(item);
        }

        public IQueryable<User> GetUserByAuthorGroupId(int authorGroupId)
        {
            var listUserIds =
                authorGroupRepository.Select()
                    .Where(x => x.AuthorGroupId.Equals(authorGroupId))
                    .Select(x => x.UserId)
                    .Distinct().ToList();

            var userQuery = repository.Select().Where(x => listUserIds.Contains(x.Id));

            return userQuery;
        }

        public IQueryable<User> GetUserByStateIdAndDistrictIdAndSchoolId(int stateId, int districtId, int schoolId)
        {
            var query = repository.Select().Where(u => u.StateId == stateId);

            if (districtId > 0)
            {
                query = query.Where(u => u.DistrictId == districtId);
            }

            if (schoolId > 0)
            {
                query = query.Where(u => u.SchoolId == schoolId);
            }

            return query;
        }

        public IQueryable<User> GetActiveUserByStateIdAndDistrictIdAndSchoolId(int stateId, int districtId, int schoolId)
        {
            var query = repository.Select().Where(u => u.UserStatusId == 1
                && (u.RoleId == (int)Permissions.Publisher || u.RoleId == (int)Permissions.DistrictAdmin || u.RoleId == (int)Permissions.SchoolAdmin || u.RoleId == (int)Permissions.Teacher || u.RoleId == (int)Permissions.NetworkAdmin)
                && u.StateId == stateId);

            if (districtId > 0)
            {
                query = query.Where(u => u.DistrictId == districtId);
            }

            if (schoolId > 0)
            {
                query = query.Where(u => u.SchoolId == schoolId);
            }

            return query;
        }

        public User GetUserByEmailAndDistrict(string email, int districtId)
        {
            return repository.Select().FirstOrDefault(x => x.EmailAddress.Equals(email) && x.DistrictId.Equals(districtId));
        }

        public User GetStudentByUserName(string userName, int districtId, int roleId = (int)Permissions.Student)
        {
            var user = repository.Select().FirstOrDefault(x => x.UserName.Equals(userName) && x.DistrictId == districtId && x.RoleId == roleId);
            if(roleId == (int)Permissions.Parent && user != null)
            {
                var parent = _parentRepository.Select().FirstOrDefault(o => o.UserID == user.Id);
                if (parent == null)
                    return null;
            }
            return user;
        }

        public void UpdateDateConfirmActive(int userId)
        {
            var vUser = repository.Select().FirstOrDefault(o => o.Id == userId);
            if (vUser != null && vUser.Id > 0 && vUser.UserStatusId == (int)UserStatus.Active)
            {
                try
                {
                    vUser.DateConfirmedActive = DateTime.UtcNow;
                    vUser.ModifiedDate = DateTime.UtcNow;
                    repository.Save(vUser);
                }
                catch (Exception exception)
                {
                    if (!exception.Message.Equals("Row not found or changed."))
                    {
                        throw exception;
                    }
                }
            }
        }

        public List<User> GetUsersByUserNameandHashedPassword(string userName, string hashedPassword)
        {
            return repository.Select().Where(o => o.UserName == userName && o.HashedPassword == hashedPassword).ToList();
        }

        public int GetUserLoginFailCount(int userID)
        {
            return userManageRepository.GetUserLoginFailedCount(userID);
        }

        public void DeleteUserLoginFailCount(int userID)
        {
            userManageRepository.DeleteUserLoginFailedCount(userID);
        }

        public void IncreaseUserLoginFailCount(int userID)
        {
            userManageRepository.IncreaseUserLoginFailedCount(userID);
        }

        public bool CheckExistCode(string code)
        {
            return repository.Select().Any(o => o.LocalCode.ToUpper() == code.ToUpper());
        }

        public Student GetParentByRegistrationCode(string rCode, int districtId, out string errorMessage)
        {
            var parent = _manageParentRepository.GetParentByRegistrationCode(districtId, rCode);
            errorMessage = string.Empty;
            if (parent != null)
            {
                var user = new Student()
                {
                    Id = parent.UserId,
                    FirstName = parent.NameFirst,
                    LastName = parent.NameLast,
                    UserName = parent.UserName
                };
                return user;
            }

            errorMessage = "This is not a valid registration code.";
            return null;
        }

        public User GetUserByRegistrationCode(string rCode)
        {
            var student = studentRepository.Select().Where(o => o.RegistrationCode.ToUpper() == rCode.ToUpper()).FirstOrDefault();
            if (student != null)
            {                              
                var userId = _studentUserRepository.GetUserIDViaStudentUser(student.Id);
                if (userId > 0)
                {
                    return repository.Select().FirstOrDefault(x => x.Id == userId);
                }
            }
            return null;
        }

        public Student RegistrationCode(string rCode, int districtId, bool isStudentLogin, bool hasGenerateLogin, out string errorMessage)
        {
            if (!isStudentLogin)
            {
                return GetParentByRegistrationCode(rCode, districtId, out errorMessage);
            }

            errorMessage = string.Empty;

            var student = studentRepository.Select().Where(o => o.RegistrationCode.ToUpper() == rCode.ToUpper() && o.DistrictId == districtId).FirstOrDefault();
            if (student == null)
            {
                errorMessage = "This is not a valid registration code.";
                return null;
            }

            var userId = _studentUserRepository.GetUserIDViaStudentUser(student.Id);
            if (userId > 0)
            {
                var user = repository.Select().FirstOrDefault(x => x.Id == userId);
                if (!string.IsNullOrEmpty(user?.HashedPassword))
                {
                    errorMessage = "You have already created an account. Please contact your teacher to find out your username and password.";
                    return null;
                }

                if (hasGenerateLogin)
                    student.UserName = user?.UserName;
            }

            return student;
        }

        public User RegistrationCodeCreateUser(string rCode, string username, string password, out Student student, out string errorMessage)
        {
            errorMessage = string.Empty;

            student = studentRepository.Select().Where(o => o.RegistrationCode.ToUpper() == rCode.ToUpper()).FirstOrDefault();
            if (student == null)
            {
                errorMessage = "This is not a valid registration code.";
                return null;
            }

            var districtId = student.DistrictId;
            var studentId = student.Id;
                        
            int userId = _studentUserRepository.GetUserIDViaStudentUser(studentId);
            if (userId > 0 )
            {
                var user = repository.Select().FirstOrDefault(x => x.Id == userId);
                if (!string.IsNullOrEmpty(user?.HashedPassword))
                {
                    errorMessage = "You have already created an account. Please contact your teacher to find out your username and password.";
                    return null;
                }
                else
                {
                    user.Password = password;
                    user.HashedPassword = Crypto.HashPassword(password);
                    SaveUser(user);
                    return user;
                }
            }

            if (repository.Select().Any(x => x.UserName.ToLower() == username.ToLower() && x.DistrictId == districtId && x.RoleId == (int)Permissions.Student))
            {
                errorMessage = "Someone else has used this username. Please create a different one.";
                return null;
            }

            var district = districtRepository.Select().FirstOrDefault(x => x.Id.Equals(districtId));
            var newUser = new User()
            {
                DistrictId = student.DistrictId,
                UserName = username,
                FirstName = student.FirstName,
                LastName = student.LastName,
                EmailAddress = student.Email,
                RoleId = (int)Permissions.Student,
                LocalCode = student.Code,
                Password = password,
                HashedPassword = Crypto.HashPassword(password),
                HasTemporaryPassword = false,
                UserStatusId = (int)UserStatus.Active,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                StateId = district.StateId,
                LastLoginDate = DateTime.UtcNow,
                PasswordLastSetDate = DateTime.UtcNow
            };

            SaveUser(newUser);
            studentRepository.Save(student);
            
            _studentUserRepository.Save( new StudentUser()
            {
                StudentId = student.Id,
                UserId = newUser.Id
            });
            return newUser;
        }

        public User RegistrationCodeParentUser(ParentInformationDto model, out string errorMessage)
        {
            errorMessage = string.Empty;
            var parent = _manageParentRepository.GetParentByRegistrationCode(model.DistrictId, model.RegistrationCode);
            if (parent == null)
            {
                errorMessage = "This is not a valid registration code.";
                return null;
            }

            var user = repository.Select().FirstOrDefault(x => x.Id == parent.UserId);

            user.HasTemporaryPassword = false;
            user.Password = model.Password;
            user.HashedPassword = Crypto.HashPassword(model.Password);
            user.HasTemporaryPassword = false;
            user.LastLoginDate = DateTime.UtcNow;
            user.PasswordLastSetDate = DateTime.UtcNow;
            user.ModifiedDate = DateTime.UtcNow;
            user.RoleId = (int)Permissions.Parent;

            repository.Save(user);
            _manageParentRepository.DisableRegistrationCode(parent.UserId);

            return user;
        }

        public User GetParentActiveByUserName(string userName, int districtId)
        {
            var user = repository.Select().FirstOrDefault(x => x.UserName.Equals(userName) && x.DistrictId == districtId && x.RoleId == (int)Permissions.Parent && x.UserStatusId == (int)UserStatus.Active);
            if (user != null && _manageParentRepository.CheckRegistrationCodeActive(user.Id))
            {
                return user;
            }
            return null;
        }

        public bool CheckUserCodeExistsStartWithRezoByDistrictID(int districtId, string userCode, int userId)
        {
            var result = userManageRepository.CheckUserCodeExistsStartWithRezoByDistrictID(districtId, userCode, userId);
            return result;
        }

        public void GetWelcomeMessage(User objUser)
        {
            string strReturn = objUser.Name;

            var roleAndGroupName = userManageRepository.GetRoleAndGroupNameByUserId(objUser.Id);
            if (string.IsNullOrEmpty(roleAndGroupName.RoleAndGroupName) == false)
            {
                strReturn += ", " + roleAndGroupName.RoleAndGroupName;
            }
            objUser.WelcomeMessage = strReturn.Trim();
            objUser.RoleAndGroupName = roleAndGroupName;
        }

        public SSODistrictGroup GetConfigLogonBySSO(int districtID, string type = "auth0")
        {
            return _ssoDistrictGroupRepository.GetByDistrictID(districtID, type);
        }

        public SSODistrictGroup GetSSODistrictGroupByTenantID(int tenantID, string type = "auth0")
        {
            return _ssoDistrictGroupRepository.GetByTenantID(tenantID, type);
        }

        public SSOInformation GetSSOInformationByType(string type)
        {
            return _ssoInformationRepository.GetByTpe(type);
        }

        public User GetLinkitUserFromMapping(string adUsername, int districtId, string type = "auth0")
        {
            var user = _ssoUserMappingRepository.GetLinkitUserFromMapping(adUsername, districtId, type);

            if (user != null && user.UserEntity != null)
            {
                return new User
                {
                    Id = user.UserEntity.UserID,
                    UserName = user.UserEntity.UserName,
                    RoleId = user.UserEntity.RoleID
                };
            }

            return new User();
        }

        public User GetLinkitUserFromMapping(string adUsername, int districtId, string type, IEnumerable<int> roleIds)
        {
            var user = _ssoUserMappingRepository.GetLinkitUserFromMapping(adUsername, districtId, type, roleIds);

            if (user != null && user.UserEntity != null)
            {
                return new User
                {
                    Id = user.UserEntity.UserID,
                    UserName = user.UserEntity.UserName,
                    RoleId = user.UserEntity.RoleID,
                    UserStatusId = user.UserEntity.UserStatusID
                };
            }

            return new User();
        }

        public int GetUserSurveyByUserNameAndDistrictId(int districtId, string userName)
        {
            var objUser = repository.Select().FirstOrDefault(o => o.DistrictId == districtId && o.UserName == userName);
            if (objUser != null)
                return objUser.Id;
            return 0;
        }

        public UserMappingResultDto GetCanvasUserMapping(int districtId, UserMappingDto userMappingDto)
        {
            var userMappingResult = new UserMappingResultDto();

            User user = null;
            var userMapping = _ssoUserMappingRepository.GetLinkitUserFromMapping(userMappingDto.Sub, districtId, SSOProvider.CANVAS);
            if (userMapping != null)
            {
                userMappingResult.UserName = userMapping.UserEntity.UserName;
                userMappingResult.RoleId = userMapping.UserEntity.RoleID;
                return userMappingResult;
            }

            var users = repository.Select().Where(m => m.DistrictId == districtId && m.EmailAddress == userMappingDto.Email).ToList();
            if (users != null && users.Count == 1)
            {
                userMappingResult.UserName = users.First().UserName;
                userMappingResult.RoleId = users.First().RoleId;
                return userMappingResult;
            }

            user = repository.Select().FirstOrDefault(m => m.DistrictId == districtId && (m.SISID == userMappingDto.PersonSourcedId || m.LocalCode == userMappingDto.PersonSourcedId));
            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                userMappingResult.UserName = user.UserName;
                userMappingResult.RoleId = user.RoleId;
                return userMappingResult;
            }

            if (string.IsNullOrEmpty(userMappingResult.UserName))
            {
                int sisId = 0;
                var student = studentRepository.Select().FirstOrDefault(c => c.DistrictId == districtId && c.Email == userMappingDto.Email && c.Status == (int)UserStatus.Active);
                if (student == null)
                {
                    if (int.TryParse(userMappingDto.PersonSourcedId, out sisId))
                    {
                        student = studentRepository.Select().FirstOrDefault(c => c.DistrictId == districtId && (c.SISID == sisId || c.Code == userMappingDto.PersonSourcedId || c.AltCode == userMappingDto.PersonSourcedId) && c.Status == (int)UserStatus.Active);

                        if (student == null)
                            return userMappingResult;

                        return CreateNewUser(student, userMappingResult, districtId, userMappingDto.Sub);
                    }
                }
                else
                    return CreateNewUser(student, userMappingResult, districtId, userMappingDto.Sub);
            }
            return userMappingResult;
        }

        private UserMappingResultDto CreateNewUser(Student student, UserMappingResultDto userMappingResultDto, int districtId, string sub)
        {
            var user = CreateUserByStudent(student);

            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                SaveSSOUserMapping(sub, user.Id, districtId, SSOProvider.CANVAS);
                userMappingResultDto.UserName = user.UserName;
                userMappingResultDto.RoleId = user.RoleId;
                userMappingResultDto.IsFirstStudentLogonSSO = true;
                return userMappingResultDto;
            }
            return null;
        }

        public ClassLinkUserResultDTO GetClassLinkUserMapping(int districtId, ClassLinkProfileDTO profile)
        {
            var result = new ClassLinkUserResultDTO();
            var classlinkmappingkey = _districtDecodeService.GetDistrictDecodesByLabel(SSOMappingKey.CLASSLINK).FirstOrDefault(m => m.DistrictID == districtId)?.Value;

            ClassLinkMapping[] classLinkMappings = JsonConvert.DeserializeObject<ClassLinkMapping[]>(classlinkmappingkey);
            var mappings = classLinkMappings
                .FirstOrDefault(x => x.LinkItRoleIDs.Contains(profile.Role.ToLinkitRole()))?.Mapping
                .OrderBy(x => x.Priority)
                .ToList();

            if (mappings == null)
            {
                return null;
            }

            foreach (var mapping in mappings)
            {
                var valueMapping = profile.GetType().GetProperty(mapping.ClassLinkField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).GetValue(profile, null).ToString();

                if (string.IsNullOrEmpty(valueMapping))
                {
                    continue;
                }

                var linkitFields = mapping.LinkItField
                    .Split('|')
                    .Where(x => !string.IsNullOrEmpty(x?.Trim()))
                    .Select(x => x.Trim().ToUpper());

                foreach (var fieldMapping in linkitFields)
                {
                    result = TryMappingClassLink(fieldMapping, valueMapping, districtId);

                    if (result.Users.Any() || result.Students.Any() || result.Parents.Any())
                    {
                        return result;
                    }
                }
            }

            return result;
        }

        public List<User> Find(string email, int districtId, IEnumerable<int> roleIds)
        {
            var users = repository.Select()
                .Where(x => x.EmailAddress != null && x.EmailAddress.ToLower() == email.ToLower()
                && x.DistrictId == districtId && roleIds.Contains(x.RoleId)).ToList();

            return users;
        }

        public User CreateUserByStudent(Student student, string userName = null, string password = null, bool noPassword = false)
        {
            User user = null;

            if (student != null)
            {
                var userId = _studentMetaService.GetUserIdByStudentId(student.Id);
                user = GetUserById(userId);
                if (user == null)
                {
                    var district = districtRepository.Select().FirstOrDefault(x => x.Id.Equals(student.DistrictId));

                    var hasGenLogin = !string.IsNullOrEmpty(password);
                    userName = userName ?? GetUniqueUserName(student.DistrictId, (int)Permissions.Student, student.Code);
                    password = password ?? _testCodeGenerator.GenerateTestCode(8, string.Empty);

                    user = new User()
                    {
                        DistrictId = student.DistrictId,
                        UserName = userName,
                        LocalCode = student.Code,
                        FirstName = student.FirstName,
                        LastName = student.LastName,
                        EmailAddress = student.Email,
                        RoleId = (int)Permissions.Student,
                        Password = noPassword ? null : password,
                        HashedPassword = noPassword ? null : Crypto.HashPassword(password),
                        HasTemporaryPassword = false,
                        UserStatusId = (int)UserStatus.Active,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        LastLoginDate = DateTime.UtcNow,
                        SISID = student.StateCode,
                        StateId = district.StateId,
                        ModifiedBy = TextConstants.MODIFIED_BY_DEFAULT
                    };

                    if (hasGenLogin)
                    {
                        user.LastLoginDate = DateTime.UtcNow;
                        user.PasswordLastSetDate = DateTime.UtcNow;
                    }

                    var isExist = repository.Select().Any(x => x.DistrictId == user.DistrictId && x.RoleId == user.RoleId && x.UserName == user.UserName);
                    if (isExist) return null;

                    SaveUser(user);                    
                    _studentUserRepository.Save(new StudentUser() {
                        StudentId = student.Id,
                        UserId = user.Id
                    });
                }
                return user;
            }

            return user;
        }

        private string GetUniqueUserName(int districtId, int roleId, string userName)
        {
            var isUserExisted = repository.Select()
                .Any(x => x.DistrictId == districtId && x.UserName.ToLower() == userName.ToLower() && x.RoleId == roleId);
            if(isUserExisted == false) return userName;

            var charactersRange = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int randomNumber = new Random().Next(0, charactersRange.Length - 1);
            return GetUniqueUserName(districtId, roleId, $"{userName}{charactersRange[randomNumber]}");
        } 

        public void SaveSSOUserMapping(string externalUser, int linkItUserId, int districtId, string type)
        {
            var ssoUserMapping = new SSOUserMapping
            {
                ADUsername = externalUser,
                UserID = linkItUserId,
                DistrictID = districtId,
                Type = type
            };

            _ssoUserMappingRepository.Save(ssoUserMapping);
        }

        public List<UserDto> GetUserByEmails(string emails, int districtId)
        {
            return repository.Select()
                .Select(o => new UserDto
                {
                    UserId = o.Id,
                    EmailAddress = o.EmailAddress,
                    UserName = o.UserName,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    DistrictId = o.DistrictId,
                    StateId = o.StateId,
                    RoleId = o.RoleId,
                    UserStatusId = o.UserStatusId
                })
                .Where(x => x.EmailAddress != null && x.EmailAddress != string.Empty && !string.IsNullOrEmpty(emails) && emails.ToLower().IndexOf(x.EmailAddress.ToLower()) >= 0 && x.DistrictId == districtId && x.UserStatusId == 1)
                .ToList();
        }

        public List<PopulationUser> GetDistrictAdminsDropdown(int districtId, IEnumerable<int> excludeIds)
        {
            var data =  GetDistrictAdmins(districtId, excludeIds)
                .Select(x => new PopulationUser
                {
                    Name = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Id = x.Id
                }).ToList();
            return data;
        }


        private ClassLinkUserResultDTO TryMappingClassLink(string fieldMapping, string valueMapping, int districtId)
        {
            var result = new ClassLinkUserResultDTO();

            if (string.IsNullOrEmpty(valueMapping))
            {
                return result;
            }

            switch (fieldMapping.ToUpper())
            {
                case "INTEROPERABILITY.SOURCEID":
                    var interoperabilities = _interoperabilityRepository.Select().Where(x => x.DistrictID == districtId && x.SourceId == valueMapping).ToList();

                    foreach (var item in interoperabilities)
                    {
                        switch (item.LinkitObjectTypeId)
                        {
                            case (int)ELinkitObjectType.Student:
                                var student = studentRepository.Select().FirstOrDefault(x => x.Id == item.ObjectId);

                                if (student != null)
                                {
                                    result.Students.Add(student);
                                }
                                break;
                            case (int)ELinkitObjectType.User:
                                var user = repository.Select().FirstOrDefault(x => x.Id == item.ObjectId);

                                if (user != null)
                                {
                                    result.Users.Add(user);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "USER.SISID":
                    result.Users = repository.Select().Where(m => m.DistrictId == districtId && m.SISID == valueMapping).ToList();
                    break;
                case "USER.CODE":
                    result.Users = repository.Select().Where(m => m.DistrictId == districtId && m.LocalCode == valueMapping).ToList();
                    break;
                case "USER.STATECODE":
                    result.Users = repository.Select().Where(m => m.DistrictId == districtId && m.StateCode == valueMapping).ToList();
                    break;
                case "USER.EMAIL":
                    result.Users = repository.Select().Where(m => m.DistrictId == districtId && m.EmailAddress == valueMapping).ToList();
                    break;
                case "USER.USERNAME":
                    result.Users = repository.Select().Where(m => m.DistrictId == districtId && m.UserName == valueMapping).ToList();
                    break;
                case "STUDENT.CODE":
                    result.Students = studentRepository.Select().Where(m => m.DistrictId == districtId && m.Code == valueMapping).ToList();
                    break;
                case "STUDENT.EMAIL":
                    result.Students = studentRepository.Select().Where(m => m.DistrictId == districtId && m.Email == valueMapping).ToList();
                    break;
                case "STUDENT.SISID":
                    int sisid;
                    if (int.TryParse(valueMapping, out sisid))
                    {
                        result.Students = studentRepository.Select().Where(m => m.DistrictId == districtId && m.SISID == sisid).ToList();
                    }

                    break;
                case "STUDENT.STATECODE":
                    result.Students = studentRepository.Select().Where(m => m.DistrictId == districtId && m.StateCode == valueMapping).ToList();
                    break;
                case "STUDENT.ALTCODE":
                    result.Students = studentRepository.Select().Where(m => m.DistrictId == districtId && m.AltCode == valueMapping).ToList();
                    break;
                case "PARENT.EMAIL":
                    result.Parents = _parentRepository.Select().Where(m => m.DistrictID == districtId && m.Email == valueMapping).ToList();
                    break;
                default:
                    break;
            }

            return result;
        }

        public User GenerateRandomStudentAccount(int? stateId, Student student, List<Configuration> configurations)
        {
            var userId = _studentMetaService.GetUserIdByStudentId(student.Id);
            var user = userId > 0 ? GetUserById(userId) : null;

            if (user == null)
            {
                var account = RandomStudentAccount(student.DistrictId, configurations);
                var password = string.IsNullOrEmpty(student.Email) ? account.Password : null;

                user = new User()
                {
                    DistrictId = student.DistrictId,
                    UserName = account.UserName,
                    LocalCode = student.Code,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    EmailAddress = student.Email,
                    RoleId = (int)Permissions.Student,
                    Password = password,
                    HashedPassword = !string.IsNullOrEmpty(password) ? Crypto.HashPassword(password) : null,
                    LastLoginDate = DateTime.UtcNow,
                    SISID = student.StateCode,
                    StateId = stateId,
                    PasswordLastSetDate = DateTime.UtcNow
                };

                var success = repository.AddNewUser(user);
                if (!success) return null;

                _studentUserRepository.Save(new StudentUser()
                {
                    StudentId = student.Id,
                    UserId = user.Id
                });

                if (!string.IsNullOrEmpty(password))
                {
                    student.SharedSecret = password;
                    studentRepository.Save(student);
                }
            }

            return user;
        }

        private void UpdateStudentSecret(int userId, string sharedSecret)
        {
            var studentId = _studentUserRepository.GetStudentIDViaStudentUser(userId);
            if (studentId > 0)
            {
                var student = studentRepository.Select().FirstOrDefault(x => x.Id == studentId);
                if (student != null)
                {
                    var allowGenAccount = _districtDecodeService.GetDistrictDecodeOrConfigurationByLabel(student.DistrictId, Constanst.ALLOW_STUDENT_USER_GENERATION);
                    if (allowGenAccount)
                    {
                        student.SharedSecret = sharedSecret;
                        studentRepository.Save(student);
                    }
                }
            }
        }

        private RandomStudentAccountDto RandomStudentAccount(int districtId, List<Configuration> configurations = null)
        {
            if (configurations == null)
            {
                var configNames = new List<string>
                {
                    Constanst.STUDENT_SECRET_LETTERS,
                    Constanst.STUDENT_SECRET_NUMBERS,
                    Constanst.STUDENT_SECRET_USERNAME_PATTERN,
                    Constanst.STUDENT_SECRET_PASSWORD_PATTERN
                };
                configurations = _districtDecodeService.GetConfigurationValues(configNames);
            }

            var random = new Random();
            var letters = configurations.FirstOrDefault(x => x.Name.Equals(Constanst.STUDENT_SECRET_LETTERS))?.Value.ToStringList();
            var numbers = configurations.FirstOrDefault(x => x.Name.Equals(Constanst.STUDENT_SECRET_NUMBERS))?.Value.ToStringList();
            var userNamePatterns = configurations.FirstOrDefault(x => x.Name.Equals(Constanst.STUDENT_SECRET_USERNAME_PATTERN))?.Value.ToStringList("-");
            var passwordPatterns = configurations.FirstOrDefault(x => x.Name.Equals(Constanst.STUDENT_SECRET_PASSWORD_PATTERN))?.Value.ToStringList("-");

            var userName = GetUniqueStudentUserName(districtId, userNamePatterns, letters, numbers);
            var password = passwordPatterns.Select(pattern => GetRandomCharacter(pattern, random, letters, numbers)).JoinToString("");

            return new RandomStudentAccountDto
            {
                UserName = userName,
                Password = password
            };
        }

        private string GetUniqueStudentUserName(int districtId, List<string> patterns, List<string> letters, List<string> numbers)
        {
            var random = new Random();

            while (true)
            {
                var userName = patterns.Select(pattern => GetRandomCharacter(pattern, random, letters, numbers)).JoinToString("");

                var isUserExisted = Select().Any(x =>
                    x.DistrictId == districtId &&
                    x.UserName.ToLower() == userName &&
                    x.RoleId == (int)RoleEnum.Student
                );

                if (!isUserExisted)
                    return userName;
            }
        }

        private static string GetRandomCharacter(string pattern, Random random, List<string> letters, List<string> numbers)
        {
            switch (pattern)
            {
                case "L": return letters[random.Next(letters.Count)];
                case "N": return numbers[random.Next(numbers.Count)];
                default: return pattern;
            }
        }
    }
}
