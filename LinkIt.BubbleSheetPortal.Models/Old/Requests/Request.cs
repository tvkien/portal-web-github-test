using System;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models.Requests
{
    public class Request : ValidatableEntity<Request>
    {
        private string importedFileName = string.Empty;
        private string emailAddress = string.Empty;
        private string rosterType = string.Empty;

        public int Id { get; set; }
        public int UserId { get; set; }
        public int DistrictId { get; set; }
        public DateTime RequestTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool HasBeenMoved { get; set; }
        public bool HasEmailContent { get; set; }
        public RequestType RequestType { get; set; }
        public DataRequestType DataRequestType { get; set; }
        public RequestMode RequestMode { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public string DistrictName { get; set; }


        public string ImportedFileName
        {
            get { return importedFileName; }
            set { importedFileName = value.ConvertNullToEmptyString(); }
        }

        public string EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value.ConvertNullToEmptyString(); }
        }

        public string RosterType
        {
            get { return rosterType; }
            set { rosterType = value.ConvertNullToEmptyString(); }
        }
    }
}
