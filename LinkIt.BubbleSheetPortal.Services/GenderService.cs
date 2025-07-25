using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class GenderService
    {
        private readonly IReadOnlyRepository<Gender> repository;

        public GenderService(IReadOnlyRepository<Gender> repository)
        {
            this.repository = repository;
        }

        public IQueryable<Gender> GetAllGenders()
        {
            return repository.Select();
        }

        public string GetGenderNameByGenderID(int genderID)
        {
            var gender = repository.Select().FirstOrDefault(o => o.GenderID == genderID);
            return (gender == null) ? string.Empty : gender.Name;
        }
    }
}