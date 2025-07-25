using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.Helpers.SGO;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SGO;
using System.Drawing;
using DevExpress.Data.PLinq.Helpers;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class EstablishStudentGroupViewModel
    {
        public int SGOID { get; set; }
        public int SGOGroupID { get; set; }
        public string SGOName { get; set; }

        public List<ListItemsViewModel> ListStudents { get; set; }
        public List<SGODataPoint> ListDataPoint { get; set; }
        public List<SGOGroup> ListGroups { get; set; }

        public List<SGOGroup> ListCustomGroups
        {
            get
            {
                return ListGroups.Where(o => o.Order != Constanst.ExcludedGroupOrder && o.Order != Constanst.ToBePlacedGroupOrder).ToList();
            }
        }

        public List<SGOGroupViewModel> ListGroupsViewModel
        {
            get
            {
                return ListGroups.Select(x => new SGOGroupViewModel
                {
                    Name = x.Name,
                    Order = x.Order,
                    SGOGroupID = x.SGOGroupID,
                    SGOID = x.SGOID,
                    IsExcludedGroup = x.Order == Constanst.ExcludedGroupOrder,
                    Color =
                                                      x.Order != Constanst.ExcludedGroupOrder &&
                                                      x.Order != Constanst.ToBePlacedGroupOrder
                                                          ? ColorHelper.GetColorHexList(ListCustomGroups.Count, true)[x.Order - 1]
                                                          : ColorHelper.HexConverter(Color.Gray)
                }).ToList();
            }
        }

        public List<SGOStudent> ListSGOStudents { get; set; }

        public Dictionary<SGOGroup, List<SGOStudent>> ListStudentInGroup
        {
            get
            {
                var data = new Dictionary<SGOGroup, List<SGOStudent>>();
                if (ListGroups.Any() && ListSGOStudents.Any())
                {
                    foreach (var group in ListGroups)
                    {
                        var listStudent = ListSGOStudents.Where(x => x.SGOGroupID == group.SGOGroupID).ToList();
                        data.Add(group, listStudent);
                    }
                }
                return data;
            }
        }

        public Dictionary<int, List<ListItemStr>> DicScoreTypes { get; set; }

        public List<DataPointInfoViewModel> ListDataPointInfo
        {
            get
            {
                var preAssessmentDataPoint = ListDataPoint.Where(
                    x =>
                        x.SGOID == SGOID
                        && x.Type != (int)SGODataPointTypeEnum.PostAssessment
                        && x.Type != (int)SGODataPointTypeEnum.PostAssessmentToBeCreated
                        && x.Type != (int)SGODataPointTypeEnum.PostAssessmentCustom
                        && x.Type != (int)SGODataPointTypeEnum.PostAssessmentExternal
                        && x.Type != (int)SGODataPointTypeEnum.PostAssessmentHistorical
                        && (x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom
                            && (x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA1
                                || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA2
                                || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA3
                                || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA4)) == false
                    ).ToList();
                var sumWeight = preAssessmentDataPoint.Any()
                    ? preAssessmentDataPoint.Select(x => x.Weight).Sum()
                    : 0;
                var listDataPointInfo = ListDataPoint.Select(x => new DataPointInfoViewModel
                {
                    SGOID = x.SGOID,
                    Name = x.Name,
                    SGODataPointID = x.SGODataPointID,
                    Weight = x.Weight,
                    WeightPercent = (sumWeight == 0 || x.Weight == 0) ? 0 : x.Weight * 100 / sumWeight,
                    IsCustomCutScore = x.IsCustomCutScore,
                    Type = x.Type,
                    ScoreType = SGOHelper.GetDataPointScoreType(x),
                    ScoreTypesList = DicScoreTypes.ContainsKey(x.SGODataPointID) ? DicScoreTypes[x.SGODataPointID] : new List<ListItemStr>(),
                    VirtualTestSubScoreId = x.VirtualTestCustomScoreId.GetValueOrDefault()
                }).ToList();
                foreach (var dataPointInfoViewModel in listDataPointInfo)
                {
                    List<SGOStudentScoreInDataPointViewModel> studentScoreInfo;
                    if (ListStudentScoreInDataPoint != null
                        &&
                        ListStudentScoreInDataPoint.TryGetValue(dataPointInfoViewModel.SGODataPointID,
                            out studentScoreInfo))
                    {
                        if (studentScoreInfo.Any())
                        {
                            dataPointInfoViewModel.TotalQuestions = studentScoreInfo.Max(x => x.TotalQuestions);
                            dataPointInfoViewModel.TotalStudentHasScore =
                                studentScoreInfo.Where(x => x.Score.HasValue && x.Score.Value >= 0)
                                    .Select(x => x.StudentID)
                                    .Distinct()
                                    .Count();
                        }
                        else
                        {
                            dataPointInfoViewModel.TotalQuestions = -1;
                            dataPointInfoViewModel.TotalStudentHasScore = 0;
                        }
                    }
                    else
                    {
                        dataPointInfoViewModel.TotalQuestions = -1;
                        dataPointInfoViewModel.TotalStudentHasScore = 0;
                    }
                    var toolTip = new StringBuilder();
                    if (dataPointInfoViewModel.TotalQuestions > 0)
                    {
                        toolTip.AppendFormat("Items: {0}<br />", dataPointInfoViewModel.TotalQuestions);
                    }
                    toolTip.AppendFormat("Students: {0}", dataPointInfoViewModel.TotalStudentHasScore);
                    dataPointInfoViewModel.Tooltip = toolTip.ToString();
                    ////get ScoreType

                }
                return listDataPointInfo;
            }
        }

        public Dictionary<int, List<SGOStudentScoreInDataPointViewModel>> ListStudentScoreInDataPoint { get; set; }

        public bool ExistCustomDataPoint { get; set; }

        public EstablishStudentGroupViewModel()
        {
            ListStudents = new List<ListItemsViewModel>();
            ListDataPoint = new List<SGODataPoint>();
            ListGroups = new List<SGOGroup>();
            ListSGOStudents = new List<SGOStudent>();
        }
    }
}