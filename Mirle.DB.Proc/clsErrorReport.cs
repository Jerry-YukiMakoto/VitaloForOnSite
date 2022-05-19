using Mirle.ASRS.WCS.Controller;
using Mirle.Def;
using Mirle.DataBase;
using Mirle.ASRS.WCS.Model.DataAccess;
using Mirle.CENS.U0NXMA30;
using WCS_API_Client.ReportInfo;
using Mirle.ASRS.WCS.Model.PLCDefinitions;
using System.Timers;
using System.Collections.Generic;
using System;
using Mirle.ASRS.Conveyors;

namespace Mirle.DB.Proc
{
    public class clsErrorReport
    {
        public delegate void AlarmEventHandler(object sender, AlarmEventArgs e);
        public delegate void BufferEventHandler(object sender, BufferEventArgs e);

        private DB.Fun.clsCmd_Mst CMD_MST = new DB.Fun.clsCmd_Mst();
        private DB.Fun.clsCVC_Alarm CVC_Alarm = new DB.Fun.clsCVC_Alarm();
        private clsDbConfig _config = new clsDbConfig();
        public static Dictionary<string, int> EmptyFlag = new Dictionary<string, int>();
        public static Dictionary<string, int> DisplayFlag = new Dictionary<string, int>();
        private Dictionary<string, string> _CVCalarm = new Dictionary<string, string>();

        public clsErrorReport(clsDbConfig config)
        {
            _config = config;
        }

        public void ErrorInsertReport()
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        var _conveyor = ControllerReader.GetCVControllerr().GetConveryor();
                        var _conveyor2 = ControllerReader.GetCVControllerr().GetConveryor2();

                        _conveyor.OnSystemAlarmClear += Converyor_OnSystemAlarmClear;
                        _conveyor.OnSystemAlarmTrigger += Converyor_OnSystemAlarmTrigger;

                        foreach (var buffer in _conveyor.Buffers)
                        {
                            buffer.OnAlarmTrigger += Buffer_OnAlarmTrigger;
                            buffer.OnAlarmClear += Buffer_OnAlarmClear;
                        }

                        _conveyor2.OnSystemAlarmClear += Converyor_OnSystemAlarmClear;
                        _conveyor2.OnSystemAlarmTrigger += Converyor_OnSystemAlarmTrigger;

