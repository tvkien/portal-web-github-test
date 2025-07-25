using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryPrintingGroupDataRepository : IReadOnlyRepository<PrintingGroupData>
    {
        private readonly List<PrintingGroupData> table;

        public InMemoryPrintingGroupDataRepository()
        {
            table = AddPrintingGroupData();
        }

        private List<PrintingGroupData> AddPrintingGroupData()
        {
             
            return new List<PrintingGroupData> 
            {
                new PrintingGroupData{ CreatedUserID=4290, GroupName="PrintingGroup so 101", ClassName="Star", SchoolName="Clark Intermediate", DistrictTermName="Spring 10", ClassID=39202, GroupID=20 },
                new PrintingGroupData{ CreatedUserID=4290, GroupName="PrintingGroup so 101", ClassName="Reading", SchoolName="Clark Intermediate", DistrictTermName="Spring 11", ClassID=39635, GroupID=20 },
                new PrintingGroupData{ CreatedUserID=4290, GroupName="Group So 18", ClassName="Math", SchoolName="Clark Intermediate", DistrictTermName="Spring 11", ClassID=47780, GroupID=18 },
                new PrintingGroupData{ CreatedUserID=4290, GroupName="group new 1 edit ok", ClassName="Chemistry Period 1", SchoolName="Crest Elementary", DistrictTermName="Fall 07", ClassID=35164, GroupID=23 },
                new PrintingGroupData{ CreatedUserID=4290, GroupName="123 New Group 1", ClassName="Home Room", SchoolName="Lincoln High School", DistrictTermName="First Term.sys", ClassID=6561, GroupID=26 }
            };
        }

        public IQueryable<PrintingGroupData> Select()
        {
            return table.AsQueryable();
        }
    }
}
