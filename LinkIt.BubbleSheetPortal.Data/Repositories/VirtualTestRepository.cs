using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserDefinedTypesDto;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualTestRepository : IVirtualTestRepository
    {
        private readonly Table<VirtualTestDBEntity> table;
        private readonly TestDataContext _testDataContext;

        public VirtualTestRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<VirtualTestDBEntity>();
            _testDataContext = TestDataContext.Get(connectionString);
        }

        public IQueryable<VirtualTestData> Select()
        {
            return table.Select(x => new VirtualTestData
            {
                AchievementLevelSettingID = x.AchievementLevelSettingID,
                Archived = x.Archived,
                AuthorUserID = x.AuthorUserID,
                BankID = x.BankID,
                CreatedDate = x.CreatedDate,
                EditedByUserID = x.EditedByUserID,
                Instruction = x.Instruction,
                Name = x.Name,
                PreProdVTID = x.PreProdVTID,
                PreQTIVirtualTestID = x.PreQTIVirtualTestID,
                StateID = x.StateID,
                UpdatedDate = x.UpdatedDate,
                VirtualTestID = x.VirtualTestID,
                VirtualTestSourceID = x.VirtualTestSourceID,
                VirtualTestType = x.VirtualTestType,
                VirtualTestSubTypeID = x.VirtualTestSubTypeID,
                TestScoreMethodID = x.TestScoreMethodID,
                IsTeacherLed = x.IsTeacherLed,
                HasUseRationale = x.HasUseRationale,
                IsNumberQuestions = x.IsNumberQuestions,
                IsMultipleTestResult = x.IsMultipleTestResult,
                NavigationMethodID = x.NavigationMethodID,
                DatasetCategoryID = x.DataSetCategoryID,
                DatasetOriginID = x.DataSetOriginID,
                ParentTestID = x.ParentTestID,
                OriginalTestID = x.OriginalTestID
            });
        }

        public void Save(VirtualTestData item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestID.Equals(item.VirtualTestID));

            if (entity.IsNull())
            {
                entity = new VirtualTestDBEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.VirtualTestID = entity.VirtualTestID;
        }

        public void Delete(VirtualTestData item)
        {
            var entity = table.FirstOrDefault(x => x.VirtualTestID.Equals(item.VirtualTestID));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualTestData item, VirtualTestDBEntity entity)
        {
            entity.AchievementLevelSettingID = item.AchievementLevelSettingID;
            entity.Archived = item.Archived;
            entity.AuthorUserID = item.AuthorUserID;
            entity.BankID = item.BankID;
            entity.CreatedDate = item.CreatedDate;
            entity.EditedByUserID = item.EditedByUserID;
            entity.Instruction = item.Instruction;
            entity.Name = item.Name;
            entity.PreProdVTID = item.PreProdVTID;
            entity.PreQTIVirtualTestID = item.PreQTIVirtualTestID;
            entity.StateID = item.StateID;
            entity.UpdatedDate = item.UpdatedDate;
            entity.VirtualTestID = item.VirtualTestID;
            entity.VirtualTestSourceID = item.VirtualTestSourceID;
            entity.VirtualTestType = item.VirtualTestType;
            entity.VirtualTestSubTypeID = item.VirtualTestSubTypeID;
            entity.TestScoreMethodID = item.TestScoreMethodID;
            entity.IsTeacherLed = item.IsTeacherLed;
            entity.HasUseRationale = item.HasUseRationale;
            entity.IsNumberQuestions = item.IsNumberQuestions;
            entity.IsMultipleTestResult = item.IsMultipleTestResult;
            entity.NavigationMethodID = item.NavigationMethodID;
            entity.DataSetOriginID = item.DatasetOriginID;
            entity.DataSetCategoryID = item.DatasetCategoryID;
            entity.OriginalTestID = item.OriginalTestID;
            entity.ParentTestID = item.ParentTestID;
        }


        public VirtualTestProperty GetVirtualTestProperties(int virtualTestId, int roleId, int districtId)
        {
            var result = _testDataContext.VirtualTestProperitesNew(virtualTestId, roleId, districtId).FirstOrDefault();
            if (result != null)
            {
                return new VirtualTestProperty()
                {
                    VirtualTestId = result.VirtualTestID,
                    Author = result.Author,
                    AuthorUserId = result.AuthorUserID ?? 0,
                    CreatedDate = result.CreatedDate ?? DateTime.MinValue,
                    UpdatedDate = result.UpdatedDate ?? DateTime.MinValue,
                    MaxResultDate = result.MaxResultDate ?? DateTime.MinValue,
                    MinResultDate = result.MinResultDate ?? DateTime.MinValue,
                    Name = result.NAME,
                    TotalQuestion = result.TotalQuestion ?? 0,
                    TotalTestResult = result.TotalTestResult ?? 0,
                    Instruction = result.Instruction,
                    DataSetCategoryID = result.DataSetCategoryID
                };
            }
            return new VirtualTestProperty();
        }

        public IQueryable<VirtualSectionQuestionQtiItem> GetVirtualSectionQuestionQtiItem(int virtualTestId)
        {
            return _testDataContext.GetVirtualSectionQuestionQtiItem(virtualTestId).Select(x => new VirtualSectionQuestionQtiItem
            {
                VirtualSectionID = x.VirtualSectionID,
                VirtualQuestionID = x.VirtualQuestionID,
                Order = x.Order,
                QTIItemID = x.QTIItemID,
                XmlContent = x.XmlContent,
                QTIGroupID = x.QTIGroupID,
                VirtualSectionOrder = x.VirtualSectionOrder,
                VirtualSectionTitle = string.IsNullOrEmpty(x.VirtualSectionTitle) ? string.Empty : x.VirtualSectionTitle,
                VirtualSectionQuestionID = x.VirtualSectionQuestionID,
                QuestionOrder = x.QuestionOrder,
                BaseVirtualQuestionId = x.BaseVirtualQuestionId ?? 0,
                QTISchemaID = x.QTISchemaID,
                AnswerIdentifiers = x.AnswerIdentifiers
            }).AsQueryable();
        }

        public IQueryable<VirtualQuestionWithCorrectAnswer> GetVirtualQuestionWithCorrectAnswer(int virtualTestId)
        {
            return _testDataContext.GetVirtualQuestionWithCorrectAnswer(virtualTestId).Select(x => new VirtualQuestionWithCorrectAnswer
            {
                VirtualSectionID = x.VirtualSectionID,
                VirtualQuestionID = x.VirtualQuestionID,
                Order = x.Order,
                QTIItemID = x.QTIItemID,
                XmlContent = x.XmlContent,
                QTIGroupID = x.QTIGroupID,
                VirtualSectionOrder = x.VirtualSectionOrder,
                VirtualSectionTitle = string.IsNullOrEmpty(x.VirtualSectionTitle) ? string.Empty : x.VirtualSectionTitle,
                VirtualSectionQuestionID = x.VirtualSectionQuestionID,
                QuestionOrder = x.QuestionOrder,
                BaseVirtualQuestionId = x.BaseVirtualQuestionId ?? 0,
                QTISchemaID = x.QTISchemaID,
                AnswerIdentifiers = x.AnswerIdentifiers,
                PointsPossible = x.PointsPossible,
                CorrectAnswer = x.CorrectAnswer,
                IsRubricBasedQuestion = x.IsRubricBasedQuestion
            }).AsQueryable();
        }

        public int CanDeleteVirtualTestById(int virtualtestId)
        {
            var result = _testDataContext.CanDeleteVirtualTest(virtualtestId).FirstOrDefault();
            if (result == null || !result.Column1.HasValue)
                return 0;
            return result.Column1.Value;
        }

        public void DeleteVirtualTestById(int virtualTestId, int userId, int roleId, out string error)
        {
            error = string.Empty;
            _testDataContext.DeleteVirtualTestByID(virtualTestId, userId, roleId, ref error);
        }

        public VirtualQuestionProperties GetVirtualQuestionProperties(int virtualQuestionId)
        {
            var x = _testDataContext.GetVirtualQuestionProperties(virtualQuestionId).FirstOrDefault();
            if (x != null)
            {
                return new VirtualQuestionProperties()
                {
                    VirtualTestID = x.VirtualTestID,
                    VirtualQuestionID = x.VirtualQuestionID,
                    QtiSchemaId = x.QTISchemaID,
                    QtiSchemaDes = x.QtiSchemaDes,
                    PointsPossible = x.PointsPossible,
                    ItemBank = x.ItemBank,
                    ItemSet = x.ItemSet,
                    StandardNumberList = x.StandardNumberList,
                    TopicList = x.TopicList,
                    SkillList = x.SkillList,
                    OtherList = x.OtherList,
                    XmlContent = x.XmlContent,
                    ItemTagList = x.ItemTagList,
                    QtiPointsPossible = x.QtiPointsPossible,
                    QTIBankId = x.QTIBankID,
                    BaseVirtualQuestionId = x.BaseVirtualQuestionId ?? 0,
                    QTIGroupId = x.QTIGroupID,
                    ResponseProcessingTypeId = x.ResponseProcessingTypeID ?? 0,
                    QuestionLabel = x.QuestionLabel,
                    QuestionNumber = x.QuestionNumber,
                    IsRubricBasedQuestion = x.IsRubricBasedQuestion ?? false,
                    ScoreName = x.ScoreName
                };
            }
            return new VirtualQuestionProperties();
        }

        public void RemoveVirtualSection(int virtualSectionId, out string error)
        {
            error = string.Empty;
            _testDataContext.RemoveVirtualSection(virtualSectionId, ref error);
        }

        public void ReassignVirtualQuestionOrder(int virtualTestID)
        {
            _testDataContext.ReassignVirtualQuestionOrder(virtualTestID);
        }

        public void RemoveVirtualQuestion(int virtualQuestionId, out string error)
        {
            error = string.Empty;
            _testDataContext.RemoveVirtualQuestion(virtualQuestionId, ref error);
        }

        public bool CanRemoveVirtualQuestion(string virtualQuestionIds, out string error)
        {
            error = string.Empty;
            var result = _testDataContext.CanRemoveQuestion(virtualQuestionIds, ref error).FirstOrDefault();
            return result != null && (result.CanRemoveVirtualQuestion ?? false);
        }

        public IQueryable<ComplexVirtualQuestionAnswerScore> GetComplexVirtualQuestionAnswerScores(int virtualQuestionId)
        {
            return _testDataContext.GetComplexVirtualQuestionAnswerScores(virtualQuestionId).Select(x => new ComplexVirtualQuestionAnswerScore
            {
                Answer = x.Answer,
                QTIItemAnswerScoreId = x.VirtualQuestionAnswerScoreID,
                QtiItemScore = x.QTIItemScore,
                ResponseIdentifier = x.ResponseIdentifier,
                Score = x.Score,
                SubPointsPossible = x.subPointsPossible,
                VirtualQuestionAnswerScoreId = x.VirtualQuestionAnswerScoreID,
                VirtualQuestionId = x.VirtualQuestionID,
                QiCorrectAnswer = x.QiCorrectAnswer,
                QiSubCorrectAnswer = x.QiSubCorrectAnswer,
                QiSubPointsPossible = (x.QiSubPointsPossible ?? 0).ToString(),
                QTISchemaID = x.QTISchemaID ?? 0,
                ResponseProcessingTypeId = x.ResponseProcessingTypeID ?? 0
            }).AsQueryable();
        }

        public void UpdateComplexVirtualQuestionAnswerScores(int virtualQuestionId, string responseIdentifier, int score,
                                               int subPointsPossible, int pointsPossible, out string error)
        {
            error = string.Empty;
            _testDataContext.UpdateComplexVirtualQuestionAnswerScores(virtualQuestionId, responseIdentifier, score, subPointsPossible, pointsPossible, ref error);
        }

        public void ReassignVirtualSectionOrder(int virtualTestID)
        {
            _testDataContext.ReassignVirtualSectionOrder(virtualTestID);
        }
        public void ReassignVirtualSectionQuestionOrder(int virtualTestId, int virtualSectionId)
        {
            _testDataContext.ReassignVirtualSectionQuestionOrder(virtualTestId, virtualSectionId);
        }

        public int FixSectionDataForVirtualTest(int virtualTestId)
        {
            return _testDataContext.FixSectionDataForVirtualTest(virtualTestId);
        }

        public void MoveVirtualSectionQuestion(int virtualTestId, int sourceIndex, int sourceSectionId, int targetIndex,
                                        int targetSectionId)
        {
            _testDataContext.MoveVirtualSectionQuestion(virtualTestId, sourceIndex, sourceSectionId, targetIndex,
                                                        targetSectionId);
        }

        public void MoveManyVirtualQuestionGroup(int virtualTestId, string virtualQuestionIdString, int? sourceQuestionGroupId, int targetVirtualSectionId, int? targetQuestionGroupId, int targetIndex)
        {
            _testDataContext.MoveManyVirtualQuestionGroup(virtualTestId, virtualQuestionIdString, sourceQuestionGroupId, targetQuestionGroupId, targetIndex, targetVirtualSectionId);
        }

        public void MoveVirtualSection(int virtualTestId, int sourceIndex, int targetIndex)
        {
            _testDataContext.MoveVirtualSection(virtualTestId, sourceIndex, targetIndex);
        }

        public IQueryable<VirtualQuestionS3Object> GetVirtualQuestionToCreateS3Object(int virtualTestId)
        {
            return _testDataContext.GetVirtualQuestionToCreateS3Object(virtualTestId).Select(x => new VirtualQuestionS3Object
            {
                VirtualQuestionID = x.VirtualQuestionID,
                QTIItemID = x.QTIItemID,
                QTISchemaID = x.QTISchemaID,
                XmlContent = x.XmlContent,
                QTIGroupID = x.QTIGroupID,
                PointsPossible = x.PointsPossible,
                QuestionOrder = x.QuestionOrder,
                VirtualSectionQuestionID = x.VirtualSectionQuestionID,
                VirtualSectionQuestionOrder = x.VirtualSectionQuestionOrder,
                VirtualSectionID = x.VirtualSectionID ?? 0,
                BaseVirtualQuestionID = x.BaseVirtualQuestionID,
                QuestionLabel = x.QuestionLabel
            }).AsQueryable();
        }

        public List<GhostQuestion> GetGhostQuestions(int baseVirtualQuestionID)
        {
            var ghostQuestions = _testDataContext.GetGhostQuestions(baseVirtualQuestionID).ToList();
            var result = new List<GhostQuestion>();
            foreach (var getGhostQuestionsResult in ghostQuestions)
            {
                var item = new GhostQuestion
                {
                    BaseVirtualQuestionID = baseVirtualQuestionID,
                    VirtualQuestionID = getGhostQuestionsResult.VirtualQuestionID,
                    QuestionOrder = getGhostQuestionsResult.QuestionOrder
                };

                result.Add(item);
            }

            return result;
        }

        public int? GetMaxQuestionOrder(int virtualTestID)
        {
            var result = _testDataContext.GetMaxQuestionOrder(virtualTestID).SingleOrDefault();
            return result.Column1;
        }

        public void ReassignBaseVirtualSectionQuestionOrder(int virtualTestId, int? virtualSectionId)
        {
            _testDataContext.ReassignBaseVirtualSectionQuestionOrder(virtualTestId, virtualSectionId);
        }

        public List<VirtualSectionQuestion> GetBaseQuestions(int virtualTestId, int virtualQuestionId)
        {
            return
                _testDataContext.GetBaseQuestions(virtualTestId, virtualQuestionId).Select(
                    x => new VirtualSectionQuestion
                    {
                        VirtualSectionQuestionId = x.VirtualSectionQuestionId,
                        VirtualSectionId = x.VirtualSectionId,
                        VirtualQuestionId = x.VirtualQuestionId,
                        Order = x.Order,
                        VirtualTestId = virtualTestId
                    }).ToList();
        }

        public VirtualTestData GetVirtualTestByID(int virtualTestID)
        {
            return Select().SingleOrDefault(x => x.VirtualTestID == virtualTestID);
        }

        public void UpdateBaseVirtualQuestionClone(int oldVirtualTestId, int newVirtualTestId)
        {
            _testDataContext.UpdateBaseVirtualQuestionClone(oldVirtualTestId, newVirtualTestId);
        }

        public void MoveManyVirtualSectionQuestion(int virtualTestId, string virtualQuestionIdString, int sourceIndex,
            int sourceSectionId, int targetIndex,
            int targetSectionId, out string message)
        {
            message = string.Empty;
            _testDataContext.MoveManyVirtualSectionQuestion(virtualTestId, virtualQuestionIdString, sourceIndex, sourceSectionId, targetIndex, targetSectionId, ref message);
        }

        public IQueryable<ChoiceVariableVirtualQuestionAnswerScore> GetChoiceVariableVirtualQuestionAnswerScores(int virtualQuestionId)
        {
            return _testDataContext.GetChoiceVariableVirtualQuestionAnswerScores(virtualQuestionId).Select(x => new ChoiceVariableVirtualQuestionAnswerScore
            {
                VirtualQuestionAnswerScoreId = x.VirtualQuestionAnswerScoreID,
                VirtualQuestionId = x.VirtualQuestionID,
                Answer = x.Answer,
                QtiItemScore = x.QTIItemScore,
                ResponseIdentifier = x.ResponseIdentifier,
                Score = x.Score,
                QtiSchemaID = x.QTISchemaID
            }).AsQueryable();
        }

        public AlgorithmicMaxPoint GetMaxPointAlgorithmicByVirtualQuestionIDAndQTIItemID(int virtualQuestionId, int qtiItemId)
        {
            var result = _testDataContext.GetMaxPointAlgorithmicByVirtualQuestionIDAndQTIItemID(virtualQuestionId, qtiItemId).FirstOrDefault();
            return new AlgorithmicMaxPoint()
            {
                QTIItemMaxPoint = result.QTIITemPointEarned.GetValueOrDefault(),
                QuestionMaxPoint = result.QuestionPointEarned.GetValueOrDefault()
            };
        }

        public void ClearQuestionLabelQuestionLNumber(int virtualTestId)
        {
            try
            {
                //TODO:
                _testDataContext.ClearQuestionLabelQuestionLNumber(virtualTestId);
            }
            catch (Exception ex)
            {
                //TODO:
            }
        }

        public List<VirtualTestOrder> GetVirtualTestOrders(int districtId)
        {
            var query = _testDataContext.GetVirtualTestOrder(districtId);
            if (query != null)
            {
                return query.Select(x => new VirtualTestOrder()
                {
                    VirtualTestID = x.VirtualTestID,
                    Order = x.Order
                }).OrderBy(x => x.Order).ToList();
            }
            return new List<VirtualTestOrder>();
        }

        public int CountOpendedQuestionPerTest(int virtualtestId)
        {
            var obj = _testDataContext.CountOpendedByVirtualTest(virtualtestId);
            if (obj != null)
            {
                return obj.FirstOrDefault().TotalQuestionOpended.GetValueOrDefault();
            }
            return 0;
        }

        public List<ListItem> VirtualTestWithOutTestResultForPublisher(int districtId)
        {
            var query = _testDataContext.VirtualTestWithOutTestResultForPublisherProc(districtId).ToList();
            var virtualTestResult = GetVirtualTestHasCheckRetake(query);
            if (virtualTestResult.Any())
            {
                return virtualTestResult.Select(o => new ListItem()
                {
                    Id = o.VirtualTestID,
                    Name = o.Name
                }).ToList();
            }
            return new List<ListItem>();
        }
        public void DeleteVirtualQuestionBranchingByTestID(int virtualTestId)
        {
            _testDataContext.DeleteVirtualQuestionBranchingByVirtualTestID(virtualTestId);
        }
        public void DeleteVirtualSectionBranchingByTestID(int virtualTestId)
        {
            _testDataContext.DeleteVirtualSectionBranchingByVirtualTestID(virtualTestId);
        }
        public void DeleteVirtualQuestionBranchingAlgorithmByTestID(int virtualTestId)
        {
            _testDataContext.DeleteVirtualQuestionBranchingAlgorithmByVirtualTestID(virtualTestId);
        }
        public void ChangePositionVirtualSection(int virtualTestId, int sourceIndex, int targetIndex)
        {
            _testDataContext.ChangePositionVirtualSection(virtualTestId, sourceIndex, targetIndex);
        }
        public List<ConstructedResponseQuestion> GetConstructedResponseQuestions(int virtualTestId)
        {
            return _testDataContext.GetConstructedResponseQuestions(virtualTestId)
                .Select(x => new ConstructedResponseQuestion()
                {
                    VirtualQuestionId = x.VirtualQuestionID,
                    XmlContent = x.XmlContent
                }).ToList();
        }
        private List<VirtualTestWithOutTestResultForPublisherProcResult> GetVirtualTestHasCheckRetake(List<VirtualTestWithOutTestResultForPublisherProcResult> virtualTests)
        {
            var virtualTestsHasIncludeRetakeOrigin = virtualTests.Where(x => x.OriginalTestID == null);
            var virtualTestsHasRetake = virtualTests.Where(x => x.OriginalTestID.HasValue)
                .GroupBy(x => x.OriginalTestID)
                .Select(x => x.OrderByDescending(y => y.VirtualTestID).FirstOrDefault());
            var virtualTestIDsRetakeOrigin = virtualTestsHasRetake.Select(x => x.OriginalTestID.Value).ToArray();
            return virtualTestsHasIncludeRetakeOrigin
                .Where(x => !virtualTestIDsRetakeOrigin.Contains(x.VirtualTestID))
                .Concat(virtualTestsHasRetake).ToList();
        }

        public void ClonePBSForTestRetake(int oldTestId, int newTestId)
        {
            _testDataContext.ClonePBSForTestRetake(oldTestId, newTestId);
        }

        public GetTestByAdvanceFilter GetTestByAdvanceFilter(GetTestByAdvanceFilterRequest request)
        {
            var parameters = new List<(string, string, SqlDbType, object, ParameterDirection)>();
            var totalRecords = 0;

            parameters.Add(("BankIDList", "IntegerList", SqlDbType.Structured, request.Filters.BankIds.Select(i => new IntegerListDto { Id = i }).ToDataTable(), ParameterDirection.Input));
            parameters.Add(("CategoryIDList", "IntegerList", SqlDbType.Structured, request.Filters.CategoryIds.Select(i => new IntegerListDto { Id = i }).ToDataTable(), ParameterDirection.Input));
            parameters.Add(("ExcludedVirtualTestIDList", "IntegerList", SqlDbType.Structured, request.Filters.ExcludedVirtualTestIds.Select(i => new IntegerListDto { Id = i }).ToDataTable(), ParameterDirection.Input));
            parameters.Add(("PageIndex", "", SqlDbType.Int, request.Pagination.PageIndex, ParameterDirection.Input));
            parameters.Add(("PageSize", "", SqlDbType.Int, request.Pagination.PageSize, ParameterDirection.Input));
            parameters.Add(("SortColumns", "", SqlDbType.VarChar, $"{request.Pagination.SortBy} {request.Pagination.SortDirection}", ParameterDirection.Input));
            parameters.Add(("GeneralSearch", "", SqlDbType.VarChar, request.GeneralSearch ?? "", ParameterDirection.Input));
            parameters.Add(("TotalRecords", "", SqlDbType.Int, totalRecords, ParameterDirection.Output));

            var data = _testDataContext.Query<GetTestResult>(new SqlParameterRequest()
            {
                StoredName = "GetTestByAdvanceFilter",
                Parameters = parameters
            }, out IEnumerable<(string ParameterName, object Value)> outputs);

            totalRecords = (int)outputs.First(p => p.ParameterName == "TotalRecords").Value;

            return new GetTestByAdvanceFilter
            {
                Data = data,
                TotalRecords = totalRecords
            };

        }


        public (string CategoriesName,IList<TestPropertyExportData> DataExport) GetTestPropertyToExportByVirtualTestIDs(int districtID, string strVirtualTestID)
        {
            var testMultiResult = _testDataContext.GetTestPropertyToExportByVirtualTestIDs(districtID, strVirtualTestID);
            var firstResult = testMultiResult.GetResult<string>().First();            
            var secondResult = testMultiResult.GetResult<GetTestPropertyToExportResult>()
            .Select(o => new TestPropertyExportData
            {
                VirtualTestID = o.VirtualTestID,
                TestName = o.TestName,
                CreatedDate = o.CreatedDate,
                UpdatedDate = o.UpdatedDate,
                CreatedBy = o.CreatedBy,
                QuestionCount = o.QuestionCount,
                TotalPointsPossible = o.TotalPointsPossible,
                TestResultCount = o.TestResultCount,
                EarliestResultDate = o.EarliestResultDate,
                MostRecentResultDate = o.MostRecentResultDate,
                TestCategory = o.TestCategory,
                InterviewStyleAssessment = o.InterviewStyleAssessment,
                BankName = o.BankName,
                BankGrade = o.BankGrade,
                BankSubject = o.BankSubject,
                QuestionNumber = o.QuestionNumber,
                PassageNumber = o.PassageNumber,
                QTIItemID = o.QTIItemID,
                QTIItemTitle = o.QTIItemTitle,
                VirtualQuestionTags = o.VirtualQuestionTags,
                QTISchemaName = o.QTISchemaName,
                PointsPossible = o.PointsPossible,
                StandardNumbers = o.StandardNumbers,
                IsRubricBasedQuestion = o.IsRubricBasedQuestion,
                CorrectAnswer = o.CorrectAnswer,
                QTISchemaID = o.QTISchemaID,
                XmlContent = o.XmlContent,
                ResponseProcessing = o.ResponseProcessing,
                AlgorithmicExpression = o.AlgorithmicExpression,
                VirtualQuestionID = o.VirtualQuestionID
            }).ToList();
            return (firstResult, secondResult);
        }

        public IList<TestLibraryExportData> GetTestLibraryToExportByDistrictID(int districtID)
        {
            return _testDataContext.GetTestLibraryToExportByDistrictID(districtID)
                            .Select(o => new TestLibraryExportData
                            {
                                VirtualTestID = o.VirtualTestID,
                                TestName = o.TestName,
                                CreatedDate = o.CreatedDate,
                                UpdatedDate = o.UpdatedDate,
                                CreatedBy = o.CreatedBy,
                                QuestionCount = o.QuestionCount,
                                TotalPointsPossible = o.TotalPointsPossible,
                                TestResultCount = o.TestResultCount,
                                EarliestResultDate = o.EarliestResultDate,
                                MostRecentResultDate = o.MostRecentResultDate,
                                TestCategory = o.TestCategory,
                                InterviewStyleAssessment = o.InterviewStyleAssessment,
                                BankName = o.BankName,
                                BankGrade = o.BankGrade,
                                BankSubject = o.BankSubject
                            }).ToList();
        }

        public IList<ChoiceVariableVirtualQuestionAnswerScore> GetChoiceVariableVirtualQuestionAnswerScoresByVirtualQuestionIDs(string strVirtualQuestionID)
        {
            return _testDataContext.GetChoiceVariableVirtualQuestionAnswerScoresByVirtualQuestionIDs(strVirtualQuestionID)
                            .Select(x => new ChoiceVariableVirtualQuestionAnswerScore
                            {
                                VirtualQuestionAnswerScoreId = x.VirtualQuestionAnswerScoreID,
                                VirtualQuestionId = x.VirtualQuestionID,
                                Answer = x.Answer,
                                QtiItemScore = x.QTIItemScore,
                                ResponseIdentifier = x.ResponseIdentifier,
                                Score = x.Score,
                                QtiSchemaID = x.QTISchemaID
                            }).ToList();
        }

        public IList<ComplexVirtualQuestionAnswerScore> GetComplexVirtualQuestionAnswerScoresByVirtualQuestionIDs(string strVirtualQuestionID)
        {
            return _testDataContext.GetComplexVirtualQuestionAnswerScoresByVirtualQuestionIDs(strVirtualQuestionID).Select(x => new ComplexVirtualQuestionAnswerScore
            {
                Answer = x.Answer,
                QTIItemAnswerScoreId = x.VirtualQuestionAnswerScoreID,
                QtiItemScore = x.QTIItemScore,
                ResponseIdentifier = x.ResponseIdentifier,
                Score = x.Score,
                SubPointsPossible = x.subPointsPossible,
                VirtualQuestionAnswerScoreId = x.VirtualQuestionAnswerScoreID,
                VirtualQuestionId = x.VirtualQuestionID,
                QiCorrectAnswer = x.QiCorrectAnswer,
                QiSubCorrectAnswer = x.QiSubCorrectAnswer,
                QiSubPointsPossible = (x.QiSubPointsPossible ?? 0).ToString(),
                QTISchemaID = x.QTISchemaID ?? 0,
                ResponseProcessingTypeId = x.ResponseProcessingTypeID ?? 0
            }).ToList();
        }

    }
}
