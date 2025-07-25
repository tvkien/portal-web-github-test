using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StandardColumn
    {
        private string name;
        private string description;
        
        public int ColumnID { get; set; }
        public MappingLoaderType LoaderType { get; set; }
        public StandardColumnTypes ColumnType { get; set; }
        public bool IsDefaultForCommonPhase { get; set; }
        public bool IsDefaultForTestPhase { get; set; }
        public bool IsRequiredForCommonPhase { get; set; }
        public bool IsRequiredForTestPhase { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string Description
        {
            get { return description; }
            set { description = value.ConvertNullToEmptyString(); }
        }
    }
}