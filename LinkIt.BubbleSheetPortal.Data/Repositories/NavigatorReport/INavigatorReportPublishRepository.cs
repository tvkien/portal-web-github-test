using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport
{
    public interface INavigatorReportPublishRepository
    {
        BaseResponseModel<PublishResultDto> PublishByRoleAndIdentifiers(string navigatorReportId, string rolesToBePublish, int UserId, int roleId, int? districtId, string excludeUserIds);
        BaseResponseModel<PublishResultDto> Publish(NavigatorReportPublishRequestDto model, int userId, int roleId);
        BaseResponseModel<bool> Unpublish(NavigatorReportUnpublishRequestDto model, int userId, int roleId);

        int CountPublishedAtLeastOneUser(IEnumerable<int> navigatorReportIds);
        IQueryable<T> Select<T>(Expression<Func<NavigatorReportPublishEntity, bool>> filter, Expression<Func<NavigatorReportPublishEntity, T>> selector);
    }
}
