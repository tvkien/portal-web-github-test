using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;


namespace LinkIt.BubbleSheetPortal.Services
{
    public class XLIMenuPermissionService
    {
        private readonly IXLIIconAccessRepository _repositoryIconAccess;
        private readonly IXLIModuleAccessRepository _repositoryModuleAccess;
        private readonly IUserSchoolRepository<UserSchool> _repositoryUserSchool;
        private readonly IReadOnlyRepository<XliArea> _repositoryXliArea;
        private readonly IReadOnlyRepository<XliModule> _repositoryXliModule;
        private readonly IRepository<User> repositoryUser;
        private readonly IReadOnlyRepository<DistrictDecode> _repositoryDistrictDecode;
        private readonly IReadOnlyRepository<Configuration> _repositoryConfiguration;
        private readonly IReadOnlyRepository<District> _repositoryDistrict;
        private readonly IReadOnlyRepository<State> _repositoryState;


        public XLIMenuPermissionService(IXLIIconAccessRepository repositoryIconAccess,
            IXLIModuleAccessRepository repositoryModuleAccess,
            IUserSchoolRepository<UserSchool> repositoryUserSchool,
            IReadOnlyRepository<XliArea> xliArea,
            IReadOnlyRepository<XliModule> xliModule,
            IRepository<User> repositoryUser,
            IReadOnlyRepository<DistrictDecode> repositoryDistrictDecode,
            IReadOnlyRepository<Configuration> repositoryConfiguration,
            IReadOnlyRepository<District> repositoryDistrict,
            IReadOnlyRepository<State> repositoryState
            )
        {
            _repositoryIconAccess = repositoryIconAccess;
            _repositoryModuleAccess = repositoryModuleAccess;
            _repositoryUserSchool = repositoryUserSchool;
            _repositoryXliArea = xliArea;
            _repositoryXliModule = xliModule;
            this.repositoryUser = repositoryUser;
            _repositoryDistrictDecode = repositoryDistrictDecode;
            _repositoryConfiguration = repositoryConfiguration;
            _repositoryDistrict = repositoryDistrict;
            _repositoryState = repositoryState;
        }

