using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryRequestRepository : IRepository<Request>
    {
        private readonly List<Request> table = new List<Request>();
        private static int nextUniqueId = 1;

        public InMemoryRequestRepository()
        {
            table = AddRequests();
        }

        private List<Request> AddRequests()
        {
            return new List<Request>
                {
                    new Request{Id = 1, UserId = 2, DataRequestType = DataRequestType.StudentAddUpdate, EmailAddress = "cpierce@envoc.com", ImportedFileName = "roster.txt", RequestMode = RequestMode.Validation, RequestStatus = RequestStatus.Pending, IsDeleted = false, HasBeenMoved = false },
                    new Request{Id = 2, UserId = 2, DataRequestType = DataRequestType.StudentAddUpdate, EmailAddress = "cpierce@envoc.com", ImportedFileName = "roster.txt", RequestMode = RequestMode.Validation, RequestStatus = RequestStatus.Pending, IsDeleted = false, HasBeenMoved = false },
                    new Request{Id = 3, UserId = 3, DataRequestType = DataRequestType.StudentAddUpdate, EmailAddress = "kjoiner@envoc.com", ImportedFileName = "roster.txt", RequestMode = RequestMode.Validation, RequestStatus = RequestStatus.Pending, IsDeleted = false, HasBeenMoved = false },
                    new Request{Id = 4, UserId = 3, DataRequestType = DataRequestType.StudentFullRefresh, EmailAddress = "kjoiner@envoc.com", ImportedFileName = "roster.txt",RequestMode = RequestMode.Validation, RequestStatus = RequestStatus.Pending, IsDeleted = true, HasBeenMoved = false }
                };
        }

        public IQueryable<Request> Select()
        {
            return table.AsQueryable();
        }

        public void Save(Request item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));

            if (entity.IsNull())
            {
                item.Id = nextUniqueId;
                nextUniqueId++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<Request, Request>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(Request item)
        {
            var entity = table.FirstOrDefault(x => x.Id.Equals(item.Id));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}
