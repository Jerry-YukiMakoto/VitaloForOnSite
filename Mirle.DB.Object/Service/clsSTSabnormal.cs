using Mirle.ASRS.WCS.Model.PLCDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.DB.Object.Service
{
    public class clsSTSabnormal
    {
        public static void StoreAbnormalSTS_WriteCV()
        {
            try
            {
                for (int bufferIndex = 1; bufferIndex <= 11; bufferIndex += 2)
                {
                    clsDB_Proc.GetDB_Object().GetProcess().FunSTS_Abonormal_WritePLC(bufferIndex);
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public static void StoreAbnormalSTS_CrateCrane()
        {
            try
            {
                for (int bufferIndex = 1; bufferIndex <= 11; bufferIndex += 2)
                {
                    clsDB_Proc.GetDB_Object().GetProcess().FunSTSabnormal_CreateEquCmd(bufferIndex);
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public static void StoreAbnormalSTS_Finish()
        {
            try
            {
                    clsDB_Proc.GetDB_Object().GetProcess().FunStoreStsEquCmdFinish();
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }
    }
}
