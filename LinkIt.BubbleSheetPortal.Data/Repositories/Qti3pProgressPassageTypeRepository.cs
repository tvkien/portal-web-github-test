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
    public class Qti3pProgressPassageTypeRepository : IRepository<Qti3pProgressPassageType>
    {
        private readonly Table<Qti3pProgressPassageTypeEntity> table;

        public Qti3pProgressPassageTypeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<Qti3pProgressPassageTypeEntity>();
            Mapper.CreateMap<Qti3pProgressPassageType, Qti3pProgressPassageTypeEntity>();
        }

        public IQueryable<Qti3pProgressPassageType> Select()
        {
            return table.Select(x => new Qti3pProgressPassageType
                                {
                                    Qti3pProgressPassageTypeID = x.Qti3pProgressPassageTypeID,
                                    Qti3pProgressPassageTypeName = x.Qti3pProgressPassageTypeName
                                });
        }

        public void Save(Qti3pProgressPassageType item)
        {
            var entity = table.FirstOrDefault(x => x.Qti3pProgressPassageTypeID.Equals(item.Qti3pProgressPassageTypeID));

            if (entity.IsNull())
            {
                entity = new Qti3pProgressPassageTypeEntity();
                table.InsertOnSubmit(entity);
            }
            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
        }
        public void Delete(Qti3pProgressPassageType item)
        {
            throw new NotImplementedException();
        }

    }
}
