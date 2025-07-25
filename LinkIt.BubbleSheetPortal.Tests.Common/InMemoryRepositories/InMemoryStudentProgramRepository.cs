using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryStudentProgramRepository : IReadOnlyRepository<StudentProgram>
    {
        private List<StudentProgram> table;

        public InMemoryStudentProgramRepository()
        {
            table = AddStudentProgram();
        }

        private List<StudentProgram> AddStudentProgram()
        {
            return new List<StudentProgram>
                       {
                           new StudentProgram{ StudentID = 1, ProgramID = 1, ProgramName = "Some Program", StudentProgramID = 1},
                           new StudentProgram{ StudentID = 2, ProgramID = 2, ProgramName = "Program 2", StudentProgramID = 2}
                       };
        }

        public IQueryable<StudentProgram> Select()
        {
            return table.AsQueryable();
        }
    }
}