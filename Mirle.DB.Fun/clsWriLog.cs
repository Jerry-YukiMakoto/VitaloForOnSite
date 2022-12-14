using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mirle.WriLog;

namespace Mirle.DB.Fun
{
    public class clsWriLog
    {
        public static clsLog Log = new clsLog("DB_Fun", true);
        public static void StoreInLogTrace(int BufferIndex, string BufferName, string Msg)
        {
            Log.FunWriTraceLog_CV($"StoreIn => {BufferIndex} | {BufferName}: {Msg}");
        }

        public static void StoreOutLogTrace(int BufferIndex, string BufferName, string Msg)
        {
            Log.FunWriTraceLog_CV($"StoreOut => {BufferIndex} | {BufferName}: {Msg}");
        }

        public static void L2LLogTrace(int CmdType, string IoType, string Msg)
        {
            Log.FunWriTraceLog_CV($"L2L => {CmdType} | {IoType}: {Msg}");
        }
    }
}
