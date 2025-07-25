using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.UserGuide;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class UserSecurityCodeRepository : IRepository<UserSecurityCodeData>
    {
        private readonly Table<UserSecurityCodeEntity> table;

        public UserSecurityCodeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = TestDataContext.Get(connectionString);
            table = dataContext.GetTable<UserSecurityCodeEntity>();
        }

        public IQueryable<UserSecurityCodeData> Select()
        {
            return table.Select(x => new UserSecurityCodeData
                {
                    UserSecurityCodeID = x.UserSecurityCodeID,
                    UserID = x.UserID,
                    IssueDate = x.IssueDate,
                    Expired = x.Expired,
                    Email = x.Email,
                    Code = x.Code
                });
        }

        public void Save(UserSecurityCodeData item)
        {
            var entity = table.FirstOrDefault(x => x.UserSecurityCodeID.Equals(item.UserSecurityCodeID));

            if (entity == null)
            {
                entity = new UserSecurityCodeEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.UserSecurityCodeID = entity.UserSecurityCodeID;
        }

        public void Delete(UserSecurityCodeData item)
        {
            throw new NotImplementedException();
        }

        private void MapModelToEntity(UserSecurityCodeEntity entity, UserSecurityCodeData item)
        {
            entity.UserID = item.UserID;
            entity.IssueDate = item.IssueDate;
            entity.Expired = item.Expired;
            entity.Code = item.Code;
            entity.Email = item.Email;
        }
    }
}