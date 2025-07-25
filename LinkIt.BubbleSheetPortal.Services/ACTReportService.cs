using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ACTReport;
using LinkIt.BubbleSheetPortal.Models.ACTSummaryReport;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ACTReportService
    {
        private readonly IACTReportRepository actReportRepository;

        public ACTReportService(IACTReportRepository actReportRepository)
        {
            this.actReportRepository = actReportRepository;
        }

        public List<ACTAnswerSectionData> GetACTAnswerSectionData(int testResultID)
        {
            var aCTAnswerSectionDatas = actReportRepository.ACTGetAnswerSectionData(testResultID).ToList();

            var answerData = aCTAnswerSectionDatas.Select(x => new ACTReIndexModel
                                                                   {
                                                                       AnswerID = x.AnswerID,
                                                                       SectionID = x.SectionID,
                                                                       Order = x.QuestionOrder
                                                                   }).Distinct(new ACTReIndexModelComparer()).ToList();            

            // Calculate question order by ordinary of current question order
            int currentVirtualSectionID = 0;
            int counter = 0;
            foreach (var actAnswerSectionData in answerData)
            {
                if(currentVirtualSectionID != actAnswerSectionData.SectionID)
                {
                    currentVirtualSectionID = actAnswerSectionData.SectionID;
                    counter = 1;
                }
                actAnswerSectionData.Order = counter;
                counter++;
            }

            foreach (var actAnswerSectionData in aCTAnswerSectionDatas)
            {
                actAnswerSectionData.QuestionOrder =
                    answerData.First(x => x.AnswerID == actAnswerSectionData.AnswerID).Order;
            }

            return aCTAnswerSectionDatas;
        }

        public List<ACTSectionTagData> GetACTSectionTagData(int studentID, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTGetSectionTagData(studentID, virtualTestSubTypeId).ToList();
        }

        public List<ACTTestHistoryData> GetACTTestHistoryData(int studentID, int virtualTestSubTypeId)
        {
            var aCTTestHistoryDatas = actReportRepository.ACTGetTestHistoryData(studentID, virtualTestSubTypeId).ToList();
            RenameSectionName(aCTTestHistoryDatas);

            return aCTTestHistoryDatas;
        }

        // Rename SectioName based on TestResultSubScoreID
        private void RenameSectionName(List<ACTTestHistoryData> aCTTestHistoryDatas)
        {
            var testResultList = aCTTestHistoryDatas.Select(en => en.TestResultID);

            foreach (var testResultId in testResultList)
            {
                var testResultData = aCTTestHistoryDatas.Where(en => en.TestResultID == testResultId).OrderBy(en => en.TestResultSubScoreID).ToList();
                for (int i = 0; i < testResultData.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            testResultData[i].SectionName = "English";
                            break;
                        case 1:
                            testResultData[i].SectionName = "Math";
                            break;
                        case 2:
                            testResultData[i].SectionName = "Reading";
                            break;
                        case 3:
                            testResultData[i].SectionName = "Science";
                            break;
                        case 4:
                            testResultData[i].SectionName = "Writing";
                            break;
                        case 5:
                            testResultData[i].SectionName = "E/W";
                            break;
                    }
                }
            }
        }

        public ACTStudentInformation GetACTStudentInformation(int studentID, int testResultID)
        {
            return actReportRepository.ACTGetStudentInformation(studentID, testResultID);
        }

        public List<ACTSummaryClassLevelData> GetACTSummaryClassLevelData(int classID, int virtualTestID, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetClassLevelData(classID, virtualTestID, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummaryTeacherLevelData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetTeacherLevelData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummaryTeacherLevelAverageData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetTeacherLevelAverageData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummaryTeacherLevelBaselineData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetTeacherLevelBaselineData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummaryTeacherLevelImprovementData(int userId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetTeacherLevelImprovementData(userId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummarySchoolLevelImprovementData(int schoolId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetSchoolLevelImprovementData(schoolId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummaryDistrictLevelImprovementData(int districtId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetDistrictLevelImprovementData(districtId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummarySchoolLevelData(int schoolID, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetSchoolLevelData(schoolID, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummarySchoolLevelAverageData(int schoolID, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetSchoolLevelAverageData(schoolID, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummarySchoolLevelBaselineData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetSchoolLevelBaselineData(schoolId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummaryDistrictLevelData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetDistrictLevelData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummaryDistrictLevelAverageData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetDistrictLevelAverageData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetACTSummaryDistrictLevelBaselineData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetDistrictLevelBaselineData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummaryClassLevelData> GetACTSummaryClassLevelImprovementData(int classId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetClassLevelImprovementData(classId, virtualTestIds, improvementOption, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummaryClassLevelData> GetACTSummaryClassLevelBaselineData(int classId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.ACTSummaryGetClassLevelBaselineData(classId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummaryClassLevelData> GetSATSummaryClassLevelBaselineData(int classId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetClassLevelBaselineData(classId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTAnswerSectionData> GetSATAnswerSectionData(int testResultID)
        {
            var aCTAnswerSectionDatas = actReportRepository.SATGetAnswerSectionData(testResultID).ToList();

            var answerData = aCTAnswerSectionDatas.Select(x => new ACTReIndexModel
            {
                AnswerID = x.AnswerID,
                SectionID = x.SectionID,
                Order = x.QuestionOrder,
                SubjectID = x.SubjectID,
                VirtualQuestionID = x.VirtualQuestionID
            }).Distinct(new ACTReIndexModelComparer()).ToList();

            // Calculate question order by ordinary of current question order
            int currentSubjectId = 0;
            int currentSectionID = 0;
            int counter = 0;
            foreach (var actAnswerSectionData in answerData)
            {
                if (currentSubjectId != actAnswerSectionData.SubjectID)
                {
                    currentSubjectId = actAnswerSectionData.SubjectID;
                    currentSectionID = actAnswerSectionData.SectionID;
                    counter = 1;
                }
                else
                {
                    if (currentSectionID != actAnswerSectionData.SectionID)
                    {
                        currentSectionID = actAnswerSectionData.SectionID;
                        counter = 1;
                    }
                }
                
                actAnswerSectionData.Order = counter;
                counter++;
            }

            foreach (var actAnswerSectionData in aCTAnswerSectionDatas)
            {
                actAnswerSectionData.QuestionOrder =
                    answerData.First(x => x.VirtualQuestionID == actAnswerSectionData.VirtualQuestionID).Order;
            }

            return aCTAnswerSectionDatas;
        }

        public List<ACTSectionTagData> GetSATSubjectTagData(int studentID, int virtualTestSubTypeID)
        {
            return actReportRepository.SATGetSectionTagData(studentID, virtualTestSubTypeID).ToList();
        }

        public List<ACTTestHistoryData> GetSATTestHistoryData(int studentID, int virtualTestSubTypeId)
        {
            var aCTTestHistoryDatas = actReportRepository.SATGetTestHistoryData(studentID, virtualTestSubTypeId).ToList();
            //RenameSectionName(aCTTestHistoryDatas);

            return aCTTestHistoryDatas;
        }

        public ACTStudentInformation GetSATStudentInformation(int studentID, int testResultID)
        {
            return actReportRepository.SATGetStudentInformation(studentID, testResultID);
        }
        
        public List<ReportType> GetReportTypes(List<int> reportTypeIds)
        {
            return actReportRepository.GetReportTypes(reportTypeIds).OrderBy(x => x.ReportOrder).ToList();
        }

        public List<ReportType> GetAllReportTypes()
        {
            return actReportRepository.GetAllReportTypes().OrderBy(x => x.ReportOrder).ToList();
        }


        public List<ACTSummaryClassLevelData> GetSATSummaryClassLevelData(int classID, int virtualTestID, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetClassLevelData(classID, virtualTestID, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummaryTeacherLevelData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetTeacherLevelData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummaryTeacherLevelAverageData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetTeacherLevelAverageData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummaryTeacherLevelBaselineData(int userId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetTeacherLevelBaselineData(userId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummaryTeacherLevelImprovementData(int userId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetTeacherLevelImprovementData(userId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummarySchoolLevelImprovementData(int schoolId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetSchoolLevelImprovementData(schoolId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummaryDistrictLevelImprovementData(int districtId, int districtTermId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetDistrictLevelImprovementData(districtId, districtTermId, virtualTestIds, improvementOption, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummarySchoolLevelData(int schoolID, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetSchoolLevelData(schoolID, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummarySchoolLevelAverageData(int schoolID, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetSchoolLevelAverageData(schoolID, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummarySchoolLevelBaselineData(int schoolId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetSchoolLevelBaselineData(schoolId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummaryDistrictLevelData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetDistrictLevelData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummaryDistrictLevelAverageData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetDistrictLevelAverageData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummarySchoolOrTeacherLevelData> GetSATSummaryDistrictLevelBaselineData(int districtId, int districtTermId, List<int> virtualTestIds, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetDistrictLevelBaselineData(districtId, districtTermId, virtualTestIds, virtualTestSubTypeId).ToList();
        }

        public List<ACTSummaryClassLevelData> GetSATSummaryClassLevelImprovementData(int classId, List<int> virtualTestIds, string improvementOption, int virtualTestSubTypeId)
        {
            return actReportRepository.SATSummaryGetClassLevelImprovementData(classId, virtualTestIds, improvementOption, virtualTestSubTypeId).ToList();
        }
    }
}