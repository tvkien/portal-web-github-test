using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryDistrictConfigurationRepository : IReadOnlyRepository<DistrictConfiguration>
    {
        private readonly List<DistrictConfiguration> table = new List<DistrictConfiguration>();
        private static int nextUniqueID = 1;

        public InMemoryDistrictConfigurationRepository()
        {
            table = AddDistrictConfigurations();
        }

        private List<DistrictConfiguration> AddDistrictConfigurations()
        {
            return new List<DistrictConfiguration>
                       {
                           new DistrictConfiguration { DistrictId = 0, Name = "PC-PortalLinkRaw", Value="https://demo.linkitdev.com" },
                           new DistrictConfiguration { DistrictId = 0, Name = "Use-CustomeSlideShow", Value="Yes" },
                           new DistrictConfiguration { DistrictId = 0, Name = "BS-Question-Label", Value="Question" },
                       };
        }

        public IQueryable<DistrictConfiguration> Select()
        {
            return table.AsQueryable();
        }

     
    }
}
