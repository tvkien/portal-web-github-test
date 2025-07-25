using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.ManageAccess;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    public partial class NavigatorReportDataContext
    {
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.NavigatorReportPublishByRoleAndIdentifiers")]
        [ResultType(typeof(PublishUserAndNavigatorReportIdsDto))]
        [ResultType(typeof(PublishUserInformationDto))]
        [ResultType(typeof(PublishReportInformationDto))]
        [ResultType(typeof(PublishByRoleDetailDto))]
        public IMultipleResults NavigatorReportPublishByRoleAndIdentifiers_Multiple([global::System.Data.Linq.Mapping.ParameterAttribute(Name = "NavigatorReportId", DbType = "VarChar(MAX)")] string navigatorReportId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "RolesTobePublised", DbType = "VarChar(100)")] string rolesTobePublised, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "UserId", DbType = "Int")] System.Nullable<int> userId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "RoleId", DbType = "Int")] System.Nullable<int> roleId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "DistrictId", DbType = "Int")] System.Nullable<int> districtId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "ExcludeUserIds", DbType = "VarChar(MAX)")] string excludeUserIds)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), navigatorReportId, rolesTobePublised, userId, roleId, districtId, excludeUserIds);
            return ((IMultipleResults)(result.ReturnValue));
        }
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.NavigatorReportPublishByUserId")]
        [ResultType(typeof(PublishUserAndNavigatorReportIdsDto))]
        [ResultType(typeof(PublishUserInformationDto))]
        [ResultType(typeof(PublishReportInformationDto))]
        [ResultType(typeof(PublishByRoleDetailDto))]
        public IMultipleResults NavigatorReportPublishByUserId_Multiple([global::System.Data.Linq.Mapping.ParameterAttribute(Name = "UserIds", DbType = "VarChar(MAX)")] string userIds, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "NavigatorReportIds", DbType = "VarChar(MAX)")] string navigatorReportIds, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "PublishDistrictAdmin", DbType = "Bit")] System.Nullable<bool> publishDistrictAdmin, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "PublishSchoolAdmin", DbType = "Bit")] System.Nullable<bool> publishSchoolAdmin, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "PublishTeacher", DbType = "Bit")] System.Nullable<bool> publishTeacher, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "PublishStudent", DbType = "Bit")] System.Nullable<bool> publishStudent, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "PublishTime", DbType = "DateTime")] System.Nullable<System.DateTime> publishTime, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "PublisherId", DbType = "Int")] System.Nullable<int> publisherId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "DistrictId", DbType = "Int")] System.Nullable<int> districtId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "UserId", DbType = "Int")] System.Nullable<int> userId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "RoleId", DbType = "Int")] System.Nullable<int> roleId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), userIds, navigatorReportIds, publishDistrictAdmin, publishSchoolAdmin, publishTeacher, publishStudent, publishTime, publisherId, districtId, userId, roleId);
            return ((IMultipleResults)(result.ReturnValue));
        }

        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.NavigatorGetManageAccessPopupDetail")]
        [ResultType(typeof(ManageAccessNavigatorReportNameDto))]
        [ResultType(typeof(ManageAccessNavigatorUserMappingDto))]
        public IMultipleResults NavigatorGetManageAccessPopupDetail_Multiple([global::System.Data.Linq.Mapping.ParameterAttribute(Name = "NavigatorReportId", DbType = "VarChar(MAX)")] string navigatorReportId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "UserId", DbType = "Int")] System.Nullable<int> userId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "RoleId", DbType = "Int")] System.Nullable<int> roleId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "DistrictId", DbType = "Int")] System.Nullable<int> districtId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "UserIdsToBePublished", DbType = "VarChar(MAX)")] string userIdsToBePublished)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), navigatorReportId, userId, roleId, districtId, userIdsToBePublished);
            return ((IMultipleResults)(result.ReturnValue));
        }

    }
}
