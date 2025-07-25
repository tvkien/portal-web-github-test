using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class RoleService
    {
        private readonly IReadOnlyRepository<Role> repository;

        public RoleService(IReadOnlyRepository<Role> repository)
        {
            this.repository = repository;
        }

        public Role GetRoleById(int roleId)
        {
            return repository.Select().FirstOrDefault(x => x.RoleId.Equals(roleId));
        }

        public Role GetRoleByName(string name)
        {
            return repository.Select().FirstOrDefault(x => x.Name.Equals(name));
        }
    }
}