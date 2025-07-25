using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Service;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class RequestService : PersistableModelService<Request>
    {
        private readonly IReadOnlyRepository<RequestEmailNotification> requestEmailNotificationRepository;
        private readonly List<DataRequestType> ignoreSubmitRequests = new List<DataRequestType>() {  DataRequestType.PowerSchoolFullRefresh,
                DataRequestType.OneRosterFullRefresh,
                DataRequestType.SchoolTooleScholarFullRefresh };

        public RequestService(IRepository<Request> repository, IReadOnlyRepository<RequestEmailNotification> requestEmailNotificationRepository) : base(repository)
        {
            this.requestEmailNotificationRepository = requestEmailNotificationRepository;
        }

        public Request GetRequestById(int requestId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(requestId));
        }

        public IQueryable<Request> GetRequestsByUserId(int userId)
        {
            return repository.Select().Where(x => x.UserId.Equals(userId) && !x.IsDeleted);
        }

        public Request GetRequestByIdAndUserId(int userId, int requestId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(requestId) && x.UserId.Equals(userId));
        }

        public Request CreateRequestWithFileName(User user, string filename, RequestType requestType, int districtId)
        {
            var request = new Request
            {
                RequestType = requestType,
                UserId = user.Id,
                DistrictId = districtId,
                EmailAddress = user.EmailAddress,
                ImportedFileName = filename,
                RequestTime = DateTime.UtcNow,
                RequestStatus = RequestStatus.Pending
            };

            AssignDataRequestTypeId(request);
            return request;
        }

        private void AssignDataRequestTypeId(Request request)
        {
            switch (request.RequestType)
            {
                case RequestType.StudentFullRefresh:
                    request.DataRequestType = DataRequestType.StudentFullRefresh;
                    request.RequestMode = RequestMode.Validation;
                    break;
                case RequestType.StudentAddUpdate:
                    request.DataRequestType = DataRequestType.StudentAddUpdate;
                    request.RequestMode = RequestMode.Validation;
                    break;
                case RequestType.Staff:
                    request.DataRequestType = DataRequestType.Staff;
                    request.RequestMode = RequestMode.Validation;
                    break;
                case RequestType.StaffFullRefresh:
                    request.DataRequestType = DataRequestType.StaffFullRefresh;
                    request.RequestMode = RequestMode.Validation;
                    break;
                case RequestType.StudentProgram:
                    request.DataRequestType = DataRequestType.StudentProgram;
                    request.RequestMode = RequestMode.Import;
                    break;
                case RequestType.Parent:
                    request.DataRequestType = DataRequestType.Parent;
                    request.RequestMode = RequestMode.Validation;
                    break;
                case RequestType.StudentMeta:
                    request.DataRequestType = DataRequestType.StudentMeta;
                    request.RequestMode = RequestMode.Import;
                    break;
                case RequestType.OneRosterFullRefresh:
                    request.DataRequestType = DataRequestType.OneRosterFullRefresh;
                    request.RequestMode = RequestMode.Validation;
                    break;
                case RequestType.PowerSchoolFullRefresh:
                    request.DataRequestType = DataRequestType.PowerSchoolFullRefresh;
                    request.RequestMode = RequestMode.Validation;
                    break;
                case RequestType.SchoolTooleScholarFullRefresh:
                    request.DataRequestType = DataRequestType.SchoolTooleScholarFullRefresh;
                    request.RequestMode = RequestMode.Validation;
                    break;
            }
        }

        public string GetEmailContentForRequest(int requestId)
        {
            var requestNotificationEmail = requestEmailNotificationRepository.Select().Where(x => x.RequestId.Equals(requestId)).OrderByDescending(c => c.Id).FirstOrDefault();
            return requestNotificationEmail.IsNull() ? string.Empty : requestNotificationEmail.EmailContent;
        }

        public bool CanSubmitRoster(Request request)
        {
            return !request.HasBeenMoved && !ignoreSubmitRequests.Contains(request.DataRequestType) &&
                (request.RequestStatus == RequestStatus.ValidationFailedWarnings || request.RequestStatus == RequestStatus.ValidationPassed);
        }

        public bool IsRosterSubmitted(Request request)
        {
            return request.RequestMode == RequestMode.Import && (request.RequestStatus != RequestStatus.ProcessedWithWarnings && request.RequestStatus != RequestStatus.Processed);
        }

        public Request CreateRequestForTestDataUpload(User user, string filename)
        {
            var request = new Request
            {
                DataRequestType = DataRequestType.TestDataUpload,
                UserId = user.Id,
                DistrictId = user.DistrictId.GetValueOrDefault(),
                EmailAddress = user.EmailAddress,
                ImportedFileName = filename,
                RequestTime = DateTime.UtcNow,
            };
            return request;
        }
    }
}
