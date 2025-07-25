using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.ETL
{
    public class MappingTransferHelper
    {
        public List<MappingRowTransfer> GenerateMappingTransferByPhase(MappingRule mappingRule, int phaseId)
        {
            if (mappingRule.IsNull())
                return null;
            List<MappingRowTransfer> mappingTransferList = null;
            if (phaseId == 0)
            {
                if (mappingRule.CommonField.IsNotNull() && mappingRule.CommonField.MappingList.IsNotNull() && mappingRule.CommonField.MappingList.Count > 0)
                    mappingTransferList = GenerateMappingTransferFromMappingList(mappingRule.CommonField.MappingList);
            }
            else
            {
                if (mappingRule.TestList.IsNull())
                    return null;
                TestMapping testMapping = mappingRule.TestList.Find(t => t.ID == phaseId);
                if (testMapping.IsNotNull())
                {
                    mappingTransferList = GenerateMappingTransferFromMappingList(testMapping.MappingList);
                }
            }
            return mappingTransferList;
        }

        private List<MappingRowTransfer> GenerateMappingTransferFromMappingList(List<BaseMapping> mappingList)
        {
            List<MappingRowTransfer> transferList = new List<MappingRowTransfer>();
            MappingRowTransfer transferedMapping = null;
            foreach (BaseMapping mapping in mappingList)
            {
                transferedMapping = GenerateTransferSourceColumnMapping(mapping);
                if (transferedMapping.IsNull())
                {
                    transferedMapping = GenerateTransferFixedValueMapping(mapping);
                    if (transferedMapping.IsNull())
                    {
                        transferedMapping = GenerateTransferPrefixMapping(mapping);
                        if (transferedMapping.IsNull())
                        {
                            transferedMapping = GenerateTransferLookupMapping(mapping);
                        }
                    }
                }
                transferList.Add(transferedMapping);
            }
            return transferList;
        }

        private MappingRowTransfer GenerateTransferSourceColumnMapping(BaseMapping baseMapping)
        {
            var specifiedMapping = baseMapping as SourceColumnMapping;
            MappingRowTransfer transferedSourceMapping = null;
            if (specifiedMapping.IsNotNull())
            {
                transferedSourceMapping = new MappingRowTransfer
                                              {
                    Source = specifiedMapping.Source,
                    SourcePosition = specifiedMapping.SourcePosition,
                    Destination = specifiedMapping.Destination,
                    DestinationColumnID = specifiedMapping.DestinationColumnID,
                    MappingType = ((int)MappingDestinationColumnTypes.SourceColumn).ToString(),
                    MappingTypeName = "Source Column",
                    MappingValue = ""
                };
            }
            return transferedSourceMapping;
        }

        private MappingRowTransfer GenerateTransferFixedValueMapping(BaseMapping baseMapping)
        {
            var specifiedMapping = baseMapping as FixedValueMapping;
            MappingRowTransfer transferedFixedValueMapping = null;
            if (specifiedMapping.IsNotNull())
            {
                transferedFixedValueMapping = new MappingRowTransfer
                                                  {
                    Source = specifiedMapping.Source,
                    SourcePosition = specifiedMapping.SourcePosition,
                    Destination = specifiedMapping.Destination,
                    DestinationColumnID = specifiedMapping.DestinationColumnID,
                    MappingType = ((int)MappingDestinationColumnTypes.FixedValue).ToString(),
                    MappingTypeName = "Fixed Value",
                    MappingValue = specifiedMapping.Value
                };
            }
            return transferedFixedValueMapping;
        }

        private MappingRowTransfer GenerateTransferPrefixMapping(BaseMapping baseMapping)
        {
            var specifiedMapping = baseMapping as PrefixMapping;
            MappingRowTransfer transferedPrefixMapping = null;
            if (specifiedMapping.IsNotNull())
            {
                transferedPrefixMapping = new MappingRowTransfer
                                              {
                    Source = specifiedMapping.Source,
                    SourcePosition = specifiedMapping.SourcePosition,
                    Destination = specifiedMapping.Destination,
                    DestinationColumnID = specifiedMapping.DestinationColumnID,
                    MappingType = ((int)MappingDestinationColumnTypes.PrefixColumn).ToString(),
                    MappingTypeName = "Prefix + Source Column",
                    MappingValue = specifiedMapping.Prefix
                };
            }
            return transferedPrefixMapping;
        }

        private MappingRowTransfer GenerateTransferLookupMapping(BaseMapping baseMapping)
        {
            var specifiedMapping = baseMapping as LookupMapping;
            MappingRowTransfer transferedLookupMapping = null;
            if (specifiedMapping.IsNotNull())
            {
                transferedLookupMapping = GenerateTransferLookupMapping(specifiedMapping);
            }
            return transferedLookupMapping;
        }

        private MappingRowTransfer GenerateTransferLookupMapping(LookupMapping lookupMapping)
        {
            MappingRowTransfer transferedLookupMapping = new MappingRowTransfer
                                                             {
                Source = lookupMapping.Source,
                SourcePosition = lookupMapping.SourcePosition,
                Destination = lookupMapping.Destination,
                DestinationColumnID = lookupMapping.DestinationColumnID,
                MappingType = ((int)MappingDestinationColumnTypes.LookupMapping).ToString(),
                MappingTypeName = "Lookup Mapping"
            };
            List<string> lookupValues = new List<string>();
            foreach (LookupData lookup in lookupMapping.LookupValue)
            {
                lookupValues.Add(string.Format("[\"{0}\",\"{1}\"]", lookup.Existing, lookup.New));
            }
            string lookupValueText = lookupValues.Count > 0 ? string.Format("[{0}]", string.Join(",", lookupValues.ToArray())) : string.Empty;
            transferedLookupMapping.MappingValue = lookupValueText;
            return transferedLookupMapping;
        }
    }
}