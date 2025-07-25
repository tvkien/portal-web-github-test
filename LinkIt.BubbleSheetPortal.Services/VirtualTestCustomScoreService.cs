using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestUtilitiesDefineTemplates;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualTestCustomScoreService
    {
        private readonly IRepository<VirtualTestCustomScore> virtualTestCustomScoreRepository;
        private readonly IVirtualTestCustomMetaDataRepository _virtualTestCustomMetaDataRepository;
        private readonly IConversionSetService _conversionSetService;

        public VirtualTestCustomScoreService(IRepository<VirtualTestCustomScore> virtualTestCustomScoreRepository
            , IVirtualTestCustomMetaDataRepository virtualTestCustomMetaDataRepository
            , IConversionSetService conversionSetService)
        {
            _conversionSetService = conversionSetService;
            this.virtualTestCustomScoreRepository = virtualTestCustomScoreRepository;
            _virtualTestCustomMetaDataRepository = virtualTestCustomMetaDataRepository;
        }

        public IQueryable<VirtualTestCustomScore> Select()
        {
            return virtualTestCustomScoreRepository.Select();
        }

        public VirtualTestCustomScore CreateCustomScore(string name, bool? isMultiDate, User obj)
        {
            var objCustomScore = new VirtualTestCustomScore()
            {
                Name = name,
                AuthorUserID = obj.Id,
                DistrictId = obj.DistrictId.GetValueOrDefault(),
                VirtualTestSubTypeId = 1, //TODO:
                TestScoreMethodId = 1,//TODO:
                VirtualTestTypeId = 5, //TODO: Results Entry/Data Locker on VirtualTestType table
                VirtualTestSourceId = 3, //TODO: legacy test,
                UseAchievementLevel = true,
                AchievementLevelSettingId = 125,
                IsMultiDate = isMultiDate,
                DataSetOriginID = (int)DataSetOriginEnum.Data_Locker
            };
            virtualTestCustomScoreRepository.Save(objCustomScore);
            return objCustomScore;
        }
        public VirtualTestCustomScore CreateCustomScoreForSurvey(string name, User obj)
        {
            var objCustomScore = new VirtualTestCustomScore()
            {
                Name = name,
                AuthorUserID = obj.Id,
                DistrictId = obj.DistrictId.GetValueOrDefault(),
                VirtualTestSubTypeId = 1,
                TestScoreMethodId = (int)TestScoreMethodEnum.Survey,
                VirtualTestTypeId = 3,
                VirtualTestSourceId = 1,
                UseAchievementLevel = true,
                AchievementLevelSettingId = 0,
                IsMultiDate = true,
                DataSetOriginID = (int)DataSetOriginEnum.Survey
            };
            virtualTestCustomScoreRepository.Save(objCustomScore);
            return objCustomScore;
        }

        public VirtualTestCustomScore GetCustomScoreByNameAndDistrictID(int districtId, string name)
        {
            return
                virtualTestCustomScoreRepository.Select()
                    .Where(x => x.DistrictId == districtId && x.Name.Equals(name))
                    .FirstOrDefault();
        }

        public VirtualTestCustomScore GetCustomScoreByID(int virtualTestCustomScoreID)
        {
            return
                virtualTestCustomScoreRepository.Select()
                    .Where(x => x.VirtualTestCustomScoreId == virtualTestCustomScoreID)
                    .FirstOrDefault();
        }

        public VirtualTestCustomScore UpdateCustomScoreName(int userId, int templateId, string name, bool? isMultiDate)
        {
            var item = GetCustomScoreByID(templateId);
            if (item != null)
            {
                item.Name = name;
                item.IsMultiDate = isMultiDate;
                SetUpdatingInformationCustomScore(item, userId);
                virtualTestCustomScoreRepository.Save(item);
            }
            return item;
        }
        public void Delete(VirtualTestCustomScore item)
        {
            virtualTestCustomScoreRepository.Delete(item);
        }

        private void SetUpdatingInformationCustomScore(VirtualTestCustomScore item, int userId)
        {
            item.UpdatedDate = DateTime.UtcNow;
        }

        public TemplateConversionSetDto ImportConversionTable(int id, string fileName, List<string> headers, IEnumerable<ConversionTableExcelItem> excelItems, bool isReplace)
        {
            var item = virtualTestCustomScoreRepository.Select().FirstOrDefault(x => x.VirtualTestCustomScoreId == id);
            if (isReplace)
            {
                item.UseRaw = false;
                item.UseCustomA1 = item.UseCustomA2 = item.UseCustomA3 = item.UseCustomA4
                    = item.UseCustomN1 = item.UseCustomN2 = item.UseCustomN3 = item.UseCustomN4 = null;

                _virtualTestCustomMetaDataRepository.DeleteMetaData(item.VirtualTestCustomScoreId);
            }

            AutoGenerateTemplateColumns(item, headers, excelItems);
            return UpdateConversionSet(item, fileName, excelItems);
        }

        public bool HasConversionSet(int id)
        {
            return virtualTestCustomScoreRepository.Select()
                .Any(x => x.VirtualTestCustomScoreId == id && (x.CustomN_1_ConversionSetID > 0 || x.CustomA_1_ConversionSetID > 0));
        }

        #region Private

        private void AutoGenerateTemplateColumns(VirtualTestCustomScore item, List<string> headers, IEnumerable<ConversionTableExcelItem> excelItems)
        {
            var virtualTestCustomScoreId = item.VirtualTestCustomScoreId;

            if (!item.UseRaw)
            {
                item.UseRaw = true;
                SaveNumericDefaultMetaData(virtualTestCustomScoreId, VirtualTestCustomMetaData.Raw);
            }

            var columnName = headers[1];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomN1 == true))
            {
                item.UseCustomN1 = true;
                item.CustomN1Label = columnName;

                var decimalScale = CommonUtils.GetMaxDecimalPlace(excelItems.Select(x => x.CustomNumeric1));
                SaveNumericDefaultMetaData(virtualTestCustomScoreId, VirtualTestCustomMetaData.CustomN_1, decimalScale);
            }

            columnName = headers[2];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomA1 == true))
            {
                item.UseCustomA1 = true;
                item.CustomA1Label = columnName;
                SaveTextDefaultMetaData(virtualTestCustomScoreId, VirtualTestCustomMetaData.CustomA_1);
            }

            columnName = headers[3];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomN2 == true))
            {
                item.UseCustomN2 = true;
                item.CustomN2Label = columnName;

                var decimalScale = CommonUtils.GetMaxDecimalPlace(excelItems.Select(x => x.CustomNumeric2));
                SaveNumericDefaultMetaData(virtualTestCustomScoreId, VirtualTestCustomMetaData.CustomN_2, decimalScale);
            }

            columnName = headers[4];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomA2 == true))
            {
                item.UseCustomA2 = true;
                item.CustomA2Label = columnName;
                SaveTextDefaultMetaData(virtualTestCustomScoreId, VirtualTestCustomMetaData.CustomA_2);
            }

            columnName = headers[5];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomN3 == true))
            {
                item.UseCustomN3 = true;
                item.CustomN3Label = columnName;

                var decimalScale = CommonUtils.GetMaxDecimalPlace(excelItems.Select(x => x.CustomNumeric3));
                SaveNumericDefaultMetaData(virtualTestCustomScoreId, VirtualTestCustomMetaData.CustomN_3, decimalScale);
            }

            columnName = headers[6];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomA3 == true))
            {
                item.UseCustomA3 = true;
                item.CustomA3Label = columnName;
                SaveTextDefaultMetaData(virtualTestCustomScoreId, VirtualTestCustomMetaData.CustomA_3);
            }

            columnName = headers[7];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomN4 == true))
            {
                item.UseCustomN4 = true;
                item.CustomN4Label = columnName;

                var decimalScale = CommonUtils.GetMaxDecimalPlace(excelItems.Select(x => x.CustomNumeric4));
                SaveNumericDefaultMetaData(virtualTestCustomScoreId, VirtualTestCustomMetaData.CustomN_4, decimalScale);
            }

            columnName = headers[8];
            if (!string.IsNullOrEmpty(columnName) && !(item.UseCustomA4 == true))
            {
                item.UseCustomA4 = true;
                item.CustomA4Label = columnName;
                SaveTextDefaultMetaData(virtualTestCustomScoreId, VirtualTestCustomMetaData.CustomA_4);
            }

            virtualTestCustomScoreRepository.Save(item);
        }

        private TemplateConversionSetDto UpdateConversionSet(VirtualTestCustomScore item, string fileName, IEnumerable<ConversionTableExcelItem> excelItems)
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

            virtualTestCustomScoreRepository.Save(item);

            return template;
        }

        private void SaveNumericDefaultMetaData(int virtualTestCustomScoreId, string scoreType, int decimalScale = 0)
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
                ScoreType = scoreType,
                MetaData = meta.GetJsonString()
            };
            _virtualTestCustomMetaDataRepository.Save(metaData);
        }

        private void SaveTextDefaultMetaData(int virtualTestCustomScoreId, string scoreType)
        {
            var meta = new VirtualTestCustomMetaModel
            {
                DataType = VirtualTestCustomMetaModel.DataTypeFreeForm,
                MaxLength = 100
            };

            var metaData = new VirtualTestCustomMetaData()
            {
                VirtualTestCustomScoreID = virtualTestCustomScoreId,
                ScoreType = scoreType,
                MetaData = meta.GetJsonString()
            };
            _virtualTestCustomMetaDataRepository.Save(metaData);
        }

        #endregion
    }
}