        #region Old code get Icon & Tab
        public MenuAccessItems GetMenuAccessByDistrictOld(User currentUser)
        {
            int districtId = currentUser.DistrictId.GetValueOrDefault();

            var lstIconAccess = new List<XLIIconAccess>();
            //TODO: TimeOut -> repositoryIconAccess.Select().Where(o => o.DistrictId == districtId).Distinct().ToList();
            var lstModuleAccess = new List<XLIModuleAccess>();
            //TODO: Timout -> repositoryModuleAccess.Select().Where(o => o.DMDistrictId == districtId).Distinct().ToList();

            var obj = new MenuAccessItems();
            if (currentUser.RoleId == (int)Permissions.SchoolAdmin || currentUser.RoleId == (int)Permissions.Teacher)
            {
                var query = _repositoryUserSchool.Select().Where(o => o.UserId == currentUser.Id);
                if (query.Any())
                    obj.ListSchoolId = query.Select(o => o.SchoolId.GetValueOrDefault()).ToList();
            }
            obj.DistrictId = districtId;
            obj.RoleId = currentUser.RoleId;

            XliArea xliAreaHome = _repositoryXliArea.Select().FirstOrDefault(o => o.Code.ToUpper().Equals(ContaintUtil.Home));
            var lstAreas = new List<XliArea>();
            if (xliAreaHome != null)
                lstAreas = _repositoryXliArea.Select().Where(o => o.XliAreaId >= xliAreaHome.XliAreaId).ToList();
            else
            {
                lstAreas = _repositoryXliArea.Select().ToList();
            }
            foreach (XliArea xliArea in lstAreas)
            {
                lstIconAccess = _repositoryIconAccess.Select().Where(o => o.DistrictId == districtId && o.IconCode.Equals(xliArea.Code)).Distinct().ToList();
                CheckDisplayIconNew(xliArea, obj, lstIconAccess);

                if (obj.HasDisplayedItem(xliArea.Code))
                {
                    List<XliModule> lstXliModules = _repositoryXliModule.Select().Where(o => o.XliAreaId == xliArea.XliAreaId).ToList();
                    lstModuleAccess = _repositoryModuleAccess.Select().Where(o => o.DMDistrictId == districtId && o.AreaCode.Equals(xliArea.Code)).Distinct().ToList();
                    foreach (XliModule xliModule in lstXliModules)
                    {
                        CheckDisplaySubItemNew(xliArea.Code, xliModule, obj, lstModuleAccess);
                    }
                }
                if (xliArea.Code.Equals(ContaintUtil.Techsupport))
                {
                    if (!obj.HasDisplayedItem(ContaintUtil.Techsupport))
                    {
                        if (currentUser.IsNetworkAdmin)
                        {
                            CheckMenuTechSupportForNetworkAdmin(obj, currentUser.Id);
                        }
                        //Impersonate Update, if current user is impersonated from another user
                        if (currentUser.OriginalID > 0)
                        {
                            CheckMenuTechSupportForOrginalUser(obj, currentUser);
                        }
                    }

                }
            }

            //Check submenu Impersonate for NetworkAdmin
            if (obj.HasDisplayedItem(ContaintUtil.Techsupport))
            {
                if (currentUser.IsNetworkAdmin)
                {
                    // use the same logic for adding submenu as CheckDisplaySubItemNew
                    CheckSubMenuImpersonateForNetworkAdmin(obj, currentUser.Id, (int)Permissions.NetworkAdmin);
                }
                //Impersonate Update, if current user is impersonated from another user
                if (currentUser.OriginalID > 0)
                {
                    CheckSubMenuImpersonateForOriginalUser(obj, currentUser);
                }
            }
            return obj;
        }
        private void CheckMenuTechSupportForNetworkAdmin(MenuAccessItems obj, int userId)
        {
            //Network Admin is able to see the Impersonation tab if the tab is authorized for the service organization district
            var user = repositoryUser.Select().Where(x => x.Id == userId).FirstOrDefault();
            if (user != null)
            {
                //get the IconAccess of NetworkAdmin
                List<XLIIconAccess> lstIconAccessOrgDistrict =
                    _repositoryIconAccess.Select().Where(o => o.DistrictId == user.DistrictId.Value).Distinct
                        ().ToList();
                //if Organization District of NetworkAdmin include TECH SUPPORT, then add this TECH SUPPORT to current user ( the user that network admin impersonated when login)
                if (lstIconAccessOrgDistrict.Any(x => x.IconCode.Equals(ContaintUtil.Techsupport)))
                {
                    obj.DisplayedItems.Add(string.Format("|{0}|", ContaintUtil.Techsupport));
                }
            }
        }

