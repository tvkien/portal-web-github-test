using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using Envoc.Core.Shared.Extensions;
using AutoMapper;
using System;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class LookupStudentRepository : ILookupStudentRepository
    {
        private readonly StudentDataContext studentDataContext;
        private readonly SGODataContext sgoDataContext;

        public LookupStudentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            studentDataContext = StudentDataContext.Get(connectionString);
            sgoDataContext = SGODataContext.Get(connectionString);
        }
        public List<StudentLoginSlipDto> GetStudentLoginSlip(string studentIds, string url, string logo)
        {
            return studentDataContext.SP_Student_GetStudentLoginSlip(studentIds).Select(s => new StudentLoginSlipDto()
            {
                Url = url,
                DistrictLogo = logo,
                FirstName = s.FirstName,
                LastName = s.LastName,
                SchoolName = s.SchoolName,
                LocalCode = s.LocalCode,
                AltCode = s.AltCode,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                RegistrationCode = s.RegistrationCode,
                YearLevel = s.YearLevel,
                UserName = s.UserName,
                PassCode = s.PassCode,
                Class = s.Class,
                Classes = s.Classes,
            }).ToList();
        }

        public List<TestResultMetaDto> GetSessionStudent(int studentId)
        {
            return studentDataContext.SP_TestResultMeta_GetStudentSession(studentId).Select(s => new TestResultMetaDto()
            {
                StudentId = studentId,
                Data = s.Data
            }).ToList();
        }


        public IEnumerable<StudentLookupResult> LookupStudent(LookupStudentCustom obj, int skip, int pageSize, string sortColumns, string selectedUserIds = "")
        {
            int? studentStatus = 1;
            if (obj.ShowInactiveStudent)
                studentStatus = null;

            return studentDataContext.StudentLookup(obj.DistrictId, obj.UserId, obj.RoleId, obj.FirstName, obj.LastName,
                obj.Code, obj.StateCode, obj.SchoolId, obj.GradeId, obj.RaceName, obj.GenderId, studentStatus, obj.SSearch, skip, pageSize, sortColumns, selectedUserIds, obj.ClassId).ToArray();
        }

        public List<LookupStudent> SGOLookupStudent(LookupStudentCustom obj, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns)
        {
            int? studentStatus = 1;
            if (obj.ShowInactiveStudent)
                studentStatus = null;
            return sgoDataContext.SGOStudentLookup(obj.ClassId, obj.DistrictId, obj.UserId, obj.RoleId, obj.FirstName, obj.LastName,
                obj.Code, obj.StateCode, obj.SchoolId, obj.GradeId, obj.RaceName, obj.GenderId, obj.SGOID, studentStatus, obj.SSearch, pageIndex, pageSize,
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
                    Status = x.Status,
                    AdminSchoolId = x.AdminSchoolID,
                    DistrictId = x.DistrictID
                }).ToList();
        }

        public List<Race> LookupStudentGetRace(int districtId, int userId, int roleId)
        {
            return studentDataContext.StudentLookupGetRace(districtId, userId, roleId).ToList().Select(x => new Race
            {
                Id = x.RaceID,
                Name = x.Name
            }).ToList();
        }

        public List<School> LookupStudentGetAdminSchool(int districtId, int userId, int roleId)
        {
            return studentDataContext.StudentLookupGetAdminSchool(districtId, userId, roleId).ToList().Select(x => new School
            {
                Id = x.SchoolID,
                Name = x.Name
            }).ToList();
        }

        public void GenRCode(Dictionary<int, string> studentRCodes)
        {
            var studentIds = studentRCodes.Keys.ToList();
            var students = studentDataContext.StudentEntities.Where(x => studentIds.Contains(x.StudentID)).ToList();
            foreach (var student in students)
            {
                student.RegistrationCode = studentRCodes[student.StudentID];
            }

            studentDataContext.SubmitChanges();
        }
        public Student CheckExistCodeStartWithZero(int districtId, string code, int studentId)
        {
            var query = studentDataContext.GetAllStudentStartWithCodeByDistrictID(districtId, code.TrimStart('0'));
            if (studentId > 0)
            {
                //var result = query.FirstOrDefault(o => o.StudentID != studentId && o.Code.TrimStart('0').Equals(code) && o.DistrictID == districtId);
                //if (result != null)
                //{
                //    return new Student()
                //    {
                //        Id = result.StudentID,
                //        Code = result.Code,
                //        DistrictId = result.DistrictID,
                //        Status = result.Status,
                //        FirstName = result.FirstName,
                //        LastName = result.LastName
                //    };
                //}

                return query.Where(o => o.StudentID != studentId && o.Code.TrimStart('0').Equals(code) && o.DistrictID == districtId)
                            .Select(d => new Student()
                            {
                                Id = d.StudentID,
                                Code = d.Code,
                                DistrictId = d.DistrictID,
                                Status = d.Status,
                                FirstName = d.FirstName,
                                LastName = d.LastName
                            }).FirstOrDefault();
            }
            else
            {
                //var result = query.FirstOrDefault(o => o.Code.TrimStart('0').Equals(code) && o.DistrictID == districtId);
                //if (result != null)
                //{
                //    return new Student()
                //    {
                //        Id = result.StudentID,
                //        Code = result.Code,
                //        DistrictId = result.DistrictID,
                //        Status = result.Status,
                //        FirstName = result.FirstName,
                //        LastName = result.LastName
                //    };
                //}

                return query.Where(o => o.Code.TrimStart('0').Equals(code) && o.DistrictID == districtId)
                          .Select(d => new Student()
                          {
                              Id = d.StudentID,
                              Code = d.Code,
                              DistrictId = d.DistrictID,
                              Status = d.Status,
                              FirstName = d.FirstName,
                              LastName = d.LastName
                          }).FirstOrDefault();
            }
            //return null;
        }

        public void TrackingLastSendDistributeEmail(int studentId, DateTime utcNow)
        {
            var student = studentDataContext
                .StudentEntities
                .Where(c => c.StudentID == studentId)
                .FirstOrDefault();
            if (student != null)
            {
                student.RegistrationCodeEmailLastSent = utcNow;
                studentDataContext.SubmitChanges();
            }
        }

        public IQueryable<SurveyGetGridPopulationUserDto> SurveyGetGridPopulationUser(GetUserResultsRequest request)
        {
            return studentDataContext.SurveyGetGridPopulationUser(request.DistrictId, request.UserId, request.RoleId, request.SchoolId, request.TeacherId, request.ClassId,
                                                                   request.TermId, request.GradeIds, request.ProgramIds, request.Roles,
                                                                   request.SortBy, request.SearchText, request.StartIndex, request.PageSize)
                .Select(x => new SurveyGetGridPopulationUserDto() {
                    UserId = x.UserId,
                    FullName = x.FullName,
                    RoleId = x.RoleId,
                    RoleName = x.RoleName,
                    SchoolName = x.SchoolName,
                    TotalRecords = x.TotalRecords,
                    UserName = x.UserName
                }).AsQueryable();
        }
    }
}
