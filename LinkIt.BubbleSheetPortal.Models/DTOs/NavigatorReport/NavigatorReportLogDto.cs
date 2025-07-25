using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportLogDto
    {
        public int? NavigatorReportID;

        public int? NavigatorReportDetailID;

        public DateTime LogTime;

        public string Message;
        protected NavigatorReportLogDto()
        {

        }
        public static NavigatorReportLogDto FromMessage(int navigatorReportID, int? navigatorReportDetailID = null, string message = "")
        {
            return new NavigatorReportLogDto()
            {
                NavigatorReportDetailID = navigatorReportDetailID,
                NavigatorReportID = navigatorReportID,
                LogTime = DateTime.UtcNow,
                Message = message
            };
        }

        public static string CombineMessage(List<NavigatorReportLogDto> _lstReportDetailError)
        {
            return _lstReportDetailError == null ? "" : string.Join(Environment.NewLine, _lstReportDetailError.Select(c => $"- {c.Message}"));
        }
    }
}
