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
                string EquNo = "";
                for (int bufferIndex = 1; bufferIndex <= 11; bufferIndex += 2)
                {
                    if (bufferIndex == 1)
                    {
                        EquNo = "1";
                    }
                    else if (bufferIndex == 3)
                    {
                        EquNo = "2";
                    }
                    else if (bufferIndex == 5)
                    {
                        EquNo = "3";
                    }
                    else if (bufferIndex == 7)
                    {
                        EquNo = "4";
                    }
                    else if(bufferIndex == 9)
                    {
                        EquNo = "5";
                    }
                    else if(bufferIndex == 11)
                    {
                        EquNo = "6";
                    }
                    clsDB_Proc.GetDB_Object().GetProcess().FunStoreOut_A01_01ToA01_11_WriPlc(EquNo, bufferIndex);
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
                var EQU = new List<string>()
                {
                    "1",
                    "2",
                    "3",
                    "4",
                    "5",
                    "6",
                };
                clsDB_Proc.GetDB_Object().GetProcess().FunStoreOutEquCmdFinish(EQU);
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public static void StoreOut_ShowKanBanStart()
        {
            try
            {
                string Stn_No = "";

                for (int bufferIndex = 31; bufferIndex <= 41; bufferIndex += 10)//出庫口buffer:A11-02 和 A11-04
                {
                    if (bufferIndex == 31)
                    {
                        Stn_No = StnNo.A11_02;
                    }
                    else
                    {
                        Stn_No = StnNo.A11_04;
                    }

                    clsDB_Proc.GetDB_Object().GetProcess().StoreOut_ShowOnKanBanStart(bufferIndex,Stn_No);
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public static void StoreOut_ShowKanBanFinish()
        {
            try
            {
                string Stn_No = "";

                for (int bufferIndex = 31; bufferIndex <= 41; bufferIndex += 10)//出庫口buffer:A11-02 和 A11-04
                {
                    if (bufferIndex == 31)
                    {
                        Stn_No = StnNo.A11_02;
                    }
                    else
                    {
                        Stn_No = StnNo.A11_04;
                    }
                    clsDB_Proc.GetDB_Object().GetProcess().StoreOut_ShowOnKanBanFinish(bufferIndex,Stn_No);
                }
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
