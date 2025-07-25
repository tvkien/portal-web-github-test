using Amazon.CloudSearch.Model;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport
{
    public class NavigatorReportPublishRepository : INavigatorReportPublishRepository
    {

        private readonly Table<NavigatorReportPublishEntity> table;
        private readonly Table<NavigatorReportEntity> navigatorTable;
        private string _connectionString;
        private readonly NavigatorReportDataContext _navigatorReportDataContext;
        private readonly IDspDistrictRepository dspDistrictRepository;

        public NavigatorReportPublishRepository(IConnectionString conn, IDspDistrictRepository dspDistrictRepository)
        {
            this._connectionString = conn.GetLinkItConnectionString();
            _navigatorReportDataContext = new NavigatorReportDataContext(_connectionString);
            table = _navigatorReportDataContext.GetTable<NavigatorReportPublishEntity>();
            navigatorTable = _navigatorReportDataContext.GetTable<NavigatorReportEntity>();
            this.dspDistrictRepository = dspDistrictRepository;
        }
        public BaseResponseModel<PublishResultDto> PublishByRoleAndIdentifiers(string navigatorReportId, string rolesToBePublish, int UserId, int roleId, int? districtId, string excludeUserIds)
        {
            try
            {
                var publishByRoleResult = _navigatorReportDataContext.NavigatorReportPublishByRoleAndIdentifiers_Multiple(navigatorReportId, rolesToBePublish, UserId, roleId, districtId, excludeUserIds);

                return ExtractPublishResult(publishByRoleResult);
            }
            catch (Exception ex)
            {
                return BaseResponseModel<PublishResultDto>.InstanceError(ex.Message, "Error", null);
            }
        }

        private  BaseResponseModel<PublishResultDto> ExtractPublishResult(IMultipleResults publishByRoleResult)
        {
            var userAndNavigatorReportIds = publishByRoleResult.GetResult<PublishUserAndNavigatorReportIdsDto>().ToArray();
            var userInfo = publishByRoleResult.GetResult<PublishUserInformationDto>().ToArray();
            var reportInfo = publishByRoleResult.GetResult<PublishReportInformationDto>().ToArray();
            var publishDetail = publishByRoleResult.GetResult<PublishByRoleDetailDto>().FirstOrDefault();

            var publishResult = new PublishResultDto()
            {
                ReportCount = publishDetail?.ReportCount ?? 0,
                InitiatorEmail = publishDetail?.InitiatorEmail ?? "",
                InitiatorName = publishDetail?.InitiatorName ?? "",
                NewPublishRecordInformation = new NewPublishRecordInformation(userAndNavigatorReportIds, userInfo, reportInfo)

            };
            return BaseResponseModel<PublishResultDto>.InstanceSuccess(publishResult);
        }
        public BaseResponseModel<PublishResultDto> Publish(NavigatorReportPublishRequestDto model, int userId, int roleId)
        {
            try
            {
                if (model == null)
                    return BaseResponseModel<PublishResultDto>.InstanceError("Bad request");
                var publishResult = _navigatorReportDataContext.NavigatorReportPublishByUserId_Multiple(model.UserIds, model.NavigatorReportIds, model.PublishDistrictAdmin, model.PublishSchoolAdmin, model.PublishTeacher, model.PublishStudent, model.PublishTime, model.PublisherId, model.DistrictId, userId,roleId);

                return ExtractPublishResult(publishResult);

            }
            catch (Exception ex)
            {
                return BaseResponseModel<PublishResultDto>.InstanceError(ex.Message);
            }
        }
        public BaseResponseModel<bool> Unpublish(NavigatorReportUnpublishRequestDto model, int userId, int roleId)
        {
            try
            {
                if (model == null)
                    return BaseResponseModel<bool>.InstanceError("Bad request");
                var reuslt = _navigatorReportDataContext.NavigatorReportUnPublishByUserId(model.UserIds, model.NavigatorReportIds, model.PublishDistrictAdmin, model.PublishSchoolAdmin, model.PublishTeacher, model.PublishStudent, model.PublishTime, model.PublisherId, model.DistrictId, userId, roleId);
                bool isSuccess = (int)reuslt.ReturnValue == 1 ? true : false;
                return BaseResponseModel<bool>.InstanceSuccess(isSuccess);
            }
            catch (Exception ex)
            {
                return BaseResponseModel<bool>.InstanceError(ex.Message);
            }
        }

        public int CountPublishedAtLeastOneUser(IEnumerable<int> navigatorReportIds)
        {
            return navigatorReportIds.GroupBy(navigatorId => navigatorId / 1000)
                  .Select(navigatorId =>
                  {
                      var currentScopeReportIdss = navigatorId.ToArray();
                      return table.Where(c => currentScopeReportIdss.Contains(c.NavigatorReportID) && true == c.IsPublished)
                      .Select(c=>c.NavigatorReportID).Distinct().Count();
                  }
                  ).Sum();

        }

        public void Save(NavigatorReportPublishDto item)
        {
        }

        public void Delete(NavigatorReportPublishDto item)
        {
        }

        public IQueryable<T> Select<T>(Expression<Func<NavigatorReportPublishEntity, bool>> filter, Expression<Func<NavigatorReportPublishEntity, T>> selector)
        {
            var query = table.AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.Select(selector);
        }
    }
}
