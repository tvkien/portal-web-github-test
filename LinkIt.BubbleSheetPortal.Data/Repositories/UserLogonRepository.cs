using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class UserLogonRepository : IRepository<UserLogon>
    {
        private readonly Table<UserLogonEntity> table;
        private readonly DbDataContext _context;

        public UserLogonRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = DbDataContext.Get(connectionString);
            table = _context.GetTable<UserLogonEntity>();
            
        }

        public IQueryable<UserLogon> Select()
        {
            return table.Select(x => new UserLogon
                                {
                                    UserID = x.UserID,
                                    GUIDSession = x.GUIDSession
                                });
        }

        public void Save(UserLogon item)
        {
            var entity = table.FirstOrDefault(x => x.UserID.Equals(item.UserID));

            if (entity == null)
            {
                entity = new UserLogonEntity();
                table.InsertOnSubmit(entity);
            }
            entity.UserID = item.UserID;
            entity.GUIDSession = item.GUIDSession;

            try
            {
                table.Context.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException) { } // do not remove this line
        }

        public void Delete(UserLogon item)
        {
            var entity = table.FirstOrDefault(x => x.UserID.Equals(item.UserID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

       
    }
}
