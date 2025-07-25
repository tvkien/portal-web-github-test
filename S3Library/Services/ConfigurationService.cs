using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3Library.Entities;

namespace S3Library.Services
{
    public class ConfigurationService : BaseService
    {
        public ConfigurationService(string connectionString) : base(connectionString)
        {
        }

        public Configuration GetByNameAndType(string name, int type)
        {
            var result = _context.Configurations.FirstOrDefault(o => o.Name == name && o.Type == type);
            return result;
        }
    }
}
