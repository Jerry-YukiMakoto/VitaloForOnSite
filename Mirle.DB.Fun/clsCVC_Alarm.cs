using System;
using Mirle.Def;
using System.Data;
using Mirle.Structure;
using Mirle.DataBase;
using Mirle.ASRS.WCS.Model.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace Mirle.DB.Fun
{
    public class clsCVC_Alarm
    {

        public GetDataResult GetSystemAlarmInfo(string alarmBit, out DataObject<AlarmCVCInfo> dataObject, SqlServer db)
        {

                string sql = $"SELECT * FROM ALARMCVCDEF ";
                sql += $"WHERE BUFFERID='{0}' ";
                sql += $"AND ALARMCVCBIT='{alarmBit}' ";
                return db.GetData(sql, out dataObject);
            
        }
        public GetDataResult GetAlarmCVCInfo(int PLCNO,int bufferIndex, string alarmBit, out DataObject<AlarmCVCInfo> dataObject, SqlServer db)
        {

                string sql = $"SELECT * FROM ALARMCVCDEF ";
                sql += $"WHERE BUFFERID='{bufferIndex}' ";
                sql += $"AND ALARMCVCBIT='{alarmBit}' ";
                sql += $"AND PLCNO='{PLCNO}' ";
            return db.GetData(sql, out dataObject);
            
        }

        public GetDataResult GetAlarmLog(string alarmCvcCode, out DataObject<AlarmCVCLog> dataObject, SqlServer db)
        {

            string sql = $"SELECT * FROM ALARMCVCLOG ";
            sql += $"WHERE ALARMCVCCODE='{alarmCvcCode}' ";
            sql += $"AND CLRDT in (NULL,'')  ";
            return db.GetData(sql, out dataObject);
        }

        public ExecuteSQLResult InsertNewAlarmCVCLog(string AlarmCVCcode, SqlServer db)
        {

            string sql = "INSERT INTO ALARMCVCLOG (";
            sql += "STRDT, ";
            sql += "ALARMCVCCODE, ";
            sql += "CLRDT, ";
            sql += "TOTALSECS ";
            sql += ") VALUES (";
            sql += $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', ";
            sql += $"'{AlarmCVCcode}', ";
            sql += $"'', ";
            sql += $"'{1}'";
            sql += $")";
            return db.ExecuteSQL2(sql);

        }

        public ExecuteSQLResult UpdateAlarmLogEnd(string strDT, string alarmCVCCode, string clrDT, string secs, SqlServer db)
        {
                string sql = "UPDATE ALARMCVCLOG ";
                sql += $"SET CLRDT='{clrDT}',TOTALSECS='{secs}' ";
                sql += $"WHERE STRDT='{strDT}' AND ALARMCVCCODE='{alarmCVCCode}' ";
                return db.ExecuteSQL2(sql);
        }

    }
}

