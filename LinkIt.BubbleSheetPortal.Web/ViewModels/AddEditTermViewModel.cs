using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AddEditTermViewModel : ValidatableEntity<AddEditTermViewModel>
    {
        private string name = string.Empty;

        public int TermID { get; set; }
        public int DistrictID { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}