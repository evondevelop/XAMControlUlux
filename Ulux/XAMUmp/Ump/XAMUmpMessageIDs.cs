using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XAMIO.Common.Net;
using XAMCommon.Trace;

namespace XAMIO.Ulux.Ump
{
    public enum UmpMessageID
    {
        IdState = 0x01,
        IdControl = 0x21,
        IdList = 0x0f,
        PageCount = 0x0e,
        PageIndex = 0x2e,
        EditValue = 0x42,
        DateTime = 0x2F,
        I2C_Temperature = 0x71,
        VideoStart = 0xA2,
        VideoState = 0xA1,
        AudioPlayRemote = 0x99,
    }
}
