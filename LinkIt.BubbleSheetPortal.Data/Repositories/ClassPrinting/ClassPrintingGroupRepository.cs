using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.ClassPrinting
{
    public class ClassPrintingGroupRepository : IClassPrintingGroupRepository
    {
        private readonly DbDataContext _dbContext;
        private readonly Table<ClassPrintingGroupEntity> _table;

        public ClassPrintingGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _dbContext = DbDataContext.Get(connectionString);
            _table = _dbContext.GetTable<ClassPrintingGroupEntity>();
        }

        public IQueryable<ClassPrintingGroup> Select()
        {
            return _table.Select(x => new ClassPrintingGroup
            {
                ClassGroupID = x.ClassGroupID,
                ClassID = x.ClassID,
                GroupID = x.GroupID,
                UserId = x.UserID
            });
        }

        public void Save(ClassPrintingGroup item)
        {
            var entity = _table.FirstOrDefault(x => x.ClassID.Equals(item.ClassID) && x.GroupID.Equals(item.GroupID));

            if (entity.IsNull())
            {
                entity = new ClassPrintingGroupEntity
                {
                    ClassID = item.ClassID,
                    GroupID = item.GroupID,
                    UserID = item.UserId
                };
                _table.InsertOnSubmit(entity);
                _table.Context.SubmitChanges();
            }
            item.ClassGroupID = entity.ClassGroupID;
        }

        public void Delete(ClassPrintingGroup item)
        {
            var entity = _table.FirstOrDefault(x => x.ClassID.Equals(item.ClassID) && x.GroupID.Equals(item.GroupID));

            if (entity.IsNotNull())
            {
                _table.DeleteOnSubmit(entity);
                _table.Context.SubmitChanges();
            }
        }

        public int CountActiveStudentInGroup(int groupId)
        {
            return _dbContext.CountActiveStudentInGroup(groupId).FirstOrDefault()?.Count ?? 0;
        }
    }
}
