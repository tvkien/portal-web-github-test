using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.DistrictReferenceData
{
    public class WebUtilityHelper
    {
        private const string noDataText = "(data not found)";
        
        public string GetDistrictFullName(District district)
        {            
            string districtFullName = string.Empty;
            if (district.IsNotNull())
            {
                districtFullName = string.Format("District Reference Data for {0} ({1}{2})", district.Name, string.IsNullOrEmpty(district.LICode) ? string.Empty : string.Format("{0} ", district.LICode), district.Id);
            }
            return districtFullName;
        }

        public string GetStateName(State state)
        {
            string stateName = string.Empty;
            if (state.IsNotNull())
            {
                stateName = string.Format("State: {0} ({1})", state.Name, state.Id);
            }
            else
            {
                stateName = string.Format("State {0}", noDataText);
            }
            return stateName;
        }
    }
}