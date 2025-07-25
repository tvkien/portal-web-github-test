using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ReportGroupRepository : IReadOnlyRepository<ReportGroup>, IInsertDeleteRepository<ReportGroup>
    {
        private readonly Table<ReportGroupEntity> table;
        private readonly ParentDataContext _parentDataContext;

        public ReportGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = ParentDataContext.Get(connectionString).GetTable<ReportGroupEntity>();
            _parentDataContext = ParentDataContext.Get(connectionString);
        }

        #region Implementation of IReadOnlyRepository<StudentParentRepository>

        public IQueryable<ReportGroup> Select()
        {
            return table.Select(x => new ReportGroup
            {
                 CreatedDateTime = x.CreatedDateTime.Value,
                 GroupType = x.GroupType,
                 Name = x.Name,
                 ReportGroupId = x.ReportGroupID,
                 UserId = x.UserID
            });
        }

        #endregion
        
        #region Implementation of IInsertDeleteRepository<StudentParent>

        public void Save(ReportGroup item)
        {
            var entity = table.FirstOrDefault(x => x.ReportGroupID.Equals(item.ReportGroupId));
            if (entity.IsNull())
            {
                entity = new ReportGroupEntity();
                table.InsertOnSubmit(entity);
            }
            BindItem(entity, item);
            table.Context.SubmitChanges();
            item.ReportGroupId = entity.ReportGroupID;
        }

        public void Delete(ReportGroup item)
        {
            var entity = table.FirstOrDefault(x => x.ReportGroupID.Equals(item.ReportGroupId));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        #endregion

        private void BindItem(ReportGroupEntity entity, ReportGroup item)
        {
            if (entity.ReportGroupID == 0)
                entity.CreatedDateTime = DateTime.Now;

            entity.GroupType = item.GroupType;
            entity.Name = item.Name;
            entity.UserID = item.UserId;
        }

        public void InsertMultipleRecord(List<ReportGroup> items)
        {
            foreach (var item in items)
            {
                var entity = new ReportGroupEntity();
                BindItem(entity, item);
                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
