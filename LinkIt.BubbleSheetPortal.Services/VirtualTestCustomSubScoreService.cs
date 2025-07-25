using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestUtilitiesDefineTemplates;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualTestCustomSubScoreService
    {
        private readonly IRepository<VirtualTestCustomSubScore> _virtualTestCustomSubScoreRepository;
        private readonly IVirtualTestCustomMetaDataRepository _virtualTestCustomMetaDataRepository;
        private readonly IConversionSetService _conversionSetService;

        public VirtualTestCustomSubScoreService(IRepository<VirtualTestCustomSubScore> virtualTestCustomSubScoreRepository
            , IVirtualTestCustomMetaDataRepository virtualTestCustomMetaDataRepository
            , IConversionSetService conversionSetService)
        {
            _conversionSetService = conversionSetService;
            _virtualTestCustomSubScoreRepository = virtualTestCustomSubScoreRepository;
            _virtualTestCustomMetaDataRepository = virtualTestCustomMetaDataRepository;
        }

        public IQueryable<VirtualTestCustomSubScore> Select()
        {
            return _virtualTestCustomSubScoreRepository.Select();
        }

        public VirtualTestCustomSubScore Create(User obj, VirtualTestCustomSubScore model)
        {
            _virtualTestCustomSubScoreRepository.Save(model);
            return model;
        }

        public VirtualTestCustomSubScore GetById(int virtualTestCustomSubScoreId)
        {
            var data =
                _virtualTestCustomSubScoreRepository.Select()
                    .FirstOrDefault(x => x.VirtualTestCustomSubScoreId == virtualTestCustomSubScoreId);
            return data;
        }

        public bool CheckExistSubScoreName(int virtualTestCustomScoreID, int virtualTestCustomSubScoreID, string name)
        {
            var result = _virtualTestCustomSubScoreRepository.Select().Any(x => x.VirtualTestCustomScoreId == virtualTestCustomScoreID
                                                                            && x.VirtualTestCustomSubScoreId != virtualTestCustomSubScoreID
                                                                            && x.Name == name);
            return result;
        }
        public VirtualTestCustomSubScore GetByName(int virtualTestCustomScoreID, string name)
        {
            var data =
                _virtualTestCustomSubScoreRepository.Select()
                    .FirstOrDefault(x => x.VirtualTestCustomScoreId == virtualTestCustomScoreID && x.Name.Equals(name));
            return data;
        }
        public List<VirtualTestCustomSubScore> GetSubscoreOfTemplate(int virtualTestCustomScoreId)
        {
            var data =
                _virtualTestCustomSubScoreRepository.Select()
                    .Where(x => x.VirtualTestCustomScoreId == virtualTestCustomScoreId)
                    .ToList();
            return data;
        }

        public int GetMaxSequenceOfSubscores(int virtualTestCustomScoreID)
        {
            if (
                !_virtualTestCustomSubScoreRepository.Select()
                    .Any(x => x.VirtualTestCustomScoreId == virtualTestCustomScoreID))
            {
                return 0;
            }
            else
            {
                var maxSequence =
              _virtualTestCustomSubScoreRepository.Select()
                  .Where(x => x.VirtualTestCustomScoreId == virtualTestCustomScoreID).Max(x=>x.Sequence);
                return maxSequence ?? 0;
            }
        }

        public void Save(VirtualTestCustomSubScore item)
        {
            _virtualTestCustomSubScoreRepository.Save(item);
        }
        public void Delete(VirtualTestCustomSubScore item)
        {
            _virtualTestCustomSubScoreRepository.Delete(item);
        }

        public void SaveItemTag(int virtualTestCustomSubScoreId, int itemTagId)
        {
            var subScore = _virtualTestCustomSubScoreRepository.Select()
                .FirstOrDefault(x => x.VirtualTestCustomSubScoreId == virtualTestCustomSubScoreId);

            if (subScore != null)
            {
                subScore.ItemTagID = itemTagId;
                _virtualTestCustomSubScoreRepository.Save(subScore);
            }
        }

        public TemplateConversionSetDto ImportConversionTable(int id, string fileName, List<string> headers, IEnumerable<ConversionTableExcelItem> excelItems, bool isReplace)
        {
            var item = _virtualTestCustomSubScoreRepository.Select().FirstOrDefault(x => x.VirtualTestCustomSubScoreId == id);
            if (isReplace)
            {
                item.UseRaw = false;
                item.UseCustomA1 = item.UseCustomA2 = item.UseCustomA3 = item.UseCustomA4
                    = item.UseCustomN1 = item.UseCustomN2 = item.UseCustomN3 = item.UseCustomN4 = null;

                _virtualTestCustomMetaDataRepository.DeleteMetaData(item.VirtualTestCustomScoreId, id);
            }

            AutoGenerateTemplateColumns(item, headers, excelItems);
            return UpdateConversionSet(item, fileName, excelItems);
        }

        public bool HasConversionSet(int id)
        {
            return _virtualTestCustomSubScoreRepository.Select()
                .Any(x => x.VirtualTestCustomSubScoreId == id && (x.CustomN_1_ConversionSetID > 0 || x.CustomA_1_ConversionSetID > 0));
        }

        #region Private

        private void AutoGenerateTemplateColumns(VirtualTestCustomSubScore item, List<string> headers, IEnumerable<ConversionTableExcelItem> excelItems)
        {
            var virtualTestCustomScoreId = item.VirtualTestCustomScoreId;
            var virtualTestCustomSubScoreId = item.VirtualTestCustomSubScoreId;

            if (!item.UseRaw)
            {
                item.UseRaw = true;
                SaveNumericDefaultMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, VirtualTestCustomMetaData.Raw);
            }

            var columnName = headers[1];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomN1 == true))
            {
                item.UseCustomN1 = true;
                item.CustomN1Label = columnName;

                var decimalScale = CommonUtils.GetMaxDecimalPlace(excelItems.Select(x => x.CustomNumeric1));
                SaveNumericDefaultMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, VirtualTestCustomMetaData.CustomN_1, decimalScale);
            }

            columnName = headers[2];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomA1 == true))
            {
                item.UseCustomA1 = true;
                item.CustomA1Label = columnName;
                SaveTextDefaultMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, VirtualTestCustomMetaData.CustomA_1);
            }

            columnName = headers[3];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomN2 == true))
            {
                item.UseCustomN2 = true;
                item.CustomN2Label = columnName;
                var decimalScale = CommonUtils.GetMaxDecimalPlace(excelItems.Select(x => x.CustomNumeric2));
                SaveNumericDefaultMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, VirtualTestCustomMetaData.CustomN_2, decimalScale);
            }

            columnName = headers[4];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomA2 == true))
            {
                item.UseCustomA2 = true;
                item.CustomA2Label = columnName;
                SaveTextDefaultMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, VirtualTestCustomMetaData.CustomA_2);
            }

            columnName = headers[5];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomN3 == true))
            {
                item.UseCustomN3 = true;
                item.CustomN3Label = columnName;

                var decimalScale = CommonUtils.GetMaxDecimalPlace(excelItems.Select(x => x.CustomNumeric1));
                SaveNumericDefaultMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, VirtualTestCustomMetaData.CustomN_3, decimalScale);
            }

            columnName = headers[6];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomA3 == true))
            {
                item.UseCustomA3 = true;
                item.CustomA3Label = columnName;
                SaveTextDefaultMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, VirtualTestCustomMetaData.CustomA_3);
            }

            columnName = headers[7];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomN4 == true))
            {
                item.UseCustomN4 = true;
                item.CustomN4Label = columnName;

                var decimalScale = CommonUtils.GetMaxDecimalPlace(excelItems.Select(x => x.CustomNumeric1));
                SaveNumericDefaultMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, VirtualTestCustomMetaData.CustomN_4, decimalScale);
            }

            columnName = headers[8];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomA4 == true))
            {
                item.UseCustomA4 = true;
                item.CustomA4Label = columnName;
                SaveTextDefaultMetaData(virtualTestCustomScoreId, virtualTestCustomSubScoreId, VirtualTestCustomMetaData.CustomA_4);
            }

            _virtualTestCustomSubScoreRepository.Save(item);
        }

        private TemplateConversionSetDto UpdateConversionSet(VirtualTestCustomSubScore item, string fileName, IEnumerable<ConversionTableExcelItem> excelItems)
        {
            var template = new TemplateConversionSetDto();

            var conversionSetDetails = excelItems.Select(x => new ConversionSetDetailDto
            {
                Input1 = int.Parse(x.RawScore),
                ConvertedScore = decimal.TryParse(x.CustomNumeric1, out var score) ? score : 0,
                ConvertedScore_A = x.CustomText1
            }).ToList();
            var conversionSetId = _conversionSetService.SaveConversionSet(item.CustomA_1_ConversionSetID, fileName, conversionSetDetails);
            item.CustomA_1_ConversionSetID = item.CustomN_1_ConversionSetID = conversionSetId;
            template.Custom1_ConversionSetId = conversionSetId;

            conversionSetDetails = excelItems.Select(x => new ConversionSetDetailDto
            {
                Input1 = int.Parse(x.RawScore),
                ConvertedScore = decimal.TryParse(x.CustomNumeric2, out var score) ? score : 0,
                ConvertedScore_A = x.CustomText2
            }).ToList();
            conversionSetId = _conversionSetService.SaveConversionSet(item.CustomA_2_ConversionSetID, fileName, conversionSetDetails);
            item.CustomA_2_ConversionSetID = item.CustomN_2_ConversionSetID = conversionSetId;
            template.Custom2_ConversionSetId = conversionSetId;

            conversionSetDetails = excelItems.Select(x => new ConversionSetDetailDto
            {
                Input1 = int.Parse(x.RawScore),
                ConvertedScore = decimal.TryParse(x.CustomNumeric3, out var score) ? score : 0,
                ConvertedScore_A = x.CustomText3
            }).ToList();
            conversionSetId = _conversionSetService.SaveConversionSet(item.CustomA_3_ConversionSetID, fileName, conversionSetDetails);
            item.CustomA_3_ConversionSetID = item.CustomN_3_ConversionSetID = conversionSetId;
            template.Custom3_ConversionSetId = conversionSetId;

            conversionSetDetails = excelItems.Select(x => new ConversionSetDetailDto
            {
                Input1 = int.Parse(x.RawScore),
                ConvertedScore = decimal.TryParse(x.CustomNumeric4, out var score) ? score : 0,
                ConvertedScore_A = x.CustomText4
            }).ToList();
            conversionSetId = _conversionSetService.SaveConversionSet(item.CustomA_4_ConversionSetID, fileName, conversionSetDetails);
            item.CustomA_4_ConversionSetID = item.CustomN_4_ConversionSetID = conversionSetId;
            template.Custom4_ConversionSetId = conversionSetId;

            _virtualTestCustomSubScoreRepository.Save(item);

            return template;
        }

        private void SaveNumericDefaultMetaData(int virtualTestCustomScoreId, int? virtualTestCustomSubScoreId, string scoreType, int decimalScale = 0)
        {
            if (decimalScale < 0 || decimalScale > 3) decimalScale = 3;

            var meta = new VirtualTestCustomMetaModel
            {
                DataType = VirtualTestCustomMetaModel.DataTypeNumeric,
                MaxValue = 0,
                MinValue = 0,
                DecimalScale = decimalScale,
                DataHostPot = "manuall",
                DerivedName = "radioRawManualEntry",
                FormatOption = "FreeText",
                DisplayOption = "label"
            };

            var metaData = new VirtualTestCustomMetaData()
            {
                VirtualTestCustomScoreID = virtualTestCustomScoreId,
                VirtualTestCustomSubScoreID = virtualTestCustomSubScoreId,
                ScoreType = scoreType,
                MetaData = meta.GetJsonString()
            };
            _virtualTestCustomMetaDataRepository.Save(metaData);
        }

        private void SaveTextDefaultMetaData(int virtualTestCustomScoreId, int? virtualTestCustomSubScoreId, string scoreType)
        {
            var meta = new VirtualTestCustomMetaModel
            {
                DataType = VirtualTestCustomMetaModel.DataTypeFreeForm,
                MaxLength = 100
            };

            var metaData = new VirtualTestCustomMetaData()
            {
                VirtualTestCustomScoreID = virtualTestCustomScoreId,
                VirtualTestCustomSubScoreID = virtualTestCustomSubScoreId,
                ScoreType = scoreType,
                MetaData = meta.GetJsonString()
            };
            _virtualTestCustomMetaDataRepository.Save(metaData);
        }

        #endregion
    }
}
