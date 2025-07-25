using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class ClassAdminSchoolRepository : IReadOnlyRepository<ClassAdminSchool>
    {
        private readonly Table<ClassAdminSchoolView> _table;
        public ClassAdminSchoolRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<ClassAdminSchoolView>();
        }

        public IQueryable<ClassAdminSchool> Select()
        {
            return _table.Select(x => new ClassAdminSchool
                                    {
                                        UserId = x.UserID,
                                        ClassId = x.ClassID,
                                        SchoolId = x.SchoolID,
                                        ClassName = x.Name
                                    });
        } 
    }
}
