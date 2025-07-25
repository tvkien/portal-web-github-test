using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassUserRepository : IRepository<ClassUser>
    {
        private readonly Table<ClassUserEntity> table;

        public ClassUserRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassUserEntity>();
        }

        public IQueryable<ClassUser> Select()
        {
            return table.Select(x => new ClassUser
                                         {
                                             Id = x.ClassUserID,
                                             ClassId = x.ClassID,
                                             ClassUserLOEId = x.ClassUserLOEID,
                                             UserId = x.UserID
                                         });
        }

        public void Save(ClassUser item)
        {
            var entity = table.FirstOrDefault(x => x.ClassUserID.Equals(item.Id));

            if (entity.IsNull())
            {
                entity = new ClassUserEntity();
                table.InsertOnSubmit(entity);
            }

            BindEntityToItem(entity, item);
            table.Context.SubmitChanges();
            item.Id = entity.ClassUserID;
        }

        private void BindEntityToItem(ClassUserEntity entity, ClassUser item)
        {
            entity.ClassID = item.ClassId;
            entity.UserID = item.UserId;
            entity.ClassUserLOEID = item.ClassUserLOEId.GetValueOrDefault();
        }

        public void Delete(ClassUser item)
        {
            var entity = table.FirstOrDefault(x => x.ClassUserID.Equals(item.Id));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
