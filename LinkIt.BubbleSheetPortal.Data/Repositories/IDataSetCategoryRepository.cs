using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace LinkIt.BubbleSheetPortal.Data
{
    public interface IDataSetCategoryRepository : IRepository<DataSetCategory>
    {
        IQueryable<T> Select<T>(Expression<Func<DataSetCategory, T>> converter);
        IEnumerable<DataSetCategoryDTO> GetCategoryByUser(int UserId, int? stateId, int? dictrictId, int? categoryId);
    }
}
