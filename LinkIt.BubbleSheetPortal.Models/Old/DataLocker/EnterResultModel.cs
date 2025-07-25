using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class EnterResultModel
    {
        public bool IsAdmin { get; set; }
        public bool CanSelectTeachers { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }


        public bool IsUseTestExtract { get; set; }
        public List<int> ListDistricIds { get; set; }

        public string StrIds
        {
            get
            {
                var ids = string.Empty;
                if (!this.ListDistricIds.Any())
                {
                    return ids;
                }
                ids = this.ListDistricIds.Aggregate(ids, (current, id) => current + (id + ","));
                return ids.TrimEnd(new[] { ',' });
            }
        }
    }
}
