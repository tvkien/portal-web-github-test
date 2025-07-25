using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ConversionSetRepository : IRepository<ConversionSet>
    {
        private readonly TestDataContext _testDataContext;
        private readonly Table<ConversionSetEntity> _table;

        public ConversionSetRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testDataContext = TestDataContext.Get(connectionString);
            _table = _testDataContext.GetTable<ConversionSetEntity>();
        }

        public IQueryable<ConversionSet> Select()
        {
            return _table.Select(
                x => new ConversionSet()
                         {
                             ConverstionSetID = x.ConverstionSetID,
                             Name = x.Name,
                             Description = x.Description,
                             ConversionMethod = x.ConversionMethod,
                             VirtualTestSubTypeID = x.VirtualTestSubTypeID
                         }
                );
        }

        public void Save(ConversionSet item)
        {
            var entity = _table.FirstOrDefault(x => x.ConverstionSetID == item.ConverstionSetID);
            if (entity == null)
            {
                entity = new ConversionSetEntity
                {
                    Name = item.Name,
                    Description = item.Description ?? string.Empty,
                    ConversionMethod = item.ConversionMethod ?? string.Empty,
                    VirtualTestSubTypeID = item.VirtualTestSubTypeID
                };
                _table.InsertOnSubmit(entity);
                _table.Context.SubmitChanges();
                item.ConverstionSetID = entity.ConverstionSetID;
            }
            else
            {
                entity.Name = item.Name;
                entity.Description = item.Description ?? string.Empty;
                entity.ConversionMethod = item.ConversionMethod ?? string.Empty;
                entity.VirtualTestSubTypeID = item.VirtualTestSubTypeID;
                _table.Context.SubmitChanges();
            }
        }

        public void Delete(ConversionSet item)
        {
            throw new NotImplementedException();
        }
    }
}
