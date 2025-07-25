using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTITestStudentAssignmentRepository : IQTITestStudentAssignmentRepository
    {
        private readonly Table<QTITestStudentAssignmentEntity> table;

        public QTITestStudentAssignmentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<QTITestStudentAssignmentEntity>();
        }

        public IQueryable<QTITestStudentAssignmentData> Select()
        {
            return table.Select(x => new QTITestStudentAssignmentData
                                {
                                    QTITestStudentAssignmentId = x.QTITestStudentAssignmentID,
                                    StudentId = x.StudentId,
                                    QTITestClassAssignmentId = x.QTITestClassAssignmentID,
                                    Status = x.Status,
                                    IsHide = x.IsHide
                                });
        }

        public void Save(QTITestStudentAssignmentData item)
        {
            var entity = table.FirstOrDefault(x => x.QTITestStudentAssignmentID.Equals(item.QTITestStudentAssignmentId));

            if (entity.IsNull())
            {
                entity = new QTITestStudentAssignmentEntity();
                table.InsertOnSubmit(entity);
            }

            //Mapper.Map(item, entity);
            BuildQTITestStudentAssignment(item, entity);
            table.Context.SubmitChanges();
            item.QTITestStudentAssignmentId = entity.QTITestStudentAssignmentID;
        }

        public void Delete(QTITestStudentAssignmentData item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.QTITestStudentAssignmentID.Equals(item.QTITestStudentAssignmentId));
                if (entity != null)
                {
                    table.DeleteOnSubmit(entity);
                    table.Context.SubmitChanges();
                }
            }
        }

        private void BuildQTITestStudentAssignment(QTITestStudentAssignmentData source,
                                                   QTITestStudentAssignmentEntity destionation)
        {
            destionation.QTITestClassAssignmentID = source.QTITestClassAssignmentId;
            destionation.StudentId = source.StudentId;
            destionation.Status = source.Status;
            destionation.IsHide = source.IsHide;
        }

        public void InsertMultipleRecord(List<QTITestStudentAssignmentData> items)
        {
            foreach (var item in items)
            {
                var entity = new QTITestStudentAssignmentEntity
                {
                    QTITestClassAssignmentID = item.QTITestClassAssignmentId,
                    StudentId = item.StudentId,
                    Status = item.Status,
                    IsHide = item.IsHide
                };

                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
