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
    public class clsCmd_Mst
    {
        private clsTool tool = new clsTool();

        #region Store In

        public GetDataResult GetCmdMstByStoreInStart(string stations, string Item_No,string Lot_No, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst as A full join Cmd_Dtl as B On A.Cmd_Sno=B.Cmd_Sno ";
            sql += $"WHERE A.Cmd_Mode IN ('{clsConstValue.CmdMode.StockIn}') ";
            sql += $"AND A.Cmd_Sts='{clsConstValue.CmdSts.strCmd_Initial}' ";
            sql += $"AND A.Stn_No = '{stations}' ";
            sql += $"AND B.Item_No = '{Item_No}' ";
            sql += $"AND B.Lot_No = '{Lot_No}' ";
            sql += $"order by A.prty , A.crt_date , A.cmd_sno";
            return db.GetData(sql, out dataObject);
        }
        public GetDataResult GetCmdMstByStoreInStart(string stations, string Item_No, string Lot_No,bool pltfish,string BCRplt, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst as A full join Cmd_Dtl as B On A.Cmd_Sno=B.Cmd_Sno ";
            sql += $"WHERE A.Cmd_Mode IN ('{clsConstValue.CmdMode.StockIn}') ";
            sql += $"AND A.Cmd_Sts IN ('{clsConstValue.CmdSts.strCmd_Initial}') ";
            sql += $"AND B.Item_No = '{Item_No}' ";
            sql += $"AND B.Lot_No = '{Lot_No}' ";
            sql += $"AND A.Plt_Id = '{BCRplt}' ";
            sql += $"order by A.prty , A.crt_date , A.cmd_sno";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetCmdMstByStoreInForKanBan(string cmdsno, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst  ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.StockIn}'), '{clsConstValue.CmdMode.Cycle}' ";
            sql += $"AND Cmd_Sts IN ('{clsConstValue.CmdSts.strCmd_Running}') ";
            sql += $"AND Cmd_Sno='{cmdsno}' ";
            sql += $"AND Trace = '{Trace.StoreInWriteCmdToCV}' ";
            sql += $"AND Remark<>'{Remark.StoreInKanBanStart}' ";
            sql += $"order by prty , crt_date , cmd_sno";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetCmdMstByStoreInReject(string cmdsno, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst  ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.Deposit}') ";
            sql += $"AND Cmd_Sts IN ('{clsConstValue.CmdSts.strCmd_Running}') ";
            sql += $"AND Cmd_Sno='{cmdsno}' ";
            sql += $"AND Remark<>'{Remark.StoreInRejectFinish}' ";
            sql += $"order by prty , crt_date , cmd_sno";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetCmdMstByStoreInForKanBanFinish(string cmdsno, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst  ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.StockIn}','{clsConstValue.CmdMode.Cycle}') ";
            sql += $"AND Cmd_Sts IN ('{clsConstValue.CmdSts.strCmd_Running}') ";
            sql += $"AND Cmd_Sno='{cmdsno}' ";
            sql += $"AND Trace = '{Trace.StoreInKanBanStart}' ";
            sql += $"AND Remark<>'{Remark.StoreInKanBanFinsh}' ";
            sql += $"order by prty , crt_date , cmd_sno";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetCmdMstByCycleStart(string stations, string Item_No, string Lot_No, bool pltfish, string BCRplt, out DataObject<CmdMst> dataObject, SqlServer db)//委外入庫口(盤點)
        {
            string sql = "SELECT * FROM Cmd_Mst as A full join Cmd_Dtl as B On A.Cmd_Sno=B.Cmd_Sno ";
            sql += $"WHERE A.Cmd_Mode IN ('{clsConstValue.CmdMode.Cycle}') ";
            sql += $"AND A.Cmd_Sts IN ('{clsConstValue.CmdSts.strCmd_Running}') ";
            sql += $"AND B.Item_No = '{Item_No}' ";
            sql += $"AND B.Lot_No = '{Lot_No}' ";
            sql += $"AND A.Plt_Id = '{BCRplt}' ";
            sql += $"AND A.IO_Type IN ('{IOtype.Cycle}') ";
            sql += $"order by A.prty , A.crt_date , A.cmd_sno";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetCmdMstByStoreInCrane(string cmdsno, out DataObject<CmdMst> dataObject, SqlServer db) //同時處理盤點入庫
        {
            string sql = "SELECT * FROM Cmd_Mst ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.StockIn}', '{clsConstValue.CmdMode.Cycle}') ";
            sql += $"AND Cmd_Sno='{cmdsno}' ";
            sql += $"AND TRACE IN ('{Trace.StoreInKanBanFinish}') "; 
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Running}' ";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetCmdMstAndDtlCheck(string cmdsno, out DataObject<CmdMst> dataObject, SqlServer db) //檢查BCR資料用途
        {
            string sql = "SELECT * FROM Cmd_Mst as A full join Cmd_Dtl as B On A.Cmd_Sno=B.Cmd_Sno ";
            sql += $"WHERE A.Cmd_Sno='{cmdsno}' ";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetCmdMstByStoreInFinish(IEnumerable<string> stations, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.StockIn}', '{clsConstValue.CmdMode.Cycle}') ";
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Running}' ";
            sql += $"AND TRACE IN ('{Trace.StoreInCreateCraneCmd}') ";
            sql += $"AND Stn_No IN (";
            foreach (var stn in stations)
            {
                if (stations.Last() == stn)
                {
                    sql += $" '{stn}'";
                }
                else if (sql.EndsWith(","))
                {
                    sql += $" '{stn}',";
                }
                else
                {
                    sql += $"'{stn}',";
                }
            }
            sql += $")";
            return db.GetData(sql, out dataObject);
        }

        #endregion Store In



        #region Store Out

        public GetDataResult GetCmdMstByStoreOutStart(string stations, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.StockOut}', '{clsConstValue.CmdMode.Cycle}') ";
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Initial}' ";
            sql += $"AND New_Loc = '{stations}' ";
            sql += $"order by prty , crt_date , cmd_sno";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetCmdMstByStoreOutCrane(string CmdSno, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.StockOut}', '{clsConstValue.CmdMode.Cycle}') ";
            sql += $"AND Cmd_Sno='{CmdSno}' ";
            sql += $"AND TRACE='{Trace.StoreOutWriteCraneCmdToCV}' ";
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Running}' ";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetCmdMstByStoreOutFinish(IEnumerable<string> stations, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.StockOut}','{clsConstValue.CmdMode.Cycle}') ";
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Running}' ";
            sql += $"AND TRACE IN ('{Trace.StoreOutCreateCraneCmd}') ";
            sql += $"AND New_Loc IN (";
            foreach (var stn in stations)
            {
                if (stations.Last() == stn)
                {
                    sql += $" '{stn}'";
                }
                else if (sql.EndsWith(","))
                {
                    sql += $" '{stn}',";
                }
                else
                {
                    sql += $"'{stn}',";
                }
            }
            sql += $")";
            return db.GetData(sql, out dataObject);
        }


        public GetDataResult GetCmdMstByStoreOutForKanBan(string cmdsno, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst  ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.StockOut}'), '{clsConstValue.CmdMode.Cycle}' ";
            sql += $"AND Cmd_Sts IN ('{clsConstValue.CmdSts.strCmd_Running}') ";
            sql += $"AND Cmd_Sno='{cmdsno}' ";
            sql += $"AND Trace = '{Trace.StoreOutCraneCmdFinish}' ";
            sql += $"AND Remark<>'{Remark.StoreOutKanBanStart}' ";
            sql += $"order by prty , crt_date , cmd_sno";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetCmdMstByStoreOutForKanBanFinish(string cmdsno, out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst  ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.StockOut}','{clsConstValue.CmdMode.Cycle}') ";
            sql += $"AND Cmd_Sts IN ('{clsConstValue.CmdSts.strCmd_Running}') ";
            sql += $"AND Cmd_Sno='{cmdsno}' ";
            sql += $"AND Trace = '{Trace.StoreOutKanBanStart}' ";
            sql += $"AND Remark<>'{Remark.StoreOutKanBanFinish}' ";
            sql += $"order by prty , crt_date , cmd_sno";
            return db.GetData(sql, out dataObject);
        }


        #endregion Store Out


        #region L2L

        public GetDataResult GetLocToLoc(out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.L2L}') ";
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Initial}' ";
            return db.GetData(sql, out dataObject);
        }

        public GetDataResult GetLoctoLocFinish( out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM Cmd_Mst ";
            sql += $"WHERE Cmd_Mode IN ('{clsConstValue.CmdMode.L2L}') ";
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Running}' ";
            sql += $"AND TRACE IN ('{Trace.LoctoLocReady}') ";
            return db.GetData(sql, out dataObject);
        }

        #endregion  L2L




        #region Update

        public ExecuteSQLResult UpdateCmdMst(string cmdSno, string trace, SqlServer db)
        {
            string sql = "UPDATE Cmd_Mst  ";
            sql += $"SET TRACE='{trace}',";
            sql += $"Remark=''";
            sql += $"WHERE Cmd_Sno='{cmdSno}' ";
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Running}' ";
            return db.ExecuteSQL2(sql);
        }

        public ExecuteSQLResult UpdateCmdMst(string cmdSno, string cmdSts, string trace, SqlServer db)
        {
            string sql = "UPDATE Cmd_Mst ";
            sql += $"SET TRACE='{trace}', ";
            sql += $"Cmd_Sts='{cmdSts}' ";
            sql += $"WHERE Cmd_Sno='{cmdSno}' ";
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Running}' ";
            return db.ExecuteSQL2(sql);
        }

        public ExecuteSQLResult UpdateCmdMstTransferring(string cmdSno, string trace, SqlServer db)
        {
            string sql = "UPDATE Cmd_Mst ";
            sql += $"SET Cmd_Sts='{clsConstValue.CmdSts.strCmd_Running}', ";
            sql += $"TRACE='{trace}', ";
            sql += $"Remark='', ";
            sql += $"EXP_Date='{DateTime.Now:yyyy-MM-dd HH:mm:ss}' ";
            sql += $"WHERE Cmd_Sno='{cmdSno}' ";
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Initial}' ";
            return db.ExecuteSQL2(sql);
        }

        public ExecuteSQLResult UpdateCmdMstTransferring(string cmdSno, string trace,string New_Loc,string Plt_Id, SqlServer db)
        {
            string sql = "UPDATE Cmd_Mst ";
            sql += $"SET Cmd_Sts='{clsConstValue.CmdSts.strCmd_Running}', ";
            sql += $"Plt_Id='{Plt_Id}', ";
            sql += $"TRACE='{trace}', ";
            sql += $"New_Loc='{New_Loc}', ";
            sql += $"Remark='', ";
            sql += $"EXP_Date='{DateTime.Now:yyyy-MM-dd HH:mm:ss}' ";
            sql += $"WHERE Cmd_Sno='{cmdSno}' ";
            sql += $"AND Cmd_Sts='{clsConstValue.CmdSts.strCmd_Initial}' ";
            return db.ExecuteSQL2(sql);
        }

        public ExecuteSQLResult UpdateCmdDtlTransferring(string cmdSno,string New_Loc, string Plt_Id, SqlServer db)
        {
            string sql = "UPDATE Cmd_Dtl ";
            sql += $"SET Plt_Id='{Plt_Id}' ,";
            sql += $"Loc='{New_Loc}', ";
            sql += $"In_Date='{DateTime.Now:yyyy-MM-dd HH:mm:ss}', ";
            sql += $"Updated_by='WCS', ";
            sql += $"Updated_Date='{DateTime.Now:yyyy-MM-dd HH:mm:ss}' ";
            sql += $"WHERE Cmd_Sno='{cmdSno}' ";
            return db.ExecuteSQL2(sql);
        }

        public ExecuteSQLResult UpdateCmdMstRemark(string cmdSno, string cmdSts, string REMARK, SqlServer db)
        {
            string sql = "UPDATE CMDMST ";
            sql += $"SET EndDate='{DateTime.Now:yyyy-MM-dd HH:mm:ss}' ,";
            sql += $"CmdSts='{cmdSts}' ,";
            sql += $"REMARK='{REMARK}' ";
            sql += $"WHERE CmdSno='{cmdSno}' ";
            return db.ExecuteSQL2(sql);
        }

        public ExecuteSQLResult UpdateCmdMstRemark(string cmdSno, string REMARK, SqlServer db)
        {
            string sql = "UPDATE Cmd_Mst ";
            sql += $"SET Exp_Date='{DateTime.Now:yyyy-MM-dd HH:mm:ss}', ";
            sql += $"REMARK='{REMARK}' ";
            sql += $"WHERE Cmd_Sno='{cmdSno}' ";
            return db.ExecuteSQL2(sql);
        }

        public ExecuteSQLResult UpdateCmdMstRemarkandAbnormal(string cmdSno, string REMARK,string abnormal, SqlServer db)
        {
            string sql = "UPDATE Cmd_Mst ";
            sql += $"SET Exp_Date='{DateTime.Now:yyyy-MM-dd HH:mm:ss}', ";
            sql += $"End_Date='{DateTime.Now:yyyy-MM-dd HH:mm:ss}' ,";
            sql += $"REMARK='{REMARK}' ,";
            sql += $"Cmd_Abnormal='{abnormal}' ";
            sql += $"WHERE Cmd_Sno='{cmdSno}' ";
            return db.ExecuteSQL2(sql);
        }

        #endregion Update




        public ExecuteSQLResult FunInsCmdMst(struCmdMst stuCmdMst, SqlServer db)
        {
                string sSQL = "INSERT INTO Cmd_Mst (Cmd_Sno, Cmd_Sts, Prty, Cmd_Abnormal, Trace, Stn_No, Cmd_Mode, IO_Type, WH_ID,Equ_No, Loc, New_Loc,";
                sSQL += "Crt_Date, Trn_User,Host_Name, Plt_Id) values(";
                sSQL += "'" + stuCmdMst.CmdSno + "', ";
                sSQL += "'" + stuCmdMst.CmdSts + "', ";
                sSQL += "'" + stuCmdMst.Prty + "', ";
                sSQL += "'" + stuCmdMst.CmdAbnormal + "', ";
                sSQL += "'" + stuCmdMst.Trace + "', ";
                sSQL += "'" + stuCmdMst.StnNo + "', ";
                sSQL += "'" + stuCmdMst.CmdMode + "', ";
                sSQL += "'" + stuCmdMst.IoType + "', ";
                sSQL += "'" + stuCmdMst.WhId + "', ";
                sSQL += "'" + stuCmdMst.EquNo + "', ";
                sSQL += "'" + stuCmdMst.Loc + "', ";
                sSQL += "'" + stuCmdMst.NewLoc + "', ";
                sSQL += "'" + stuCmdMst.CrtDate + "', ";
                sSQL += "'" + stuCmdMst.TrnUser + "', ";
                sSQL += "'" + stuCmdMst.HostName+"', ";
                sSQL += "'" + stuCmdMst.Plt_Id + "')";
                return db.ExecuteSQL2(sSQL);
        }

        public ExecuteSQLResult FunInsCmdDtl(struCmdDtl stuCmdDtl, SqlServer db)
        {
            string sSQL = "INSERT INTO Cmd_Dtl (Cmd_Txno, Cmd_Sno, Plt_Qty, Trn_Qty, Loc , In_Date, Item_No, Lot_No, Plt_Id , Company_ID, Item_Desc, Uom,";
            sSQL += "Created_by,Created_Date) values(";
            sSQL += "'" + stuCmdDtl.Cmd_Txno + "', ";
            sSQL += "'" + stuCmdDtl.Cmd_Sno + "', ";
            sSQL += "'" + stuCmdDtl.Plt_Qty + "', ";
            sSQL += "'" + stuCmdDtl.Trn_Qty + "', ";
            sSQL += "'" + stuCmdDtl.Loc + "', ";
            sSQL += "'" + stuCmdDtl.In_Date + "', ";
            sSQL += "'" + stuCmdDtl.Item_No + "', ";
            sSQL += "'" + stuCmdDtl.Lot_No + "', ";
            sSQL += "'" + stuCmdDtl.Plt_Id + "', ";
            sSQL += "'" + stuCmdDtl.Company_ID + "', ";
            sSQL += "'" + stuCmdDtl.Item_Desc + "', ";
            sSQL += "'" + stuCmdDtl.Uom+ "', ";
            sSQL += "'" + stuCmdDtl.Created_by + "', ";
            sSQL += "'" + stuCmdDtl.Created_Date + "')";
            return db.ExecuteSQL2(sSQL);
        }

        public GetDataResult EmptyInOutCheck(int Iotype,out DataObject<CmdMst> dataObject, SqlServer db)
        {
            string sql = "SELECT * FROM CMDMST";
            sql += $"WHERE IOtype='{Iotype}' ";
            sql += $"AND CmdSts in ('{clsConstValue.CmdSts.strCmd_Initial}','{clsConstValue.CmdSts.strCmd_Running}') ";
            return db.GetData(sql, out dataObject);
        }

        #region Micron Fun
        public int FunGetFinishCommand(ref DataTable dtTmp, SqlServer db)
        {
            try
            {
                string strSql = $"select * from Cmd_Mst where Cmd_Sts in ('{clsConstValue.CmdSts.strCmd_Cancel}', '{clsConstValue.CmdSts.strCmd_Finished}')";
                string strEM = "";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet != DBResult.Success && iRet != DBResult.NoDataSelect)
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");

                return iRet;
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return DBResult.Exception;
            }
        }

        public bool FunGetCommand(string sCmdSno, ref int iRet, SqlServer db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = "select * from CMD_MST where Cmd_Sno = '" + sCmdSno + "' ";
                iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    return true;
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSql + " => " + strEM);
                    return false;
                }
            }
            catch (Exception ex)
            {
                int errorLine = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
                return false;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public int FunGetCmdMst_Grid(ref DataTable dtTmp, SqlServer db)
        {
            try
            {
                string strEM = "";
                string strSql = $"select * from CMD_MST" +
                    $" where Cmd_Sts < '{clsConstValue.CmdSts.strCmd_Finished}' ";
                strSql += " ORDER BY prty, Crt_Date, Cmd_Sno";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet != DBResult.Success && iRet != DBResult.NoDataSelect)
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                }

                return iRet;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return DBResult.Exception;
            }
        }

        public bool FunDelCmdMst(string sCmdSno, SqlServer db)
        {
            try
            {
                string strEM = "";
                string strSQL = "delete from CMD_MST where Cmd_Sno = '" + sCmdSno + "' ";
                int Ret = db.ExecuteSQL(strSQL, ref strEM);
                if (Ret == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSQL); return true;
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSQL + " => " + strEM); return false;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunInsertCMD_MST_His(string sCmdSno, SqlServer db)
        {
            try
            {
                string SQL = "INSERT INTO CMD_MST_His ";
                SQL += $" SELECT '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}', * FROM CMD_MST ";
                SQL += $" WHERE Cmd_Sno='{sCmdSno}'";

                int iRet = db.ExecuteSQL(SQL);
                if (iRet == DBResult.Success)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunDelCMD_MST_His(double dblDay, SqlServer db)
        {
            try
            {
                string strDelDay = DateTime.Today.Date.AddDays(dblDay * (-1)).ToString("yyyy-MM-dd");
                string strSql = "delete from CMD_MST_His where HisDT <= '" + strDelDay + "' ";

                int iRet = db.ExecuteSQL(strSql);
                if (iRet == DBResult.Success)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        #endregion
    }
}
