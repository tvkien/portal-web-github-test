using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class UserMetaRepository : IRepository<UserMeta>
    {
        private readonly Table<UserMetaEntity> table;

        public UserMetaRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<UserMetaEntity>();
        }

        public IQueryable<UserMeta> Select()
        {
            return table.Select(x => new UserMeta
            {
                MetaValue = x.MetaValue,
                MetaLabel = x.MetaLabel,
                UserId = x.UserID,
                UserMetaId = x.UserMetaID
            });
        }

        public void Save(UserMeta item)
        {
            var entity = table.FirstOrDefault(x => x.UserMetaID.Equals(item.UserMetaId));

            if (entity.IsNull())
            {
                entity = new UserMetaEntity();
                table.InsertOnSubmit(entity);
            }

            BindEntityToItem(entity, item);
            table.Context.SubmitChanges();
            item.UserMetaId = entity.UserMetaID;
        }

        private void BindEntityToItem(UserMetaEntity entity, UserMeta item)
        {
            entity.MetaValue = item.MetaValue;
            entity.MetaLabel = item.MetaLabel;
            entity.UserID = item.UserId;
        }

        public void Delete(UserMeta item)
        {
            var entity = table.FirstOrDefault(x => x.UserMetaID.Equals(item.UserMetaId));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
