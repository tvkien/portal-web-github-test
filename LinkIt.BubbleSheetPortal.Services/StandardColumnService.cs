using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StandardColumnService
    {
        private readonly IReadOnlyRepository<StandardColumn> standardColumnRepository;
        private readonly IReadOnlyRepository<ColumnLookupData> columnLookupDataRepository;

        public StandardColumnService(IReadOnlyRepository<StandardColumn> standardColumnRepository, IReadOnlyRepository<ColumnLookupData> columnLookupDataRepository)
        {
            this.standardColumnRepository = standardColumnRepository;
            this.columnLookupDataRepository = columnLookupDataRepository;
        }

        public IQueryable<StandardColumn> GetAllColumnByLoaderTypeAndColumnType(MappingLoaderType type, StandardColumnTypes columnType)
        {
            return standardColumnRepository.Select().Where(c => c.LoaderType == type && c.ColumnType == columnType);
        }

        public List<string> GetFixedOrLookupValueByColumn(int columnID)
        {
            return columnLookupDataRepository.Select().Where(c => c.ColumnID == columnID).Select(c => c.Data).ToList();
        }

        public IQueryable<StandardColumn> GetDefaultColumnsByLoaderTypeAndColumnType(MappingLoaderType type, StandardColumnTypes columnType)
        {
            if (columnType == StandardColumnTypes.CommonField)
            {
                return standardColumnRepository.Select().Where(c => c.LoaderType == type && c.ColumnType == columnType && c.IsDefaultForCommonPhase);
            }

            return standardColumnRepository.Select().Where(c => c.LoaderType == type && c.ColumnType == columnType && c.IsDefaultForTestPhase);
        }

        public IQueryable<StandardColumn> GetRequiredColumnsByLoaderTypeAndColumnType(MappingLoaderType type, StandardColumnTypes columnType)
        {
            if (columnType == StandardColumnTypes.CommonField)
            {
                return standardColumnRepository.Select().Where(c => c.LoaderType == type && c.ColumnType == columnType && c.IsRequiredForCommonPhase);
            }

            return standardColumnRepository.Select().Where(c => c.LoaderType == type && c.ColumnType == columnType && c.IsRequiredForTestPhase);
        }
    }
}