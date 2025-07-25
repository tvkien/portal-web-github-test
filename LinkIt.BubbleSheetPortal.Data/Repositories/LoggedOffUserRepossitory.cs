using System;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class LoggedOffUserRepossitory : IRepository<LoggedOffUser>
    {
        private readonly Table<LoggedOffUserEntity> table;

         public LoggedOffUserRepossitory(IConnectionString conn)
         {
             var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<LoggedOffUserEntity>();
            Mapper.CreateMap<LoggedOffUser, LoggedOffUserEntity>();
        }

         public IQueryable<LoggedOffUser> Select()
        {
            return table.Select(x => new LoggedOffUser
                                {
                                     LoggedOffUserID = x.LoggedOffUserID,
                                     CreatedDate = x.CreatedDate,
                                     RedirectURL = x.RedirectURL,
                                     UserID = x.UserID
                                });
        }

        public void Save(LoggedOffUser item)
        {
            var entity = table.FirstOrDefault(x => x.LoggedOffUserID == item.LoggedOffUserID);

            if (entity.IsNull())
            {
                entity = new LoggedOffUserEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.LoggedOffUserID = entity.LoggedOffUserID;
        }

        public void Delete(LoggedOffUser item)
        {
            var entity = table.FirstOrDefault(x => x.LoggedOffUserID == item.LoggedOffUserID);

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
