using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryUserRepository : IRepository<User>
    {
        private readonly List<User> table = new List<User>();
        private int nextUniqueID = 1;

        public InMemoryUserRepository()
        {
            table = AddUsers();
            foreach (var user in table)
            {
                user.HashedPassword = Crypto.HashPassword(user.Password);
            }
        }

        private List<User> AddUsers()
        {
            return new List<User>
                       {
                           new User { Id = 1, EmailAddress = "user1@envoc.com", Name = "User 1", UserName = "user1", Password = "password", DistrictId = 7, RoleId = 6, HashedPassword = "AF9EdPRtvEyA8/nIwdqjmGoPyIYvRgpZVWJxBM8LrDhqVNDaERaz3CW1g6FuaClT6g=="},
                           new User { Id = 2, EmailAddress = "user2@envoc.com", Name = "User 2", UserName = "user2", Password = "password", DistrictId = 44, RoleId = 5 },
                           new User { Id = 3, EmailAddress = "user3@envoc.com", Name = "User 3", UserName = "user3", Password = "password", DistrictId = 44, RoleId = 8, PasswordQuestion = "What comes after Monday", PasswordAnswer = "Tuesday" },
                           new User { Id = 4, EmailAddress = "user4@envoc.com", Name = "User 4", UserName = "user4", Password = "password", DistrictId = 44, RoleId = 15 },
                           new User { Id = 5, EmailAddress = "user5@envoc.com", Name = "User 5", UserName = "user5", Password = "password", DistrictId = 79, RoleId = 3, UserStatusId = 1},
                           new User { Id = 100, EmailAddress = "user100@envoc.com", Name = "User 100", UserName = "user100", Password = "password", DistrictId = 1, RoleId = 3 },
                       };
        }

        public IQueryable<User> Select()
        {
            return table.AsQueryable();
        }

        public void Save(User item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));

            if (entity.IsNull())
            {
                item.Id = nextUniqueID;
                nextUniqueID++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<User, User>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(User item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}