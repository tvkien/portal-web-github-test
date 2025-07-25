using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.UserGuide;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class UserGuideService
    {
        private readonly IRepository<UserSecurityCodeData> _userSecurityRepository;
        private readonly IRepository<FreshdeskLogData> _freshdeskLogRepository;

        public UserGuideService(IRepository<UserSecurityCodeData> userSecurityRepository, IRepository<FreshdeskLogData> freshdeskLogRepository)
        {
            _userSecurityRepository = userSecurityRepository;
            _freshdeskLogRepository = freshdeskLogRepository;
        }

        public FreshdeskLogData GetLastLoginLog(int userID)
        {
            var lastEntry =
               _freshdeskLogRepository.Select()
                   .Where(o => o.UserID == userID)
                   .OrderByDescending(o => o.LastLoginDate)
                   .FirstOrDefault();
            return lastEntry;
        }

        public UserSecurityCodeData IssueSecurityCode(int userID, string email)
        {
            var securityCode = new UserSecurityCodeData
            {
                Code = Guid.NewGuid().ToString().Substring(0, 8),
                Expired = false,
                UserID = userID,
                IssueDate = DateTime.UtcNow,
                Email = email
            };
            _userSecurityRepository.Save(securityCode);

            return securityCode;
        }

        public bool VerifySecurityCode(int userID, string email, string code)
        {
            if (code == null) code = string.Empty;
            code = code.Trim();
            var securityCode = QuerySecurityCodes(userID, email).OrderByDescending(o => o.IssueDate).FirstOrDefault();
            if (securityCode == null || !securityCode.Code.Equals(code)) return false;

            return true;
        }

        public void LogRedirectFreshdesk(int userID, string email)
        {
            var log = new FreshdeskLogData
            {
                UserID = userID,
                Email = email,
                LastLoginDate = DateTime.UtcNow
            };
            _freshdeskLogRepository.Save(log);
        }

        public UserSecurityCodeData GetCurrentSecurityCode(int userID, string email)
        {
            var result = QuerySecurityCodes(userID, email).FirstOrDefault();
            return result;
        }

        public IQueryable<UserSecurityCodeData> QuerySecurityCodes(int userID, string email)
        {
            var expiredDate = DateTime.UtcNow.AddMinutes(-5);
            var securityCodes =
               _userSecurityRepository
                   .Select().Where(o => o.UserID == userID && string.Compare(email, o.Email, StringComparison.OrdinalIgnoreCase) == 0 &&
                           !o.Expired && o.IssueDate >= expiredDate);

            return securityCodes;
        }

        public DateTime GetExpiredDateTime()
        {
            var expiredDate = DateTime.UtcNow.AddMinutes(-5);
            return expiredDate;
        }
    }
}