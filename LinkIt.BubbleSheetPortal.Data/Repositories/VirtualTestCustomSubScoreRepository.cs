using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualTestCustomSubScoreRepository : IRepository<VirtualTestCustomSubScore>
    {
        private readonly Table<VirtualTestCustomSubScoreEntity> table;

        public VirtualTestCustomSubScoreRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DataLockerContextDataContext.Get(connectionString).GetTable<VirtualTestCustomSubScoreEntity>();
        }

        public IQueryable<VirtualTestCustomSubScore> Select()
        {
            return table.Select(x => new VirtualTestCustomSubScore
            {
                VirtualTestCustomSubScoreId = x.VirtualTestCustomSubScoreID,
                VirtualTestCustomScoreId = x.VirtualTestCustomScoreID,
                CustomA1Label = x.CustomA_1_Label,
                CustomA2Label = x.CustomA_2_Label,
                CustomA3Label = x.CustomA_3_Label,
                CustomA4Label = x.CustomA_4_Label,
                CustomN1Label = x.CustomN_1_Label,
                CustomN2Label = x.CustomN_2_Label,
                CustomN3Label = x.CustomN_3_Label,
                CustomN4Label = x.CustomN_4_Label,
                Name = x.Name,
                UseAchievementLevel = x.UseAchievementLevel,
                UseCustomA1 = x.UseCustomA_1,
                UseCustomA2 = x.UseCustomA_2,
                UseCustomA3 = x.UseCustomA_3,
                UseCustomA4 = x.UseCustomA_4,
                UseCustomN1 = x.UseCustomN_1,
                UseCustomN2 = x.UseCustomN_2,
                UseCustomN3 = x.UseCustomN_3,
                UseCustomN4 = x.UseCustomN_4,
                UsePercent = x.UsePercent.HasValue ? x.UsePercent.Value : false,
                UsePercentile = x.UsePercentile.HasValue ? x.UsePercentile.Value : false,
                UseRaw = x.UseRaw.HasValue ? x.UseRaw.Value : false,
                UseScaled = x.UseScaled.HasValue ? x.UseScaled.Value : false,
                AchievementLevelSettingId = x.AchievementLevelSettingID,
                Sequence = x.Sequence,
                ItemTagID = x.ItemTagID,
                CustomA1ValueList = x.CustomA_1_ValueList,
                CustomA2ValueList = x.CustomA_2_ValueList,
                PointsPossible = x.PointsPossible,
                ScaledScoreMax = x.ScaledScoreMax,
                ScaledScoreMin = x.ScaledScoreMin,
                UseArtifact = x.UseArtifact,
                UseNote = x.UseNote,
                CustomA_1_ConversionSetID = x.CustomA_1_ConversionSetID,
                CustomA_2_ConversionSetID = x.CustomA_2_ConversionSetID,
                CustomA_3_ConversionSetID = x.CustomA_3_ConversionSetID,
                CustomA_4_ConversionSetID = x.CustomA_4_ConversionSetID,
                CustomN_1_ConversionSetID = x.CustomN_1_ConversionSetID,
                CustomN_2_ConversionSetID = x.CustomN_2_ConversionSetID,
                CustomN_3_ConversionSetID = x.CustomN_3_ConversionSetID,
                CustomN_4_ConversionSetID = x.CustomN_4_ConversionSetID
            });
        }

        public void Save(VirtualTestCustomSubScore item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestCustomSubScoreID.Equals(item.VirtualTestCustomSubScoreId));

            if (entity == null)
            {
                entity = new VirtualTestCustomSubScoreEntity()
                {
                };
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.VirtualTestCustomSubScoreId = entity.VirtualTestCustomSubScoreID;
        }

        public void Delete(VirtualTestCustomSubScore item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestCustomSubScoreID.Equals(item.VirtualTestCustomSubScoreId));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualTestCustomSubScore model, VirtualTestCustomSubScoreEntity entity)
        {
            entity.VirtualTestCustomSubScoreID = model.VirtualTestCustomSubScoreId;
            entity.VirtualTestCustomScoreID = model.VirtualTestCustomScoreId;
            entity.CustomA_1_Label = model.CustomA1Label;
            entity.CustomA_1_Label = model.CustomA1Label;
            entity.CustomA_2_Label = model.CustomA2Label;
            entity.CustomA_3_Label = model.CustomA3Label;
            entity.CustomA_4_Label = model.CustomA4Label;
            entity.CustomN_1_Label = model.CustomN1Label;
            entity.CustomN_2_Label = model.CustomN2Label;
            entity.CustomN_3_Label = model.CustomN3Label;
            entity.CustomN_4_Label = model.CustomN4Label;
            entity.Name = model.Name;
            entity.UseAchievementLevel = model.UseAchievementLevel;
            entity.UseCustomA_1 = model.UseCustomA1;
            entity.UseCustomA_2 = model.UseCustomA2;
            entity.UseCustomA_3 = model.UseCustomA3;
            entity.UseCustomA_4 = model.UseCustomA4;
            entity.UseCustomN_1 = model.UseCustomN1;
            entity.UseCustomN_2 = model.UseCustomN2;
            entity.UseCustomN_3 = model.UseCustomN3;
            entity.UseCustomN_4 = model.UseCustomN4;
            entity.UsePercent = model.UsePercent;
            entity.UsePercentile = model.UsePercentile;
            entity.UseRaw = model.UseRaw;
            entity.UseScaled = model.UseScaled;
            entity.AchievementLevelSettingID = model.AchievementLevelSettingId;
            entity.CustomA_1_ValueList = model.CustomA1ValueList;
            entity.CustomA_2_ValueList = model.CustomA2ValueList;
            entity.PointsPossible = model.PointsPossible;
            entity.ScaledScoreMax = model.ScaledScoreMax;
            entity.ScaledScoreMin = model.ScaledScoreMin;
            entity.Sequence = model.Sequence;
            entity.ItemTagID = model.ItemTagID;
            entity.UseArtifact = model.UseArtifact;
            entity.UseNote = model.UseNote;
            entity.CustomA_1_ConversionSetID = model.CustomA_1_ConversionSetID;
            entity.CustomA_2_ConversionSetID = model.CustomA_2_ConversionSetID;
            entity.CustomA_3_ConversionSetID = model.CustomA_3_ConversionSetID;
            entity.CustomA_4_ConversionSetID = model.CustomA_4_ConversionSetID;
            entity.CustomN_1_ConversionSetID = model.CustomN_1_ConversionSetID;
            entity.CustomN_2_ConversionSetID = model.CustomN_2_ConversionSetID;
            entity.CustomN_3_ConversionSetID = model.CustomN_3_ConversionSetID;
            entity.CustomN_4_ConversionSetID = model.CustomN_4_ConversionSetID;
        }
    }
}
