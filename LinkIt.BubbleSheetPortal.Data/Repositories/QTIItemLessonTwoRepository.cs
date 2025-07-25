using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIItemLessonTwoRepository  : IRepository<QTIItemLessonTwo>
    {
        private readonly Table<QTIItemLessonTwoEntity> table;
        private readonly Table<QTIItemLessonTwoView> view;

        public QTIItemLessonTwoRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTIItemLessonTwoEntity>();
            view = AssessmentDataContext.Get(connectionString).GetTable<QTIItemLessonTwoView>();
            Mapper.CreateMap<QTIItemLessonTwo, QTIItemLessonTwoEntity>();
        }

        public IQueryable<QTIItemLessonTwo> Select()
        {
            return view.Select(x => new QTIItemLessonTwo
                {
                    QTIItemLessonTwoID = x.QTIItemLessonTwoID,
                    QTIItemID = x.QTIItemID,
                    LessonTwoID = x.LessonTwoId,
                    Name = x.Name
                });
        }
        public void Save(QTIItemLessonTwo item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemLessonTwoID.Equals(item.QTIItemLessonTwoID));

            if (entity.IsNull())
            {
                entity = new QTIItemLessonTwoEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.QTIItemLessonTwoID = entity.QTIItemLessonTwoID;
        }

        public void Delete(QTIItemLessonTwo item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemLessonTwoID.Equals(item.QTIItemLessonTwoID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}