using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Godot.Common.Helper
{
    public static class TimeHelper
    {
        public static ulong GetSystemTimeMsec()
        {
            double unixTime = Time.GetUnixTimeFromSystem();
            ulong unixTimeLong = (ulong)(unixTime * 1000.0);
            //return (int)((unixTime - unixTimeInt) * 1000.0);
            return unixTimeLong;
        }
    }
}
