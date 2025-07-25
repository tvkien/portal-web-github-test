using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Xml.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.Old.DataLocker;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualTestCustomMetaDataRepository : IVirtualTestCustomMetaDataRepository
    {
        private readonly Table<VirtualTestCustomMetaDataEntity> table;
        private readonly DataLockerContextDataContext dbContext;
        private readonly IConnectionString _connectionString;

        public VirtualTestCustomMetaDataRepository(IConnectionString conn)
        {
            _connectionString = conn;
            var connectionString = conn.GetLinkItConnectionString();
            dbContext = DataLockerContextDataContext.Get(connectionString);
            table = dbContext.GetTable<VirtualTestCustomMetaDataEntity>();
        }

        public IQueryable<VirtualTestCustomMetaData> Select()
        {
            return table.Select(x => new VirtualTestCustomMetaData
            {
                VirtualTestCustomMetaDataID = x.VirtualTestCustomMetaDataID,
                VirtualTestCustomScoreID = x.VirtualTestCustomScoreID,
                VirtualTestCustomSubScoreID = x.VirtualTestCustomSubScoreID,
                ScoreType = x.ScoreType,
                MetaData = x.MetaData,
                Order = x.Order
            });
        }
        public DataContext GetDataContext()
        {
            return DataLockerContextDataContext.Get(_connectionString.GetLinkItConnectionString());
        }
        public void Save(VirtualTestCustomMetaData item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestCustomMetaDataID.Equals(item.VirtualTestCustomMetaDataID));

            if (entity == null)
            {
                entity = new VirtualTestCustomMetaDataEntity();
                table.InsertOnSubmit(entity);
                if (item.VirtualTestCustomSubScoreID > 0)
                {
                    item.Order = TotalMetaDataByVirtualTestCustomSubScoreID(item.VirtualTestCustomSubScoreID.GetValueOrDefault());
                }
                else if (item.VirtualTestCustomScoreID > 0)
                {
                    item.Order = TotalMetaDataByVirtualTestCustomScoreID(item.VirtualTestCustomScoreID);
                }
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.VirtualTestCustomMetaDataID = entity.VirtualTestCustomMetaDataID;
        }

        public void Delete(VirtualTestCustomMetaData item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestCustomMetaDataID.Equals(item.VirtualTestCustomMetaDataID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualTestCustomMetaData model, VirtualTestCustomMetaDataEntity entity)
        {
            entity.VirtualTestCustomMetaDataID = model.VirtualTestCustomMetaDataID;
            entity.VirtualTestCustomScoreID = model.VirtualTestCustomScoreID;
            entity.VirtualTestCustomSubScoreID = model.VirtualTestCustomSubScoreID;
            entity.ScoreType = model.ScoreType;
            entity.MetaData = model.MetaData;
            entity.Order = model.Order;
        }

        public void UpdateOrderForMetaData(int? virtualTestCustomScoreId, int? virtualTestCustomSubScoreId, string scoreTypeOrderXml)
        {
            dbContext.DTLUpdateOrderForMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, XElement.Parse(scoreTypeOrderXml));
        }

        private int TotalMetaDataByVirtualTestCustomScoreID(int id)
        {
            var metaData = dbContext.VirtualTestCustomMetaDataEntities.Where(o => o.VirtualTestCustomScoreID == id && o.VirtualTestCustomSubScoreID.HasValue == false);
            var lastedOrder = metaData.OrderByDescending(o => o.Order).FirstOrDefault()?.Order ?? 0;
            var note = metaData.Where(f => f.ScoreType == ScoreTypeModel.NOTE_COMMENT).Select(x => new VirtualTestCustomMetaData
            {
                VirtualTestCustomMetaDataID = x.VirtualTestCustomMetaDataID,
                VirtualTestCustomScoreID = x.VirtualTestCustomScoreID,
                VirtualTestCustomSubScoreID = x.VirtualTestCustomSubScoreID,
                ScoreType = x.ScoreType,
                MetaData = x.MetaData,
                Order = x.Order
            }).FirstOrDefault();
            if (note != null)
            {
                var noteMeta = VirtualTestCustomMetaDataExtension.ParseMetaToObject(note);
                if (noteMeta != null && noteMeta.ListNoteComment != null && noteMeta.ListNoteComment.Any())
                {
                    var lastedNoteOrder = noteMeta.ListNoteComment.OrderByDescending(o => o.Order).FirstOrDefault().Order;
                    if (lastedNoteOrder != null && lastedOrder < lastedNoteOrder.Value)
                    {
                        lastedOrder = lastedNoteOrder.Value;
                    }
                }
            }
            return lastedOrder;
        }

        private int TotalMetaDataByVirtualTestCustomSubScoreID(int id)
        {
            var metaData = dbContext.VirtualTestCustomMetaDataEntities.Where(o => o.VirtualTestCustomSubScoreID == id);
            var lastedOrder = metaData.OrderByDescending(o => o.Order).FirstOrDefault()?.Order ?? 0;
            var note = metaData.Where(f => f.ScoreType == ScoreTypeModel.NOTE_COMMENT).Select(x => new VirtualTestCustomMetaData
            {
                VirtualTestCustomMetaDataID = x.VirtualTestCustomMetaDataID,
                VirtualTestCustomScoreID = x.VirtualTestCustomScoreID,
                VirtualTestCustomSubScoreID = x.VirtualTestCustomSubScoreID,
                ScoreType = x.ScoreType,
                MetaData = x.MetaData,
                Order = x.Order
            }).FirstOrDefault();
            if (note != null)
            {
                var noteMeta = VirtualTestCustomMetaDataExtension.ParseMetaToObject(note);
                if (noteMeta != null && noteMeta.ListNoteComment != null && noteMeta.ListNoteComment.Any())
                {
                    var lastedNoteOrder = noteMeta.ListNoteComment.OrderByDescending(o => o.Order).FirstOrDefault().Order;
                    if (lastedNoteOrder != null && lastedOrder < lastedNoteOrder.Value)
                    {
                        lastedOrder = lastedNoteOrder.Value;
                    }
                }
            }
            return lastedOrder;
        }

        public List<CustomMetaSettingMinMaxDto> GetCustomMetaSettings(int templateId)
        {
            var parameters = new List<(string, string, SqlDbType, object, ParameterDirection)>();
            parameters.Add(("templateIds", "", SqlDbType.NVarChar, templateId.ToString(), ParameterDirection.Input));
            var res = GetDataContext().Query<CustomMetaSettingMinMaxDto>(new SqlParameterRequest()
            {
                StoredName = "DL_GetAndCalculationMinMaxValue",
                Parameters = parameters
            }, out _);
            return res;
        }

        public void DeleteMetaData(int templateId, int? subTemplateId = null)
        {
            var items = table
                .Where(x => x.VirtualTestCustomScoreID == templateId && (x.ScoreType == VirtualTestCustomMetaData.Raw || x.ScoreType.StartsWith("Custom")))
                .ToList();

            foreach (var item in items.Where(x => x.VirtualTestCustomSubScoreID == subTemplateId))
            {
                table.DeleteOnSubmit(item);
            }

            table.Context.SubmitChanges();
        }
    }
}
