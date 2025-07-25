using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AuthorGroupUserRepository : IRepository<AuthorGroupUser>
    {
        private readonly Table<AuthorGroupUserEntity> table;
        private readonly AssessmentDataContext dbContext;

        public AuthorGroupUserRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<AuthorGroupUserEntity>();
            dbContext = AssessmentDataContext.Get(connectionString);

            Mapper.CreateMap<AuthorGroupUser, AuthorGroupUserEntity>();
        }

        public IQueryable<AuthorGroupUser> Select()
        {
            return table.Select(x => new AuthorGroupUser
                                     {
                                         AuthorGroupId = x.AuthorGroupID,
                                         UserId = x.UserID
                                     });
        }

        public void Save(AuthorGroupUser item)
        {
            var entity =
                table.FirstOrDefault(x => x.AuthorGroupID.Equals(item.AuthorGroupId) && x.UserID.Equals(item.UserId));

            if (entity.IsNull())
            {
                entity = new AuthorGroupUserEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
        }

        public void Delete(AuthorGroupUser item)
        {
            var entity =
                table.FirstOrDefault(x => x.AuthorGroupID.Equals(item.AuthorGroupId) && x.UserID.Equals(item.UserId));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}