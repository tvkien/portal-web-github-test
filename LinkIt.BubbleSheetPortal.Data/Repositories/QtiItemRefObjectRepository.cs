using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QtiItemRefObjectRepository : IRepository<QtiItemRefObject>
    {
        private readonly Table<QtiItemRefObjectEntity> table;
        private readonly AssessmentDataContext _assessmentDataContext;

        public QtiItemRefObjectRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<QtiItemRefObjectEntity>();

            _assessmentDataContext = AssessmentDataContext.Get(connectionString);
        }

        public IQueryable<QtiItemRefObject> Select()
        {
            return table
                .Select(x => new QtiItemRefObject
                {
                   QtiItemRefObjectId = x.QtiItemRefObjectID,
                   QtiItemId =  x.QtiItemId.GetValueOrDefault(),
                   QtiRefObjectId = x.QtiRefObjectId.GetValueOrDefault()
                }).AsQueryable();
        }


        public void Save(QtiItemRefObject item)
        {
            var entity = table.FirstOrDefault(x => x.QtiItemRefObjectID == item.QtiItemRefObjectId);

            if (entity == null)
            {
                entity = new QtiItemRefObjectEntity();
                table.InsertOnSubmit(entity);
            }
            entity.QtiRefObjectId = item.QtiRefObjectId;
            entity.QtiItemId = item.QtiItemId;

            table.Context.SubmitChanges();

            item.QtiItemRefObjectId = entity.QtiItemRefObjectID;
        }

        public void Delete(QtiItemRefObject item)
        {
            var entity = table.FirstOrDefault(x => x.QtiItemRefObjectID.Equals(item.QtiItemRefObjectId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

    }
}
