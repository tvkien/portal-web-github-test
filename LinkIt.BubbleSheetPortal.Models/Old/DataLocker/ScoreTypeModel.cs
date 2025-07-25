using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class ScoreTypeModel
    {
        public static readonly string RAW_SCORE = "RAW_SCORE";
        public static readonly string PERCENT_SCORE = "PERCENT_SCORE";
        public static readonly string PERCENTILE_SCORE = "PERCENTILE_SCORE";
        public static readonly string SCALED_SCORE = "SCALED_SCORE";
        public static readonly string NUMERIC_CUSTOM_SCORE = "NUMERIC_CUSTOM_SCORE";
        public static readonly string TEXT_CUSTOM_SCORE = "TEXT_CUSTOM_SCORE";
        public static readonly string ARTIFACT_SCORE = "ARTIFACT_SCORE";
        public static readonly string NOTE_COMMENT = "NOTE_COMMENT";

        public ScoreTypeModel()
        {
            IsFreeText = false;
            IsListText = false;
            UseCustomN1 =
                UseCustomN2 = UseCustomN3 = UseCustomN4 = UseCustomA1 = UseCustomA2 = UseCustomA3 = UseCustomA4 = false;
            DisplayOrder = 0;
            UseNote = false;

            Meta = new VirtualTestCustomMetaModel();
            AssessmentArtifactFileTypeGroupViewModel = new List<AssessmentArtifactFileTypeGroupEditScoreModel>();
       }
        public int TemplateID { get; set; }
        public int? SubscoreId { get; set; }//SubscoreId has value greater than 0 if this score type belong to a subscore ( VirtualTestCustomSubScore )
        public string ScoreTypeCode { get; set; }
        public string ScoreTypeName
        {
            get
            {
                if (ScoreTypeCode == RAW_SCORE)
                {
                    return "Raw score";
                }
                if (ScoreTypeCode == PERCENT_SCORE)
                {
                    return "Percent score";
                }
                if (ScoreTypeCode == PERCENTILE_SCORE)
                {
                    return "Percentile score";
                }
                if (ScoreTypeCode == SCALED_SCORE)
                {
                    return "Scaled score";
                }
                if (ScoreTypeCode == NUMERIC_CUSTOM_SCORE)
                {
                    return "Numeric custom score";
                }
                if (ScoreTypeCode == TEXT_CUSTOM_SCORE)
                {
                    return "Text custom score";
                }
                if (ScoreTypeCode == ARTIFACT_SCORE)
                {
                    return "Artifact";
                }
                if (ScoreTypeCode == NOTE_COMMENT)
                {
                    return "Notes/Comments";
                }
                return string.Empty;
            }

        }
        public string ScoreName { get; set; }
        public string ScoreType { get; set; }//one of these values: Raw, Scaled, Percent, Percentile, CustomN_1, CustomN_2, CustomN_3, CustomN_4, CustomA_1, CustomA_2, CustomA_3, CustomA_4
        public string CustomScoreName { get; set; }
        public string Description { get; set; }
        public decimal? MinScore { get; set; }
        public decimal? MaxScore { get; set; }
        public int? NumberOfDecimal { get; set; }
        public bool? IsFreeText { get; set; }
        public bool? IsListText { get; set; }
        public int? MaxAllowedCharacterNumber { get; set; }
        public bool? AllowFreeForm { get; set; }
        public bool? AllowAlphanumericOnly { get; set; }
        public bool? AllowUpperCaseOnly { get; set; }
        public bool? AllowAlphabeticOnly { get; set; }
        public string UploadFileTypes { get; set; }
        public int MaxFileSize { get; set; }
        public bool IsAutoCalculation { get; set; }
        public string DerivedName { get; set; } //sum, average, percent
        public string Expression { get; set; }
        public string FormatOption { get; set; }
        public string DisplayOption { get; set; }
        public List<string> ListText
        {
            get
            {
                if (ListTextString != null)
                {
                    if (!string.IsNullOrEmpty(FormatOption) && FormatOption.ToLower() == "labelvaluetext")
                    {
                        var selectList = new JavaScriptSerializer()
                            .Deserialize<List<VirtualTestCustomMetaSelectListOptionModel>>
                            (ListTextString);
                        return selectList.Select(x => x.Option).ToList();
                    }

                    var array = ListTextString.Split(',');
                    if (array != null)
                    {
                        return array.ToList();
                    }
                }
                return new List<string>();
            }
        }

        public string ListTextString { get; set; }
        public List<string> ListArtifactTag
        {
            get
            {
                if (ListArtifactTagString != null)
                {
                    var array = ListArtifactTagString.Split(',');
                    if (array != null)
                    {
                        return array.ToList();
                    }
                }
                return new List<string>();
            }
        }
        public string ListArtifactTagString { get; set; }
        public string Overview
        {
            get
            {
                string overview
                    = string.Empty;
                if ((ScoreTypeCode == ScoreTypeModel.RAW_SCORE) ||
                     ScoreTypeCode == ScoreTypeModel.PERCENT_SCORE ||
                     ScoreTypeCode == ScoreTypeModel.PERCENTILE_SCORE ||
                     ScoreTypeCode == ScoreTypeModel.SCALED_SCORE ||
                     (ScoreTypeCode == ScoreTypeModel.NUMERIC_CUSTOM_SCORE))
                {
                    //Numeric
                    if (IsAutoCalculation)
                    {
                        if (DerivedName == "calculation")
                        {
                            overview = string.Format("{0}:{1}", DerivedName, Expression);
                        } else if (DerivedName == "percent")
                        {
                            overview =
                                string.Format("- {0} of Raw and</br> {1}",
                                    DerivedName, "{MaxScore}");
                        }
                    }    
                    else
                    {
                        var minScoreString = "NA";
                        var maxScoreString = "NA";
                        if (MinScore.HasValue && NumberOfDecimal.HasValue)
                        {                              
                            minScoreString = MinScore.Value.ToString();                            
                        }
                        if (MaxScore.HasValue && NumberOfDecimal.HasValue)
                        {                            
                            maxScoreString = MaxScore.Value.ToString(); 
                        }
                        if (ListText != null && ListText.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(FormatOption) && FormatOption.ToLower() == "labelvaluetext")
                            {
                                var options =
                                new JavaScriptSerializer().Deserialize<List<VirtualTestCustomMetaSelectListOptionModel>>
                                    (ListTextString);
                                var value = options.Select(x => string.Format("{0} ({1})", x.Label, x.Option)).ToList();
                                overview = string.Format("- List value: {0} </br>", string.Join(", ", value));
                                if (DisplayOption == "label")
                                {
                                    overview += "- Display Labels Only";
                                }
                                if (DisplayOption == "value")
                                {
                                    overview += "- Display Value Only";
                                }
                                if (DisplayOption == "both")
                                {
                                    overview += "- Display Both Label and Value";
                                }
                            }
                            else
                            {
                                overview = string.Format("- List value: {0}", ListTextString);
                            }
                        }
                        else
                        {
                            overview =
                                string.Format("- Min Score:{0}</br> - Max Score:{1} </br> - Number of decimal:{2}</br>",
                                    minScoreString,
                                    maxScoreString,
                                    NumberOfDecimal.HasValue ? NumberOfDecimal.Value.ToString() : "NA");
                        }
                    }
                }
                else if (ScoreTypeCode == ScoreTypeModel.ARTIFACT_SCORE)
                {
                    overview = string.Format("- File Types:{0}",
                        UploadFileTypes);
                }
                else
                {
                    var prefix = ScoreTypeCode == ScoreTypeModel.TEXT_CUSTOM_SCORE ? "text" : "value";
                    if (IsFreeText.HasValue && IsFreeText.Value)
                    {
                        overview =
                            string.Format(
                                "- Free {5} </br> - Max number of characters allowed:{0} </br> - Allow free-form:{1} </br> - Allow alphanumeric only:{2} </br> - Allow upper case only:{3} </br> - Allow alphabetic only:{4}", MaxAllowedCharacterNumber.HasValue ? MaxAllowedCharacterNumber.Value.ToString() : "NA",
                                AllowFreeForm.HasValue ? AllowFreeForm.Value == true ? "Yes" : "No" : "No",
                                AllowAlphanumericOnly.HasValue ? AllowAlphanumericOnly.Value == true ? "Yes" : "No" : "No",
                                AllowUpperCaseOnly.HasValue ? AllowUpperCaseOnly.Value == true ? "Yes" : "No" : "No",
                                AllowAlphabeticOnly.HasValue ? AllowAlphabeticOnly.Value == true ? "Yes" : "No" : "No", prefix);
                    }
                    if (IsListText.HasValue && IsListText.Value)
                    {
                        overview =
                            string.Format("- List {1}: {0}", ListTextString, prefix);
                    }
                }
                return overview;
            }
        }
        public bool IsCustomScoreType
        {
            get
            {
                if (ScoreTypeCode == ScoreTypeModel.NUMERIC_CUSTOM_SCORE
                    || ScoreTypeCode == ScoreTypeModel.TEXT_CUSTOM_SCORE || ScoreTypeCode == ScoreTypeModel.NOTE_COMMENT
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool IsNoteType
        {
            get
            {
                if (ScoreTypeCode == NOTE_COMMENT)
                    return true;
                return false;
            }
        }
        public bool IsNumeric
        {
            get
            {

                if ((ScoreTypeCode == ScoreTypeModel.RAW_SCORE) ||
                    ScoreTypeCode == ScoreTypeModel.PERCENT_SCORE ||
                    ScoreTypeCode == ScoreTypeModel.PERCENTILE_SCORE ||
                    ScoreTypeCode == ScoreTypeModel.SCALED_SCORE ||
                    (ScoreTypeCode == ScoreTypeModel.NUMERIC_CUSTOM_SCORE))
                {
                    return true;
                }
                return false;
            }
        }
        public string ScoreNameAbsolute
        {
            get
            {
                if (IsCustomScoreType || IsNoteType)
                {
                    return CustomScoreName;
                }
                return ScoreName;
            }
        }

        //A score type has no or one VirtualTestCustomMetaModel
        public int? VirtualTestCustomMetaDataID { get; set; }
        public VirtualTestCustomMetaModel Meta
        {
            get
            {
                var meta = new VirtualTestCustomMetaModel();
                meta.Description = Description;

                if (ScoreTypeCode == ScoreTypeModel.ARTIFACT_SCORE)
                {
                    meta.DataType = VirtualTestCustomMetaModel.DataTypeArtifact;
                    meta.UploadFileTypes = UploadFileTypes;
                    meta.MaxFileSize = MaxFileSize;
                    meta.ListArtifactTag = ListArtifactTag;
                }
                if (IsNumeric)
                {
                    meta.DataType = VirtualTestCustomMetaModel.DataTypeNumeric;
                    meta.MaxValue = MaxScore;
                    meta.MinValue = MinScore;
                    meta.DecimalScale = NumberOfDecimal;
                    meta.DataHostPot = DataHostPot;
                    meta.IsAutoCalculation = IsAutoCalculation;
                    meta.DerivedName = DerivedName;
                    meta.Expression = Expression;
                    meta.FormatOption = FormatOption;
                    meta.DisplayOption = DisplayOption;
                    //for predefinelist
                    if (ListText != null && ListText.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(FormatOption) && FormatOption.ToLower() == "labelvaluetext")
                        {
                            meta.SelectListOptions =
                                new JavaScriptSerializer().Deserialize<List<VirtualTestCustomMetaSelectListOptionModel>>
                                    (ListTextString);
                        }
                        else
                        {
                            int order = 0;
                            meta.SelectListOptions =
                                ListText.Select(x => new VirtualTestCustomMetaSelectListOptionModel()
                                {
                                    Option = x,
                                    Order = order++
                                }).ToList();
                        }
                    }

                }
                else
                {
                    //Text
                    if (IsListText.HasValue && IsListText.Value)
                    {
                        meta.DataType = VirtualTestCustomMetaModel.DataTypeSelectList;
                        if (ListText != null && ListText.Count > 0)
                        {
                            int order = 0;
                            meta.SelectListOptions = ListText.Select(x => new VirtualTestCustomMetaSelectListOptionModel()
                            {
                                Option = x,
                                Order = order++
                            }).ToList();
                        }
                    }
                    else
                    {
                        if (IsFreeText.HasValue && IsFreeText.Value)
                        {
                            meta.DataType = VirtualTestCustomMetaModel.DataTypeFreeForm;
                        }

                        if (AllowAlphabeticOnly.HasValue && AllowAlphabeticOnly.Value)
                        {
                            meta.DataType = VirtualTestCustomMetaModel.DataTypeAlphabetic;
                        }
                        if (AllowAlphanumericOnly.HasValue && AllowAlphanumericOnly.Value)
                        {
                            meta.DataType = VirtualTestCustomMetaModel.DataTypeAlphanumeric;
                        }

                        meta.MaxLength = MaxAllowedCharacterNumber;
                        meta.UpperCaseOnly = AllowUpperCaseOnly;
                    }
                }

                return meta;
            }

            set
            {
                if (value != null)
                {
                    var meta = (VirtualTestCustomMetaModel)value;
                    Description = meta.Description;
                    if (meta.DataType == VirtualTestCustomMetaModel.DataTypeSelectList)
                    {
                        IsListText = true;
                    }
                    else
                    {
                        if (meta.DataType == VirtualTestCustomMetaModel.DataTypeAlphabetic)
                        {
                            IsFreeText = true;
                            AllowAlphabeticOnly = true;
                        }
                        if (meta.DataType == VirtualTestCustomMetaModel.DataTypeAlphanumeric)
                        {
                            IsFreeText = true;
                            AllowAlphanumericOnly = true;
                        }

                        if (meta.DataType == VirtualTestCustomMetaModel.DataTypeFreeForm)
                        {
                            AllowFreeForm = true;
                            IsFreeText = true;
                        }
                    }
                    if (meta.DataType == VirtualTestCustomMetaModel.DataTypeArtifact)
                    {
                        UploadFileTypes = meta.UploadFileTypes;
                        MaxFileSize = meta.MaxFileSize;
                        if (meta.ListArtifactTag != null && meta.ListArtifactTag.Count > 0)
                        {
                            ListArtifactTagString = string.Join(",", meta.ListArtifactTag.ToList());
                        }
                    }

                    if (meta.SelectListOptions != null && meta.SelectListOptions.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(meta.FormatOption) && meta.FormatOption.ToLower() == "labelvaluetext")
                            ListTextString = new JavaScriptSerializer().Serialize(meta.SelectListOptions);
                        else
                            ListTextString = string.Join(",", meta.SelectListOptions.OrderBy(x => x.Order).Select(x => x.Option).ToList());
                    }

                    MaxScore = meta.MaxValue;
                    MinScore = meta.MinValue;
                    NumberOfDecimal = meta.DecimalScale;
                    AllowUpperCaseOnly = meta.UpperCaseOnly;
                    MaxAllowedCharacterNumber = meta.MaxLength;
                    DataHostPot = meta.DataHostPot;

                    IsAutoCalculation = meta.IsAutoCalculation;
                    DerivedName = meta.DerivedName;
                    Expression = meta.Expression;
                    FormatOption = meta.FormatOption;
                    DisplayOption = meta.DisplayOption;
                }
            }

        }
        public bool UseCustomN1 { get; set; }
        public bool UseCustomN2 { get; set; }
        public bool UseCustomN3 { get; set; }
        public bool UseCustomN4 { get; set; }
        public bool UseCustomA1 { get; set; }
        public bool UseCustomA2 { get; set; }
        public bool UseCustomA3 { get; set; }
        public bool UseCustomA4 { get; set; }
        public bool UseArtifact { get; set; }
        public bool? UseNote { get; set; }
        public string ValidateChange(bool hasTestResult, ScoreTypeModel existingObj)
        {
            // existingObj is the scoretype in database, which will be changed
            if (IsNumeric)
            {
                if (NumberOfDecimal.HasValue && (NumberOfDecimal.Value > 3 || NumberOfDecimal.Value < 0))
                {
                    return "No. Of Decimal Points must be between 0 and 3.";
                }

                if (hasTestResult)
                {
                    // if there are test results, they can only decrease the min and/or increase the max, in order to protect the integrity of existing data.
                    if (existingObj.MinScore.HasValue)
                    {
                        if (MinScore.HasValue)
                        {
                            if (MinScore.Value > existingObj.MinScore.Value)
                            {
                                return
                                    string.Format(
                                        "The new Min Score {0} is invalid. It must be equal or less than {1}", MinScore,
                                        existingObj.MinScore);
                            }
                        }
                    }
                    if (existingObj.MaxScore.HasValue)
                    {
                        if (MaxScore.HasValue)
                        {
                            if (MaxScore.Value < existingObj.MaxScore.Value)
                            {
                                return
                                    string.Format(
                                        "The new Max Score {0} is invalid. It must be equal or greater than {1}",
                                        MaxScore,
                                        existingObj.MaxScore);
                            }
                        }
                    }
                }
            }
            else
            {

                if (hasTestResult)
                {
                    if (MaxAllowedCharacterNumber.HasValue && existingObj.MaxAllowedCharacterNumber.HasValue)
                    {
                        if (MaxAllowedCharacterNumber.Value < existingObj.MaxAllowedCharacterNumber.Value)
                        {
                            return
                                    string.Format(
                                        "The new Max characters allowed = {0} is not valid. It must be equal or greater than {1}",
                                        MaxAllowedCharacterNumber,
                                        existingObj.MaxAllowedCharacterNumber);
                        }
                    }

                    /*
                     *The values in a dropdown list. They can always reorder the values on the list,
                     *  and add to the list. And if there are no test results, they can also delete and
                     *   rename the values. However, if there are test results, they can NOT delete or rename.
                     */
                    if (IsListText.HasValue && IsListText.Value)
                    {
                        //check if there's any item has been deleted or added
                        if (ListText.Count != existingObj.ListText.Count)
                        {
                            return "The list text can not be deleted or added.";
                        }
                        else
                        {
                            //check if there's any item has been renamed
                            for (var i = 0; i < ListText.Count; i++)
                            {
                                if (ListText[i] != existingObj.ListText[i])
                                {
                                    return string.Format("{0} has been changed to {1}.", existingObj.ListText[i],
                                        ListText[i]);
                                }
                            }
                        }
                    }

                }
            }

            return null;
        }
        public void CorrectModelData()
        {
            if (IsNumeric)
            {
                IsFreeText = null;
                IsListText = null;
                MaxAllowedCharacterNumber = null;
                AllowFreeForm = null;
                AllowAlphanumericOnly = null;
                AllowUpperCaseOnly = null;
                AllowAlphabeticOnly = null;
                IsAutoCalculation = IsAutoCalculation;
                DerivedName = DerivedName;
            }
            else
            {
                //Text -> clear some fields of numeric
                MinScore = null;
                MaxScore = null;
                NumberOfDecimal = null;
                DataHostPot = string.Empty;
            }
            if (ScoreTypeCode == NOTE_COMMENT)
            {
                UseNote = true;
            }
        }
        public int DisplayOrder { get; set; }
        public string GetDuplicateNameErrorMessage()
        {
            var error = string.Empty;
            if (ScoreTypeCode == NOTE_COMMENT)
            {
                error = string.Format("{0} already exists. Please give the note a different name.", ScoreNameAbsolute);
                return error;
            }

            if (IsCustomScoreType)
            {
                error = string.Format("{0} already exists. Please give the score a different name.",
                    ScoreNameAbsolute);
            }
            else
            {
                error = string.Format("{0} score already exists. Please choose a different score type.",
                  ScoreNameAbsolute);
            }

            return error;
        }
        public string ShortOverview
        {
            get
            {
                string overview
                    = string.Empty;
                if (ScoreTypeCode == ScoreTypeModel.RAW_SCORE || ScoreTypeCode == ScoreTypeModel.RAW_SCORE ||
                    ScoreTypeCode == ScoreTypeModel.PERCENT_SCORE
                    || ScoreTypeCode == ScoreTypeModel.PERCENTILE_SCORE || ScoreTypeCode == ScoreTypeModel.SCALED_SCORE ||
                    ScoreTypeCode == ScoreTypeModel.NUMERIC_CUSTOM_SCORE)
                {
                    //Numeric
                    var minScoreString = "NA";
                    var maxScoreString = "NA";
                    if (MinScore.HasValue && NumberOfDecimal.HasValue)
                    {                        
                         minScoreString = MinScore.Value.ToString();                        
                    }
                    if (MaxScore.HasValue && NumberOfDecimal.HasValue)
                    {                        
                         maxScoreString = MaxScore.Value.ToString();                        
                    }
                    if (ListText != null && ListText.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(FormatOption) && FormatOption.ToLower() == "labelvaluetext")
                        {
                            var options =
                            new JavaScriptSerializer().Deserialize<List<VirtualTestCustomMetaSelectListOptionModel>>
                                (ListTextString);
                            var value = options.Select(x => string.Format("{0} ({1})", x.Label, x.Option)).ToList();
                            overview = string.Format("- List value: {0} </br>", string.Join(", ", value));
                            if (DisplayOption == "label")
                            {
                                overview += "- Display Labels Only";
                            }
                            if (DisplayOption == "value")
                            {
                                overview += "- Display Value Only";
                            }
                            if (DisplayOption == "both")
                            {
                                overview += "- Display Both Label and Value";
                            }
                        }
                        else
                        {
                            overview = string.Format("- List value: {0}", ListTextString);
                        }
                    }
                    else
                    {
                        overview = string.Format("- Min Score:{0}</br> - Max Score:{1} ...",
                            minScoreString,
                            maxScoreString);
                    }
                }
                if (ScoreTypeCode == ScoreTypeModel.TEXT_CUSTOM_SCORE)
                {
                    if (IsFreeText.HasValue && IsFreeText.Value)
                    {
                        overview =
                            string.Format(
                                "- Free text </br> - Max number of characters allowed:{0} ...",
                                MaxAllowedCharacterNumber.HasValue ? MaxAllowedCharacterNumber.Value.ToString() : "NA"
                                );
                    }
                    if (IsListText.HasValue && IsListText.Value)
                    {
                        overview =
                            string.Format("- List text: {0}", ListTextString);
                    }
                }
                return overview;
            }
        }
        public string DataHostPot { get; set; }
        public string NoteDefaultValue { get; set; }
        public string NoteType { get; set; }
        public bool HasAssociatedAutoSave { get; set; } 
        public bool HasImDataPointAssigned { get; set; }
        public bool Disabled {
            get {
                return HasImDataPointAssigned;
            }
        }
        public List<VirtualTestCustomMetaNoteCommentModel> ListNoteComment { get; set; }

        public List<AssessmentArtifactFileTypeGroupEditScoreModel> AssessmentArtifactFileTypeGroupViewModel { get; set; }
    }
    public class AssessmentArtifactFileTypeGroupEditScoreModel
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public List<string> SupportFileType { get; set; }
    }

    public class UpdateOrderScoreTypeModel
    {
        public string ScoreType { get; set; }
        public string Name { get; set; }
        public int? Order { get; set; }
    }

    public class OrderScoreTypeUpdateModel
    {
        public int ScoreId { get; set; }
        public int? SubScoreId { get; set; }
        public string ScoreType { get; set; }
        public string Name { get; set; }
        public int? RawIndex { get; set; } //for drag, drop and clone
    }
}
