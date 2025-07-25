using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TTLConfigService
    {
        private readonly IReadOnlyRepository<TTLConfigs> repository;

        public TTLConfigService(IReadOnlyRepository<TTLConfigs> repository)
        {
            this.repository = repository;
        }

        public List<TTLConfigs> GetAllTTLConfigs()
        {
            var query = repository.Select();
            if(query.Any())
            {
                return query.ToList();
            }

            return new List<TTLConfigs>();
        }

        public int GetSGORetentionInDay()
        {
            var query = repository.Select();
            if (query.Any())
            {
                var obj = query.ToList().FirstOrDefault(o=>o.DynamoTableName.Equals(ContaintUtil.SGOManageLogTableConfigTTL, System.StringComparison.OrdinalIgnoreCase));
                if(obj != null)
                {
                    return obj.RetentionInDay;
                }
            }
            return 0;
        }

    }
}
