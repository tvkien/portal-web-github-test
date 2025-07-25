using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using System;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public  class QTI3pItemRepository : IRepository<QTI3pItem>
    {
        private readonly Table<QTI3pItemEntity> table;

        public QTI3pItemRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pItemEntity>();
        }

        public IQueryable<QTI3pItem> Select()
        {
            return table.Select(x => new QTI3pItem
                {
                    ABStandardGUIDs = x.ABStandardGUIDs,
                    BloomsID = x.BloomsID,
                    BloomsTaxonomy = x.BloomsTaxonomy,
                    ContentFocus = x.ContentFocus,
                    ContentFocusID = x.ContentFocusID,
                    CorrectAnswer = x.CorrectAnswer,
                    Difficulty = x.Difficulty,
                    FilePath = x.FilePath,
                    GradeID = x.GradeID,
                    GradeLevel = x.GradeLevel,
                    Identifier = x.Identifier,
                    ItemDifficultyID = x.ItemDifficultyID,
                    ItemRubric = x.ItemRubric,
                    MathOriginPath = x.MathmlOriginPath,
                    OriginPath = x.OriginPath,
                    Pvalue = x.Pvalue,
                    QTI3pItemID = x.QTI3pItemID,
                    QTI3pSourceID = x.QTI3pSourceID,
                    QTISchemaID = x.QTISchemaID,
                    Subject = x.Subject,
                    SubjectID = x.SubjectID,
                    Title = x.Title,
                    UrlPath = x.Urlpath,
                    XmlContent = x.XmlContent,
                    XmlSource = x.XmlSource,
                    From3pUpload = x.From3pUpload ?? false
                });
        }

        public void Save(QTI3pItem item)
        {
            var entity = table.FirstOrDefault(x => x.QTI3pItemID.Equals(item.QTI3pItemID));

            if (entity.IsNull())
            {
                entity = new QTI3pItemEntity();
                table.InsertOnSubmit(entity);
            }
            entity.Title = item.Title;
            entity.QTISchemaID = item.QTISchemaID;
            entity.CorrectAnswer = item.CorrectAnswer;
            entity.OriginPath = item.OriginPath;
            entity.XmlContent = item.XmlContent;
            entity.FilePath = item.FilePath;
            //entity.Urlpath = item.Urlpath;
            entity.Difficulty = item.Difficulty;
            entity.Pvalue = item.Pvalue;
            entity.Subject = item.Subject;
            entity.GradeLevel = item.GradeLevel;
            entity.GradeID = item.GradeID;
            entity.BloomsTaxonomy = item.BloomsTaxonomy;
            entity.ABStandardGUIDs = item.ABStandardGUIDs;
            entity.Identifier = item.Identifier;
            entity.ItemRubric = item.ItemRubric;
            //entity.MathmlOriginPath = item.MathmlOriginPath;
            entity.BloomsID = item.BloomsID;
            entity.ContentFocusID = item.ContentFocusID;
            entity.ItemDifficultyID = item.ItemDifficultyID;
            entity.QTI3pSourceID = item.QTI3pSourceID;
            entity.XmlSource = item.XmlSource;
            entity.ContentFocus = item.ContentFocus;
            entity.SubjectID = item.SubjectID;
            entity.From3pUpload = item.From3pUpload;

            table.Context.SubmitChanges();
            item.QTI3pItemID = entity.QTI3pItemID;
        }

        public void Delete(QTI3pItem item)
        {
            throw new NotImplementedException();
        }
    }
}