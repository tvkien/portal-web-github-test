using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Repositories;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ImpersonateLogService
    {
        //private readonly IRepository<ImpersonateLog> _repository;
        private readonly ImpersonateLogRepository _repository;

        public ImpersonateLogService(ImpersonateLogRepository repository)
        {
            this._repository = repository;
        }

        public IQueryable<ImpersonateLog> GetImpersonateLogsBySessionCookieGUID(string sessionCookieGUID)
        {
            return _repository.Select().Where(x => x.SessionCookieGUID.Equals(sessionCookieGUID));
        }

        public void SaveImpersonateLog(ImpersonateLog log)
        {
            if (log.IsNull())
            {
                throw new ArgumentNullException();
            }

            _repository.Save(log);
        }
        public void SaveImpersonateLog(string sessionCookieGUID, string actionType, int? orginalUserId, int currentUserId, int? impersonateUserId)
        {
            if(orginalUserId.HasValue && orginalUserId.Value==0)
            {
                orginalUserId = null;
            }
            if (impersonateUserId.HasValue && impersonateUserId.Value == 0)
            {
                impersonateUserId = null;
            }
            var log = new ImpersonateLog();
            log.SessionCookieGUID = sessionCookieGUID;
            log.ActionType = actionType;
            log.ActionTime = DateTime.UtcNow;
            log.OriginalUserId = orginalUserId;
            log.CurrentUserId = currentUserId;
            log.ImpersonatedUserId = impersonateUserId;
            _repository.Save(log);
        }

    }
}