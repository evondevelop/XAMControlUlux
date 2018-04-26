using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XAMIO.Common.Net;
using XAMCommon.Trace;
using XAMIO.Common;

namespace XAMIO.Ulux.Ump
{
    public class XAMUmUtils 
    {
        public static string GetSwitchIdentifier( int projectID,int switchID, int designID)
        {
            return projectID + "-" + switchID + "-" + designID;
        }
    }

}
