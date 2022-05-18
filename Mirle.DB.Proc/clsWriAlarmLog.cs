using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirle.WriLog;

namespace Mirle.DB.Proc
{
    public class clsWriAlarmLog
    {
        public static clsLog Log = new clsLog("Buffer Alarm Log", true);

        public static void AlarmLogTrace(int BufferIndex, string BufferName, string Msg)
        {
            Log.FunWriTraceLog_CV($"Alarm => {BufferIndex} | {BufferName}: {Msg}");
        }



    }
}
