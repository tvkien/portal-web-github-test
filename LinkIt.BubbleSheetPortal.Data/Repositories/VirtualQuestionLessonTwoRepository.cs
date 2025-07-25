using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualQuestionLessonTwoRepository : IVirtualQuestionLessonTwoRepository
    {
        private readonly Table<VirtualQuestionLessonTwoEntity> table;
        private readonly Table<VirtualQuestionLessonTwoView> view;

        public VirtualQuestionLessonTwoRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<VirtualQuestionLessonTwoEntity>();
            view = dataContext.GetTable<VirtualQuestionLessonTwoView>();
        }

        public IQueryable<VirtualQuestionLessonTwo> Select()
        {
            return view.Select(x => new VirtualQuestionLessonTwo
                {
                    VirtualQuestionLessonTwoId = x.VirtualQuestionLessonTwoID,
                    VirtualQuestionId = x.VirtualQuestionID,
                    VirtualTestId = x.VirtualTestID,
                    LessonTwoId = x.LessonTwoId,
                    Name = x.Name
                });
        }

        public void Save(VirtualQuestionLessonTwo item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionLessonTwoID.Equals(item.VirtualQuestionLessonTwoId));

            if (entity == null)
            {
                entity = new VirtualQuestionLessonTwoEntity();
                table.InsertOnSubmit(entity);
            }
            entity.VirtualQuestionID = item.VirtualQuestionId;
            entity.LessonTwoID = item.LessonTwoId;

            table.Context.SubmitChanges();
            item.VirtualQuestionLessonTwoId = entity.VirtualQuestionLessonTwoID;
        }

        public void Delete(VirtualQuestionLessonTwo item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionLessonTwoID.Equals(item.VirtualQuestionLessonTwoId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void InsertMultipleRecord(List<VirtualQuestionLessonTwo> items)
        {
            foreach (var item in items)
            {
                var entity = new VirtualQuestionLessonTwoEntity
                {
                    VirtualQuestionID = item.VirtualQuestionId,
                    LessonTwoID = item.LessonTwoId
                };

                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
