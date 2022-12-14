using System.Collections.Generic;
using Mirle.DataBase;
using Mirle.Def;

namespace Mirle.DB.Proc
{
    public class clsHost
    {
        private readonly clsProc Process;
        private readonly clsCmd_Mst CMD_MST;
        private readonly clsSno SNO;
        private readonly clsLocMst LocMst;
        private readonly clsEqu_Cmd EQU_CMD;
        private readonly clsAlarmData ALARMDATA;
        private readonly clsCmd_Mst_His CMD_MST_HIS;
        private readonly clsUnitStsLog unitStsLog;
        private readonly clsUnitModeDef unitModeDef;
        private readonly clsCVC_Alarm CVC_Alarm;
        private readonly clsErrorReport errorReport;
        private static object _Lock = new object();
        private static bool _IsConn = false;
        public static bool IsConn
        {
            get { return _IsConn; }
            set
            {
                lock(_Lock)
                {
                    _IsConn = value;
                }
            }
        }

        private static clsDbConfig _config_Sqlite = new clsDbConfig
        {
            DBType = DBTypes.SQLite
        };

        public clsHost(clsDbConfig config, string DB_Name_Sqlite,bool OnlyMonitor)
        {
            _config_Sqlite.DbName = DB_Name_Sqlite;
            Process = new clsProc(config, _config_Sqlite);
            CMD_MST = new clsCmd_Mst(config);
            SNO = new clsSno(config);
            LocMst = new clsLocMst(config);
            EQU_CMD = new clsEqu_Cmd(config, _config_Sqlite);
            ALARMDATA = new clsAlarmData(config);
            CMD_MST_HIS = new clsCmd_Mst_His(config);
            unitStsLog = new clsUnitStsLog(config);
            unitModeDef = new clsUnitModeDef(config);
            CVC_Alarm = new clsCVC_Alarm(config);
            errorReport = new clsErrorReport(config,OnlyMonitor);
        }

        public clsProc GetProcess()
        {
            return Process;
        }

        public clsErrorReport GeterrorReport()
        {
            return errorReport;
        }

        public clsCVC_Alarm GetCVC_Alarm()
        {
            return CVC_Alarm;
        }

        public clsCmd_Mst GetCmd_Mst()
        {
            return CMD_MST;
        }

        public clsLocMst GetLocMst()
        {
            return LocMst;
        }

        public clsEqu_Cmd GetEqu_Cmd()
        {
            return EQU_CMD;
        }

        public clsSno GetSNO()
        {
            return SNO;
        }

        public clsAlarmData GetAlarmData()
        {
            return ALARMDATA;
        }
        public clsCmd_Mst_His GetCmd_Mst_His()
        {
            return CMD_MST_HIS;
        }

        

        public clsUnitStsLog GetUnitStsLog()
        {
            return unitStsLog;
        }

        public clsUnitModeDef GetUnitModeDef()
        {
            return unitModeDef;
        }

       

        public List<Element_Port>[] GetLstPort()
        {
            return Process.GetLstPort();
        }

    }
}
