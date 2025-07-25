using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Report : ValidatableEntity<Report>, IIdentifiable
    {
        private string name = string.Empty;
        private string url = string.Empty;

        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string URL
        {
            get { return url; }
            set { url = value.ConvertNullToEmptyString(); }
        }
    }
}