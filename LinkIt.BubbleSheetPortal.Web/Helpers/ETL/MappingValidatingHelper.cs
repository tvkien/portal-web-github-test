using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.ETL
{
    public class MappingValidatingHelper
    {
        private const string InvalidMapping = "Mapping does not exist.";
        private const string InvalidDestinationColumnError = "destination column is invalid";
        private const string EmptyDestinationColumnError = "destination column is not selected.";
        private const string InvalidSourceColumnError = "invalid source column selection.";
        private const string EmptySourceColumnError = "source column is not selected.";
        private const string EmptyPrefixValueError = "prefix value is not specified.";
        private const string EmptyFixedValueError = "fixed value is not specified.";
        private const string UnknownFixedValueError = "fixed value has unknown value.";
        private const string EmptyLookupValueError = "lookup mapping is not specified.";
        private const string EmptyExistingValueError = "lookup’s existing value is not specified.";
        private const string EmptyNewValueError = "lookup’s new value is not specified.";
        private const string UnknownNewValueError = "lookup’s new value has unknown value.";
        private const string InvalidMappingRuleError = "stored mapping is invalid.";
        private const string CommonPhaseLostError = "Mapping does not have common mappings.";
        private const string TestPhaseLostError = "Mapping does not have any test mappings.";
        private const string InvalidTestPhaseError = "Test {0} mapping is not valid.";
        private const string NotAllRequiredColumnIsMappedError = "Mapping for '{0}' destination column is required.";
        private const string UnknownValue = "unknown";
        private bool isCheckUnknownValue;

        public bool IsCheckUnknownValue 
        { 
            get { return isCheckUnknownValue; }
            set { isCheckUnknownValue = value; }
        }
 
        public string ValidateMappingRule(MappingRule mappingRule, List<StandardColumn> commonRequiredColumns, List<StandardColumn> testRequiredColumns)
        {
            if (mappingRule.IsNull())
                return InvalidMappingRuleError;
            if (mappingRule.CommonField.IsNull() || mappingRule.CommonField.MappingList.IsNull() || mappingRule.CommonField.MappingList.Count == 0)
                return CommonPhaseLostError;
            if (mappingRule.TestList.IsNull() || mappingRule.TestList.Count == 0)
                return TestPhaseLostError;
            string errorMessage = string.Empty;
            errorMessage = CommonPhaseValidate(mappingRule.CommonField.MappingList, commonRequiredColumns);
            if (!string.IsNullOrEmpty(errorMessage))
                return errorMessage;
            errorMessage = TestPhaseValidate(mappingRule.TestList, testRequiredColumns);
            return errorMessage;
        }

        private string CommonPhaseValidate(List<BaseMapping> mappings, List<StandardColumn> requiredColumns)
        {
            string errorMessage = MappingListWithRequiredColumnValidate(mappings, requiredColumns);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return string.Format("Common phase >> {0}", errorMessage);
            }
            return string.Empty;
        }

        private string TestPhaseValidate(List<TestMapping> testMappingList, List<StandardColumn> requiredColumns)
        {
            string errorMessage = string.Empty;
            for (int i = 0; i < testMappingList.Count; i++)
            {
                TestMapping testMapping = testMappingList[i];
                if (testMapping.IsNull())
                {
                    errorMessage = string.Format(InvalidTestPhaseError, i + 1);
                    break;
                }
                errorMessage = MappingListWithRequiredColumnValidate(testMapping.MappingList, requiredColumns);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = string.Format("Test phase {0} >> {1}", testMapping.ID, errorMessage);
                    break;
                }
            }
            return errorMessage;
        }

        public string MappingListWithRequiredColumnValidate(List<BaseMapping> mappings, List<StandardColumn> requiredColumns)
        {
            string errorMessage = string.Empty;          
            errorMessage = RequiredColumnsValidate(requiredColumns, mappings);
            if (string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = MappingListValidate(mappings);
            }
            return errorMessage;
        }

        private string RequiredColumnsValidate(List<StandardColumn> requiredColumns, List<BaseMapping> mappings)
        {
            string errorMessage = string.Empty;
            foreach (var colum in requiredColumns)
            {
                if (!mappings.Any(m => m.DestinationColumnID == colum.ColumnID))
                {
                    errorMessage = string.Format(NotAllRequiredColumnIsMappedError, colum.Name);
                    break;
                }
            }
            return errorMessage;
        }
        public string MappingListValidate(List<BaseMapping> mappings)
        {
            string errorMessage = string.Empty;
            for (int i = 0; i < mappings.Count; i++)
            {
                errorMessage = MappingValidate(mappings[i]);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = string.Format("Mapping row {0}: {1}", i + 1, errorMessage);
                    break;
                }
            }
            return errorMessage;
        }

        private string MappingValidate(BaseMapping mapping)
        {
            SourceColumnMapping sourceColumnMapping = mapping as SourceColumnMapping;
            if (sourceColumnMapping.IsNotNull())
                return SourceColumnMappingValidate(sourceColumnMapping);
            else
            {
                FixedValueMapping fixedValueMapping = mapping as FixedValueMapping;
                if (fixedValueMapping.IsNotNull())
                    return FixedValueMappingValidate(fixedValueMapping);
                else
                {
                    PrefixMapping prefixMapping = mapping as PrefixMapping;
                    if (prefixMapping.IsNotNull())
                        return PrefixMappingValidate(prefixMapping);
                    else
                    {
                        LookupMapping lookupMapping = mapping as LookupMapping;
                        if (lookupMapping.IsNotNull())
                            return LookupMappingValidate(lookupMapping);
                    }
                }
            }
            return InvalidMapping;
        }

        private string SourceColumnMappingValidate(SourceColumnMapping mapping)
        {
            if (mapping.IsNull())
                return InvalidMapping;
            if (mapping.DestinationColumnID < 0)
                return InvalidDestinationColumnError;
            if (string.IsNullOrEmpty(mapping.Destination))
                return EmptyDestinationColumnError;
            if (mapping.SourcePosition < 0)
                return InvalidSourceColumnError;
            if (string.IsNullOrEmpty(mapping.Source))
                return EmptySourceColumnError;
            return string.Empty;
        }

        private string FixedValueMappingValidate(FixedValueMapping mapping)
        {
            if (mapping.IsNull())
                return InvalidMapping;
            if (mapping.DestinationColumnID < 0)
                return InvalidDestinationColumnError;
            if (string.IsNullOrEmpty(mapping.Destination))
                return EmptyDestinationColumnError;
            if (isCheckUnknownValue)
            {
                if (mapping.Value.Trim().ToLower() == UnknownValue)
                    return UnknownFixedValueError;
            }            
            return string.Empty;
        }

        private string PrefixMappingValidate(PrefixMapping mapping)
        {
            if (mapping.IsNull())
                return InvalidMapping;
            if (mapping.DestinationColumnID < 0)
                return InvalidDestinationColumnError;
            if (string.IsNullOrEmpty(mapping.Destination))
                return EmptyDestinationColumnError;
            if (mapping.SourcePosition < 0)
                return InvalidSourceColumnError;
            if (string.IsNullOrEmpty(mapping.Source))
                return EmptySourceColumnError;
            if (string.IsNullOrEmpty(mapping.Prefix))
                return EmptyPrefixValueError;
            return string.Empty;
        }

        private string LookupMappingValidate(LookupMapping mapping)
        {
            if (mapping.IsNull())
                return InvalidMapping;
            if (mapping.DestinationColumnID < 0)
                return InvalidDestinationColumnError;
            if (string.IsNullOrEmpty(mapping.Destination))
                return EmptyDestinationColumnError;
            if (mapping.SourcePosition < 0)
                return InvalidSourceColumnError;
            if (string.IsNullOrEmpty(mapping.Source))
                return EmptySourceColumnError;
            if (mapping.LookupValue.IsNull() || mapping.LookupValue.Count == 0)
                return EmptyLookupValueError;
            return LookupValueValidate(mapping.LookupValue);
        }

        private string LookupValueValidate(List<LookupData> lookupValue)
        {
            string errorMessage = string.Empty;
            foreach (var lookupData in lookupValue)
            {
                if (string.IsNullOrEmpty(lookupData.Existing))
                {
                    errorMessage = EmptyExistingValueError;
                    break;
                }
                if (isCheckUnknownValue)
                {
                    if (string.IsNullOrEmpty(lookupData.New))
                    {
                        errorMessage = EmptyNewValueError;
                        break;
                    }
                    if (lookupData.New.Trim().ToLower() == UnknownValue)
                    {
                        errorMessage = UnknownNewValueError;
                        break;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(lookupData.New))
                    {
                        errorMessage = EmptyNewValueError;
                        break;
                    }
                }
            }
            return errorMessage;
        }
    }
}