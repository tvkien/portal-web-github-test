using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualTestCustomScoreRepository : IRepository<VirtualTestCustomScore>
    {
        private readonly Table<VirtualTestCustomScoreEntity> table;

        public VirtualTestCustomScoreRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<VirtualTestCustomScoreEntity>();
        }

        public IQueryable<VirtualTestCustomScore> Select()
        {
            return table.Select(x => new VirtualTestCustomScore
            {
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
                UsePercent = x.UsePercent ?? false,
                UsePercentile = x.UsePercentile ?? false,
                UseRaw = x.UseRaw ?? false,
                UseScaled = x.UseScaled ?? false,
                VirtualTestCustomScoreId = x.VirtualTestCustomScoreID,
                AchievementLevelSettingId = x.AchievementLevelSettingID,
                BankLabel = x.BankLabel,
                CustomA1ValueList = x.CustomA_1_ValueList,
                CustomA2ValueList = x.CustomA_2_ValueList,
                DistrictId = x.DistrictID,
                Label = x.Label,
                PointsPossible = x.PointsPossible,
                ScaledScoreMax = x.ScaledScoreMax,
                ScaledScoreMin = x.ScaledScoreMin,
                TestScoreMethodId = x.TestScoreMethodID,
                VirtualTestSourceId = x.VirtualTestSourceID,
                VirtualTestSubTypeId = x.VirtualTestSubTypeID,
                VirtualTestTypeId = x.VirtualTestTypeID,
                AuthorUserID = x.AuthorUserID,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                UseArtifact = x.UseArtifact,
                UseNote = x.UseNote,
                IsMultiDate = x.IsMultiDate,
                DataSetOriginID = x.DataSetOriginID,
                Archived = x.Archived,
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

        public void Save(VirtualTestCustomScore item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestCustomScoreID.Equals(item.VirtualTestCustomScoreId));

            if (entity == null)
            {
                entity = new VirtualTestCustomScoreEntity()
                {
                    CreatedDate = DateTime.UtcNow,
                };
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            entity.UpdatedDate = DateTime.UtcNow;
            table.Context.SubmitChanges();
            item.VirtualTestCustomScoreId = entity.VirtualTestCustomScoreID;
        }

        public void Delete(VirtualTestCustomScore item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestCustomScoreID.Equals(item.VirtualTestCustomScoreId));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualTestCustomScore model, VirtualTestCustomScoreEntity entity)
        {
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
            entity.VirtualTestCustomScoreID = model.VirtualTestCustomScoreId;
            entity.AchievementLevelSettingID = model.AchievementLevelSettingId;
            entity.BankLabel = model.BankLabel;
            entity.CustomA_1_ValueList = model.CustomA1ValueList;
            entity.CustomA_2_ValueList = model.CustomA2ValueList;
            entity.DistrictID = model.DistrictId;
            entity.Label = model.Label;
            entity.PointsPossible = model.PointsPossible;
            entity.ScaledScoreMax = model.ScaledScoreMax;
            entity.ScaledScoreMin = model.ScaledScoreMin;
            entity.TestScoreMethodID = model.TestScoreMethodId;
            entity.VirtualTestSourceID = model.VirtualTestSourceId;
            entity.VirtualTestSubTypeID = model.VirtualTestSubTypeId;
            entity.VirtualTestTypeID = model.VirtualTestTypeId;
            entity.AuthorUserID = model.AuthorUserID;
            entity.UseArtifact = model.UseArtifact;
            entity.UseNote = model.UseNote;
            entity.IsMultiDate = model.IsMultiDate;
            entity.DataSetOriginID = model.DataSetOriginID;
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
