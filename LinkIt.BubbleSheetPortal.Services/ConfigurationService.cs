using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageStudent;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ConfigurationService
    {
        private readonly IReadOnlyRepository<Configuration> repository;

        public ConfigurationService(IReadOnlyRepository<Configuration> repository)
        {
            this.repository = repository;
        }

        public IQueryable<Configuration> Select()
        {
            return repository.Select();
        }

        public Configuration GetConfigurationByKey(string key)
        {
            return repository.Select().FirstOrDefault(o => o.Name.Equals(key));
        }

        public int GetConfigurationByKeyWithDefaultValue(string key, int defaultValue)
        {
            var v = repository.Select().FirstOrDefault(o => o.Name.ToUpper().Equals(key.ToUpper()));
            if (v != null)
            {
                int i = 0;
                int.TryParse(v.Value, out i);
                return i;
            }
            return defaultValue;
        }

        public string GetConfigurationByKeyWithDefaultValue(string key, string defaultValue)
        {
            var v = repository.Select().FirstOrDefault(o => o.Name.ToUpper().Equals(key.ToUpper()));
            if (v != null)
            {
                return v.Value;
            }
            return defaultValue;
        }

        public bool GetConfigurationByKeyWithDefaultValue(string key, bool defaultValue)
        {
            var v = repository.Select().FirstOrDefault(o => o.Name.ToUpper().Equals(key.ToUpper()));
            if (v != null)
            {
                bool result;
                if (bool.TryParse(v.Value, out result)) return result;
            }
            return defaultValue;
        }

        public bool GetShowAnnouncement()
        {
            try
            {
                var v = repository.Select().FirstOrDefault(o => o.Name.ToUpper().Equals(ContaintUtil.IsShowAnnouncement.ToUpper()));
                if (v != null && v.Value.Trim().ToUpper().Equals("TRUE"))
                {
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public string GetAnnouncementText()
        {
            try
            {
                var v = repository.Select().FirstOrDefault(o => o.Name.ToUpper().Equals(ContaintUtil.AnnouncementText.ToUpper()));
                if (v != null)
                {
                    return v.Value.Trim();
                }
                return string.Empty;
            }
            catch (Exception exception)
            {
                return string.Empty;
            }
        }
    }
}
