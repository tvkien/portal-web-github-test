using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryDistrictSlideRepository : IReadOnlyRepository<DistrictSlide>
    {
        private readonly List<DistrictSlide> table = new List<DistrictSlide>();

        public InMemoryDistrictSlideRepository()
        {
            table = AddDistrictSlides();
        }

        private List<DistrictSlide> AddDistrictSlides()
        {
            return new List<DistrictSlide>
                       {
                           new DistrictSlide { DistrictId = 1,SlideOrder = 1,ImageName = "slide1.png",LinkTo = "",NewTabOpen = false},
                           new DistrictSlide { DistrictId = 1,SlideOrder = 10,ImageName = "slide2.png",LinkTo="linkit.com",NewTabOpen=false}
                       };
        }

        public IQueryable<DistrictSlide> Select()
        {
            return table.AsQueryable();
        }

       
    }
}
