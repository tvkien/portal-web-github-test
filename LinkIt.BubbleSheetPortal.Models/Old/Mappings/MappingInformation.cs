using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using System.Text.RegularExpressions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class MappingInformation : ValidatableEntity<MappingInformation>
    {
        private const int maxSampleRows = 15;
        private string name = string.Empty;
        private string sourceFileContent = string.Empty;
        private string xmlTransform = string.Empty;
        private DateTime createdDate;
        private DateTime lastestModifiedDate;

        public int MapID { get; set; }


        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public MappingLoaderType LoaderType { get; set; }

        public MappingProgressStatus ProgressStatus { get; set; }

        public string SourceFileContent
        {
            get { return sourceFileContent; }
            set { sourceFileContent = value; }
        }

        public string XmlTransform
        {
            get { return xmlTransform; }
            set { xmlTransform = value.ConvertNullToEmptyString(); }
        }

        public int UserID { get; set; }

        public DateTime CreatedDate
        {
            get { return createdDate; }
            set { createdDate = value; }
        }

        public DateTime LastestModifiedDate
        {
            get { return lastestModifiedDate; }
            set { lastestModifiedDate = value; }
        }

        public Dictionary<int, string> GetSourceColumnList()
        {
            Dictionary<int, string> columnList = new Dictionary<int, string>();

            if (!string.IsNullOrEmpty(sourceFileContent))
            {
                string header = Regex.Split(sourceFileContent, Environment.NewLine)[0];
                string[] columns = header.Split('\t');
                for (int i = 0; i < columns.Length; i++)
                {
                    columnList.Add(i, columns[i]);
                }
            }

            return columnList;
        }

        public List<string> GetSampleData(int indexColumn)
        {
            List<string> sampleDataList = new List<string>();
            if (!string.IsNullOrEmpty(sourceFileContent))
            {
                string[] lines = Regex.Split(sourceFileContent, Environment.NewLine);
                string header = lines[0];
                List<string> columnList = header.Split('\t').ToList();
                if (indexColumn != -1)
                {
                    int maxSampleRowCount = lines.Length > maxSampleRows ? maxSampleRows : lines.Length - 1;
                    for (int i = 1; i <= maxSampleRowCount; i++)
                    {
                        string[] lineContents = lines[i].Split('\t');
                        if (lineContents.Length > indexColumn && lineContents[indexColumn].IsNotNull())
                        {
                            sampleDataList.Add(GetShortString(lineContents[indexColumn], 23));
                        }                        
                    }
                }
            }
            return sampleDataList;
        }

        private string GetShortString(string input, int maxLength)
        {
            string shortData = input;
            if (input.Length > maxLength)
            {
                shortData = input.Substring(0, maxLength);
                int lastSpace = shortData.LastIndexOf(' ');
                if (lastSpace > 0)
                    shortData = shortData.Substring(0, lastSpace);
                if (!string.IsNullOrEmpty(input))
                    shortData += " ...";
            }
            
            return shortData;
        }

        public List<string> GetUniqueValues(int indexColumn)
        {
            List<string> uniqueValuesList = new List<string>();
            if (!string.IsNullOrEmpty(sourceFileContent))
            {
                string[] lines = Regex.Split(sourceFileContent, Environment.NewLine);
                string header = lines[0];
                List<string> columnList = header.Split('\t').ToList();
                if (indexColumn != -1)
                {
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] lineContents = lines[i].Split('\t');
                        if (lineContents.Length > indexColumn && lineContents[indexColumn].IsNotNull() && !string.IsNullOrEmpty(lineContents[indexColumn].Trim()) && !uniqueValuesList.Contains(lineContents[indexColumn]))
                        {
                            uniqueValuesList.Add(lineContents[indexColumn]);
                        }
                    }
                }
            }
            uniqueValuesList.Sort((x, y) =>
            {
                int ix, iy;
                return int.TryParse(x, out ix) && int.TryParse(y, out iy)
                      ? ix.CompareTo(iy) : string.Compare(x, y);
            });
            return uniqueValuesList;
        }
    }
}
