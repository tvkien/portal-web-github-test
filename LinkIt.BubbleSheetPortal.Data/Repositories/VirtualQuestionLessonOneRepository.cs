using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualQuestionLessonOneRepository : IVirtualQuestionLessonOneRepository
    {
        private readonly Table<VirtualQuestionLessonOneEntity> table;
        private readonly Table<VirtualQuestionLessonOneView> view;

        public VirtualQuestionLessonOneRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<VirtualQuestionLessonOneEntity>();
            view = dataContext.GetTable<VirtualQuestionLessonOneView>();
        }

        public IQueryable<VirtualQuestionLessonOne> Select()
        {
            return view.Select(x => new VirtualQuestionLessonOne
                {
                    VirtualQuestionLessonOneId = x.VirtualQuestionLessonOneID,
                    VirtualQuestionId = x.VirtualQuestionID,
                    VirtualTestId = x.VirtualTestID,
                    LessonOneId = x.LessonOneId,
                    Name = x.Name
                });
        }

        public void Save(VirtualQuestionLessonOne item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionLessonOneID.Equals(item.VirtualQuestionLessonOneId));

            if (entity == null)
            {
                entity = new VirtualQuestionLessonOneEntity();
                table.InsertOnSubmit(entity);
            }
            entity.VirtualQuestionID = item.VirtualQuestionId;
            entity.LessonOneID = item.LessonOneId;

            table.Context.SubmitChanges();
            item.VirtualQuestionLessonOneId = entity.VirtualQuestionLessonOneID;
        }

        public void Delete(VirtualQuestionLessonOne item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionLessonOneID.Equals(item.VirtualQuestionLessonOneId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void InsertMultipleRecord(List<VirtualQuestionLessonOne> items)
        {
            foreach (var item in items)
            {
                var entity = new VirtualQuestionLessonOneEntity
                {
                    LessonOneID = item.LessonOneId,
                    VirtualQuestionID = item.VirtualQuestionId,
                };

                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
