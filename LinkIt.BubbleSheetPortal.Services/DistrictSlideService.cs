using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using System.CodeDom;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DistrictSlideService
    {
        private readonly IReadOnlyRepository<DistrictSlide> repository;

        public DistrictSlideService(IReadOnlyRepository<DistrictSlide> repository)
        {
            this.repository = repository;
        }

        public List<DistrictSlide> GetDistrictSlides(int districtId, bool? includeStudentSlide = false)
        {
            if (includeStudentSlide == false)
            {
                var data = repository.Select().Where(x => x.DistrictId == districtId && x.RoleID == null).OrderBy(x => x.SlideOrder).ToList();
                return data.ToList();
            }
            else
            {
                return repository.Select().Where(x => x.DistrictId == districtId && x.RoleID != (int)Permissions.Parent).OrderBy(x => x.SlideOrder).ToList();    
            }

        }

        public List<DistrictSlide> GetDistrictParentlides(int districtId, bool? includeStudentSlide = false)
        {
            if (includeStudentSlide == false)
            {
                var data = repository.Select().Where(x => x.DistrictId == districtId && x.RoleID == (int)Permissions.Parent).OrderBy(x => x.SlideOrder).ToList();
                return data.ToList();
            }
            else
            {
                return repository.Select().Where(x => x.DistrictId == districtId && (x.RoleID == (int)Permissions.Parent || x.RoleID == (int)Permissions.Student)).OrderBy(x => x.SlideOrder).ToList();
            }            
        }

        public List<DistrictSlide> GetDistrictStudentSlides(int districtId)
        {
            return repository.Select().Where(x => x.DistrictId == districtId && x.RoleID == (int)Permissions.Student).OrderBy(x => x.SlideOrder).ToList();
        }
    }
}
