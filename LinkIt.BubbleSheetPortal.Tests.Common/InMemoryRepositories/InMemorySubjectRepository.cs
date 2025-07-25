using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemorySubjectRepository : ISubjectRepository
    {
        private readonly List<Subject> table;

        public InMemorySubjectRepository()
        {
            table = AddSubjects();
        }

        private List<Subject> AddSubjects()
        {
            return new List<Subject>
                       {
                           new Subject{ Id = 1, GradeId = 1, Name = "Math", ShortName = "M", StateId = 4 },
                           new Subject{ Id = 2, GradeId = 1, Name = "Reading", ShortName = "R", StateId = 4 },
                           new Subject{ Id = 3, GradeId = 2, Name = "Language Arts", ShortName = "LA", StateId = 2 },
                           new Subject{ Id = 4, GradeId = 4, Name = "Science", ShortName = "S", StateId = 1 },
                           new Subject{ Id = 5, GradeId = 4, Name = "Math", ShortName = "M", StateId = 4 }
                       };
        }

        public IQueryable<Subject> Select()
        {
            return table.AsQueryable();
        }

        public List<Subject> GetSubjectSByGradeId(int gradeId, int districtId, int userId, int userRole)
        {
            //TODO 
            return null;
        }

        public List<Subject> ACTGetSubjectSByGradeId(int gradeId, int districtId, int userId, int userRole)
        {
            return null;
        }
        public List<Subject> GetSubjectsForItemSetSaveTest(int gradeId, int districtId, int userId, int userRole)
        {
            //TODO 
            return null;
        }

        public List<Subject> GetSubjectSByGradeIdAndAuthor(SearchBankCriteria criteria)
        {
            return null;
        }
        
        public List<Subject> SATGetSubjectSByGradeId(int gradeId, int districtId, int userId, int userRole)
        {
            return null;
        }
    }
}
