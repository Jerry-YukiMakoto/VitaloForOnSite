using System;
using System.Collections.Generic;
using Mirle.Def;
using System.Data;
using Mirle.DataBase;
using Mirle.ASRS.WCS.Model.DataAccess;
using System.Linq;

namespace Mirle.DB.Fun
{
    public class clsLocMst
    {
        public int FunGetTeachLoc_Grid(ref DataTable dtTmp, SqlServer db)
        {
            try
            {
                string strEM = "";
                string strSql = "select * from Teach_Loc";
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
        //public GetDataResult GetLocMst_EmptyLoc(string Equ_No,string Item_Type, out DataObject<LocMst> dataObject, SqlServer db)
        //{
        //    {
        //        string strSql = $"select Top 1 Loc from Loc_Mst where LOC_STS='N' and Equ_No ='{Equ_No}' and Loc_Size='L'";
        //        strSql += $" AND Storage_Type = '{Item_Type}'";
        //        strSql += $" ORDER BY LVL_Z,BAY_Y,ROW_X desc ";

        //        return db.GetData(strSql, out dataObject);
        //    }
        //}
        //public GetDataResult GetLocMst_EmptyLochigh(string Equ_No,string Item_Type, out DataObject<LocMst> dataObject, SqlServer db)
        //{   
        //    {
        //        string strSql = $"select Top 1 Loc from Loc_Mst where LOC_STS='N' and Equ_No ='{Equ_No}' and Loc_Size='H'";
        //        strSql += $" AND Storage_Type = '{Item_Type}'";
        //        strSql += $" ORDER BY LVL_Z,BAY_Y,ROW_X desc ";
                
        //        return db.GetData(strSql, out dataObject);
        //    }
        //}

        public GetDataResult GetLocMst_EmptyLoc(string Equ_No, out DataObject<LocMst> dataObject, SqlServer db)
        {
            {
                string strSql = $"select Top 1 Loc from Loc_Mst where LOC_STS='N' and Equ_No ='{Equ_No}' and Loc_Size='L' ";
                strSql += $" ORDER BY LVL_Z,BAY_Y,ROW_X desc ";

                return db.GetData(strSql, out dataObject);
            }
        }
        public GetDataResult GetLocMst_EmptyLochigh(string Equ_No, out DataObject<LocMst> dataObject, SqlServer db)
        {
            {
                string strSql = $"select Top 1 Loc from Loc_Mst where LOC_STS='N' and Equ_No ='{Equ_No}' and Loc_Size='H' ";
                strSql += $" ORDER BY LVL_Z,BAY_Y,ROW_X desc ";

                return db.GetData(strSql, out dataObject);
            }
        }

        public ExecuteSQLResult UpdateStoreInLocMst(string Loc, SqlServer db)
        {
            string sql = "UPDATE Loc_Mst ";
            sql += $"SET Loc_sts='I', ";
            sql += $"TRN_DATE='{DateTime.Now:yyyy-MM-dd HH:mm:ss}',";
            sql += $"TRN_USER='WCS'";
            sql += $" WHERE LOC='{Loc}' ";
            return db.ExecuteSQL2(sql);
        }

        public ExecuteSQLResult UpdateStoreInLocMstAbnormal(string Loc, SqlServer db)
        {
            string sql = "UPDATE Loc_Mst ";
            sql += $"SET Loc_sts='N', ";
            sql += $"TRN_DATE='{DateTime.Now:yyyy-MM-dd HH:mm:ss}', ";
            sql += $"TRN_USER='WCS' ";
            sql += $" WHERE LOC='{Loc}' ";
            sql += $"AND Loc_Sts='I' ";
            return db.ExecuteSQL2(sql);
        }

        public string GetLoc(string BoxID, SqlServer db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = $"select Loc from Teach_Loc where BoxId = '{BoxID}' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    return Convert.ToString(dtTmp.Rows[0][0]);
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return string.Empty;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public string GetLoc_ForDeposit(int StockerID, SqlServer db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = $"select Loc from Teach_Loc where DeviceID = '{StockerID}' ";
                strSql += $" and LocSts = '{clsEnum.LocSts.N.ToString()}' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    return Convert.ToString(dtTmp.Rows[0][0]);
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return string.Empty;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public string GetLoc_ForDeposit(ref string DeviceID, SqlServer db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strEM = "";
                string strSql = $"select DeviceID, Loc from Teach_Loc where LocSts = '{clsEnum.LocSts.N.ToString()}' ";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success)
                {
                    DeviceID = Convert.ToString(dtTmp.Rows[0]["DeviceID"]);
                    return Convert.ToString(dtTmp.Rows[0]["Loc"]);
                }
                else
                {
                    clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return string.Empty;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public int CheckIsTeach(string DeviceID, string Loc, ref bool IsTeach, SqlServer db)
        {
            DataTable dtTmp = new DataTable();
            try
            {
                string strSql = $"select * from Teach_Loc where DeviceID = '{DeviceID}' and Loc = '{Loc}' ";
                string strEM = "";
                int iRet = db.GetDataTable(strSql, ref dtTmp, ref strEM);
                if (iRet == DBResult.Success) IsTeach = true;
                else if (iRet == DBResult.NoDataSelect) IsTeach = false;
                else clsWriLog.Log.FunWriTraceLog_CV($"{strSql} => {strEM}");

                return iRet;
            }
            catch (Exception ex)
            {
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return DBResult.Exception;
            }
            finally
            {
                dtTmp = null;
            }
        }

        public bool FunUpdLocSts(string sDeviceID, string sLoc, clsEnum.LocSts sts, SqlServer db)
        {
            string strEM = "";
            try
            {
                string strSql = "update Teach_Loc set LocSts = '" + sts.ToString() + "' ";
                strSql += ", OldSts = LocSts ";
                strSql += $", TrnDate = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}' ";
                strSql += " where Loc = '" + sLoc + "' ";
                strSql += $" and DeviceID = '{sDeviceID}' ";
                if (db.ExecuteSQL(strSql, ref strEM) == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSql);
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
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunUpdLocSts(string sDeviceID, string sLoc, clsEnum.LocSts sts, string sBoxID, SqlServer db)
        {
            string strEM = "";
            try
            {
                string strSql = "update Teach_Loc set LocSts = '" + sts.ToString() + "' ";
                strSql += $", BoxId = '{sBoxID}' ";
                strSql += ", OldSts = LocSts ";
                strSql += $", TrnDate = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}' ";
                strSql += " where Loc = '" + sLoc + "' ";
                strSql += $" and DeviceID = '{sDeviceID}' ";
                if (db.ExecuteSQL(strSql, ref strEM) == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSql);
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
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunClearTeachLoc_byBoxID(string sBoxID, SqlServer db)
        {
            string strEM = "";
            try
            {
                string strSql = "update Teach_Loc set LocSts = '" + clsEnum.LocSts.N.ToString() + "' ";
                strSql += $", BoxId = '' ";
                strSql += ", OldSts = LocSts ";
                strSql += $", TrnDate = '{DateTime.Now:yyyy-MM-dd HH:mm:ss}' ";
                strSql += " where BoxId = '" + sBoxID + "' ";
                if (db.ExecuteSQL(strSql, ref strEM) == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSql);
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
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }

        public bool FunInsTeachLoc(string sDeviceID, string sLoc, clsEnum.LocSts sts, string sBoxID, SqlServer db)
        {
            string strEM = "";
            try
            {
                string strSql = "insert into Teach_Loc (DeviceID, Loc, LocSts, BoxId) VALUES";
                strSql += $"('{sDeviceID}','{sLoc}','{sts.ToString()}','{sBoxID}')";
                if (db.ExecuteSQL(strSql, ref strEM) == DBResult.Success)
                {
                    clsWriLog.Log.FunWriTraceLog_CV(strSql);
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
                var cmet = System.Reflection.MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, ex.Message);
                return false;
            }
        }
    }
}
