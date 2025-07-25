using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.ETL
{
    public class MappingRuleHelper
    {
        public string CreateNewMappingRuleAndReturnXmlTransform(List<BaseMapping> mappings)
        {
            string xmlMappingRule = string.Empty;
            MappingRule predefinedMappingRule = new MappingRule();
            ETLXmlSerialization<MappingRule> mappingRuleSerialization = new ETLXmlSerialization<MappingRule>();
            CommonField commonField = new CommonField();
            commonField.MappingList = mappings;
            predefinedMappingRule.CommonField = commonField;
            xmlMappingRule = mappingRuleSerialization.SerializeObjectToXml(predefinedMappingRule);
            return xmlMappingRule;
        }

        public string CreateNewOrUpdatingCommonPhaseAndReturnXmlTransform(string currentXmlTransform, List<BaseMapping> mappings)
        {
            string xmlMappingRule = string.Empty;
            ETLXmlSerialization<MappingRule> mappingRuleSerialization = new ETLXmlSerialization<MappingRule>();
            MappingRule predefinedMappingRule = mappingRuleSerialization.DeserializeXmlToObject(currentXmlTransform);
            if (predefinedMappingRule.CommonField.IsNull())
            {
                predefinedMappingRule.CommonField = new CommonField();
                predefinedMappingRule.CommonField.MappingList = new List<BaseMapping>();
            }
            if (predefinedMappingRule.CommonField.MappingList.IsNull())
                predefinedMappingRule.CommonField.MappingList = new List<BaseMapping>();
            predefinedMappingRule.CommonField.MappingList = mappings;
            xmlMappingRule = mappingRuleSerialization.SerializeObjectToXml(predefinedMappingRule);
            return xmlMappingRule;
        }

        public string CreateNewTestPhaseAndReturnXmlTransform(string currentXmlTransform, List<BaseMapping> mappings, int phaseId)
        {
            string xmlMappingRule = string.Empty;
            ETLXmlSerialization<MappingRule> mappingRuleSerialization = new ETLXmlSerialization<MappingRule>();
            MappingRule predefinedMappingRule = mappingRuleSerialization.DeserializeXmlToObject(currentXmlTransform);
            if (predefinedMappingRule.TestList.IsNull())
                predefinedMappingRule.TestList = new List<TestMapping>();
            int maxPhase = 0;
            if (predefinedMappingRule.TestList.Count > 0)
                maxPhase = predefinedMappingRule.TestList.Max(t => t.ID);
            if (phaseId == maxPhase + 1)
            {
                TestMapping newTest = new TestMapping();
                newTest.ID = phaseId;
                newTest.MappingList = mappings;
                predefinedMappingRule.TestList.Add(newTest);
                xmlMappingRule = mappingRuleSerialization.SerializeObjectToXml(predefinedMappingRule);
            }
            return xmlMappingRule;
        }

        public string UpdatingTestPhaseAndReturnXmlTransform(string currentXmlTransform, List<BaseMapping> mappings, int phaseId)
        {
            string xmlMappingRule = string.Empty;
            ETLXmlSerialization<MappingRule> mappingRuleSerialization = new ETLXmlSerialization<MappingRule>();
            MappingRule predefinedMappingRule = mappingRuleSerialization.DeserializeXmlToObject(currentXmlTransform);
            if (predefinedMappingRule.IsNotNull())
            {
                if (predefinedMappingRule.IsNull())
                    predefinedMappingRule.TestList = new List<TestMapping>();
                TestMapping testMapping = predefinedMappingRule.TestList.Find(t => t.ID == phaseId);
                if (testMapping.IsNotNull())
                {
                    testMapping.MappingList = mappings;
                    xmlMappingRule = mappingRuleSerialization.SerializeObjectToXml(predefinedMappingRule);
                }
            }
            return xmlMappingRule;
        }

        public MappingRule GetMappingRuleFromXmlTransform(string xmlTransform)
        {
            MappingRule mappingRule = new MappingRule();
            ETLXmlSerialization<MappingRule> xmlSerialization = new ETLXmlSerialization<MappingRule>();
            mappingRule = xmlSerialization.DeserializeXmlToObject(xmlTransform);
            return mappingRule;
        }

        public string GetXmlTransformFromMappingRule(MappingRule mappingRule)
        {
            ETLXmlSerialization<MappingRule> xmlSerialization = new ETLXmlSerialization<MappingRule>();
            return xmlSerialization.SerializeObjectToXml(mappingRule);
        }

        public string DeleteMappingPhaseAndReturnXmlTransform(MappingRule mappingRule, int deletedPhaseId)
        {
            if (mappingRule.IsNull())
                return string.Empty;
            if (deletedPhaseId == 0)
            {
                mappingRule.CommonField = null;
            }
            else
            {
                DeleteTestPhaseFromMappingRule(mappingRule.TestList, deletedPhaseId);
            }
            return GetXmlTransformFromMappingRule(mappingRule);
        }

        private void DeleteTestPhaseFromMappingRule(List<TestMapping> testMappingList, int testPhaseId)
        {
            if (testMappingList.IsNull())
                return;
            if (testPhaseId <= 0)
                return;
            testMappingList.RemoveAll(t => t.ID == testPhaseId);
            foreach (var testMapping in testMappingList)
            {
                if (testMapping.ID > testPhaseId)
                    testMapping.ID--;
            }
        }
    }
}