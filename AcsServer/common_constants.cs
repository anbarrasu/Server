using System;
using System.Collections.Generic;
using System.Text;

namespace AcsServer
{
    class common_constants
    {
        public const int first = 0;
    }

    class message_id
    {
        public const int ping = 1;
        public const int ping_rply = ping+1;
        public const int id = ping_rply + 1;

        public const int testmsg = id + 1;
    }

    class message_key
    {
        public const int pingkey = 0x11223344;

    }

}
