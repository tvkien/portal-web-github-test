using System.Collections.Generic;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AddEditPrintingGroupViewModel 
    {
        private List<string> classIdList = new List<string>();
        private List<int> teacherIdList = new List<int>(); 
        private string name = string.Empty;
                
        public int GroupId { get; set; }         
        public int DistrictId { get; set; }
        public int CreatedUserId { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public List<string> ClassIdList
        {
            get { return classIdList; }
            set { classIdList = value ?? new List<string>(); }
        }

        public List<int> TeacherIdList
        {
            get { return teacherIdList; }
            set { teacherIdList = value ?? new List<int>(); }
        }
    }
}