using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ProgramToView
    {
        private string name = string.Empty;
        private string code = string.Empty;

        public int ProgramId { get; set; }
        public int? AccessLevelId { get; set; }
        public int StudentNumber { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }
    }
}