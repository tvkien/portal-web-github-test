using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIItemLessonOneRepository  : IRepository<QTIItemLessonOne>
    {
        private readonly Table<QTIItemLessonOneEntity> table;
        private readonly Table<QTIItemLessonOneView> view;

        public QTIItemLessonOneRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTIItemLessonOneEntity>();
            view = AssessmentDataContext.Get(connectionString).GetTable<QTIItemLessonOneView>();
            Mapper.CreateMap<QTIItemLessonOne, QTIItemLessonOneEntity>();
        }

        public IQueryable<QTIItemLessonOne> Select()
        {
            return view.Select(x => new QTIItemLessonOne
                {
                    QTIItemLessonOneID = x.QTIItemLessonOneID,
                    QTIItemID = x.QTIItemID,
                    LessonOneID = x.LessonOneId,
                    Name = x.Name
                });
        }
        public void Save(QTIItemLessonOne item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemLessonOneID.Equals(item.QTIItemLessonOneID));

            if (entity.IsNull())
            {
                entity = new QTIItemLessonOneEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.QTIItemLessonOneID = entity.QTIItemLessonOneID;
        }

        public void Delete(QTIItemLessonOne item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemLessonOneID.Equals(item.QTIItemLessonOneID));

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}