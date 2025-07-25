using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models
{
    public class GetAvailableClassesRequest : GenericDataTableRequest
    {
        public int? SchoolId { get; set; }

        public int StudentId { get; set; }
    }

    public class GetAvailableAddNewStudentAssignClassRequest : GenericDataTableRequest
    {
        public int? UserId { get; set; }

        public int? SchoolID { get; set; }

        public string AssignedClassIdString { get; set; }
    }
}
