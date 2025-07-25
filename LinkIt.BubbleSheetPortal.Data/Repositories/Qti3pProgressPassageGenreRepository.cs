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
    public class Qti3pProgressPassageGenreRepository : IRepository<Qti3pProgressPassageGenre>
    {
        private readonly Table<Qti3pProgressPassageGenreEntity> table;

        public Qti3pProgressPassageGenreRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<Qti3pProgressPassageGenreEntity>();
            Mapper.CreateMap<Qti3pProgressPassageGenre, Qti3pProgressPassageGenreEntity>();
        }

        public IQueryable<Qti3pProgressPassageGenre> Select()
        {
            return table.Select(x => new Qti3pProgressPassageGenre
                                {
                                    Qti3pProgressPassageGenreID = x.Qti3pProgressPassageGenreID,
                                    Qti3pProgressPassageGenreName = x.Qti3pProgressPassageGenreName
                                });
        }

        public void Save(Qti3pProgressPassageGenre item)
        {
            var entity = table.FirstOrDefault(x => x.Qti3pProgressPassageGenreID.Equals(item.Qti3pProgressPassageGenreID));

            if (entity.IsNull())
            {
                entity = new Qti3pProgressPassageGenreEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
        }
        public void Delete(Qti3pProgressPassageGenre item)
        {
            throw new NotImplementedException();
        }

    }
}
