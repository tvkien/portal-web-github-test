using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StudentGradeHistoryService
    {
        private readonly IRepository<StudentGradeHistory> _studentGradeHistoryRepository;
        public StudentGradeHistoryService(IRepository<StudentGradeHistory> studentGradeHistoryRepository)
        {
            this._studentGradeHistoryRepository = studentGradeHistoryRepository;
        }

        public void Save(Student student)
        {
            var studentGradeHistory = _studentGradeHistoryRepository.Select().FirstOrDefault(s => s.StudentID.Equals(student.Id) && s.DateEnd == null);
            var studentGradeHistoryNew = new StudentGradeHistory()
            {
                StudentID = student.Id,
                GradeID = student.CurrentGradeId.Value,
                DateStart = DateTime.UtcNow
            };
            if (studentGradeHistory == null)
            {
                _studentGradeHistoryRepository.Save(studentGradeHistoryNew);
            }
            else if (studentGradeHistory.GradeID != student.CurrentGradeId)
            {
                studentGradeHistory.DateEnd = DateTime.UtcNow;
                _studentGradeHistoryRepository.Save(studentGradeHistory);
                _studentGradeHistoryRepository.Save(studentGradeHistoryNew);
            }
        }
    }
}
