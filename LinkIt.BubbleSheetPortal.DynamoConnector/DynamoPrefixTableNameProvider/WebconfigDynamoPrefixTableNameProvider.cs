using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider
{
    public class WebconfigDynamoPrefixTableNameProvider : IDynamoPrefixTableNameProvider
    {
        public string DynamoPrefixTableName
        {
            get { return ConfigurationManager.AppSettings["DynamoPrefixTableName"]; }
        }
    }
}
