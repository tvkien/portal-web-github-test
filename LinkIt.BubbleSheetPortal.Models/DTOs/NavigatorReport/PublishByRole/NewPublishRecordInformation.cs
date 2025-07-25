using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole
{
    public class NewPublishRecordInformation
    {
        private IEnumerable<PublishUserAndNavigatorReportIdsDto> _userAndNavigatorReportIds;
        private IEnumerable<PublishUserInformationDto> _usersInfos;
        private IEnumerable<PublishReportInformationDto> _reportInfos;
        public NewPublishRecordInformation(IEnumerable<PublishUserAndNavigatorReportIdsDto> userAndNavigatorReport, IEnumerable<PublishUserInformationDto> userInfors, IEnumerable<PublishReportInformationDto> reportInfos)
        {
            this._userAndNavigatorReportIds = userAndNavigatorReport;
            this._usersInfos = userInfors;
            this._reportInfos = reportInfos;

        }
        public PublishByRoleEmailListDto ToPublishByRoleEmailList(bool isPublisher, string[] emailTo, string[] emailCC, bool ignoreEmailToCheck = false)
        {
            var userinfors = this._usersInfos.Where(c => !string.IsNullOrEmpty(c.Email)).ToArray();
            if (!ignoreEmailToCheck)
            {
                if (isPublisher)
                {
                    var teacherStudentRoleIds = new List<int>()
                {
                    (int)RoleEnum.Teacher,
                    (int)RoleEnum.Student
                };

                    if (emailTo == null) emailTo = new string[0];

                    emailTo = emailTo.Select(c => c.ToLower()).ToArray();

                    userinfors = userinfors.Where(c => emailTo.Contains(c.Email?.ToLower()) || teacherStudentRoleIds.Contains(c.RoleId)).ToArray();

                    var lowerUserEmails = userinfors.Select(us => us.Email.ToLower()).ToArray();

                    var customSendToEmails = emailTo.Where(c => !lowerUserEmails.Contains(c.ToLower())).ToArray();

                    emailCC = (emailCC ?? new string[0]).Concat(customSendToEmails).GroupBy(c => c.ToLower()).Select(c => c.FirstOrDefault())
                        .Where(c => !string.IsNullOrEmpty(c)).ToArray();

                    if (userinfors.Count() == 0)
                    {
                        return new PublishByRoleEmailListDto();
                    }
                }

            }
            var userEmails = userinfors
                 .Select(userInfo => new
                 {
                     UserInfo = userInfo,
                     ReportIds = GetAllRelatedReportIds(userInfo).OrderBy(c => c).ToArray()
                 })
                 .Where(c => c.ReportIds?.Length > 0);

            var normalUserEmails = userEmails
                .Select(user => new PublishByRoleNormalUserEmailDto()
                {
                    UserInfo = user.UserInfo,
                    Reports = GetReportByIds(user.ReportIds)
                }).ToArray();

            return new PublishByRoleEmailListDto()
            {
                NormalUserEmailList = normalUserEmails
            };
        }

        private PublishReportInformationDto[] GetReportByIds(int[] reportIds)
        {
            return this._reportInfos.Where(ri => reportIds.Contains(ri.NavigatorReportId)).ToArray();
        }

        private int[] GetAllRelatedReportIds(PublishUserInformationDto userInfo)
        {
            return this._userAndNavigatorReportIds.Where(un => un.UserId == userInfo.UserId).SelectMany(un => un.NavigatorReportIds?.ToIntArray() ?? new int[0])?.ToArray() ?? new int[0];
        }

        public PublishByRoleEmailListDto ToPublishByRoleEmailList(bool isPublisher, object emailTo, string[] emailCC)
        {
            throw new NotImplementedException();
        }
    }
}
