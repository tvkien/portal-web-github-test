using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ItemTagViewModel
    {

        public ItemTagViewModel()
        {
        }

        private int roleId = 0;
        public int RoleId
        {
            get { return roleId; }
            set { roleId = value; }
        }
        public int DistrictId { get; set; }
        
        public bool IsDistrictAdmin
        {
            get
            {
                return RoleId.Equals((int)Permissions.DistrictAdmin);
            }
        }
        public bool IsPublisher
        {
            get
            {
                return RoleId.Equals((int)Permissions.Publisher);
            }
        }
        public int ItemTagCategoryID { get; set; }
        public string CategoryName { get; set; }

        public int ItemTagID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //recover search condition when back to District Category
        public int SelectedStateId { get; set; }
        public int SelectedDistrictId { get; set; }
        public string SearchBoxText { get; set; }
    }
}