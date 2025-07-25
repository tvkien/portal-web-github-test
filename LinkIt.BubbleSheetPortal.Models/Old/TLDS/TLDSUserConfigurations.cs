using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSUserConfigurations
    {
        private EarlyChildHoodServiceConfiguration _erlyChildHoodServiceConfiguration;

        public EarlyChildHoodServiceConfiguration EarlyChildHoodServiceConfiguration
        {
            get
            {
                if (_erlyChildHoodServiceConfiguration == null)
                {
                    _erlyChildHoodServiceConfiguration = new EarlyChildHoodServiceConfiguration();
                }
                return _erlyChildHoodServiceConfiguration;
            }
            set
            {
                _erlyChildHoodServiceConfiguration = value;
                
            }
        }

        private ContextSpecificConfiguration _contextSpecificConfiguration;

        public ContextSpecificConfiguration ContextSpecificConfiguration
        {
            get
            {
                if (_contextSpecificConfiguration == null)
                {
                    _contextSpecificConfiguration = new ContextSpecificConfiguration();
                }
                return _contextSpecificConfiguration;
            }
            set { _contextSpecificConfiguration = value; }
        }
    }
}
