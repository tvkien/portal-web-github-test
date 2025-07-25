using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class LessonProvider
    {
        private string name = string.Empty;
        private string liCode = string.Empty;

        public int Id { get; set; }
        public int StateId { get; set; }
        public int DistrictGroupId { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string LICode
        {
            get { return liCode; }
            set { liCode = value.ConvertNullToEmptyString(); }
        }
        private string stateName = string.Empty;
        public string StateName
        {
            get { return stateName; }
            set { stateName = value.ConvertNullToEmptyString(); }
        }
       public int AllDistrict { get; set; }
    }
}