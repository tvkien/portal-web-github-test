using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirutalQuestionRepository : IVirtualQuestionRepository
    {
        private readonly Table<VirtualQuestionEntity> table;

        public VirutalQuestionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<VirtualQuestionEntity>();
        }

        public IQueryable<VirtualQuestionData> Select()
        {
            return table.Select(x => new VirtualQuestionData
                {
                    VirtualTestID = x.VirtualTestID,
                    PointsPossible = x.PointsPossible,
                    Deductions = x.Deductions,
                    ETSItemID = x.ETSItemID,
                    MasterQuestionID = x.MasterQuestionID,
                    MasterTestID = x.MasterTestID,
                    PreProdVQID = x.PreProdVQID,
                    PreQTIVirtualQuestionID = x.PreQTIVirtualQuestionID,
                    QTIItemID = x.QTIItemID,
                    QuestionOrder = x.QuestionOrder,
                    VirtualQuestionID = x.VirtualQuestionID,
                    BaseVirtualQuestionId = x.BaseVirtualQuestionId,
                    QuestionLabel = x.QuestionLabel,
                    QuestionNumber = x.QuestionNumber,
                    IsRubricBasedQuestion = x.IsRubricBasedQuestion,
                    ScoreName = x.ScoreName
                });
        }

        public void Save(VirtualQuestionData item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualQuestionID.Equals(item.VirtualQuestionID));

            if (entity == null)
            {
                entity = new VirtualQuestionEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.VirtualQuestionID = entity.VirtualQuestionID;
        }

        public void Delete(VirtualQuestionData item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemID.Equals(item.VirtualQuestionID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualQuestionEntity entity, VirtualQuestionData item)
        {
            entity.VirtualTestID = item.VirtualTestID;
            entity.PointsPossible = item.PointsPossible;
            entity.Deductions = item.Deductions;
            entity.ETSItemID = item.ETSItemID;
            entity.MasterQuestionID = item.MasterQuestionID;
            entity.MasterTestID = item.MasterTestID;
            entity.PreProdVQID = item.PreProdVQID;
            entity.PreQTIVirtualQuestionID = item.PreQTIVirtualQuestionID;
            entity.QTIItemID = item.QTIItemID;
            entity.QuestionOrder = item.QuestionOrder;
            entity.BaseVirtualQuestionId = item.BaseVirtualQuestionId;
            entity.QuestionLabel = item.QuestionLabel;
            entity.QuestionNumber = item.QuestionNumber;
            entity.IsRubricBasedQuestion = item.IsRubricBasedQuestion;
            entity.ScoreName = item.ScoreName;
        }

        public void InsertMultipleRecord(List<VirtualQuestionData> items)
        {
            var entities = items.Select(item => new VirtualQuestionEntity
            {
                VirtualTestID = item.VirtualTestID,
                PointsPossible = item.PointsPossible,
                Deductions = item.Deductions,
                ETSItemID = item.ETSItemID,
                MasterQuestionID = item.MasterQuestionID,
                MasterTestID = item.MasterTestID,
                PreProdVQID = item.PreProdVQID,
                PreQTIVirtualQuestionID = item.PreQTIVirtualQuestionID,
                QTIItemID = item.QTIItemID,
                QuestionOrder = item.QuestionOrder,
                BaseVirtualQuestionId = item.BaseVirtualQuestionId,
                QuestionLabel = item.QuestionLabel,
                QuestionNumber = item.QuestionNumber,
                IsRubricBasedQuestion = item.IsRubricBasedQuestion,
                ScoreName = item.ScoreName,
            }).ToList();

            entities.ForEach(entity =>
            {
                table.InsertOnSubmit(entity);
            });

            table.Context.SubmitChanges();

            items.ForEach(item =>
            {
                item.VirtualQuestionID = entities.FirstOrDefault(x => x.QTIItemID == item.QTIItemID && x.QuestionOrder == item.QuestionOrder)?.VirtualQuestionID ?? 0;
            });
        }
    }
}
