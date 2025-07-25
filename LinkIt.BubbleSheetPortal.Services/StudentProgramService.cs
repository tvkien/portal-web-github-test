using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StudentProgramService
    {
        private readonly IStudentProgramRepository _repository;
        public StudentProgramService(IStudentProgramRepository studentProgramRepository)
        {
            this._repository = studentProgramRepository;
        }

        public IQueryable<StudentProgram> GetStudentsProgramsByStudentId(int studentId)
        {
            return _repository.Select().Where(x => x.StudentID.Equals(studentId));
        }

        public void DeleteStudentProgram(StudentProgram studentProgram)
        {
            _repository.Delete(studentProgram);
        }
        public void DeleteStudentProgram(int studentProgramId)
        {
            var entity = _repository.Select().FirstOrDefault(x => x.StudentProgramID == studentProgramId);
            _repository.Delete(entity);
        }

        public StudentProgram GetStudentProgramById(int studentProgramId)
        {
            return _repository.Select().FirstOrDefault(x => x.StudentProgramID.Equals(studentProgramId));
        }

        public void AddStudentProgram(int studentId, int programId)
        {
            var studentProgram = GetStudentProgramByIds(studentId, programId);
            if (studentProgram.IsNull())
            {
                studentProgram = new StudentProgram
                                     {
                                         ProgramID = programId,
                                         StudentID = studentId
                                     };

                _repository.Save(studentProgram);
            }
        }

        public StudentProgram GetStudentProgramByIds(int studentId, int programId)
        {
            return _repository.Select().FirstOrDefault(x => x.ProgramID.Equals(programId) && x.StudentID.Equals(studentId));
        }

        public IQueryable<StudentProgramManage> GetStudentProgramView()
        {
            return _repository.GetStudentPrograms();
        }

        public List<int> GetActiveStudentsOfProgram(int programId, string date)
        {
            return _repository.GetActiveStudentsOfProgram(programId, date);
        }
        public IQueryable<Student> GetUnassignedStudents(int programId, string studentCode, string firstName,
            string lastName, int districtId)
        {
            return _repository.GetUnassignedStudents(programId, studentCode, firstName, lastName, districtId);
        }

        public IQueryable<StudentProgram> GetAll()
        {
            return _repository.Select();
        }
    }
}