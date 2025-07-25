using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTI3pPassageProgressRepository : IRepository<QTI3pPassageProgress>
    {
        private readonly Table<QTI3pPassageProgressEntity> table;

        public QTI3pPassageProgressRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pPassageProgressEntity>();
            Mapper.CreateMap<QTI3pPassageProgress, QTI3pPassageProgressEntity>();
        }

        public IQueryable<QTI3pPassageProgress> Select()
        {
            return table.Select(x => new QTI3pPassageProgress
                                {
                                    QTI3pPassageProgressID = x.QTI3pPassageProgressID,
                                    Qti3pPassageID = x.Qti3pPassageID,
                                    Qti3pProgressPassageTypeID = x.Qti3pProgressPassageTypeID,
                                    Qti3pProgressPassageGenreID = x.Qti3pProgressPassageGenreID,
                                    Lexile = x.Lexile,
                                    Spache = x.Spache,
                                    DaleChall = x.DaleChall,
                                    RMM = x.RMM
                                });
        }

        public void Save(QTI3pPassageProgress item)
        {
            var entity = table.FirstOrDefault(x => x.QTI3pPassageProgressID.Equals(item.QTI3pPassageProgressID));

            if (entity.IsNull())
            {
                entity = new QTI3pPassageProgressEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
        }
        public void Delete(QTI3pPassageProgress item)
        {
            throw new NotImplementedException();
        }

    }
}
