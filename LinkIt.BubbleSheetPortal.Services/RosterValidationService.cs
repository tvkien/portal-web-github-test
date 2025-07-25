using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.Old.XpsQueue;
using LinkIt.BubbleSheetPortal.Models.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class RosterValidationService
    {
        private readonly XpsDistrictUploadService _xpsDistrictUploadService;
        private readonly IValidator<XpsQueue> _xpsQueueValidator;
        private readonly IValidator<Request> _requestValidator;
        private readonly IValidator<RequestParameter> _requestParameterValidator;
        private readonly RequestService _requestService;
        private readonly XpsQueueService _xpsQueueService;
        private readonly RequestParameterService _requestParameterService;
        public RosterValidationService(XpsDistrictUploadService xpsDistrictUploadService,
            IValidator<XpsQueue> xpsQueueValidator,
            IValidator<Request> requestValidator,
            IValidator<RequestParameter> requestParameterValidator,
            RequestService requestService,
            XpsQueueService xpsQueueService,
            RequestParameterService requestParameterService)
        {
            _xpsDistrictUploadService = xpsDistrictUploadService;
            _xpsQueueValidator = xpsQueueValidator;
            _requestValidator = requestValidator;
            _requestParameterValidator = requestParameterValidator;
            _requestService = requestService;
            _xpsQueueService = xpsQueueService;
            _requestParameterService = requestParameterService;
        }

        public RosterValidationDto InitRosterValidationRequest(IEnumerable<HttpPostedFileBase> postedFiles,
                int? stateId, int? districtId, int? requestTypeId, User currentUser)
        {
            if (postedFiles == null || !postedFiles.Any())
            {
                return new RosterValidationDto() { Message = "Invalid file, please try again.", Success = false  };
            }

            var extensions = postedFiles.Select(file => Path.GetExtension(file.FileName)).ToList();

            bool containBothZipAndTextFile = extensions.Any(x => x.EqualSupportNull(".zip", isIgnoreCase: true)) &&
                                       (extensions.Any(x => x.EqualSupportNull(".txt", isIgnoreCase: true)) ||
                                        extensions.Any(x => x.EqualSupportNull(".csv", isIgnoreCase: true)));

            if (containBothZipAndTextFile)
            {
                return new RosterValidationDto
                {
                    Message = "Unsupported file extension. " +
                    "We only support file extensions of one of the two specified types ('.txt', '.csv') OR ('.zip'). " +
                    "Simultaneous support for multiple extensions is not available.",
                    Success = false,
                };
            }

            if (extensions.Where(e => e.EqualSupportNull(".zip", isIgnoreCase: true)).Count() > 1)
            {
                return new RosterValidationDto
                {
                    Message = "The batch upload contains multiple files in a ZIP format. Please only select only ONE file zip",
                    Success = false,
                };
            }

            if (!requestTypeId.HasValue || !Enum.IsDefined(typeof(RequestType), requestTypeId))
            {
                return new RosterValidationDto
                {
                    Message = "Invalid request type, please try again.",
                    Success = false,
                };
            }

            var xpsUpLoadTypeID = GetXpsUploadTypeByRequestType(requestTypeId.Value);
            if (xpsUpLoadTypeID == null)
            {
                return new RosterValidationDto
                {
                    Message = "Invalid request type, please try again.",
                    Success = false,
                };
            }

            var xpsDistrictUploads = _xpsDistrictUploadService.GetXpsDistrictUploadByUploadTypeId(districtId.GetValueOrDefault(), xpsUpLoadTypeID.Value).ToList();
            if (!xpsDistrictUploads.Any())
            {
                return new RosterValidationDto
                {
                    Message = $"The district must be configured with an XpsDistrictUpload that has the corresponding UploadTypeId set \"{xpsUpLoadTypeID}\".",
                    Success = false,
                };
            }

            var xpsDistrictUploadGroup = xpsDistrictUploads.GroupBy(m => m.ClassNameType).ToList();
            if (xpsDistrictUploadGroup.Count > 1)
            {
                return new RosterValidationDto
                {
                    Message = "There are multiple configurations of 'xpsDistrictUpload', with different 'ClassNameType' values.",
                    Success = false,
                };
            }

            var xpsQueue = new XpsQueue
            {
                XpsQueueStatusID = null,
                SchedStart = DateTime.UtcNow.AddSeconds(-10),
                XpsDistrictUploadID = xpsDistrictUploads.First().xpsDistrictUploadID,
                XpsUpLoadTypeID = xpsUpLoadTypeID,
                IsValidation = true,
            };

            xpsQueue.SetValidator(_xpsQueueValidator);
            if (!xpsQueue.IsValid)
            {
                return new RosterValidationDto
                {
                    Message = "An error has occured, please try again.",
                    Success = false,
                };
            }
            _xpsQueueService.Insert(xpsQueue);

            var request = _requestService.CreateRequestWithFileName(currentUser, "--EMPTY FILE NAME--", (RequestType)requestTypeId, districtId.GetValueOrDefault());

            request.SetValidator(_requestValidator);
            if (!request.IsValid)
            {
                return new RosterValidationDto
                {
                    Message = "An error has occured, please try again.",
                    Success = false,
                };
            }

            _requestService.Insert(request);

            CreateRequestParameter(request, "FileName", $"{request.Id}.zip");
            CreateRequestParameter(request, "XpsQueueId", xpsQueue.XpsQueueID.ToString());

            string importFileName = $"batch_request_id_{request.Id}.zip";
            bool isSingleZipFile = extensions.Count == 1 && extensions[0].Equals(".zip");
            if (isSingleZipFile)
            {
                importFileName = postedFiles.First().FileName;
            }

            /*** Update Request about ImportFileName */
            request.ImportedFileName = importFileName;
            _requestService.Update(request);

            return new RosterValidationDto { Request = request, XpsQueue = xpsQueue, Success = true };
        }

        public void MakeQueueCanStart(XpsQueue xpsQueue)
        {
            xpsQueue.XpsQueueStatusID = (int)XpsQueueStatus.Pending;
            xpsQueue.XpsQueueResultID = (int)XpsQueueResult.NotStarted;
            _xpsQueueService.Update(xpsQueue);
        }

        public int? GetClassNameType(int districtId, XpsUploadType xpsUpLoadTypeID)
        {
            var classNameType = _xpsDistrictUploadService.GetXpsDistrictUploadByUploadTypeId(districtId, (int)xpsUpLoadTypeID)
                .Where(x => x.Run)
                .OrderBy(x => x.ScheduledTime)
                .Select(x => x.ClassNameType)
                .FirstOrDefault();

            return classNameType;
        }

        private void CreateRequestParameter(Request request, string name, string value)
        {
            var requestParamter = new RequestParameter
            {
                RequestId = request.Id,
                Name = name,
                Value = value
            };

            requestParamter.SetValidator(_requestParameterValidator);
            if (requestParamter.IsValid)
            {
                _requestParameterService.Insert(requestParamter);
            }
        }

        private static int? GetXpsUploadTypeByRequestType(int requestTypeId)
        {
            switch (requestTypeId)
            {
                case (int)RequestType.PowerSchoolFullRefresh:
                    return (int)XpsUploadType.PowerSchool;
                case (int)RequestType.SchoolTooleScholarFullRefresh:
                    return (int)XpsUploadType.EScholarTemplate;
                case (int)RequestType.OneRosterFullRefresh:
                    return (int)XpsUploadType.OneRoster;
                default:
                    return null;
            }
        }
    }
}
