using Mirle.ASRS.WCS.Model.PLCDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.DB.Object.Service
{
    public class clsStoreIn
    {
        public static void FunOutsourceStoreIn_WriteCV()
        {
            try
            {
                int bufferIndex = 48;
                clsDB_Proc.GetDB_Object().GetProcess().FunOutsourceStoreInWriPlc(StnNo.A11_06, bufferIndex);
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public static void FunProduceStoreInWriteCV()//A2toA4寫入buffer
        {
            try
            {
                int bufferIndex = 43;
                clsDB_Proc.GetDB_Object().GetProcess().FunProduceStoreInWriPlc(StnNo.A08, bufferIndex);
                
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }



        public static void StoreIn_CreateEquCmd()
        {
            try
            {
                for (int bufferIndex = 1; bufferIndex <= 11; bufferIndex += 2)
                {
                    clsDB_Proc.GetDB_Object().GetProcess().FunStoreIn_CreateEquCmd(bufferIndex);
                }     
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public static void StoreIn_EquCmdFinish()
        {
            try
            {
                clsDB_Proc.GetDB_Object().GetProcess().FunStoreInEquCmdFinish();  
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
