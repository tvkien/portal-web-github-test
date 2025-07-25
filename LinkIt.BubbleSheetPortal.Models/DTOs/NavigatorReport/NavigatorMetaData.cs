using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorMetaData
    {
        public int FromPageIndex { get; set; }
        public int ToPageIndex { get; set; }
        public string SchoolName { get; set; }
        public string TeacherUserName { get; set; }
        public string ClassName { get; set; }
        public string CurrentGrade { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string StudentGenger { get; set; }

        internal static NavigatorMetaData InitFingerTipFromListString(List<string> firstFiveLines)
        {
            try
            {
                NavigatorMetaData res = new NavigatorMetaData();
                if (firstFiveLines[0].StartsWith("Student Name:", StringComparison.OrdinalIgnoreCase))
                {
                    //Student Name: Bryson, Taraji Z School: Green Grove Elementary School

                    string[] splt = firstFiveLines[0].Split(new String[] { "School:" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splt.Length == 2)
                    {
                        res.StudentName = splt[0].Substring(splt[0].IndexOf(':') + 1).Trim();
                        res.SchoolName = splt[1].Trim();
                    }
                    else
                    {
                        return null;
                    }
                    // process Student Name, School
                    if (firstFiveLines[1].StartsWith("Student ID:", StringComparison.OrdinalIgnoreCase))
                    {
                        // process Student Id, Teacher
                        splt = firstFiveLines[1].Split(new String[] { "Teacher:" }, StringSplitOptions.RemoveEmptyEntries);
                        res.StudentCode = splt[0].Substring(splt[0].IndexOf(':') + 1).Trim();
                        // teacher: teachername (username)]

                        if (splt.Length >= 2)
                        {
                            string _splt1 = splt[1].Trim();
                            if (_splt1.Contains("("))
                            {
                                string _teacherUserName = _splt1.Substring(_splt1.LastIndexOf('(') + 1).Replace(")", "");

                                res.TeacherUserName = _teacherUserName;
                            } 
                        }
                        return res;

                        //if (firstFiveLines[2].StartsWith("Current Grade:", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    // process Current Grade, Class
                        //    splt = firstFiveLines[2].Split(new String[] { "Class:" }, StringSplitOptions.RemoveEmptyEntries);
                        //    res.CurrentGrade = splt[0].Substring(splt[0].IndexOf(':') + 1).Trim();
                        //    if (splt.Length >= 2)
                        //        res.ClassName = splt[1].Trim();

                        //    if (firstFiveLines[3].StartsWith("Student Gender:", StringComparison.OrdinalIgnoreCase))
                        //    {
                        //        // process Student Gender ~> don't need to
                        //        res.StudentGenger = firstFiveLines[3].Substring(firstFiveLines[3].IndexOf(':') + 1).Trim();
                        //        if (firstFiveLines[4].StartsWith("Student Race:", StringComparison.OrdinalIgnoreCase))
                        //        {
                        //            // process Student Race
                        //            return res;
                        //        }
                        //    }
                        //}
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static BaseResponseModel<List<NavigatorMetaData>> InitFromFingerTipPagesData(Dictionary<int, string> textPerPageData)
        {
            NavigatorMetaData currFingerTip = null;
            var fingertipMetaDatas = BaseResponseModel<List<NavigatorMetaData>>.InstanceSuccess(new List<NavigatorMetaData>());
            for (int i = 1; i <= textPerPageData.Count; i++)
            {
                string _val = textPerPageData[i];
                List<string> firstFiveLines = _val.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Take(5).ToList();
                // check condition
                if (firstFiveLines.Count == 5)
                {
                    NavigatorMetaData fingertipMetaData = InitFingerTipFromListString(firstFiveLines);
                    if (fingertipMetaData != null)
                    {
                        fingertipMetaData.FromPageIndex = i;
                        fingertipMetaData.ToPageIndex = i;
                        fingertipMetaDatas.StrongData.Add(fingertipMetaData);
                    }
                    else
                    {
                        if (currFingerTip != null)
                            currFingerTip.ToPageIndex += 1;
                        else
                            return null;
                    }

                }
                else
                {
                    if (currFingerTip != null)
                        currFingerTip.ToPageIndex += 1;
                }

            }
            return fingertipMetaDatas;
        }

        public static BaseResponseModel<List<NavigatorMetaData>> InitTeacherSlidesFromPagesData(Dictionary<int, string> textPerPageData)
        {
            NavigatorMetaData currFingerTip = null;
            var fingertipMetaDatas = BaseResponseModel<List<NavigatorMetaData>>.InstanceSuccess(new List<NavigatorMetaData>());
            for (int i = 1; i <= textPerPageData.Count; i++)
            {
                string _val = textPerPageData[i];
                List<string> firstFourLines = _val.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Take(4).ToList();
                // check condition
                if (firstFourLines.Count == 4)
                {
                    NavigatorMetaData fingertipMetaData = InitTeacherSlidesFromListString(firstFourLines);
                    if (fingertipMetaData != null)
                    {
                        fingertipMetaData.FromPageIndex = i;
                        fingertipMetaData.ToPageIndex = i;
                        fingertipMetaDatas.StrongData.Add(fingertipMetaData);
                    }
                    else
                    {
                        if (currFingerTip != null)
                            currFingerTip.ToPageIndex += 1;
                        return null;
                    }

                }
                else
                {
                    if (currFingerTip != null)
                        currFingerTip.ToPageIndex += 1;
                }

            }
            return fingertipMetaDatas;
        }

        private static NavigatorMetaData InitTeacherSlidesFromListString(List<string> firstFourLines)
        {
            string _splt1 = firstFourLines[3].Trim();
            if (_splt1.Contains("("))
            {
                string _teacherUserName = _splt1.Substring(_splt1.LastIndexOf('(') + 1).Replace(")", "");
                var res = new NavigatorMetaData()
                {
                    TeacherUserName = _teacherUserName,
                    ClassName = firstFourLines[2],
                    SchoolName = firstFourLines[1]
                };
                return res;
            }
            return null;
        }
    }
}
