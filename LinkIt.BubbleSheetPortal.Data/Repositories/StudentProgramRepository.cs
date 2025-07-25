using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StudentProgramRepository : IStudentProgramRepository
    {
        private readonly Table<StudentProgramEntity> studentProgramEntityTable;
        private readonly Table<StudentProgramView> studentProgramViewTable;
        private readonly Table<StudentProgramManageView> view;
        private StudentDataContext datacontext;
        public StudentProgramRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            datacontext = StudentDataContext.Get(connectionString);
            studentProgramEntityTable = datacontext.GetTable<StudentProgramEntity>();
            studentProgramViewTable = datacontext.GetTable<StudentProgramView>();
            view = datacontext.GetTable<StudentProgramManageView>();
        }

        public IQueryable<StudentProgram> Select()
        {
            return studentProgramViewTable.Select(x => new StudentProgram
                {
                    StudentProgramID = x.StudentProgramID,
                    ProgramID = x.ProgramID,
                    StudentID = x.StudentID,
                    ProgramName = x.Name,
                    AccessLevelId = x.AccessLevelID
                });
        }

        public void Save(StudentProgram item)
        {
            var entity = studentProgramEntityTable.FirstOrDefault(x => x.StudentProgramID.Equals(item.StudentProgramID));

            if (entity.IsNull())
            {
                entity = new StudentProgramEntity();
                studentProgramEntityTable.InsertOnSubmit(entity);
            }
            BindStudentProgramEntityToStudentProgramItem(entity, item);
            studentProgramEntityTable.Context.SubmitChanges();
        }

        public void Delete(StudentProgram item)
        {
            var entity = studentProgramEntityTable.FirstOrDefault(x => x.StudentProgramID.Equals(item.StudentProgramID));
            if (entity.IsNotNull())
            {
                studentProgramEntityTable.DeleteOnSubmit(entity);
                studentProgramEntityTable.Context.SubmitChanges();
            }
        }

        private void BindStudentProgramEntityToStudentProgramItem(StudentProgramEntity entity, StudentProgram item)
        {
            entity.StudentProgramID = item.StudentProgramID;
            entity.StudentID = item.StudentID;
            entity.ProgramID = item.ProgramID;
        }
        public IQueryable<StudentProgramManage> GetStudentPrograms()
        {
            return view.Select(x => new StudentProgramManage
            {
                StudentProgramID = x.StudentProgramID,
                ProgramID = x.ProgramID,
                Name = x.Name,
                Code = x.Code,
                DistrictID = x.DistrictID,
                StudentID = x.StudentID,
                StudentCode = x.StudentCode,
                AltCode = x.AltCode,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
                GenderID = x.GenderID,
                RaceID = x.RaceID,
                Email = x.Email,
                Phone = x.Phone,
                LoginCode = x.LoginCode,
                Dateofbirth = x.Dateofbirth,
                PrimaryLanguageID = x.PrimaryLanguageID,
                Status = x.Status,
                SISID = x.SISID,
                StateCode = x.StateCode,
                Note01 = x.Note01,
                CurrentGradeID = x.CurrentGradeID,
                AdminSchoolID = x.AdminSchoolID
            });
        }
        public IQueryable<Student> GetUnassignedStudents(int programId, string studentCode, string firstName, string lastName, int districtId)
        {
            return datacontext.GetUnassignedStudents(programId, studentCode, firstName, lastName, districtId)
                .Select(x => new Student
            {
                Id = x.StudentID,
                Code = x.StudentCode,             
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
            }).AsQueryable();
        }
        public List<int> GetActiveStudentsOfProgram(int programId, string date)
        {
            return datacontext.GetActiveStudentsOfProgram(programId, date).Select(x => x.StudentID).ToList();
        }

    }
}