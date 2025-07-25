using System;
using System.Collections.Generic;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTIItemDataForDataTable
    {
        public QTIItemDataForDataTable()
        {
            ToolTip = string.Empty;
        }
        public int QuestionOrder { get; set; }
        public int QTIItemID { get; set; }
        public string Title { get; set; }
        public int VirtualTestCount { 
            get
            {
                if (_testList != null && _testList.Count > 0)
                {
                    return _testList.Count;
                }
                else
                {
                    return 0;
                }
            }
        }
        public string TestDisplay
        {
            get
            {
                if (_testList != null && _testList.Count > 0)
                {
                    StringBuilder testDisplay = new StringBuilder();

                    for (int i = 0; i < _testList.Count; i++)
                    {
                        testDisplay.Append(_testList[i].Name);
                        if (i < _testList.Count - 1)
                        {
                            testDisplay.Append(", ");
                        }
                    }
                    return testDisplay.ToString();
                }
                else
                {

                    return string.Empty;
                }
            }
        }
        public string ToolTip { get; set; }
        public int MaxItemTooltipLength
        {
            get
            {
                if(ToolTip==null)
                {
                    ToolTip = string.Empty;
                }
                string[] lines = ToolTip.Split(new string[] { "<br>"}, StringSplitOptions.None);
                int maxLength = 0;
                foreach (var line in lines)
                {
                    if (line.Length > maxLength)
                    {
                        maxLength = line.Length;
                    }
                }
                return maxLength;
            }
        }
        private List<VirtualTestData> _testList;
        public List<VirtualTestData> TestList
        {
            get { return _testList; }
            set { _testList = value; }
        }
    }
}