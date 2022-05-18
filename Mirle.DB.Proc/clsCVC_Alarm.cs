using System;
using Mirle.ASRS.WCS.Model.DataAccess;
using Mirle.DataBase;
using Mirle.Def;

namespace Mirle.DB.Proc
{
    public class clsCVC_Alarm
    {
        private clsDbConfig _config = new clsDbConfig();
        private Fun.clsCVC_Alarm _alarm = new Fun.clsCVC_Alarm();
        public clsCVC_Alarm(clsDbConfig config)
        {
            _config = config;
        }

        public GetDataResult GetSystemAlarmInfo(string alarmbit, out DataObject<AlarmCVCInfo> dataObject)
        {
            try
            {
                using (var db = clsGetDB.GetDB(_config))
                {
                    int iRet = clsGetDB.FunDbOpen(db);
                    if (iRet == DBResult.Success)
                    {
                        return _alarm.GetSystemAlarmInfo(alarmbit, out dataObject, db);
                    }
                    else
                    {
                        dataObject = null;
                        return null;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                dataObject = null;
                return null;
            }
        }
    }
}
