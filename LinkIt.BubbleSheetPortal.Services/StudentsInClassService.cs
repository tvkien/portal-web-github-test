using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StudentsInClassService
    {
        private readonly IReadOnlyRepository<StudentInClass> repository;

        public StudentsInClassService(IReadOnlyRepository<StudentInClass> repository)
        {
            this.repository = repository;
        }

        public IQueryable<StudentInClass> GetAllStudentInClass(int classId)
        {
            return repository.Select().Where(s => s.Active.GetValueOrDefault() && s.ClassID.Equals(classId));
        }

        public IQueryable<StudentInClass> GetAllStudentInClassRegardlessStatus(int classId)
        {
            return repository.Select().Where(s => s.ClassID.Equals(classId));
        }        
    }
}