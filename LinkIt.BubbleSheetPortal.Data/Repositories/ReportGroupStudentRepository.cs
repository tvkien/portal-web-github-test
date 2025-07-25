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
    public class ReportGroupStudentRepository : IReadOnlyRepository<ReportGroupStudent>, IInsertDeleteRepository<ReportGroupStudent>
    {
        private readonly Table<ReportGroupStudentEntity> table;
        private readonly ParentDataContext _parentDataContext;

        public ReportGroupStudentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = ParentDataContext.Get(connectionString).GetTable<ReportGroupStudentEntity>();
            _parentDataContext = ParentDataContext.Get(connectionString);
        }

        #region Implementation of IReadOnlyRepository

        public IQueryable<ReportGroupStudent> Select()
        {
            return table.Select(x => new ReportGroupStudent
            {
                 ReportGroupId = x.ReportGroupID,
                 ReportGroupStudentId = x.ReportGroupStudentID,
                 StudentId = x.StudentID
            });
        }

        #endregion
        
        #region Implementation of IInsertDeleteRepository

        public void Save(ReportGroupStudent item)
        {
            var entity = table.FirstOrDefault(x => x.ReportGroupStudentID.Equals(item.ReportGroupStudentId));
            if (entity.IsNull())
            {
                entity = new ReportGroupStudentEntity();
                table.InsertOnSubmit(entity);
            }
            BindItem(entity, item);
            table.Context.SubmitChanges();
        }

        public void Delete(ReportGroupStudent item)
        {
            var entity = table.FirstOrDefault(x => x.ReportGroupID.Equals(item.ReportGroupId));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        #endregion

        private void BindItem(ReportGroupStudentEntity entity, ReportGroupStudent item)
        {
            entity.ReportGroupID = item.ReportGroupId;
            entity.StudentID = item.StudentId;
        }

        public void InsertMultipleRecord(List<ReportGroupStudent> items)
        {
            foreach (var item in items)
            {
                var entity = new ReportGroupStudentEntity();
                BindItem(entity, item);
                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
