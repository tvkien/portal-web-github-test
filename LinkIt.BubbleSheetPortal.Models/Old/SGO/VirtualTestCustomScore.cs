using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using System.Web.Script.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class VirtualTestCustomScore
    {
        public VirtualTestCustomScore()
        {
            HasAssociatedTest = false;
            HasAssociatedTestResult = false;
            HasAssociatedAutoSave = false;
        }

        public int VirtualTestCustomScoreId { get; set; }
        public string Name { get; set; }
        public int DistrictId { get; set; }
        public bool UsePercent { get; set; }
        public bool UsePercentile { get; set; }
        public bool UseRaw { get; set; }
        public bool UseScaled { get; set; }
        public bool UseAchievementLevel { get; set; }
        public int? PointsPossible { get; set; }
        public int? ScaledScoreMin { get; set; }
        public int? ScaledScoreMax { get; set; }
        public bool? UseCustomN1 { get; set; }
        public bool? UseCustomN2 { get; set; }
        public bool? UseCustomN3 { get; set; }
        public bool? UseCustomN4 { get; set; }
        public bool? UseCustomA1 { get; set; }
        public bool? UseCustomA2 { get; set; }
        public bool? UseCustomA3 { get; set; }
        public bool? UseCustomA4 { get; set; }
        public string CustomN1Label { get; set; }
        public string CustomN2Label { get; set; }
        public string CustomN3Label { get; set; }
        public string CustomN4Label { get; set; }
        public string CustomA1Label { get; set; }
        public string CustomA2Label { get; set; }
        public string CustomA3Label { get; set; }
        public string CustomA4Label { get; set; }
        public string CustomA1ValueList { get; set; }
        public string CustomA2ValueList { get; set; }
        public string Label { get; set; }
        public string BankLabel { get; set; }
        public int AchievementLevelSettingId { get; set; }
        public int VirtualTestSourceId { get; set; }
        public int VirtualTestTypeId { get; set; }
        public int TestScoreMethodId { get; set; }
        public int VirtualTestSubTypeId { get; set; }
        public bool? UseArtifact { get; set; }
        public bool? UseNote { get; set; }

        public bool? IsMultiDate { get; set; }

        public int? AuthorUserID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? DataSetOriginID { get; set; }
        public int? DatasetCategoryID { get; set; }

        public bool? UseGradeLevelEquiv { get; set; }
        public bool Archived { get; set; }

        public int? CustomA_1_ConversionSetID { get; set; }
        public int? CustomA_2_ConversionSetID { get; set; }
        public int? CustomA_3_ConversionSetID { get; set; }
        public int? CustomA_4_ConversionSetID { get; set; }

        public int? CustomN_1_ConversionSetID { get; set; }
        public int? CustomN_2_ConversionSetID { get; set; }
        public int? CustomN_3_ConversionSetID { get; set; }
        public int? CustomN_4_ConversionSetID { get; set; }

        public List<ScoreTypeModel> ScoreTypeList
        {
            get
            {
                var scoreTypeList = new List<ScoreTypeModel>();
                if (UseRaw)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.RAW_SCORE,
                        ScoreName = "Raw"
                    });
                }
                if (UseScaled)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.SCALED_SCORE,
                        ScoreName = "Scaled"
                    });
                }
                if (UsePercent)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.PERCENT_SCORE,
                        ScoreName = "Percent"
                    });
                }
                if (UsePercentile)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.PERCENTILE_SCORE,
                        ScoreName = "Percentile"
                    });
                }
                if (UseCustomN1.HasValue && UseCustomN1.Value)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.NUMERIC_CUSTOM_SCORE,
                        CustomScoreName = CustomN1Label
                    });
                }
                if (UseCustomN2.HasValue && UseCustomN2.Value)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.NUMERIC_CUSTOM_SCORE,
                        CustomScoreName = CustomN2Label
                    });
                }
                if (UseCustomN3.HasValue && UseCustomN3.Value)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.NUMERIC_CUSTOM_SCORE,
                        CustomScoreName = CustomN3Label
                    });
                }
                if (UseCustomN4.HasValue && UseCustomN4.Value)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.NUMERIC_CUSTOM_SCORE,
                        CustomScoreName = CustomN4Label
                    });
                }
                if (UseCustomA1.HasValue && UseCustomA1.Value)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.TEXT_CUSTOM_SCORE,
                        CustomScoreName = CustomA1Label
                    });
                }
                if (UseCustomA2.HasValue && UseCustomA2.Value)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.TEXT_CUSTOM_SCORE,
                        CustomScoreName = CustomA2Label
                    });
                }
                if (UseCustomA3.HasValue && UseCustomA3.Value)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.TEXT_CUSTOM_SCORE,
                        CustomScoreName = CustomA3Label
                    });
                }
                if (UseCustomA4.HasValue && UseCustomA4.Value)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.TEXT_CUSTOM_SCORE,
                        CustomScoreName = CustomA4Label
                    });
                }
                if (UseArtifact.HasValue && UseArtifact.Value)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.ARTIFACT_SCORE,
                        ScoreName = "Artifact"
                    });
                }
                if (UseNote.HasValue && UseNote.Value)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.NOTE_COMMENT,
                        ScoreName = "Notes/Comments",
                        CustomScoreName = "Notes/Comments"
                    });
                }
                return scoreTypeList;
            }
        }

        public ScoreTypeModel RawScore
        {
            get
            {
                if (UseRaw == true)
                {
                    return new ScoreTypeModel()
                    {
                        TemplateID = VirtualTestCustomScoreId,
                        ScoreTypeCode = ScoreTypeModel.RAW_SCORE,
                        ScoreName = "Raw"
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsTemplateScoreTypeExisting(ScoreTypeModel scoreType)
        {
            return
                ScoreTypeList.Any(
                    x =>
                        (x.ScoreName != null && x.ScoreName.ToLower().Equals(scoreType.ScoreNameAbsolute.ToLower())) ||
                         (x.CustomScoreName != null &&
                          x.CustomScoreName.ToLower().Equals(scoreType.ScoreNameAbsolute.ToLower())));
        }

        public VirtualTestCustomMetaData AddScoreTypeToTemplate(ScoreTypeModel scoreType)
        {
            if (scoreType.SubscoreId.HasValue && scoreType.SubscoreId.Value > 0)
            {
                throw new Exception(string.Format("{0} is belong to a subscore, unable to add it to a Template.", scoreType.ScoreNameAbsolute));
            }
            var meta = new VirtualTestCustomMetaData()
            {
                VirtualTestCustomScoreID = VirtualTestCustomScoreId
            };
            if (!IsTemplateScoreTypeExisting(scoreType))
            {
                if (scoreType.IsCustomScoreType)
                {
                    //CustomN_1, CustomN_2, CustomN_3, CustomN_4, CustomA_1, CustomA_2, CustomA_3, CustomA_4
                    if (scoreType.ScoreTypeCode == ScoreTypeModel.NUMERIC_CUSTOM_SCORE)
                    {
                        if (!UseCustomN1.HasValue || UseCustomN1.Value == false)
                        {
                            UseCustomN1 = true;
                            CustomN1Label = scoreType.CustomScoreName;
                            meta.ScoreType = VirtualTestCustomMetaData.CustomN_1;
                            CustomN_1_ConversionSetID = CustomA_1_ConversionSetID;
                        }
                        else
                        {
                            if (!UseCustomN2.HasValue || UseCustomN2.Value == false)
                            {
                                UseCustomN2 = true;
                                CustomN2Label = scoreType.CustomScoreName;
                                meta.ScoreType = VirtualTestCustomMetaData.CustomN_2;
                                CustomN_2_ConversionSetID = CustomN_1_ConversionSetID;
                            }
                            else
                            {
                                if (!UseCustomN3.HasValue || UseCustomN3.Value == false)
                                {
                                    UseCustomN3 = true;
                                    CustomN3Label = scoreType.CustomScoreName;
                                    meta.ScoreType = VirtualTestCustomMetaData.CustomN_3;
                                    CustomN_3_ConversionSetID = CustomN_1_ConversionSetID;
                                }
                                else
                                {
                                    if (!UseCustomN4.HasValue || UseCustomN4.Value == false)
                                    {
                                        UseCustomN4 = true;
                                        CustomN4Label = scoreType.CustomScoreName;
                                        meta.ScoreType = VirtualTestCustomMetaData.CustomN_4;
                                        CustomN_4_ConversionSetID = CustomN_1_ConversionSetID;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //text
                        if (!UseCustomA1.HasValue || UseCustomA1.Value == false)
                        {
                            UseCustomA1 = true;
                            CustomA1Label = scoreType.CustomScoreName;
                            meta.ScoreType = VirtualTestCustomMetaData.CustomA_1;
                            CustomA_1_ConversionSetID = CustomN_1_ConversionSetID;
                        }
                        else
                        {
                            if (!UseCustomA2.HasValue || UseCustomA2.Value == false)
                            {
                                UseCustomA2 = true;
                                CustomA2Label = scoreType.CustomScoreName;
                                meta.ScoreType = VirtualTestCustomMetaData.CustomA_2;
                                CustomA_2_ConversionSetID = CustomA_1_ConversionSetID;
                            }
                            else
                            {
                                if (!UseCustomA3.HasValue || UseCustomA3.Value == false)
                                {
                                    UseCustomA3 = true;
                                    CustomA3Label = scoreType.CustomScoreName;
                                    meta.ScoreType = VirtualTestCustomMetaData.CustomA_3;
                                    CustomA_3_ConversionSetID = CustomA_1_ConversionSetID;
                                }
                                else
                                {
                                    if (!UseCustomA4.HasValue || UseCustomA4.Value == false)
                                    {
                                        UseCustomA4 = true;
                                        CustomA4Label = scoreType.CustomScoreName;
                                        meta.ScoreType = VirtualTestCustomMetaData.CustomA_4;
                                        CustomA_4_ConversionSetID = CustomA_1_ConversionSetID;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Raw, Scaled, Percent, Percentile
                    if (scoreType.ScoreTypeCode == ScoreTypeModel.RAW_SCORE)
                    {
                        UseRaw = true;
                        meta.ScoreType = VirtualTestCustomMetaData.Raw;
                    }
                    else if (scoreType.ScoreTypeCode == ScoreTypeModel.SCALED_SCORE)
                    {
                        UseScaled = true;
                        meta.ScoreType = VirtualTestCustomMetaData.Scaled;
                    }
                    else if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENT_SCORE)
                    {
                        UsePercent = true;
                        meta.ScoreType = VirtualTestCustomMetaData.Percent;
                    }
                    else if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENTILE_SCORE)
                    {
                        UsePercentile = true;
                        meta.ScoreType = VirtualTestCustomMetaData.Percentile;
                    }
                    else if (scoreType.ScoreTypeCode == ScoreTypeModel.ARTIFACT_SCORE)
                    {
                        UseArtifact = true;
                        meta.ScoreType = VirtualTestCustomMetaData.Artifact;
                    }
                }
                meta.MetaData = scoreType.Meta.GetJsonString();
                return meta;
            }
            else
            {
                throw new Exception(string.Format("{0} is existing.", scoreType.ScoreNameAbsolute));
            }
        }

        /// <summary>
        /// NOTE_COMMENTS
        /// </summary>
        /// <param name="scoreType"></param>
        /// <returns></returns>
        public VirtualTestCustomMetaData AddScoreTypeNoteToTemplate(ScoreTypeModel scoreType, VirtualTestCustomMetaData noteMetaData, int lastedOrder)
        {
            VirtualTestCustomMetaModel metaModel = new VirtualTestCustomMetaModel();
            metaModel.ListNoteComment = new List<VirtualTestCustomMetaNoteCommentModel>();
            if (noteMetaData == null)
            {
                noteMetaData = new VirtualTestCustomMetaData() { VirtualTestCustomScoreID = VirtualTestCustomScoreId };
            }
            else
            {
                //metaModel = new JavaScriptSerializer().Deserialize<VirtualTestCustomMetaModel>(noteMetaData.MetaData);
                metaModel = noteMetaData.ParseMetaToObject();
                if (metaModel.ListNoteComment == null)
                {
                    metaModel.ListNoteComment = new List<VirtualTestCustomMetaNoteCommentModel>();
                }
                else
                {
                    var lastedOrderNote = metaModel.ListNoteComment.OrderByDescending(x => x.Order).FirstOrDefault()?.Order ?? 0;
                    if (lastedOrder < lastedOrderNote)
                    {
                        lastedOrder = lastedOrderNote;
                    }
                }
            }

            metaModel.ListNoteComment.Add(new VirtualTestCustomMetaNoteCommentModel()
            {
                DefaultValue = scoreType.NoteDefaultValue,
                Description = scoreType.Description,
                NoteName = scoreType.CustomScoreName,
                NoteType = scoreType.NoteType,
                Order = lastedOrder + 1
            });

            this.UseNote = true;
            noteMetaData.ScoreType = VirtualTestCustomMetaData.NOTE_COMMENT;
            noteMetaData.MetaData = metaModel.GetJsonString();

            return noteMetaData;
        }

        public List<int> SubscoreIdList = new List<int>();

        public void DeleteScoreTypeFromTemplate(string scoreTypeName)
        {
            //Opposite with AddScoreTypeToSubScore
            var scoreTypeList = this.ScoreTypeList;
            foreach (var scoreType in scoreTypeList)
            {
                if (scoreType.ScoreNameAbsolute.ToLower().Equals(scoreTypeName.ToLower()))
                {
                    if (scoreType.IsCustomScoreType)
                    {
                        if (scoreType.IsNumeric)
                        {
                            if (!string.IsNullOrEmpty(CustomN1Label) &&
                                CustomN1Label.ToLower().Equals(scoreTypeName.ToLower()))
                            {
                                UseCustomN1 = false;
                                CustomN1Label = null;
                            }
                            if (!string.IsNullOrEmpty(CustomN2Label) &&
                             CustomN2Label.ToLower().Equals(scoreTypeName.ToLower()))
                            {
                                UseCustomN2 = false;
                                CustomN2Label = null;
                            }
                            if (!string.IsNullOrEmpty(CustomN3Label) &&
                             CustomN3Label.ToLower().Equals(scoreTypeName.ToLower()))
                            {
                                UseCustomN3 = false;
                                CustomN3Label = null;
                            }
                            if (!string.IsNullOrEmpty(CustomN4Label) &&
                             CustomN4Label.ToLower().Equals(scoreTypeName.ToLower()))
                            {
                                UseCustomN4 = false;
                                CustomN4Label = null;
                            }
                            RearrangeNumericCustomScoreType();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(CustomA1Label) &&
                                CustomA1Label.ToLower().Equals(scoreTypeName.ToLower()))
                            {
                                UseCustomA1 = false;
                                CustomA1Label = null;
                            }
                            if (!string.IsNullOrEmpty(CustomA2Label) &&
                             CustomA2Label.ToLower().Equals(scoreTypeName.ToLower()))
                            {
                                UseCustomA2 = false;
                                CustomA2Label = null;
                            }
                            if (!string.IsNullOrEmpty(CustomA3Label) &&
                             CustomA3Label.ToLower().Equals(scoreTypeName.ToLower()))
                            {
                                UseCustomA3 = false;
                                CustomA3Label = null;
                            }
                            if (!string.IsNullOrEmpty(CustomA4Label) &&
                             CustomA4Label.ToLower().Equals(scoreTypeName.ToLower()))
                            {
                                UseCustomA4 = false;
                                CustomA4Label = null;
                            }
                            RearrangeTextCustomScoreType();
                        }
                    }
                    else
                    {
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.RAW_SCORE)
                        {
                            this.UseRaw = false;
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.SCALED_SCORE)
                        {
                            this.UseScaled = false;
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENT_SCORE)
                        {
                            this.UsePercent = false;
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENTILE_SCORE)
                        {
                            this.UsePercentile = false;
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.ARTIFACT_SCORE)
                        {
                            this.UseArtifact = false;
                        }
                    }
                }
            }
        }

        public void RearrangeNumericCustomScoreType()
        {
            var conversionSetId = CustomA_1_ConversionSetID ?? CustomA_2_ConversionSetID ?? CustomA_3_ConversionSetID ?? CustomA_4_ConversionSetID
                ?? CustomN_1_ConversionSetID ?? CustomN_2_ConversionSetID ?? CustomN_3_ConversionSetID ?? CustomN_4_ConversionSetID;

            var scoreTypeNameList = new List<string>();
            if (!string.IsNullOrEmpty(CustomN1Label))
            {
                scoreTypeNameList.Add(CustomN1Label);
            }
            if (!string.IsNullOrEmpty(CustomN2Label))
            {
                scoreTypeNameList.Add(CustomN2Label);
            }
            if (!string.IsNullOrEmpty(CustomN3Label))
            {
                scoreTypeNameList.Add(CustomN3Label);
            }
            if (!string.IsNullOrEmpty(CustomN4Label))
            {
                scoreTypeNameList.Add(CustomN4Label);
            }

            UseCustomN1 = UseCustomN2 = UseCustomN3 = UseCustomN4 = null;
            CustomN1Label = CustomN2Label = CustomN3Label = CustomN4Label = null;
            CustomN_1_ConversionSetID = CustomN_2_ConversionSetID = CustomN_3_ConversionSetID = CustomN_4_ConversionSetID = null;

            for (var i = 0; i < scoreTypeNameList.Count; i++)
            {
                if (i == 0)
                {
                    UseCustomN1 = true;
                    CustomN1Label = scoreTypeNameList[i];
                    CustomN_1_ConversionSetID = conversionSetId;
                }
                if (i == 1)
                {
                    UseCustomN2 = true;
                    CustomN2Label = scoreTypeNameList[i];
                    CustomN_2_ConversionSetID = conversionSetId;
                }
                if (i == 2)
                {
                    UseCustomN3 = true;
                    CustomN3Label = scoreTypeNameList[i];
                    CustomN_3_ConversionSetID = conversionSetId;
                }
                if (i == 3)//maximum index = 3
                {
                    UseCustomN4 = true;
                    CustomN4Label = scoreTypeNameList[i];
                    CustomN_4_ConversionSetID = conversionSetId;
                }
            }
        }

        public void RearrangeTextCustomScoreType()
        {
            var conversionSetId = CustomA_1_ConversionSetID ?? CustomA_2_ConversionSetID ?? CustomA_3_ConversionSetID ?? CustomA_4_ConversionSetID
                ?? CustomN_1_ConversionSetID ?? CustomN_2_ConversionSetID ?? CustomN_3_ConversionSetID ?? CustomN_4_ConversionSetID;

            var scoreTypeNameList = new List<string>();
            if (!string.IsNullOrEmpty(CustomA1Label))
            {
                scoreTypeNameList.Add(CustomA1Label);
            }
            if (!string.IsNullOrEmpty(CustomA2Label))
            {
                scoreTypeNameList.Add(CustomA2Label);
            }
            if (!string.IsNullOrEmpty(CustomA3Label))
            {
                scoreTypeNameList.Add(CustomA3Label);
            }
            if (!string.IsNullOrEmpty(CustomA4Label))
            {
                scoreTypeNameList.Add(CustomA4Label);
            }

            UseCustomA1 = UseCustomA2 = UseCustomA3 = UseCustomA4 = null;
            CustomA1Label = CustomA2Label = CustomA3Label = CustomA4Label = null;
            CustomA_1_ConversionSetID = CustomA_2_ConversionSetID = CustomA_3_ConversionSetID = CustomA_4_ConversionSetID = null;

            for (var i = 0; i < scoreTypeNameList.Count; i++)
            {
                if (i == 0)
                {
                    UseCustomA1 = true;
                    CustomA1Label = scoreTypeNameList[i];
                    CustomA_1_ConversionSetID = conversionSetId;
                }
                if (i == 1)
                {
                    UseCustomA2 = true;
                    CustomA2Label = scoreTypeNameList[i];
                    CustomA_2_ConversionSetID = conversionSetId;
                }
                if (i == 2)
                {
                    UseCustomA3 = true;
                    CustomA3Label = scoreTypeNameList[i];
                    CustomA_3_ConversionSetID = conversionSetId;
                }
                if (i == 3)//maximum index = 3
                {
                    UseCustomA4 = true;
                    CustomA4Label = scoreTypeNameList[i];
                    CustomA_4_ConversionSetID = conversionSetId;
                }
            }
        }

        public bool HasAssociatedTest { get; set; }
        public bool HasAssociatedTestResult { get; set; }
        public bool HasAssociatedAutoSave { get; set; }
        public bool UseIndex { get; set; }
        public bool UseLexile { get; set; }
        public bool HasImDataPointAssigned { get; set; }
        public bool HasConversionSet { get; set; }
        public bool Disabled
        {
            get
            {
                return HasImDataPointAssigned;
            }
        }
        public string CheckScoreTypeBeforeAdding(ScoreTypeModel newScoreType)
        {
            if (newScoreType.IsCustomScoreType)
            {
                if (newScoreType.IsNumeric)
                {
                    //There are only maximum 4 numeric custom score
                    if (UseCustomN1.HasValue && UseCustomN1.Value
                        && UseCustomN2.HasValue && UseCustomN2.Value
                        && UseCustomN3.HasValue && UseCustomN3.Value
                        && UseCustomN4.HasValue && UseCustomN4.Value)
                    {
                        return "You have created the maximum number of numeric custom score allowed (4). If you need to create more scores, you can set them up as new subscores.";
                    }
                }
                else
                {
                    if (UseCustomA1.HasValue && UseCustomA1.Value
                        && UseCustomA2.HasValue && UseCustomA2.Value
                        && UseCustomA3.HasValue && UseCustomA3.Value
                        && UseCustomA4.HasValue && UseCustomA4.Value)
                    {
                        return "You have created the maximum number of text custom score allowed (4). If you need to create more scores, you can set them up as new subscores.";
                    }
                }
            }

            return null;
        }
    }
}
