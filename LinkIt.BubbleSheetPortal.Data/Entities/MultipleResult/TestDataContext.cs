using LinkIt.BubbleSheetPortal.Models.DTOs.StudentPreference;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    internal partial class TestDataContext
    {
        [global::System.Data.Linq.Mapping.FunctionAttribute(Name = "dbo.GetAssociatedClassesThatHasTestResult")]
        [ResultType(typeof(VirtualTestIdClassIdDto))]
        [ResultType(typeof(ClassDto))]

        public IMultipleResults GetAssociatedClassesThatHasTestResult_Multiple([global::System.Data.Linq.Mapping.ParameterAttribute(Name = "DistrictID", DbType = "Int")] System.Nullable<int> districtID, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "SchoolID", DbType = "Int")] System.Nullable<int> schoolID, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "UserID", DbType = "Int")] System.Nullable<int> userID, [global::System.Data.Linq.Mapping.ParameterAttribute(Name = "RoleID", DbType = "Int")] System.Nullable<int> roleID)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), districtID, schoolID, userID, roleID);
            return ((IMultipleResults)(result.ReturnValue));
        }
    }
}
