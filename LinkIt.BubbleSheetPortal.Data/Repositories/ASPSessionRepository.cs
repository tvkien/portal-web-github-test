using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ASPSessionRepository : IASPSessionRepository
    {
        private readonly Table<ASPsessionEntity> table;
        public ASPSessionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<ASPsessionEntity>();
            Mapper.CreateMap<User, UserEntity>();
        }

        public IQueryable<ASPSession> Select()
        {
            return table.Select(x => new ASPSession
                                {
                                    ASPSessionTokenID = x.ASPSessionTokenID,
                                    expires = x.expires,
                                    LoginID = x.LoginID,
                                    SessionToken = x.SessionToken,
                                    CKSession = x.CKSession,
                                    UserID = x.UserID,
                                    UserName = x.UserName,
                                });
        }

        public void Save(ASPSession item)
        {
            var entity = table.FirstOrDefault(x => x.ASPSessionTokenID.Equals(item.ASPSessionTokenID));

            if (entity == null)
            {
                entity = new ASPsessionEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.ASPSessionTokenID = entity.ASPSessionTokenID;
        }

        public void Delete(ASPSession item)
        {
            var entity = table.FirstOrDefault(x => x.ASPSessionTokenID.Equals(item.ASPSessionTokenID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(ASPSession item, ASPsessionEntity entity)
        {
            entity.ASPSessionTokenID = item.ASPSessionTokenID;
            entity.expires = item.expires;
            entity.LoginID = item.LoginID;
            entity.SessionToken = item.SessionToken;
            entity.CKSession = item.CKSession;
            entity.UserID = item.UserID;
            entity.UserName = item.UserName;
        }
    }
}