                        foreach (var buffer in _conveyor2.Buffers)
                        {
                            buffer.OnAlarmTrigger += Buffer_OnAlarmTrigger;
                            buffer.OnAlarmClear += Buffer_OnAlarmClear;
                        }

                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);

                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        private void Buffer_OnAlarmTrigger(object sender, AlarmEventArgs e)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {

                        string sAlarmDesc = "";
                        int sBufferIndex = e.BufferIndex;
                        string hAlarmBit = e.AlarmBit.ToString("X");

                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, "Alarm bit on");
                        if (CVC_Alarm.GetAlarmCVCInfo(sBufferIndex, hAlarmBit, out var alarminfo, db) == GetDataResult.Success)
                        {
                            sAlarmDesc = alarminfo[0].AlarmDesc;
                            try
                            {
                                _CVCalarm.Add(sAlarmDesc, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            catch (Exception ex)
                            {
                                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                            }

                            if (CVC_Alarm.InsertNewAlarmCVCLog(alarminfo[0].AlarmCode, db) == ExecuteSQLResult.Success)
                            {
                                clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Write alarm log:[{sAlarmDesc}]");
                            }
                            else
                            {
                                clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Write alarm log:[{sAlarmDesc}] fail.");
                            }
                        }
                        else
                        {
                            clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Get alarm info fail. (Alarm Bit:{hAlarmBit})");
                        }

                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);

                    }
                }

            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }


        private void Buffer_OnAlarmClear(object sender, AlarmEventArgs e)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {

                        string sAlarmDesc = "";
                        int sBufferIndex = e.BufferIndex;
                        string hAlarmBit = e.AlarmBit.ToString("X");

                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, "Alarm bit off");
                        if (CVC_Alarm.GetAlarmCVCInfo(sBufferIndex, hAlarmBit, out var alarminfo, db) == GetDataResult.Success)
                        {
                            sAlarmDesc = alarminfo[0].AlarmDesc;
                            try
                            {
                                _CVCalarm.Remove(sAlarmDesc);
                            }
                            catch (Exception ex)
                            {
                                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                            }

                            if (CVC_Alarm.GetAlarmLog(alarminfo[0].AlarmCode, out var alarmlog, db) == ExecuteSQLResult.Success)
                            {
                                foreach (var alog in alarmlog.Data)
                                {
                                    if (!DateTime.TryParse(alog.StrDt, out DateTime starttmp))
                                    {
                                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Get start time fail. ({alarminfo[0].AlarmCode})");
                                        continue;
                                    }
                                    String date;
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (!DateTime.TryParse(date, out DateTime cleartmp))
                                    {
                                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Get clear time fail. ({alarminfo[0].AlarmCode})");
                                        continue;
                                    }
                                    TimeSpan secs = cleartmp - starttmp;

                                    if (CVC_Alarm.UpdateAlarmLogEnd(alog.StrDt, alarminfo[0].AlarmCode, date, Convert.ToInt32(secs.TotalSeconds).ToString(), db) == ExecuteSQLResult.Success)
                                    {
                                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Update alarm log success. ({alarminfo[0].AlarmCode})");
                                    }
                                    else
                                    {
                                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Update alarm log fail. ({alarminfo[0].AlarmCode})");
                                    }

                                }

                            }
                            else
                            {
                                clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Get alarm log:[{sAlarmDesc}] fail.");
                            }
                        }
                        else
                        {
                            clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Get alarm info fail. (Alarm Bit:{hAlarmBit})");
                        }

                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);

                    }
                }

            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        private void Converyor_OnSystemAlarmTrigger(object sender, AlarmEventArgs e)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {

                        string hAlarmBit = e.AlarmBit.ToString("X");

                        if (CVC_Alarm.GetSystemAlarmInfo(hAlarmBit, out var alarminfo, db) == GetDataResult.Success)
                        {
                            string alarmdesc = alarminfo[0].AlarmDesc;
                            if (CVC_Alarm.InsertNewAlarmCVCLog(alarminfo[0].AlarmCode, db) == ExecuteSQLResult.Success)
                            {
                                clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Write alarm log:[{alarmdesc}]");
                            }
                            else
                            {
                                clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Write alarm log:[{alarmdesc}] fail.");
                            }
                        }
                        else
                        {
                            clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, "Get system alarm info fail.");
                        }


                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);

                    }
                }

            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }

        }

        private void Converyor_OnSystemAlarmClear(object sender, AlarmEventArgs e)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {

                        string hAlarmBit = e.AlarmBit.ToString("X");
                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Alarm bit off. (Alarm Bit:{hAlarmBit})");

                        if (CVC_Alarm.GetSystemAlarmInfo(hAlarmBit, out var alarminfo, db) == GetDataResult.Success)
                        {
                            if (CVC_Alarm.GetAlarmLog(alarminfo[0].AlarmCode, out var alarmlog, db) == GetDataResult.Success)
                            {
                                foreach (var alog in alarmlog.Data)
                                {
                                    if (!DateTime.TryParse(alog.StrDt, out DateTime starttmp))
                                    {

                                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Get start time fail. ({alarminfo[0].AlarmCode})");
                                        continue;
                                    }
                                    String date;
                                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    if (!DateTime.TryParse(date, out DateTime cleartmp))
                                    {

                                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Get clear time fail. ({alarminfo[0].AlarmCode})");
                                        continue;
                                    }

                                    TimeSpan secs = cleartmp - starttmp;

                                    if (CVC_Alarm.UpdateAlarmLogEnd(alog.StrDt, alarminfo[0].AlarmCode, date, Convert.ToInt32(secs.TotalSeconds).ToString(), db) == ExecuteSQLResult.Success)
                                    {
                                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Update alarm log success. ({alarminfo[0].AlarmCode})");
                                    }
                                    else
                                    {
                                        clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Update alarm log fail. ({alarminfo[0].AlarmCode})");
                                    }
                                }
                            }
                            else
                            {
                                clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Can't find alarm log.");
                            }
                        }
                        else
                        {
                            clsWriAlarmLog.AlarmLogTrace(e.BufferIndex, hAlarmBit, $"Get alarm info fail. (Alarm Bit:{hAlarmBit})");
                        }
                    }
                    else
                    {
                        string strEM = "Error: 開啟DB失敗！";
                        clsWriLog.Log.FunWriTraceLog_CV(strEM);

                    }
                }

            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public Dictionary<string, string> GetAlarmData()
        {
            return _CVCalarm;
        }
        public bool isAlarm()
        {
            return _CVCalarm.Count > 0;
        }
    }
}