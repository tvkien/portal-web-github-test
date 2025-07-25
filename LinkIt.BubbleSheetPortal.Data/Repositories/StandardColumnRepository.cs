using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class StandardColumnRepository : IReadOnlyRepository<StandardColumn>
    {
        private readonly Table<StandardColumnEntity> table;

        public StandardColumnRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<StandardColumnEntity>();
        }

        public IQueryable<StandardColumn> Select()
        {
            return table.Select(c => new StandardColumn
                {
                    ColumnID = c.ColumnID,
                    LoaderType = (MappingLoaderType)c.LoaderType,
                    Name = c.Name,
                    Description = c.Description,
                    ColumnType = (StandardColumnTypes)c.ColumnType,
                    IsDefaultForCommonPhase = c.IsDefaultForCommonPhase,
                    IsDefaultForTestPhase = c.IsDefaultForTestPhase,
                    IsRequiredForCommonPhase = c.IsRequiredForCommonPhase,
                    IsRequiredForTestPhase = c.IsRequiredForTestPhase
                });
        }
    }
}