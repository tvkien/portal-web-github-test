using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.Loaders
{
    public class AssessmentResponseLoader
    {
        private int DistrictId;
        private string StrTestResultIds;
        private ManualResetEvent _doneEvent;
        public List<Export_ASSESSMENT_RESPONSE_NewResult> Results;
        public string ConnectionString;

        internal AssessmentResponseLoader(string conn, int districtId, string strTestResultIds, ManualResetEvent doneEvent)
        {
            DistrictId = districtId;
            StrTestResultIds = strTestResultIds;
            _doneEvent = doneEvent;
            ConnectionString = conn;
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            Results = GetTestResults(DistrictId, StrTestResultIds);
            _doneEvent.Set();
        }

        public List<Export_ASSESSMENT_RESPONSE_NewResult> GetTestResults(int districtId, string strTestResultIds)
        {
            using (var context = new ExtractTestDataContext(ConnectionString))
            {
                var lstExportAssessmentItemResponseResult = context.Export_ASSESSMENT_RESPONSE_New(strTestResultIds, districtId).ToList();
                return lstExportAssessmentItemResponseResult;
            }
        }
    }
}
