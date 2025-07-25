using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AuthorGroupSchoolRepository : IRepository<AuthorGroupSchool>
    {
        private readonly Table<AuthorGroupSchoolEntity> table;
        private readonly AssessmentDataContext dbContext;

        public AuthorGroupSchoolRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<AuthorGroupSchoolEntity>();
            dbContext = AssessmentDataContext.Get(connectionString);

            Mapper.CreateMap<AuthorGroupSchool, AuthorGroupSchoolEntity>();
        }

        public IQueryable<AuthorGroupSchool> Select()
        {
            return table.Select(x => new AuthorGroupSchool
            {
                AuthorGroupId = x.AuthorGroupID,
                SchoolId = x.SchoolID
            });
        }

        public void Save(AuthorGroupSchool item)
        {
            var entity =
                table.FirstOrDefault(x => x.AuthorGroupID.Equals(item.AuthorGroupId) && x.SchoolID.Equals(item.SchoolId));

            if (entity.IsNull())
            {
                entity = new AuthorGroupSchoolEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
        }

        public void Delete(AuthorGroupSchool item)
        {
            var entity =
                table.FirstOrDefault(x => x.AuthorGroupID.Equals(item.AuthorGroupId) && x.SchoolID.Equals(item.SchoolId));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();

            }
        }
    }
}