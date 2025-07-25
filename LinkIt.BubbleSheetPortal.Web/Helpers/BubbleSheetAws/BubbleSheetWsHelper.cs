using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using LinkIt.BubbleService.Models.Corrections;
//using LinkIt.BubbleService.Models.Reading;
using LinkIt.BubbleService.Models.Test;
using LinkIt.BubbleSheetPortal.Web.Models.ErrorCorrection;
using Newtonsoft.Json;
using RequestSheet = LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator.RequestSheet;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws
{
    public static class BubbleSheetWsHelper
    {
        private static string ServiceUrl
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BubbleSheetWsURL"]))
                {
                    return string.Empty;
                }
                return ConfigurationManager.AppSettings["BubbleSheetWsURL"];
            }
        }

        private const string HandleRequestSheetPath = "RequestSheet/HandleRequest";
        private const string HandleRequestSheetResultPath = "RequestSheet/HandleRequestSheetResult";
        private const string GetTestImageUrlPath = "RequestSheet/GetImageUrl";
        private const string GetTestFileUrlPath = "RequestSheet/GetTestFileUrl";

        private const string UploadTestImage = "BubbleSheetUpload/Post";
        private const string GetTotalProcessingFileProcessJob = "BubbleSheetUpload/GetTotalPage";
        private const string GetTotalPageOfTestForOneStudentPath = "BubbleSheetUpload/GetTotalPageOfTestForOneStudent";

        private const string SubmitCorrectNumberOfQuestionsErrorPath =
            "ErrorCorrection/SubmitCorrectNumberOfQuestionsError";

        private const string SubmitManualBarcodeCorrectionPath = "ErrorCorrection/SubmitManualBarcodeCorrection";
        private const string SubmitResultWithBarcodePath = "ErrorCorrection/SubmitResultWithBarcode";
        private const string SubmitResultWithBarcodeGenericActSat = "ErrorCorrection/SubmitResultWithBarcodeGenericActSat";
        private const string SubmitRosterPositionErrorPath = "ErrorCorrection/SubmitRosterPositionError";
        private const string ValidateBubbleOutsideCropMarkPath = "RequestSheet/ValidateBubbleOutsideCropMark";

        private const string GetReadResultPath = "ReadResult/GetReadResult";
        private const string SendGradeRequestPath = "ReadResult/SendGradeRequest";
        private const string SendListGradeRequestPath = "ReadResult/SendListGradeRequest";
        private const string SendGradeRequestBatchPath = "ReadResult/SendGradeRequestBatch";

        public static T PostJsonRequest<T, T1>(T1 data, string serviceUrl, string environmentId = "")
        {
            using (var client = new HttpClient())
            {
                try
                {
                    if (string.IsNullOrEmpty(environmentId))
                        environmentId = LinkitConfigurationManager.Vault.DatabaseID;
                    client.DefaultRequestHeaders.Add("EnvInd", environmentId);
                    var response = client.PostAsJsonAsync(serviceUrl, data).Result;
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(responseString);
                }
                catch (WebException ex)
                {
                    var response = (HttpWebResponse)ex.Response;
                    string errorMessage = response.StatusDescription;
                    throw new BubbleSheetServiceException(errorMessage);
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    return default(T);
                }
            }
        }

        public static T GetRequest<T>(string requestUrl, bool throwException = false)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("EnvInd", LinkitConfigurationManager.Vault.DatabaseID);
                    var response = client.GetAsync(requestUrl).Result;
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(responseString);
                }
                catch (HttpRequestException)
                {
                    if (throwException) throw new GetRequestTimeoutException("Timeout");
                    return default(T);
                }
                catch (TaskCanceledException)
                {
                    if (throwException) throw new GetRequestTimeoutException("Timeout");
                    return default(T);
                }
                catch (Exception)
                {
                    if (throwException) throw new GetRequestTimeoutException("Timeout");
                    return default(T);
                }
            }
        }

        public static ApiResponse<RequestSheetResponse> HandleRequestSheet(RequestSheet requestSheet, string environmentId = "")
        {
            if (requestSheet.Orientation == null) requestSheet.Orientation = string.Empty;
            if (requestSheet.PostToOnCompleted == null) requestSheet.PostToOnCompleted = string.Empty;
            if (requestSheet.PostToOnError == null) requestSheet.PostToOnError = string.Empty;
            string serviceUrl = string.Format("{0}{1}", ServiceUrl, HandleRequestSheetPath);

            return PostJsonRequest<ApiResponse<RequestSheetResponse>, RequestSheet>(requestSheet, serviceUrl, environmentId);
        }

        public static ValidateCropMarkResponse ValidateBubbleOutsideCropMark(RequestSheet requestSheet, string environmentId = "")
        {
            if (requestSheet.IsExtraPagesOnly)
            {
                return new ValidateCropMarkResponse()
                {
                    IsValid = true
                };
            }
            string serviceUrl = string.Format("{0}{1}", ServiceUrl, ValidateBubbleOutsideCropMarkPath);
            var response = PostJsonRequest<ApiResponse<ValidateCropMarkResponse>, RequestSheet>(requestSheet, serviceUrl, environmentId);
            if (response == null || response.IsSuccess == false) return null;
            return response.Data;
        }

        public static ApiResponse<BubbleSheetProcessingRequestSheetResultResponse> HandleRequestSheetResult(
            string ticket, string apiKey)
        {
            string serviceUrl = string.Format("{0}{1}?ticket={2}&apiKey={3}", ServiceUrl, HandleRequestSheetResultPath,
                ticket, apiKey);
            return GetRequest<ApiResponse<BubbleSheetProcessingRequestSheetResultResponse>>(serviceUrl);
        }

        public static string GetTestImageUrl(string fileName, string apiKey)
        {
            string serviceUrl = string.Format("{0}{1}?fileName={2}&apiKey={3}", ServiceUrl, GetTestImageUrlPath,
                fileName, apiKey);
            var data = GetRequest<ApiResponse<string>>(serviceUrl);
            if (data == null || data.IsSuccess == false || string.IsNullOrEmpty(data.Data)) return string.Empty;
            return data.Data;
        }

        public static string GetTestFileUrl(string fileName, string apiKey)
        {
            string serviceUrl = string.Format("{0}{1}?fileName={2}&apiKey={3}", ServiceUrl, GetTestFileUrlPath,
                fileName, apiKey);
            var data = GetRequest<ApiResponse<string>>(serviceUrl);
            if (data == null || data.IsSuccess == false || string.IsNullOrEmpty(data.Data)) return string.Empty;
            return data.Data;
        }

        public static int GetTotalPages(string fileName, string apiKey)
        {
            string serviceUrl = string.Format("{0}{1}?fileName={2}&apiKey={3}", ServiceUrl, GetTotalProcessingFileProcessJob,
               HttpUtility.UrlEncode(fileName), apiKey);
            return GetRequest<int>(serviceUrl);
        }

        public static int GetTotalPageOfTestForOneStudent(string ticket, string apiKey)
        {
            string serviceUrl = string.Format("{0}{1}?ticket={2}&apiKey={3}", ServiceUrl, GetTotalPageOfTestForOneStudentPath,
               HttpUtility.UrlEncode(ticket), apiKey);
            return GetRequest<int>(serviceUrl);
        }

        public static bool CreateReadRequest(LinkIt.BubbleService.Models.Reading.ReadRequest readRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            string serviceUrl = string.Format("{0}{1}?fileName={2}&apiKey={3}&userId={4}", ServiceUrl, UploadTestImage,
              HttpUtility.UrlEncode(readRequest.Filename), readRequest.ApiKey, readRequest.UserId);

            var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
            request.Method = "POST";
            request.Headers.Add("EnvInd", LinkitConfigurationManager.Vault.DatabaseID);

            var requestStream = request.GetRequestStream();
            readRequest.FileStream.Position = 0;
            readRequest.FileStream.CopyTo(requestStream);

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return true;
                }
                else
                {
                    errorMessage = "Unknown error";
                    return false;
                }
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;
                errorMessage = response.StatusDescription;
                return false;
            }
        }

        public static ApiResponse<BoolResponse> SubmitCorrectNumberOfQuestionsError(ReadResult readResult)
        {
            string serviceUrl = string.Format("{0}{1}", ServiceUrl, SubmitCorrectNumberOfQuestionsErrorPath);
            return PostJsonRequest<ApiResponse<BoolResponse>, ReadResult>(readResult, serviceUrl);
        }

        public static ApiResponse<BoolResponse> SubmitManualBarcodeCorrection(BarcodeCorrectionRequest request)
        {
            string serviceUrl = string.Format("{0}{1}", ServiceUrl, SubmitManualBarcodeCorrectionPath);
            return PostJsonRequest<ApiResponse<BoolResponse>, BarcodeCorrectionRequest>(request, serviceUrl);
        }

        public static ApiResponse<BoolResponse> SubmitResultWithBarcode(ChangeResultBarcodeRequest request)
        {
            string serviceUrl = string.Format("{0}{1}", ServiceUrl, SubmitResultWithBarcodePath);
            return PostJsonRequest<ApiResponse<BoolResponse>, ChangeResultBarcodeRequest>(request, serviceUrl);
        }

        public static ApiResponse<BoolResponse> SubmitRosterPositionError(RosterPositionCorrectionModel data)
        {
            string serviceUrl = string.Format("{0}{1}", ServiceUrl, SubmitRosterPositionErrorPath);
            return PostJsonRequest<ApiResponse<BoolResponse>, RosterPositionCorrectionModel>(data, serviceUrl);
        }

        public static ReadResult GetReadResult(string inputFilePath, string urlSafeOutputFileName)
        {
            string serviceUrl = string.Format("{0}{1}?inputFilePath={2}&urlSafeOutputFileName={3}", ServiceUrl,
                GetReadResultPath, inputFilePath, urlSafeOutputFileName);
            ApiResponse<ReadResult> data;
            try
            {
                data = GetRequest<ApiResponse<ReadResult>>(serviceUrl, true);
            }
            catch (GetRequestTimeoutException)
            {
                throw;
            }
            if (data == null || data.IsSuccess == false)
            {
                return null;
            }
            return data.Data;
        }

        public static ApiResponse<BoolResponse> SendGradeRequest(ReadResult readResult)
        {
            string serviceUrl = string.Format("{0}{1}", ServiceUrl, SendGradeRequestPath);
            return PostJsonRequest<ApiResponse<BoolResponse>, ReadResult>(readResult, serviceUrl);
        }

        public static ApiResponse<BoolResponse> SendListGradeRequest(List<ReadResult> readResult)
        {
            string serviceUrl = string.Format("{0}{1}", ServiceUrl, SendListGradeRequestPath);
            return PostJsonRequest<ApiResponse<BoolResponse>, List<ReadResult>>(readResult, serviceUrl);
        }

        public static ApiResponse<BoolResponse> SendGradeRequestBatch(List<ReadResult> readResult)
        {
            string serviceUrl = string.Format("{0}{1}", ServiceUrl, SendGradeRequestBatchPath);
            return PostJsonRequest<ApiResponse<BoolResponse>, List<ReadResult>>(readResult, serviceUrl);
        }

        public static ApiResponse<BoolResponse> SubmitResultWithBarcodeGenericActSatAPI(List<ChangeResultBarcodeRequest> lstRequest)
        {
            string serviceUrl = string.Format("{0}{1}", ServiceUrl, SubmitResultWithBarcodeGenericActSat);
            return PostJsonRequest<ApiResponse<BoolResponse>, List<ChangeResultBarcodeRequest>>(lstRequest, serviceUrl);
        }
        public static string GetGradedCountForStudent(BubbleSheetStudentResults student, int totalTestPage)
        {
            var returnString = string.Format("{0} / {1} ({2}/{3})", student.AnsweredCount ?? 0, student.TotalCount,
                student.ProcessedPage ?? 0,
                totalTestPage);

            return returnString;
        }
    }
}
