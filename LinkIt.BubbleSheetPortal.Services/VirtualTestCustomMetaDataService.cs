using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Autofac.Features.Metadata;
using LinkIt.BubbleService.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.Old.DataLocker;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualTestCustomMetaDataService
    {
        private readonly IVirtualTestCustomMetaDataRepository _virtualTestCustomMetaDataRepository;

        public VirtualTestCustomMetaDataService(IVirtualTestCustomMetaDataRepository virtualTestCustomMetaDataRepository)
        {
            _virtualTestCustomMetaDataRepository = virtualTestCustomMetaDataRepository;
        }

        public IQueryable<VirtualTestCustomMetaData> Select()
        {
            return _virtualTestCustomMetaDataRepository.Select();
        }

        public void Save(VirtualTestCustomMetaData meta)
        {
            _virtualTestCustomMetaDataRepository.Save(meta);
        }
        public void Delete(VirtualTestCustomMetaData meta)
        {
            _virtualTestCustomMetaDataRepository.Delete(meta);
        }
        public void Delete(int virtualTestCustomMetaDataID)
        {
            var item = GetById(virtualTestCustomMetaDataID);
            if (item != null)
            {
                _virtualTestCustomMetaDataRepository.Delete(item);
            }
            
        }
        public VirtualTestCustomMetaData GetById(int virtualTestCustomMetaDataID)
        {
            return _virtualTestCustomMetaDataRepository.Select()
                .FirstOrDefault(
                    x => x.VirtualTestCustomMetaDataID == virtualTestCustomMetaDataID);
        }

        public List<VirtualTestCustomMetaData> GetAllMetaDataOfTemplate(int virtualTestCustomScoreId)
        {
            var metaData = _virtualTestCustomMetaDataRepository.Select()
                .Where(
                    x => x.VirtualTestCustomScoreID == virtualTestCustomScoreId)
                .Select(x => new VirtualTestCustomMetaData()
                {
                    VirtualTestCustomMetaDataID = x.VirtualTestCustomMetaDataID,
                    VirtualTestCustomScoreID = x.VirtualTestCustomScoreID,
                    VirtualTestCustomSubScoreID = x.VirtualTestCustomSubScoreID,
                    ScoreType = x.ScoreType,
                    MetaData = x.MetaData,
                    Order = x.Order
                }).ToList();
            return metaData;
        }
        public List<VirtualTestCustomMetaData> GetMetaDataOfTemplate(int virtualTestCustomScoreId)
        {
            var metaData = _virtualTestCustomMetaDataRepository.Select()
                .Where(
                    x => x.VirtualTestCustomScoreID == virtualTestCustomScoreId && x.VirtualTestCustomSubScoreID == null)
                .Select(x => new VirtualTestCustomMetaData()
                {
                    VirtualTestCustomMetaDataID = x.VirtualTestCustomMetaDataID,
                    VirtualTestCustomScoreID = x.VirtualTestCustomScoreID,
                    ScoreType = x.ScoreType,
                    MetaData = x.MetaData,
                    Order = x.Order
                }).ToList();
            return metaData;
        }

        public List<VirtualTestCustomMetaData> GetMetaDataOfSubscore(int virtualTestCustomSubScoreId)
        {
            var metaData = _virtualTestCustomMetaDataRepository.Select()
                .Where(
                    x => x.VirtualTestCustomSubScoreID == virtualTestCustomSubScoreId)
                .Select(x => new VirtualTestCustomMetaData()
                {
                    VirtualTestCustomMetaDataID = x.VirtualTestCustomMetaDataID,
                    VirtualTestCustomScoreID = x.VirtualTestCustomScoreID,
                    VirtualTestCustomSubScoreID = x.VirtualTestCustomSubScoreID,
                    ScoreType = x.ScoreType,
                    MetaData = x.MetaData,
                    Order = x.Order
                }).ToList();
            return metaData;
        }

        public VirtualTestCustomMetaData GetMetaDataOfTemplateScoreType(ScoreTypeModel scoreType)
        {
            var metaData = _virtualTestCustomMetaDataRepository.Select()
                .Where(
                    x => x.VirtualTestCustomScoreID == scoreType.TemplateID && x.VirtualTestCustomSubScoreID == null)
                .Select(x => new VirtualTestCustomMetaData()
                {
                    VirtualTestCustomMetaDataID = x.VirtualTestCustomMetaDataID,
                    VirtualTestCustomScoreID = x.VirtualTestCustomScoreID,
                    ScoreType = x.ScoreType,
                    MetaData = x.MetaData,
                    Order = x.Order
                }).ToList();
            if (scoreType.IsCustomScoreType)
            {
                if (scoreType.UseCustomN1)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomN_1);
                }
                if (scoreType.UseCustomN2)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomN_2);
                }
                if (scoreType.UseCustomN3)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomN_3);
                }
                if (scoreType.UseCustomN4)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomN_4);
                }
                if (scoreType.UseCustomA1)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomA_1);
                }
                if (scoreType.UseCustomA2)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomA_2);
                }
                if (scoreType.UseCustomA3)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomA_3);
                }
                if (scoreType.UseCustomA4)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomA_4);
                }
            }
            else
            {
                if (scoreType.ScoreTypeCode == ScoreTypeModel.RAW_SCORE)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.Raw);
                }
                if (scoreType.ScoreTypeCode == ScoreTypeModel.SCALED_SCORE)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.Scaled);
                }
                if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENT_SCORE)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.Percent);
                }
                if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENTILE_SCORE)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.Percentile);
                }
                if (scoreType.ScoreTypeCode == ScoreTypeModel.ARTIFACT_SCORE)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.Artifact);
                }
            }

            return null;
        }
        public VirtualTestCustomMetaData GetMetaDataOfSubscoreScoreType(ScoreTypeModel scoreType)
        {
            var metaData = _virtualTestCustomMetaDataRepository.Select()
                .Where(
                    x => x.VirtualTestCustomScoreID == scoreType.TemplateID
                    && x.VirtualTestCustomSubScoreID == scoreType.SubscoreId)
                .Select(x => new VirtualTestCustomMetaData()
                {
                    VirtualTestCustomMetaDataID = x.VirtualTestCustomMetaDataID,
                    VirtualTestCustomScoreID = x.VirtualTestCustomScoreID,
                    VirtualTestCustomSubScoreID = x.VirtualTestCustomSubScoreID,
                    ScoreType = x.ScoreType,
                    MetaData = x.MetaData,
                    Order = x.Order
                }).ToList();
            if (scoreType.IsCustomScoreType)
            {
                if (scoreType.UseCustomN1)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomN_1);
                }
                if (scoreType.UseCustomN2)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomN_2);
                }
                if (scoreType.UseCustomN3)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomN_3);
                }
                if (scoreType.UseCustomN4)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomN_4);
                }
                if (scoreType.UseCustomA1)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomA_1);
                }
                if (scoreType.UseCustomA2)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomA_2);
                }
                if (scoreType.UseCustomA3)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomA_3);
                }
                if (scoreType.UseCustomA4)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.CustomA_4);
                }
            }
            else
            {
                if (scoreType.ScoreTypeCode == ScoreTypeModel.RAW_SCORE)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.Raw);
                }
                if (scoreType.ScoreTypeCode == ScoreTypeModel.SCALED_SCORE)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.Scaled);
                }
                if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENT_SCORE)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.Percent);
                }
                if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENTILE_SCORE)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.Percentile);
                }
                if (scoreType.ScoreTypeCode == ScoreTypeModel.ARTIFACT_SCORE)
                {
                    return metaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.Artifact);
                }
            }

            return null;
        }

        public List<ScopeTypeUpdate> RearrangeScoreTypeofCustomScoreType(int templateId,bool isNumericCustom, int? subscoreId)
        {
            var outScoreType = new List<ScopeTypeUpdate>();
            List<VirtualTestCustomMetaData> metaDataList = null;
            if (subscoreId.HasValue)
            {
                //Update meta data of a subscore
                metaDataList = GetMetaDataOfSubscore(subscoreId??0);
            }
            else
            {
                //Update meta data of a template
                metaDataList = GetMetaDataOfTemplate(templateId);
            }

            if (metaDataList != null)
            {
                if (isNumericCustom)
                {
                    //ScoreType:CustomN_1->CustomN_2->CustomN_3->CustomN_4
                    var metaDataOfCustomScore= metaDataList.Where(x => x.ScoreType.StartsWith("CustomN")).OrderBy(x => x.ScoreType).ToList();
                    for (int i = 0; i < metaDataOfCustomScore.Count; i++)
                    {
                        var oldValue = metaDataOfCustomScore[i].ScoreType;
                        metaDataOfCustomScore[i].ScoreType = string.Format("CustomN_{0}", (i + 1).ToString());
                        var replaceScore = string.Empty;
                        if (metaDataOfCustomScore[i].VirtualTestCustomSubScoreID.HasValue && metaDataOfCustomScore[i].VirtualTestCustomSubScoreID.Value > 0)
                        {
                            oldValue = $"{metaDataOfCustomScore[i].VirtualTestCustomSubScoreID.Value}&{oldValue}";
                            replaceScore = $"{metaDataOfCustomScore[i].VirtualTestCustomSubScoreID.Value}&{metaDataOfCustomScore[i].ScoreType}";
                        }
                        else
                        {
                            oldValue = $"0&{oldValue}";
                            replaceScore = $"0&{metaDataOfCustomScore[i].ScoreType}";
                        }
                        Save(metaDataOfCustomScore[i]);
                        outScoreType.Add(new ScopeTypeUpdate()
                        {
                            ScoreId = templateId,
                            SubScoreId = subscoreId.HasValue && subscoreId.Value > 0 ? subscoreId : 0,
                            ScoreTypeOld = oldValue,
                            ScoreTypeNew = replaceScore
                        });
                    }
                }
                else
                {
                    //ScoreType:CustomA_1->CustomA_2->CustomA_3->CustomA_4
                    var metaDataOfCustomScore = metaDataList.Where(x => x.ScoreType.StartsWith("CustomA")).OrderBy(x => x.ScoreType).ToList();
                    for (int i = 0; i < metaDataOfCustomScore.Count; i++)
                    {
                        var oldValue = metaDataOfCustomScore[i].ScoreType;
                        metaDataOfCustomScore[i].ScoreType = string.Format("CustomA_{0}", (i + 1).ToString());
                        var replaceScore = string.Empty;
                        if (metaDataOfCustomScore[i].VirtualTestCustomSubScoreID.HasValue && metaDataOfCustomScore[i].VirtualTestCustomSubScoreID.Value > 0)
                        {
                            oldValue = $"{metaDataOfCustomScore[i].VirtualTestCustomSubScoreID.Value}&{oldValue}";
                            replaceScore = $"{metaDataOfCustomScore[i].VirtualTestCustomSubScoreID.Value}&{metaDataOfCustomScore[i].ScoreType}";
                        }
                        else
                        {
                            oldValue = $"0&{oldValue}";
                            replaceScore = $"0&{metaDataOfCustomScore[i].ScoreType}";
                        }
                        Save(metaDataOfCustomScore[i]);
                        outScoreType.Add(new ScopeTypeUpdate()
                        {
                            ScoreId = templateId,
                            SubScoreId = subscoreId.HasValue && subscoreId.Value > 0 ? subscoreId : 0,
                            ScoreTypeOld = oldValue,
                            ScoreTypeNew = replaceScore
                        });
                    }
                }
            }
            return outScoreType;
        }

        public void RearrangeScoreTypeInExpression(List<VirtualTestCustomMetaData> metaDataList, List<ScopeTypeUpdate> metaUpdates)
        {   
            //ScoreType:CustomN_1->CustomN_2->CustomN_3->CustomN_4
            var metaDataOfCustomScore = metaDataList.Where(x => x.ScoreType.Contains(ScoreTypeColumnConstants.Raw)
                || x.ScoreType.Contains(ScoreTypeColumnConstants.CustomN)).OrderBy(x => x.ScoreType).ToList();
            foreach (var metaUpdate in metaDataOfCustomScore)
            {
                if (!string.IsNullOrEmpty(metaUpdate.MetaData))
                {
                    var metaData = JsonConvert.DeserializeObject<VirtualTestCustomMetaModel>(metaUpdate.MetaData);
                    if (!string.IsNullOrEmpty(metaData.Expression))
                    {
                        foreach (var update in metaUpdates)
                        {
                            metaData.Expression = metaData.Expression.Replace(update.ScoreTypeOld, update.ScoreTypeNew);
                        }
                        metaUpdate.MetaData = JsonConvert.SerializeObject(metaData);
                        Save(metaUpdate);
                    }
                }
            }           
        }

        public bool HasIsAutoCalculationScoreType(int? virtualTestCustomScoreId, int? virtualTestCustomSubScoreId)
        {
            bool result = false;
            var metaDataSubScoreDelete = _virtualTestCustomMetaDataRepository.Select()
                .Where(x => x.VirtualTestCustomSubScoreID == virtualTestCustomSubScoreId)
                .Select(x => x.ScoreType);
            if (metaDataSubScoreDelete != null && metaDataSubScoreDelete.Any())
            {
                VirtualTestCustomMetaData virtualTestCustomMetaData = new VirtualTestCustomMetaData();
                //Check overall score and sub score using score type delete
                var metaDataScore = _virtualTestCustomMetaDataRepository.Select()
                .Where(x => x.VirtualTestCustomScoreID == virtualTestCustomScoreId
                    && (x.VirtualTestCustomSubScoreID != virtualTestCustomSubScoreId || x.VirtualTestCustomSubScoreID == null))
                .Select(x => x.MetaData).ToList();
                if (metaDataScore != null)
                {
                    foreach (var metaDataScoreItem in metaDataScore)
                    {
                        var metaData = virtualTestCustomMetaData.ParseMetaToObject(metaDataScoreItem);
                        if (metaData != null && metaData.Expression != null)
                        {
                            var exits = metaDataSubScoreDelete.ToList().Where(w => metaData.Expression.Contains($"{virtualTestCustomSubScoreId}&{w}"));
                            if (metaData.IsAutoCalculation && exits != null && exits.Any())
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }
        public void UpdateOrderForMetaData(int? virtualTestCustomScoreId, int? virtualTestCustomSubScoreId, string scoreTypeOrderXml)
        {
            _virtualTestCustomMetaDataRepository.UpdateOrderForMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, scoreTypeOrderXml);
        }
    }
}
