using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemoryVirtualTestDistrictRepository : IVirtualTestDistrictRepository
    {
         private readonly List<VirtualTestDistrict> table = new List<VirtualTestDistrict>();

         public InMemoryVirtualTestDistrictRepository()
        {
            table = AddVirtualTestDistrict();
        }

        private List<VirtualTestDistrict> AddVirtualTestDistrict()
        {
            return new List<VirtualTestDistrict>
                    {                           
                        new VirtualTestDistrict{ClassId = 1, DistrictId = 272, Name = "GK 0708DRA 02 Winter 1",VirtualTestId = 1, VirtualTestSourceId = 3},   
                        new VirtualTestDistrict{ClassId = 2, DistrictId = 272, Name = "GK 0708DRA 02 Winter 2",VirtualTestId = 2, VirtualTestSourceId = 1},
                        new VirtualTestDistrict{ClassId = 3, DistrictId = 272, Name = "GK 0708DRA 02 Winter 3",VirtualTestId = 3, VirtualTestSourceId = 3},
                        new VirtualTestDistrict{ClassId = 4, DistrictId = 272, Name = "GK 0708DRA 02 Winter 4",VirtualTestId = 4, VirtualTestSourceId = 2},
                        new VirtualTestDistrict{ClassId = 5, DistrictId = 272, Name = "GK 0708DRA 02 Winter 5",VirtualTestId = 5, VirtualTestSourceId = 3},
                        new VirtualTestDistrict{ClassId = 6, DistrictId = 272, Name = "GK 0708DRA 02 Winter 6",VirtualTestId = 6, VirtualTestSourceId = 3},
                        new VirtualTestDistrict{ClassId = 7, DistrictId = 272, Name = "GK 0708DRA 02 Winter 7",VirtualTestId = 7, VirtualTestSourceId = 3},
                        new VirtualTestDistrict{ClassId = 8, DistrictId = 272, Name = "GK 0708DRA 02 Winter 8",VirtualTestId = 8, VirtualTestSourceId = 4},
                        new VirtualTestDistrict{ClassId = 9, DistrictId = 272, Name = "GK 0708DRA 02 Winter 9",VirtualTestId = 9, VirtualTestSourceId = 3},
                        new VirtualTestDistrict{ClassId = 10, DistrictId = 272, Name = "GK 0708DRA 02 Winter 10",VirtualTestId = 10, VirtualTestSourceId = 5},
                        new VirtualTestDistrict{ClassId = 11, DistrictId = 272, Name = "GK 0708DRA 02 Winter 11",VirtualTestId = 11, VirtualTestSourceId = 3}
                    };
        }

        public IQueryable<VirtualTestDistrict> Select()
        {
            return table.AsQueryable();
        }

        public IQueryable<VirtualTestDistrict> GetVirtualTestDistricts(int districtId, int userId, int roleId,
            int? schoolId, int? teacherId, int? classId, int? studentId, int isRegrader)
        {
            return null;
        }

        public IQueryable<ClassDistrict> GetClassDistrictByRole(int districtId, int userId, int roleId,
            int? virtualtestId, int? studentId, int? SchoolId, int? teacherId, int isRegrader)
        {
            return null;
        }

        public IQueryable<StudentTestResultDistrict> GetStudentDistrictByRole(int districtId, int userId, int roleId,
            int? schoolId, int? teacherId, int? classId, int? virtualTestId, int isRegrader)
        {
            return null;
        }

        public IQueryable<SchoolTestResultDistrict> GetSchoolDistrictByRole(int districtId, int userId, int roleId,
            int? teacherId, int? classId, int? studentId, int? virtualTestId, int isRegrader)
        {
            return null;
        }

        public IQueryable<TeacherTestResultDistrict> GetPrimaryTeacherDistrictByRole(int districtId, int userId,
            int roleId,
            int? schoolid, int? classId, int? studentId, int? virtualTestId, int isRegrader)
        {
            return null;
        }

        public IQueryable<DisplayTestResultDistrict> GetTestResutDistrictProcByRole(int districtId, int userId,
            int roleId,int? schoolid, int? teacherId, int? classId, int? studentId, int? virtualTestId, int termId, int pageIndex,int pageSize,ref int? totalRecords, string sortColumns, int isRegrader)
        {
            return null;
        }

        public IQueryable<TermDistrict> GetTermDistrictByRole(int districtId, int userId, int roleId, int virtualTestId,
            int studentId, int classId, int schoolId, int teacherId, int isRegrader)
        {
            return null;
        }

        public IQueryable<TestResultLog> GetTestResultDetails(string testresultIds)
        {
            return null;
        }

        public IQueryable<TestResultScoreLog> GetTestResultScores(string testresultIds)
        {
            return null;
        }

        public IQueryable<TestResultSubScoreLog> GetTestResultSubScores(string listTestResultScoreId)
        {
            return null;
        }

        public IQueryable<TestResultProgramLog> GetTestResultProgram(string testresultIds)
        {
            return null;
        }

        public IQueryable<AnswerLog> GetAnswersByTestResultId(string testresultIds)
        {
            return null;
        }

        public IQueryable<AnswerSubLog> GetAnswerSubsByAnswerId(string listAnswerId)
        {
            return null;
        }

        public IQueryable<DisplayTestResultDistrict> GetTestResutRetaggedProcByRole(int districtId, int userId, int roleId, int? schoolid, int? teacherId,
            int? classId, int? virtualTestId, int termId, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns)
        {
            throw new NotImplementedException();
        }

        public bool CheckUserCanPurgeTest(int userID, int roleID, int virtualTestID)
        {
            throw new NotImplementedException();
        }
    }
}