        private void CheckSubMenuImpersonateForNetworkAdmin(MenuAccessItems obj, int userId, int roleId)
        {
            //Network Admin is able to see the Impersonation tab if the tab is authorized for the service organization district
            var user = repositoryUser.Select().Where(x => x.Id == userId).FirstOrDefault();
            if (user != null)
            {
                //get the Module access of NetworkAdmin
                List<XLIModuleAccess> lstModuleAccessOrgDistrict =
                    _repositoryModuleAccess.Select().Where(o => o.DMDistrictId == user.DistrictId.Value).Distinct().ToList();

                //Check Item on XLIModule table
                var vSubItem = lstModuleAccessOrgDistrict.FirstOrDefault(o => o.AreaCode.Equals(ContaintUtil.Techsupport, StringComparison.CurrentCultureIgnoreCase)
                && o.ModuleCode.Equals(ContaintUtil.TechUserImpersonation, StringComparison.CurrentCultureIgnoreCase));
                if (vSubItem == null) return;

                if (vSubItem.ModuleRestrict)
                {
                    //Check AreaDistrictModule
                    var vSubItemAccessDistrict = lstModuleAccessOrgDistrict.FirstOrDefault(o =>
                                o.AreaCode.Equals(ContaintUtil.Techsupport, StringComparison.CurrentCultureIgnoreCase)
                                && o.ModuleCode.Equals(ContaintUtil.TechUserImpersonation, StringComparison.CurrentCultureIgnoreCase)
                                && o.DMDistrictId.HasValue && o.DMDistrictId.Value == user.DistrictId.Value
                                && (!o.DMExpires.HasValue || o.DMExpires.Value == false)
                                && (!o.DMEndDate.HasValue || o.DMEndDate.Value >= DateTime.UtcNow)
                                && (!o.DMStartDate.HasValue || o.DMStartDate.Value <= DateTime.UtcNow));
                    if (vSubItemAccessDistrict == null)
                        return;

                    if (vSubItemAccessDistrict.DMAllRoles != null && vSubItemAccessDistrict.DMAllRoles.Value == false)
                    {
                        //Check AreaModuleRole
                        var vSubItemAreaDMRole = lstModuleAccessOrgDistrict.FirstOrDefault(o =>
                                                o.AreaCode.Equals(ContaintUtil.Techsupport, StringComparison.CurrentCultureIgnoreCase)
                                                && o.ModuleCode.Equals(ContaintUtil.TechUserImpersonation, StringComparison.CurrentCultureIgnoreCase)
                                                && o.DMDistrictId.HasValue && o.DMDistrictId.Value == user.DistrictId.Value
                                                && (!o.DMExpires.HasValue || o.DMExpires.Value == false)
                                                && (!o.DMEndDate.HasValue || o.DMEndDate.Value >= DateTime.UtcNow)
                                                && (!o.DMStartDate.HasValue || o.DMStartDate.Value <= DateTime.UtcNow)
                                                && o.DMRoleId.HasValue && o.DMRoleId.Value == roleId);
                        if (vSubItemAreaDMRole == null)
                            return;
                    }
                }
                if (!obj.HasDisplayedItem(ContaintUtil.TechUserImpersonation))
                {
                    obj.DisplayedItems.Add(string.Format("|{0}|", ContaintUtil.TechUserImpersonation));
                    if (!obj.DisplayedItems.Contains(string.Format("|{0}|", ContaintUtil.Techsupport)))
                    {
                        obj.DisplayedItems.Add(string.Format("|{0}|", ContaintUtil.Techsupport));
                    }
                }
            }
        }
        private void CheckMenuTechSupportForOrginalUser(MenuAccessItems obj, User currentUser)
        {
            if (!obj.HasDisplayedItem(ContaintUtil.Techsupport))
            {
                if (currentUser.IsOrginalPublisher)
                {
                    obj.DisplayedItems.Add(string.Format("|{0}|", ContaintUtil.Techsupport));
                }
                else if (currentUser.IsOrginalNetworkAdmin)
                {
                    //Network Admin is able to see the Impersonation tab if the tab is authorized for the service organization district
                    CheckMenuTechSupportForNetworkAdmin(obj, currentUser.OriginalID);
                }
            }
        }
        private void CheckSubMenuImpersonateForOriginalUser(MenuAccessItems obj, User currentUser)
        {
            if (currentUser.IsOrginalPublisher)
            {
                if (!obj.HasDisplayedItem(ContaintUtil.TechUserImpersonation))
                {
                    obj.DisplayedItems.Add(string.Format("|{0}|", ContaintUtil.TechUserImpersonation));
                    if (!obj.DisplayedItems.Contains(string.Format("|{0}|", ContaintUtil.Techsupport)))
                    {
                        obj.DisplayedItems.Add(string.Format("|{0}|", ContaintUtil.Techsupport));
                    }
                }
            }
            else if (currentUser.IsOrginalNetworkAdmin)
            {
                //if original user is Network Admin, Network Admin is able to see the Impersonation tab if the tab is authorized for the service organization district
                CheckSubMenuImpersonateForNetworkAdmin(obj, currentUser.OriginalID, (int)Permissions.NetworkAdmin);

            }

        }
        public void CheckDisplayIconNew(XliArea xliArea, MenuAccessItems obj, List<XLIIconAccess> lstIconAccess)
        {
            string strAreaCode = xliArea.Code;
            string strFormatCode = string.Format("|{0}|", strAreaCode);
            //IsPublisher
            if (obj.RoleId == (int)Permissions.Publisher || !xliArea.Restrict)
            {
                obj.DisplayedItems.Add(strFormatCode);
                return;
            }
            //if (obj.RoleId == (int)Permissions.NetworkAdmin)
            //{
            //    obj.DisplayedItems.Add(strFormatCode);
            //    return;
            //}
            // check exists Icon
            var vIcon = lstIconAccess.FirstOrDefault(o => o.IconCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase));
            if (vIcon == null) return;// Not exist AreaCode on XLIArea table -> not show

            //Check AreaDistrict
            if (vIcon.Restrict)
            {
                var vHomeIconAccess = lstIconAccess.FirstOrDefault(o => o.IconCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase) && o.DistrictId.HasValue && o.DistrictId.Value == obj.DistrictId);
                if (vHomeIconAccess == null) return;

