using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTI3pPassageRepository : IRepository<QTI3pPassage>
    {
        private readonly Table<QTI3pPassageEntity> table;

        public QTI3pPassageRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pPassageEntity>();
        }

        public IQueryable<QTI3pPassage> Select()
        {
            return table.Select(x => new QTI3pPassage
                                {
                                   QTI3pPassageID = x.QTI3pPassageID,
                                   PassageName = x.PassageName,
                                   Number = x.Number,
                                   Subject = x.Subject,
                                   PassageTitle = x.PassageTitle,
                                   Identifier = x.Identifier,
                                   Qti3pSourceID = x.Qti3pSourceID,
                                   Fullpath = x.Fullpath,
                                   GradeID = x.GradeID
                                });
        }

        public void Save(QTI3pPassage item)
        {
            var entity = table.FirstOrDefault(x => x.QTI3pPassageID.Equals(item.QTI3pPassageID));

            if (entity.IsNull())
            {
                entity = new QTI3pPassageEntity();
                table.InsertOnSubmit(entity);
            }
            entity.PassageName = item.PassageName;
            entity.Fullpath = item.Fullpath;
            entity.Number = item.Number;
            entity.Qti3pSourceID = item.Qti3pSourceID;
            entity.Identifier = item.Identifier;
            entity.PassageTitle = item.PassageTitle;
            entity.Subject = item.Subject;
            entity.GradeLevel = item.GradeLevel;
            entity.PassageStimulus = item.PassageStimulus;
            entity.ContentArea = item.ContentArea;
            entity.TextType = item.TextType;
            entity.TextSubType = item.TextSubType;
            entity.PassageSource = item.PassageSource;
            entity.WordCount = item.WordCount;
            entity.Ethnicity = item.Ethnicity;
            entity.CommissionedStatus = item.CommissionedStatus;
            entity.FleschKincaid = item.FleschKincaid;
            entity.Gender = item.Gender;
            entity.MultiCultural = item.MultiCultural;
            entity.CopyrightYear = item.CopyrightYear;
            entity.CopyrightOwner = item.CopyrightOwner;
            entity.PassageSourceTitle = item.PassageSourceTitle;
            entity.Author = item.Author;
            entity.GradeID = item.GradeID;
            entity.ContentAreaID = item.ContentAreaID;
            entity.TextTypeID = item.TextTypeID;
            entity.TextSubTypeID = item.TextSubTypeID;
            entity.WordCountID = item.WordCountID;
            entity.FleschKincaidID = item.FleschKincaidID;

            table.Context.SubmitChanges();
            item.QTI3pPassageID = entity.QTI3pPassageID;
        }

        public void Delete(QTI3pPassage item)
        {
            throw new NotImplementedException();
        }
    }
}
