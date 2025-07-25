using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TestAssignmentRepository : IRepository<TestAssignment>
    {
        private readonly Table<TestAssignmentEntity> table;

        public TestAssignmentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<TestAssignmentEntity>();
        }

        public IQueryable<TestAssignment> Select()
        {
            return table.Select(x => new TestAssignment
                {
                    AssignedByUserId = x.AssignedByUserID,
                    AvailableDateTime = x.AvailableDateTime,
                    ClassId = x.ClassID,
                    ClosingDateTime = x.ClosingDateTime,
                    StudentId = x.StudentID,
                    TestAssignmentId = x.TestAssignmentID,
                    TestTaken = x.TestTaken,
                    VirtualTestId = x.VirtualTestID
                });
        }

        public void Save(TestAssignment item)
        {
            var entity = table.FirstOrDefault(x => x.TestAssignmentID.Equals(item.TestAssignmentId));

            if (entity == null)
            {
                entity = new TestAssignmentEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.TestAssignmentId = entity.TestAssignmentID;
        }

        public void Delete(TestAssignment item)
        {
            var entity = table.FirstOrDefault(x => x.TestAssignmentID.Equals(item.TestAssignmentId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(TestAssignmentEntity entity, TestAssignment item)
        {
            entity.AssignedByUserID = item.AssignedByUserId;
            entity.AvailableDateTime = item.AvailableDateTime;
            entity.ClassID = item.ClassId;
            entity.ClosingDateTime = item.ClosingDateTime;
            entity.StudentID = item.StudentId;
            entity.TestAssignmentID = item.TestAssignmentId;
            entity.TestTaken = item.TestTaken;
            entity.VirtualTestID = item.VirtualTestId;
        }
    }
}