                if (!vHomeIconAccess.Expires.HasValue || vHomeIconAccess.Expires == true
                    || (vHomeIconAccess.EndDate != null && vHomeIconAccess.EndDate.Value < DateTime.UtcNow)
                    || (vHomeIconAccess.StartDate != null && vHomeIconAccess.StartDate.Value > DateTime.UtcNow)
                    )
                {
                    return; //TODO Not exist record valid.
                }
            }

            if (obj.RoleId == (int)Permissions.DistrictAdmin || obj.RoleId == (int)Permissions.Parent || obj.RoleId == (int)Permissions.Student || obj.RoleId == (int)Permissions.NetworkAdmin)
            {
                obj.DisplayedItems.Add(strFormatCode);
                return;
            }
            if (obj.RoleId != (int)Permissions.SchoolAdmin && obj.RoleId != (int)Permissions.Teacher)
            {
                return;
            }
            if (obj.RoleId == (int)Permissions.SchoolAdmin || obj.RoleId == (int)Permissions.Teacher)
            {
                //Check AreaDistrictSchool
                var vHomeAreaDistrictSchool = lstIconAccess.FirstOrDefault(o =>
                        o.IconCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase)
                        && o.SchoolDistrictId.HasValue
                        && o.SchoolDistrictId.Value == obj.DistrictId);
                if (vHomeAreaDistrictSchool == null)
                {
                    obj.DisplayedItems.Add(strFormatCode);
                    return;
                }
                if (vHomeAreaDistrictSchool.DistrictSchoolRestrict.HasValue &&
                    vHomeAreaDistrictSchool.DistrictSchoolRestrict.Value)
                {
                    //Check AreaSchool
                    var vHomeAreaSchool = lstIconAccess.FirstOrDefault(o =>
                        o.IconCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase)
                        && o.SchoolId.HasValue && obj.ListSchoolId.Contains(o.SchoolId.Value)
                        && (!o.AreaSchoolExprires.HasValue || !o.AreaSchoolExprires.Value)
                        && (!o.AreaSchoolStartDate.HasValue || o.AreaSchoolStartDate.Value <= DateTime.UtcNow)
                        && (!o.AreaSchoolEndDate.HasValue || o.AreaSchoolEndDate.Value >= DateTime.UtcNow));
                    if (vHomeAreaSchool != null)
                    {
                        if (!vHomeAreaSchool.AllRoles.HasValue || vHomeAreaSchool.AllRoles.Value)
                        {
                            obj.DisplayedItems.Add(strFormatCode);
                        }
                        else
                        {
                            //Check AreaSchoolRole
                            var vHomeAreaSchoolRole = lstIconAccess.FirstOrDefault(o =>
                            o.IconCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase)
                            && o.SchoolId.HasValue && obj.ListSchoolId.Contains(o.SchoolId.Value)
                            && (!o.AreaSchoolExprires.HasValue || !o.AreaSchoolExprires.Value)
                            && (!o.AreaSchoolStartDate.HasValue || o.AreaSchoolStartDate.Value <= DateTime.UtcNow)
                            && (!o.AreaSchoolEndDate.HasValue || o.AreaSchoolEndDate.Value >= DateTime.UtcNow)
                            && (o.RoleID.HasValue && o.RoleID.Value == obj.RoleId)
                            );
                            if (vHomeAreaSchoolRole != null)
                            {
                                obj.DisplayedItems.Add(strFormatCode);
                            }
                            //else is Deny Icon
                        }
                    }
                }
                else
                {
                    obj.DisplayedItems.Add(strFormatCode);
                }
            }
        }

        public void CheckDisplaySubItemNew(string strAreaCode, XliModule xliModule, MenuAccessItems obj, List<XLIModuleAccess> lstModuleAccess)
        {
            var strSubItemCode = xliModule.Code;
            string strFormatItem = string.Format("|{0}|", strSubItemCode);

            //IsPublisher
            if (obj.RoleId == (int)Permissions.Publisher || !xliModule.Restrict)
            {
                obj.DisplayedItems.Add(strFormatItem);
                return;
            }

            //Check Item on XLIModule table
            var vSubItem = lstModuleAccess.FirstOrDefault(o => o.AreaCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase)
                && o.ModuleCode.Equals(strSubItemCode, StringComparison.CurrentCultureIgnoreCase));
            if (vSubItem == null) return;

            if (vSubItem.ModuleRestrict)
            {
                //Check AreaDistrictModule
                var vSubItemAccessDistrict = lstModuleAccess.FirstOrDefault(o =>
                            o.AreaCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase)
                            && o.ModuleCode.Equals(strSubItemCode, StringComparison.CurrentCultureIgnoreCase)
                            && o.DMDistrictId.HasValue && o.DMDistrictId.Value == obj.DistrictId
                            && (!o.DMExpires.HasValue || o.DMExpires.Value == false)
                            && (!o.DMEndDate.HasValue || o.DMEndDate.Value >= DateTime.UtcNow)
                            && (!o.DMStartDate.HasValue || o.DMStartDate.Value <= DateTime.UtcNow));
                if (vSubItemAccessDistrict == null)
                    return;

                if (vSubItemAccessDistrict.DMAllRoles != null && vSubItemAccessDistrict.DMAllRoles.Value == false)
                {
                    //Check AreaModuleRole
                    var vSubItemAreaDMRole = lstModuleAccess.FirstOrDefault(o =>
                                            o.AreaCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase)
                                            && o.ModuleCode.Equals(strSubItemCode, StringComparison.CurrentCultureIgnoreCase)
                                            && o.DMDistrictId.HasValue && o.DMDistrictId.Value == obj.DistrictId
                                            && (!o.DMExpires.HasValue || o.DMExpires.Value == false)
                                            && (!o.DMEndDate.HasValue || o.DMEndDate.Value >= DateTime.UtcNow)
                                            && (!o.DMStartDate.HasValue || o.DMStartDate.Value <= DateTime.UtcNow)
                                            && o.DMRoleId.HasValue && o.DMRoleId.Value == obj.RoleId);
                    if (vSubItemAreaDMRole == null)
                        return;
                }
            }

            if (obj.RoleId == (int)Permissions.DistrictAdmin || obj.RoleId == (int)Permissions.Parent || obj.RoleId == (int)Permissions.Student || obj.RoleId == (int)Permissions.NetworkAdmin)
            {
                obj.DisplayedItems.Add(strFormatItem);
                return;
            }
            if (obj.RoleId == (int)Permissions.SchoolAdmin || obj.RoleId == (int)Permissions.Teacher)
            {
                //Check DistrictSchoolModule
                var vSubItemDistrictSchoolModule = lstModuleAccess.FirstOrDefault(o =>
                                                                                  o.AreaCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase)
                                                                                  && o.ModuleCode.Equals(strSubItemCode, StringComparison.CurrentCultureIgnoreCase)
                                                                                  && o.DSMDistrictId.HasValue && o.DSMDistrictId.Value == obj.DistrictId);
                if (vSubItemDistrictSchoolModule == null ||
                    (vSubItemDistrictSchoolModule.DSMRestrict.HasValue
                    && !vSubItemDistrictSchoolModule.DSMRestrict.Value))
                {
                    obj.DisplayedItems.Add(strFormatItem);
                }
                else
                {
                    //Check Record AreaSchoolModule
                    var vSubItemSchoolModule = lstModuleAccess.Where(o =>
                                                                     o.AreaCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase)
                                                                     && o.ModuleCode.Equals(strSubItemCode, StringComparison.CurrentCultureIgnoreCase)
                                                                     && (!o.SMExprires.HasValue || o.SMExprires.Value == false)
                                                                     && (!o.SMEndDate.HasValue || o.SMEndDate.Value >= DateTime.UtcNow)
                                                                     && (!o.SMStartDate.HasValue || o.SMStartDate.Value <= DateTime.UtcNow)
                                                                     && (o.SchoolId.HasValue && obj.ListSchoolId.Contains(o.SchoolId.Value)))
                                                              .ToList();
                    if (vSubItemSchoolModule.Count == 0)
                        return;
                    if (vSubItemSchoolModule.Any(o => o.SMAllRole == null) || vSubItemSchoolModule.Any(o => o.SMAllRole.HasValue && o.SMAllRole.Value))
                    {
                        obj.DisplayedItems.Add(strFormatItem);
                    }
                    else
                    {
                        //Check XLIAreaSMRole
                        var vSubItemSchoolModuleRole = lstModuleAccess.FirstOrDefault(o =>
                                        o.AreaCode.Equals(strAreaCode, StringComparison.CurrentCultureIgnoreCase)
                                         && o.ModuleCode.Equals(strSubItemCode, StringComparison.CurrentCultureIgnoreCase)
                                         && (!o.SMExprires.HasValue || o.SMExprires.Value == false)
                                         && (!o.SMEndDate.HasValue || o.SMEndDate.Value >= DateTime.UtcNow)
                                         && (!o.SMStartDate.HasValue || o.SMStartDate.Value <= DateTime.UtcNow)
                                         && (o.SchoolId.HasValue && obj.ListSchoolId.Contains(o.SchoolId.Value))
                                         && o.SMRoleId == obj.RoleId);
                        if (vSubItemSchoolModuleRole != null)
                        {
                            obj.DisplayedItems.Add(strFormatItem);
                        }
                    }
                }
            }
        }

        #endregion
        //       CheckDisplaySubItemNew(ContaintUtil.Testmanagement, ContaintUtil.TmgmtExtractTestResult, obj, lstModuleAccess);

        #region Optimage code get Icon & Tab [Use Store]
        public MenuAccessItems GetMenuAccessByDistrict(User currentUser)
        {
            int districtId = currentUser.DistrictId.GetValueOrDefault();

            var obj = new MenuAccessItems();
            obj.DistrictId = districtId;
            obj.RoleId = currentUser.RoleId;

            var lstArea = _repositoryIconAccess.GetAllXliAreasByUserId(currentUser.Id, districtId, currentUser.RoleId);

            // Get list AreaIds
            var areaIds = lstArea.Select(o => o.XliAreaId);
            var strListXliAreaIds = string.Join(",", areaIds);

            var lstModule = _repositoryModuleAccess.GetAllModulesAccessByUser(currentUser.Id, districtId, currentUser.RoleId, strListXliAreaIds);
            //Deny Flash Module
            CheckDenyFlashModule(lstModule);

            foreach (var xliArea in lstArea)
            {
                var areaHasModule = lstModule.Any(x => x.XliAreaId == xliArea.XliAreaId);
                if (areaHasModule || xliArea.XliAreaId == 13)
                {
                    obj.DisplayedItems.Add(string.Format("|{0}|", xliArea.Code));
                }
            }

            foreach (var xliModule in lstModule)
            {
                obj.DisplayedItems.Add(string.Format("|{0}|", xliModule.Code));
            }

            // Config CustomHelp link based on DistrictDecode data
            obj.CustomHelpUrl = "#";
            var districtDecode = _repositoryDistrictDecode.Select()
                .FirstOrDefault(x => x.DistrictID == districtId && x.Label == ContaintUtil.CustomHelpUrl);
            if (districtDecode != null)
            {
                obj.CustomHelpUrl = districtDecode.Value;
            }

            // Custom permission for Student and Parent
            if (currentUser.RoleId == (int)Permissions.Student)
            {
                var displayItems = MergeList(obj.DisplayedItems, GetStudentMenus());
                obj.DisplayedItems = displayItems;
            }

            if (currentUser.RoleId == (int)Permissions.Parent)
            {
                var displayItems = MergeList(obj.DisplayedItems, GetParentMenus());
                obj.DisplayedItems = displayItems;
            }

            var techSupportConfig = _repositoryConfiguration.Select().FirstOrDefault(x => x.Name == ContaintUtil.TechSupportUrl);
            var riseTechSupportConfig = _repositoryConfiguration.Select().FirstOrDefault(x => x.Name == ContaintUtil.RiseTechSupportUrl);

            var query = string.Empty;
            if (techSupportConfig != null || riseTechSupportConfig != null)
            {
                var user = repositoryUser.Select().Where(x => x.Id == currentUser.Id).FirstOrDefault();
                var district = _repositoryDistrict.Select().FirstOrDefault(x => x.Id == districtId);
                var state = _repositoryState.Select().FirstOrDefault(x => x.Id == currentUser.StateId);

                query = $"?name={user?.FirstName} {user?.LastName}&email={user?.EmailAddress}&district-name={district?.Name}&licode={district?.LICode}&state={state?.Name}";
            }

            obj.TechSupportUrl = techSupportConfig != null ? techSupportConfig.Value + query : "#";
            obj.RiseTechSupportUrl = riseTechSupportConfig != null ? riseTechSupportConfig.Value + query : "#";

            return obj;
        }

        private void CheckDenyFlashModule(List<XliModule> listXliModule)
        {
            var vDenyFlashModule = _repositoryConfiguration.Select().FirstOrDefault(o => o.Name.Equals("DenyFlashModule"));
            if (vDenyFlashModule != null &&
                vDenyFlashModule.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                var listFlashModule = _repositoryConfiguration.Select().FirstOrDefault(o => o.Name.Equals("ListFlashModuleCode"));
                if (listFlashModule != null && !string.IsNullOrEmpty(listFlashModule.Value))
                {
                    var listFlashModuleCode = listFlashModule.Value.Split(Convert.ToChar(","));
                    listXliModule.RemoveAll(o => listFlashModuleCode.Contains(o.Code));
                }
            }
        }

        #endregion


        public List<XliArea> GetMainMenus()
        {
            var homeItem = _repositoryXliArea.Select().Single(x => x.Code == ContaintUtil.Home);
            return _repositoryXliArea.Select().Where(x => x.XliAreaId >= homeItem.XliAreaId).OrderBy(x => x.AreaOrder).ToList();
        }

        public List<XliModule> GetSubMenus()
        {
            var homeItem = _repositoryXliArea.Select().Single(x => x.Code == ContaintUtil.Home);
            return _repositoryXliModule.Select().Where(x => x.XliAreaId >= homeItem.XliAreaId).OrderBy(x => x.ModuleOrder).ToList();
        }
        public XliModule GetXliModule(string displayName)
        {
            return _repositoryXliModule.Select().FirstOrDefault(x => x.DisplayName.ToLower() == displayName.ToLower());
        }

        public bool IsAccessModuleByDistrict(int districtId, string code)
        {
            var moduleAccess =
                _repositoryModuleAccess.Select()
                    .Where(x => x.DMDistrictId == districtId && x.ModuleCode == code);
            if (moduleAccess.Any())
                return true;
            return false;
        }

        private List<string> GetStudentMenus()
        {
            var menus = new List<string>()
            {
                string.Format("|{0}|", ContaintUtil.Home),
                string.Format("|{0}|", ContaintUtil.Reporting),
                string.Format("|{0}|", ContaintUtil.ReportingTestResult),
                string.Format("|{0}|", ContaintUtil.ReportingTestResultCS),
                string.Format("|{0}|", ContaintUtil.Onlinetesting),
                string.Format("|{0}|", ContaintUtil.TestLaunch),
                string.Format("|{0}|", ContaintUtil.OnlineTesting),
                string.Format("|{0}|", ContaintUtil.StudentProfile),
                string.Format("|{0}|", ContaintUtil.StudentProfileEdit),
                string.Format("|{0}|", ContaintUtil.Help),
                string.Format("|{0}|", ContaintUtil.HelpResource),
                string.Format("|{0}|", ContaintUtil.NavigatorReport),
                string.Format("|{0}|", ContaintUtil.SurveyModule),
                string.Format("|{0}|", ContaintUtil.TakeSurveys),
                string.Format("|{0}|", ContaintUtil.ResultsEntryDataLocker),
                string.Format("|{0}|", ContaintUtil.AttachmentForStudent)
            };
            return menus;
        }

        private List<string> GetParentMenus()
        {
            var menus = new List<string>()
            {
                string.Format("|{0}|", ContaintUtil.Home),
                string.Format("|{0}|", ContaintUtil.Reporting),
                string.Format("|{0}|", ContaintUtil.ReportingTestResult),
                string.Format("|{0}|", ContaintUtil.Help),
                string.Format("|{0}|", ContaintUtil.HelpResource),
                string.Format("|{0}|", ContaintUtil.NavigatorReport),
                string.Format("|{0}|", ContaintUtil.SurveyModule),
                string.Format("|{0}|", ContaintUtil.TakeSurveys),
                string.Format("|{0}|", ContaintUtil.ReportingTestResultCS)
            };
            return menus;
        }

        private List<string> MergeList(List<string> allMenus, List<string> restrictMenus)
        {
            List<string> result = null;

            if (allMenus.Count > restrictMenus.Count)
                result = allMenus.Where(x => restrictMenus.Contains(x)).ToList();
            else
                result = restrictMenus.Where(x => allMenus.Contains(x)).ToList();

            return result ?? new List<string>();
        }

    }
}
