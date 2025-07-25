using System;
using System.Linq;
using System.Net.Sockets;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class UserMetaService
    {
        private readonly IRepository<UserMeta> repository;

        public UserMetaService(IRepository<UserMeta> repository)
        {
            this.repository = repository;
        }

        public IQueryable<UserMeta> Select()
        {
            return repository.Select();
        }

        public UserMeta GetByUserId(int userId, string metaLabel)
        {
            return repository.Select().FirstOrDefault(x => x.UserId == userId && x.MetaLabel == metaLabel);
        }

        public void Save(UserMeta userMeta)
        {
            userMeta.MetaLabel = userMeta.MetaLabel ?? "NOTIFICATION";
            repository.Save(userMeta);
        }

        //public TLDSUserConfigurations GetTLDSUserConfigurations(int userId)
        //{
        //    var userMeta = repository.Select().FirstOrDefault(x => x.UserId == userId);
        //    if (userMeta != null)
        //    {
        //        if (userMeta.UserMetaValue != null && userMeta.UserMetaValue.TLDSUserConfigurations != null)
        //        {
        //            return userMeta.UserMetaValue.TLDSUserConfigurations;
        //        }
        //    }
        //    return new TLDSUserConfigurations();
        //}

        //public void SaveTLDSUserConfigurations(int userId, TLDSUserConfigurations TLDSUserConfigurations)
        //{
        //    var userMeta = repository.Select().FirstOrDefault(x => x.UserId == userId);
        //    if (userMeta == null)
        //    {
        //        userMeta = new UserMeta();
        //    }
        //    if (userMeta.UserMetaValue == null)
        //    {
        //        userMeta.UserMetaValue = new UserMetaValue();
        //    }
        //    var userMetaValue = userMeta.UserMetaValue;
        //    userMetaValue.TLDSUserConfigurations = TLDSUserConfigurations;
        //    userMeta.UserMetaValue = userMetaValue;
        //    Save(userMeta);
        //}
    }
}
