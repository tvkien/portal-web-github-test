using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class PreferencesRepository : IPreferencesRepository
    {
        private readonly Table<PreferenceEntity> table;
        public PreferencesRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<PreferenceEntity>();
            Mapper.CreateMap<Preferences, PreferenceEntity>();
        }

        public IQueryable<Preferences> Select()
        {
            return table.Select(x => new Preferences
            {
                PreferenceId = x.PreferenceID,
                Level = x.Level,
                Id = x.ID,
                Label = x.Label,
                Value = x.Value,
                UpdatedDate = x.UpdatedDate,
                UpdatedBy = x.UpdatedBy
            });
        }

        public void Save(Preferences item)
        {
            var entity = table.FirstOrDefault(x => x.PreferenceID.Equals(item.PreferenceId));

            if (entity.IsNull())
            {
                entity = new PreferenceEntity();
                table.InsertOnSubmit(entity);
            }
            MapObject(item, entity);
            var updatedDate = DateTime.UtcNow;
            entity.UpdatedDate = updatedDate;
            table.Context.SubmitChanges();
            item.PreferenceId = entity.PreferenceID;
            item.UpdatedDate = updatedDate;
        }

        public void Delete(Preferences item)
        {
            var entity = table.FirstOrDefault(x => x.PreferenceID.Equals(item.PreferenceId));
            if (entity.IsNotNull())
            {

                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapObject(Preferences source, PreferenceEntity destination)
        {
            destination.Level = source.Level;
            destination.ID = source.Id;
            destination.Label = source.Label;
            destination.Value = source.Value;
            destination.UpdatedBy = source.UpdatedBy;
        }

        public void InsertMultipleRecord(List<Preferences> items)
        {
            foreach(var item in items)
            {
                var entity = new PreferenceEntity
                {
                    Level = item.Level,
                    ID = item.Id,
                    Label = item.Label,
                    Value = item.Value,
                    UpdatedBy = item.UpdatedBy
                };

                table.InsertOnSubmit(entity);
            }

            table.Context.SubmitChanges();
        }
    }
}
