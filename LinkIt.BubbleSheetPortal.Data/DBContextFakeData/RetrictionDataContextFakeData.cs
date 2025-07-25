using LinkIt.BubbleSheetPortal.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.DBContextFakeData
{
    public class RetrictionDataContextFakeData
    {
        public EntitySet<XLITestRestrictionModuleRole> XLITestRestrictionModuleRoles { set; get; }

        public EntitySet<XLITestRestrictionModule> XLITestRestrictionModules { set; get; }

        public EntitySet<Role> Roles { set; get; }

        public RetrictionDataContextFakeData() {
            Roles = new EntitySet<Role>();
            XLITestRestrictionModules = new EntitySet<XLITestRestrictionModule>();
            XLITestRestrictionModuleRoles = new EntitySet<XLITestRestrictionModuleRole>();
            InnitFakeData();
        }
        private void InnitFakeData() {
            //for table Role
            Roles.Add(new Role
            {
                Display = "District Administrator",
                Name = "Administrator",
                RoleID = 3
            });
            Roles.Add(new Role
            {
                Display = "Network Admin",
                Name = "NetworkAdmin",
                RoleID = 27
            });
            Roles.Add(new Role
            {
                Display = "Classroom Teacher",
                Name = "Teacher",
                RoleID = 2
            });
            Roles.Add(new Role
            {
                Display = "School Administrator",
                Name = "School",
                RoleID = 8
            });
            Roles.Add(new Role
            {
                Display = "Publisher",
                Name = "publisher",
                RoleID = 5
            });

            #region // for table XLITestRestrictionModule
            XLITestRestrictionModules.Add(new XLITestRestrictionModule
            {
                XLITestRestrictionModuleID = 1,
                Code = "print",
                Name = "Print Test",
                CreatedDate = DateTime.Now,
            });
            XLITestRestrictionModules.Add(new XLITestRestrictionModule
            {
                XLITestRestrictionModuleID = 2,
                Code = "assign",
                Name = "Assign Test",
                CreatedDate = DateTime.Now,
            });
            XLITestRestrictionModules.Add(new XLITestRestrictionModule
            {
                XLITestRestrictionModuleID = 5,
                Code = "manager",
                Name = "Manager Assignment",
                CreatedDate = DateTime.Now,
            });
            XLITestRestrictionModules.Add(new XLITestRestrictionModule
            {
                XLITestRestrictionModuleID = 6,
                Code = "preview",
                Name = "Teacher Preview Online",
                CreatedDate = DateTime.Now,
            });
            XLITestRestrictionModules.Add(new XLITestRestrictionModule
            {
                XLITestRestrictionModuleID = 9,
                Code = "reporting",
                Name = "Reporting",
                CreatedDate = DateTime.Now,
            });
            XLITestRestrictionModules.Add(new XLITestRestrictionModule
            {
                XLITestRestrictionModuleID = 10,
                Code = "review",
                Name = "Review Assignment and Manual Grade",
                CreatedDate = DateTime.Now,
            });
            XLITestRestrictionModules.Add(new XLITestRestrictionModule
            {
                XLITestRestrictionModuleID = 14,
                Code = "view_grade",
                Name = "Can only View/Grade questions that require manual grading",
                CreatedDate = DateTime.Now,
            });
            #endregion


            //for table XLITestRestrictionModuleRoles
            XLITestRestrictionModuleRoles.Add(new XLITestRestrictionModuleRole
            {
                XLITestRestrictionModuleID = 14,
                XLITestRestrictionModuleRoleID = 997,
                PublishedLevelID=2520,
                PublishedLevelName = "district",
                RestrictedObjectID = 17945,
                RestrictedObjectName= "bank",
                RoleID=27,
                Role= Roles.FirstOrDefault(x=>x.RoleID==27),
                XLITestRestrictionModule = XLITestRestrictionModules.FirstOrDefault(x=>x.XLITestRestrictionModuleID== 14)
            });

            XLITestRestrictionModuleRoles.Add(new XLITestRestrictionModuleRole
            {
                XLITestRestrictionModuleID = 14,
                XLITestRestrictionModuleRoleID = 997,
                PublishedLevelID = 2520,
                PublishedLevelName = "district",
                RestrictedObjectID = 17945,
                RestrictedObjectName = "bank",
                RoleID = 3,
                Role = Roles.FirstOrDefault(x => x.RoleID == 3),
                XLITestRestrictionModule = XLITestRestrictionModules.FirstOrDefault(x => x.XLITestRestrictionModuleID == 14)
            });

            for (int i = 0; i < XLITestRestrictionModules.Count; i++)
            {
                XLITestRestrictionModules[i].XLITestRestrictionModuleRoles.AddRange(XLITestRestrictionModuleRoles.Where(x => x.XLITestRestrictionModuleID == XLITestRestrictionModules[i].XLITestRestrictionModuleID));
            }

        }
    }
}
