
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorPathwayDto
    {
        [JsonProperty("path")]
        public string Path { get; set; } = string.Empty;
    }

    public class NavigatorReportDto
    {
        public string NodePath { get; set; }
        public string Name { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string DocumentType { get; set; }
    }

    public abstract class NavigatorReportTreeDto
    {
        protected NavigatorReportTreeDto()
        {
            Children = new List<NavigatorReportTreeDto>();
        }

        public string Name { get; set; }
        public NavigatorReportTreeDto Parent { get; set; }
        public IList<NavigatorReportTreeDto> Children { get; set; }

        public string OriginalNodePath => string.Join("/", FindAllParentNodes().Where(x => !string.IsNullOrEmpty(x.GetUniqueKeyInFolder())).Select(x => $"{{{x.GetUniqueKeyInFolder()}}}"));
        public string NodePath => Uri.EscapeDataString(OriginalNodePath);
        public abstract bool IsFolder();
        public abstract bool IsLeaf();
        public abstract bool IsFile();
        public abstract List<NavigatorReportFileNodeDto> FindAllLeafNodes();
        public abstract bool IsSchoolFolder();
        public abstract DateTime? GetLastModifiedDate();
        public abstract string GetUniqueKeyInFolder();
        public abstract string GetDocumentType();
        public List<NavigatorReportTreeDto> FindAllParentNodes()
        {
            var nodes = new List<NavigatorReportTreeDto>();
            if (Parent != null)
                nodes.AddRange(Parent.FindAllParentNodes());
            nodes.Add(this);
            return nodes;
        }
        public NavigatorReportTreeDto FindNodeByPath(string nodePath)
        {
            if(NodePath == nodePath)
                return this;

            foreach (var child in Children)
            {
                var node = child.FindNodeByPath(nodePath);
                if (node != null)
                    return node;
            }
            return null;
        }
        public abstract bool IsYearFolder();
        public IEnumerable<NavigatorReportTreeDto> GetChildren()
        {
            if (Children.All(x => x.IsYearFolder()))
                return Children.OrderByDescending(x => x.Name);
            return Children.OrderBy(x => x.Name);
        }
    }

    public class NavigatorReportFolderNodeDto : NavigatorReportTreeDto
    {
        public NavigatorKeywords NavigatorKeywords { get; set; }

        public override bool IsFolder() => true;
        public override bool IsLeaf() => false;
        public override bool IsFile() => false;
        public override List<NavigatorReportFileNodeDto> FindAllLeafNodes()
        {
            var leafNodes = new List<NavigatorReportFileNodeDto>();
            foreach (var child in Children)
            {
                leafNodes.AddRange(child.FindAllLeafNodes());
            }
            return leafNodes;
        }
        public override DateTime? GetLastModifiedDate()
        {
            var leafNodes = FindAllLeafNodes();
            return leafNodes.Max(x => x.GetLastModifiedDate());
        }
        public override string GetUniqueKeyInFolder() => Name;
        public override string GetDocumentType() => "Folder";
        public override bool IsSchoolFolder()
        {
            var parents = FindAllParentNodes();
            var navigatorKeywords = parents
                .Select(x => x is NavigatorReportFolderNodeDto folder ? folder.NavigatorKeywords : NavigatorKeywords.None)
                .Aggregate((item, next) => item | next);

            if (!navigatorKeywords.HasFlag(NavigatorKeywords.School) || !navigatorKeywords.HasFlag(NavigatorKeywords.Category))
                return false;

            var leafNodes = FindAllLeafNodes();
            var numberOfSchool = leafNodes.Where(x => x.SchoolId.HasValue).DistinctBy(x => x.SchoolId).Count();
            var numberOfCategory = leafNodes.Where(x => x.NavigatorReportCategoryId.HasValue).DistinctBy(x => x.NavigatorReportCategoryId).Count();

            if (numberOfSchool != 1 || numberOfCategory != 1)
                return false;
            return true;
        }
        public override bool IsYearFolder() => NavigatorKeywords.HasFlag(NavigatorKeywords.Year);
    }

    public class NavigatorReportFileNodeDto : NavigatorReportTreeDto
    {
        public int NavigatorReportId { get; set; }
        public int? NavigatorConfigurationID { get; set; }
        public int? NavigatorReportCategoryId { get; set; }
        public int? SchoolId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string S3FileFullName { get; set; }

        public override string GetDocumentType()
        {
            var extensionDefinition = new Dictionary<string, string> {
                {  ".pdf",  "PDF" },
                {  ".xlsx", "Excel" },
                {  ".xls",  "Excel" },
                {  ".ppt",  "PowerPoint" },
                {  ".pptx", "PowerPoint" }
            };
            var extension = Path.GetExtension(S3FileFullName.Replace("\n", "").Replace("\r", ""))?.ToLower();
            return extensionDefinition.TryGetValue(extension, out string value) ? value : "Folder";
        }
        public override bool IsFolder() => false;
        public override bool IsLeaf() => true;
        public override bool IsFile() => true;
        public override List<NavigatorReportFileNodeDto> FindAllLeafNodes()
        {
            return new List<NavigatorReportFileNodeDto> { this };
        }
        public override DateTime? GetLastModifiedDate() => LastModifiedDate;
        public override string GetUniqueKeyInFolder() => NavigatorReportId.ToString();
        public override bool IsSchoolFolder() => false;
        public override bool IsYearFolder() => false;
    }
}
