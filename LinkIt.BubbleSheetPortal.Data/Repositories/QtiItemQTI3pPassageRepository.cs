using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QtiItemQTI3pPassageRepository : IRepository<QtiItemQTI3pPassage>
    {
        private readonly Table<QtiItemQTI3pPassageEntity> table;
        private readonly AssessmentDataContext _assessmentDataContext;

        public QtiItemQTI3pPassageRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<QtiItemQTI3pPassageEntity>();

            _assessmentDataContext = AssessmentDataContext.Get(connectionString);
        }

        public IQueryable<QtiItemQTI3pPassage> Select()
        {
            return table
                .Select(x => new QtiItemQTI3pPassage
                {
                    QtiItemQTI3pPassageID = x.QtiItemQTI3pPassageID,
                    QtiItemId =  x.QtiItemId.GetValueOrDefault(),
                    QTI3pPassageId = x.QTI3pPassageId.GetValueOrDefault()
                }).AsQueryable();
        }


        public void Save(QtiItemQTI3pPassage item)
        {
            var entity = table.FirstOrDefault(x => x.QtiItemQTI3pPassageID == item.QtiItemQTI3pPassageID);

            if (entity == null)
            {
                entity = new QtiItemQTI3pPassageEntity();
                table.InsertOnSubmit(entity);
            }
            entity.QTI3pPassageId = item.QTI3pPassageId;
            entity.QtiItemId = item.QtiItemId;

            table.Context.SubmitChanges();

            item.QtiItemQTI3pPassageID = entity.QtiItemQTI3pPassageID;
        }

        public void Delete(QtiItemQTI3pPassage item)
        {
            var entity = table.FirstOrDefault(x => x.QtiItemQTI3pPassageID.Equals(item.QtiItemQTI3pPassageID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

     

    }
}