using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories.Helper;
using LinkIt.BubbleSheetPortal.Data;
using System.Data;
using LinkIt.BubbleSheetPortal.Models.DTOs.RegistrationCode;
using LinkIt.BubbleSheetPortal.Data.Repositories.StudentData;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Models.DTOs.Users;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StudentService
    {
        private readonly IRepository<Student> repository;
        private readonly IRepository<StudentChangedLog> logRepository;
        private readonly IRepository<StudentChangedLogValue> logValueRepository;
        private readonly IReadOnlyRepository<StudentGenderGrade> studentGenderGradeRepository;
        private readonly IReadOnlyRepository<StudentTeacher> studentTeacher;
        private readonly ILookupStudentRepository lookupStudentRepository;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<StudentMeta> studentMetaRepository;
        private readonly IStudentGradeRepository studentGradeRepository;
        private readonly ConfigurationService _configurationService;
        private readonly IConnectionString _connectionString;
        private readonly IStudentUserRepository _studentUserRepository;
        private readonly UserService _userService;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly IStateRepository _stateRepository;
        private readonly IReadOnlyRepository<District> _districtRepository;

        public StudentService(IRepository<Student> repository,
            IRepository<StudentChangedLog> logRepository,
            IRepository<StudentChangedLogValue> logValueRepository,
            IReadOnlyRepository<StudentGenderGrade> studentGenderGradeRepository,
            IReadOnlyRepository<StudentTeacher> studentTeacher,
            ILookupStudentRepository lookupStudentRepository,
            IRepository<User> userRepository,
            IRepository<StudentMeta> studentMetaRepository,
            IStudentGradeRepository studentGradeRepository,
            ConfigurationService configurationService,
            IConnectionString connectionString,
            IStudentUserRepository studentUserRepository,
            UserService userService,
            DistrictDecodeService districtDecodeService,
            IStateRepository stateRepository,
            IReadOnlyRepository<District> districtRepository
            )
        {
            this.repository = repository;
            this.logRepository = logRepository;
            this.logValueRepository = logValueRepository;
            this.studentGenderGradeRepository = studentGenderGradeRepository;
            this.studentTeacher = studentTeacher;
            this.lookupStudentRepository = lookupStudentRepository;
            this.userRepository = userRepository;
            this.studentMetaRepository = studentMetaRepository;
            this.studentGradeRepository = studentGradeRepository;
            this._configurationService = configurationService;
            this._connectionString = connectionString;
            this._studentUserRepository = studentUserRepository;
            this._userService = userService;
            this._districtDecodeService = districtDecodeService;
            _stateRepository = stateRepository;
            _districtRepository = districtRepository;
        }

        public void Save(Student student)
        {
            repository.Save(student);
        }

        public Student GetStudentById(int studentId)
        {
            return repository.Select().FirstOrDefault(s => s.Id.Equals(studentId));
        }
        public StudentDataForRegistrationCodeEmailDto GetStudentDataForRegistrationCodeEmail(int studentId)
        {
            var studentData = repository.Select().Where(s => s.Id.Equals(studentId))
                .Select(c => new StudentDataForRegistrationCodeEmailDto
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    FullName = c.FullName,
                    DistrictId = c.DistrictId,
                    AdminSchoolId = c.AdminSchoolId,
                    Email = c.Email,
                    SharedSecret = c.SharedSecret
                }).FirstOrDefault();
            return studentData;
        }

        public Student GetStudentByCode(string code)
        {
            return repository.Select().FirstOrDefault(x => x.Code == code);
        }

        public IQueryable<StudentGenderGrade> GetAllStudentsInDistrict(int districtId, List<int> studentIdList)
        {
            return
                studentGenderGradeRepository.Select().Where(
                    x => x.DistrictId.Equals(districtId) && (!studentIdList.Contains(x.StudentId)));
        }

        public List<Student> GetDuplicateStudentRoster(int districtId, int classId, List<Student> students)
        {
            List<string> studentCodes = students.Select(en => en.Code.ToLower()).ToList();

            // Get all duplicate students in database by student code
            var duplicateStudentCodes =
                repository.Select().Where(x => x.DistrictId.Equals(districtId) && studentCodes.Contains(x.Code.ToLower())).ToList();

            // Get duplicate students by removing updated student list
            return duplicateStudentCodes.Where(en => students.Any(x => x.Code.ToLower() == en.Code.ToLower() && x.Id != en.Id)).ToList();

        }

        public IQueryable<StudentGenderGrade> GetAvailableStudentsForTeacherSchoolAdmin(int districtId, List<int?> schoolIdList, List<int> studentIdList)
        {
            /*School admins and teachers can only see and assign students that are in schools the user has access to. 
             * To determine the school the student is in, use the AdminSchoolID on the Student table. 
             * If there is no value there, the student is available for assignment by any user in the district. */
            return studentGenderGradeRepository.Select().Where(x => x.DistrictId.Equals(districtId)
                                                                    &&
                                                                    ((x.AdminSchoolID != null &&
                                                                      schoolIdList.Contains(x.AdminSchoolID)) ||
                                                                     (x.AdminSchoolID == null))
                                                                    && (!studentIdList.Contains(x.StudentId)));

        }

        public void TrackingLastSendDistributeEmail(int studentId)
        {
            lookupStudentRepository.TrackingLastSendDistributeEmail(studentId, DateTime.UtcNow);
        }

        public void UpdateAndSaveLog(Student newStudent, int updatedBy)
        {
            string errorMessage = string.Empty;
            this.UpdateAndSaveLog(newStudent, updatedBy, null, null, out errorMessage);
        }

        public void UpdateAndSaveLog(Student newStudent, int updatedBy, string newUserName, string newPassword, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (newStudent.IsNotNull())
            {
                Student oldStudent = GetStudentById(newStudent.Id);
                oldStudent.ModifiedDate = DateTime.UtcNow;

                List<StudentChangedLogValue> logList = CompareAndGetChangedLogList(newStudent, oldStudent);
                if (logList.IsNotNull())
                {
                    oldStudent.LoginCode = newStudent.LoginCode;

                    if (!string.IsNullOrEmpty(newUserName))
                    {
                        newUserName = newUserName.Trim();
                        var userId = _studentUserRepository.GetUserIDViaStudentUser(oldStudent.Id);
                        if (userId > 0 )
                        {
                            var userStudent = userRepository.Select().FirstOrDefault(x => x.Id == userId);
                            var dubUser = userRepository.Select()
                                .FirstOrDefault(x => x.UserName.ToLower() == newUserName.ToLower() && x.DistrictId == userStudent.DistrictId && x.RoleId == userStudent.RoleId && x.Id != userStudent.Id);
                            if (dubUser != null)
                            {
                                errorMessage = "Someone else has used this username. Please create a different one.";
                                return;
                            }

                            userStudent.UserName = newUserName;
                            if (!string.IsNullOrEmpty(newPassword))
                            {
                                userStudent.HashedPassword = newPassword;
                            }
                            userRepository.Save(userStudent);
                        }
                    }

                    Save(newStudent);
                    SaveChangedLog(logList, oldStudent.Id, updatedBy);
                }
            }
        }

        private List<StudentChangedLogValue> CompareAndGetChangedLogList(Student newStudent, Student oldStudent)
        {
            if (newStudent.IsNull() || oldStudent.IsNull())
            {
                return null;
            }
            List<StudentChangedLogValue> logValueList = new List<StudentChangedLogValue>();

            // First Name
            if (!newStudent.FirstName.Equals(oldStudent.FirstName))
            {
                logValueList.Add(GetLogValue("FirstName", oldStudent.FirstName, newStudent.FirstName));
                oldStudent.FirstName = newStudent.FirstName;
            }

            // Last Name
            if (!newStudent.LastName.Equals(oldStudent.LastName))
            {
                logValueList.Add(GetLogValue("LastName", oldStudent.LastName, newStudent.LastName));
                oldStudent.LastName = newStudent.LastName;
            }

            // Middle Name
            if (!newStudent.MiddleName.Equals(oldStudent.MiddleName))
            {
                logValueList.Add(GetLogValue("MiddleName", oldStudent.MiddleName, newStudent.MiddleName));
                oldStudent.MiddleName = newStudent.MiddleName;
            }

            // Gender
            if (!newStudent.GenderId.Equals(oldStudent.GenderId))
            {
                logValueList.Add(GetLogValue("GenderID", oldStudent.GenderId.ToString(), newStudent.GenderId.ToString()));
                oldStudent.GenderId = newStudent.GenderId;
            }

            // Race
            if (!newStudent.RaceId.Equals(oldStudent.RaceId))
            {
                logValueList.Add(GetLogValue("RaceID", oldStudent.RaceId.ToString(), newStudent.RaceId.ToString()));
                oldStudent.RaceId = newStudent.RaceId;
            }

            // Code
            if (!newStudent.Code.Equals(oldStudent.Code))
            {
                logValueList.Add(GetLogValue("Code", oldStudent.Code, newStudent.Code));
                oldStudent.Code = newStudent.Code;
            }

            // AltCode
            if (!newStudent.AltCode.Equals(oldStudent.AltCode))
            {
                logValueList.Add(GetLogValue("AltCode", oldStudent.AltCode, newStudent.AltCode));
                oldStudent.AltCode = newStudent.AltCode;
            }

            // StateCode
            if (!newStudent.StateCode.Equals(oldStudent.StateCode))
            {
                logValueList.Add(GetLogValue("StateCode", oldStudent.StateCode, newStudent.StateCode));
                oldStudent.StateCode = newStudent.StateCode;
            }

            // DateOfBirth
            if (!newStudent.DateOfBirth.Equals(oldStudent.DateOfBirth))
            {
                logValueList.Add(GetLogValue("Dateofbirth", oldStudent.DateOfBirth.ToString(), newStudent.DateOfBirth.ToString()));
                oldStudent.DateOfBirth = newStudent.DateOfBirth;
            }

            // Note01
            if (!newStudent.Note01.Equals(oldStudent.Note01))
            {
                logValueList.Add(GetLogValue("Note01", oldStudent.Note01, newStudent.Note01));
                oldStudent.Note01 = newStudent.Note01;
            }

            // Email
            if (!newStudent.Email.Equals(oldStudent.Email))
            {
                logValueList.Add(GetLogValue("Email", oldStudent.Email, newStudent.Email));
                oldStudent.Email = newStudent.Email;
            }

            // Grade
            if (!newStudent.CurrentGradeId.Equals(oldStudent.CurrentGradeId))
            {
                logValueList.Add(GetLogValue("Grade", oldStudent.CurrentGradeId.GetValueOrDefault().ToString(), newStudent.CurrentGradeId.GetValueOrDefault().ToString()));
                oldStudent.CurrentGradeId = newStudent.CurrentGradeId;
            }

            return logValueList;
        }

        private StudentChangedLogValue GetLogValue(string valueChanged, string oldValue, string newValue)
        {
            StudentChangedLogValue log = new StudentChangedLogValue();
            log.ValueChanged = valueChanged;
            log.OldValue = oldValue;
            log.NewValue = newValue;
            return log;
        }

        private void SaveChangedLog(List<StudentChangedLogValue> logList, int studentId, int updatedBy)
        {
            if (logList.Count > 0)
            {
                StudentChangedLog log = new StudentChangedLog
                {
                    StudentIDChanged = studentId,
                    UpdatedBy = updatedBy
                };
                logRepository.Save(log);

                foreach (var logItem in logList)
                {
                    logItem.LogID = log.LogID;
                    logValueRepository.Save(logItem);
                }
            }
        }

        public Student GetStudentByCodeOrEmail(int districtId, string email)
        {
            return repository.Select().FirstOrDefault(s => s.DistrictId == districtId && (s.Code == email || s.Email == email) && s.Status == (int)UserStatus.Active);
        }

        public Student GetStudentByEmail(int districtId, string email)
        {
            return repository.Select().FirstOrDefault(s => s.DistrictId == districtId && (s.Code == email || s.Email == email));
        }

        public IEnumerable<LookupStudent> LookupStudents(LookupStudentCustom obj, int skip, int pageSize, ref int? totalRecords, string sortColumns, string selectedUserIds = "")
        {
            var studentLookupResult = lookupStudentRepository.LookupStudent(obj, skip, pageSize, sortColumns, selectedUserIds);

            totalRecords = studentLookupResult?.FirstOrDefault()?.TotalRecords ?? 0;

            return studentLookupResult.Select(x => new LookupStudent
            {
                Code = x.Code,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FullName = x.FullName,
                GenderCode = x.GenderCode,
                GradeName = x.GradeName,
                RaceName = x.RaceName,
                SchoolName = x.SchoolName,
                StateCode = x.StateCode,
                StudentId = x.StudentID ?? 0,
                Status = x.Status,
                AdminSchoolId = x.AdminSchoolID,
                DistrictId = x.DistrictID,
                RegistrationCode = x.RegistrationCode,
                HasEmailAddress = x.Email?.Length > 1,
                Email = x.Email,
                RegistrationCodeEmailLastSent = x.RegistrationCodeEmailLastSent,
                TheUserLogedIn = x.TheUserLogedIn ?? false,
                SharedSecret = x.SharedSecret,
                UserName = x.UserName,
                Classes = x.Classes,
                HasPassword = x.HasPassword ?? false
            }).ToArray();

        }

        public void GenerateRegistrationCode(List<int> studentIds, int currentUserId, int studentRegistrationCodeLength)
        {
            if (studentIds.Count() <= 0)
            {
                return;
            }
            BulkHelper bulkHelper = new BulkHelper(_connectionString);
            string tempTableName = "#StudentIdAndRegistrationCode";
            string tempTableCreateScript = $@"CREATE TABLE [{tempTableName}](StudentId int,RegistrationCode varchar({studentRegistrationCodeLength}))";
            string updateRegistrationCodeProcedureName = "UpdateStudentRegistrationCode";



            var randomGuids = ServiceUtil.RandomGuidIds(studentRegistrationCodeLength, studentIds.Count());

            var parentUserIdAndGegistrationCode = (from student in studentIds.Select((studentId, index) => new { studentId, index })
                                                   join guid in randomGuids.Select((guidString, index) => new { guidString, index })
                                                   on student.index equals guid.index
                                                   select new
                                                   {
                                                       StudentId = student.studentId,
                                                       RegistrationCode = guid.guidString
                                                   }).ToArray();



            using (DataSet dataSetDuplicateRegistrationCode = bulkHelper.BulkCopy(tempTableCreateScript, tempTableName, parentUserIdAndGegistrationCode, updateRegistrationCodeProcedureName, "@UserId", currentUserId, "@Now", DateTime.UtcNow))
            {
                if (dataSetDuplicateRegistrationCode?.Tables.Cast<DataTable>()?.FirstOrDefault() is DataTable userIdTable)
                {
                    string userIdColumnName = "DuplicateStudentIdRegistrationCode";
                    if (userIdTable.Columns.Contains(userIdColumnName))
                    {
                        var studentIdsNeedreGenerateRegistrationCode = userIdTable
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
                        if (studentIdsNeedreGenerateRegistrationCode.Count == 0)
                        {
                            return;
                        }
                        GenerateRegistrationCode(studentIdsNeedreGenerateRegistrationCode, currentUserId, studentRegistrationCodeLength);
                    }
                }
            }
        }

        public List<LookupStudent> SGOLookupStudents(LookupStudentCustom obj, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns)
        {
            return lookupStudentRepository.SGOLookupStudent(obj, pageIndex, pageSize, ref totalRecords, sortColumns);
        }

        public List<Race> LookupStudentGetRace(int districtId, int userId, int roleId)
        {
            return lookupStudentRepository.LookupStudentGetRace(districtId, userId, roleId);
        }

        public List<School> LookupStudentGetAdminSchool(int districtId, int userId, int roleId)
        {
            return lookupStudentRepository.LookupStudentGetAdminSchool(districtId, userId, roleId);
        }

        public Student CheckExistCodeStartWithZero(int districtId, string code, int studentId)
        {
            var student = lookupStudentRepository.CheckExistCodeStartWithZero(districtId, code.TrimStart('0'), studentId);
            return student;
        }

        public List<StudentGrade> GetGradesStudent(int districtId, int userId, int roleId)
        {
            return studentGradeRepository.GetGradesStudent(districtId, userId, roleId);
        }

        public List<StudentProgram> GetProgramsStudent(int districtId, int userId, int roleId)
        {
            return studentGradeRepository.GetProgramsStudent(districtId, userId, roleId);
        }

        public List<StudentGenderGrade> ManageParentGetStudentsAvailableByFilter(int districtId, string programIdList,
            string gradeIdList, bool showInactive, int userId, int roleId)
        {
            return studentGradeRepository.ManageParentGetStudentsAvailableByFilter(districtId, programIdList, gradeIdList,
                showInactive, userId, roleId);
        }

        public List<StudentGenderGrade> GetStudentsAvailableByFilter(int districtId, string programIdList,
          string gradeIdList, bool showInactive, int userId, int roleId)
        {
            return studentGradeRepository.GetStudentsAvailableByFilter(districtId, programIdList, gradeIdList,
                showInactive, userId, roleId);
        }

        public void AddManyStudentsToClass(int classId, string studentIdList)
        {
            studentGradeRepository.AddManyStudentsToClass(classId, studentIdList);
        }

        public Student SurveyInsertStudent(Student objStudent)
        {
            if (objStudent != null)
                repository.Save(objStudent);

            return objStudent;
        }

        public int GetStudentRegistrationCodeLength()
        {
            var studentRegistrationCodeLength = 5;
            var configuration = _configurationService.GetConfigurationByKey(ConfigurationNameConstant.StudentRegistrationCodeLength);
            if (configuration != null)
            {
                try
                {
                    studentRegistrationCodeLength = Convert.ToInt32(configuration.Value);
                }
                catch (Exception ex) { }
            }

            return studentRegistrationCodeLength;
        }

        public int GetUserIdViaStudentUser(int studentId)
        {
            return _studentUserRepository.GetUserIDViaStudentUser(studentId);
        }

        public IEnumerable<Student> GetStudents(IEnumerable<int> studentIds)
        {
            return repository.Select().Where(s => studentIds.Contains(s.Id));
        }
        public List<StudentSessionDto> GetStudentSession(int studentId, int districtId, int stateId)
        {
            var result = new List<StudentSessionDto>();

            //get login portal
            var loginPortalInfo = _userService.GetUserInfoByStudentId(studentId);
            var configDateFormat = _districtDecodeService.GetDateFormat(districtId);
            var formatDatetime = string.Format("{0} {1}", configDateFormat.DateFormat, configDateFormat.TimeFormat);
            if (loginPortalInfo != null)
            {
                result.Add(new StudentSessionDto
                {
                    TimeStampDate = loginPortalInfo.LastLoginDate,
                    PointOfEntryDisplay = "Student Portal",
                    BrowserNameDisplay = string.Empty,
                    StudentWANIP = string.Empty,
                    PointOfEntry = "Student Portal",
                    TestCode = string.Empty,
                    BrowserName = string.Empty,
                    BrowserVersion = string.Empty,
                    Type = 1
                });
            }
            var studentSessions = lookupStudentRepository.GetSessionStudent(studentId);
            if (studentSessions != null && studentSessions.Count > 0)
            {
                foreach (var session in studentSessions)
                {
                    var sessionItem = JsonConvert.DeserializeObject<StudentSessionInfoDto>(session.Data);
                    result.Add(new StudentSessionDto
                    {
                        BrowserName = sessionItem.BrowserName,
                        BrowserNameDisplay = string.Format("{0} {1}", sessionItem.BrowserName, sessionItem.BrowserVersion),
                        BrowserVersion = sessionItem.BrowserVersion,
                        PointOfEntry = sessionItem.PointOfEntry,
                        PointOfEntryDisplay = string.Format("{0} code {1}", sessionItem.PointOfEntry, sessionItem.TestCode),
                        StudentWANIP = sessionItem.StudentWANIP,
                        TestCode = sessionItem.TestCode,
                        TimeStampDate = sessionItem.TimeStamp,
                        Type = sessionItem.Type,
                    });
                }
            }

            if (result.Any())
            {
                var timeZoneId = _stateRepository.GetTimeZoneId(stateId);
                foreach (var session in result)
                {
                    session.TimeStamp = string.IsNullOrEmpty(timeZoneId) ?
                        session.TimeStampDate.ToString(formatDatetime) : session.TimeStampDate.ConvertTimeFromUtc(timeZoneId).ToString(formatDatetime);
                }
            }

            return result;
        }
        public List<StudentLoginSlipDto> GetStudentLoginSlip(string studentIds, string url, string logo)
        {
            return lookupStudentRepository.GetStudentLoginSlip(studentIds, url, logo);
        }

        public void GenerateStudentLogin(IEnumerable<Student> students)
        {
            var configNames = new List<string>
            {
                Constanst.STUDENT_SECRET_LETTERS,
                Constanst.STUDENT_SECRET_NUMBERS,
                Constanst.STUDENT_SECRET_USERNAME_PATTERN,
                Constanst.STUDENT_SECRET_PASSWORD_PATTERN
            };
            var configurations = _districtDecodeService.GetConfigurationValues(configNames);

            students = GetStudentsWithoutAccount(students);
            var stateId = students.Any() ? _districtRepository.Select().FirstOrDefault(x => x.Id == students.First().DistrictId)?.StateId : null;
            foreach (var student in students)
            {
                _userService.GenerateRandomStudentAccount(stateId, student, configurations);
            }
        }

        public bool HasStudentSecret(int userId)
        {
            var studentId = _studentUserRepository.GetStudentIDViaStudentUser(userId);
            var student = repository.Select().FirstOrDefault(x => x.Id == studentId);

            return !string.IsNullOrEmpty(student?.SharedSecret);
        }

        private List<Student> GetStudentsWithoutAccount(IEnumerable<Student> students)
        {
            var studentIds = students.Select(x => x.Id);
            var studentHasAccount = _studentUserRepository.Select().Where(x => studentIds.Contains(x.StudentId)).Select(x => x.StudentId).Distinct().ToList();
            
            return students.Where(x => !studentHasAccount.Contains(x.Id)).ToList();
        }
    }
}
