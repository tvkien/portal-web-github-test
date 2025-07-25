using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SchoolStudentRepository : IReadOnlyRepository<SchoolStudent>
    {
        private readonly Table<SchoolStudentView> table;

        public SchoolStudentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<SchoolStudentView>();
        }

        public IQueryable<SchoolStudent> Select()
        {
            return table.Select(x => new SchoolStudent
                {
                   StudentID = x.StudentID,
                   SchoolID = x.SchoolID,
                   Active = x.Active,
                   DateEnd = x.DateEnd,
                   DateStart = x.DateStart,
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   MiddleName = x.MiddleName,
                   Code = x.Code,
                   AltCode = x.AltCode ,
                   StateCode = x.StateCode,
                   Gender = x.Gender,
                   Grade = x.Grade
                });
        }
    }
}