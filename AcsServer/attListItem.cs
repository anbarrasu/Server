using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Access_Control_System
{
    class attListItem
    {
        public string empid;
        public string empname;
        public DateTime intime;
        public DateTime outtime;


        public attListItem(string aempid, string aempname, DateTime aintime, DateTime aouttime)
        {
            empid = aempid;
            empname = aempname;
            intime = aintime;
            outtime = aouttime;

        }
    }

   
}
