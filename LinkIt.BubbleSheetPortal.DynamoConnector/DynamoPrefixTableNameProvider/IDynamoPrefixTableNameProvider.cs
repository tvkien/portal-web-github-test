using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider
{
    public interface IDynamoPrefixTableNameProvider
    {
        string DynamoPrefixTableName { get; }
    }
}
