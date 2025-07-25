using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class UserLogonService
    {
        private readonly IRepository<UserLogon> _repository;

        public UserLogonService(IRepository<UserLogon> repository)
        {
            this._repository = repository;
        }


        public UserLogon GetById(int userId)
        {
            return _repository.Select().FirstOrDefault(x => x.UserID == userId);
        }


        public void Save(UserLogon userLogon)
        {
            _repository.Save(userLogon);
        }

        public void Delete(UserLogon userLogon)
        {
            _repository.Delete(userLogon);
        }
    }
}