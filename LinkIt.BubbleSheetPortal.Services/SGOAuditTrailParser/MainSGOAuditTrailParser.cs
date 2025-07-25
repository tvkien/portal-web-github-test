using System.Collections.Generic;
using System.Linq;
using System.Xml;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services.SGOAuditTrailParser
{
    public class MainSGOAuditTrailParser
    {
        private List<int> _studentIDs;
        private List<int> _groupIDs;
        private List<int> _sgoDataPointIDs;

        public List<int> StudentIDs
        {
            get
            {
                if(_studentIDs == null)_studentIDs = new List<int>();
                return _studentIDs;
            }
            set { _studentIDs = value; }
        }

        public List<int> GroupIDs
        {
            get
            {
                if (_groupIDs == null) _groupIDs = new List<int>();
                return _groupIDs;
            }
            set { _groupIDs = value; }
        }

        public List<int> SGODataPointIDs
        {
            get
            {
                if (_sgoDataPointIDs == null) _sgoDataPointIDs = new List<int>();
                return _sgoDataPointIDs;
            }
            set { _sgoDataPointIDs = value; }
        } 

        public void Parse(List<SGOAuditTrailSearchItem> auditTrails)
        {
            var sgoAuditTrails =
                auditTrails.Where(o => o.SourceOfData == "SGOAuditTrail" && o.ActionType != null).ToList();
            foreach (var auditTrail in sgoAuditTrails)
            {
                if (auditTrail.ActionType == 1 || auditTrail.ActionType == 2)
                {
                    auditTrail.GroupIDs = ParseAuditTrailToGetGroupIDs(auditTrail.Details);
                    GroupIDs.AddRange(auditTrail.GroupIDs);

                    auditTrail.StudentIDs = ParseAuditTrailToGetStudentIDs(auditTrail.Details);
                    StudentIDs.AddRange(auditTrail.StudentIDs);
                }
                else if (auditTrail.ActionType == 3)
                {
                    auditTrail.SGODataPointIDs = ParseAuditTrailToGetDataPointIDs(auditTrail.Details);
                    SGODataPointIDs.AddRange(auditTrail.SGODataPointIDs);
                }
            }
        }

        public List<int> ParseAuditTrailToGetStudentIDs(string xml)
        {
            var result = new List<int>();

            var xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            var studentIDs = xdoc.GetElementsByTagName("studentid");
            if (studentIDs.Count > 0)
            {
                foreach (XmlNode studentIDNode in studentIDs)
                {                    
                    result.Add(studentIDNode.InnerText.Contains(";")
                        ? int.Parse(studentIDNode.InnerText.Split(';')[0]) // in case store studentid and classid at at student in prepairnessed group page[eg: 123;321]
                        : int.Parse(studentIDNode.InnerText));
                }
            }

            return result;
        }

        public List<int> ParseAuditTrailToGetGroupIDs(string xml)
        {
            var result = new List<int>();

            var xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            var groups = xdoc.GetElementsByTagName("group");
            if (groups.Count > 0)
            {
                foreach (XmlNode groupNode in groups)
                {
                    if (groupNode.Attributes == null) continue;
                    var groupID = groupNode.Attributes["id"];
                    if (groupID == null) continue;
                    result.Add(int.Parse(groupID.Value));
                }
            }

            return result;
        }

        public List<int> ParseAuditTrailToGetDataPointIDs(string xml)
        {
            var result = new List<int>();

            var xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            var dataPoints = xdoc.GetElementsByTagName("datapointid");
            if (dataPoints.Count > 0)
            {
                foreach (XmlNode dataPointNode in dataPoints)
                {
                    result.Add(int.Parse(dataPointNode.InnerText));
                }
            }

            return result;
        }
    }
}
