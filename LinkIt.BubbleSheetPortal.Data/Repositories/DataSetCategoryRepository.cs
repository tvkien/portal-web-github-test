using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs;

namespace LinkIt.BubbleSheetPortal.Data
{
    public class DataSetCategoryRepository : IDataSetCategoryRepository
    {
        private readonly Table<DataSetCategoryEntity> table;

        private readonly DbDataContext _context;

        public DataSetCategoryRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<DataSetCategoryEntity>();
            _context = DbDataContext.Get(connectionString);
        }

        public void Delete(DataSetCategory item)
        {
            throw new NotImplementedException();
        }

        public void Save(DataSetCategory item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<DataSetCategory> Select()
        {
            return table.Select(o => new DataSetCategory()
            {
                DataSetCategoryID = o.DataSetCategoryID,
                DataSetCategoryName = o.DataSetCategoryName,
                DistrictID = o.DistrictID
                //StateID = o.StateID,
            });
        }

        public IQueryable<T> Select<T>(Expression<Func<DataSetCategory, T>> converter)
        {
            return Select().Select(converter);
        }

        public IEnumerable<DataSetCategoryDTO> GetCategoryByUser(int UserId, int? stateId, int? dictrictId, int? categoryId)
        {
            return _context.GetCategoryByUser(UserId, stateId, dictrictId, categoryId).Select(x => new DataSetCategoryDTO()
            {
                DataSetCategoryID = x.DataSetCategoryID.GetValueOrDefault(),
                DataSetCategoryName = x.DataSetCategoryName,
                DistrictID = x.DistrictID,
                DistrictName = x.DistrictName,
                DisplayName = x.DisplayName
            });
        }
    }
}

