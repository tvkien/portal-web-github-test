using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.Commons;
using LinkIt.BubbleSheetPortal.Models.DTOs.DistrictDecode;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageClass;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageStudent;
using LinkIt.BubbleSheetPortal.Models.Enum;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DistrictDecodeService
    {
        private readonly IReadOnlyRepository<DistrictDecode> repository;
        private readonly IReadOnlyRepository<Configuration> _configurationRepository;
        private readonly IReadOnlyRepository<DistrictDataParmDTO> _districtDataParamRepository;
        private readonly IReadOnlyRepository<AggregateSubjectMapping> _repositoryAggregateSubjectMapping;
        public DistrictDecodeService(IReadOnlyRepository<DistrictDecode> repository,
            IReadOnlyRepository<Configuration> configurationRepository,
            IReadOnlyRepository<DistrictDataParmDTO> districtDataParamRepository,
            IReadOnlyRepository<AggregateSubjectMapping> repositoryAggregateSubjectMapping)
        {
            this.repository = repository;
            _configurationRepository = configurationRepository;
            _districtDataParamRepository = districtDataParamRepository;
            _repositoryAggregateSubjectMapping = repositoryAggregateSubjectMapping;
        }

        public IQueryable<DistrictDecode> GetDistrictDecodesByLabel(string label)
        {
            return repository.Select().Where(x => x.Label.Equals(label));
        }

        public IQueryable<DistrictDecode> GetDistrictDecodesOfSpecificDistrictByLabel(int districtId, string label)
        {
            var dataConfig = repository.Select().Where(x => x.DistrictID == districtId && x.Label.Equals(label));
            if (dataConfig == null || dataConfig.FirstOrDefault() == null || (dataConfig.FirstOrDefault() != null && string.IsNullOrEmpty(dataConfig.FirstOrDefault().Value)))
            {
                return _configurationRepository.Select().Where(w => w.Name == label).Select(s => new DistrictDecode()
                {
                    DistrictID = districtId,
                    Value = s.Value,
                });
            }
            return dataConfig;
        }
        public IQueryable<AggregateSubjectMapping> GetAggregateSubjectMappingByDistrict(int districtId)
        {
            var checkConfigDecode = repository.Select().Where(x => x.DistrictID == districtId && x.Label.Equals(Constanst.CLASS_META_DATA));

            if (checkConfigDecode == null || checkConfigDecode.FirstOrDefault() == null || (checkConfigDecode.FirstOrDefault() != null && string.IsNullOrEmpty(checkConfigDecode.FirstOrDefault().Value)))
            {
                return _repositoryAggregateSubjectMapping.Select().Where(x => x.DistrictID == null).OrderBy(o => o.AggregateSubjectName);
            }
            return _repositoryAggregateSubjectMapping.Select().Where(x => x.DistrictID == districtId).OrderBy(o => o.AggregateSubjectName);
        }

        public string GetDistrictDecodeValidations(int districtId, Dictionary<string, string> validateValues)
        {
            var message = string.Empty;
            var validates = repository.Select().Where(x => x.DistrictID == districtId && validateValues.Keys.ToList().Contains(x.Label)).ToList();
            if (validates.Any())
            {
                foreach (var validateValue in validateValues)
                {
                    try
                    {
                        var districtDecodeConfig = validates.FirstOrDefault(x => x.Label == validateValue.Key && !string.IsNullOrEmpty(x.Value));
                        if (districtDecodeConfig == null) continue;

                        var validationRule = JsonConvert.DeserializeObject<ValidateLocalCodeDTO>(districtDecodeConfig.Value);
                        foreach (var item in validationRule.Formats)
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(!string.IsNullOrEmpty(validateValue.Value) ? validateValue.Value : string.Empty, item))
                            {
                                message = string.Empty;
                                break;
                            }
                            message = validationRule.ErrorMessage;
                        }
                        if (!string.IsNullOrEmpty(message))
                            return message;
                    }
                    catch
                    {
                        return "Validation error: stored validation format is incorrect. Please contact your system administrator.";
                    }
                }
            }
            return message;
        }

        public int GetSGOMAXPostAssessment(int districtID)
        {
            var sgoMAXPostAssessmentDistrictDecode =
              GetDistrictDecodesOfSpecificDistrictByLabel(districtID,
                   "SGOMAXPostAssessment").FirstOrDefault();
            var sgoMAXPostAssessment = 0;
            if (sgoMAXPostAssessmentDistrictDecode != null)
            {
                int.TryParse(sgoMAXPostAssessmentDistrictDecode.Value, out sgoMAXPostAssessment);
            }

            if (sgoMAXPostAssessment <= 0) sgoMAXPostAssessment = 1;

            return sgoMAXPostAssessment;
        }

        public int GetSGOMAXPreAssessment(int districtID)
        {
            var sgoMAXPreAssessmentDistrictDecode =
               GetDistrictDecodesOfSpecificDistrictByLabel(districtID,
                   "SGOMAXPreAssessment").FirstOrDefault();
            var sgoMAXPreAssessment = 0;
            if (sgoMAXPreAssessmentDistrictDecode != null)
            {
                int.TryParse(sgoMAXPreAssessmentDistrictDecode.Value, out sgoMAXPreAssessment);
            }

            if (sgoMAXPreAssessment <= 0) sgoMAXPreAssessment = 4;

            return sgoMAXPreAssessment;
        }
        public DateFormatModel GetDateFormat(int districtId)
        {
            var model = new DateFormatModel();
            model.DistrictId = districtId;
            var districtDecodes = repository.Select().Where(x => x.DistrictID == districtId
                && (x.Label == Constanst.DateFormat || x.Label == Constanst.TimeFormat || x.Label == Constanst.JQueryDateFormat || x.Label == Constanst.HandsonTableDateFormat)).ToList();

            var decode = districtDecodes.SingleOrDefault(x => x.Label == Constanst.DateFormat);
            model.DateFormat = decode == null ? string.Empty : decode.Value;

            decode = districtDecodes.SingleOrDefault(x => x.Label == Constanst.TimeFormat);
            model.TimeFormat = decode == null ? string.Empty : decode.Value;

            decode = districtDecodes.SingleOrDefault(x => x.Label == Constanst.JQueryDateFormat);
            model.JQueryDateFormat = decode == null ? string.Empty : decode.Value;

            decode = districtDecodes.SingleOrDefault(x => x.Label == Constanst.HandsonTableDateFormat);
            model.HandsonTableDateFormat = decode == null ? string.Empty : decode.Value;

            //Get from table Configuration if there's no specified for district on DistrictDecode
            if (string.IsNullOrEmpty(model.DateFormat))
            {
                var config = _configurationRepository.Select().FirstOrDefault(x => x.Name == Constanst.DateFormat);
                if (config != null)
                {
                    model.DateFormat = config.Value;
                }
                else
                {
                    //use default value
                    model.DateFormat = Constanst.DefaultDateFormatValue;
                }
            }
            if (string.IsNullOrEmpty(model.TimeFormat))
            {
                var config = _configurationRepository.Select().FirstOrDefault(x => x.Name == Constanst.TimeFormat);
                if (config != null)
                {
                    model.TimeFormat = config.Value;
                }
                else
                {
                    //use default value
                    model.TimeFormat = Constanst.DefaultTimeFormatValue;
                }
            }

            if (string.IsNullOrEmpty(model.JQueryDateFormat))
            {
                var config = _configurationRepository.Select().FirstOrDefault(x => x.Name == Constanst.JQueryDateFormat);
                if (config != null)
                {
                    model.JQueryDateFormat = config.Value;
                }
                else
                {
                    //use default value
                    model.JQueryDateFormat = Constanst.DefaultJqueryDateFormatValue;
                }
            }
            if (string.IsNullOrEmpty(model.HandsonTableDateFormat))
            {
                var config = _configurationRepository.Select().FirstOrDefault(x => x.Name == Constanst.HandsonTableDateFormat);
                if (config != null)
                {
                    model.HandsonTableDateFormat = config.Value;
                }
                else
                {
                    //use default value
                    model.HandsonTableDateFormat = Constanst.DefaultHandsonTableDateFormat;
                }
            }
            return model;
        }

        public bool GetAccessRegistration(int districtId)
        {
            var val = this.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, "IsAccessRegistration").SingleOrDefault();
            bool isAccessRegistration = false;
            bool.TryParse(val == null ? string.Empty : val.Value, out isAccessRegistration);
            return isAccessRegistration;
        }

        public bool GetDistrictDecodeByLabel(int districtId, string label)
        {
            var districtDecode = GetDistrictDecodesOfSpecificDistrictByLabel(districtId, label).FirstOrDefault();
            var value = false;
            if (districtDecode != null)
            {
                bool.TryParse(districtDecode.Value, out value);
            }
            return value;
        }

        public bool CheckDistrictDecodeExistDistricts(List<int> districtIds, string label)
        {
            var distinctDistrictIds = districtIds.Distinct().ToList();
            var districtDecode = repository.Select().Where(x => distinctDistrictIds.Contains(x.DistrictID) && x.Label.Equals(label) && x.Value.ToLower() == "true");
            if (districtDecode.Select(x => x.DistrictID).Distinct().Count() == distinctDistrictIds.Count)
                return true;
            return false;
        }

        public List<string> GetStudentMeta(int districtId)
        {
            var districtDecode = GetDistrictDecodesOfSpecificDistrictByLabel(districtId, ContaintUtil.StudentMetaData).FirstOrDefault();
            if (districtDecode != null)
            {
                var lst = districtDecode.Value.Split('|').ToList();
                return lst;
            }
            return new List<string>();
        }

        public List<StudentMetaDataDto> GetStudentMetaLabel(int districtId)
        {
            var districtDecode = GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, ContaintUtil.StudentMetaData);
            if (districtDecode == null)
                return null;

            try
            {
                return JsonConvert.DeserializeObject<StudentMetaDataLabelsDto>(districtDecode.Value).StudentMetaDataLabels;
            }
            catch
            {
                return null;
            }
        }

        public bool GetDistrictDecodesIsTrueByLabel(int districtId, string label)
        {
            try
            {
                if (districtId > 0 && !string.IsNullOrEmpty(label))
                {
                    var obj = repository.Select().FirstOrDefault(x => x.DistrictID == districtId && x.Label.ToLower().Equals(label.ToLower()));
                    if (obj != null && obj.Value.ToLower().Equals("true"))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private const string AbleToReceiveTLDSProfile = "AbleToReceiveTLDSProfile";
        public List<int> GetReceivingTLDSProfileDistrictID()
        {
            var districtDecodes = GetDistrictDecodesByLabel(AbleToReceiveTLDSProfile);
            var result = new List<int>();
            foreach (var districtDecode in districtDecodes)
            {
                if (districtDecode.Value != null && districtDecode.Value.ToLower() == "true")
                {
                    result.Add(districtDecode.DistrictID);
                }
            }
            return result;
        }

        public LogOnViaSSOConfig GetLogOnViaSsoConfig(int districtId)
        {
            var result = new LogOnViaSSOConfig();
            result.DistrictID = districtId;
            var logOnViaSSO = this.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, "LogOnViaSSO").FirstOrDefault();
            if (logOnViaSSO != null && !string.IsNullOrEmpty(logOnViaSSO.Value))
            {
                bool value = false;
                bool.TryParse(logOnViaSSO.Value, out value);
                result.LogOnViaSSO = value;
            }
            if (result.LogOnViaSSO)
            {
                var config = this.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, "SSOLogonURL").FirstOrDefault();
                if (config != null)
                {
                    result.SSOLogonURL = config.Value;
                }
            }
            return result;
        }

        public DistrictDecode GetDistrictDecodeOfDistrictOrConfigurationByLabel(int districtId, string label)
        {
            var districtDecode = repository.Select().Where(x => x.DistrictID == districtId && x.Label.Equals(label)).FirstOrDefault();

            if (districtDecode == null)
            {
                var config = _configurationRepository.Select().FirstOrDefault(x => x.Name == label);
                if (config != null)
                {
                    districtDecode = new DistrictDecode
                    {
                        Label = config.Name,
                        Value = config.Value
                    };
                }
            }

            return districtDecode;
        }

        public bool GetDistrictDecodeOrConfigurationByLabel(int districtId, string label, bool defaultValue = false)
        {
            var districtDecode = GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, label);
            if (districtDecode != null)
            {
                bool.TryParse(districtDecode.Value, out defaultValue);
            }

            return defaultValue;
        }

        public MetaFormatDateDto GetMetaFormatDate(int districtId)
        {
            try
            {
                var configuration = _configurationRepository.Select().FirstOrDefault(d => d.Name == TextConstants.META_FORMAT_DATE);
                if (configuration != null)
                    return JsonConvert.DeserializeObject<MetaFormatDateDto>(configuration.Value);
            }
            catch
            {
                throw new Exception("There is incorrect MetaFormatDate value in Configuration. Student custom fields are not available.");
            }
            return null;
        }

        public bool GetTimeOutDistrictDecodeByLabel(int districtId, string label)
        {
            var districtDecode = GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, label);
            var value = true;
            if (districtDecode != null)
            {
                bool.TryParse(districtDecode.Value, out value);
            }
            return value;
        }

        public bool StudentLogonRequireKioskMode(int districtId)
        {
            if (districtId > 0)
            {
                var districtDecode = repository.Select().FirstOrDefault(x => x.DistrictID == districtId && x.Label.Equals(Constanst.STUDENT_LOGIN_REQUIRE_KEY));
                if (districtDecode != null && string.IsNullOrEmpty(districtDecode.Value) == false)
                {
                    return districtDecode.Value.Equals(Constanst.BOOL_STRING_TRUE, StringComparison.OrdinalIgnoreCase);
                }
            }
            return false;
        }

        public List<DistrictDataParmDTO> GetDistrictSuportAttendanceGradeByDistrictId(int categoryId, int districtId, string strListLACImportTypes)
        {
            List<DistrictDataParmDTO> lstReturn = new List<DistrictDataParmDTO>();
            List<string> lstImportTypes = strListLACImportTypes.Split(',').ToList();
            if (lstImportTypes.Count > 0)
            {
                return _districtDataParamRepository.Select()
                    .Where(o => o.DistrictID == districtId && o.DataSetCategoryID == categoryId && lstImportTypes.Contains(o.ImportType))
                    .ToList();
            }
            return lstReturn;
        }

        public List<AttendanceCustom> GetAllDistrictSuportAttendance(string strListLACImportTypes)
        {
            List<AttendanceCustom> lstReturn = new List<AttendanceCustom>();
            List<string> lstImportTypes = strListLACImportTypes.Split(',').ToList();
            if (lstImportTypes.Count > 0)
            {
                var lstDistrictDataParmDTO = _districtDataParamRepository
                                .Select()
                                .Where(o => o.DistrictID > 0 && lstImportTypes.Contains(o.ImportType))
                                .ToList();

                if (lstDistrictDataParmDTO != null && lstDistrictDataParmDTO.Count > 0)
                {
                    lstReturn = lstDistrictDataParmDTO.Select(o => new AttendanceCustom()
                    {
                        DistrictID = o.DistrictID,
                        DataSetCategoryID = o.DataSetCategoryID
                    }).ToList();
                }
            }
            return lstReturn;
        }

        public bool DistrictSupportTestTakerNewSkin(int districtId)
        {
            try
            {
                bool districtSupportNewSkin = false;
                var configValue = GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, ContaintUtil.DISTRICT_NEWSKIN_KEY)?.Value;
                if (!string.IsNullOrEmpty(configValue) && bool.TryParse(configValue, out districtSupportNewSkin))
                {
                    return districtSupportNewSkin;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DistrictSupporPortalNewSkin(int districtId)
        {
            try
            {
                var configValue = GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, ContaintUtil.PORTAL_USENEWDESIGN)?.Value;
                return bool.Parse(configValue);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<MetaDataKeyValueDto> GetParentMetaLabel(int districtId)
        {
            try
            {
                var districtDecode = GetDistrictDecodesOfSpecificDistrictByLabel(districtId, ContaintUtil.PARENTMETADATA).FirstOrDefault();
                if (districtDecode == null)
                {
                    var config = _configurationRepository.Select().FirstOrDefault(x => x.Name == ContaintUtil.PARENTMETADATA);
                    if (config != null)
                    {
                        return JsonConvert.DeserializeObject<MetaDataKeyValueLabelsDto>(config.Value).MetaDataLabels;
                    }
                }
                else
                {
                    return JsonConvert.DeserializeObject<MetaDataKeyValueLabelsDto>(districtDecode.Value).MetaDataLabels;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<AssessmentArtifactFileTypeGroupDTO> GetAssessmentArtifactFileTypeGroups(int districtId)
        {
            try
            {
                var assessmentArtifactFileTypeGroups =
                    this.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId
                        , DistrictDecodeLabelConstant.AssessmentArtifactFileTypeGroup);

                if (assessmentArtifactFileTypeGroups == null || assessmentArtifactFileTypeGroups.Value == null)
                {
                    return Enumerable.Empty<AssessmentArtifactFileTypeGroupDTO>();
                }

                return JsonConvert.DeserializeObject<List<AssessmentArtifactFileTypeGroupDTO>>((assessmentArtifactFileTypeGroups.Value));
            }

            catch (Exception)
            {
                return Enumerable.Empty<AssessmentArtifactFileTypeGroupDTO>();
            }
        }

        public List<SchoolMetaDataDto> GetSchoolMetaLabel(int districtId)
        {
            var districtDecode = GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, ContaintUtil.SchoolMetaData);
            if (districtDecode == null)
                return null;
            try
            {
                return JsonConvert.DeserializeObject<SchoolMetaDataLabelsDto>(districtDecode.Value).SchoolMetaDataLabels;
            }
            catch
            {
                return null;
            }
        }

        public List<Configuration> GetConfigurationValues(List<string> names)
        {
            return _configurationRepository.Select().Where(x => names.Contains(x.Name)).ToList();
        }
        public List<ClassMetaDataDto> GetClassMetaLabel(int districtId)
        {
            var districtDecode = GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, ContaintUtil.ClassMetaData);
            if (districtDecode == null)
                return null;
            try
            {
                return JsonConvert.DeserializeObject<ClassMetaDataLabelDto>(districtDecode.Value).ClassMetaDataLabels;
            }
            catch
            {
                return null;
            }
        }

        public bool IsPortalStaffSkipsRuleCheckLeadingZerosInUserCode(int districtId)
        {
            var districtDecode = GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, ContaintUtil.PORTAL_STAFF_SKIPS_RULE_CHECK_LEADING_ZEROS_IN_USER_CODE);

            if (districtDecode == null)
                return false;

            bool.TryParse(districtDecode.Value?.ToLower(), out var isSkip);
            return isSkip;
        }

        public int GetDistrictDecodeOrConfigurationByLabel(int districtId, string label, int defaultValue)
        {
            var districtDecode = GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, label);
            if (districtDecode != null)
            {
                int.TryParse(districtDecode.Value, out defaultValue);
            }

            return defaultValue;
        }
    }
}
