using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirle.ASRS.WCS.Model.PLCDefinitions;

namespace Mirle.DB.Object.Service
{
    public class clsStoreOut
    {
       
        public static void StoreOut_A01_01ToA01_11_WriteCV()
        {
            try
            {
                string stn = "";
                for (int bufferIndex = 1; bufferIndex <= 11; bufferIndex += 2)
                {
                    if (bufferIndex == 1)
                    {
                        stn = StnNo.A01_01;
                    }
                    else if (bufferIndex == 3)
                    {
                        stn = StnNo.A01_03;
                    }
                    else if (bufferIndex == 5)
                    {
                        stn = StnNo.A01_05;
                    }
                    else if (bufferIndex == 7)
                    {
                        stn = StnNo.A01_07;
                    }
                    else if(bufferIndex == 9)
                    {
                        stn = StnNo.A01_09;
                    }
                    else if(bufferIndex == 11)
                    {
                        stn = StnNo.A01_11;
                    }
                    clsDB_Proc.GetDB_Object().GetProcess().FunStoreOut_A01_01ToA01_11_WriPlc(stn, bufferIndex);
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public static void StoreOutt_A01_01ToA01_11_CreateEquCmd()
        {
            try
            {
                for (int bufferIndex = 1; bufferIndex <= 11; bufferIndex += 2)
                {
                    clsDB_Proc.GetDB_Object().GetProcess().FunStoreOut_A01_01ToA01_11_CreateEquCmd(bufferIndex);
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public static void StoreOut_EquCmdFinish()
        {
            try
            {
                var stn = new List<string>()
                {
                    StnNo.A01_01,
                    StnNo.A01_03,
                    StnNo.A01_05,
                    StnNo.A01_07,
                    StnNo.A01_09,
                    StnNo.A01_11,
                };
                clsDB_Proc.GetDB_Object().GetProcess().FunStoreOutEquCmdFinish(stn);
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
