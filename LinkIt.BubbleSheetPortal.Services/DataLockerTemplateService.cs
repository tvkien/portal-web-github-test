using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Common;
using System.CodeDom;
using System.Collections;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DataLockerTemplateService
    {
        private readonly IRepository<VirtualTestCustomScore> _virtualTestCustomScoreRepository;
        private readonly IResultEntryTemplateRepository _resultEntryTemplateRepository;
        private readonly IRepository<VirtualTestCustomSubScore> _virtualTestCustomSubScoreRepository;
        private readonly VirtualTestCustomMetaDataService _virtualTestCustomMetaDataService;
        private readonly IRepository<VirtualTestVirtualTestCustomScore> _virtualTestCustomScoreSettingRepository;
        private readonly IDataLockerRepository _dataLockerRepository;
        private readonly IReadOnlyRepository<District> _districtRepository;
        private readonly IReadOnlyRepository<DistrictDecode> _districtDecodeRepository;

        public DataLockerTemplateService(IRepository<VirtualTestCustomScore> virtualTestCustomScoreRepository,
            IResultEntryTemplateRepository resultEntryTemplateRepository,
            IRepository<VirtualTestVirtualTestCustomScore> virtualTestCustomScoreSettingRepository,
            IRepository<VirtualTestCustomSubScore> virtualTestCustomSubScoreRepository,
            VirtualTestCustomMetaDataService virtualTestCustomMetaDataService,
            IDataLockerRepository dataLockerRepository,
            IReadOnlyRepository<District> districtRepository,
            IReadOnlyRepository<DistrictDecode> districtDecodeRepository)
        {
            _virtualTestCustomScoreRepository = virtualTestCustomScoreRepository;
            _resultEntryTemplateRepository = resultEntryTemplateRepository;
            _virtualTestCustomSubScoreRepository = virtualTestCustomSubScoreRepository;
            _virtualTestCustomMetaDataService = virtualTestCustomMetaDataService;
            _virtualTestCustomScoreSettingRepository = virtualTestCustomScoreSettingRepository;
            _dataLockerRepository = dataLockerRepository;
            _districtRepository = districtRepository;
            _districtDecodeRepository = districtDecodeRepository;
        }

        public List<int> GetPublishedDistrictIdsForTemplateId(int templateId)
        {
            return _resultEntryTemplateRepository.GetPublishedDistrictIdsForTemplate(templateId);
        }
        public IQueryable<ResultEntryTemplateModel> GetTemplate(int userId, int roleId, int districtId, bool archived)
        {
            var useMultiDate = false;
            var districtDecode = _districtDecodeRepository.Select().Where(x => x.DistrictID == districtId && x.Label.Equals(Constanst.UseMultiDateTemplate)).FirstOrDefault();
            if (districtDecode != null && !string.IsNullOrEmpty(districtDecode.Value))
                bool.TryParse(districtDecode.Value, out useMultiDate);

            var result = _resultEntryTemplateRepository.GetTemplates(userId, roleId, districtId, archived);
            if (!useMultiDate)
                return result.Where(x => !x.IsMultiDate);

            return result;
        }

        public void PublicTemplateToDistrict(int templateId, int districtId, int userCreated)
        {
            PublishTemplateDistrictModel publishTemplateDistrict = new PublishTemplateDistrictModel();
            publishTemplateDistrict.VirtualTestCustomScoreID = templateId;
            publishTemplateDistrict.DistrictId = districtId;
            publishTemplateDistrict.CreatedDate = DateTime.Now;
            publishTemplateDistrict.CreatedUserId = userCreated;

            _resultEntryTemplateRepository.AddVirtualTestCustomScoreDistrictShares(publishTemplateDistrict);
        }

        public bool TemplateScoreTypeIsExisting(int templateID, ScoreTypeModel model)
        {
            var customScore = _virtualTestCustomScoreRepository.Select().FirstOrDefault(x => x.VirtualTestCustomScoreId == templateID);

            if (model.ScoreTypeCode == ScoreTypeModel.NOTE_COMMENT && customScore.UseNote.HasValue && customScore.UseNote.Value == true)
            {
                var note = _virtualTestCustomMetaDataService.Select().FirstOrDefault(x => x.VirtualTestCustomScoreID == templateID
                                        && x.VirtualTestCustomSubScoreID.HasValue == false
                                        && x.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                if (note != null)
                {
                    //   VirtualTestCustomMetaModel metaModel2 = new VirtualTestCustomMetaModel();
                    // note.MetaData
                    var metaModel = note.ParseMetaToObject();
                    //var metaModel = new JavaScriptSerializer().Deserialize<VirtualTestCustomMetaModel>(note.MetaData);
                    if (metaModel.ListNoteComment != null && metaModel.ListNoteComment.Any(x => x.NoteName.Equals(model.CustomScoreName)))
                    {
                        return true;
                    }
                }

                return false;
            }
            return customScore.IsTemplateScoreTypeExisting(model);
        }

        public bool SubscoreScoreTypeIsExisting(int virtualTestCustomSubScoreId, ScoreTypeModel model)
        {
            var subscore = _virtualTestCustomSubScoreRepository
                                        .Select()
                                        .FirstOrDefault(x => x.VirtualTestCustomSubScoreId == virtualTestCustomSubScoreId);

            if (model.ScoreTypeCode == ScoreTypeModel.NOTE_COMMENT && subscore.UseNote.HasValue && subscore.UseNote.Value == true)
            {
                var note = _virtualTestCustomMetaDataService.Select().FirstOrDefault(x => x.VirtualTestCustomSubScoreID == virtualTestCustomSubScoreId && x.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                if (note != null)
                {
                    var metaModel = note.ParseMetaToObject();
                    if (metaModel.ListNoteComment != null && metaModel.ListNoteComment.Any(x => x.NoteName.Equals(model.CustomScoreName)))
                    {
                        return true;
                    }
                }

                return false;
            }

            return subscore.IsSubscoreScoreTypeExisting(model);
        }

        public IQueryable<int> GetPublishedTemplateIdsFromDistrict(int districtId)
        {
            return _resultEntryTemplateRepository.GetVirtualTestCustomScoreDistrictShareByDistrictId(districtId).Select(vt => vt.VirtualTestCustomScoreID);
        }

        public List<District> GetUnPublishedDistrict(int stateId, int templateId)
        {
            var publishedDistricts = _resultEntryTemplateRepository.GetPublishedDistrictsForTemplate(templateId);
            var districts = _districtRepository.Select().Where(x => x.StateId.Equals(stateId)).ToList();

            var unPublishedDistricts = districts.Where(d => !publishedDistricts.Any(pd => pd.Id == d.Id)).OrderBy(x => x.Name).ToList();
            return unPublishedDistricts;
        }

        public IQueryable<PublishTemplateDistrictModel> GetTemplateDistricts(int templateId)
        {
            var templateDistricts = _resultEntryTemplateRepository.GetVirtualTestCustomScoreDistrictShare(templateId);
            return templateDistricts;
        }

        public PublishTemplateDistrictModel GetTemplateDistrictById(int templateDistrictId)
        {
            var templateDistrict = _resultEntryTemplateRepository.GetVirtualTestCustomScoreDistrictShareById(templateDistrictId);
            return templateDistrict;
        }

        public void DepublicTemplateDistrictById(PublishTemplateDistrictModel templateDistrict)
        {
            _resultEntryTemplateRepository.DeleteVirtualTestCustomScoreDistrictShare(templateDistrict);
        }

        public string CreateNewScoreType(int userId, ScoreTypeModel model)
        {
            Exception methodException = null;//support manually transaction
            //using (var scope = new TransactionScope())
            {
                var allScores = _virtualTestCustomMetaDataService.Select()
                        .Where(x => x.VirtualTestCustomScoreID == model.TemplateID);
                //Update VirtualTestCustomScore first
                var customScore = _virtualTestCustomScoreRepository.Select().FirstOrDefault(x => x.VirtualTestCustomScoreId == model.TemplateID);

                VirtualTestCustomMetaData metaEntity = null;
                if (model.ScoreTypeCode == ScoreTypeModel.NOTE_COMMENT)
                {
                    var lastedOrder = allScores.OrderByDescending(o => o.Order).FirstOrDefault()?.Order ?? 0;
                    var noteMetaData = _virtualTestCustomMetaDataService.Select().FirstOrDefault(x => x.VirtualTestCustomScoreID == model.TemplateID
                                                            && x.VirtualTestCustomSubScoreID.HasValue == false
                                                            && x.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                    metaEntity = customScore.AddScoreTypeNoteToTemplate(model, noteMetaData, lastedOrder);
                }
                else
                {
                    metaEntity = customScore.AddScoreTypeToTemplate(model);//update some fields of VirtualTestCustomScore
                }

                _virtualTestCustomMetaDataService.Save(metaEntity);

                if (metaEntity.VirtualTestCustomMetaDataID > 0)
                {
                    //Unable to use transaction scope so there's one way to manage manually
                    try
                    {
                        _virtualTestCustomScoreRepository.Save(customScore);
                    }
                    catch (Exception ex)
                    {
                        methodException = ex;
                        //try to delete the inserted metaEntity
                        try
                        {
                            _virtualTestCustomMetaDataService.Delete(metaEntity.VirtualTestCustomMetaDataID);
                        }
                        catch (Exception exDeleteMeta)
                        {
                            methodException = exDeleteMeta;
                        }
                    }
                    finally
                    {
                        if (methodException != null)
                        {
                            throw new Exception("There was exception why adding new score type.", methodException);
                        }
                    }
                }

                // scope.Complete();
                return metaEntity.ScoreType;
            }
        }

        public List<ScoreTypeModel> GetScoreTypesOfTemplate(int templateId)
        {
            var customScore =
                _virtualTestCustomScoreRepository.Select().FirstOrDefault(x => x.VirtualTestCustomScoreId == templateId);
            if (customScore != null)
            {
                var allMetaDatas = _virtualTestCustomMetaDataService.GetAllMetaDataOfTemplate(templateId);
                var templateScoreTypeMetaData = allMetaDatas.Where(w => w.VirtualTestCustomSubScoreID == null).ToList();
                var scoreTypeList = customScore.ScoreTypeList;
                foreach (var scoreType in scoreTypeList)
                {
                    VirtualTestCustomMetaData meta = null;
                    if (scoreType.IsCustomScoreType)
                    {
                        if (scoreType.IsNumeric)
                        {
                            if (customScore.UseCustomN1 != null && customScore.UseCustomN1.Value
                                && customScore.CustomN1Label != null &&
                                customScore.CustomN1Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    templateScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomN_1);
                                scoreType.UseCustomN1 = true;
                            }
                            if (customScore.UseCustomN2 != null && customScore.UseCustomN2.Value
                                && customScore.CustomN2Label != null &&
                                customScore.CustomN2Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    templateScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomN_2);
                                scoreType.UseCustomN2 = true;
                            }
                            if (customScore.UseCustomN3 != null && customScore.UseCustomN3.Value
                                && customScore.CustomN3Label != null &&
                                customScore.CustomN3Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    templateScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomN_3);
                                scoreType.UseCustomN3 = true;
                            }
                            if (customScore.UseCustomN4 != null && customScore.UseCustomN4.Value
                                && customScore.CustomN4Label != null &&
                                customScore.CustomN4Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    templateScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomN_4);
                                scoreType.UseCustomN4 = true;
                            }
                        }
                        else
                        {
                            if (customScore.UseCustomA1 != null && customScore.UseCustomA1.Value
                                && customScore.CustomA1Label != null &&
                               customScore.CustomA1Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    templateScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomA_1);
                                scoreType.UseCustomA1 = true;
                            }

                            if (customScore.UseCustomA2 != null && customScore.UseCustomA2.Value
                                && customScore.CustomA2Label != null &&
                               customScore.CustomA2Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    templateScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomA_2);
                                scoreType.UseCustomA2 = true;
                            }
                            if (customScore.UseCustomA3 != null && customScore.UseCustomA3.Value
                                && customScore.CustomA3Label != null &&
                               customScore.CustomA3Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    templateScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomA_3);
                                scoreType.UseCustomA3 = true;
                            }
                            if (customScore.UseCustomA4 != null && customScore.UseCustomA4.Value
                                && customScore.CustomA4Label != null &&
                               customScore.CustomA4Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    templateScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomA_4);
                                scoreType.UseCustomA4 = true;
                            }
                        }
                    }
                    else
                    {
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.RAW_SCORE)
                        {
                            meta =
                                templateScoreTypeMetaData.FirstOrDefault(
                                    x => x.ScoreType == VirtualTestCustomMetaData.Raw);
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.SCALED_SCORE)
                        {
                            meta =
                                templateScoreTypeMetaData.FirstOrDefault(
                                    x => x.ScoreType == VirtualTestCustomMetaData.Scaled);
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENT_SCORE)
                        {
                            meta =
                                templateScoreTypeMetaData.FirstOrDefault(
                                    x => x.ScoreType == VirtualTestCustomMetaData.Percent);
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENTILE_SCORE)
                        {
                            meta =
                                templateScoreTypeMetaData.FirstOrDefault(
                                    x => x.ScoreType == VirtualTestCustomMetaData.Percentile);
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.ARTIFACT_SCORE)
                        {
                            meta =
                                templateScoreTypeMetaData.FirstOrDefault(
                                    x => x.ScoreType == VirtualTestCustomMetaData.Artifact);
                        }
                    }

                    if (scoreType.IsNoteType)
                    {
                        if (customScore.UseNote.HasValue && customScore.UseNote.Value)
                        {
                            meta = templateScoreTypeMetaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.NOTE_COMMENT);
                        }
                    }

                    if (meta != null)
                    {
                        var metaObj = meta.ParseMetaToObject();
                        if (metaObj != null)
                        {
                            scoreType.Meta = metaObj;
                            scoreType.ListNoteComment = metaObj.ListNoteComment;
                            scoreType.ScoreType = meta.ScoreType;
                            scoreType.NoteType = metaObj.NoteType;
                            scoreType.MinScore = metaObj.MinValue;
                            scoreType.MaxScore = metaObj.MaxValue;
                        }
                        scoreType.DisplayOrder = meta.Order ?? 0;
                        scoreType.VirtualTestCustomMetaDataID = meta.VirtualTestCustomMetaDataID;
                    }
                }

                return scoreTypeList;
            }
            else
            {
                return new List<ScoreTypeModel>();
            }
        }

        public void SortScoreTypeList(List<ScoreTypeModel> scoreTypeList)
        {
            int order = 1;
            ScoreTypeModel scoreType = null;
            //select top 10* from VirtualTestCustomScore ( follow the order of columns UsePercent, UsePercentile, UseRaw, UseScaled, UseCustomN_1,...

            scoreType = scoreTypeList.FirstOrDefault(x => x.ScoreTypeCode == ScoreTypeModel.RAW_SCORE);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.ScoreTypeCode == ScoreTypeModel.SCALED_SCORE);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.ScoreTypeCode == ScoreTypeModel.PERCENT_SCORE);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.ScoreTypeCode == ScoreTypeModel.PERCENTILE_SCORE);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.UseCustomN1 == true);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.UseCustomN2 == true);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.UseCustomN3 == true);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.UseCustomN4 == true);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }

            scoreType = scoreTypeList.FirstOrDefault(x => x.UseCustomA1 == true);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.UseCustomA2 == true);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.UseCustomA3 == true);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.UseCustomA4 == true);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            scoreType = scoreTypeList.FirstOrDefault(x => x.UseArtifact == true);
            if (scoreType != null)
            {
                scoreType.DisplayOrder = order;
                order++;
            }
            //scoreType = scoreTypeList.FirstOrDefault(x => x.ScoreTypeCode == ScoreTypeModel.NOTE_COMMENT);
            //if (scoreType != null)
            //{
            //    scoreType.DisplayOrder = order;
            //    order++;
            //}
        }

        public ScoreTypeModel GetScoreTypeOfTemplate(int templateId, string name)
        {
            var scoreTypeList = GetScoreTypesOfTemplate(templateId);
            foreach (var scoreType in scoreTypeList)
            {
                if (scoreType.IsCustomScoreType)
                {
                    if (scoreType.CustomScoreName.ToLower().Equals(name.ToLower()))
                    {
                        return scoreType;
                    }
                }
                else
                {
                    if (scoreType.ScoreName.ToLower().Equals(name.ToLower()))
                    {
                        return scoreType;
                    }
                }
            }

            return null;
        }

        public ScoreTypeModel GetNoteScoreOfTemplate(int templateId)
        {
            var noteMetaData = _virtualTestCustomMetaDataService.GetMetaDataOfTemplate(templateId)
                .FirstOrDefault(x => x.ScoreType == ScoreTypeModel.NOTE_COMMENT);

            if (noteMetaData != null)
            {
                var noteTypeModel = new ScoreTypeModel()
                {
                    ScoreTypeCode = ScoreTypeModel.NOTE_COMMENT,
                    TemplateID = templateId
                };

                var metaObj = noteMetaData.ParseMetaToObject();
                noteTypeModel.Meta = metaObj;
                noteTypeModel.ListNoteComment = metaObj.ListNoteComment;
                return noteTypeModel;
            }

            return null;
        }

        public ScoreTypeModel GetNoteScoreOfSubScore(int templateId, int subscoreId)
        {
            var noteMetaData = _virtualTestCustomMetaDataService.GetMetaDataOfSubscore(subscoreId)
                                        .FirstOrDefault(x => x.ScoreType == ScoreTypeModel.NOTE_COMMENT);

            if (noteMetaData != null)
            {
                var noteTypeModel = new ScoreTypeModel()
                {
                    ScoreTypeCode = ScoreTypeModel.NOTE_COMMENT,
                    TemplateID = templateId,
                    SubscoreId = subscoreId
                };

                var metaObj = noteMetaData.ParseMetaToObject();
                noteTypeModel.Meta = metaObj;
                noteTypeModel.ListNoteComment = metaObj.ListNoteComment;
                return noteTypeModel;
            }

            return null;
        }

        public string UpdateNoteScoreType(int userId, string oldName, ScoreTypeModel model)
        {
            VirtualTestCustomMetaData noteMetaData = null;

            if (model.SubscoreId.HasValue && model.SubscoreId.Value > 0)
            {
                noteMetaData = _virtualTestCustomMetaDataService.GetMetaDataOfSubscore(model.SubscoreId.Value)
                                        .FirstOrDefault(x => x.ScoreType == ScoreTypeModel.NOTE_COMMENT);
            }
            else
            {
                noteMetaData = _virtualTestCustomMetaDataService.GetMetaDataOfTemplate(model.TemplateID)
                                        .FirstOrDefault(x => x.ScoreType == ScoreTypeModel.NOTE_COMMENT);
            }

            if (noteMetaData != null)
            {
                var metaObj = noteMetaData.ParseMetaToObject();
                if (oldName != model.CustomScoreName)
                {
                    bool isExistsName = metaObj.ListNoteComment.Any(x => x.NoteName == model.CustomScoreName);
                    if (isExistsName)
                        return model.GetDuplicateNameErrorMessage();
                }
                var note = metaObj.ListNoteComment.FirstOrDefault(x => x.NoteName == oldName);
                if (note != null)
                {
                    note.NoteName = model.ScoreName;
                    note.Description = model.Description;
                    note.DefaultValue = model.NoteDefaultValue;
                    note.NoteType = model.NoteType;
                    noteMetaData.MetaData = metaObj.GetJsonString();
                    _virtualTestCustomMetaDataService.Save(noteMetaData);
                }
            }

            return string.Empty;
        }

        public void UpdateScoreType(int userId, string oldName, ScoreTypeModel model)
        {
            if (model.SubscoreId.HasValue && model.SubscoreId.Value > 0)
            {
                //score type of a subscore
                var subscore =
                _virtualTestCustomSubScoreRepository.Select()
                    .FirstOrDefault(x => x.VirtualTestCustomSubScoreId == model.SubscoreId.Value);
                if (subscore != null)
                {
                    //Update some value
                    if (model.IsCustomScoreType)
                    {
                        if (model.UseCustomN1)
                        {
                            subscore.CustomN1Label = model.CustomScoreName;
                        }
                        if (model.UseCustomN2)
                        {
                            subscore.CustomN2Label = model.CustomScoreName;
                        }
                        if (model.UseCustomN3)
                        {
                            subscore.CustomN3Label = model.CustomScoreName;
                        }
                        if (model.UseCustomN4)
                        {
                            subscore.CustomN4Label = model.CustomScoreName;
                        }
                        if (model.UseCustomA1)
                        {
                            subscore.CustomA1Label = model.CustomScoreName;
                        }
                        if (model.UseCustomA2)
                        {
                            subscore.CustomA2Label = model.CustomScoreName;
                        }
                        if (model.UseCustomA3)
                        {
                            subscore.CustomA3Label = model.CustomScoreName;
                        }
                        if (model.UseCustomA4)
                        {
                            subscore.CustomA4Label = model.CustomScoreName;
                        }
                        _virtualTestCustomSubScoreRepository.Save(subscore);
                    }

                    //get meta data
                    var meta = _virtualTestCustomMetaDataService.GetMetaDataOfSubscoreScoreType(model);
                    if (meta != null)
                    {
                        //update
                        meta.MetaData = model.Meta.GetJsonString();
                        _virtualTestCustomMetaDataService.Save(meta);
                    }
                }
            }
            else
            {
                //score type of a Template

                //get custom score
                var customScore =
                    _virtualTestCustomScoreRepository.Select()
                        .FirstOrDefault(x => x.VirtualTestCustomScoreId == model.TemplateID);
                //Update some value
                if (model.IsCustomScoreType)
                {
                    if (model.UseCustomN1)
                    {
                        customScore.CustomN1Label = model.CustomScoreName;
                    }
                    if (model.UseCustomN2)
                    {
                        customScore.CustomN2Label = model.CustomScoreName;
                    }
                    if (model.UseCustomN3)
                    {
                        customScore.CustomN3Label = model.CustomScoreName;
                    }
                    if (model.UseCustomN4)
                    {
                        customScore.CustomN4Label = model.CustomScoreName;
                    }
                    if (model.UseCustomA1)
                    {
                        customScore.CustomA1Label = model.CustomScoreName;
                    }
                    if (model.UseCustomA2)
                    {
                        customScore.CustomA2Label = model.CustomScoreName;
                    }
                    if (model.UseCustomA3)
                    {
                        customScore.CustomA3Label = model.CustomScoreName;
                    }
                    if (model.UseCustomA4)
                    {
                        customScore.CustomA4Label = model.CustomScoreName;
                    }
                    _virtualTestCustomScoreRepository.Save(customScore);
                }
                //get meta data
                var meta = _virtualTestCustomMetaDataService.GetMetaDataOfTemplateScoreType(model);
                if (meta != null)
                {
                    //update
                    meta.MetaData = model.Meta.GetJsonString();
                    _virtualTestCustomMetaDataService.Save(meta);
                }
            }
        }

        private void UpdateLastModifiedOfTemplate(int userId, int templateId)
        {
        }

        public void DeleteScoreTypeNote(int userId, int templateId, string noteName, int? subscoreId)
        {
            if (subscoreId.HasValue && subscoreId.Value > 0)
            {
                DeleteSubScoreTypeNote(templateId, subscoreId.Value, noteName);
            }
            else
            {
                var noteTypeMeta = _virtualTestCustomMetaDataService.GetMetaDataOfTemplate(templateId)
                                                .FirstOrDefault(x => x.ScoreType == ScoreTypeModel.NOTE_COMMENT);

                var metaModel = noteTypeMeta.ParseMetaToObject();

                var note = metaModel.ListNoteComment.FirstOrDefault(x => x.NoteName == noteName);
                metaModel.ListNoteComment.Remove(note);

                if (metaModel.ListNoteComment.Count == 0)
                {
                    _virtualTestCustomMetaDataService.Delete(noteTypeMeta);
                    var template = _virtualTestCustomScoreRepository.Select().FirstOrDefault(x => x.VirtualTestCustomScoreId == templateId);
                    template.UseNote = false;
                    _virtualTestCustomScoreRepository.Save(template);
                }
                else
                {
                    noteTypeMeta.MetaData = metaModel.GetJsonString();
                    _virtualTestCustomMetaDataService.Save(noteTypeMeta);
                }
            }
        }

        public void DeleteSubScoreTypeNote(int templateId, int subScoreId, string noteName)
        {
            var noteTypeMeta = _virtualTestCustomMetaDataService.GetMetaDataOfSubscore(subScoreId)
                                    .FirstOrDefault(x => x.ScoreType == ScoreTypeModel.NOTE_COMMENT);

            var metaModel = noteTypeMeta.ParseMetaToObject();

            var note = metaModel.ListNoteComment.FirstOrDefault(x => x.NoteName == noteName);
            metaModel.ListNoteComment.Remove(note);

            if (metaModel.ListNoteComment.Count == 0)
            {
                _virtualTestCustomMetaDataService.Delete(noteTypeMeta);
                var subScore = _virtualTestCustomSubScoreRepository.Select().FirstOrDefault(x => x.VirtualTestCustomSubScoreId == subScoreId);
                subScore.UseNote = false;
                _virtualTestCustomSubScoreRepository.Save(subScore);
            }
            else
            {
                noteTypeMeta.MetaData = metaModel.GetJsonString();
                _virtualTestCustomMetaDataService.Save(noteTypeMeta);
            }
        }

        public void DeleteScoreType(int userId, int templateId, string scoreTypeName, int? subscoreId)
        {
            //demo on memory
            List<ScoreTypeModel> scoreTypeList;
            if (subscoreId.HasValue && subscoreId.Value > 0)
            {
                //score type of subscore
                var scoreType = GetScoreTypeOfSubscore(subscoreId.Value, scoreTypeName);
                //delete the meta

                var metaData = _virtualTestCustomMetaDataService.GetMetaDataOfSubscoreScoreType(scoreType);
                if (metaData != null)
                {
                    _virtualTestCustomMetaDataService.Delete(metaData);
                }
                //get subscore
                var subscore =
               _virtualTestCustomSubScoreRepository.Select()
                   .FirstOrDefault(x => x.VirtualTestCustomSubScoreId == subscoreId.Value);
                if (subscore != null)
                {
                    subscore.DeleteScoreTypeFromSubScore(scoreTypeName);//some fields have been updated
                    //save the subscore
                    _virtualTestCustomSubScoreRepository.Save(subscore);
                }
                if (scoreType.IsCustomScoreType)
                {
                    //Update the order of A1->A4 or N1->N4
                    var outScoreType = _virtualTestCustomMetaDataService.RearrangeScoreTypeofCustomScoreType(templateId, scoreType.IsNumeric, subscoreId);
                    var metaDataUpdate = _virtualTestCustomMetaDataService.GetMetaDataOfTemplate(templateId).Where(w => w.ScoreType == ScoreTypeColumnConstants.Raw || w.ScoreType.Contains(ScoreTypeColumnConstants.CustomN)).ToList();
                    if (metaDataUpdate != null)
                    {
                        _virtualTestCustomMetaDataService.RearrangeScoreTypeInExpression(metaDataUpdate, outScoreType);
                    }
                    var metaDataUpdateSub = _virtualTestCustomMetaDataService.GetMetaDataOfSubscore(subscoreId.Value).Where(w => w.ScoreType == ScoreTypeColumnConstants.Raw || w.ScoreType.Contains(ScoreTypeColumnConstants.CustomN)).ToList();
                    if (metaDataUpdateSub != null)
                    {
                        _virtualTestCustomMetaDataService.RearrangeScoreTypeInExpression(metaDataUpdateSub, outScoreType);
                    }
                }
            }
            else
            {
                //score type of Template
                var scoreType = GetScoreTypeOfTemplate(templateId, scoreTypeName);
                //delete the meta

                var metaData = _virtualTestCustomMetaDataService.GetMetaDataOfTemplateScoreType(scoreType);
                if (metaData != null)
                {
                    _virtualTestCustomMetaDataService.Delete(metaData);
                }
                //get subscore
                var template =
               _virtualTestCustomScoreRepository.Select()
                   .FirstOrDefault(x => x.VirtualTestCustomScoreId == templateId);
                if (template != null)
                {
                    template.DeleteScoreTypeFromTemplate(scoreTypeName);//some fields have been updated
                    //save the subscore
                    _virtualTestCustomScoreRepository.Save(template);
                }
                if (scoreType.IsCustomScoreType)
                {
                    //Update the order of A1->A4 or N1->N4
                    var outScoreType = _virtualTestCustomMetaDataService.RearrangeScoreTypeofCustomScoreType(templateId, scoreType.IsNumeric, subscoreId);
                    var metaDataUpdate = _virtualTestCustomMetaDataService.GetMetaDataOfTemplate(templateId).Where(w => w.ScoreType == ScoreTypeColumnConstants.Raw || w.ScoreType.Contains(ScoreTypeColumnConstants.CustomN)).ToList();
                    if (metaDataUpdate != null)
                    {
                        _virtualTestCustomMetaDataService.RearrangeScoreTypeInExpression(metaDataUpdate, outScoreType);
                    }
                }
            }

            UpdateLastModifiedOfTemplate(userId, templateId);
        }

        public VirtualTestCustomSubScore UpdateSubscoreName(int subscoreId, string name)
        {
            var subscore =
                _virtualTestCustomSubScoreRepository.Select()
                    .FirstOrDefault(x => x.VirtualTestCustomSubScoreId == subscoreId);
            if (subscore != null)
            {
                subscore.Name = name;
                _virtualTestCustomSubScoreRepository.Save(subscore);
            }
            return subscore;
        }

        public List<ScoreTypeModel> GetScoreTypesOfSubscore(int virtualTestCustomSubScoreId)
        {
            var subScore =
                _virtualTestCustomSubScoreRepository.Select()
                    .FirstOrDefault(x => x.VirtualTestCustomSubScoreId == virtualTestCustomSubScoreId);
            if (subScore != null)
            {
                var allMetaDatas = _virtualTestCustomMetaDataService.GetAllMetaDataOfTemplate(subScore.VirtualTestCustomScoreId);
                var subScoreTypeMetaData = allMetaDatas.Where(w => w.VirtualTestCustomSubScoreID == virtualTestCustomSubScoreId).ToList();
                var scoreTypeList = subScore.ScoreTypeList;
                foreach (var scoreType in scoreTypeList)
                {
                    VirtualTestCustomMetaData meta = null;
                    if (scoreType.IsCustomScoreType)
                    {
                        if (scoreType.IsNumeric)
                        {
                            if (subScore.UseCustomN1 != null && subScore.UseCustomN1.Value
                                && subScore.CustomN1Label != null &&
                                subScore.CustomN1Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    subScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomN_1);
                                scoreType.UseCustomN1 = true;
                            }
                            if (subScore.UseCustomN2 != null && subScore.UseCustomN2.Value
                                && subScore.CustomN2Label != null &&
                                subScore.CustomN2Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    subScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomN_2);
                                scoreType.UseCustomN2 = true;
                            }
                            if (subScore.UseCustomN3 != null && subScore.UseCustomN3.Value
                                && subScore.CustomN3Label != null &&
                                subScore.CustomN3Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    subScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomN_3);
                                scoreType.UseCustomN3 = true;
                            }
                            if (subScore.UseCustomN4 != null && subScore.UseCustomN4.Value
                                && subScore.CustomN4Label != null &&
                                subScore.CustomN4Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    subScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomN_4);
                                scoreType.UseCustomN4 = true;
                            }
                        }
                        else
                        {
                            if (subScore.UseCustomA1 != null && subScore.UseCustomA1.Value
                                && subScore.CustomA1Label != null &&
                               subScore.CustomA1Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    subScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomA_1);
                                scoreType.UseCustomA1 = true;
                            }

                            if (subScore.UseCustomA2 != null && subScore.UseCustomA2.Value
                                && subScore.CustomA2Label != null &&
                               subScore.CustomA2Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    subScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomA_2);
                                scoreType.UseCustomA2 = true;
                            }
                            if (subScore.UseCustomA3 != null && subScore.UseCustomA3.Value
                                && subScore.CustomA3Label != null &&
                               subScore.CustomA3Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    subScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomA_3);
                                scoreType.UseCustomA3 = true;
                            }
                            if (subScore.UseCustomA4 != null && subScore.UseCustomA4.Value
                                && subScore.CustomA4Label != null &&
                               subScore.CustomA4Label.ToLower().Equals(scoreType.CustomScoreName.ToLower()))
                            {
                                meta =
                                    subScoreTypeMetaData.FirstOrDefault(
                                        x => x.ScoreType == VirtualTestCustomMetaData.CustomA_4);
                                scoreType.UseCustomA4 = true;
                            }

                            //if (subScore.UseNote.HasValue && subScore.UseNote.Value)
                            //{
                            //    meta = subScoreTypeMetaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.NOTE_COMMENT);
                            //}
                        }
                    }
                    else
                    {
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.RAW_SCORE)
                        {
                            meta =
                                subScoreTypeMetaData.FirstOrDefault(
                                    x => x.ScoreType == VirtualTestCustomMetaData.Raw);
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.SCALED_SCORE)
                        {
                            meta =
                                subScoreTypeMetaData.FirstOrDefault(
                                    x => x.ScoreType == VirtualTestCustomMetaData.Scaled);
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENT_SCORE)
                        {
                            meta =
                                subScoreTypeMetaData.FirstOrDefault(
                                    x => x.ScoreType == VirtualTestCustomMetaData.Percent);
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.PERCENTILE_SCORE)
                        {
                            meta =
                                subScoreTypeMetaData.FirstOrDefault(
                                    x => x.ScoreType == VirtualTestCustomMetaData.Percentile);
                        }
                        if (scoreType.ScoreTypeCode == ScoreTypeModel.ARTIFACT_SCORE)
                        {
                            meta =
                                subScoreTypeMetaData.FirstOrDefault(
                                    x => x.ScoreType == VirtualTestCustomMetaData.Artifact);
                        }
                    }

                    if (scoreType.IsNoteType)
                    {
                        if (subScore.UseNote.HasValue && subScore.UseNote.Value)
                        {
                            meta = subScoreTypeMetaData.FirstOrDefault(x => x.ScoreType == VirtualTestCustomMetaData.NOTE_COMMENT);
                        }
                    }

                    if (meta != null)
                    {
                        var metaObj = meta.ParseMetaToObject();
                        if (metaObj != null)
                        {
                            scoreType.Meta = metaObj;
                            scoreType.ListNoteComment = metaObj.ListNoteComment;
                            scoreType.ScoreType = meta.ScoreType;
                            scoreType.NoteType = metaObj.NoteType;
                        }
                        scoreType.DisplayOrder = meta.Order ?? 0;
                        scoreType.VirtualTestCustomMetaDataID = meta.VirtualTestCustomMetaDataID;
                    }
                }

                return scoreTypeList;
            }
            else
            {
                return new List<ScoreTypeModel>();
            }
        }

        public ScoreTypeModel GetScoreTypeOfSubscore(int virtualTestCustomSubScoreId, string scoreTypeName)
        {
            var scoreTypeList = GetScoreTypesOfSubscore(virtualTestCustomSubScoreId);
            foreach (var scoreType in scoreTypeList)
            {
                if (scoreType.IsCustomScoreType)
                {
                    if (scoreType.CustomScoreName.ToLower().Equals(scoreTypeName.ToLower()))
                    {
                        return scoreType;
                    }
                }
                else
                {
                    if (scoreType.ScoreName.ToLower().Equals(scoreTypeName.ToLower()))
                    {
                        return scoreType;
                    }
                }
            }

            return null;
        }

        public void DeleteSubscore(int subscoreId)
        {
            //
            _dataLockerRepository.DTLDeleteSubscore(subscoreId);
        }

        public void DeleteTemplate(int userId, int templateId)
        {
            _dataLockerRepository.DTLDeleteTemplate(templateId);
        }

        public void ArchiveTemplate(int templateId, bool archived)
        {
            _dataLockerRepository.DTLArchiveTemplate(templateId, archived);
        }

        public string CheckScoreTypeBeforeAddingToSubscore(int userId, ScoreTypeModel newScoreType)
        {
            var subscore =
                     _virtualTestCustomSubScoreRepository.Select()
                         .FirstOrDefault(x => x.VirtualTestCustomSubScoreId == newScoreType.SubscoreId.Value);

            return subscore.CheckScoreTypeBeforeAdding(newScoreType);
        }

        public string CreateNewSubscoreScoreType(int userId, ScoreTypeModel model)
        {
            Exception methodException = null;//support manually transaction
            //using (var scope = new TransactionScope())
            {
                //Update VirtualTestCustomScore first
                var allScores = _virtualTestCustomMetaDataService.Select()
                        .Where(x => x.VirtualTestCustomSubScoreID == model.SubscoreId.Value);
                var subScore = _virtualTestCustomSubScoreRepository.Select()
                        .FirstOrDefault(x => x.VirtualTestCustomSubScoreId == model.SubscoreId.Value);

                VirtualTestCustomMetaData metaEntity = null;
                if (model.ScoreTypeCode == ScoreTypeModel.NOTE_COMMENT)
                {
                    var lastedOrder = allScores.OrderByDescending(o => o.Order).FirstOrDefault()?.Order ?? 0;
                    var noteMetaData = _virtualTestCustomMetaDataService.Select().FirstOrDefault(x => x.VirtualTestCustomScoreID == model.TemplateID && x.VirtualTestCustomSubScoreID == model.SubscoreId.Value
                                                            && x.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                    metaEntity = subScore.AddScoreTypeNoteToSubScore(model, noteMetaData, lastedOrder);
                }
                else
                {
                    metaEntity = subScore.AddScoreTypeToSubScore(model);//update some fields of VirtualTestCustomSubScore
                }
                //var metaEntity = subscore.AddScoreTypeToSubScore(model);//update some fields of VirtualTestCustomSubScore

                _virtualTestCustomMetaDataService.Save(metaEntity);
                if (metaEntity.VirtualTestCustomMetaDataID > 0)
                {
                    //Unable to use transaction scope so there's one way to manage transaction manually
                    try
                    {
                        _virtualTestCustomSubScoreRepository.Save(subScore);
                    }
                    catch (Exception ex)
                    {
                        methodException = ex;
                        //try to delete the inserted metaEntity
                        try
                        {
                            _virtualTestCustomMetaDataService.Delete(metaEntity.VirtualTestCustomMetaDataID);
                        }
                        catch (Exception exDeleteMeta)
                        {
                            methodException = exDeleteMeta;
                        }
                    }
                    finally
                    {
                        if (methodException != null)
                        {
                            throw new Exception("There was exception why adding new score type to subscore.", methodException);
                        }
                    }
                }

                // scope.Complete();
                return metaEntity.ScoreType;
            }
        }

        public bool HasAssociatedTest(int virtualTestCustomScoreId)
        {
            return
                _virtualTestCustomScoreSettingRepository.Select()
                    .Any(x => x.VirtualTestCustomScoreId == virtualTestCustomScoreId);
        }

        public bool HasAssociatedTestResult(int virtualTestCustomScoreId)
        {
            return
                _dataLockerRepository.DTLTemplateHasAssociatedTestResult(virtualTestCustomScoreId);
        }

        public void DeleteVirtualTestLegacyById(int virtualTestId, int userId, int roleId, out string error)
        {
            _dataLockerRepository.DeleteVirtualTestLegacyById(virtualTestId, userId, roleId, out error);
        }

        public string CheckScoreTypeBeforeAddingToTemplate(int userId, ScoreTypeModel newScoreType)
        {
            var customScore =
                _virtualTestCustomScoreRepository.Select()
                    .FirstOrDefault(x => x.VirtualTestCustomScoreId == newScoreType.TemplateID);

            return customScore.CheckScoreTypeBeforeAdding(newScoreType);
        }

        public bool CheckUserPermissionOnTemplate(int templateId, int userDistrictId)
        {
            var template = _virtualTestCustomScoreRepository.Select().FirstOrDefault(x => x.VirtualTestCustomScoreId == templateId);
            return template != null && userDistrictId == template.DistrictId;
        }

        public int CopyTemplateByID(int templateId, int userId, string templateName)
        {
            return _dataLockerRepository.CopyTemplateByID(templateId, userId, templateName);
        }
        public bool HasAssociatedAutoSave(int virtualTestCustomScoreId)
        {
            return
                _dataLockerRepository.HasAssociatedAutoSave(virtualTestCustomScoreId);
        }

        public (bool exist, string message) HasAssociatedCalculation(string scoreType, List<VirtualTestCustomMetaData> metaDatas, int? subScoreId, List<VirtualTestCustomSubScoreModel> subScoreNames)
        {
            var data = string.Empty;
            bool exist = false;
            var columnNameRelateds = new List<string>();
            foreach (var item in metaDatas)
            {
                if (item.ScoreType != scoreType)
                {
                    var metaDataObj = item.ParseMetaToObject();
                    if (metaDataObj.IsAutoCalculation && metaDataObj.Expression.Contains($"{item.VirtualTestCustomSubScoreID ?? 0}&{scoreType}"))
                    {
                        var subScoreName = subScoreNames.FirstOrDefault(f => f.Id == item.VirtualTestCustomSubScoreID)?.Name;
                        columnNameRelateds.Add($"{(item.VirtualTestCustomSubScoreID == null ? "Overall" : subScoreName)} with {scoreType}");
                    }
                }
            }
            if (columnNameRelateds.Any())
            {
                exist = true;
                data = $"Try removing or changing these references, or moving the formulas that include [{string.Join(", ", columnNameRelateds.Distinct())}] to a different column.";
            }
            return (exist, data);
        }
    }
}
