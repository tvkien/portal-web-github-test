using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TeacherClassStudentService
    {
        private readonly IReadOnlyRepository<TeacherClassStudent> repository;

        public TeacherClassStudentService(IReadOnlyRepository<TeacherClassStudent> repository)
        {
            this.repository = repository;
        }

        public List<int> GetListStudents(int teacherId)
        {
            return repository.Select().Where(o => o.UserId == teacherId).Select(o=>o.StudentId).ToList();
        }

    }
}
