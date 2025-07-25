using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ASPSessionService
    {
        private readonly IASPSessionRepository _repository;

        public ASPSessionService(IASPSessionRepository repository)
        {
            this._repository = repository;
        }
    }
}