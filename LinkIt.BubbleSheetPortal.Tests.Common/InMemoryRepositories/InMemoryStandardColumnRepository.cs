using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryStandardColumnRepository:IReadOnlyRepository<StandardColumn>
    {
        private readonly List<StandardColumn> table;

        public InMemoryStandardColumnRepository()
        {
            table = AddStandardColumns();
        }

        private List<StandardColumn> AddStandardColumns()
        {
            return new List<StandardColumn> 
            {
                new StandardColumn{ ColumnID=1, ColumnType= StandardColumnTypes.CommonField, Description=string.Empty, IsDefaultForCommonPhase=true, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =true, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Test Year"},
                new StandardColumn{ ColumnID=2, ColumnType= StandardColumnTypes.CommonField, Description=string.Empty, IsDefaultForCommonPhase=true, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =true, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Test Name"},
                new StandardColumn{ ColumnID=3, ColumnType= StandardColumnTypes.CommonField, Description=string.Empty, IsDefaultForCommonPhase=true, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =true, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Subject"},
                new StandardColumn{ ColumnID=4, ColumnType= StandardColumnTypes.CommonField, Description=string.Empty, IsDefaultForCommonPhase=true, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =true, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Subject Ext"},
                new StandardColumn{ ColumnID=5, ColumnType= StandardColumnTypes.CommonField, Description=string.Empty, IsDefaultForCommonPhase=true, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =true, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "School"},
                new StandardColumn{ ColumnID=6, ColumnType= StandardColumnTypes.CommonField, Description=string.Empty, IsDefaultForCommonPhase=true, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =true, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Grade"},
                new StandardColumn{ ColumnID=7, ColumnType= StandardColumnTypes.CommonField, Description=string.Empty, IsDefaultForCommonPhase=true, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =true, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Local ID"},
                new StandardColumn{ ColumnID=8, ColumnType= StandardColumnTypes.CommonField, Description=string.Empty, IsDefaultForCommonPhase=true, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =true, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "SID"},
                new StandardColumn{ ColumnID=9, ColumnType= StandardColumnTypes.CommonField, Description=string.Empty, IsDefaultForCommonPhase=true, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =true, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Last Name"},
                new StandardColumn{ ColumnID=10, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "First Name"},
                new StandardColumn{ ColumnID=11, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Middle Initial"},
                new StandardColumn{ ColumnID=12, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Gender"},
                new StandardColumn{ ColumnID=13, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Ethnicity"},
                new StandardColumn{ ColumnID=14, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Total Raw Score"},
                new StandardColumn{ ColumnID=15, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = true, LoaderType = MappingLoaderType.DataLoader, Name = "Raw Score"},
                new StandardColumn{ ColumnID=16, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = true, LoaderType = MappingLoaderType.DataLoader, Name = "Percent"},
                new StandardColumn{ ColumnID=17, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = true, LoaderType = MappingLoaderType.DataLoader, Name = "Percentile"},
                new StandardColumn{ ColumnID=18, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = true, LoaderType = MappingLoaderType.DataLoader, Name = "Lexile"},
                new StandardColumn{ ColumnID=19, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = true, LoaderType = MappingLoaderType.DataLoader, Name = "Proficiency Level"},
                new StandardColumn{ ColumnID=20, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Pgm 1"},
                new StandardColumn{ ColumnID=21, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Pgm 2"},
                new StandardColumn{ ColumnID=22, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Pgm 3"},
                new StandardColumn{ ColumnID=23, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Pgm 4"},
                new StandardColumn{ ColumnID=24, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Pgm 5"},
                new StandardColumn{ ColumnID=25, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Pgm 6"},
                new StandardColumn{ ColumnID=26, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Pgm 7"},
                new StandardColumn{ ColumnID=27, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Pgm 8"},
                new StandardColumn{ ColumnID=28, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Pgm 9"},
                new StandardColumn{ ColumnID=29, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = true, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Pgm 10"},
                new StandardColumn{ ColumnID=30, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Cluster 1 Name"},
                new StandardColumn{ ColumnID=31, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Cluster 1 Total Raw"},
                new StandardColumn{ ColumnID=32, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Cluster 1 Raw Score"},
                new StandardColumn{ ColumnID=33, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Cluster 1 Scaled Score"},
                new StandardColumn{ ColumnID=34, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Cluster 1 Percent"},
                new StandardColumn{ ColumnID=35, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Cluster 1 Percentile"},
                new StandardColumn{ ColumnID=36, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Cluster 1 Lexile"},
                new StandardColumn{ ColumnID=37, ColumnType= StandardColumnTypes.TestField, Description=string.Empty, IsDefaultForCommonPhase=false, IsDefaultForTestPhase = false, IsRequiredForCommonPhase =false, IsRequiredForTestPhase = false, LoaderType = MappingLoaderType.DataLoader, Name = "Cluster 1 Proficiency Level"}
            };
        }

        public IQueryable<StandardColumn> Select()
        {
            return table.AsQueryable();
        }
    }
}
