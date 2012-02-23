using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Access_Control_System
{
    class acsConstant
    {
        public const int recordAvailable = 1;
        public const int recordUnavailable = recordAvailable + 1;
        public const int invalidIid = recordUnavailable + 1;
        public const int tableNotAvailable = invalidIid+1;


        public const int exceptionoccured = tableNotAvailable + 1;
    }
}
