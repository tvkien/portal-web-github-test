using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryUserStudentRepository : IReadOnlyRepository<UserStudent>
    {
        private List<UserStudent> table;

        public InMemoryUserStudentRepository()
        {
            table = AddUserStudent();
        }

        private List<UserStudent> AddUserStudent()
        {
            return new List<UserStudent>
                       {
                           new UserStudent{ ClassID = 1, ClassName = "Class 1", StudentID = 1, SchoolID = 1, TeacherID = 1, UserID = 1},
                           new UserStudent{ ClassID = 2, ClassName = "Class 2", StudentID = 2, SchoolID = 2, TeacherID = 2, UserID = 2}
                       };
        }

        public IQueryable<UserStudent> Select()
        {
            return table.AsQueryable();
        }
    }
}