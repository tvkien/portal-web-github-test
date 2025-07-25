using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class ETLTransformEngine
    {
        public string Transform(MappingRule mappingRule, string sourceFileContent)
        {
            if (mappingRule == null)
            {
                return string.Empty;
            }

            string[] fileLines = Regex.Split(sourceFileContent, Environment.NewLine);
            StringBuilder destinationContentBuilder = new StringBuilder();
            destinationContentBuilder.Append(CreateHeaderRow(mappingRule));
            for (int i = 1; i < fileLines.Length; i++)
            {
                if (!string.IsNullOrEmpty(fileLines[i]))
                {
                    string destinationRows = TransformSourceRowToDestinationRows(fileLines[i], mappingRule);
                    destinationContentBuilder.Append(destinationRows);
                }
            }
            return destinationContentBuilder.ToString();
        }

        private string CreateHeaderRow(MappingRule mappingRule)
        {
            string commonHeader = BuilderHeaderFromCommonMapping(mappingRule.CommonField);
            string testHeader = BuilderHeaderFromTestMapping(mappingRule.TestList);
            string rowData = string.Format("{0}{1}", commonHeader, testHeader);
            if (rowData.Length > 0)
                rowData = rowData.Substring(0, rowData.Length - 1);
            return string.Format("{0}{1}", rowData, Environment.NewLine);
        }

        private string BuilderHeaderFromCommonMapping(CommonField commonfield)
        {
            if (commonfield.IsNull() || commonfield.MappingList.IsNull())
                return string.Empty;
            return BuildHeaderFromMappingList(commonfield.MappingList);
        }

        private string BuilderHeaderFromTestMapping(List<TestMapping> testMappingList)
        {
            if (testMappingList.IsNull() || testMappingList.Count <= 0 || testMappingList[0].IsNull() || testMappingList[0].MappingList.IsNull())
                return string.Empty;
            return BuildHeaderFromMappingList(testMappingList[0].MappingList);
        }

        private string TransformSourceRowToDestinationRows(string sourceRow, MappingRule mappingRule)
        {
            StringBuilder mappingContentBuilder = new StringBuilder();
            StringBuilder finalContentBuilder = new StringBuilder();
            string[] lineData = sourceRow.Split("\t".ToCharArray());
            string commonMapping = string.Empty;

            BuildContentFromMappingList(mappingContentBuilder, lineData, mappingRule.CommonField.MappingList);
            commonMapping = mappingContentBuilder.ToString();            
            foreach (TestMapping test in mappingRule.TestList)
            {
                mappingContentBuilder = new StringBuilder();
                BuildContentFromMappingList(mappingContentBuilder, lineData, test.MappingList);
                string rowData = string.Format("{0}{1}",commonMapping, mappingContentBuilder.ToString());
                if (rowData.Length > 0)
                    rowData = rowData.Substring(0, rowData.Length - 1);
                finalContentBuilder.AppendFormat("{0}{1}", rowData, Environment.NewLine);                
            }            
            return finalContentBuilder.ToString();
        }

        private string BuildHeaderFromMappingList(List<BaseMapping> mappingList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var mapping in mappingList)
            {
                sb.AppendFormat("{0}\t",mapping.Destination);
            }            
            return sb.ToString();
        }

        private void BuildContentFromMappingList(StringBuilder builder, string[] lineData, List<BaseMapping> listMappings)
        {
            foreach (BaseMapping mapping in listMappings)
            {
                if (mapping.GetType() == typeof(PrefixMapping))
                {
                    ProcessPrefixMapping(builder, lineData, (PrefixMapping)mapping);
                }
                else if (mapping.GetType() == typeof(SourceColumnMapping))
                {
                    ProcessSourceColumnMapping(builder, lineData, (SourceColumnMapping)mapping);
                }
                else if (mapping.GetType() == typeof(LookupMapping))
                {
                    ProcessLookupMapping(builder, lineData, (LookupMapping)mapping);
                }
                else if (mapping.GetType() == typeof(FixedValueMapping))
                {
                    ProcessFixedValueMapping(builder, lineData, (FixedValueMapping)mapping);
                }
            }
        }

        private void ProcessPrefixMapping(StringBuilder builder, string[] lineData, PrefixMapping mapping)
        {
            if (lineData.Length > mapping.SourcePosition)
            {
                string content = mapping.Prefix + lineData[mapping.SourcePosition]; //TODO: need to check if sourceposition larger than array length
                builder.AppendFormat("{0}\t", content);
            }
        }

        private void ProcessSourceColumnMapping(StringBuilder builder, string[] lineData, SourceColumnMapping mapping)
        {
            if (lineData.Length > mapping.SourcePosition)
            {
                string content = lineData[mapping.SourcePosition];
                builder.AppendFormat("{0}\t", content);
            }
        }

        private void ProcessLookupMapping(StringBuilder builder, string[] lineData, LookupMapping mapping)
        {
            if (lineData.Length > mapping.SourcePosition)
            {
                string source = lineData[mapping.SourcePosition];
                LookupData data = mapping.LookupValue.FirstOrDefault(d => d.Existing.Equals(source));
                string content = data != null ? data.New : string.Empty;
                builder.AppendFormat("{0}\t", content);
            }
        }

        private void ProcessFixedValueMapping(StringBuilder builder, string[] lineData, FixedValueMapping mapping)
        {
            string content = mapping.Value;
            builder.AppendFormat("{0}\t", content);
        }
    }
}