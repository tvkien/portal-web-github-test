using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.DistrictReferenceData
{
    public class InMemoryRequestParameterDistrictRepository: IReadOnlyRepository<RequestParameterDistrict>
    {
        private readonly List<RequestParameterDistrict> table;

        public InMemoryRequestParameterDistrictRepository()
        {
            table = AddRequestParameterDistricts();
        }

        private List<RequestParameterDistrict> AddRequestParameterDistricts()
        {
            return new List<RequestParameterDistrict>()
            {
                new RequestParameterDistrict{ DistrictID = 1, Name = "achievelevelsetID", RequestID = 1219, RequestParameterID = 1, Value = "1" },
                new RequestParameterDistrict{ DistrictID = 1, Name = "achievelevelsetID", RequestID = 1219, RequestParameterID = 2, Value = "2" },
                new RequestParameterDistrict{ DistrictID = 1, Name = "achievelevelsetID", RequestID = 1219, RequestParameterID = 3, Value = "3" },
                new RequestParameterDistrict{ DistrictID = 1, Name = "achievelevelsetID", RequestID = 1219, RequestParameterID = 4, Value = "4" },
                new RequestParameterDistrict{ DistrictID = 1, Name = "achievelevelsetID", RequestID = 1219, RequestParameterID = 5, Value = "5" },
                new RequestParameterDistrict{ DistrictID = 1, Name = "achievelevelsetID", RequestID = 1219, RequestParameterID = 6, Value = "6" },
                new RequestParameterDistrict{ DistrictID = 1, Name = "achievelevelsetID", RequestID = 1219, RequestParameterID = 7, Value = "7" },
                new RequestParameterDistrict{ DistrictID = 1, Name = "achievelevelsetID", RequestID = 1219, RequestParameterID = 8, Value = "8" },
                new RequestParameterDistrict{ DistrictID = 1, Name = "achievelevelsetID", RequestID = 1219, RequestParameterID = 9, Value = "9" },
                new RequestParameterDistrict{ DistrictID = 1, Name = "achievelevelsetID", RequestID = 1219, RequestParameterID = 10, Value = "10" }
            };
        }

        public IQueryable<RequestParameterDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}
