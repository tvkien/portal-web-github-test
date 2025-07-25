using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class BaseResponseModel<TStrongData>
    {
        protected BaseResponseModel()
        {

        }
        public string Status { get; set; }
        public string Message { get; set; }
        public TStrongData StrongData { get; set; }
        public bool IsSuccess { get; set; } = false;
        public static BaseResponseModel<TStrongData> InstanceError(string message, string status = "", TStrongData data = default(TStrongData))
        {
            return new BaseResponseModel<TStrongData>()
            {
                IsSuccess = false,
                Message = message,
                Status = string.IsNullOrEmpty(status) ? "error" : status,
                StrongData = data
            };
        }
        public static BaseResponseModel<TStrongData> InstanceSuccess(TStrongData data, string message = "", string status = "")
        {
            return new BaseResponseModel<TStrongData>()
            {
                IsSuccess = true,
                Message = message,
                StrongData = data,
                Status = string.IsNullOrEmpty(status) ? "success" : status
            };
        }

    }
}
