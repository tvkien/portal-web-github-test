using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        bool AddNewUser(User item);
    }

    public class UserRepository : IUserRepository
    {
        private readonly Table<UserEntity> table;
        private readonly UserDataContext _userDataContext;

        public UserRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _userDataContext = UserDataContext.Get(connectionString);
            table = _userDataContext.GetTable<UserEntity>();
            Mapper.CreateMap<User, UserEntity>();
        }

        public IQueryable<User> Select()
        {
            return table.Select(user => new User
                {
                    EmailAddress = user.Email,
                    Id = user.UserID,
                    DistrictId = user.DistrictID,
                    StateId = user.StateID,
                    RoleId = user.RoleID,
                    Privileges = user.Privileges,
                    Hint = user.Hint,
                    Name = user.NameFirst + " " + user.NameLast,
                    UserName = user.UserName,
                    FirstName = user.NameFirst,
                    LastName = user.NameLast,
                    PhoneNumber = user.Phone,
                    LocalCode = user.Code,
                    StateCode = user.AltCode,
                    DistrictGroupId = user.DistrictGroupID,
                    UserStatusId = user.UserStatusID,
                    AddedByUserId = user.AddedByUserID,
                    ApiAccess = user.APIaccess,
                    SchoolId = user.SchoolID,
                    HashedPassword = user.HashedPassword,
                    PasswordQuestion = user.PasswordQuestion,
                    PasswordAnswer = user.PasswordAnswer,
                    Active = user.Active,
                    HasTemporaryPassword = user.HasTemporaryPassword,
                    LastLoginDate = user.LastLoginDate,
                    PasswordLastSetDate = user.PasswordLastSetDate,
                    DateConfirmedActive = user.DateConfirmedActive,
                    ModifiedDate = user.ModifiedDate,
                    MessageNumber = user.MessageNumber,       
                    CreatedDate = user.CreatedDate,
                    ModifiedBy = user.ModifiedBy,
                    ModifiedUser = user.ModifiedUser,
                    TermOfUseAccepted = user.TermOfUseAccepted,
                    SISID = user.SISID
            });
        }

        public void Save(User item)
        {
            var entity = table.FirstOrDefault(x => x.UserID.Equals(item.Id));

            if(entity.IsNull())
            {
                entity = new UserEntity();
                table.InsertOnSubmit(entity);
            }

            BindUserEntityToUserItem(entity, item);
            table.Context.SubmitChanges();
            item.Id = entity.UserID;
        }

        public void Delete(User item)
        {
            var entity = table.FirstOrDefault(x => x.UserID.Equals(item.Id));

            if(!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public bool AddNewUser(User item)
        {
            item.Id = _userDataContext.AddNewUser(item.EmailAddress, item.DistrictId, item.StateId, item.RoleId, item.UserName,
                item.FirstName, item.LastName, item.LocalCode, item.HashedPassword, item.LastLoginDate, item.PasswordLastSetDate, item.SISID);

            return item.Id > 0;
        }

        private void BindUserEntityToUserItem(UserEntity userEntity, User item)
        {
            userEntity.Email = item.EmailAddress;
            userEntity.DistrictID = item.DistrictId;
            userEntity.StateID = item.StateId;
            userEntity.RoleID = item.RoleId;
            userEntity.UserName = item.UserName;
            userEntity.NameFirst = item.FirstName;
            userEntity.NameLast = item.LastName;
            userEntity.Phone = item.PhoneNumber;
            userEntity.Code = item.LocalCode;
            userEntity.AltCode = item.StateCode;
            userEntity.Privileges = item.Privileges;
            userEntity.Hint = item.Hint;
            userEntity.SchoolID = item.SchoolId;
            userEntity.DistrictGroupID = item.DistrictGroupId;
            userEntity.UserStatusID = item.UserStatusId;
            userEntity.AddedByUserID = item.AddedByUserId;
            userEntity.APIaccess = item.ApiAccess;
            userEntity.HashedPassword = item.HashedPassword;
            userEntity.PasswordQuestion = item.PasswordQuestion;
            userEntity.PasswordAnswer = item.PasswordAnswer;
            userEntity.Active = item.Active;
            userEntity.HasTemporaryPassword = item.HasTemporaryPassword;
            userEntity.LastLoginDate = item.LastLoginDate;
            userEntity.PasswordLastSetDate = item.PasswordLastSetDate;
            userEntity.DateConfirmedActive = item.DateConfirmedActive;
            userEntity.MessageNumber = item.MessageNumber;

            userEntity.ModifiedDate = item.ModifiedDate;
            userEntity.CreatedDate = item.CreatedDate;

            userEntity.ModifiedBy = item.ModifiedBy;
            userEntity.ModifiedUser = item.ModifiedUser;
            userEntity.TermOfUseAccepted = item.TermOfUseAccepted;
            userEntity.SISID = item.SISID;
        }
    }
}
